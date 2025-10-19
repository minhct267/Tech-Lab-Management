using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public enum MaintenanceStatus { Open, Completed }

public sealed class MaintenanceTask : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid EquipmentId { get; set; }
	public DateTime DueDate { get; set; }
	public string Type { get; set; } = string.Empty;
	public string Notes { get; set; } = string.Empty;
	public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Open;
}


