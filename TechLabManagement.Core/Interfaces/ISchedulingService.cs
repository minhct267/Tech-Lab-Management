using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface ISchedulingService
{
	bool HasConflict(Booking candidate);
	(bool ok, string? error) TryCreateBooking(Booking candidate);
	IEnumerable<Booking> GetBookingsForResource(Guid? labId, Guid? equipmentId, DateTime from, DateTime to);
	
	/// <summary>
	/// Get all bookings that conflict with the specified time slot for a resource
	/// </summary>
	/// <param name="labId">Optional lab ID to check</param>
	/// <param name="equipmentId">Optional equipment ID to check</param>
	/// <param name="start">Start time of the time slot</param>
	/// <param name="end">End time of the time slot</param>
	/// <returns>List of conflicting bookings</returns>
	IEnumerable<Booking> GetConflicts(Guid? labId, Guid? equipmentId, DateTime start, DateTime end);
}


