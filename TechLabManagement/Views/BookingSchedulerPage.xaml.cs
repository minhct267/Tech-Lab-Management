using System.Windows.Controls;

namespace TechLabManagement.Views;

public partial class BookingSchedulerPage : Page
{
	public BookingSchedulerPage()
	{
		InitializeComponent();
		this.DataContext = new ViewModels.BookingSchedulerViewModel();
	}
}


