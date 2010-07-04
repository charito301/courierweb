using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace MMarinov.WebCrawler.UI
{
    public partial class _Default : System.Web.UI.Page
    {
        private MMarinov.WebCrawler.Indexer.CrawlingManager manager = null;
        private Timer timerRefresh = new Timer();
        private string errorMessage = "";
        private string logMessage = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            btnStopCrawl.Enabled = false;

            timerRefresh.Interval = 3000;
            timerRefresh.Tick += new EventHandler<EventArgs>(timerRefresh_Tick);

            manager = new MMarinov.WebCrawler.Indexer.CrawlingManager();
        }

        void timerRefresh_Tick(object sender, EventArgs e)
        {
            ////don't work
            //if (errorMessage != "")
            //{
            //    Report.Logger.ErrorLog(new Exception(errorMessage));
            //    errorMessage = "";
            //}

            //if (logMessage != "")
            //{
            //    Report.Logger.MessageLog(logMessage);
            //    logMessage = "";
            //}
        }

        protected void btnCrawl_Click(object sender, EventArgs e)
        {
            manager.StartSpider();
            MMarinov.WebCrawler.Indexer.CrawlingManager.CrawlerEvent += new MMarinov.WebCrawler.Indexer.CrawlingManager.CrawlerEventHandler(CrawlingManager_CrawlerEvent);

            btnCrawl.Enabled = false;
            btnStopCrawl.Enabled = true;
        }

        void CrawlingManager_CrawlerEvent(Report.ProgressEventArgs pea)
        {
            //tbLog.WordName += message;
            //UpdatePanel1.Update();

            if (pea.EventType == MMarinov.WebCrawler.Report.EventTypes.Error)
            {
                errorMessage += pea.Message;
            }
            else
            {
                logMessage += pea.Message;
            }
        }

        protected void btnStopCrawl_Click(object sender, EventArgs e)
        {
            manager.StopSpiders();

            btnStopCrawl.Enabled = false;
            btnCrawl.Enabled = true;
        }
    }
}