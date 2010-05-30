namespace MMarinov.WebCrawler.Report
{
    /// <summary>
    /// Summary description for Logger
    /// </summary>
    public class Logger
    {
        public static void ErrorLog(System.Exception ex)
        {
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Preferences.WorkingPath + "\\WebCrawlerErrorLog.txt", true))
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

        public static void ErrorLog(string msg)
        {
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Preferences.WorkingPath + "\\WebCrawlerErrorLog.txt", true))
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

        public static void ErrorWebLog(string msg)
        {
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Preferences.WorkingPath + "\\WebCrawlerErrorWebLog.txt", true))
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

        public static void MessageLog(string msg)
        {
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Preferences.WorkingPath + "\\WebCrawlerMessageLog.txt", true))
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
            string str = System.Environment.NewLine + "------LOG------" + System.DateTime.Now + "------" + System.Environment.NewLine;
            return str + msg + System.Environment.NewLine + "-------------------------------";
        }

        public static string FormatErrorMsg(System.Exception ex)
        {
            string str = System.Environment.NewLine + "------ERROR------" + System.DateTime.Now + "------" + System.Environment.NewLine;

            while (ex != null)
            {
                str += ex.Message + System.Environment.NewLine + ex.StackTrace;
                ex = ex.InnerException;
            }

            return str + System.Environment.NewLine + "------------------------------------";
        }
    }
}
