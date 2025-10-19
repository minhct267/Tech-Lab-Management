using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Auth;
using TechLabManagement.Services.Repositories;
using TechLabManagement.Services.Security;

namespace TechLabManagement.Tests;

public sealed class AuthTests
{
    private IRepository<User> _users = null!;
    private IAuthService _auth = null!;

    [SetUp]
    public void Setup()
    {
        _users = new InMemoryRepository<User>();
        PasswordHasher.CreateHash("secret", out var salt, out var hash);
        _users.Add(new User { Username = "u", Name = "U", Email = "u@x", PasswordHash = hash, PasswordSalt = salt, Role = UserRole.Student });
        _auth = new AuthService(_users);
    }

    [Test]
    public void Login_Success_SetsCurrentUser()
    {
        var ok = _auth.Login("u", "secret");
        Assert.That(ok, Is.True);
        Assert.That(_auth.CurrentUser, Is.Not.Null);
        Assert.That(_auth.CurrentUser!.Username, Is.EqualTo("u"));
    }

    [Test]
    public void Login_Failure_DoesNotSetUser()
    {
        var ok = _auth.Login("u", "wrong");
        Assert.That(ok, Is.False);
        Assert.That(_auth.CurrentUser, Is.Null);
    }
}


