using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;

namespace TechLabManagement.ViewModels;

public sealed class BookingSchedulerViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public ObservableCollection<ResourceItem> Resources { get; } = new();
	public ObservableCollection<string> TimeSlots { get; } = new();
	public ObservableCollection<BookingItem> ExistingBookings { get; } = new();

	private ResourceItem? _selectedResource;
	public ResourceItem? SelectedResource { get => _selectedResource; set { if (SetProperty(ref _selectedResource, value)) LoadBookings(); } }

	private DateTime _selectedDate = DateTime.Today;
	public DateTime SelectedDate { get => _selectedDate; set { if (SetProperty(ref _selectedDate, value)) LoadBookings(); } }

	public string StartTime { get; set; } = "09:00";
	public string EndTime { get; set; } = "11:00";

	public ICommand BookCommand { get; }

	public BookingSchedulerViewModel()
	{
		foreach (var lab in _svc.Labs.GetAll()) Resources.Add(new ResourceItem { Id = lab.Id, Name = $"Lab: {lab.Name}", IsEquipment = false });
		foreach (var eq in _svc.Equipment.GetAll()) Resources.Add(new ResourceItem { Id = eq.Id, Name = $"Eq: {eq.Name}", IsEquipment = true });
		SelectedResource = Resources.FirstOrDefault();
		for (int h = 8; h <= 20; h++) TimeSlots.Add($"{h:00}:00");
		BookCommand = new RelayCommand(_ => Book());
		LoadBookings();
	}

	private void LoadBookings()
	{
		ExistingBookings.Clear();
		if (SelectedResource == null) return;
		var from = SelectedDate.Date;
		var to = from.AddDays(1);
		var bookings = _svc.SchedulingService.GetBookingsForResource(
			SelectedResource.IsEquipment ? null : SelectedResource.Id,
			SelectedResource.IsEquipment ? SelectedResource.Id : null,
			from,
			to).OrderBy(b => b.Start);
		foreach (var b in bookings)
		{
			ExistingBookings.Add(new BookingItem
			{
				Start = b.Start.ToString("HH:mm"),
				End = b.End.ToString("HH:mm"),
				ResourceName = SelectedResource.Name,
				UserName = _svc.Users.GetById(b.UserId)?.Name ?? "Unknown"
			});
		}
	}

	private void Book()
	{
		if (SelectedResource == null) return;
		var user = _svc.Users.GetAll().First();
		var start = DateTime.Parse($"{SelectedDate:yyyy-MM-dd} {StartTime}");
		var end = DateTime.Parse($"{SelectedDate:yyyy-MM-dd} {EndTime}");
		var booking = new Booking
		{
			UserId = user.Id,
			LabId = SelectedResource.IsEquipment ? null : SelectedResource.Id,
			EquipmentId = SelectedResource.IsEquipment ? SelectedResource.Id : null,
			Start = start,
			End = end,
			Purpose = "General"
		};
		var (ok, error) = _svc.SchedulingService.TryCreateBooking(booking);
		if (!ok) { MessageBox.Show(error ?? "Error"); return; }
		LoadBookings();
	}
}

public sealed class ResourceItem
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool IsEquipment { get; set; }
}

public sealed class BookingItem
{
	public string Start { get; set; } = string.Empty;
	public string End { get; set; } = string.Empty;
	public string ResourceName { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
}


