using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;

namespace TechLabManagement.Services.Access;

/// <summary>
/// Service for managing access requests and grants
/// </summary>
public sealed class AccessService : IAccessService
{
	private readonly IRepository<AccessRequest> _accessRequests;
	private readonly IRepository<AccessGrant> _accessGrants;
	private readonly IRepository<InductionTest> _inductionTests;
	private readonly IInductionEvaluator _evaluator;
	private readonly INotifier _notifier;

	public AccessService(
		IRepository<AccessRequest> accessRequests,
		IRepository<AccessGrant> accessGrants,
		IRepository<InductionTest> inductionTests,
		IInductionEvaluator evaluator,
		INotifier notifier)
	{
		_accessRequests = accessRequests;
		_accessGrants = accessGrants;
		_inductionTests = inductionTests;
		_evaluator = evaluator;
		_notifier = notifier;
	}

	/// <summary>
	/// Submits an access request after evaluating induction test
	/// </summary>
	public AccessRequest SubmitAccessRequest(AccessRequest request, IList<int> answers)
	{
		// Find the induction test for the lab
		var test = _inductionTests.Query(t => t.LabId == request.LabId).FirstOrDefault();
		if (test == null)
		{
			throw new InvalidOperationException($"No induction test found for lab {request.LabId}");
		}

		// Evaluate the induction test
		var score = _evaluator.EvaluateScore(test, answers);
		var passed = _evaluator.IsPass(score, test.PassThresholdPercentage);

		// Update request with score and status
		request.Score = score;
		request.Status = passed ? AccessRequestStatus.Pending : AccessRequestStatus.Rejected;
		request.SubmittedAt = DateTime.UtcNow;

		// Save the request
		var savedRequest = _accessRequests.Add(request);

		// Send notification
		if (passed)
		{
			_notifier.Notify(request.UserId, "Access Request Submitted", $"Access request submitted for review. Score: {score}%");
		}
		else
		{
			_notifier.Notify(request.UserId, "Induction Test Failed", $"Induction test failed. Score: {score}%, Required: {test.PassThresholdPercentage}%");
		}

		return savedRequest;
	}

	/// <summary>
	/// Approves an access request and creates an access grant
	/// </summary>
	public AccessGrant ApproveAccessRequest(Guid requestId, Guid approverId)
	{
		var request = _accessRequests.GetById(requestId);
		if (request == null)
		{
			throw new InvalidOperationException($"Access request {requestId} not found");
		}

		if (request.Status != AccessRequestStatus.Pending)
		{
			throw new InvalidOperationException($"Cannot approve request with status {request.Status}");
		}

		// Update request status
		request.Status = AccessRequestStatus.Approved;
		_accessRequests.Update(request);

		// Create access grant
		var grant = new AccessGrant
		{
			UserId = request.UserId,
			LabId = request.LabId,
			GrantedAt = DateTime.UtcNow
		};
		var savedGrant = _accessGrants.Add(grant);

		// Send notification
		_notifier.Notify(request.UserId, "Access Request Approved", $"Your access request to lab {request.LabId} has been approved.");

		return savedGrant;
	}

	/// <summary>
	/// Rejects an access request
	/// </summary>
	public void RejectAccessRequest(Guid requestId, Guid rejectorId)
	{
		var request = _accessRequests.GetById(requestId);
		if (request == null)
		{
			throw new InvalidOperationException($"Access request {requestId} not found");
		}

		if (request.Status != AccessRequestStatus.Pending)
		{
			throw new InvalidOperationException($"Cannot reject request with status {request.Status}");
		}

		// Update request status
		request.Status = AccessRequestStatus.Rejected;
		_accessRequests.Update(request);

		// Send notification
		_notifier.Notify(request.UserId, "Access Request Rejected", $"Your access request to lab {request.LabId} has been rejected.");
	}

	/// <summary>
	/// Checks if a user has access to a specific lab
	/// </summary>
	public bool HasAccess(Guid userId, Guid labId)
	{
		return _accessGrants.Query(g => g.UserId == userId && g.LabId == labId).Any();
	}

	/// <summary>
	/// Gets all access requests with optional filtering
	/// </summary>
	public IEnumerable<AccessRequest> GetAccessRequests(AccessRequestStatus? status = null, Guid? labId = null)
	{
		var requests = _accessRequests.GetAll().AsEnumerable();

		if (status.HasValue)
		{
			requests = requests.Where(r => r.Status == status.Value);
		}

		if (labId.HasValue)
		{
			requests = requests.Where(r => r.LabId == labId.Value);
		}

		return requests.OrderByDescending(r => r.SubmittedAt);
	}
}

