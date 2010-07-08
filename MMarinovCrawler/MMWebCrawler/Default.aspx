<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="MMarinov.WebCrawler.UI._Default" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="Stylesheet" href="Styles/Page.css" type="text/css" />
    <title>Margent - MMarinov's Search Agent</title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="Title">
        Margent</div>
        <div class="Subtitle">
            MMarinov's Search Agent</div>
    <div class="SearchBar">
        <asp:Label ID="lblSearch" runat="server" CssClass="SearchQuery" Text="Search the knowledge base:"></asp:Label>
        <asp:TextBox ID="tbSearchQuery" runat="server" CssClass="SearchQuery"></asp:TextBox>
        <asp:ImageButton ID="btnDoSearch" runat="server" Height="25px" CssClass="SearchQuery"
            ImageUrl="~/Images/btnSearch.jpg" OnClick="btnDoSearch_Click" />
    </div>
    <div>
        <asp:GridView ID="gvKeywords" runat="server" AutoGenerateColumns="False" BorderStyle="None"
            BorderColor="#CCCCCC" BorderWidth="1px" BackColor="White" AllowSorting="True"
            AllowPaging="True" GridLines="None" CellPadding="2">
            <FooterStyle ForeColor="#000066" BackColor="White"></FooterStyle>
            <PagerStyle ForeColor="#000066" HorizontalAlign="Center" BackColor="White"></PagerStyle>
            <HeaderStyle ForeColor="White" Font-Bold="True" BackColor="#006699"></HeaderStyle>
            <EmptyDataTemplate><div class="NoResults">
                No records could be retrieved from the database. We apologize for the invonvenience.</div>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="lnkKeyword">keyword</asp:HyperLink>
                        <div id="divLinks" runat="server">
                            <asp:GridView ID="gvLinks" runat="server">
                                <EmptyDataTemplate>
                                    No records could be retrieved from the database. We apologize for the invonvenience.
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:HyperLink runat="server" ID="lnkWebLink">follow me</asp:HyperLink>
                                            <asp:Label runat="server" ID="lblTitle">title</asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </ItemTemplate>
                    <FooterStyle BackColor="#f0f0f0" />
                    <FooterTemplate>
                        <asp:Label runat="server" ID="lblSumRevenueGrups">Sum revenue grups </asp:Label>
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
            <SelectedRowStyle ForeColor="White" Font-Bold="True" BackColor="#669999"></SelectedRowStyle>
            <RowStyle ForeColor="#000066"></RowStyle>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
