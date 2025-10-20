using System.Collections.ObjectModel;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;

namespace TechLabManagement.ViewModels;

public sealed class DashboardViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public int PendingAccessCount { get; }
	public int BookingsTodayCount { get; }
	public int DueMaintenanceCount { get; }

	public ObservableCollection<LabStat> TopLabStats { get; } = new();
	public ObservableCollection<TodayBookingItem> TodayBookings { get; } = new();

	/* Builds dashboard KPIs and today's activity from in-memory repositories. */
	public DashboardViewModel()
	{
		// Reference date for queries
		var today = DateTime.Today;
		BookingsTodayCount = _svc.Bookings.GetAll().Count(b => b.Start.Date == today);
		PendingAccessCount = _svc.AccessRequests.Query(r => r.Status == AccessRequestStatus.Pending).Count();
		DueMaintenanceCount = 0; // Placeholder for future

		// Build simple 7-day utilization stats per lab
		var to = today.AddDays(7);
		var byLab = _svc.Bookings.GetAll()
			.Where(b => b.Start >= today && b.Start < to && b.LabId != null)
			.GroupBy(b => b.LabId!.Value)
			.Select(g => new { LabId = g.Key, Hours = g.Sum(b => (b.End - b.Start).TotalHours) })
			.OrderByDescending(x => x.Hours)
			.Take(5)
			.ToList();

		var max = byLab.FirstOrDefault()?.Hours ?? 1; // scale bars relative to top lab
		foreach (var item in byLab)
		{
			var lab = _svc.Labs.GetById(item.LabId)!;
			TopLabStats.Add(new LabStat
			{
				Name = lab.Name,
				Hours = Math.Round(item.Hours, 1),
				BarWidth = 10 + 200 * item.Hours / max
			});
		}

		// Today's bookings list
		foreach (var b in _svc.Bookings.GetAll().Where(b => b.Start.Date == today).OrderBy(b => b.Start))
		{
			TodayBookings.Add(new TodayBookingItem
			{
				Start = b.Start.ToString("HH:mm"),
				ResourceName = b.EquipmentId != null ? _svc.Equipment.GetById(b.EquipmentId.Value)!.Name : _svc.Labs.GetById(b.LabId!.Value)!.Name,
				UserName = _svc.Users.GetById(b.UserId)?.Name ?? "Unknown"
			});
		}
	}
}

public sealed class LabStat
{
	public string Name { get; set; } = string.Empty;
	public double Hours { get; set; }
	public double BarWidth { get; set; }
}

public sealed class TodayBookingItem
{
	public string Start { get; set; } = string.Empty;
	public string ResourceName { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
}


