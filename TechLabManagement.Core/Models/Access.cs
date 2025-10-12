using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Core.Models;

public sealed class AccessRequest : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid UserId { get; set; }
	public Guid LabId { get; set; }
	public Guid? TeamId { get; set; }
	public string Reason { get; set; } = string.Empty;
	public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
	public AccessRequestStatus Status { get; set; } = AccessRequestStatus.Draft;
	public int? Score { get; set; }
}

public enum AccessRequestStatus
{
	Draft,
	Pending,
	Approved,
	Rejected
}

public sealed class AccessGrant : IEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid UserId { get; set; }
	public Guid LabId { get; set; }
	public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
}


