<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Margent.UI._Default" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="Stylesheet" href="Styles/Page.css" type="text/css" />
    <link rel="Stylesheet" href="Styles/GridViews.css" type="text/css" />

    <script type="text/javascript" src="JavaScripts/GridActions.js" language="javascript"></script>

    <!--[if IE 6]>
<style type="text/css">
html { overflow-y: hidden; }
body { overflow-y: auto; }
#bg { position:absolute; z-index:-1; }
#content { position:static; }
</style>
<![endif]-->
    <title>Margent - MMarinov's Search Agent</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>
    <img src="Images/2-brush-light.jpg" alt="background image" id="bg" />
    <div id="content">
        <div class="Title">
            <img src="Images/MargentTitle.PNG" alt="MMairnov Search Agent" />
        </div>
        <table width="100%">
            <tr>
                <td class="CellLabelSearch" width="30%">
                    <asp:Label ID="lblSearch" runat="server" Text="Search the knowledge base:"></asp:Label>
                </td>
                <td width="40%">
                    <asp:TextBox ID="tbSearchQuery" runat="server" CssClass="SearchQuery"></asp:TextBox>
                </td>
                <td width="30%">
                    <asp:ImageButton ID="btnDoSearch" runat="server" Height="35px" ImageUrl="~/Images/binoc.png"
                        OnClick="btnDoSearch_Click" onmouseover="this.src='Images/binoc_hover.png';"
                        onmouseout="this.src='Images/binoc.png';" />
                </td>
            </tr>
        </table>
        <div class="PositionGrids">
            <asp:GridView ID="gvKeywords" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                CssClass="GridKeywords" AllowPaging="True" GridLines="None" CellPadding="2">
                <EmptyDataTemplate>
                    No records could be retrieved from the database. We apologize for the invonvenience.
                </EmptyDataTemplate>
                <RowStyle CssClass="RowStyle" />
                <PagerStyle CssClass="PagerStyle" />
                <EmptyDataRowStyle CssClass="NoResults" />
                <HeaderStyle CssClass="HeaderStyle" />
                <AlternatingRowStyle CssClass="AltRowStyle" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="Keywords">
                                <asp:ImageButton ID="btnToggle" runat="server" ImageUrl="~/Images/Plus.gif" />
                                <asp:LinkButton runat="server" ID="lnkKeyword">keyword</asp:LinkButton></div>
                            <!-- nested view->
                            <asp:Literal ID="litGridLinks" runat="server"></asp:Literal>
                            <asp:Literal runat="server" ID="lit1" Text="</td><tr id='trCollapseGrid' class='RowStyle' style='display:none' ><td>" />
                            <asp:GridView ID="gvLinks" runat="server" AutoGenerateColumns="true" ShowHeader="false"
                                EnableViewState="False" CssClass="GridLinks" AllowPaging="True" GridLines="None">
                                <PagerStyle CssClass="PagerStyleLinks" />
                                <RowStyle CssClass="RowStyle" />
                                <EmptyDataRowStyle CssClass="NoResults" />
                                <EmptyDataTemplate>
                                    No records could be retrieved from the database. We apologize for the invonvenience.
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:HyperLink runat="server" ID="lnkWebLink" CssClass="LinkTitle">title - follow me</asp:HyperLink>
                                            <asp:Label runat="server" ID="lblLinkDescription" CssClass="LinkDescription"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Literal runat="server" ID="lit2" Text="</td></tr>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
