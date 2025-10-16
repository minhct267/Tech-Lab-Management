using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;

namespace TechLabManagement.Services.Scheduling;

public sealed class SchedulingService : ISchedulingService
{
	private readonly IRepository<Booking> _bookingRepo;

	public SchedulingService(IRepository<Booking> bookingRepo)
	{
		_bookingRepo = bookingRepo;
	}

	public IEnumerable<Booking> GetBookingsForResource(Guid? labId, Guid? equipmentId, DateTime from, DateTime to)
	{
		return _bookingRepo.Query(b =>
			(b.LabId == labId || labId == null) &&
			(b.EquipmentId == equipmentId || equipmentId == null) &&
			b.End > from && b.Start < to);
	}

	public bool HasConflict(Booking candidate)
	{
		var overlaps = _bookingRepo.Query(b =>
			b.Id != candidate.Id &&
			b.Status != BookingStatus.Rejected &&
			b.LabId == candidate.LabId &&
			b.EquipmentId == candidate.EquipmentId &&
			b.End > candidate.Start && b.Start < candidate.End);
		return overlaps.Any();
	}

	public (bool ok, string? error) TryCreateBooking(Booking candidate)
	{
		if (candidate.Start >= candidate.End)
			return (false, "End must be after Start");
		if (candidate.LabId is null && candidate.EquipmentId is null)
			return (false, "Select a lab or equipment");
		if (HasConflict(candidate))
			return (false, "Time slot conflicts with an existing booking");
		_bookingRepo.Add(candidate);
		candidate.Status = BookingStatus.Confirmed;
		return (true, null);
	}

	public IEnumerable<Booking> GetConflicts(Guid? labId, Guid? equipmentId, DateTime start, DateTime end)
	{
		return _bookingRepo.Query(b =>
			b.Status != BookingStatus.Rejected &&
			((labId.HasValue && b.LabId == labId.Value) || (equipmentId.HasValue && b.EquipmentId == equipmentId.Value)) &&
			b.End > start && b.Start < end);
	}
}


