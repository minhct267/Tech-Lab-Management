using System.Windows.Controls;

namespace TechLabManagement.Views;

public partial class AccessApprovalsPage : Page
{
	public AccessApprovalsPage()
	{
		InitializeComponent();
		this.DataContext = new ViewModels.AccessApprovalsViewModel();
	}
}


