using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HCI_PROJEKA_1
{
    public class SavingsViewModel
    {
        private ObservableCollection<SavingModel> AllSavings { get; set; }
        public ObservableCollection<SavingModel> Savings { get; set; }

        public SavingsViewModel()
        {
            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            AllSavings = new ObservableCollection<SavingModel>(SavingModel.GetAllSavings(balanceID));
            Savings = new ObservableCollection<SavingModel>(AllSavings);
        }

        public void SearchSavings(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Savings.Clear();
                foreach (var saving in AllSavings)
                {
                    Savings.Add(saving);
                }
                return;
            }

            var filteredSavings = AllSavings
                .Where(s => s.SavingName != null &&
                            s.SavingName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            Savings.Clear();
            foreach (var saving in filteredSavings)
            {
                Savings.Add(saving);
            }
        }

        public void AddSaving(string name, decimal deposit, decimal goal)
        {

            BalanceModel currentBalance = BalanceModel.GetBalanceByID(UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername));


            if (currentBalance.Cash <deposit)
            {
                if (LoginViewModel.CurrentLanguage == "English")
                {
                    MessageBox.Show("Insufficient balance to add this spending.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (LoginViewModel.CurrentLanguage == "Serbian")
                {
                    MessageBox.Show("Nedovljno sredstava za ovaj trošak.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }

            var saving = new SavingModel
            {
                SavingName = name,
                SavingDeposit = deposit,
                SavingGoal = goal,
                BalanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername)
            };

            SavingModel.AddSaving(saving);
            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            Savings.Clear();
            var allSavings = SavingModel.GetAllSavings(balanceID);
            foreach (var s in allSavings)
            {
                Savings.Add(s);
            }
        }

        public void DeleteSaving(SavingModel saving)
        {
            SavingModel.DeleteSaving(saving.SavingID);
            Savings.Remove(saving);
        }

        public void UpdateSaving(SavingModel oldSaving, SavingModel newSaving)
        {
            BalanceModel currentBalance = BalanceModel.GetBalanceByID(UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername));

            decimal depositDifference = newSaving.SavingDeposit - oldSaving.SavingDeposit;

            if (currentBalance.Cash < depositDifference)
            {
                if (LoginViewModel.CurrentLanguage == "English")
                {
                    MessageBox.Show("Insufficient balance to update this saving.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (LoginViewModel.CurrentLanguage == "Serbian")
                {
                    MessageBox.Show("Nedovoljno sredstava za ažuriranje ove štednje.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return;
            }

            if (newSaving.SavingDeposit >= newSaving.SavingGoal)
            {
                string message = (LoginViewModel.CurrentLanguage == "Serbian")
                    ? "Čestitamo! Postigli ste cilj štednje."
                    : "Congratulations! You have reached your savings goal.";
                MessageBox.Show(message, "Goal Reached");
            }
            SavingModel.UpdateSaving(newSaving);

            int index = Savings.IndexOf(oldSaving);
            if (index >= 0)
            {
                Savings[index] = newSaving;
            }
        }

    }
}
