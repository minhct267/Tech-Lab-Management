using System.Windows.Controls;
using TechLabManagement.ViewModels;

namespace TechLabManagement.Views;

public partial class AnalyticsPage : Page
{
	public AnalyticsPage()
	{
		InitializeComponent();
		DataContext = new AnalyticsViewModel();
	}
}


