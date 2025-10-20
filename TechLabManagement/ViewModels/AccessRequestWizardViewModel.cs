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
	private readonly Dictionary<Guid, int> _userAnswers = new();

	public ObservableCollection<Lab> Labs { get; } = new();
	public ObservableCollection<QuestionViewModel> CurrentQuestions { get; } = new();

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

	/* Initializes lab list and wizard commands */
	public AccessRequestWizardViewModel()
	{
		foreach (var lab in _svc.Labs.GetAll()) Labs.Add(lab);
		SelectedLab = Labs.FirstOrDefault();
		NextCommand = new RelayCommand(_ => GoNext());
		BackCommand = new RelayCommand(_ => Step = Math.Max(0, Step - 1));
		SubmitCommand = new RelayCommand(_ => Submit());
	}

	/* Loads induction questions for the selected lab and resets user answers */
	private void LoadTest()
	{
		CurrentQuestions.Clear();
		_userAnswers.Clear();
		if (SelectedLab == null) return;

		var test = _svc.InductionTests.Query(t => t.LabId == SelectedLab.Id).FirstOrDefault();
		if (test != null)
		{
			foreach (var q in test.Questions)
			{
				CurrentQuestions.Add(new QuestionViewModel(q, _userAnswers));
			}
		}
	}

	/* Advances to the next step after validating current step inputs */
	private void GoNext()
	{
		// Request
		if (Step == 0)
		{
			if (SelectedLab == null)
			{
				MessageBox.Show("Please select a lab.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if (string.IsNullOrWhiteSpace(Reason))
			{
				MessageBox.Show("Please provide a reason for access request.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}

		// Induction
		if (Step == 1)
		{
			if (_userAnswers.Count != CurrentQuestions.Count)
			{
				MessageBox.Show("Please answer all induction questions.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}

		Step = Math.Min(2, Step + 1);
	}

	/* Submits the access request after validating inputs and computing the induction score */
	private void Submit()
	{
		if (SelectedLab == null || string.IsNullOrWhiteSpace(Reason))
		{
			MessageBox.Show("Please complete all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		if (_userAnswers.Count != CurrentQuestions.Count)
		{
			MessageBox.Show("Please answer all induction questions.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		try
		{
			// Create access request
			var request = new AccessRequest
			{
				UserId = _svc.CurrentUser.Id,
				LabId = SelectedLab.Id,
				Reason = Reason,
				Status = AccessRequestStatus.Draft
			};

			// Convert answers to list
			var answersList = CurrentQuestions.Select(q => _userAnswers[q.Question.Id]).ToList();

			// Submit through access service
			var submittedRequest = _svc.AccessService.SubmitAccessRequest(request, answersList);

			// Show result
			if (submittedRequest.Status == AccessRequestStatus.Pending)
			{
				MessageBox.Show(
					$"Access request submitted successfully!\n\nInduction Score: {submittedRequest.Score}%\nStatus: Pending Review\n\nYour request will be reviewed by an administrator.",
					"Success",
					MessageBoxButton.OK,
					MessageBoxImage.Information);
			}
			else if (submittedRequest.Status == AccessRequestStatus.Rejected)
			{
				MessageBox.Show(
					$"Induction test failed.\n\nScore: {submittedRequest.Score}%\n\nPlease review the safety requirements and try again.",
					"Induction Failed",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);
			}

			// Reset form
			ResetForm();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error submitting request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	/* Resets the wizard back to the initial state */
	private void ResetForm()
	{
		Step = 0;
		Reason = string.Empty;
		SelectedLab = Labs.FirstOrDefault();
	}
}

public sealed class QuestionViewModel : BaseViewModel
{
	private readonly Dictionary<Guid, int> _userAnswers;

	public Question Question { get; }
	public ObservableCollection<OptionViewModel> Options { get; } = new();

	/* Wraps a Question and exposes selectable options with selection tracking. */
	public QuestionViewModel(Question question, Dictionary<Guid, int> userAnswers)
	{
		Question = question;
		_userAnswers = userAnswers;

		for (int i = 0; i < question.Options.Count; i++)
		{
			Options.Add(new OptionViewModel(question.Options[i], i, this));
		}
	}

	/* Records the selected option and updates option selection states. */
	public void SelectOption(int optionIndex)
	{
		_userAnswers[Question.Id] = optionIndex;

		// Update IsSelected for all options
		foreach (var option in Options)
		{
			option.UpdateSelection();
		}
	}

	/* Returns whether the provided option index is currently selected. */
	public bool IsOptionSelected(int optionIndex)
	{
		return _userAnswers.TryGetValue(Question.Id, out var selectedIndex) && selectedIndex == optionIndex;
	}
}

/// <summary>
/// View model for a single option within a question
/// </summary>
public sealed class OptionViewModel : BaseViewModel
{
	private readonly QuestionViewModel _parent;

	public string Text { get; }
	public int Index { get; }
	public ICommand SelectCommand { get; }

	private bool _isSelected;
	public bool IsSelected
	{
		get => _isSelected;
		set => SetProperty(ref _isSelected, value);
	}

	/* Binds a single answer choice and exposes a command to select it. */
	public OptionViewModel(string text, int index, QuestionViewModel parent)
	{
		Text = text;
		Index = index;
		_parent = parent;
		SelectCommand = new RelayCommand(_ => _parent.SelectOption(Index));
	}

	/* Syncs IsSelected with the parent's current selection. */
	public void UpdateSelection()
	{
		IsSelected = _parent.IsOptionSelected(Index);
	}
}


