using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Security;

namespace TechLabManagement.Services.Auth;

public sealed class AuthService : IAuthService
{
	private readonly IRepository<User> _users;
	private User? _currentUser;

	public AuthService(IRepository<User> users)
	{
		_users = users;
	}

	public User? CurrentUser => _currentUser;
	public event EventHandler<User?>? CurrentUserChanged;

	public bool Login(string username, string password)
	{
		var user = _users.Query(u => u.Username == username).FirstOrDefault();
		if (user == null || user.PasswordHash == null || user.PasswordSalt == null) return false;
		if (!PasswordHasher.Verify(password, user.PasswordSalt, user.PasswordHash)) return false;
		_currentUser = user;
		CurrentUserChanged?.Invoke(this, _currentUser);
		return true;
	}

	public void Logout()
	{
		_currentUser = null;
		CurrentUserChanged?.Invoke(this, _currentUser);
	}
}


