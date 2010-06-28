using System;
using System.Linq;

namespace MMarinov.WebCrawler.Indexer
{
    /// <summary>
    /// Summary description for CrawlingManager
    /// </summary>
    public class CrawlingManager
    {
        public static Library.Catalog GlobalCatalog = new Library.Catalog();
        public static volatile bool ShouldStopThreads = false;
        public static volatile int WaitingThreadsCount = 0;

        private Spider[] spiderArray;
        private System.Threading.Timer timer;
        private static string errorMessage = "";
        private static string logMessage = "";
        private static string indexedLinksMessage = "";
        private static string errorWebMessage = "";
        private static string errorWebProtocolMessage = "";
        private static string errorWebTimeoutMessage = "";
        private DateTime startDate;

        /// <summary>
        /// Declaring the Event Handler delegate
        /// </summary>
        public delegate void CrawlerEventHandler(Report.ProgressEventArgs pea);
        public static event CrawlerEventHandler CrawlerEvent;

        public CrawlingManager()
        {

        }

        private static void WriteLog(object o)
        {
            if (errorMessage != "")
            {
                Report.Logger.ErrorLog(errorMessage, System.Net.WebExceptionStatus.Success);
                errorMessage = "";
            }

            if (errorWebMessage != "")
            {
                Report.Logger.ErrorLog(errorWebMessage, System.Net.WebExceptionStatus.UnknownError);
                errorWebMessage = "";
            }

            if (errorWebProtocolMessage != "")
            {
                Report.Logger.ErrorLog(errorWebProtocolMessage, System.Net.WebExceptionStatus.ProtocolError);
                errorWebProtocolMessage = "";
            }

            if (errorWebTimeoutMessage != "")
            {
                Report.Logger.ErrorLog(errorWebTimeoutMessage, System.Net.WebExceptionStatus.Timeout);
                errorWebTimeoutMessage = "";
            }

            if (logMessage != "")
            {
                Report.Logger.MessageLog(logMessage, Report.EventTypes.Other);
                logMessage = "";
            }

            if (indexedLinksMessage != "")
            {
                Report.Logger.MessageLog(indexedLinksMessage, Report.EventTypes.Crawling);
                indexedLinksMessage = "";
            }
        }

        /// <summary>
        /// Number of spider threads to start
        /// </summary>
        /// <param name="p"></param>
        public void StartSpider()
        {
            // SeedList.GetTheList();
            Spider.GlobalURLsToVisit.Add("http://live.com");
            //Spider.GlobalURLsToVisit.Add("http://google.com");
            Spider.GlobalURLsToVisit.Add("http://facebook.com");
            Spider.GlobalURLsToVisit.Add("http://tweeter.com");
            Spider.GlobalURLsToVisit.Add("http://msn.com");
            Spider.GlobalURLsToVisit.Add("http://nike.com");

            ResetFolders();

            timer = new System.Threading.Timer(new System.Threading.TimerCallback(WriteLog), null, 200, 3000);
            startDate = DateTime.Now;

            spiderArray = new Spider[Preferences.ThreadsCount];

            try
            {
                for (int i = 0; i < Preferences.ThreadsCount; i++)
                {
                    spiderArray[i] = new Spider(i);
                    spiderArray[i].SpiderProgressEvent += new MMarinov.WebCrawler.Report.SpiderProgressEventHandler(OnProgressEvent);
                    spiderArray[i].StartThread();
                }
            }
            catch (System.Exception e)
            {
                GetErrorMessageForLog(new System.Exception("Error while creating Threads: ", e));
            }
        }

        private static void GetErrorMessageForLog(System.Exception e)
        {
            if (CrawlerEvent != null)
            {
                Report.ProgressEventArgs pea = new Report.ProgressEventArgs(e);

                CrawlerEvent(pea);

                errorMessage += pea.Message;
            }
        }

        private void ResetFolders()
        {
            try
            {
                System.IO.Directory.Delete(Preferences.TempPath, true);
            }
            catch { }

            if (System.IO.Directory.Exists(Preferences.WorkingPath + Common.ErrorLogsFolder))
            {
                try
                {
                    System.IO.Directory.Delete(Preferences.WorkingPath + Common.ErrorLogsFolder, true);
                }
                catch { }
            }

            try
            {
                System.IO.Directory.CreateDirectory(Preferences.WorkingPath + Common.ErrorLogsFolder);
            }
            catch { }

            if (System.IO.Directory.Exists(Preferences.WorkingPath + Common.MessageLogsFolder))
            {
                try
                {
                    System.IO.Directory.Delete(Preferences.WorkingPath + Common.MessageLogsFolder, true);
                }
                catch { }
            }

            try
            {
                System.IO.Directory.CreateDirectory(Preferences.WorkingPath + Common.MessageLogsFolder);
            }
            catch { }
        }

        private void SendMail(string bodyMsg)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add("m.marinov.de@gmail.com");
                mail.From = new System.Net.Mail.MailAddress("WebCrawler@MMarinovService.com", "WebCrawler Service");
                mail.Subject = "Crawling Report";
                mail.Body = bodyMsg;
                //mail.Attachments.Add(new System.Net.Mail.Attachment(""));
                System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient("localhost");
                sc.Send(mail);
            }
            catch (System.Exception ex)
            {
                GetErrorMessageForLog(new System.Exception("Email Notifier", ex));
            }
        }

        /// <summary>
        /// Events generated by the Spider 
        /// </summary>
        private void OnProgressEvent(Report.ProgressEventArgs pea)
        {
            if (CrawlerEvent != null && pea.EventType != Report.EventTypes.WakeJoinedThreads)
            {
                CrawlerEvent(pea);
            }

            switch (pea.EventType)
            {
                case Report.EventTypes.WakeJoinedThreads:
                    WakeThreads();
                    break;
                case Report.EventTypes.Error:
                    switch (pea.WebExStatus)
                    {
                        case System.Net.WebExceptionStatus.ProtocolError:
                            errorWebProtocolMessage += pea.Message;
                            break;
                        case System.Net.WebExceptionStatus.Timeout:
                            errorWebTimeoutMessage += pea.Message;
                            break;
                        default:
                            errorMessage += pea.Message;
                            break;
                    }
                    break;
                case Report.EventTypes.Crawling:
                    indexedLinksMessage += pea.Message + "\n";
                    break;
                default:
                    logMessage += pea.Message + "\n";
                    break;
            }
        }

        private void WakeThreads()
        {
            if (WaitingThreadsCount > 0)
            {
                foreach (Spider spider in spiderArray)
                {
                    spider.WakeWaitingThead();
                }

                WaitingThreadsCount = 0;
            }
        }

        public void SaveToActiveDB()
        {

        }

        public void StopSpider()
        {
            ShouldStopThreads = true;
            System.Threading.Thread.Sleep(300);

            for (int i = 0; i < Preferences.ThreadsCount; i++)
            {
                spiderArray[i].KillThread();
            }

            ///TODO: save current sites
            ///TODO: save current sites
            ///a.k.a FLUSH

            System.Threading.Thread.Sleep(300);

            timer.Dispose();

            SendMail(SaveStatistics());
        }

        private string SaveStatistics()
        {
            System.Text.StringBuilder statisticMsg = new System.Text.StringBuilder();
            statisticMsg.AppendLine("Report e-mail for status of the crawling process").AppendLine();
            statisticMsg.AppendLine("CrawledSuccessfulLinks = " + Spider.CrawledSuccessfulLinks);
            statisticMsg.AppendLine("CrawledTotalLinks = " + Spider.CrawledTotalLinks);
            statisticMsg.AppendLine("FoundTotalLinks = " + Document.FoundTotalLinks);
            statisticMsg.AppendLine("FoundValidLinks = " + Document.FoundValidLinks);
            statisticMsg.AppendLine("StartDate = " + startDate.ToString(Common.DateFormat));

            System.Text.StringBuilder description = new System.Text.StringBuilder();
            description.AppendLine("IndexOnlyHTMLDocuments = " + Preferences.IndexOnlyHTMLDocuments);
            description.AppendLine("RecursionLimit = " + Preferences.RecursionLimit);
            description.AppendLine("StemmingModeEnabled = " + Preferences.StemmingModeEnabled);
            description.AppendLine("StoppingMode = " + Preferences.StoppingMode);
            description.AppendLine("ThreadsCount = " + Preferences.ThreadsCount);

            TimeSpan duration = (DateTime.Now - startDate);

            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                DALWebCrawler.Statistic stat = new DALWebCrawler.Statistic()
                {
                    CrawledSuccessfulLinks = Spider.CrawledSuccessfulLinks,
                    CrawledTotalLinks = Spider.CrawledTotalLinks,
                    FoundTotalLinks = Document.FoundTotalLinks,
                    FoundValidLinks = Document.FoundValidLinks,
                    StartDate = startDate,
                    Duration = string.Format("{0}d:{1}h:{2}m ({3}min)", duration.Days, duration.Hours.ToString("00"), duration.Minutes.ToString("00"), (int)duration.TotalMinutes),
                    Words = dataContext.Words.Count(),
                    ProcessDescription = description.ToString()
                };

                statisticMsg.AppendLine("Duration = " + stat.Duration);
                statisticMsg.AppendLine("Words = " + stat.Words);

                dataContext.Statistics.InsertOnSubmit(stat);
                dataContext.SubmitChanges();
            }

            statisticMsg.AppendLine().AppendLine("Properties of the crawler:").AppendLine(description.ToString());
            return statisticMsg.ToString();
        }

        public void TruncateDBTables()
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.sp_TruncateAllTables();
            }
        }
    }
}