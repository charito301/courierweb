namespace MMarinov.WebCrawler.Report
{
    /// <summary>
    /// Summary description for Logger
    /// </summary>
    public class Logger
    {
        private static System.IO.StreamWriter swErrors;
        private static System.IO.StreamWriter swMessage;
        private static System.IO.StreamWriter swWebErrors;
        private static System.IO.StreamWriter swTimeoutEx;
        private static System.IO.StreamWriter swProtocolEx;
        private static System.IO.StreamWriter swIndexedWebsites;

        // constructor for static resources
        static Logger()
        {
            OpenFile(Preferences.WorkingPath + Common.ErrorLog, ref swErrors);
            OpenFile(Preferences.WorkingPath + Common.ErrorWebLog, ref  swWebErrors);
            OpenFile(Preferences.WorkingPath + Common.ErrorWebProtocolLog, ref  swProtocolEx);
            OpenFile(Preferences.WorkingPath + Common.ErrorWebTimeoutLog, ref  swTimeoutEx);
            OpenFile(Preferences.WorkingPath + Common.IndexedLinksLog, ref  swIndexedWebsites);
            OpenFile(Preferences.WorkingPath + Common.MessagesLog, ref  swMessage);
        }

        private static void OpenFile(string filename, ref  System.IO.StreamWriter sw)
        {
            // if the file doesn't exist, create it
            if (!System.IO.File.Exists(filename))
            {
                System.IO.FileStream fs = System.IO.File.Create(filename);
                fs.Close();
            }

            // open up the streamwriter for writing..
            sw = System.IO.File.AppendText(filename);
        }

        private static void WriteToFile(string message, System.IO.StreamWriter sw)
        {
            try
            {
                lock (sw)
                {
                    sw.Write(message);
                    sw.Flush();
                }
            }
            catch { }
        }

        public static void ErrorLog(System.Exception ex)
        {
            WriteToFile(FormatErrorMsg(ex), swErrors);
        }

        public static void ErrorLog(string msg, System.Net.WebExceptionStatus exStatus)
        {
            switch (exStatus)
            {
                case System.Net.WebExceptionStatus.ProtocolError:
                    WriteToFile(msg, swProtocolEx);
                    break;
                case System.Net.WebExceptionStatus.Timeout:
                    WriteToFile(msg, swTimeoutEx);
                    break;
                case System.Net.WebExceptionStatus.Success:
                    WriteToFile(msg, swErrors);
                    break;
                default:
                    WriteToFile(msg, swWebErrors);
                    break;
            }
        }

        public static void MessageLog(string msg, Report.EventTypes eventType)
        {
            switch (eventType)
            {
                case Report.EventTypes.Crawling:
                    WriteToFile(msg, swIndexedWebsites);
                    break;
                default:
                    WriteToFile(msg, swMessage);
                    break;
            }
        }

        public static string FormatMessage(string msg)
        {
            return System.Environment.NewLine + "--" + System.DateTime.Now.ToString(Common.DateFormat) + "-- " + msg + System.Environment.NewLine;
        }

        public static string FormatErrorMsg(System.Exception ex)
        {
            string str = System.Environment.NewLine + "--" + System.DateTime.Now.ToString(Common.DateFormat) + "-- ";

            while (ex != null)
            {
                str += ex.Message + System.Environment.NewLine + ex.StackTrace;
                ex = ex.InnerException;
            }

            return str + System.Environment.NewLine;
        }
    }
}
