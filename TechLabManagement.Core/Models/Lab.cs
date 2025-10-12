using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public abstract class Lab : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public string Location { get; set; } = string.Empty;
	public Guid OwnerId { get; set; }
	public Guid TechnicalManagerId { get; set; }
	public Guid AcademicManagerId { get; set; }
	public IAccessPolicy AccessPolicy { get; set; } = new DefaultAccessPolicy();

	public virtual IReadOnlyCollection<string> GetRequiredSafetyTags() => Array.Empty<string>();
}

public sealed class ElectricalLab : Lab
{
	public override IReadOnlyCollection<string> GetRequiredSafetyTags() => new[] { "Electrical", "PPE" };
}

public sealed class RoboticsLab : Lab
{
	public override IReadOnlyCollection<string> GetRequiredSafetyTags() => new[] { "Robotics", "EmergencyStop" };
}

public sealed class MixedRealityLab : Lab
{
	public override IReadOnlyCollection<string> GetRequiredSafetyTags() => new[] { "MR", "Motion" };
}

internal sealed class DefaultAccessPolicy : IAccessPolicy
{
	public bool CanEnter(User user, Lab lab, DateTime at)
	{
		// By default allow Managers/Owner/Admin; others require approval via AccessGrant (handled in services)
		return user.Role is UserRole.TechnicalLabManager or UserRole.AcademicLabManager or UserRole.Professor or UserRole.Admin;
	}

	public bool CanUse(User user, Equipment equipment)
	{
		// By default, require general pass; specific equipment policies can override on Equipment
		return CanEnter(user, equipment.LabRef, DateTime.UtcNow);
	}
}


