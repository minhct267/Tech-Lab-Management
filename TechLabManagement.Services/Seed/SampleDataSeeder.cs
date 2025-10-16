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
		IRepository<InductionTest> tests,
		IRepository<Booking>? bookings = null,
		IRepository<AccessRequest>? accessRequests = null)
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

		tests.Add(new InductionTest
		{
			LabId = mr.Id,
			Questions = new()
			{
				new Question { Text = "Understand motion sickness risks?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Secure loose items before platform use?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
			}
		});

		// Sample bookings
		if (bookings != null)
		{
			var today = DateTime.Today;
			
			// Today's bookings
			bookings.Add(new Booking
			{
				UserId = alice.Id,
				TeamId = teamA.Id,
				LabId = electrical.Id,
				Start = today.AddHours(9),
				End = today.AddHours(11),
				Purpose = "PCB soldering for Project A prototype",
				Status = BookingStatus.Confirmed
			});

			bookings.Add(new Booking
			{
				UserId = bob.Id,
				TeamId = teamA.Id,
				EquipmentId = robotArm.Id,
				Start = today.AddHours(14),
				End = today.AddHours(16),
				Purpose = "Robot arm calibration tests",
				Status = BookingStatus.Confirmed
			});

			// Future bookings
			bookings.Add(new Booking
			{
				UserId = alice.Id,
				TeamId = teamB.Id,
				LabId = mr.Id,
				Start = today.AddDays(1).AddHours(10),
				End = today.AddDays(1).AddHours(12),
				Purpose = "VR simulation testing",
				Status = BookingStatus.Confirmed
			});

			bookings.Add(new Booking
			{
				UserId = bob.Id,
				EquipmentId = motion.Id,
				Start = today.AddDays(2).AddHours(13),
				End = today.AddDays(2).AddHours(15),
				Purpose = "Motion platform experiments",
				Status = BookingStatus.Pending
			});

			// More bookings for stats
			for (int i = 0; i < 5; i++)
			{
				bookings.Add(new Booking
				{
					UserId = alice.Id,
					LabId = robotics.Id,
					Start = today.AddDays(i).AddHours(9 + i),
					End = today.AddDays(i).AddHours(11 + i),
					Purpose = $"Robotics research session {i + 1}",
					Status = BookingStatus.Confirmed
				});
			}
		}

		// Sample access requests
		if (accessRequests != null)
		{
			// Pending request from Bob
			accessRequests.Add(new AccessRequest
			{
				UserId = bob.Id,
				LabId = mr.Id,
				TeamId = teamA.Id,
				Reason = "Need access to Mixed Reality Lab for final year project on VR interaction.",
				SubmittedAt = DateTime.UtcNow.AddDays(-1),
				Status = AccessRequestStatus.Pending,
				Score = 100
			});

			// Another pending request
			accessRequests.Add(new AccessRequest
			{
				UserId = alice.Id,
				LabId = robotics.Id,
				TeamId = teamB.Id,
				Reason = "Require Robotics Lab access for autonomous navigation research.",
				SubmittedAt = DateTime.UtcNow.AddDays(-2),
				Status = AccessRequestStatus.Pending,
				Score = 100
			});

			// Approved request
			accessRequests.Add(new AccessRequest
			{
				UserId = alice.Id,
				LabId = electrical.Id,
				TeamId = teamA.Id,
				Reason = "Electronics assembly and testing for project prototype.",
				SubmittedAt = DateTime.UtcNow.AddDays(-5),
				Status = AccessRequestStatus.Approved,
				Score = 100
			});

			// Rejected request (low score)
			accessRequests.Add(new AccessRequest
			{
				UserId = bob.Id,
				LabId = electrical.Id,
				Reason = "Basic electronics work needed.",
				SubmittedAt = DateTime.UtcNow.AddDays(-7),
				Status = AccessRequestStatus.Rejected,
				Score = 50
			});
		}
	}
}


