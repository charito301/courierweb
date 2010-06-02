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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MMarinov.WebCrawler.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MMarinov.WebCrawler.Indexer.CrawlingManager manager = null;
        private System.Threading.Timer timer;
        private string errorMessage = "";
        private string logMessage = "";
        private Int64 elapsedSec = 0;

        public MainWindow()
        {
            InitializeComponent();
            btnStop.IsEnabled = false;

            manager = new MMarinov.WebCrawler.Indexer.CrawlingManager();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            lblStartTime.Content = "Starting...";
            elapsedSec = 0;
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(ShowElapsedTime), null, 0, 1000);

            manager.StartSpider();
            Indexer.CrawlingManager.CrawlerEvent += new Indexer.CrawlingManager.CrawlerEventHandler(CrawlingManager_CrawlerEvent);

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;

            lblStartTime.Content = "Started on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Cursor = Cursors.Arrow;
        }

        private void ShowElapsedTime(object o)
        {
            lblTimeElapsed.Dispatcher.Invoke(
                   System.Windows.Threading.DispatcherPriority.Normal,
                   (Action)(() =>
                   { lblTimeElapsed.Content = "Elapsed:" + TimeSpan.FromSeconds(++elapsedSec); }));
        }

        void CrawlingManager_CrawlerEvent(Report.ProgressEventArgs pea)
        {
            switch (pea.EventType)
            {
                case Report.EventTypes.Error:
                    switch (pea.WebExStatus)
                    {
                        case System.Net.WebExceptionStatus.ProtocolError:
                            AddMessageToTextbox(pea, tbProtocolEx);
                            break;
                        case System.Net.WebExceptionStatus.Timeout:
                            AddMessageToTextbox(pea, tbTimeoutEx);
                            break;
                        default:
                            AddMessageToTextbox(pea, tbErrors);
                            break;
                    }
                    break;
                case Report.EventTypes.Start:
                case Report.EventTypes.End:
                    AddMessageToTextbox(pea, tbCrawlerDomains);
                    break;
                case Report.EventTypes.Crawling:
                default:
                    AddMessageToTextbox(pea, tbIndexedLinks);
                    break;
            }
        }

        private void AddMessageToTextbox(Report.ProgressEventArgs pea, TextBox tbx)
        {
            tbx.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (Action)(() =>
            {
                if (tbx.LineCount > 5000)
                {
                    tbx.Text = "";
                }

                tbx.Text = pea.Message + Environment.NewLine + tbx.Text;
            }));
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            manager.StopSpider();

            timer.Dispose();

            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;
            Cursor = Cursors.Arrow;
        }
    }
}
