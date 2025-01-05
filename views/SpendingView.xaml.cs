using System;
using System.Windows;
using System.Windows.Controls;

namespace HCI_PROJEKA_1
{
    public partial class SpendingView : UserControl
    {
        public SpendingView()
        {
            InitializeComponent();
            this.DataContext = new SpendingViewModel();
        }

        private void AddSpendingButton_Click(object sender, RoutedEventArgs e)
        {
            string name = SpendingNameTextBox.Text;
            string category = (SpendingCategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string valueText = SpendingValueTextBox.Text;
            string type = (SpendingModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

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

            if (string.IsNullOrEmpty(category))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo vas da izaberete kategoriju."
                    : "Please select a category.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (string.IsNullOrWhiteSpace(valueText) || !decimal.TryParse(valueText, out decimal value))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo unesite važeći iznos za trošak."
                    : "Please enter a valid amount for the spending.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (string.IsNullOrEmpty(type))
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Molimo vas da izaberete tip plaćanja."
                    : "Please select a payment type.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            if (value < 0)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Iznos troška ne može biti negativan."
                    : "Spending amount cannot be negative.";
                MessageBox.Show(message, errorTitle);
                return;
            }

            var viewModel = DataContext as SpendingViewModel;
            if (viewModel != null)
            {
                viewModel.AddSpending(name, category, value, type);
            }

            SpendingNameTextBox.Clear();
            SpendingValueTextBox.Clear();
            SpendingCategoryComboBox.SelectedIndex = -1;
            SpendingModeComboBox.SelectedIndex = -1;
        }



        private void DeleteSpendingButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var spending = button?.DataContext as SpendingModel;
            var viewModel = DataContext as SpendingViewModel;
            viewModel?.DeleteSpending(spending);
        }

        private void SearchSpendingButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchSpendingTextBox.Text;
            var viewModel = DataContext as SpendingViewModel;
            viewModel?.SearchSpendings(query);
        }

        private void UpdateSpendingButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var spending = button?.DataContext as SpendingModel;

            if (spending == null)
                return;

            var updateWindow = new UpdateSpendingWindow(spending);

            if (updateWindow.ShowDialog() == true)
            {
                var updatedSpending = updateWindow.UpdatedSpending;

                var viewModel = DataContext as SpendingViewModel;
                if (viewModel != null)
                {
                    viewModel.UpdateSpending(spending, updatedSpending);
                }
            }
        }

        private void FilterSpendingButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = SpendingCategoryComboBoxFilter.SelectedItem as ComboBoxItem;

            if (selectedCategory != null)
            {
                string category = selectedCategory.Content.ToString();
                var viewModel = DataContext as SpendingViewModel;
                viewModel?.FilterSpendingsByCategory(category);
            }
            else
            {
                var viewModel = DataContext as SpendingViewModel;
                viewModel?.ResetFilters();
            }
        }


    }
}

