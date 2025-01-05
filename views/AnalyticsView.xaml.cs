
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace HCI_PROJEKA_1
{

    public partial class AnalyticsView : UserControl
    {
        public AnalyticsView()
        {
            InitializeComponent();
            this.DataContext = new AnalyticsViewModel();


        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
    
            string selectedPeriod = "Last month";
            var viewModel = (AnalyticsViewModel)this.DataContext;
            string pdfFilePath = viewModel.GenerateSpendingRevenueReport(selectedPeriod);

    
            byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
            String name;
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            name = "Report/Izvještaj_" + currentDate;
           ReportModel.SavePdfToDatabase(pdfBytes,name);

        }

        private void DeleteReportButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var report = button?.DataContext as ReportModel;
            var viewModel = DataContext as AnalyticsViewModel;
            viewModel?.DeleteReport(report);
        }

        private void StackPanel_Click(object sender, MouseButtonEventArgs e)
        {

            var clickedStackPanel = sender as StackPanel;

            if (clickedStackPanel != null)
            {
                var report = clickedStackPanel.DataContext as ReportModel;

                if (report != null)
                {
                    var viewModel = (AnalyticsViewModel)this.DataContext;
                    viewModel.getPDF(report.ReportID);
                }
            }
        }

    }
}
