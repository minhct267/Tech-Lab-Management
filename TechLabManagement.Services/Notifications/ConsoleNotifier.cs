using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Services.Notifications;

public sealed class ConsoleNotifier : INotifier
{
	public void Notify(Guid userId, string subject, string body)
	{
		System.Diagnostics.Debug.WriteLine($"[Notify] To:{userId} {subject} - {body}");
	}
}


