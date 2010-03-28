<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/SimpleMapWithNoBubble.aspx.cs"
    Inherits="Samples_SimpleMap" %>

<%@ Register Src="~/GoogleMapForASPNet.ascx" TagName="GoogleMapForASPNet" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Styles/Main.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="Styles/VerticalMenu.css" />

    <script type="text/javascript" src="Scripts/VerticalMenu.js"></script>

    <title>Simple Google Map</title>
</head>
<body>
    <form id="form1" runat="server">
    <a href="Login.aspx" class="rightAlignment">logout</a>
    <table>
        <tr>
            <td>
                <ul id="verticalmenu" class="glossymenu">
                    <li><a href="http://www.javascriptkit.com/" shape="rect">JavaScript Kit</a></li>
                    <li><a href="http://www.javascriptkit.com/cutpastejava.shtml">Free JavaScripts</a></li>
                    <li><a href="http://www.javascriptkit.com/">JavaScript Tutorials</a></li>
                    <li><a href="#">References</a>
                        <ul>
                            <li><a href="http://www.javascriptkit.com/jsref/">JavaScript Reference</a></li>
                            <li><a href="http://www.javascriptkit.com/domref/">DOM Reference</a></li>
                            <li><a href="http://www.javascriptkit.com/dhtmltutors/cssreference.shtml">CSS Reference</a></li>
                        </ul>
                    </li>
                    <li><a href="http://www.javascriptkit.com/cutpastejava.shtml">DHTML/ CSS Tutorials</a></li>
                    <li><a href="http://www.javascriptkit.com/howto/">web Design Tutorials</a></li>
                    <li><a href="#">Helpful Resources</a>
                        <ul>
                            <li><a href="http://www.dynamicdrive.com">Dynamic HTML</a></li>
                            <li><a href="http://www.codingforums.com">Coding Forums</a></li>
                            <li><a href="http://www.cssdrive.com">CSS Drive</a></li>
                            <li><a href="http://www.dynamicdrive.com/style/">CSS Library</a></li>
                            <li><a href="http://tools.dynamicdrive.com/imageoptimizer/">Image Optimizer</a></li>
                            <li><a href="http://tools.dynamicdrive.com/favicon/">Favicon Generator</a></li>
                        </ul>
                    </li>
                </ul>
            </td>
            <td>
                <asp:ScriptManager ID="ScriptManager1" runat="server">
                </asp:ScriptManager>
                <div>
                    <uc1:GoogleMapForASPNet ID="GoogleMapForASPNet1" runat="server" />
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
