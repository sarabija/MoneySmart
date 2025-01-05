using LiveCharts.Wpf;
using LiveCharts;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace HCI_PROJEKA_1
{
    public class DashboardViewModel
    {
      
        public BalanceModel CurrentBalance { get; private set; }
        public List<SavingModel> TopGoals { get; private set; }
        public List<MonthlyRevenue> MonthlyRevenues { get; private set; }

        public DashboardViewModel()
        {
            LoadBalance();
            LoadTopSavingsGoals();
            LoadMonthlyRevenues();
        }

        public void LoadBalance()
        {
            CurrentBalance = BalanceModel.GetBalanceByID(UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername));
        }

        public void RefreshBalance()
        {
            LoadBalance();
        }
        public void LoadTopSavingsGoals()
        {
    
            List<SavingModel> savings = SavingModel.GetAllSavings(UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername));

            TopGoals = savings.OrderByDescending(s => s.Percentage)
                              .Take(4)  
                              .ToList();

            while (TopGoals.Count < 4)
            {
                string sN;
                if(LoginViewModel.CurrentLanguage=="Serbian")
                {
                    sN = "Cilj jos nije postavljen";
                }
                else
                {
                    sN = "Goal not set yet";
                }
   
                TopGoals.Add(new SavingModel
                {
                    SavingName = sN,
                });
            }
        }

        public void RefreshTopGoals()
        {
            LoadTopSavingsGoals();
        }

        public void LoadMonthlyRevenues()
        {
            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            int currentYear = DateTime.Now.Year;

            MonthlyRevenues = RevenueModel.GetMonthlyRevenues(balanceID, currentYear);
        }

        public ChartValues<decimal> GetRevenueByMonth
        {
            get
            {
                var revenueByMonth = new ChartValues<decimal>();

                var monthlyRevenueList = new decimal[12];

                foreach (var revenue in MonthlyRevenues)
                {
                    int monthIndex = revenue.Month - 1;  
                    decimal totalRevenueForMonth = revenue.RegularRevenue + revenue.OneTimeRevenue + revenue.AnnualAverageRevenue;
                    monthlyRevenueList[monthIndex] = totalRevenueForMonth;
                }

                foreach (var revenue in monthlyRevenueList)
                {
                    revenueByMonth.Add(revenue);
                }

                return revenueByMonth;
            }
        }



    }
}
