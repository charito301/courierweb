<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="MMarinov.WebCrawler.UI._Default" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MMarinov's Web Crawler</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height: 500px;">
        <asp:Label ID="Label1" runat="server" Text="Web Crawler:" Font-Size="Large"></asp:Label>
        <br />
        <br />
        <asp:Button ID="btnCrawl" runat="server" Text="Start Crawl!" OnClick="btnCrawl_Click" />
        &nbsp;<asp:Button ID="btnStopCrawl" runat="server" Text="Stop Crawl!" OnClick="btnStopCrawl_Click" />
        <br />
        <br />
        <table width="100%" style="height: 80%">
            <tr>
                <td>
                            <asp:TextBox ID="tbLog" runat="server" TextMode="MultiLine" Height="95%" Width="95%">
                            </asp:TextBox>
                   </td>
                <td>
                    <asp:TextBox ID="tbErrorLog" runat="server" TextMode="MultiLine" Height="95%" Width="95%">
                    </asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
