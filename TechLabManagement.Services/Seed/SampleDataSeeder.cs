using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;
using TechLabManagement.Services.Policies;

namespace TechLabManagement.Services.Seed;

public static class SampleDataSeeder
{
	public static void Seed(
		IRepository<User> users,
		IRepository<Team> teams,
		IRepository<Lab> labs,
		IRepository<Equipment> equipment,
		IRepository<InductionTest> tests)
	{
		// Users
		var admin = users.Add(new User { Name = "Admin", Email = "admin@uni.local", Role = UserRole.Admin });
		var prof = users.Add(new User { Name = "Prof. Smith", Email = "smith@uni.local", Role = UserRole.Professor });
		var techMgr = users.Add(new User { Name = "Alex TechMgr", Email = "alex@uni.local", Role = UserRole.TechnicalLabManager });
		var sup = users.Add(new User { Name = "Dr. Lee", Email = "lee@uni.local", Role = UserRole.Supervisor });
		var alice = users.Add(new User { Name = "Alice", Email = "alice@uni.local", Role = UserRole.Researcher, SupervisorId = sup.Id });
		var bob = users.Add(new User { Name = "Bob", Email = "bob@uni.local", Role = UserRole.Student, SupervisorId = sup.Id });

		// Teams
		var teamA = teams.Add(new Team { Name = "Team A", ProjectName = "Project A", MemberIds = new() { alice.Id, bob.Id } });
		var teamB = teams.Add(new Team { Name = "Team B", ProjectName = "Project B", MemberIds = new() { alice.Id } });

		// Labs
		var electrical = labs.Add(new ElectricalLab
		{
			Name = "Electrical Lab",
			Location = "B1-101",
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id,
			AccessPolicy = new ElectricalAccessPolicy()
		});
		var robotics = labs.Add(new RoboticsLab
		{
			Name = "Robotics Lab",
			Location = "B2-201",
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id,
			AccessPolicy = new RoboticsAccessPolicy()
		});
		var mr = labs.Add(new MixedRealityLab
		{
			Name = "Mixed Reality Lab",
			Location = "B3-301",
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id,
			AccessPolicy = new MixedRealityAccessPolicy()
		});

		// Equipment
		var solder = equipment.Add(new SolderingStation { Name = "Soldering Station", LabId = electrical.Id, SafetyTags = new() { "Electrical", "PPE" } });
		var robotArm = equipment.Add(new RobotArm { Name = "UR10 Robot Arm", LabId = robotics.Id, SafetyTags = new() { "EmergencyStop" } });
		var motion = equipment.Add(new MotionPlatform { Name = "Motion Platform", LabId = mr.Id, SafetyTags = new() { "Motion" } });

		// Wire LabRef
		foreach (var eq in equipment.GetAll())
		{
			eq.LabRef = labs.GetById(eq.LabId)!;
		}

		// Induction tests
		tests.Add(new InductionTest
		{
			LabId = electrical.Id,
			Questions = new()
			{
				new Question { Text = "Wear PPE in Electrical Lab?", Options = new() { "No", "Yes" }, CorrectOptionIndex = 1 },
				new Question { Text = "Use ESD protection when soldering?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
			}
		});

		tests.Add(new InductionTest
		{
			LabId = robotics.Id,
			Questions = new()
			{
				new Question { Text = "Know emergency stop location?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
			}
		});
	}
}


