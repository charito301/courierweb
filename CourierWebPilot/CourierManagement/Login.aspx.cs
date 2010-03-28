using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

public partial class Login : System.Web.UI.Page
{
    private string strSelectOperator = "- select operator -";
    private string strErrInvalidLoginDetails = "Invalid login details.";
    private string strErrSelectOperator = "Select operator.";

    protected void Page_Load(object sender, EventArgs e)
    {
        pnlErrorMessage.Visible = false;
        btnLogin.Attributes.Add("onClick", "return valSubmit();");
        tbPassword.TextChanged += new EventHandler(tbPassword_TextChanged);
        btnLogin.Enabled = false;

        if (!Page.IsPostBack)
        {
            LoadOperators();
        }
    }

    void tbPassword_TextChanged(object sender, EventArgs e)
    {
        btnLogin.Enabled = tbPassword.Text.Trim().Length > 6 && ddlOperators.SelectedValue != "";
    }

    /// <summary>
    /// Loads opeator names in ddlSalesOperator.
    /// </summary>
    private void LoadOperators()
    {
        ddlOperators.Items.Clear();


        ddlOperators.DataBind();
        ddlOperators.Items.Insert(0, new ListItem(strSelectOperator, ""));
    }

    /// <summary>
    /// Checks if there is selected Operator and if specified passowrod is correct.
    /// If all input data is valid the redirects to Operator's form otherwise shows error messages.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        //If there is selected Operator then get Operator's details.
        if (ddlOperators.SelectedValue != "")
        {
            //Operator currentOperator = new Operator();
            //currentOperator.LoadOperatorDetails(Page.Server.MapPath("~/SalesOperators") + "/" + ddlOperators.SelectedValue);

            ////If specified password is correct then redirect to Operator's form.
            //if (currentOperator.Password == tbPassword.Text)
            //{
            //    Page.Response.Redirect("default.aspx?Operator=" + ddlOperators.SelectedValue + "&Session=" + Common.CreateSession(ddlOperators.SelectedValue, currentOperator.Password));
            //}
            //else
            //{
            //    ShowErrorMessage(strErrInvalidLoginDetails);
            //}
        }
        else
        {
            ShowErrorMessage(strErrSelectOperator);
        }
    }

    /// <summary>
    /// If input message is not empty then shows error message
    /// else hides it.
    /// </summary>
    /// <param name="message">Error message that will be shown.</param>
    private void ShowErrorMessage(string message)
    {
        if (message != "")
        {
            pnlErrorMessage.Visible = true;
            lblErrorMessage.Text = message;
        }
        else
        {
            pnlErrorMessage.Visible = false;
        }
    }

    protected void ddlOperators_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlOperators.SelectedValue != "")
        {
            //Operator selectedOperator = new Operator();
            //selectedOperator.LoadOperatorDetails(Page.Server.MapPath("~/SalesOperators") + "/" + ddlOperators.SelectedValue);

            tbPassword.Focus();
        }
    }
}
