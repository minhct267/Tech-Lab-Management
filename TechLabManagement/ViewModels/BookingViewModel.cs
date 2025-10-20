using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;

namespace TechLabManagement.ViewModels;

public sealed class BookingViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public ObservableCollection<ResourceItem> Resources { get; } = new();
	public ObservableCollection<BookingItem> ExistingBookings { get; } = new();

	private ResourceItem? _selectedResource;
	public ResourceItem? SelectedResource
	{
		get => _selectedResource;
		set
		{
			if (SetProperty(ref _selectedResource, value))
			{
				LoadExistingBookings(); // Refresh list when resource changes
			}
		}
	}

	private DateTime _selectedDate = DateTime.Today;
	public DateTime SelectedDate
	{
		get => _selectedDate;
		set
		{
			if (SetProperty(ref _selectedDate, value))
			{
				LoadExistingBookings(); // Refresh list when date changes
			}
		}
	}

	private int _startHour = 9;
	public int StartHour
	{
		get => _startHour;
		set
		{
			if (value < 0 || value > 23) return; // Guard hour range
			SetProperty(ref _startHour, value);
		}
	}

	private int _startMinute;
	public int StartMinute
	{
		get => _startMinute;
		set
		{
			if (value < 0 || value > 59) return; // Guard minute range
			SetProperty(ref _startMinute, value);
		}
	}

	private int _endHour = 10;
	public int EndHour
	{
		get => _endHour;
		set
		{
			if (value < 0 || value > 23) return; // Guard hour range
			SetProperty(ref _endHour, value);
		}
	}

	private int _endMinute;
	public int EndMinute
	{
		get => _endMinute;
		set
		{
			if (value < 0 || value > 59) return; // Guard minute range
			SetProperty(ref _endMinute, value);
		}
	}

	private string _purpose = string.Empty;
	public string Purpose
	{
		get => _purpose;
		set => SetProperty(ref _purpose, value);
	}

	private string _conflictMessage = string.Empty;
	public string ConflictMessage
	{
		get => _conflictMessage;
		set => SetProperty(ref _conflictMessage, value);
	}

	public ICommand CreateBookingCommand { get; }
	public ICommand CheckConflictsCommand { get; }

	/* Initializes resource list, selects a default, and wires commands */
	public BookingViewModel()
	{
		LoadResources();
		SelectedResource = Resources.FirstOrDefault();

		CreateBookingCommand = new RelayCommand(_ => CreateBooking());
		CheckConflictsCommand = new RelayCommand(_ => CheckForConflicts());
	}
	
	/* Loads all labs and equipment as bookable resources */
	private void LoadResources()
	{
		Resources.Clear();

		// Add labs
		foreach (var lab in _svc.Labs.GetAll())
		{
			Resources.Add(new ResourceItem
			{
				Id = lab.Id,
				Name = lab.Name,
				Type = ResourceType.Lab,
				Description = $"Location: {lab.Location}"
			});
		}

		// Add equipment
		foreach (var eq in _svc.Equipment.GetAll())
		{
			Resources.Add(new ResourceItem
			{
				Id = eq.Id,
				Name = eq.Name,
				Type = ResourceType.Equipment,
				Description = $"Lab: {eq.LabRef?.Name ?? "Unknown"}"
			});
		}
	}
	
	/* Load the existing bookings for the selection/date */
	private void LoadExistingBookings()
	{
		ExistingBookings.Clear();
		if (SelectedResource == null) return;

		var bookings = _svc.Bookings.GetAll()
			.Where(b => b.Start.Date == SelectedDate.Date)
			.Where(b =>
				(SelectedResource.Type == ResourceType.Lab && b.LabId == SelectedResource.Id) ||
				(SelectedResource.Type == ResourceType.Equipment && b.EquipmentId == SelectedResource.Id))
			.OrderBy(b => b.Start);

		foreach (var booking in bookings)
		{
			var user = _svc.Users.GetById(booking.UserId);
			ExistingBookings.Add(new BookingItem
			{
				Start = booking.Start.ToString("HH:mm"),
				End = booking.End.ToString("HH:mm"),
				UserName = user?.Name ?? "Unknown",
				Purpose = booking.Purpose,
				Status = booking.Status.ToString()
			});
		}
	}
	
	/* Checks whether the selected time slot overlaps existing bookings */
	private void CheckForConflicts()
	{
		if (SelectedResource == null)
		{
			ConflictMessage = "Please select a resource.";
			return;
		}

		var startTime = SelectedDate.Date.AddHours(StartHour).AddMinutes(StartMinute);
		var endTime = SelectedDate.Date.AddHours(EndHour).AddMinutes(EndMinute);

		if (endTime <= startTime)
		{
			ConflictMessage = "End time must be after start time.";
			return;
		}

		var conflicts = _svc.SchedulingService.GetConflicts(
			SelectedResource.Type == ResourceType.Lab ? SelectedResource.Id : null,
			SelectedResource.Type == ResourceType.Equipment ? SelectedResource.Id : null,
			startTime,
			endTime);

		if (conflicts.Any())
		{
			ConflictMessage = $"⚠ Conflict detected! {conflicts.Count()} existing booking(s) overlap with this time slot.";
		}
		else
		{
			ConflictMessage = "✓ No conflicts - time slot is available!";
		}
	}

	
	/* Creates a booking after validation */
	private void CreateBooking()
	{
		// Validation
		if (SelectedResource == null)
		{
			MessageBox.Show("Please select a resource to book.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		// Authorization check
		var canCreate = _svc.Authorization.CanCreateBooking(
			SelectedResource.Type == ResourceType.Lab ? SelectedResource.Id : null,
			SelectedResource.Type == ResourceType.Equipment ? SelectedResource.Id : null);
		if (!canCreate)
		{
			MessageBox.Show("You don't have permission to book this resource. Please request access first.", "Not Authorized", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		if (string.IsNullOrWhiteSpace(Purpose))
		{
			MessageBox.Show("Please provide a purpose for the booking.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		var startTime = SelectedDate.Date.AddHours(StartHour).AddMinutes(StartMinute);
		var endTime = SelectedDate.Date.AddHours(EndHour).AddMinutes(EndMinute);

		if (endTime <= startTime)
		{
			MessageBox.Show("End time must be after start time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		if (startTime < DateTime.Now)
		{
			MessageBox.Show("Cannot book in the past.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		// Check for conflicts
		var conflicts = _svc.SchedulingService.GetConflicts(
			SelectedResource.Type == ResourceType.Lab ? SelectedResource.Id : null,
			SelectedResource.Type == ResourceType.Equipment ? SelectedResource.Id : null,
			startTime,
			endTime);

		if (conflicts.Any())
		{
			var result = MessageBox.Show(
				$"There are {conflicts.Count()} existing booking(s) that conflict with this time slot.\n\nDo you want to proceed anyway?",
				"Booking Conflict",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning);

			if (result != MessageBoxResult.Yes)
			{
				return;
			}
		}

		try
		{
			var booking = new Booking
			{
				UserId = _svc.Auth.CurrentUser!.Id,
				LabId = SelectedResource.Type == ResourceType.Lab ? SelectedResource.Id : null,
				EquipmentId = SelectedResource.Type == ResourceType.Equipment ? SelectedResource.Id : null,
				Start = startTime,
				End = endTime,
				Purpose = Purpose,
				Status = BookingStatus.Confirmed
			};

			// Finalize via SchedulingService to enforce invariants
			var (ok, error) = _svc.SchedulingService.TryCreateBooking(booking);
			if (!ok)
			{
				MessageBox.Show(error ?? "Unable to create booking.", "Booking Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			MessageBox.Show(
				$"Booking created successfully!\n\nResource: {SelectedResource.Name}\nTime: {startTime:g} - {endTime:t}\nStatus: {booking.Status}",
				"Success",
				MessageBoxButton.OK,
				MessageBoxImage.Information);

			// Reset form
			Purpose = string.Empty;
			ConflictMessage = string.Empty;
			LoadExistingBookings();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error creating booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}

public sealed class ResourceItem
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public ResourceType Type { get; set; }
	public string Description { get; set; } = string.Empty;
	public string DisplayName => $"{Name} ({Type})";
}

public enum ResourceType
{
	Lab,
	Equipment
}

public sealed class BookingItem
{
	public string Start { get; set; } = string.Empty;
	public string End { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
	public string Purpose { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
}

