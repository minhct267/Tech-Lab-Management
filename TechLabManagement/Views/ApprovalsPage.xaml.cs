using System.Windows.Controls;
using TechLabManagement.ViewModels;

namespace TechLabManagement.Views;

/// <summary>
/// Interaction logic for ApprovalsPage.xaml
/// </summary>
public partial class ApprovalsPage : Page
{
	public ApprovalsPage()
	{
		InitializeComponent();
		DataContext = new ApprovalsViewModel();
	}
}

