using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;

namespace TechLabManagement.ViewModels;

/// <summary>
/// ViewModel for approving or rejecting access requests
/// </summary>
public sealed class ApprovalsViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public ObservableCollection<AccessRequestViewModel> AccessRequests { get; } = new();
	public ObservableCollection<LabFilterItem> Labs { get; } = new();

	private LabFilterItem? _filterLab;
	public LabFilterItem? FilterLab
	{
		get => _filterLab;
		set
		{
			if (SetProperty(ref _filterLab, value))
			{
				LoadAccessRequests();
			}
		}
	}

    private AccessRequestStatus? _filterStatus = AccessRequestStatus.Pending;
    public AccessRequestStatus? FilterStatus
	{
		get => _filterStatus;
		set
		{
			if (SetProperty(ref _filterStatus, value))
			{
				LoadAccessRequests();
			}
		}
	}

	private AccessRequestViewModel? _selectedRequest;
	public AccessRequestViewModel? SelectedRequest
	{
		get => _selectedRequest;
		set => SetProperty(ref _selectedRequest, value);
	}

	public ICommand ApproveCommand { get; }
	public ICommand RejectCommand { get; }
	public ICommand RefreshCommand { get; }
	public ICommand ShowAllCommand { get; }
	public ICommand ShowPendingCommand { get; }
	public ICommand ShowApprovedCommand { get; }
	public ICommand ShowRejectedCommand { get; }

	public ApprovalsViewModel()
	{
		// Load labs for filtering
		Labs.Add(new LabFilterItem { Id = null, Name = "All Labs" }); // Placeholder for "All"
		foreach (var lab in _svc.Labs.GetAll())
		{
			Labs.Add(new LabFilterItem { Id = lab.Id, Name = lab.Name });
		}
		FilterLab = Labs.First();

		// Commands
		ApproveCommand = new RelayCommand(_ => ApproveRequest(), _ => CanApprove());
		RejectCommand = new RelayCommand(_ => RejectRequest(), _ => CanReject());
		RefreshCommand = new RelayCommand(_ => LoadAccessRequests());
        ShowAllCommand = new RelayCommand(_ => { FilterStatus = null; LoadAccessRequests(); });
        ShowPendingCommand = new RelayCommand(_ => { FilterStatus = AccessRequestStatus.Pending; LoadAccessRequests(); });
        ShowApprovedCommand = new RelayCommand(_ => { FilterStatus = AccessRequestStatus.Approved; LoadAccessRequests(); });
        ShowRejectedCommand = new RelayCommand(_ => { FilterStatus = AccessRequestStatus.Rejected; LoadAccessRequests(); });

		LoadAccessRequests();
	}

	/// <summary>
	/// Load access requests based on current filters
	/// </summary>
	private void LoadAccessRequests()
	{
		AccessRequests.Clear();

		var labId = FilterLab?.Id; // Will be null if "All Labs" is selected

        IEnumerable<AccessRequest> requests = _svc.AccessService.GetAccessRequests(FilterStatus, labId);

		foreach (var request in requests)
		{
			var user = _svc.Users.GetById(request.UserId);
			var lab = _svc.Labs.GetById(request.LabId);
			var team = request.TeamId.HasValue ? _svc.Teams.GetById(request.TeamId.Value) : null;

			AccessRequests.Add(new AccessRequestViewModel
			{
				Request = request,
				UserName = user?.Name ?? "Unknown",
				LabName = lab?.Name ?? "Unknown",
				TeamName = team?.Name ?? "N/A",
				SubmittedDate = request.SubmittedAt.ToString("g"),
				StatusColor = GetStatusColor(request.Status)
			});
		}
	}

	/// <summary>
	/// Determine if the selected request can be approved
	/// </summary>
	private bool CanApprove()
	{
		if (SelectedRequest == null) return false;
		if (SelectedRequest.Request.Status != AccessRequestStatus.Pending) return false;
		return ServiceLocator.Current.Authorization.HasPermission(Core.Models.Permission.ApproveAccessRequest);
	}

	/// <summary>
	/// Determine if the selected request can be rejected
	/// </summary>
	private bool CanReject()
	{
		if (SelectedRequest == null) return false;
		if (SelectedRequest.Request.Status != AccessRequestStatus.Pending) return false;
		return ServiceLocator.Current.Authorization.HasPermission(Core.Models.Permission.RejectAccessRequest);
	}

	/// <summary>
	/// Approve the selected access request
	/// </summary>
	private void ApproveRequest()
	{
		if (SelectedRequest == null) return;

		var result = MessageBox.Show(
			$"Approve access request for {SelectedRequest.UserName} to {SelectedRequest.LabName}?\n\nReason: {SelectedRequest.Request.Reason}",
			"Confirm Approval",
			MessageBoxButton.YesNo,
			MessageBoxImage.Question);

		if (result != MessageBoxResult.Yes) return;

		try
		{
			_svc.AccessService.ApproveAccessRequest(SelectedRequest.Request.Id, _svc.CurrentUser.Id);

			MessageBox.Show(
				"Access request approved successfully!",
				"Success",
				MessageBoxButton.OK,
				MessageBoxImage.Information);

			LoadAccessRequests();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error approving request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	/// <summary>
	/// Reject the selected access request
	/// </summary>
	private void RejectRequest()
	{
		if (SelectedRequest == null) return;

		var result = MessageBox.Show(
			$"Reject access request for {SelectedRequest.UserName} to {SelectedRequest.LabName}?\n\nReason: {SelectedRequest.Request.Reason}",
			"Confirm Rejection",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning);

		if (result != MessageBoxResult.Yes) return;

		try
		{
			_svc.AccessService.RejectAccessRequest(SelectedRequest.Request.Id, _svc.CurrentUser.Id);

			MessageBox.Show(
				"Access request rejected.",
				"Request Rejected",
				MessageBoxButton.OK,
				MessageBoxImage.Information);

			LoadAccessRequests();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error rejecting request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	/// <summary>
	/// Get color based on request status
	/// </summary>
	private static string GetStatusColor(AccessRequestStatus status)
	{
		return status switch
		{
			AccessRequestStatus.Pending => "#FFA500",
			AccessRequestStatus.Approved => "#4CAF50",
			AccessRequestStatus.Rejected => "#F44336",
			_ => "#999999"
		};
	}
}

/// <summary>
/// View model for a single access request in the list
/// </summary>
public sealed class AccessRequestViewModel
{
	public AccessRequest Request { get; set; } = null!;
	public string UserName { get; set; } = string.Empty;
	public string LabName { get; set; } = string.Empty;
	public string TeamName { get; set; } = string.Empty;
	public string SubmittedDate { get; set; } = string.Empty;
	public string StatusColor { get; set; } = string.Empty;
}

/// <summary>
/// Helper class for lab filtering in the approvals view
/// </summary>
public sealed class LabFilterItem
{
	public Guid? Id { get; set; }
	public string Name { get; set; } = string.Empty;
}

