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
using TechLabManagement.Services;

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
            Loaded += OnLoaded;
            ServiceLocator.Current.Auth.CurrentUserChanged += OnCurrentUserChanged;
        }

        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            EnsureLoggedIn();
            NavigateToDashboard();
        }

        private void EnsureLoggedIn()
        {
            var svc = ServiceLocator.Current;
            if (svc.Auth.CurrentUser == null)
            {
                var login = new Views.LoginWindow();
                var result = login.ShowDialog();
                if (result != true)
                {
                    Close();
                }
            }
            UpdateAccountUi();
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
            MainFrame.Navigate(new Views.BookingPage());
        }

        private void AnalyticsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.AnalyticsPage());
        }

        private void ApprovalsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Views.ApprovalsPage());
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Tech Lab Management System\n\n" +
                "Version: 1.0\n\n" +
                "A comprehensive system for managing university tech lab access, bookings, and approvals.\n\n" +
                "Features:\n" +
                "• Lab & Equipment Management\n" +
                "• Access Request with Induction Tests\n" +
                "• Resource Booking & Conflict Detection\n" +
                "• Request Approval Workflow\n\n" +
                "© 2024 Tech Lab Management",
                "About",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void HelpMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Quick Start Guide:\n\n" +
                "1. Dashboard - View system statistics and quick actions\n" +
                "2. Labs - Browse available labs and equipment\n" +
                "3. Access Request - Submit access requests with induction test\n" +
                "4. Booking - Schedule lab/equipment usage\n" +
                "5. Approvals - Review and approve pending requests\n\n" +
                "Navigation:\n" +
                "• Use the menu bar at the top\n" +
                "• Click quick action buttons on Dashboard\n" +
                "• Press Alt + underlined letter for shortcuts\n\n" +
                "For more help, contact the lab administrator.",
                "User Guide",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void LoginMenu_Click(object sender, RoutedEventArgs e)
        {
            var login = new Views.LoginWindow();
            login.ShowDialog();
        }

        private void LogoutMenu_Click(object sender, RoutedEventArgs e)
        {
            var svc = ServiceLocator.Current;
            svc.Auth.Logout();
            EnsureLoggedIn();
            NavigateToDashboard();
        }

        private void OnCurrentUserChanged(object? sender, TechLabManagement.Core.Models.User? e)
        {
            Dispatcher.Invoke(UpdateAccountUi);
        }

        private void UpdateAccountUi()
        {
            var svc = ServiceLocator.Current;
            var user = svc.Auth.CurrentUser;
            if (user == null) return;
            AccountMenuItem.Header = $"👤 {user.Name} ({user.Role})";
            var canApprove = svc.Authorization.HasPermission(TechLabManagement.Core.Models.Permission.ApproveAccessRequest);
            ApprovalsMenuItem.Visibility = canApprove ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
