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
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            tbCrawlerDomains.Text = "";
            tbErrors.Text = "";
            tbIndexedLinks.Text = "";
            tbProtocolEx.Text = "";
            tbTimeoutEx.Text = "";
            tbWebEx.Text = "";

            btnStop.IsEnabled = true;
            btnStart.IsEnabled = !btnStop.IsEnabled;
            btnSaveToDB.IsEnabled = !btnStop.IsEnabled;           
        

            ProgressDialog dlg = new ProgressDialog("This procees will override the database! Truncating tables .. ", this, true);
            dlg.RunWorkerThread(StartCrawling);

            elapsedSec = 0;
            timer = new System.Threading.Timer(new System.Threading.TimerCallback(ShowElapsedTimeAndDLSpeed), null, 0, 1000);
  
            lblStatus.Text = "Crawling...";
            lblStartTime.Content = "Started on " + DateTime.Now.ToString(Common.DateFormat);

            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// This method will be invoked on a background thread by the ProgressDialog control.
        /// </summary>
        /// <param name="sender">The background worker component</param>
        /// <param name="e">Provides the start value as the argument.</param>
        private void StartCrawling(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //the sender property is a reference to the dialog's BackgroundWorker component
            System.ComponentModel.BackgroundWorker worker = (System.ComponentModel.BackgroundWorker)sender;

            System.Threading.Thread.Sleep(600);
            Library.DBCopier.TruncateDBTables();

            worker.ReportProgress(30, "Initializing crawling process...");

            manager = new MMarinov.WebCrawler.Indexer.CrawlingManager();           
            Indexer.CrawlingManager.CrawlerEvent += new Indexer.CrawlingManager.CrawlerEventHandler(CrawlingManager_CrawlerEvent);
            manager.StartSpider();

            worker.ReportProgress(70, "Loading the seed list...");
            System.Threading.Thread.Sleep(800);
            worker.ReportProgress(100);
        }

        private void ShowElapsedTimeAndDLSpeed(object o)
        {
            lblTimeElapsed.Dispatcher.BeginInvoke(
                   System.Windows.Threading.DispatcherPriority.Normal,
                   (Action)(() =>
                   { lblTimeElapsed.Content = "Elapsed: " + TimeSpan.FromSeconds(++elapsedSec); }));

            lblDlSpeed.Dispatcher.BeginInvoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  (Action)(() =>
                  { lblDlSpeed.Content = "Download speed: " + manager.DownloadSpeed.ToString("######0.00") + " KB/s"; }));
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

            lblTotalFoundLinks.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, 
            (Action)(() => { lblTotalFoundLinks.Content = "Total links found:" + MMarinov.WebCrawler.Indexer.Document.FoundTotalLinks; }));

            lblTotalValidLinksFound.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, 
            (Action)(() => { lblTotalValidLinksFound.Content = "Total valid links found:" + MMarinov.WebCrawler.Indexer.Document.FoundValidLinks; }));
        }

        private void AddMessageToTextbox(Report.ProgressEventArgs pea, TextBox tbx)
        {
            tbx.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Normal, 
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

            ProgressDialog dlg = new ProgressDialog("Finalizing crawling process.. ", this, true);
            dlg.RunWorkerThread(StopCrawling);

            btnStop.IsEnabled = false;
            btnStart.IsEnabled = !btnStop.IsEnabled;
            btnSaveToDB.IsEnabled = !btnStop.IsEnabled;
            lblStatus.Text = "Stopped.";

            Cursor = Cursors.Arrow;
        }

        private void StopCrawling(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //the sender property is a reference to the dialog's BackgroundWorker component
            System.ComponentModel.BackgroundWorker worker = (System.ComponentModel.BackgroundWorker)sender;

            manager.StopSpiders();
            timer.Dispose();

            worker.ReportProgress(70, "Saving statistics ...");
            System.Threading.Thread.Sleep(1000);
            worker.ReportProgress(100);
        }

        private void btnCopyToActiveDB_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            lblStatus.Text = "Coping DB to active DB, used of the web client...";

            ProgressDialog dlg = new ProgressDialog("Coping DB to active DB, used of the web client...", this, true);
            dlg.RunWorkerThread(CopyDatabase);

            lblStatus.Text = "Saved to Active Database";
            Cursor = Cursors.Arrow;
        }

        private void CopyDatabase(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //the sender property is a reference to the dialog's BackgroundWorker component
            System.ComponentModel.BackgroundWorker worker = (System.ComponentModel.BackgroundWorker)sender;

            Library.DBCopier.CopyDBToActiveDB();

            worker.ReportProgress(100);
        }

        #region Closing

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!btnStart.IsEnabled)
            {
                if (!closeCompleted && MessageBox.Show("Crawling process in progress. Are you sure you want to terminate it?", "Exit application?", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    if (!closeCompleted)
                    {
                        manager.KillSpiders();

                        e.Cancel = true;
                        FormFadeOut.Begin();
                    }
                }
            }
            else if (!closeCompleted)
            {
                e.Cancel = true;
                FormFadeOut.Begin();
            }
        }

        private bool closeCompleted = false;

        private void FormFadeOut_Completed_1(object sender, EventArgs e)
        {
            closeCompleted = true;
            Application.Current.Shutdown();//killin' me softly
        }

        #endregion

        private void btnShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            ViewStatistics stats = new ViewStatistics();
            stats.WindowState = WindowState.Maximized;
            stats.Show();
        }
    }
}
