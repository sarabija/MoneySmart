using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;

namespace HCI_PROJEKA_1
{
    public partial class DashboardView : UserControl
    {
        private readonly ToolTip tooltip;

        public DashboardView()
        {
            InitializeComponent();
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            this.DataContext = dashboardViewModel;
            Chart.DataContext = dashboardViewModel;

            tooltip = new ToolTip();
            
            PopulatePieChart();
            dashboardViewModel.RefreshTopGoals();
        }

  

        private void PopulatePieChart()
        {
  
            int balanceID = UserModel.GetBalanceIDForUser(LoginViewModel.LoggedInUsername);
            var spendingData = SpendingModel.GetTotalSpendingByCategory(balanceID);

            Chart.Series.Clear();

            foreach (var category in spendingData)
            {
                var pieSeries = new PieSeries
                {
                    Title = category.Key,
                    Values = new ChartValues<decimal> { category.Value },
                    LabelPoint = point => $"{point.Y} KM", 
                    Style = (Style)FindResource($"pieItem{spendingData.Keys.ToList().IndexOf(category.Key) + 1}Style") 
                };

                Chart.Series.Add(pieSeries);
            }
        }

        private void PieChart_DataClick(object sender, ChartPoint chartPoint)
        {
            MessageBox.Show($"You clicked on {chartPoint.SeriesView.Title}: {chartPoint.Y}%", "Slice Clicked");
        }

        private void PieChart_DataHover(object sender, ChartPoint chartPoint)
        {
            if (chartPoint != null)
            {
                var title = chartPoint.SeriesView.Title;
                var value = chartPoint.Y;

                tooltip.Content = $" {title}: {value} KM"; 

                tooltip.IsOpen = true;
                tooltip.PlacementTarget = Chart;
                tooltip.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            }
        }

        private void PieChart_MouseLeave(object sender, MouseEventArgs e)
        {
            if (tooltip.IsOpen)
            {
                tooltip.IsOpen = false;
            }
        }



    }
  


}
