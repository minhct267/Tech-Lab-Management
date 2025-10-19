using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;
using TechLabManagement.Utils;
using System.Windows.Documents;

namespace TechLabManagement.ViewModels;

public sealed class MaintenanceSafetyViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public ObservableCollection<Equipment> Equipment { get; } = new();
	public ObservableCollection<int> DueDaysOptions { get; } = new() { 3, 7, 14, 30 };
	public ObservableCollection<MaintenanceTask> Tasks { get; } = new();

	private Equipment? _selectedEquipment;
	public Equipment? SelectedEquipment { get => _selectedEquipment; set { if (SetProperty(ref _selectedEquipment, value)) Reload(); } }

	private int _selectedDueDays = 7;
	public int SelectedDueDays { get => _selectedDueDays; set { if (SetProperty(ref _selectedDueDays, value)) Reload(); } }

	public ICommand AddTaskCommand { get; }
	public ICommand CompleteTaskCommand { get; }
    public ICommand ExportCsvCommand { get; }
    public ICommand PrintCommand { get; }

	public MaintenanceSafetyViewModel()
	{
		foreach (var e in _svc.Equipment.GetAll()) Equipment.Add(e);
		SelectedEquipment = Equipment.FirstOrDefault();
        AddTaskCommand = new RelayCommand(_ => AddTask());
        CompleteTaskCommand = new RelayCommand(obj => CompleteTask(obj as MaintenanceTask));
        ExportCsvCommand = new RelayCommand(_ => ExportCsv());
        PrintCommand = new RelayCommand(_ => Print());
		Reload();
	}

    private void ExportCsv()
    {
        var rows = Tasks.Select(t => (IDictionary<string, object?>)new Dictionary<string, object?>
        {
            ["Equipment"] = _svc.Equipment.GetById(t.EquipmentId)?.Name ?? t.EquipmentId.ToString(),
            ["DueDate"] = t.DueDate,
            ["Type"] = t.Type,
            ["Notes"] = t.Notes,
            ["Status"] = t.Status
        }).ToList();
        var csv = CsvExporter.ToCsv(rows);
        System.IO.File.WriteAllText("Maintenance.csv", csv);
        MessageBox.Show("Exported to Maintenance.csv");
    }

    private void Print()
    {
        var pd = new System.Windows.Controls.PrintDialog();
        if (pd.ShowDialog() == true)
        {
            var doc = new FlowDocument();
            doc.Blocks.Add(new Paragraph(new Run("Maintenance Tasks")) { FontSize = 18, FontWeight = FontWeights.Bold });
            foreach (var t in Tasks)
            {
                var eq = _svc.Equipment.GetById(t.EquipmentId)?.Name ?? t.EquipmentId.ToString();
                doc.Blocks.Add(new Paragraph(new Run($"{t.DueDate:d} - {eq} - {t.Type} - {t.Status}")));
            }
            pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Maintenance");
        }
    }

	private void Reload()
	{
		Tasks.Clear();
		var to = DateTime.Today.AddDays(SelectedDueDays);
		var items = _svc.MaintenanceTasks.GetAll()
			.Where(t => (SelectedEquipment == null || t.EquipmentId == SelectedEquipment.Id) && t.Status == MaintenanceStatus.Open && t.DueDate <= to)
			.OrderBy(t => t.DueDate);
		foreach (var t in items) Tasks.Add(t);
	}

	private void AddTask()
	{
		if (SelectedEquipment == null) { MessageBox.Show("Select equipment"); return; }
		var task = new MaintenanceTask { EquipmentId = SelectedEquipment.Id, DueDate = DateTime.Today.AddDays(7), Type = "Inspection", Notes = "Auto-added" };
		_svc.MaintenanceTasks.Add(task);
		Reload();
	}

	private void CompleteTask(MaintenanceTask? task)
	{
		if (task == null) return;
		task.Status = MaintenanceStatus.Completed;
		_svc.MaintenanceTasks.Update(task);
		Reload();
	}
}


