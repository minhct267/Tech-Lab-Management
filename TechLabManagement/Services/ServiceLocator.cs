using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Access;
using TechLabManagement.Services.Induction;
using TechLabManagement.Services.Notifications;
using TechLabManagement.Services.Repositories;
using TechLabManagement.Services.Scheduling;
using TechLabManagement.Services.Seed;
using TechLabManagement.Services.Auth;
using TechLabManagement.Services.Analytics;

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
	public IAuthService Auth { get; }
	public IAuthorizationService Authorization { get; }
	public IAnalyticsService Analytics { get; }

	/// <summary>
	/// Current logged-in user (null until login) - provided by Auth service
	/// </summary>
	public User CurrentUser => Auth.CurrentUser!;

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
		Auth = new AuthService(Users);
		Authorization = new AuthorizationService(Auth, AccessGrants, Labs, Equipment);
		Analytics = new AnalyticsService(Users, Labs, Equipment, Bookings, AccessRequests, InductionTests);

		// No default user; require login via Auth
	}
}


