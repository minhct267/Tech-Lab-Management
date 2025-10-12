using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public sealed class User : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public UserRole Role { get; set; }
	public Guid? SupervisorId { get; set; }
}


