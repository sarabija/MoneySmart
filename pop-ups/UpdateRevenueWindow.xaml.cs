using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class UpdateRevenueWindow : Window
    {
        public RevenueModel UpdatedRevenue { get; private set; }
        private RevenueModel OriginalRevenue { get; set; }

        public UpdateRevenueWindow(RevenueModel revenueToEdit)
        {
            InitializeComponent();

            OriginalRevenue = revenueToEdit;

            RevenueNameTextBox.Text = revenueToEdit.RevenueName;
            RevenueValueTextBox.Text = revenueToEdit.RevenueValue.ToString();
            RevenueTypeComboBox.SelectedItem = RevenueTypeComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == revenueToEdit.RevenueType);
            RevenueModeComboBox.SelectedItem = RevenueModeComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == revenueToEdit.RevenueMode);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string errorTitle = (LoginViewModel.CurrentLanguage == "Serbian")
                ? "Greška u unosu"
                : "Input Error";

            if (string.IsNullOrEmpty(RevenueNameTextBox.Text) ||
                string.IsNullOrEmpty(RevenueValueTextBox.Text) ||
                RevenueTypeComboBox.SelectedItem == null ||
                RevenueModeComboBox.SelectedItem == null)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo vas da popunite sva polja."
                    : "Please fill in all fields.";
                MessageBox.Show(message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(RevenueValueTextBox.Text, out decimal revenueValue) || revenueValue < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo unesite važeći iznos prihoda (ne može biti negativan)."
                    : "Please enter a valid revenue amount (cannot be negative).";
                MessageBox.Show(message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                UpdatedRevenue = new RevenueModel
                {
                    RevenueID = OriginalRevenue.RevenueID,
                    RevenueName = RevenueNameTextBox.Text,
                    RevenueValue = revenueValue,
                    BalanceID = OriginalRevenue.BalanceID,
                    RevenueType = (RevenueTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    RevenueMode = (RevenueModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
                };

                RevenueModel.UpdateRevenue(UpdatedRevenue);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? $"Greška prilikom ažuriranja prihoda: {ex.Message}"
                    : $"Error updating revenue: {ex.Message}";
                MessageBox.Show(message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
