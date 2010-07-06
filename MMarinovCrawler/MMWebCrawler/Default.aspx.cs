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
            string javaScriptInstance = "<script type='text/javascript'>" + this.JSVarName + " = new WebCrawlerJS();</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "MakeAnInstance", javaScriptInstance, false);
        }

        protected override void OnPreRender(EventArgs e)
        {
            LoadJavaScriptData();

            base.OnPreRender(e);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadJavaScriptData()
        {
            string loadValues = "";

            //Get payment values from form and save them in javascript class.
            foreach (GridViewRow row in gvKeywords.Rows)
            {
                Label lblGroupName = (Label)row.FindControl("lblGroupName");
                GridView gvLinks = (GridView)row.FindControl("gvLinks");

                //loadValues += this.JSVarName + ".RegisteredIds.push('" + lblGroupName.ClientID + "');" + this.JSVarName + ".RegisteredValues.push(" + (lblGroupName.Text != "" ? tbRegistered.Text.Replace(',', '.') : "0.00") + ");";

                gvLinks.Style["display"] = "none";

                //tbComment.Style["display"] = "block";
            }

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadValues; ", loadValues, true);

            //loadValues = "";

            //GridViewRow currentRow = grPayments.FooterRow;
            //TextBox tbRegisteredFooter = (TextBox)currentRow.FindControl("tbRegisteredFooter");

            ////Register field's ids in wich result sum is calculated.
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "SetProperties1", this.JavaScriptVariableName + ".TbDifferenceId ='" + tbDifferenceFooter.ClientID + "';", true);
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "SetProperties2", this.JavaScriptVariableName + ".TbRegisteredId ='" + tbRegisteredFooter.ClientID + "';", true);

            //GridViewRow groupsFooterRow = grRevenueGroups.FooterRow;

        }

        void gvKeywords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //RevenueGroup currentGroup = (RevenueGroup)e.Row.DataItem;

                Label lblGroupName = (Label)e.Row.FindControl("lblGroupName");
                GridView gvLinks = (GridView)e.Row.FindControl("gvLinks");

                //tbRevenue.Attributes["OnBlur"] = this.JavaScriptVariableName + ".ValidateFloat(this);";
                //tbRevenue.Attributes["OnBlur"] += this.JavaScriptVariableName + ".UpdateData(" + this.JavaScriptVariableName + ".RevenueIds, " + this.JavaScriptVariableName + ".RevenueValues, this.id, event);";
                //tbRevenue.Attributes["OnBlur"] += this.JavaScriptVariableName + ".SumGroups();";

                //lblGroupName.Text = currentGroup.Name;
            }

            //if (e.Row.RowType == DataControlRowType.Footer)
            //{
            //    Label lblSumRevenueGrups = (Label)e.Row.FindControl("lblSumRevenueGrups");

            //    //This fixes the Tab problem of focus losing after UpdateData()
            //    tbRevenueSum.Attributes["OnFocus"] = "this.select()";

            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "SetProperties", this.JavaScriptVariableName + ".TbRevenueSumId='" + tbRevenueSum.ClientID + "';", true);
            //}
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

        public string JSVarName
        {
            get
            {
                return "mmWebCrawler";
            }
        }
    }
}