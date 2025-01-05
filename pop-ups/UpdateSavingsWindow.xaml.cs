using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class UpdateSavingsWindow : Window
    {
        public SavingModel UpdatedSavings { get; private set; }
        private readonly SavingModel _originalSavings;

        public UpdateSavingsWindow(SavingModel savingsToEdit)
        {
            InitializeComponent();

            _originalSavings = savingsToEdit;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string errorTitle = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Greška u unosu"
                : "Input Error";

            string invalidDepositMessage = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Nevažeća vrednost depozita. Molimo unesite validan broj."
                : "Invalid deposit value. Please enter a valid number.";

            string negativeDepositMessage = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Depozit ne može biti negativan. Molimo unesite validan broj."
                : "Deposit cannot be negative. Please enter a valid number.";

            if (!double.TryParse(SavingsDepositTextBox.Text, out double newDeposit))
            {
                MessageBox.Show(invalidDepositMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (newDeposit < 0)
            {
                MessageBox.Show(negativeDepositMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                decimal updatedDeposit = _originalSavings.SavingDeposit + (decimal)newDeposit;

                UpdatedSavings = new SavingModel
                {
                    SavingName = _originalSavings.SavingName,
                    SavingGoal = _originalSavings.SavingGoal,
                    SavingDeposit = updatedDeposit,
                    BalanceID = _originalSavings.BalanceID,
                    SavingID = _originalSavings.SavingID
                };

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                string exceptionMessage = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? $"Greška prilikom ažuriranja štednje: {ex.Message}"
                    : $"Error updating savings: {ex.Message}";
                MessageBox.Show(exceptionMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
