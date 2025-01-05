using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class UpdateSpendingWindow : Window
    {
        public SpendingModel UpdatedSpending { get; private set; }
        private SpendingModel OriginalSpending { get; set; }

        public UpdateSpendingWindow(SpendingModel spendingToEdit)
        {
            InitializeComponent();

            OriginalSpending = spendingToEdit;

            SpendingNameTextBox.Text = spendingToEdit.SpendingName;
            SpendingValueTextBox.Text = spendingToEdit.SpendingValue.ToString();
            SpendingCategoryComboBox.SelectedItem = SpendingCategoryComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == spendingToEdit.SpendingCategory);
            SpendingModeComboBox.SelectedItem = SpendingModeComboBox.Items
                .Cast<ComboBoxItem>()
                 .FirstOrDefault(item => item.Content.ToString() == spendingToEdit.SpendingType);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string errorTitle = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Greška u unosu"
                : "Input Error";

            string emptyFieldsMessage = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Molimo vas da popunite sva polja."
                : "Please fill in all fields.";

            string invalidAmountMessage = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Molimo unesite važeći iznos za trošak. Iznos ne može biti negativan."
                : "Please enter a valid amount for the spending. The amount cannot be negative.";

            if (string.IsNullOrEmpty(SpendingNameTextBox.Text) ||
                string.IsNullOrEmpty(SpendingValueTextBox.Text) ||
                SpendingCategoryComboBox.SelectedItem == null ||
                SpendingModeComboBox.SelectedItem == null)
            {
                MessageBox.Show(emptyFieldsMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(SpendingValueTextBox.Text, out decimal value) || value < 0)
            {
                MessageBox.Show(invalidAmountMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UpdatedSpending = new SpendingModel
                {
                    SpendingID = OriginalSpending.SpendingID,
                    SpendingName = SpendingNameTextBox.Text,
                    SpendingValue = value,
                    BalanceID = OriginalSpending.BalanceID,
                    SpendingCategory = (SpendingCategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    SpendingType = (SpendingModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
                };

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                string exceptionMessage = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? $"Došlo je do greške prilikom čuvanja troška: {ex.Message}"
                    : $"An error occurred while saving the spending: {ex.Message}";

                MessageBox.Show(exceptionMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
