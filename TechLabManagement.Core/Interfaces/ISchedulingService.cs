using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface ISchedulingService
{
	bool HasConflict(Booking candidate);
	(bool ok, string? error) TryCreateBooking(Booking candidate);
	IEnumerable<Booking> GetBookingsForResource(Guid? labId, Guid? equipmentId, DateTime from, DateTime to);
	IEnumerable<Booking> GetConflicts(Guid? labId, Guid? equipmentId, DateTime start, DateTime end);
}


