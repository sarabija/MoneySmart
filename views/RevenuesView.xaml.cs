using System;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class RevenueView : UserControl
    {
        public RevenueView()
        {
            InitializeComponent();
            this.DataContext = new RevenueViewModel();
        }

        private void AddRevenueButton_Click(object sender, RoutedEventArgs e)
        {
            string name = RevenueNameTextBox.Text;
            string type = (RevenueTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string mode = (RevenueModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string valueText = RevenueValueTextBox.Text;

            string errorTitle = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Greška u unosu"
                : "Input Error";

            if (string.IsNullOrWhiteSpace(name))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Naziv ne može biti prazan."
                    : "Name cannot be empty.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (string.IsNullOrEmpty(type))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo vas da izaberete tip prihoda."
                    : "Please select a revenue type.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (string.IsNullOrEmpty(mode))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo vas da izaberete način prihoda."
                    : "Please select a revenue mode.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (string.IsNullOrWhiteSpace(valueText) || !decimal.TryParse(valueText, out decimal value))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo unesite važeći iznos prihoda."
                    : "Please enter a valid amount for the revenue.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (value < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Iznos prihoda ne može biti negativan."
                    : "Revenue amount cannot be negative.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            var viewModel = DataContext as RevenueViewModel;
            if (viewModel != null)
            {
                viewModel.AddRevenue(name, value, type, mode);
            }

            RevenueNameTextBox.Clear();
            RevenueValueTextBox.Clear();
            RevenueTypeComboBox.SelectedIndex = -1;
            RevenueModeComboBox.SelectedIndex = -1;
        }



        private void DeleteRevenueButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var revenue = button?.DataContext as RevenueModel;
            var viewModel = DataContext as RevenueViewModel;
            viewModel?.DeleteRevenue(revenue);
        }

        private void UpdateRevenueButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var revenue = button?.DataContext as RevenueModel;

            if (revenue == null)
                return;

            var updateWindow = new UpdateRevenueWindow(revenue);

            if (updateWindow.ShowDialog() == true)
            {
                var updatedRevenue = updateWindow.UpdatedRevenue;

                var viewModel = DataContext as RevenueViewModel;
                if (viewModel != null)
                {
                    viewModel.UpdateRevenue(revenue, updatedRevenue);
                }
            }
        }

        private void SearchRevenueButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchRevenueTextBox.Text;
            var viewModel = DataContext as RevenueViewModel;
            viewModel?.SearchRevenues(query);
        }

    }
}

