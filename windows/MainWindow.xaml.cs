using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace HCI_PROJEKA_1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var dashboardContent = new DashboardView();
            MainContent.Content = dashboardContent;
        }


        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
           
            var dashboardContent = new DashboardView();
            MainContent.Content = dashboardContent;
        }

 
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
           
            var settingsView = new SettingsView();
            settingsView.ExpertModeUnlocked += SettingsView_ExpertModeUnlocked;
            MainContent.Content = settingsView;
        }


        private void RevenuesButton_Click(object sender, RoutedEventArgs e)
        {
            var revenuesContent = new RevenueView(); 
            MainContent.Content = revenuesContent;
        }

        private void SpendingButton_Click(object sender, RoutedEventArgs e)
        {
            var spendingContent = new SpendingView(); 
            MainContent.Content = spendingContent; 
        }

        private void SavingsButton_Click(object sender, RoutedEventArgs e)
        {
            var savingsContent= new SavingsView(); 
            MainContent.Content = savingsContent; 
        }


        private void GoalsButton_Click(object sender, RoutedEventArgs e)
        {
            var goalsContent = new GoalsView(); 
            MainContent.Content = goalsContent; 
        }


        private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
        {
            var analyticsContent= new AnalyticsView();
            MainContent.Content = analyticsContent; 
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
           
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }

        public void SettingsView_ExpertModeUnlocked(object sender, EventArgs e)
        {
           
            btnGoals.Visibility = Visibility.Visible;
            btnAnalytics.Visibility = Visibility.Visible;
        }
        public void SettingsView_ExpertModeUnlocked()
        {
            btnGoals.Visibility = Visibility.Visible;
            btnAnalytics.Visibility = Visibility.Visible;
        }

    }




}
