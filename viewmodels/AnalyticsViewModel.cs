
using MaterialDesignThemes.Wpf;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace HCI_PROJEKA_1
{
    public class AnalyticsViewModel
    {
        public Dictionary<string, decimal> CategoryTotals { get; set; }
        public Dictionary<string, decimal> CategoryPercentages { get; set; }

        public ObservableCollection<ReportModel> Reports { get; set; }

        public AnalyticsViewModel()
        {
            CategoryTotals = SpendingModel.GetTotalSpendingByCategory(UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername));
            CalculatePercentages();
            int balanceID = UserModel.GetIDForUser(LoginViewModel.LoggedInUsername);
            Reports = new ObservableCollection<ReportModel>(ReportModel.GetAllReports(balanceID));

        }


        public void DeleteReport(ReportModel report)
        {
           ReportModel.DeleteReport(report.ReportID);
           Reports.Remove(report);
        }

        private readonly Dictionary<string, (decimal min, decimal max)> recommendedRanges = new()
        {
            { "Food", (0, 30) },
            { "Rent and utilities", (0, 40) },
            { "Gifts", (0, 10) },
            { "Entertainment", (0, 15) },
            { "Transportation", (0, 15) },
            { "Shopping", (0, 10) },
            { "Miscellaneous", (0, 15) }
        };

        public decimal FoodPercentage => GetCategoryPercentage("Food");
        public decimal RentPercentage => GetCategoryPercentage("Rent and utilities");
        public decimal GiftsPercentage => GetCategoryPercentage("Gifts");
        public decimal EntertainmentPercentage => GetCategoryPercentage("Entertainment");
        public decimal TransportationPercentage => GetCategoryPercentage("Transportation");
        public decimal ShoppingPercentage => GetCategoryPercentage("Shopping");
        public decimal MiscellaneousPercentage => GetCategoryPercentage("Miscellaneous");
        public string FoodAnalyticsText => GetAnalyticsText("Food");
        public string RentAnalyticsText => GetAnalyticsText("Rent and utilities");
        public string GiftsAnalyticsText => GetAnalyticsText("Gifts");
        public string EntertainmentAnalyticsText => GetAnalyticsText("Entertainment");
        public string TransportationAnalyticsText => GetAnalyticsText("Transportation");
        public string ShoppingAnalyticsText => GetAnalyticsText("Shopping");
        public string MiscellaneousAnalyticsText => GetAnalyticsText("Miscellaneous");

     

        private void CalculatePercentages()
        {
            decimal totalSpending = CategoryTotals.Values.Sum();

            if (totalSpending > 0)
            {
                CategoryPercentages = CategoryTotals.ToDictionary(
                    category => category.Key,
                    category => (category.Value / totalSpending) * 100
                );
            }
            else
            {
                CategoryPercentages = new Dictionary<string, decimal>();
            }
        }

        public decimal GetCategoryPercentage(string category)
        {
            category = GetCategoryName(category);
            decimal percentage = CategoryPercentages.ContainsKey(category) ? CategoryPercentages[category] : 0;
            return Math.Round(percentage, 2); 
        }

        private string GetAnalyticsText(string category)
        {
      
            var (min, max) = recommendedRanges[category];
            category = GetCategoryName(category);
            decimal percentage = GetCategoryPercentage(category);

            if (percentage >= min && percentage <= max)
            {
                return (string)App.Current.Resources["analyticsText5"];
            }
            else
            {
                return (string)App.Current.Resources["analyticsText4"]; 
            }
        }

        private string GetCategoryName(string category)
        {
            if (LoginViewModel.CurrentLanguage == "Serbian") 
            {
                switch (category)
                {
                    case "Food": return "Hrana";
                    case "Rent and utilities": return "Kirija i računi";
                    case "Gifts": return "Pokloni";
                    case "Entertainment": return "Zabava";
                    case "Transportation": return "Prevoz";
                    case "Shopping": return "Kupovina";
                    case "Miscellaneous": return "Razno";
                    default: return category;
                }
            }
            else 
            {
                return category;
            }
        }
        public string GenerateSpendingRevenueReport(string timePeriod)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                string language = LoginViewModel.CurrentLanguage;

                if (language == "English")
                {
                    document.Info.Title = "Spending and Revenue Report";
                }
                else
                {
                    document.Info.Title = "Izvještaj o potrošnji i primanjima";
                }

                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("Verdana", 18); 
                XFont regularFont = new XFont("Verdana", 12);

                double pageWidth = page.Width;
                string title = language == "English" ? "Spending and Revenue Report" : "Izvještaj o potrošnji i primanjima";
                double titleWidth = gfx.MeasureString(title, titleFont).Width;
                double titleX = (pageWidth - titleWidth) / 2;
                decimal yPosition = 80; 
                gfx.DrawString(title, titleFont, XBrushes.Black, new XPoint(titleX, 40)); 

                string usernameText = language == "English" ? $"Username: {LoginViewModel.LoggedInUsername}" : $"Korisničko ime: {LoginViewModel.LoggedInUsername}";
                string dateText = language == "English" ? $"Date: {DateTime.Now:yyyy-MM-dd}" : $"Datum: {DateTime.Now:yyyy-MM-dd}";

                gfx.DrawString(usernameText, regularFont, XBrushes.Black, new XPoint(pageWidth - 150, (double)yPosition));
                yPosition += 20;
                gfx.DrawString(dateText, regularFont, XBrushes.Black, new XPoint(pageWidth - 150, (double)yPosition));

                yPosition += 40;

                DateTime startDate = timePeriod switch
                {
                    "Last Month" => DateTime.Now.AddMonths(-1),
                    "Last 6 Months" => DateTime.Now.AddMonths(-6),
                    "Last Year" => DateTime.Now.AddYears(-1),
                    _ => DateTime.Now.AddMonths(-1), 
                };

                var spendingData = GetSpendingData(startDate);
                var revenueData = GetRevenueData(startDate);

                string spendingHeader = language == "English" ? "Spending:" : "Potrošnja:";
                gfx.DrawString(spendingHeader, regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));
                yPosition += 20;

                gfx.DrawString(language == "English" ? "Spending Name" : "Naziv potrošnje", regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));
                gfx.DrawString(language == "English" ? "Category" : "Kategorija", regularFont, XBrushes.Black, new XPoint(200, (double)yPosition));
                gfx.DrawString(language == "English" ? "Value (KM)" : "Vrijednost (KM)", regularFont, XBrushes.Black, new XPoint(350, (double)yPosition));
                yPosition += 20;

                decimal totalSpending = 0;
                foreach (var spending in spendingData)
                {
                    gfx.DrawString(spending.SpendingName, regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));
                    gfx.DrawString(spending.SpendingCategory, regularFont, XBrushes.Black, new XPoint(200, (double)yPosition));
                    gfx.DrawString($"{spending.SpendingValue} KM", regularFont, XBrushes.Black, new XPoint(350, (double)yPosition));
                    yPosition += 20;
                    totalSpending += spending.SpendingValue;
                }

                yPosition += 20;
                string revenueHeader = language == "English" ? "Revenue:" : "Prihodi:";
                gfx.DrawString(revenueHeader, regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));
                yPosition += 20;

                gfx.DrawString(language == "English" ? "Revenue Name" : "Naziv prihoda", regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));
                gfx.DrawString(language == "English" ? "Type" : "Vrsta", regularFont, XBrushes.Black, new XPoint(200, (double)yPosition));
                gfx.DrawString(language == "English" ? "Value (KM)" : "Vrijednost (KM)", regularFont, XBrushes.Black, new XPoint(350, (double)yPosition));
                yPosition += 20;

                decimal totalRevenue = 0;
                foreach (var revenue in revenueData)
                {
                    gfx.DrawString(revenue.RevenueName, regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));
                    gfx.DrawString(revenue.RevenueType, regularFont, XBrushes.Black, new XPoint(200, (double)yPosition));
                    gfx.DrawString($"{revenue.RevenueValue} KM", regularFont, XBrushes.Black, new XPoint(350, (double)yPosition));
                    yPosition += 20;
                    totalRevenue += revenue.RevenueValue;
                }

                yPosition += 20;
                string totalSpendingText = language == "English" ? $"Total Spending: {totalSpending} KM" : $"Ukupna potrošnja: {totalSpending} KM";
                gfx.DrawString(totalSpendingText, regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));
                yPosition += 20;
                string totalRevenueText = language == "English" ? $"Total Revenue: {totalRevenue} KM" : $"Ukupni prihodi: {totalRevenue} KM";
                gfx.DrawString(totalRevenueText, regularFont, XBrushes.Black, new XPoint(50, (double)yPosition));

                string fileName = $"Spending_Revenue_Report_{timePeriod.Replace(" ", "_")}.pdf";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                document.Save(filePath);

                Console.WriteLine($"Report saved to {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating report: {ex.Message}");
                return string.Empty; 
            }
        }


        public void getPDF(int reportId)
        {
            ReportModel.GetPdfFromDatabase(reportId);
        }


        private static IEnumerable<SpendingModel> GetSpendingData(DateTime startDate)
        {
            return SpendingModel.GetAllSpendings(UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername))
                .Where(spending => spending.SpendingDate >= startDate);
        }

        private IEnumerable<RevenueModel> GetRevenueData(DateTime startDate)
        {
            return RevenueModel.GetAllRevenues(UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername))
                .Where(revenue => revenue.RevenueDate >= startDate);
        }

    }
}

