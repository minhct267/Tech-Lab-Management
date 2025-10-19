using System.Windows.Controls;

namespace TechLabManagement.Views;

public partial class MaintenanceSafetyPage : Page
{
	public MaintenanceSafetyPage()
	{
		InitializeComponent();
		this.DataContext = new ViewModels.MaintenanceSafetyViewModel();
	}
}


