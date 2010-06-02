namespace MMarinov.WebCrawler.Report
{
    /// <summary>
    /// Summary description for Logger
    /// </summary>
    public class Logger
    {
        public static void ErrorLog(System.Exception ex)
        {
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Preferences.WorkingPath + Common.ErrorLog, true))
            {
                try
                {
                    wr.Write(FormatErrorMsg(ex));
                }
                finally
                {
                    wr.Close();
                }
            }
        }

        public static void ErrorLog(string msg, System.Net.WebExceptionStatus exStatus)
        {
            string txtFile = "";

            switch (exStatus)
            {
                case System.Net.WebExceptionStatus.ProtocolError:
                    txtFile = Common.ErrorWebProtocolLog;
                    break;
                case System.Net.WebExceptionStatus.Timeout:
                    txtFile = Common.ErrorWebTimeoutLog;
                    break;
                case System.Net.WebExceptionStatus.Success:
                    txtFile = Common.ErrorLog;
                    break;
                default:
                    txtFile = Common.ErrorWebLog;
                    break;
            }

            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Preferences.WorkingPath + txtFile, true))
            {
                try
                {
                    wr.Write(msg);
                }
                finally
                {
                    wr.Close();
                }
            }
        }

        public static void MessageLog(string msg, Report.EventTypes eventType)
        {
            string txtFile = "";

            switch (eventType)
            {
                case Report.EventTypes.Crawling:
                    txtFile = Common.IndexedLinksLog;
                    break;
                default:
                    txtFile = Common.MessagesLog;
                    break;
            }

            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Preferences.WorkingPath + txtFile, true))
            {
                try
                {
                    wr.Write(FormatMessage(msg));
                }
                finally
                {
                    wr.Close();
                }
            }
        }

        public static string FormatMessage(string msg)
        {
            return System.Environment.NewLine + "--" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "-- " + msg + System.Environment.NewLine;
        }

        public static string FormatErrorMsg(System.Exception ex)
        {
            string str = System.Environment.NewLine + "--" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "-- ";

            while (ex != null)
            {
                str += ex.Message + System.Environment.NewLine + ex.StackTrace;
                ex = ex.InnerException;
            }

            return str + System.Environment.NewLine;
        }
    }
}
