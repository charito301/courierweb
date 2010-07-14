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
        private static string _emptyDataText = "No records could be retrieved from the database. We apologize for the inconvenience.";
        private static Dictionary<DALWebCrawlerActive.Word, DataFetcher.CountFileList> _resultsList = null;

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
            gvKeywords.PageIndexChanging += new GridViewPageEventHandler(gvKeywords_PageIndexChanging);
            gvKeywords.RowCommand += new GridViewCommandEventHandler(gvKeywords_RowCommand);
            gvKeywords.EmptyDataText = _emptyDataText;

            btnDoSearch.Click += new ImageClickEventHandler(btnDoSearch_Click);
        }

        void btnDoSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (tbSearchQuery.Text.Trim() == "")
            {
                return;
            }

            FetchData(tbSearchQuery.Text.Trim().ToLower());
        }

        void gvKeywords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Collections.Generic.KeyValuePair<DALWebCrawlerActive.Word, DataFetcher.CountFileList> currentWord = (System.Collections.Generic.KeyValuePair<DALWebCrawlerActive.Word, DataFetcher.CountFileList>)e.Row.DataItem;

                LinkButton lnkKeyword = (LinkButton)e.Row.FindControl("lnkKeyword");
                ImageButton btnToggle = (ImageButton)e.Row.FindControl("btnToggle");
                Label lblTotalCount = (Label)e.Row.FindControl("lblTotalCount");

                lnkKeyword.Text = currentWord.Key.WordName;
                lblTotalCount.Text = " [" + currentWord.Value.Count + "]";

                Literal ltrl = (Literal)e.Row.FindControl("lit1");
                ltrl.Text = ltrl.Text.Replace("trCollapseGrid", "trCollapseGrid" + e.Row.RowIndex);
                string str = "trCollapseGrid" + e.Row.RowIndex;
                lnkKeyword.Attributes["OnClick"] = "return OpenTable('" + str + "','" + btnToggle.ClientID + "');";
                btnToggle.Attributes["OnClick"] = "return OpenTable('" + str + "','" + btnToggle.ClientID + "');";

                GridView gvLinks = (GridView)e.Row.FindControl("gvLinks");
                gvLinks.RowDataBound += new GridViewRowEventHandler(gvLinks_RowDataBound);
                gvLinks.DataSource = currentWord.Value.FilesList;
                gvLinks.EmptyDataText = _emptyDataText;
                gvLinks.DataBind();
            }
        }

        void gvLinks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DALWebCrawlerActive.File file = (DALWebCrawlerActive.File)e.Row.DataItem;

                HyperLink lnkWebLink = (HyperLink)e.Row.FindControl("lnkLinkTitle");
                Label lblLinkDescription = (Label)e.Row.FindControl("lblLinkDescription");
                HyperLink lnkLink = (HyperLink)e.Row.FindControl("lnkLink");

                lnkWebLink.NavigateUrl = file.URL;
                if (file.Title != "")
                {
                    lnkWebLink.Text = file.Title;
                }
                else
                {
                    lnkWebLink.Text = file.URL;
                }

                if (file.Description != "/")
                {
                    lblLinkDescription.Text = file.Description;
                }

                lnkLink.NavigateUrl = file.URL;
                lnkLink.Text = file.URL;
            }
        }

        private void FetchData(string query)
        {
            _resultsList = DataFetcher.FetchResults(query);

            SetDataSource();
        }

        private void SetDataSource()
        {
            gvKeywords.DataSource = _resultsList;
            gvKeywords.DataBind();
        }

        #region gvKeywords paging

        protected void txtSlide_Changed(object sender, EventArgs e)
        {
            TextBox txtCurrentPage = sender as TextBox;
            GridViewRow rowPager = gvKeywords.BottomPagerRow;
            TextBox txtSliderExt = (TextBox)rowPager.Cells[0].FindControl("txtSlide");

            gvKeywords.PageIndex = Int32.Parse(txtSliderExt.Text) - 1;
            SetDataSource();
        }

        protected void pager_PreRender(object sender, EventArgs e)
        {
            SetDataSource();
        }

        void gvKeywords_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvKeywords.PageIndex = e.NewPageIndex;

            SetDataSource();
        }

        void gvKeywords_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow rowPager = gvKeywords.BottomPagerRow;
            TextBox txtSliderExt = (TextBox)rowPager.Cells[0].FindControl("txtSlide");
            int pageIndex = Int32.Parse(txtSliderExt.Text);

            switch (e.CommandName)
            {
                case "Next":
                    if (gvKeywords.PageCount - 1 > pageIndex)
                    {
                        txtSliderExt.Text = (pageIndex + 1).ToString();
                        gvKeywords.PageIndex = pageIndex;
                        SetDataSource();
                    }
                    break;
                case "Previous":
                    if (pageIndex > 1)
                    {
                        txtSliderExt.Text = (--pageIndex).ToString();
                        gvKeywords.PageIndex = pageIndex - 1;
                        SetDataSource();
                    }
                    break;
                case "Last":
                    txtSliderExt.Text = gvKeywords.PageCount.ToString();
                    gvKeywords.PageIndex = gvKeywords.PageCount - 1;
                    SetDataSource();
                    break;
                case "First":
                default:
                    txtSliderExt.Text = "1";
                    gvKeywords.PageIndex = 0;
                    SetDataSource();
                    break;
            }
        }

        #endregion

    }
}