using Google.Protobuf.WellKnownTypes;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class SavingsView : UserControl
    {
        public SavingsView()
        {
            InitializeComponent();
            this.DataContext = new SavingsViewModel();
        }

        private void AddSavingButton_Click(object sender, RoutedEventArgs e)
        {
            string name = SavingsNameTextBox.Text;
            string goalText = SavingsGoalTextBox.Text;
            string depositText = SavingsDepositTextBox.Text;

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

            if (string.IsNullOrWhiteSpace(goalText) || !decimal.TryParse(goalText, out decimal goal))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo unesite važeću vrednost cilja."
                    : "Please enter a valid goal value.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (string.IsNullOrWhiteSpace(depositText) || !decimal.TryParse(depositText, out decimal deposit))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo unesite važeću vrednost depozita."
                    : "Please enter a valid deposit value.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (goal < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Cilj ne može biti negativan."
                    : "Goal cannot be negative.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (deposit < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Depozit ne može biti negativan."
                    : "Deposit cannot be negative.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            var viewModel = DataContext as SavingsViewModel;
            if (viewModel != null)
            {
                viewModel.AddSaving(name, deposit, goal);
            }

            SavingsNameTextBox.Clear();
            SavingsGoalTextBox.Clear();
            SavingsDepositTextBox.Clear();
        }



        private void DeleteSavingButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var saving = button?.DataContext as SavingModel;
            var viewModel = DataContext as SavingsViewModel;
            viewModel?.DeleteSaving(saving);
        }

        private void SearchSavingsButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchSavingsTextBox.Text;
            var viewModel = DataContext as SavingsViewModel;
            viewModel?.SearchSavings(query);
        }


        private void UpdateSavingButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var saving = button?.DataContext as SavingModel;

            if (saving == null)
                return;

            var updateWindow = new UpdateSavingsWindow(saving);

            if (updateWindow.ShowDialog() == true)
            {
                var updatedSaving = updateWindow.UpdatedSavings;

                var viewModel = DataContext as SavingsViewModel;
                if (viewModel != null)
                {
                    viewModel.UpdateSaving(saving, updatedSaving);

                }
            }
        }

    }
}
