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

		// Additional labs and sublabs
		var electricalBench = labs.Add(new ElectricalLab
		{
			Name = "Electrical Lab - Soldering Bench Area",
			Location = "B1-101A",
			ParentLabId = electrical.Id,
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id,
			AccessPolicy = new ElectricalAccessPolicy()
		});

		var roboticsMobile = labs.Add(new RoboticsLab
		{
			Name = "Robotics Lab - Mobile Robots Zone",
			Location = "B2-201Z",
			ParentLabId = robotics.Id,
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id,
			AccessPolicy = new RoboticsAccessPolicy()
		});

		var acoustic = labs.Add(new AcousticLab
		{
			Name = "Acoustic Lab",
			Location = "B4-110",
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id
		});

		var dataLab = labs.Add(new DataAnalyticsLab
		{
			Name = "Multimedia Data Analytics Lab",
			Location = "C1-210",
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id
		});

		var microNano = labs.Add(new MicroNanoLab
		{
			Name = "Micro and Nanoscale Lab",
			Location = "D2-010",
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id
		});

		var geo = labs.Add(new GeotechnologyLab
		{
			Name = "Geotechnology Lab",
			Location = "E3-120",
			OwnerId = prof.Id,
			TechnicalManagerId = techMgr.Id,
			AcademicManagerId = prof.Id
		});

		// Equipment
		var solder = equipment.Add(new SolderingStation { Name = "Soldering Station", Manufacturer = "Hakko", Model = "FX-951", LabId = electricalBench.Id, SafetyTags = new() { "Electrical", "PPE", "ESD" } });
		var osc = equipment.Add(new Oscilloscope { Name = "Digital Oscilloscope", Manufacturer = "Rigol", Model = "DS1104Z", LabId = electrical.Id, SafetyTags = new() { "Electrical" } });
		var spec = equipment.Add(new SpectrumAnalyzer { Name = "RF Spectrum Analyzer", Manufacturer = "Keysight", Model = "N9000B", LabId = electrical.Id, SafetyTags = new() { "RF", "HearingProtection" } });
		var threeDPrinter = equipment.Add(new ThreeDPrinter { Name = "3D Printer", Manufacturer = "Prusa", Model = "MK4", LabId = roboticsMobile.Id, SafetyTags = new() { "FDM", "HotSurface" } });
		var robotArm = equipment.Add(new RobotArm { Name = "UR10 Robot Arm", Manufacturer = "Universal Robots", Model = "UR10e", LabId = robotics.Id, SafetyTags = new() { "EmergencyStop" } });
		var motion = equipment.Add(new MotionPlatform { Name = "Motion Platform", Manufacturer = "Moog", Model = "6DOF", LabId = mr.Id, SafetyTags = new() { "Motion" } });
		var vr = equipment.Add(new VRHeadset { Name = "VR Headset", Manufacturer = "Meta", Model = "Quest 3", LabId = mr.Id, SafetyTags = new() { "Hygiene", "Motion" } });
		var micromanip = equipment.Add(new MicroManipulator { Name = "Micro Manipulator", Manufacturer = "Narishige", Model = "MM-94", LabId = microNano.Id, SafetyTags = new() { "Cleanroom" } });
		var shearBox = equipment.Add(new SoilShearBox { Name = "Soil Shear Box", Manufacturer = "Controls Group", Model = "ShearTEST", LabId = geo.Id, SafetyTags = new() { "HeavyMachinery" } });

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
				new Question { Text = "Use ESD protection when soldering?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Check equipment is isolated before probing?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Maximum bench supply current should be set before powering device?", Options = new() { "True", "False" }, CorrectOptionIndex = 0 }
			}
		});

		tests.Add(new InductionTest
		{
			LabId = robotics.Id,
			Questions = new()
			{
				new Question { Text = "Know emergency stop location?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Is workspace clear of humans during autonomous run?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Robots must be tethered or fenced when required?", Options = new() { "True", "False" }, CorrectOptionIndex = 0 }
			}
		});

		tests.Add(new InductionTest
		{
			LabId = mr.Id,
			Questions = new()
			{
				new Question { Text = "Understand motion sickness risks?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Secure loose items before platform use?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Perform pre-run hardware checks?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
			}
		});

		// New labs tests
		tests.Add(new InductionTest
		{
			LabId = acoustic.Id,
			Questions = new()
			{
				new Question { Text = "Wear hearing protection when required?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Calibrate microphones before measurement?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
			}
		});

		tests.Add(new InductionTest
		{
			LabId = dataLab.Id,
			Questions = new()
			{
				new Question { Text = "No food or drink near workstations?", Options = new() { "True", "False" }, CorrectOptionIndex = 0 },
				new Question { Text = "Comply with data security and privacy policies?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
			}
		});

		tests.Add(new InductionTest
		{
			LabId = microNano.Id,
			Questions = new()
			{
				new Question { Text = "Cleanroom gowning required before entry?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Report any contamination immediately?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
			}
		});

		tests.Add(new InductionTest
		{
			LabId = geo.Id,
			Questions = new()
			{
				new Question { Text = "Use PPE around heavy machinery?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 },
				new Question { Text = "Keep hands clear of moving parts?", Options = new() { "Yes", "No" }, CorrectOptionIndex = 0 }
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

			// Additional facility usage
			bookings.Add(new Booking
			{
				UserId = bob.Id,
				LabId = acoustic.Id,
				Start = today.AddDays(3).AddHours(10),
				End = today.AddDays(3).AddHours(12),
				Purpose = "Acoustic chamber measurements",
				Status = BookingStatus.Pending
			});
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


