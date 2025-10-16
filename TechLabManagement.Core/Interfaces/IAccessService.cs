using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

/// <summary>
/// Service for managing access requests and grants
/// </summary>
public interface IAccessService
{
	/// <summary>
	/// Submits an access request after evaluating induction test
	/// </summary>
	/// <param name="request">The access request to submit</param>
	/// <param name="answers">User's answers to induction test questions</param>
	/// <returns>The submitted access request with score and status</returns>
	AccessRequest SubmitAccessRequest(AccessRequest request, IList<int> answers);

	/// <summary>
	/// Approves an access request and creates an access grant
	/// </summary>
	/// <param name="requestId">The ID of the request to approve</param>
	/// <param name="approverId">The ID of the user approving the request</param>
	/// <returns>The created access grant</returns>
	AccessGrant ApproveAccessRequest(Guid requestId, Guid approverId);

	/// <summary>
	/// Rejects an access request
	/// </summary>
	/// <param name="requestId">The ID of the request to reject</param>
	/// <param name="rejectorId">The ID of the user rejecting the request</param>
	void RejectAccessRequest(Guid requestId, Guid rejectorId);

	/// <summary>
	/// Checks if a user has access to a specific lab
	/// </summary>
	/// <param name="userId">The user ID</param>
	/// <param name="labId">The lab ID</param>
	/// <returns>True if user has access, false otherwise</returns>
	bool HasAccess(Guid userId, Guid labId);

	/// <summary>
	/// Gets all access requests with optional filtering
	/// </summary>
	/// <param name="status">Optional status filter</param>
	/// <param name="labId">Optional lab filter</param>
	/// <returns>Filtered list of access requests</returns>
	IEnumerable<AccessRequest> GetAccessRequests(AccessRequestStatus? status = null, Guid? labId = null);
}

