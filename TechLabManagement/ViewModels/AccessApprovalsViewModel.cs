using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;
using TechLabManagement.Utils;
using System.Windows.Documents;

namespace TechLabManagement.ViewModels;

public sealed class AccessApprovalsViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

    public ObservableCollection<ApprovalRow> PendingRequests { get; } = new();
    public ObservableCollection<ApprovalRow> ApprovedRequests { get; } = new();
    public ObservableCollection<ApprovalRow> RejectedRequests { get; } = new();

    public ObservableCollection<Lab> Labs { get; } = new();
    public ObservableCollection<User> Managers { get; } = new();
	private ApprovalRow? _selectedRequest;
    public ApprovalRow? SelectedRequest { get => _selectedRequest; set => SetProperty(ref _selectedRequest, value); }

    private Lab? _selectedLab;
    public Lab? SelectedLab { get => _selectedLab; set { if (SetProperty(ref _selectedLab, value)) Reload(); } }

    private User? _selectedManager;
    public User? SelectedManager { get => _selectedManager; set { if (SetProperty(ref _selectedManager, value)) Reload(); } }

    public ICommand ApproveCommand { get; }
    public ICommand RejectCommand { get; }
    public ICommand ExportCsvCommand { get; }
    public ICommand PrintCommand { get; }

	public AccessApprovalsViewModel()
	{
        ApproveCommand = new RelayCommand(_ => ApproveSelected(), _ => SelectedRequest != null);
        RejectCommand = new RelayCommand(_ => RejectSelected(), _ => SelectedRequest != null);
        ExportCsvCommand = new RelayCommand(_ => ExportCsv());
        PrintCommand = new RelayCommand(_ => Print());
        foreach (var l in _svc.Labs.GetAll()) Labs.Add(l);
        foreach (var u in _svc.Users.GetAll().Where(u => u.Role == UserRole.TechnicalLabManager || u.Role == UserRole.Professor || u.Role == UserRole.Admin)) Managers.Add(u);
        SelectedLab = Labs.FirstOrDefault();
        SelectedManager = Managers.FirstOrDefault();
        Reload();
	}

	private void Reload()
	{
        PendingRequests.Clear();
        ApprovedRequests.Clear();
        RejectedRequests.Clear();

        IEnumerable<AccessRequest> Filter(IEnumerable<AccessRequest> src)
        {
            if (SelectedLab != null) src = src.Where(r => r.LabId == SelectedLab.Id);
            if (SelectedManager != null)
            {
                // Filter by manager being owner/technical manager/academic manager of the lab
                src = src.Where(r =>
                {
                    var lab = _svc.Labs.GetById(r.LabId);
                    if (lab == null) return false;
                    return lab.OwnerId == SelectedManager.Id || lab.TechnicalManagerId == SelectedManager.Id || lab.AcademicManagerId == SelectedManager.Id;
                });
            }
            return src;
        }

        foreach (var r in Filter(_svc.AccessService.GetRequestsByStatus(AccessRequestStatus.Pending)))
            PendingRequests.Add(ToRow(r));
        foreach (var r in Filter(_svc.AccessService.GetRequestsByStatus(AccessRequestStatus.Approved)))
            ApprovedRequests.Add(ToRow(r));
        foreach (var r in Filter(_svc.AccessService.GetRequestsByStatus(AccessRequestStatus.Rejected)))
            RejectedRequests.Add(ToRow(r));
	}

    private ApprovalRow ToRow(AccessRequest r) => new ApprovalRow
    {
        Id = r.Id,
        UserName = _svc.Users.GetById(r.UserId)?.Name ?? "Unknown",
        LabName = _svc.Labs.GetById(r.LabId)?.Name ?? r.LabId.ToString(),
        Reason = r.Reason,
        Score = r.Score ?? 0,
        SubmittedAt = r.SubmittedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
    };

	private void ApproveSelected()
	{
		if (SelectedRequest == null) return;
		_svc.AccessService.Approve(SelectedRequest.Id);
		MessageBox.Show("Approved");
		Reload();
	}

	private void RejectSelected()
	{
		if (SelectedRequest == null) return;
		_svc.AccessService.Reject(SelectedRequest.Id, "Rejected by manager");
		MessageBox.Show("Rejected");
		Reload();
	}

    private void ExportCsv()
    {
        var rows = PendingRequests.Select(r => (IDictionary<string, object?>)new Dictionary<string, object?>
        {
            ["Id"] = r.Id,
            ["User"] = r.UserName,
            ["Lab"] = r.LabName,
            ["Reason"] = r.Reason,
            ["Score"] = r.Score,
            ["SubmittedAt"] = r.SubmittedAt
        }).ToList();
        var csv = CsvExporter.ToCsv(rows);
        System.IO.File.WriteAllText("Approvals_Pending.csv", csv);
        MessageBox.Show("Exported to Approvals_Pending.csv");
    }

    private void Print()
    {
        var pd = new System.Windows.Controls.PrintDialog();
        if (pd.ShowDialog() == true)
        {
            var doc = new FlowDocument();
            doc.Blocks.Add(new Paragraph(new Run("Pending Access Requests")) { FontSize = 18, FontWeight = FontWeights.Bold });
            foreach (var r in PendingRequests)
            {
                doc.Blocks.Add(new Paragraph(new Run($"{r.Id} - {r.UserName} - {r.LabName} - {r.Score}")));
            }
            pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Approvals");
        }
    }
}

public sealed class ApprovalRow
{
	public Guid Id { get; set; }
	public string UserName { get; set; } = string.Empty;
	public string LabName { get; set; } = string.Empty;
	public string Reason { get; set; } = string.Empty;
	public int Score { get; set; }
    public string SubmittedAt { get; set; } = string.Empty;
}


