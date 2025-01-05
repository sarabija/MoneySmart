using System;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class UpdateGoalWindow : Window
    {
        public GoalModel UpdatedGoal { get; private set; }
        private GoalModel OriginalGoal { get; set; }

        public UpdateGoalWindow(GoalModel goalToEdit)
        {
            InitializeComponent();

            OriginalGoal = goalToEdit;

            GoalNameTextBox.Text = goalToEdit.GoalName;
            GoalValueTextBox.Text = goalToEdit.GoalValue.ToString();
            GoalInstalmentTextBox.Text = goalToEdit.Instalments.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show(message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(GoalValueTextBox.Text, out var targetValue) ||
                !int.TryParse(GoalInstalmentTextBox.Text, out var instalments))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Nevažeći unos za ciljni iznos ili broj rata."
                    : "Invalid input for target value or instalments.";
                MessageBox.Show(message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (targetValue < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Ciljni iznos ne može biti negativan."
                    : "Target value cannot be negative.";
                MessageBox.Show(message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (instalments < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Broj rata ne može biti negativan."
                    : "Instalments cannot be negative.";
                MessageBox.Show(message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UpdatedGoal = new GoalModel
            {
                GoalID = OriginalGoal.GoalID,
                GoalName = GoalNameTextBox.Text,
                GoalValue = targetValue,
                Instalments = instalments,
                BalanceID = OriginalGoal.BalanceID
            };

            GoalModel.UpdateGoal(UpdatedGoal);

            this.DialogResult = true;
            this.Close();
        }


    }
}
