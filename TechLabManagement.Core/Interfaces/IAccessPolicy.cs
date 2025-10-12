namespace TechLabManagement.Core.Interfaces;

using TechLabManagement.Core.Models;

public interface IAccessPolicy
{
	bool CanEnter(User user, Lab lab, DateTime at);
	bool CanUse(User user, Equipment equipment);
}


