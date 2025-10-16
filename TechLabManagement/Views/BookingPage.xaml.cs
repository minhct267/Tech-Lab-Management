using System.Windows.Controls;
using TechLabManagement.ViewModels;

namespace TechLabManagement.Views;

/// <summary>
/// Interaction logic for BookingPage.xaml
/// </summary>
public partial class BookingPage : Page
{
	public BookingPage()
	{
		InitializeComponent();
		DataContext = new BookingViewModel();
	}
}

