using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Auth;
using TechLabManagement.Services.Repositories;
using TechLabManagement.Services.Security;

namespace TechLabManagement.Tests;

public sealed class AuthorizationTests
{
    private IRepository<User> _users = null!;
    private IRepository<Lab> _labs = null!;
    private IRepository<Equipment> _eq = null!;
    private IRepository<AccessGrant> _grants = null!;
    private IAuthService _auth = null!;
    private IAuthorizationService _z = null!;

    [SetUp]
    public void Setup()
    {
        _users = new InMemoryRepository<User>();
        _labs = new InMemoryRepository<Lab>();
        _eq = new InMemoryRepository<Equipment>();
        _grants = new InMemoryRepository<AccessGrant>();

        PasswordHasher.CreateHash("pass", out var salt, out var hash);
        var student = _users.Add(new User { Username = "s", Name = "Student", Role = UserRole.Student, PasswordHash = hash, PasswordSalt = salt });
        _auth = new AuthService(_users);
        _auth.Login("s", "pass");

        var lab = _labs.Add(new ElectricalLab { Name = "Electrical", OwnerId = Guid.NewGuid(), TechnicalManagerId = Guid.NewGuid(), AcademicManagerId = Guid.NewGuid() });
        var solder = _eq.Add(new SolderingStation { Name = "Solder", LabId = lab.Id });
        solder.LabRef = lab;

        _z = new AuthorizationService(_auth, _grants, _labs, _eq);
    }

    [Test]
    public void CanEnterLab_WithoutGrant_ReturnsFalse()
    {
        var lab = _labs.GetAll().First();
        Assert.That(_z.CanEnterLab(lab), Is.False);
    }

    [Test]
    public void CanEnterLab_WithGrant_ReturnsTrueIfPolicyAllows()
    {
        var user = _auth.CurrentUser!;
        var lab = _labs.GetAll().First();
        _grants.Add(new AccessGrant { UserId = user.Id, LabId = lab.Id });
        Assert.That(_z.CanEnterLab(lab), Is.True);
    }
}


