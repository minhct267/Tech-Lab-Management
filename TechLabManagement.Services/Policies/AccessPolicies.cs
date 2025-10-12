using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;

namespace TechLabManagement.Services.Policies;

public sealed class ElectricalAccessPolicy : IAccessPolicy
{
	public bool CanEnter(User user, Lab lab, DateTime at)
	{
		if (user.Role is UserRole.Admin or UserRole.Professor or UserRole.TechnicalLabManager) return true;
		// Researchers and Students need approved grant handled by AccessService (outside policy)
		return false;
	}

	public bool CanUse(User user, Equipment equipment)
	{
		// Soldering requires supervision; allow only if supervisor role present
		return user.Role is UserRole.TechnicalLabManager or UserRole.Professor or UserRole.Supervisor;
	}
}

public sealed class RoboticsAccessPolicy : IAccessPolicy
{
	public bool CanEnter(User user, Lab lab, DateTime at) =>
		user.Role is UserRole.Admin or UserRole.Professor or UserRole.TechnicalLabManager;

	public bool CanUse(User user, Equipment equipment)
	{
		return user.Role is UserRole.TechnicalLabManager or UserRole.Professor || equipment.RequiresSupervisor();
	}
}

public sealed class MixedRealityAccessPolicy : IAccessPolicy
{
	public bool CanEnter(User user, Lab lab, DateTime at) =>
		user.Role is UserRole.Admin or UserRole.Professor or UserRole.TechnicalLabManager;

	public bool CanUse(User user, Equipment equipment)
	{
		// Motion platform strict usage
		return user.Role is UserRole.TechnicalLabManager or UserRole.Professor;
	}
}


