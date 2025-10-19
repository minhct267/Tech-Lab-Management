using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface IAuthorizationService
{
	bool HasPermission(Permission permission);
	bool CanEnterLab(Lab lab);
	bool CanUseEquipment(Equipment equipment);
	bool CanCreateBooking(Guid? labId, Guid? equipmentId);
}


