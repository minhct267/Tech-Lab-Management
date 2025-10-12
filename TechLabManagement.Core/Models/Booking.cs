using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public enum BookingStatus { Pending, Confirmed, Rejected }

public sealed class Booking : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid? LabId { get; set; }
	public Guid? EquipmentId { get; set; }
	public Guid UserId { get; set; }
	public Guid? TeamId { get; set; }
	public DateTime Start { get; set; }
	public DateTime End { get; set; }
	public string Purpose { get; set; } = string.Empty;
	public BookingStatus Status { get; set; } = BookingStatus.Pending;
}


