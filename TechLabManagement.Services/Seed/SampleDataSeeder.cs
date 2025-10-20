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
		// Users with credentials
		CreateUser(users, out var admin, "Admin", "admin", "admin@uni.local", UserRole.Admin, "!Haidrone123");
		CreateUser(users, out var prof, "Prof. Smith", "professor", "smith@uni.local", UserRole.Professor, "prof123");
		CreateUser(users, out var techMgr, "Alex TechMgr", "techmgr", "alex@uni.local", UserRole.TechnicalLabManager, "tech123");
		CreateUser(users, out var sup, "Dr. Lee", "supervisor", "lee@uni.local", UserRole.Supervisor, "sup123");
		CreateUser(users, out var alice, "Alice", "alice", "alice@uni.local", UserRole.Researcher, "alice123", supervisorId: null);
		alice.SupervisorId = sup.Id;
		users.Update(alice);
		CreateUser(users, out var bob, "Bob", "bob", "bob@uni.local", UserRole.Student, "bob123", supervisorId: sup.Id);

		// Additional users for analytics
		CreateUser(users, out var charlie, "Charlie", "charlie", "charlie@uni.local", UserRole.Researcher, "charlie123", supervisorId: sup.Id);
		CreateUser(users, out var diana, "Diana", "diana", "diana@uni.local", UserRole.Student, "diana123", supervisorId: sup.Id);
		CreateUser(users, out var eric, "Eric", "eric", "eric@uni.local", UserRole.Staff, "eric123");
		CreateUser(users, out var fiona, "Fiona", "fiona", "fiona@uni.local", UserRole.Researcher, "fiona123", supervisorId: sup.Id);
		CreateUser(users, out var grace, "Grace", "grace", "grace@uni.local", UserRole.Student, "grace123", supervisorId: sup.Id);
		CreateUser(users, out var henry, "Henry", "henry", "henry@uni.local", UserRole.Researcher, "henry123", supervisorId: sup.Id);
		CreateUser(users, out var iris, "Iris", "iris", "iris@uni.local", UserRole.Student, "iris123", supervisorId: sup.Id);
		CreateUser(users, out var jack, "Jack", "jack", "jack@uni.local", UserRole.Staff, "jack123");
		CreateUser(users, out var kim, "Kim", "kim", "kim@uni.local", UserRole.Student, "kim123", supervisorId: sup.Id);
		CreateUser(users, out var leo, "Leo", "leo", "leo@uni.local", UserRole.Researcher, "leo123", supervisorId: sup.Id);
		CreateUser(users, out var maya, "Maya", "maya", "maya@uni.local", UserRole.Student, "maya123", supervisorId: sup.Id);
		CreateUser(users, out var nora, "Nora", "nora", "nora@uni.local", UserRole.Researcher, "nora123", supervisorId: sup.Id);
		CreateUser(users, out var owen, "Owen", "owen", "owen@uni.local", UserRole.Staff, "owen123");
		CreateUser(users, out var pia, "Pia", "pia", "pia@uni.local", UserRole.Student, "pia123", supervisorId: sup.Id);
		CreateUser(users, out var quinn, "Quinn", "quinn", "quinn@uni.local", UserRole.Researcher, "quinn123", supervisorId: sup.Id);
		CreateUser(users, out var ryan, "Ryan", "ryan", "ryan@uni.local", UserRole.Student, "ryan123", supervisorId: sup.Id);
		CreateUser(users, out var sara, "Sara", "sara", "sara@uni.local", UserRole.Researcher, "sara123", supervisorId: sup.Id);
		CreateUser(users, out var tom, "Tom", "tom", "tom@uni.local", UserRole.Staff, "tom123");
		CreateUser(users, out var uma, "Uma", "uma", "uma@uni.local", UserRole.Student, "uma123", supervisorId: sup.Id);
		CreateUser(users, out var victor, "Victor", "victor", "victor@uni.local", UserRole.Researcher, "victor123", supervisorId: sup.Id);
		CreateUser(users, out var wendy, "Wendy", "wendy", "wendy@uni.local", UserRole.Student, "wendy123", supervisorId: sup.Id);
		CreateUser(users, out var xavier, "Xavier", "xavier", "xavier@uni.local", UserRole.Researcher, "xavier123", supervisorId: sup.Id);
		CreateUser(users, out var yuki, "Yuki", "yuki", "yuki@uni.local", UserRole.Student, "yuki123", supervisorId: sup.Id);
		CreateUser(users, out var zane, "Zane", "zane", "zane@uni.local", UserRole.Staff, "zane123");

		// Teams
		var teamA = teams.Add(new Team { Name = "Team A", ProjectName = "Project A", MemberIds = new() { alice.Id, bob.Id } });
		var teamB = teams.Add(new Team { Name = "Team B", ProjectName = "Project B", MemberIds = new() { alice.Id } });
		var teamC = teams.Add(new Team { Name = "Team C", ProjectName = "Autonomous Drones", MemberIds = new() { charlie.Id, diana.Id } });
		var teamD = teams.Add(new Team { Name = "Team D", ProjectName = "Cleanroom Microfabrication", MemberIds = new() { fiona.Id, henry.Id } });
		var teamE = teams.Add(new Team { Name = "Team E", ProjectName = "VR Rehab Study", MemberIds = new() { iris.Id, bob.Id } });
		var teamOps = teams.Add(new Team { Name = "Ops Team", ProjectName = "Facility Maintenance", MemberIds = new() { eric.Id, jack.Id } });
		var teamF = teams.Add(new Team { Name = "Team F", ProjectName = "EdgeAI Vision", MemberIds = new() { charlie.Id, grace.Id, leo.Id } });
		var teamG = teams.Add(new Team { Name = "Team G", ProjectName = "Soil Sensors", MemberIds = new() { henry.Id, sara.Id, zane.Id } });
		var teamH = teams.Add(new Team { Name = "Team H", ProjectName = "VR Rehab Cohort B", MemberIds = new() { iris.Id, yuki.Id } });
		var teamI = teams.Add(new Team { Name = "Team I", ProjectName = "Acoustic Beamforming", MemberIds = new() { diana.Id, wendy.Id } });
		var teamJ = teams.Add(new Team { Name = "Team J", ProjectName = "Robotics Safety", MemberIds = new() { fiona.Id, ryan.Id, quinn.Id } });

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

			// Extra bookings for analytics
			bookings.Add(new Booking
			{
				UserId = charlie.Id,
				LabId = robotics.Id,
				Start = today.AddDays(4).AddHours(9),
				End = today.AddDays(4).AddHours(11),
				Purpose = "SLAM dataset collection",
				Status = BookingStatus.Confirmed
			});
			bookings.Add(new Booking
			{
				UserId = diana.Id,
				EquipmentId = threeDPrinter.Id,
				Start = today.AddDays(1).AddHours(13),
				End = today.AddDays(1).AddHours(15),
				Purpose = "3D print casing prototype",
				Status = BookingStatus.Pending
			});
			bookings.Add(new Booking
			{
				UserId = eric.Id,
				EquipmentId = osc.Id,
				Start = today.AddHours(16),
				End = today.AddHours(17),
				Purpose = "Oscilloscope calibration",
				Status = BookingStatus.Confirmed
			});
			bookings.Add(new Booking
			{
				UserId = fiona.Id,
				EquipmentId = micromanip.Id,
				Start = today.AddDays(2).AddHours(9),
				End = today.AddDays(2).AddHours(12),
				Purpose = "Cell positioning trials",
				Status = BookingStatus.Confirmed
			});
			bookings.Add(new Booking
			{
				UserId = grace.Id,
				LabId = dataLab.Id,
				Start = today.AddDays(5).AddHours(10),
				End = today.AddDays(5).AddHours(13),
				Purpose = "Dataset labeling sprint",
				Status = BookingStatus.Confirmed
			});
			bookings.Add(new Booking
			{
				UserId = henry.Id,
				EquipmentId = robotArm.Id,
				Start = today.AddDays(6).AddHours(14),
				End = today.AddDays(6).AddHours(16),
				Purpose = "Manipulator repeatability test",
				Status = BookingStatus.Pending
			});
			bookings.Add(new Booking
			{
				UserId = iris.Id,
				LabId = mr.Id,
				Start = today.AddDays(7).AddHours(9),
				End = today.AddDays(7).AddHours(10),
				Purpose = "VR rehab pilot session",
				Status = BookingStatus.Confirmed
			});
			bookings.Add(new Booking
			{
				UserId = jack.Id,
				EquipmentId = shearBox.Id,
				Start = today.AddDays(1).AddHours(8),
				End = today.AddDays(1).AddHours(10),
				Purpose = "Soil shear maintenance check",
				Status = BookingStatus.Rejected
			});

			// Role-based booking frequency over a wider time window
			var rng = new Random(4242);
			var allLabs = labs.GetAll().ToList();
			var allEq = equipment.GetAll().ToList();
			var allTeams = teams.GetAll().ToList();
			foreach (var u in users.GetAll())
			{
				int target = u.Role switch
				{
					UserRole.Student => 10,
					UserRole.Researcher => 12,
					UserRole.Staff => 6,
					UserRole.Supervisor => 4,
					UserRole.TechnicalLabManager => 3,
					UserRole.AcademicLabManager => 2,
					UserRole.Professor => 3,
					UserRole.Admin => 1,
					_ => 2
				};
				for (int i = 0; i < target; i++)
				{
					bool useEq = allEq.Count > 0 && rng.NextDouble() < 0.5;
					int dayOffset = rng.Next(-30, 31);
					int startHour = rng.Next(8, 18);
					int durationHours = rng.Next(1, 4);
					var startT = DateTime.Today.AddDays(dayOffset).AddHours(startHour);
					var endT = startT.AddHours(durationHours);
					var status = rng.NextDouble() < 0.7 ? BookingStatus.Confirmed : (rng.NextDouble() < 0.75 ? BookingStatus.Pending : BookingStatus.Rejected);
					var b = new Booking
					{
						UserId = u.Id,
						Start = startT,
						End = endT,
						Purpose = $"{u.Role} session #{i + 1}",
						Status = status
					};
					if (useEq)
					{
						var eq = allEq[rng.Next(allEq.Count)];
						b.EquipmentId = eq.Id;
					}
					else
					{
						var lb = allLabs[rng.Next(allLabs.Count)];
						b.LabId = lb.Id;
					}
					var myTeams = allTeams.Where(t => t.MemberIds.Contains(u.Id)).ToList();
					if (myTeams.Count > 0 && rng.NextDouble() < 0.7)
					{
						b.TeamId = myTeams[rng.Next(myTeams.Count)].Id;
					}
					bookings.Add(b);
				}
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

			// Additional access requests for analytics
			accessRequests.Add(new AccessRequest
			{
				UserId = charlie.Id,
				LabId = microNano.Id,
				TeamId = teamD.Id,
				Reason = "Access to microfabrication lab for sensor arrays.",
				SubmittedAt = DateTime.UtcNow.AddDays(-3),
				Status = AccessRequestStatus.Pending,
				Score = 85
			});
			accessRequests.Add(new AccessRequest
			{
				UserId = diana.Id,
				LabId = mr.Id,
				TeamId = teamE.Id,
				Reason = "VR study pilot access.",
				SubmittedAt = DateTime.UtcNow.AddDays(-1),
				Status = AccessRequestStatus.Approved,
				Score = 95
			});
			accessRequests.Add(new AccessRequest
			{
				UserId = eric.Id,
				LabId = dataLab.Id,
				TeamId = teamOps.Id,
				Reason = "Data lab access for maintenance analytics dashboard.",
				SubmittedAt = DateTime.UtcNow.AddDays(-4),
				Status = AccessRequestStatus.Approved,
				Score = 100
			});
			accessRequests.Add(new AccessRequest
			{
				UserId = grace.Id,
				LabId = robotics.Id,
				TeamId = teamD.Id,
				Reason = "Robotics lab access for perception benchmarking.",
				SubmittedAt = DateTime.UtcNow.AddDays(-2),
				Status = AccessRequestStatus.Rejected,
				Score = 60
			});

			// Role-based bulk access requests
			var rngAR = new Random(5252);
			var labsList = labs.GetAll().ToList();
			var teamsList = teams.GetAll().ToList();
			foreach (var u in users.GetAll().Where(u => u.Role is UserRole.Student or UserRole.Researcher))
			{
				int count = rngAR.Next(1, 4);
				for (int i = 0; i < count; i++)
				{
					var lb = labsList[rngAR.Next(labsList.Count)];
					var myTeams = teamsList.Where(t => t.MemberIds.Contains(u.Id)).ToList();
					Guid? teamId = null;
					if (myTeams.Count > 0 && rngAR.NextDouble() < 0.6) teamId = myTeams[rngAR.Next(myTeams.Count)].Id;
					var r = rngAR.NextDouble();
					var status = r < 0.5 ? AccessRequestStatus.Pending : (r < 0.8 ? AccessRequestStatus.Approved : AccessRequestStatus.Rejected);
					int score = status switch
					{
						AccessRequestStatus.Approved => rngAR.Next(90, 101),
						AccessRequestStatus.Pending => rngAR.Next(80, 96),
						_ => rngAR.Next(50, 76)
					};
					accessRequests.Add(new AccessRequest
					{
						UserId = u.Id,
						LabId = lb.Id,
						TeamId = teamId,
						Reason = $"Request {i + 1} for {lb.Name}",
						SubmittedAt = DateTime.UtcNow.AddDays(-rngAR.Next(1, 30)),
						Status = status,
						Score = score
					});
				}
			}
		}
	}

	private static void CreateUser(IRepository<User> users, out User user, string name, string username, string email, UserRole role, string password, Guid? supervisorId = null)
	{
		TechLabManagement.Services.Security.PasswordHasher.CreateHash(password, out var salt, out var hash);
		user = users.Add(new User
		{
			Name = name,
			Username = username,
			Email = email,
			PasswordHash = hash,
			PasswordSalt = salt,
			Role = role,
			SupervisorId = supervisorId
		});
	}
}


