using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;
using System.Net.Mail;
using System.Threading;
using System.IO;

namespace MMarinov.WebCrawler.UI
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //Set language
            if (Request.QueryString["Language"] == "DE")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de");
            }
            else if (Request.QueryString["Language"] == "BG")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("bg");
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }

            gvKeywords.RowDataBound += new GridViewRowEventHandler(gvKeywords_RowDataBound);

            //Create an instance of a javascript class.
            string javaScriptInstance = "<script type='text/javascript'>" + this.JSVariableName + " = new WebCrawlerJS();</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "MakeAnInstance", javaScriptInstance, false);
        }
        void gvKeywords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
          
        }


        protected void btnDoSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (tbSearchQuery.Text.Trim() == "")
            {
                return;
            }

            LoadDataSource(tbSearchQuery.Text.Trim().ToLower());
        }

        private void LoadDataSource(string query)
        {
            DataFetcher fetcher = new DataFetcher(query);
            
        }

        public string JSVariableName
        {
            get
            {
                return "mmWebCrawler";
            }
        }
    }
}