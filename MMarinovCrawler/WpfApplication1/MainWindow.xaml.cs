using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMarinov.WebCrawler.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MMarinov.WebCrawler.Indexer.CrawlingManager manager = null;
        private System.Threading.Timer timer;
        private Int64 elapsedSec = 0;

        public MainWindow()
        {
            InitializeComponent();

            btnStop.IsEnabled = false;

            manager = new MMarinov.WebCrawler.Indexer.CrawlingManager();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            tbCrawlerDomains.Text = "";
            tbErrors.Text = "";
            tbIndexedLinks.Text = "";
            tbProtocolEx.Text = "";
            tbTimeoutEx.Text = "";
            tbWebEx.Text = "";

            if (MessageBox.Show("This procees will override the database, if such exists!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                Cursor = Cursors.Wait;
                lblStartTime.Content = "Starting...";

                manager.DropTheDatabase();
            }           
           
            elapsedSec = 0;
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(ShowElapsedTime), null, 0, 1000);

            manager.StartSpider();
            Indexer.CrawlingManager.CrawlerEvent += new Indexer.CrawlingManager.CrawlerEventHandler(CrawlingManager_CrawlerEvent);

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;

            lblStartTime.Content = "Started on " + DateTime.Now.ToString(Common.DateFormat);
            Cursor = Cursors.Arrow;
        }

        private void ShowElapsedTime(object o)
        {
            lblTimeElapsed.Dispatcher.Invoke(
                   System.Windows.Threading.DispatcherPriority.Normal,
                   (Action)(() =>
                   { lblTimeElapsed.Content = "Elapsed:" + TimeSpan.FromSeconds(++elapsedSec); }));
        }

        private void CrawlingManager_CrawlerEvent(Report.ProgressEventArgs pea)
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
                        case System.Net.WebExceptionStatus.Success:
                            AddMessageToTextbox(pea, tbErrors);
                            break;
                        default:
                            AddMessageToTextbox(pea, tbWebEx);
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

            lblTotalFoundLinks.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, TimeSpan.FromMilliseconds(100),
            (Action)(() => { lblTotalFoundLinks.Content = "Total links found:" + MMarinov.WebCrawler.Indexer.Document.FoundTotalLinks; }));

            lblTotalValidLinksFound.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, TimeSpan.FromMilliseconds(100),
            (Action)(() => { lblTotalValidLinksFound.Content = "Total valid links found:" + MMarinov.WebCrawler.Indexer.Document.FoundValidLinks; }));
        }

        private void AddMessageToTextbox(Report.ProgressEventArgs pea, TextBox tbx)
        {
            tbx.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, TimeSpan.FromMilliseconds(100),
            (Action)(() =>
            {
                if (tbx.LineCount > 500)
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

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            manager.PauseSpiders();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            //TODO shutdown at all
        }
    }
}
