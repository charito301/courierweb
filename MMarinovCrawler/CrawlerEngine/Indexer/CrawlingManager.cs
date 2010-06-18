using System;

namespace MMarinov.WebCrawler.Indexer
{
    /// <summary>
    /// Summary description for CrawlingManager
    /// </summary>
    public class CrawlingManager
    {
        public static Library.Catalog GlobalCatalog = new Library.Catalog();
        public static volatile bool ShouldStopThreads = false;
        public static volatile bool HasWaitingThread = false;

        private Spider[] spiderArray;
        private System.Threading.Timer timer;
        private static string errorMessage = "";
        private static string logMessage = "";
        private static string indexedLinksMessage = "";
        private static string errorWebMessage = "";
        private static string errorWebProtocolMessage = "";
        private static string errorWebTimeoutMessage = "";

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
            Spider.GlobalURLsToVisit.Add("http://google.com");
            Spider.GlobalURLsToVisit.Add("http://facebook.com");
            Spider.GlobalURLsToVisit.Add("http://tweeter.com");
            Spider.GlobalURLsToVisit.Add("http://msn.com");
            Spider.GlobalURLsToVisit.Add("http://nike.com");

            ResetFolders();

            timer = new System.Threading.Timer(new System.Threading.TimerCallback(WriteLog), null, 200, 3000);

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

        private void SendMail()
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add("mariyan87@gmail.com");
                mail.From = new System.Net.Mail.MailAddress("WebCrawler@Service.com", "WebCrawler Service");
                mail.Subject = "Crawling Report";
                mail.Body = "Crawling Running :" + System.DateTime.Now.ToString();
                //mail.Attachments.Add(new System.Net.Mail.Attachment(""));
                System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient("localhost");
                //sc.Credentials = new System.Net.NetworkCredential("MMarinov", "matrix");
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
                    WakeJoinedThreads();
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

        private void WakeJoinedThreads()
        {
            if (HasWaitingThread)
            {
                foreach (Spider spider in spiderArray)
                {
                    spider.WakeWaitingThead();
                }

                HasWaitingThread = false;
            }
        }

        public void PauseSpiders()
        {
            //join - to finish
            //sleep
        }

        public void StopSpider()
        {
            ShouldStopThreads = true;

            System.Threading.Thread.Sleep(500);

            foreach (Spider spider in spiderArray)
            {
                spider.KillThread();
            }

            System.Threading.Thread.Sleep(500);

            //SendMail();

            timer.Dispose();
        }

        public void DropTheDatabase()
        {
            using (DALWebCrawler.WebCrawlerDataContext dataContext = new DALWebCrawler.WebCrawlerDataContext(Preferences.ConnectionString))
            {
                dataContext.sp_TruncateAllTables();
            }
        }
    }
}