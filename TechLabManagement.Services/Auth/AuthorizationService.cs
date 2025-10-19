using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;

namespace TechLabManagement.Services.Auth;

public sealed class AuthorizationService : IAuthorizationService
{
	private readonly IAuthService _auth;
	private readonly IRepository<AccessGrant> _grants;
	private readonly IRepository<Lab> _labs;
	private readonly IRepository<Equipment> _equipment;

	public AuthorizationService(IAuthService auth, IRepository<AccessGrant> grants, IRepository<Lab> labs, IRepository<Equipment> equipment)
	{
		_auth = auth;
		_grants = grants;
		_labs = labs;
		_equipment = equipment;
	}

	public bool HasPermission(Permission permission)
	{
		var user = _auth.CurrentUser;
		if (user == null) return false;
		return permission switch
		{
			Permission.Login => true,
			Permission.Logout => true,
			Permission.ViewLabs => true,
			Permission.SubmitAccessRequest => user.Role is UserRole.Student or UserRole.Researcher or UserRole.Staff or UserRole.Supervisor or UserRole.TechnicalLabManager or UserRole.AcademicLabManager or UserRole.Professor or UserRole.Admin,
			Permission.CreateBooking => true,
			Permission.ViewAllBookings => user.Role is UserRole.TechnicalLabManager or UserRole.AcademicLabManager or UserRole.Professor or UserRole.Admin,
			Permission.ApproveBooking => user.Role is UserRole.TechnicalLabManager or UserRole.Professor or UserRole.Admin,
			Permission.ApproveAccessRequest => user.Role is UserRole.TechnicalLabManager or UserRole.Professor or UserRole.Admin,
			Permission.RejectAccessRequest => user.Role is UserRole.TechnicalLabManager or UserRole.Professor or UserRole.Admin,
			Permission.ManageEquipment => user.Role is UserRole.TechnicalLabManager or UserRole.Admin,
			Permission.ManageLabs => user.Role is UserRole.Professor or UserRole.Admin,
			Permission.ManageUsers => user.Role is UserRole.Admin,
			_ => false
		};
	}

	public bool CanEnterLab(Lab lab)
	{
		var user = _auth.CurrentUser;
		if (user == null) return false;
        if (user.Role is UserRole.TechnicalLabManager or UserRole.AcademicLabManager or UserRole.Professor or UserRole.Admin) return true;
        var hasGrant = _grants.Query(g => g.UserId == user.Id && g.LabId == lab.Id).Any();
        // Grant is the authoritative permission for non-privileged users; policy can add extra checks if desired.
        return hasGrant;
	}

	public bool CanUseEquipment(Equipment equipment)
	{
		var user = _auth.CurrentUser;
		if (user == null) return false;
        if (user.Role is UserRole.TechnicalLabManager or UserRole.Professor or UserRole.Admin) return true;
        var hasGrant = _grants.Query(g => g.UserId == user.Id && g.LabId == equipment.LabId).Any();
        // For simplicity, grant on the lab allows equipment use unless equipment.RequiresSupervisor is true and user is not supervisor/manager
        if (!hasGrant) return false;
        if (equipment.RequiresSupervisor())
        {
            return user.Role is UserRole.Supervisor or UserRole.TechnicalLabManager or UserRole.Professor or UserRole.Admin;
        }
        return true;
	}

	public bool CanCreateBooking(Guid? labId, Guid? equipmentId)
	{
		var user = _auth.CurrentUser;
		if (user == null) return false;
		if (equipmentId.HasValue)
		{
			var eq = _equipment.GetById(equipmentId.Value);
			if (eq == null) return false;
			return CanUseEquipment(eq);
		}
		if (labId.HasValue)
		{
			var lab = _labs.GetById(labId.Value);
			if (lab == null) return false;
			return CanEnterLab(lab);
		}
		return false;
	}
}


