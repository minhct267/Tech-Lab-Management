using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
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

	public IInductionEvaluator InductionEvaluator { get; }
	public ISchedulingService SchedulingService { get; }
	public INotifier Notifier { get; }

	private ServiceLocator()
	{
		Users = new InMemoryRepository<User>();
		Teams = new InMemoryRepository<Team>();
		Labs = new InMemoryRepository<Lab>();
		Equipment = new InMemoryRepository<Equipment>();
		InductionTests = new InMemoryRepository<InductionTest>();
		Bookings = new InMemoryRepository<Booking>();

		SampleDataSeeder.Seed(Users, Teams, Labs, Equipment, InductionTests);

		InductionEvaluator = new InductionEvaluator();
		SchedulingService = new SchedulingService(Bookings);
		Notifier = new ConsoleNotifier();
	}
}


