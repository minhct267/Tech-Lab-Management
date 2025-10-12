using System.Windows.Controls;

namespace TechLabManagement.Views;

public partial class LabsPage : Page
{
	public LabsPage()
	{
		InitializeComponent();
		this.DataContext = new ViewModels.LabsViewModel();
	}
}


