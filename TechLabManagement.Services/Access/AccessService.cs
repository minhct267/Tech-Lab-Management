using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;

namespace TechLabManagement.Services.Access;

public sealed class AccessService : IAccessService
{
	private readonly IRepository<AccessRequest> _requests;
	private readonly IRepository<AccessGrant> _grants;
	private readonly IRepository<InductionTest> _tests;
	private readonly IRepository<User> _users;
	private readonly IRepository<Lab> _labs;
	private readonly IInductionEvaluator _evaluator;
	private readonly INotifier _notifier;

	public AccessService(
		IRepository<AccessRequest> requests,
		IRepository<AccessGrant> grants,
		IRepository<InductionTest> tests,
		IRepository<User> users,
		IRepository<Lab> labs,
		IInductionEvaluator evaluator,
		INotifier notifier)
	{
		_requests = requests;
		_grants = grants;
		_tests = tests;
		_users = users;
		_labs = labs;
		_evaluator = evaluator;
		_notifier = notifier;
	}

	public InductionTest? GetInductionTest(Guid labId) => _tests.Query(t => t.LabId == labId).FirstOrDefault();

	public AccessRequest SubmitAccessRequest(Guid userId, Guid labId, Guid? teamId, string reason, IList<int> selectedOptionIndexes)
	{
		var test = GetInductionTest(labId);
		int score = 0;
		if (test != null)
		{
			score = _evaluator.EvaluateScore(test, selectedOptionIndexes);
			if (!_evaluator.IsPass(score, test.PassThresholdPercentage))
			{
				return _requests.Add(new AccessRequest
				{
					UserId = userId,
					LabId = labId,
					TeamId = teamId,
					Reason = reason,
					Status = AccessRequestStatus.Rejected,
					Score = score
				});
			}
		}

		var req = _requests.Add(new AccessRequest
		{
			UserId = userId,
			LabId = labId,
			TeamId = teamId,
			Reason = reason,
			Status = AccessRequestStatus.Pending,
			Score = score
		});

		// notify supervisor / lab manager (mock)
		_notifier.Notify(userId, "Access request submitted", $"Request {req.Id} for lab {labId}");
		return req;
	}

	public bool HasGrant(Guid userId, Guid labId) => _grants.Query(g => g.UserId == userId && g.LabId == labId).Any();

	public IEnumerable<AccessRequest> GetRequestsByStatus(AccessRequestStatus status) => _requests.Query(r => r.Status == status);

	public AccessRequest? Approve(Guid requestId)
	{
		var req = _requests.GetById(requestId);
		if (req == null) return null;
		req.Status = AccessRequestStatus.Approved;
		_requests.Update(req);
		_grants.Add(new AccessGrant { UserId = req.UserId, LabId = req.LabId });
		_notifier.Notify(req.UserId, "Access approved", $"Your access to lab {req.LabId} is approved.");
		return req;
	}

	public AccessRequest? Reject(Guid requestId, string reason)
	{
		var req = _requests.GetById(requestId);
		if (req == null) return null;
		req.Status = AccessRequestStatus.Rejected;
		_requests.Update(req);
		_notifier.Notify(req.UserId, "Access rejected", reason);
		return req;
	}
}


