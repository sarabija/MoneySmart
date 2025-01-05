using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HCI_PROJEKA_1
{
    public class RevenueViewModel
    {
       
        public ObservableCollection<RevenueModel> AllRevenues { get; set; }
        public ObservableCollection<RevenueModel> Revenues { get; set; }

        public RevenueViewModel()
        {
            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            AllRevenues = new ObservableCollection<RevenueModel>(RevenueModel.GetAllRevenues(balanceID));
            Revenues = new ObservableCollection<RevenueModel>(AllRevenues);
        }

        public void AddRevenue(string name, decimal value, string type, string mode)
        {
            var revenue = new RevenueModel
            {
                RevenueName = name,
                RevenueValue = value,
                RevenueType = type,
                RevenueMode = mode,
                BalanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername)
            };

            RevenueModel.AddRevenue(revenue);

            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            AllRevenues.Clear();
            foreach (var r in RevenueModel.GetAllRevenues(balanceID))
            {
                AllRevenues.Add(r);
            }

            Revenues.Clear();
            foreach (var r in AllRevenues)
            {
                Revenues.Add(r);
            }

            var balanceViewModel = new DashboardViewModel();
            balanceViewModel.RefreshBalance();
        }

        public void DeleteRevenue(RevenueModel revenue)
        {
            RevenueModel.DeleteRevenue(revenue.RevenueID);
            AllRevenues.Remove(revenue);
            Revenues.Remove(revenue);


            var balanceViewModel = new DashboardViewModel();
            balanceViewModel.RefreshBalance();
        }

        public void UpdateRevenue(RevenueModel oldRevenue, RevenueModel newRevenue)
        {
            RevenueModel.UpdateRevenue(newRevenue);

            int indexInAllRevenues = AllRevenues.IndexOf(oldRevenue);
            if (indexInAllRevenues >= 0)
            {
                AllRevenues[indexInAllRevenues] = newRevenue;
            }

            int indexInRevenues = Revenues.IndexOf(oldRevenue);
            if (indexInRevenues >= 0)
            {
                Revenues[indexInRevenues] = newRevenue;
            }

            var balanceViewModel = new DashboardViewModel();
            balanceViewModel.RefreshBalance();
        }

        public void SearchRevenues(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Revenues.Clear();
                foreach (var revenue in AllRevenues)
                {
                    Revenues.Add(revenue);
                }
                return;
            }

            var filteredRevenues = AllRevenues
                .Where(r => r.RevenueName != null &&
                            r.RevenueName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            Revenues.Clear();
            foreach (var revenue in filteredRevenues)
            {
                Revenues.Add(revenue);
            }
        }
    }
}
