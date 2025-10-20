using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface IAccessService
{
	AccessRequest SubmitAccessRequest(AccessRequest request, IList<int> answers);
	AccessGrant ApproveAccessRequest(Guid requestId, Guid approverId);
	void RejectAccessRequest(Guid requestId, Guid rejectorId);
	bool HasAccess(Guid userId, Guid labId);
	IEnumerable<AccessRequest> GetAccessRequests(AccessRequestStatus? status = null, Guid? labId = null);
}

