namespace TechLabManagement.Core.Interfaces;

public interface INotifier
{
	void Notify(Guid userId, string subject, string body);
}


