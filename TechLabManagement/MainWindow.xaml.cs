using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TechLabManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (_, __) => NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            MainFrame.Navigate(new Views.DashboardPage());
        }

        private void DashboardMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void LabsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.LabsPage());
        }

        private void AccessRequestMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.AccessRequestWizardPage());
        }

        private void BookingMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.BookingSchedulerPage());
        }

        private void ApprovalsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.AccessApprovalsPage());
        }

        private void MaintenanceMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.MaintenanceSafetyPage());
        }
    }
}
