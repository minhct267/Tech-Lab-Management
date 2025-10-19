using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Induction;
using TechLabManagement.Services.Notifications;
using TechLabManagement.Services.Repositories;
using TechLabManagement.Services.Scheduling;
using TechLabManagement.Services.Seed;
using TechLabManagement.Services.Access;

namespace TechLabManagement.Services;

public sealed class ServiceLocator
{
	private static ServiceLocator? _current;
	public static ServiceLocator Current => _current ??= new ServiceLocator();

	public IRepository<User> Users { get; }
	public IRepository<Team> Teams { get; }
	public IRepository<Lab> Labs { get; }
	public IRepository<Equipment> Equipment { get; }
	public IRepository<InductionTest> InductionTests { get; }
	public IRepository<Booking> Bookings { get; }
	public IRepository<AccessRequest> AccessRequests { get; }
	public IRepository<AccessGrant> AccessGrants { get; }
	public IRepository<MaintenanceTask> MaintenanceTasks { get; }

	public IInductionEvaluator InductionEvaluator { get; }
	public ISchedulingService SchedulingService { get; }
	public INotifier Notifier { get; }
	public IAccessService AccessService { get; }

	private ServiceLocator()
	{
		Users = new InMemoryRepository<User>();
		Teams = new InMemoryRepository<Team>();
		Labs = new InMemoryRepository<Lab>();
		Equipment = new InMemoryRepository<Equipment>();
		InductionTests = new InMemoryRepository<InductionTest>();
		Bookings = new InMemoryRepository<Booking>();
		AccessRequests = new InMemoryRepository<AccessRequest>();
		AccessGrants = new InMemoryRepository<AccessGrant>();
		MaintenanceTasks = new InMemoryRepository<MaintenanceTask>();

		SampleDataSeeder.Seed(Users, Teams, Labs, Equipment, InductionTests, MaintenanceTasks);

		InductionEvaluator = new InductionEvaluator();
		SchedulingService = new SchedulingService(Bookings);
		Notifier = new ConsoleNotifier();
		AccessService = new AccessService(AccessRequests, AccessGrants, InductionTests, Users, Labs, InductionEvaluator, Notifier);
	}
}


