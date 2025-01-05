using HCI_PROJEKA_1;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

public class SpendingViewModel
{
    private ObservableCollection<SpendingModel> AllSpendings { get; set; }
    public ObservableCollection<SpendingModel> Spendings { get; set; }

    public SpendingViewModel()
    {
        int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
        AllSpendings = new ObservableCollection<SpendingModel>(SpendingModel.GetAllSpendings(balanceID));
        Spendings = new ObservableCollection<SpendingModel>(AllSpendings);
    }
    public void AddSpending(string name, string category, decimal value, string type)
    {
        int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
        BalanceModel currentBalance = BalanceModel.GetBalanceByID(balanceID);
 
        if (currentBalance.Cash<value)
        {
           if(type == "Cash")
            {
                MessageBox.Show("Insufficient balance to add this spending.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (type == "Gotovina")
            {
                MessageBox.Show("Nedovljno sredstava za ovaj trošak.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

        }
        if ( currentBalance.Credit < value)
        {
            if (type == "Credit card")
            {
                MessageBox.Show("Insufficient balance to add this spending.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (type == "Kartica")
            {
                MessageBox.Show("Nedovljno sredstava za ovaj trošak.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        var spending = new SpendingModel
        {
            SpendingName = name,
            SpendingCategory = category,
            SpendingValue = value,
            SpendingType = type,
            BalanceID = balanceID
        };

        SpendingModel.AddSpending(spending);

        RefreshSpendings();
        var balanceViewModel = new DashboardViewModel();
        balanceViewModel.RefreshBalance();
    }

    public void UpdateSpending(SpendingModel oldSpending, SpendingModel newSpending)
    {
        int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
        BalanceModel currentBalance = BalanceModel.GetBalanceByID(balanceID);

        decimal valueDifference = newSpending.SpendingValue - oldSpending.SpendingValue;


        if (newSpending.SpendingType == "Cash" || newSpending.SpendingType == "Gotovina")
        {
            if (currentBalance.Cash < valueDifference)
            {

                ShowInsufficientFundsMessage(newSpending.SpendingType);
           
                return; 
            }
        }
        else if (newSpending.SpendingType == "Credit card" || newSpending.SpendingType == "Kartica")
        {
            if (currentBalance.Credit < valueDifference)
            {
                ShowInsufficientFundsMessage(newSpending.SpendingType);
     
                return;
            }
        }

            SpendingModel.UpdateSpending(newSpending);

            int indexInAllSpendings = AllSpendings.IndexOf(oldSpending);
            if (indexInAllSpendings >= 0)
            {
                AllSpendings[indexInAllSpendings] = newSpending;
            }

            int indexInSpendings = Spendings.IndexOf(oldSpending);
            if (indexInSpendings >= 0)
            {
                Spendings[indexInSpendings] = newSpending;
            }
            var balanceViewModel = new DashboardViewModel();
            balanceViewModel.RefreshBalance();

    
    }

    private void ShowInsufficientFundsMessage(string spendingType)
    {
        if (spendingType == "Cash" || spendingType == "Gotovina")
        {
            if (LoginViewModel.CurrentLanguage == "English")
            {
                MessageBox.Show("Insufficient cash balance to update this spending.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (LoginViewModel.CurrentLanguage == "Serbian")
            {
                MessageBox.Show("Nedovoljno gotovinskih sredstava za ažuriranje ovog troška.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        else if (spendingType == "Credit card" || spendingType == "Kartica")
        {
            if (LoginViewModel.CurrentLanguage == "English")
            {
                MessageBox.Show("Insufficient credit balance to update this spending.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (LoginViewModel.CurrentLanguage == "Serbian")
            {
                MessageBox.Show("Nedovoljno kredita za ažuriranje ovog troška.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }



    public void DeleteSpending(SpendingModel spending)
    {
        SpendingModel.DeleteSpending(spending.SpendingID);

        AllSpendings.Remove(spending);
        Spendings.Remove(spending);
    }

    public void SearchSpendings(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Spendings.Clear();
            foreach (var spending in AllSpendings)
            {
                Spendings.Add(spending);
            }
            return;
        }

        var filteredSpendings = AllSpendings
            .Where(s => s.SpendingName != null &&
                        s.SpendingName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();

        Spendings.Clear();
        foreach (var spending in filteredSpendings)
        {
            Spendings.Add(spending);
        }
    }

    private void RefreshSpendings()
    {
        int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);

        AllSpendings.Clear();
        foreach (var spending in SpendingModel.GetAllSpendings(balanceID))
        {
            AllSpendings.Add(spending);
        }

        Spendings.Clear();
        foreach (var spending in AllSpendings)
        {
            Spendings.Add(spending);
        }
    }

    public void FilterSpendingsByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            ResetFilters();
            return;
        }

        var filteredSpendings = AllSpendings
            .Where(s => s.SpendingCategory != null &&
                        s.SpendingCategory.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Spendings.Clear();
        foreach (var spending in filteredSpendings)
        {
            Spendings.Add(spending);
        }
    }

    public void ResetFilters()
    {
        Spendings.Clear();
        foreach (var spending in AllSpendings)
        {
            Spendings.Add(spending);
        }
    }

}
