using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechLabManagement.Commands;
using TechLabManagement.Core.Models;
using TechLabManagement.Services;

namespace TechLabManagement.ViewModels;

public sealed class AccessRequestWizardViewModel : BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	public ObservableCollection<Lab> Labs { get; } = new();
    public ObservableCollection<Question> CurrentQuestions { get; } = new();
    public ObservableCollection<int> SelectedAnswers { get; } = new();

	private Lab? _selectedLab;
	public Lab? SelectedLab { get => _selectedLab; set { if (SetProperty(ref _selectedLab, value)) LoadTest(); } }

	private string _reason = string.Empty;
	public string Reason { get => _reason; set => SetProperty(ref _reason, value); }

	private int _step;
	public int Step { get => _step; set => SetProperty(ref _step, value); }

	public string Summary => SelectedLab == null ? "" : $"Lab: {SelectedLab.Name}\nReason: {Reason}";

	public ICommand NextCommand { get; }
	public ICommand BackCommand { get; }
	public ICommand SubmitCommand { get; }

	public AccessRequestWizardViewModel()
	{
		foreach (var lab in _svc.Labs.GetAll()) Labs.Add(lab);
		SelectedLab = Labs.FirstOrDefault();
        NextCommand = new RelayCommand(_ => Step = Math.Min(2, Step + 1));
		BackCommand = new RelayCommand(_ => Step = Math.Max(0, Step - 1));
        SubmitCommand = new RelayCommand(_ => Submit());
	}

	private void LoadTest()
	{
		CurrentQuestions.Clear();
        SelectedAnswers.Clear();
		if (SelectedLab == null) return;
        var test = _svc.InductionTests.Query(t => t.LabId == SelectedLab.Id).FirstOrDefault();
		if (test != null)
		{
            foreach (var q in test.Questions) { CurrentQuestions.Add(q); SelectedAnswers.Add(0); }
		}
	}

	private void Submit()
	{
		if (SelectedLab == null || string.IsNullOrWhiteSpace(Reason))
		{
			MessageBox.Show("Please select lab and provide a reason.");
			return;
		}
        var user = _svc.Users.GetAll().First();
        var req = _svc.AccessService.SubmitAccessRequest(user.Id, SelectedLab.Id, null, Reason, SelectedAnswers.ToList());
        MessageBox.Show($"Submitted with status: {req.Status} (score {req.Score})");
	}
}


