using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Access;
using TechLabManagement.Services.Induction;
using TechLabManagement.Services.Notifications;
using TechLabManagement.Services.Repositories;
using TechLabManagement.Services.Scheduling;
using TechLabManagement.Services.Seed;

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

	public IInductionEvaluator InductionEvaluator { get; }
	public ISchedulingService SchedulingService { get; }
	public INotifier Notifier { get; }
	public IAccessService AccessService { get; }

	/// <summary>
	/// Current logged-in user (for demo purposes, set to Alice)
	/// </summary>
	public User CurrentUser { get; set; } = null!;

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

		SampleDataSeeder.Seed(Users, Teams, Labs, Equipment, InductionTests, Bookings, AccessRequests);

		InductionEvaluator = new InductionEvaluator();
		SchedulingService = new SchedulingService(Bookings);
		Notifier = new ConsoleNotifier();
		AccessService = new AccessService(AccessRequests, AccessGrants, InductionTests, InductionEvaluator, Notifier);

		// Set current user to Alice for demo purposes
		CurrentUser = Users.Query(u => u.Name == "Alice").FirstOrDefault() ?? Users.GetAll().First();
	}
}


