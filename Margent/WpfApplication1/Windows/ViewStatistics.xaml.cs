using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace MMarinov.WebCrawler.UI
{
    /// <summary>
    /// Interaction logic for ViewStatistics.xaml
    /// </summary>
    public partial class ViewStatistics : Window
    {
        public ViewStatistics()
        {
            InitializeComponent();

            DisplayStatistics();
        }

        private void DisplayStatistics()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("StartDate");
            dt.Columns.Add("Duration");
            dt.Columns.Add("Words");
            dt.Columns.Add("FoundValidLinks");
            dt.Columns.Add("FoundTotalLinks");
            dt.Columns.Add("CrawledTotalLinks");
            dt.Columns.Add("CrawledSuccessfulLinks");
            dt.Columns.Add("ProcessDescription");

            foreach (DataColumn dc in dt.Columns)
            {
                Microsoft.Windows.Controls.DataGridTextColumn col = new Microsoft.Windows.Controls.DataGridTextColumn();
                col.Header = dc.Caption;
                col.Binding = new Binding(dc.Caption);
                this.gdStats.Columns.Add(col);
            }

            Library.StatisticsCollectionReadOnly statColl = Library.StatisticsCollectionReadOnly.GetStatisticsCollReadOnly();

            foreach (Library.StatisticsCollectionReadOnly.Statistics stat in statColl)
            {
                dt.Rows.Add(new object[] { stat.StartDate, stat.Duration, stat.Words, stat.FoundValidLinks, stat.FoundTotalLinks, stat.CrawledTotalLinks, stat.CrawledSuccessfulLinks, stat.ProcessDescription.Replace("\r\n", " ") });
            }

            this.gdStats.DataContext = dt;
            this.gdStats.IsReadOnly = true;
        }
    }
}
