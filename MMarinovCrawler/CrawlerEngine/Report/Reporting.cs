using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace MMarinov.WebCrawler.Report
{
    /// <summary>
    /// Summary description for Reporting
    /// </summary>
    public class Reporting
    {
        public static string REPORT_FILENAME = Preferences.WorkingPath + "\\CrawlingReport_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".htm";
        /*
        public static void WriteReportToDisk(System.Collections.Generic.List<WebPage> internalPages, System.Collections.Generic.List<string> urlsToVisit)
        {
            FileStream fStream = null;
            if (File.Exists(Reporting.REPORT_FILENAME))
            {
                File.Delete(Reporting.REPORT_FILENAME);
                fStream = File.Create(Reporting.REPORT_FILENAME);
            }
            else
            {
                fStream = File.OpenWrite(Reporting.REPORT_FILENAME);
            }

            using (TextWriter writer = new StreamWriter(fStream))
            {
                writer.WriteLine(Reporting.CreateReport(internalPages, urlsToVisit).ToString());
                writer.Flush();
            }

            fStream.Dispose();
        }

        /// <summary>
        /// Creates a report out of the data gathered.
        /// </summary>
        /// <returns></returns>
        public static StringBuilder CreateReport(System.Collections.Generic.List<WebPage> internalPages, System.Collections.Generic.List<string> urlsToVisit)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html><head><title>Crawl Report</title><style>");
            sb.Append("table { border: solid 2px black; border-collapse: collapse; }");
            sb.Append("table tr th { font-weight: bold; padding: 2px; padding-left: 10px; padding-right: 10px; }");
            sb.Append("table tr td { border: solid 1px black; padding: 2px;}");
            sb.Append("h1, h2, p { font-family: Arial; }");
            sb.Append("p { font-family: Arial; font-size: smaller; }");
            sb.Append("h2 { margin-top: 25px; }");
            sb.Append("</style></head><body>");
            sb.Append("<h1>Crawl Report</h1>");

            sb.Append("<h2>Internal Urls - In Order Crawled</h2>");
            sb.Append("<p>These are the pages found within the site. The size is calculated by getting value of the Length of the text of the response text. This is the order in which they were crawled.</p>");

            sb.Append("<table><tr><th>Page Size</th><th>Viewstate Size</th><th>Url</th></tr>");

            foreach (WebPage page in internalPages)
            {
                sb.Append("<tr><td>");
                sb.Append(page.CharSumSize.ToString());
                sb.Append("</td><td>");
                sb.Append(page.Url);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("<h2>External Urls</h2>");
            sb.Append("<p>These are the links to the pages outside the site.</p>");

            sb.Append("<table><tr><th>Url</th></tr>");

            foreach (string str in urlsToVisit)
            {
                sb.Append("<tr><td>");
                sb.Append(str);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            sb.Append("</body></html>");
            return sb;
        }
*/
    }
}