using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace Margent.UI
{
    public partial class _Default : System.Web.UI.Page
    {
        private Dictionary<DALWebCrawlerActive.Word, DataFetcher.CountFileList> resultsList = null;

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
        }

        void gvKeywords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Collections.Generic.KeyValuePair<DALWebCrawlerActive.Word, DataFetcher.CountFileList> currentWord = (System.Collections.Generic.KeyValuePair<DALWebCrawlerActive.Word, DataFetcher.CountFileList>)e.Row.DataItem;

                LinkButton lnkKeyword = (LinkButton)e.Row.FindControl("lnkKeyword");
                ImageButton btnToggle = (ImageButton)e.Row.FindControl("btnToggle");

                lnkKeyword.Text = currentWord.Key.WordName + " [" + currentWord.Value.Count + "]";

                //System.Data.DataTable dtLinks = new System.Data.DataTable();
                //dtLinks.Columns.Add("LiteralData");

                //StringBuilder sbGridLinks;

                //foreach (DALWebCrawlerActive.File file in currentWord.Value.FilesList)
                //{
                //    sbGridLinks = new StringBuilder();
                //    sbGridLinks.AppendLine("<div class='LinkTitle'>");
                //    if (file.Title != "")
                //    {
                //        sbGridLinks.AppendLine("<a href='" + file.URL + "'>" + file.Title + "</a>");
                //    }
                //    else
                //    {
                //        sbGridLinks.AppendLine("<a href='" + file.URL + "'>" + file.URL + "</a>");
                //    }
                //    sbGridLinks.AppendLine("</div>");
                //    sbGridLinks.AppendLine("<div class='LinkDescription'>" + file.Description + "</div>");
                //    sbGridLinks.AppendLine("<br/>");

                //    dtLinks.Rows.Add(new object[] { sbGridLinks.ToString() });
                //}

                Literal ltrl = (Literal)e.Row.FindControl("lit1");
                ltrl.Text = ltrl.Text.Replace("trCollapseGrid", "trCollapseGrid" + e.Row.RowIndex.ToString());
                string str = "trCollapseGrid" + (e.Row.RowIndex + 1).ToString();
                lnkKeyword.Attributes["OnClick"] = "return OpenTable('" + str + "','" + btnToggle.ClientID + "');";
                btnToggle.Attributes["OnClick"] = "return OpenTable('" + str + "','" + btnToggle.ClientID + "');";

                GridView gvLinks = (GridView)e.Row.FindControl("gvLinks");
                gvLinks.DataSource = currentWord.Value.FilesList;
                gvLinks.DataBind();
            }
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
            resultsList = DataFetcher.FetchResults(query);

            gvKeywords.DataSource = resultsList;
            gvKeywords.DataBind();
        }

        public string JSVarName
        {
            get
            {
                return "mmWebCrawler";
            }
        }
    }
}