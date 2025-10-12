using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public sealed class Team : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public string ProjectName { get; set; } = string.Empty;
	public List<Guid> MemberIds { get; set; } = new();
}


