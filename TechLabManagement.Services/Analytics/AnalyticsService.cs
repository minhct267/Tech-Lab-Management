using Microsoft.Data.Analysis;
using System.Globalization;
using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;

namespace TechLabManagement.Services.Analytics;

public sealed class AnalyticsService : IAnalyticsService
{
	private readonly IRepository<User> _users;
	private readonly IRepository<Lab> _labs;
	private readonly IRepository<Equipment> _equipment;
	private readonly IRepository<Booking> _bookings;
	private readonly IRepository<AccessRequest> _accessRequests;
	private readonly IRepository<InductionTest> _tests;

	public AnalyticsService(
		IRepository<User> users,
		IRepository<Lab> labs,
		IRepository<Equipment> equipment,
		IRepository<Booking> bookings,
		IRepository<AccessRequest> accessRequests,
		IRepository<InductionTest> tests)
	{
		_users = users;
		_labs = labs;
		_equipment = equipment;
		_bookings = bookings;
		_accessRequests = accessRequests;
		_tests = tests;
	}

	public IEnumerable<BookingVolumeByRoleHour> GetBookingVolumeByRoleAndHour(DateTime from, DateTime to)
	{
		var users = _users.GetAll().ToDictionary(u => u.Id, u => u.Role.ToString());
		var rows = _bookings.GetAll()
			.Where(b => b.Start < to && b.End > from)
			.Select(b => new
			{
				Role = users.TryGetValue(b.UserId, out var r) ? r : "Unknown",
				Hour = b.Start.Hour
			});
		return rows
			.GroupBy(x => new { x.Role, x.Hour })
			.Select(g => new BookingVolumeByRoleHour(g.Key.Role, g.Key.Hour, g.Count()))
			.OrderBy(x => x.Role).ThenBy(x => x.Hour);
	}

	public IEnumerable<ApprovalRateByRole> GetAccessApprovalRateByRole(DateTime from, DateTime to)
	{
		var users = _users.GetAll().ToDictionary(u => u.Id, u => u.Role.ToString());
		var rows = _accessRequests.GetAll()
			.Where(r => r.SubmittedAt >= from && r.SubmittedAt <= to)
			.Select(r => new { Role = users.TryGetValue(r.UserId, out var role) ? role : "Unknown", r.Status });
		return rows
			.GroupBy(x => x.Role)
			.Select(g => new ApprovalRateByRole(
				g.Key,
				g.Count(x => x.Status == AccessRequestStatus.Approved),
				g.Count()))
			.OrderByDescending(x => x.Total);
	}

	public IEnumerable<AverageInductionScoreByLab> GetAverageInductionScoreByLab()
	{
		var labNames = _labs.GetAll().ToDictionary(l => l.Id, l => l.Name);
		var rows = _accessRequests.GetAll()
			.Where(r => r.Score.HasValue)
			.Select(r => new { r.LabId, Score = r.Score!.Value });
		return rows
			.GroupBy(x => x.LabId)
			.Select(g => new AverageInductionScoreByLab(
				g.Key,
				labNames.TryGetValue(g.Key, out var name) ? name : "Unknown",
				g.Average(x => x.Score)))
			.OrderByDescending(x => x.AverageScore);
	}

	public IEnumerable<EquipmentBookedHoursByWeek> GetEquipmentBookedHoursByWeek(DateTime from, DateTime to)
	{
		var equipmentNames = _equipment.GetAll().ToDictionary(e => e.Id, e => e.Name);
		static int GetIsoWeek(DateTime dt)
		{
			var cal = CultureInfo.InvariantCulture.Calendar;
			return cal.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

		var rows = _bookings.GetAll()
			.Where(b => b.EquipmentId.HasValue && b.End > from && b.Start < to)
			.Select(b => new
			{
				EquipmentId = b.EquipmentId!.Value,
				IsoWeek = GetIsoWeek(b.Start),
				Hours = (b.End - b.Start).TotalHours
			});

		return rows
			.GroupBy(x => new { x.EquipmentId, x.IsoWeek })
			.Select(g => new EquipmentBookedHoursByWeek(
				g.Key.EquipmentId,
				equipmentNames.TryGetValue(g.Key.EquipmentId, out var name) ? name : "Unknown",
				g.Key.IsoWeek,
				g.Sum(x => x.Hours)))
			.OrderBy(x => x.EquipmentId).ThenBy(x => x.IsoWeek);
	}
}


