using System.Windows;
using TechLabManagement.Services;

namespace TechLabManagement.Views;

public partial class LoginWindow : Window
{
	public LoginViewModel VM { get; }

	public LoginWindow()
	{
		InitializeComponent();
		VM = new LoginViewModel();
		DataContext = VM;
	}

	private void OnLoginClick(object sender, RoutedEventArgs e)
	{
		var password = PasswordBox.Password;
		if (VM.TryLogin(password))
		{
			DialogResult = true;
			Close();
		}
	}

	private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (e.Key == System.Windows.Input.Key.Enter)
		{
			OnLoginClick(sender, new RoutedEventArgs());
		}
	}

	private void PasswordRevealTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (e.Key == System.Windows.Input.Key.Enter)
		{
			PasswordBox.Password = PasswordRevealTextBox.Text;
			OnLoginClick(sender, new RoutedEventArgs());
		}
	}

	private void ShowPassword_Checked(object sender, RoutedEventArgs e)
	{
		PasswordRevealTextBox.Text = PasswordBox.Password;
		PasswordRevealTextBox.Visibility = Visibility.Visible;
		PasswordBox.Visibility = Visibility.Collapsed;
		PasswordRevealTextBox.Focus();
		PasswordRevealTextBox.CaretIndex = PasswordRevealTextBox.Text.Length;
	}

	private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
	{
		PasswordBox.Password = PasswordRevealTextBox.Text;
		PasswordBox.Visibility = Visibility.Visible;
		PasswordRevealTextBox.Visibility = Visibility.Collapsed;
		PasswordBox.Focus();
	}

	private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
	{
		if (ShowPasswordCheck.IsChecked == true)
		{
			PasswordRevealTextBox.Text = PasswordBox.Password;
		}
	}
}

public sealed class LoginViewModel : TechLabManagement.ViewModels.BaseViewModel
{
	private readonly ServiceLocator _svc = ServiceLocator.Current;

	private string _username = string.Empty;
	public string Username { get => _username; set => SetProperty(ref _username, value); }

	private string _errorMessage = string.Empty;
	public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

	public bool TryLogin(string password)
	{
		ErrorMessage = string.Empty;
		if (string.IsNullOrWhiteSpace(Username)) { ErrorMessage = "Please enter username."; return false; }
		if (string.IsNullOrEmpty(password)) { ErrorMessage = "Please enter password."; return false; }
		var ok = _svc.Auth.Login(Username, password);
		if (!ok)
		{
			ErrorMessage = "Invalid username or password.";
			return false;
		}
		return true;
	}
}


