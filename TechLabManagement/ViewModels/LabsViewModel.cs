using System.Collections.ObjectModel;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;

namespace TechLabManagement.ViewModels;

public sealed class LabsViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public ObservableCollection<Lab> Labs { get; } = new();
	public ObservableCollection<Equipment> Equipment { get; } = new();

	private Lab? _selectedLab;
	public Lab? SelectedLab
	{
		get => _selectedLab;
		set { if (SetProperty(ref _selectedLab, value)) LoadEquipment(); } // update equipment list when lab changes
	}

	public ICommand RequestAccessCommand { get; }

	/* Initializes labs list and default selection; wires the request access command. */
	public LabsViewModel()
	{
		foreach (var lab in _svc.Labs.GetAll().OrderBy(l => l.Name)) Labs.Add(lab);
		SelectedLab = Labs.FirstOrDefault();
		RequestAccessCommand = new RelayCommand(_ => System.Windows.MessageBox.Show("Access request flow will open."));
	}

	/* Loads equipment for the selected lab into the right-hand list. */
	private void LoadEquipment()
	{
		Equipment.Clear();
		if (SelectedLab == null) return;
		foreach (var e in _svc.Equipment.GetAll().Where(e => e.LabId == SelectedLab.Id)) Equipment.Add(e);
	}
}


