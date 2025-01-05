using System;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class GoalsView : UserControl
    {
        private readonly GoalViewModel _viewModel;

        public GoalsView()
        {
            InitializeComponent();
            _viewModel = new GoalViewModel();
            this.DataContext = _viewModel;
        }

        private void AddGoalButton_Click(object sender, RoutedEventArgs e)
        {
            string errorTitle = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Greška u unosu"
                : "Input Error";

            if (string.IsNullOrWhiteSpace(GoalNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(GoalValueTextBox.Text) ||
                string.IsNullOrWhiteSpace(GoalInstalmentTextBox.Text))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo vas da popunite sva polja."
                    : "Please fill in all fields.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (!decimal.TryParse(GoalValueTextBox.Text, out var targetValue) ||
                !int.TryParse(GoalInstalmentTextBox.Text, out var instalments))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Nevažeći unos za ciljni iznos ili broj rata."
                    : "Invalid input for target value or instalments.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (targetValue < 0 || instalments < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Ciljni iznos i broj rata moraju biti nenegativni."
                    : "Target value and instalments must be non-negative.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            try
            {
                _viewModel.AddGoal(GoalNameTextBox.Text, targetValue, instalments);
                GoalNameTextBox.Clear();
                GoalValueTextBox.Clear();
                GoalInstalmentTextBox.Clear();
            }
            catch (Exception ex)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? $"Greška prilikom dodavanja cilja: {ex.Message}"
                    : $"Error adding goal: {ex.Message}";
                MessageBox.Show(message, errorTitle);
            }
        }

        private void DeleteGoalButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is GoalModel goal)
            {
                var confirmationMessage = LoginViewModel.CurrentLanguage == "Serbian"
                    ? "Da li ste sigurni da želite da obrišete ovaj cilj?"
                    : "Are you sure you want to delete this goal?";

                if (MessageBox.Show(confirmationMessage, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _viewModel.DeleteGoal(goal);
                        ShowMessage("Goal deleted successfully.", "Cilj je uspešno obrisan.");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error deleting goal: {ex.Message}", $"Greška prilikom brisanja cilja: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateGoalButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var goal = button?.DataContext as GoalModel;

            if (goal == null)
                return;

            var updateWindow = new UpdateGoalWindow(goal);

            if (updateWindow.ShowDialog() == true)
            {
                var updatedGoal = updateWindow.UpdatedGoal;
                var viewModel = DataContext as GoalViewModel;
                if (viewModel != null)
                {
                    viewModel.UpdateGoal(goal, updatedGoal);
                }
            }
        }

        private static void ShowMessage(string englishMessage, string serbianMessage)
        {
            string message = LoginViewModel.CurrentLanguage == "Serbian" ? serbianMessage : englishMessage;
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SearchGoalButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchGoalTextBox.Text;
            var viewModel = DataContext as GoalViewModel;
            viewModel?.SearchGoals(query);
        }
    }
}
