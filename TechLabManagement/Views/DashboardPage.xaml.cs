using System.Windows.Controls;

namespace TechLabManagement.Views;

public partial class DashboardPage : Page
{
	public DashboardPage()
	{
		InitializeComponent();
		this.DataContext = new ViewModels.DashboardViewModel();
	}
}


