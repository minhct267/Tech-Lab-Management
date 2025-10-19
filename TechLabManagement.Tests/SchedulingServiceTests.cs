using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Repositories;
using TechLabManagement.Services.Scheduling;

namespace TechLabManagement.Tests;

public sealed class SchedulingServiceTests
{
    private IRepository<Booking> _repo = null!;
    private ISchedulingService _svc = null!;
    private Guid _labId;
    private Guid _equipId;

    [SetUp]
    public void Setup()
    {
        _repo = new InMemoryRepository<Booking>();
        _svc = new SchedulingService(_repo);
        _labId = Guid.NewGuid();
        _equipId = Guid.NewGuid();
    }

    [Test]
    public void HasConflict_ReturnsTrue_WhenTimeOverlapsSameResource()
    {
        var start = DateTime.Today.AddHours(9);
        var end = DateTime.Today.AddHours(11);
        _repo.Add(new Booking { LabId = _labId, Start = start, End = end, UserId = Guid.NewGuid(), Status = BookingStatus.Confirmed });

        var candidate = new Booking { LabId = _labId, Start = DateTime.Today.AddHours(10), End = DateTime.Today.AddHours(12), UserId = Guid.NewGuid() };

        Assert.That(_svc.HasConflict(candidate), Is.True);
    }

    [Test]
    public void HasConflict_ReturnsFalse_WhenDifferentResource()
    {
        var start = DateTime.Today.AddHours(9);
        var end = DateTime.Today.AddHours(11);
        _repo.Add(new Booking { EquipmentId = _equipId, Start = start, End = end, UserId = Guid.NewGuid(), Status = BookingStatus.Confirmed });

        var candidate = new Booking { LabId = _labId, Start = start, End = end, UserId = Guid.NewGuid() };

        Assert.That(_svc.HasConflict(candidate), Is.False);
    }

    [Test]
    public void TryCreateBooking_Fails_WhenEndBeforeStart()
    {
        var (ok, error) = _svc.TryCreateBooking(new Booking { LabId = _labId, Start = DateTime.Today.AddHours(11), End = DateTime.Today.AddHours(10), UserId = Guid.NewGuid() });
        Assert.That(ok, Is.False);
        Assert.That(error, Does.Contain("End must be after Start"));
    }

    [Test]
    public void TryCreateBooking_Fails_WhenNoResource()
    {
        var (ok, error) = _svc.TryCreateBooking(new Booking { Start = DateTime.Today.AddHours(9), End = DateTime.Today.AddHours(10), UserId = Guid.NewGuid() });
        Assert.That(ok, Is.False);
        Assert.That(error, Does.Contain("Select a lab or equipment"));
    }

    [Test]
    public void TryCreateBooking_Succeeds_WhenNoConflict()
    {
        var (ok, error) = _svc.TryCreateBooking(new Booking { LabId = _labId, Start = DateTime.Today.AddHours(9), End = DateTime.Today.AddHours(10), UserId = Guid.NewGuid() });
        Assert.That(ok, Is.True);
        Assert.That(error, Is.Null);
    }

    [Test]
    public void GetConflicts_ReturnsOverlappingBookings_ForSpecificResource()
    {
        var a = _repo.Add(new Booking { LabId = _labId, Start = DateTime.Today.AddHours(9), End = DateTime.Today.AddHours(11), UserId = Guid.NewGuid(), Status = BookingStatus.Confirmed });
        var b = _repo.Add(new Booking { LabId = _labId, Start = DateTime.Today.AddHours(12), End = DateTime.Today.AddHours(13), UserId = Guid.NewGuid(), Status = BookingStatus.Confirmed });

        var conflicts = _svc.GetConflicts(_labId, null, DateTime.Today.AddHours(10), DateTime.Today.AddHours(12)).ToList();
        Assert.That(conflicts.Count, Is.EqualTo(1));
        Assert.That(conflicts[0].Id, Is.EqualTo(a.Id));
    }
}


