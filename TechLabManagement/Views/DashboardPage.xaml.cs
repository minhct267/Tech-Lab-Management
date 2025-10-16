using System.Windows;
using System.Windows.Controls;

namespace TechLabManagement.Views;

public partial class DashboardPage : Page
{
	public DashboardPage()
	{
		InitializeComponent();
		this.DataContext = new ViewModels.DashboardViewModel();
	}

	/// <summary>
	/// Navigate to Labs page
	/// </summary>
	private void NavigateToLabs_Click(object sender, RoutedEventArgs e)
	{
		NavigationService?.Navigate(new LabsPage());
	}

	/// <summary>
	/// Navigate to Access Request page
	/// </summary>
	private void NavigateToAccessRequest_Click(object sender, RoutedEventArgs e)
	{
		NavigationService?.Navigate(new AccessRequestWizardPage());
	}

	/// <summary>
	/// Navigate to Booking page
	/// </summary>
	private void NavigateToBooking_Click(object sender, RoutedEventArgs e)
	{
		NavigationService?.Navigate(new BookingPage());
	}

	/// <summary>
	/// Navigate to Approvals page
	/// </summary>
	private void NavigateToApprovals_Click(object sender, RoutedEventArgs e)
	{
		NavigationService?.Navigate(new ApprovalsPage());
	}
}


