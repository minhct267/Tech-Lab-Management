using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Access;
using TechLabManagement.Services.Induction;
using TechLabManagement.Services.Repositories;

namespace TechLabManagement.Tests;

public sealed class AccessServiceTests
{
    private IRepository<AccessRequest> _requests = null!;
    private IRepository<AccessGrant> _grants = null!;
    private IRepository<InductionTest> _tests = null!;
    private IInductionEvaluator _evaluator = null!;
    private TestNotifier _notifier = null!;
    private IAccessService _service = null!;
    private Guid _userId;
    private Guid _labId;

    [SetUp]
    public void Setup()
    {
        _requests = new InMemoryRepository<AccessRequest>();
        _grants = new InMemoryRepository<AccessGrant>();
        _tests = new InMemoryRepository<InductionTest>();
        _evaluator = new InductionEvaluator();
        _notifier = new TestNotifier();
        _service = new AccessService(_requests, _grants, _tests, _evaluator, _notifier);

        _userId = Guid.NewGuid();
        _labId = Guid.NewGuid();

        _tests.Add(new InductionTest
        {
            LabId = _labId,
            Questions = new()
            {
                new Question { Text = "Q1", Options = new() { "A","B" }, CorrectOptionIndex = 0 },
                new Question { Text = "Q2", Options = new() { "A","B" }, CorrectOptionIndex = 1 },
            },
            PassThresholdPercentage = 80
        });
    }

    [Test]
    public void SubmitAccessRequest_Pass_TransitionsToPending_AndNotifies()
    {
        var req = new AccessRequest { UserId = _userId, LabId = _labId, Reason = "Need access" };

        var saved = _service.SubmitAccessRequest(req, new List<int> { 0, 1 });

        Assert.That(saved.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(saved.Score, Is.EqualTo(100));
        Assert.That(saved.Status, Is.EqualTo(AccessRequestStatus.Pending));
        Assert.That(_notifier.Messages.Any(m => m.userId == _userId && m.subject.Contains("Submitted")), Is.True);
    }

    [Test]
    public void SubmitAccessRequest_Fail_TransitionsToRejected_AndNotifies()
    {
        var req = new AccessRequest { UserId = _userId, LabId = _labId, Reason = "Need access" };

        var saved = _service.SubmitAccessRequest(req, new List<int> { 1, 1 }); // one wrong

        Assert.That(saved.Score, Is.EqualTo(50));
        Assert.That(saved.Status, Is.EqualTo(AccessRequestStatus.Rejected));
        Assert.That(_notifier.Messages.Any(m => m.userId == _userId && m.subject.Contains("Failed")), Is.True);
    }

    [Test]
    public void ApproveAccessRequest_CreatesGrant_AndNotifies()
    {
        var req = new AccessRequest { UserId = _userId, LabId = _labId, Reason = "Need access", Status = AccessRequestStatus.Pending, Score = 100 };
        _requests.Add(req);

        var grant = _service.ApproveAccessRequest(req.Id, Guid.NewGuid());

        Assert.That(grant.UserId, Is.EqualTo(_userId));
        Assert.That(grant.LabId, Is.EqualTo(_labId));
        Assert.That(_notifier.Messages.Any(m => m.userId == _userId && m.subject.Contains("Approved")), Is.True);
    }

    [Test]
    public void RejectAccessRequest_Transitions_AndNotifies()
    {
        var req = new AccessRequest { UserId = _userId, LabId = _labId, Reason = "Need access", Status = AccessRequestStatus.Pending, Score = 100 };
        _requests.Add(req);

        _service.RejectAccessRequest(req.Id, Guid.NewGuid());

        var updated = _requests.GetById(req.Id)!;
        Assert.That(updated.Status, Is.EqualTo(AccessRequestStatus.Rejected));
        Assert.That(_notifier.Messages.Any(m => m.userId == _userId && m.subject.Contains("Rejected")), Is.True);
    }

    private sealed class TestNotifier : INotifier
    {
        public List<(Guid userId, string subject, string body)> Messages { get; } = new();
        public void Notify(Guid userId, string subject, string body)
        {
            Messages.Add((userId, subject, body));
        }
    }
}


