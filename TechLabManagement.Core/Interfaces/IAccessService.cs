using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface IAccessService
{
	InductionTest? GetInductionTest(Guid labId);
	AccessRequest SubmitAccessRequest(Guid userId, Guid labId, Guid? teamId, string reason, IList<int> selectedOptionIndexes);
	bool HasGrant(Guid userId, Guid labId);
	IEnumerable<AccessRequest> GetRequestsByStatus(AccessRequestStatus status);
	AccessRequest? Approve(Guid requestId);
	AccessRequest? Reject(Guid requestId, string reason);
}


