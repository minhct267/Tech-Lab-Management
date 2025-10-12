using System.Windows.Controls;

namespace TechLabManagement.Views;

public partial class AccessRequestWizardPage : Page
{
	public AccessRequestWizardPage()
	{
		InitializeComponent();
		this.DataContext = new ViewModels.AccessRequestWizardViewModel();
	}
}


