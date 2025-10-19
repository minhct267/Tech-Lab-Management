using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface IAuthService
{
	User? CurrentUser { get; }
	event EventHandler<User?>? CurrentUserChanged;
	bool Login(string username, string password);
	void Logout();
}


