<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Margent.UI._Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="Stylesheet" href="Styles/Page.css" type="text/css" />
    <link rel="Stylesheet" href="Styles/GridViews.css" type="text/css" />

    <script type="text/javascript" src="JavaScripts/GridActions.js" language="javascript"></script>

    <script type="text/javascript" src="JavaScripts/JQuery.js" language="javascript"></script>

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
    <img src="Images/backgroundMain.jpg" alt="background image" id="bg" />
    <div id="content">
        <div class="Title">
            <img src="Images/MargentTitle.PNG" alt="MMairnov Search Agent" />
        </div>
        <table width="100%">
            <tr>
                <td class="CellLabelSearch" width="35%">
                    <asp:Label ID="lblSearch" runat="server" Text="Search the knowledge base:"></asp:Label>
                </td>
                <td width="30%">
                    <asp:TextBox ID="tbSearchQuery" runat="server" CssClass="SearchQuery"></asp:TextBox>
                </td>
                <td width="35%" style="padding-left:5px;" >
                    <asp:ImageButton ID="btnDoSearch" runat="server" Height="37px" ImageUrl="~/Images/binoc.png"
                        onmouseover="this.src='Images/binoc_hover.png';" onmouseout="this.src='Images/binoc.png';" />
                </td>
            </tr>
        </table>
        <div class="PositionGrids">
            <asp:Label runat="server" ID="lblError" CssClass="ErrorMessage"> error message</asp:Label>
            <asp:UpdatePanel ID="updPanel" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="gvKeywords" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                        CssClass="GridKeywords" AllowPaging="True" ShowHeader="true" GridLines="None"
                        CellPadding="2">
                        <RowStyle CssClass="RowStyle" />
                        <PagerStyle CssClass="PagerStyle" />
                        <EmptyDataRowStyle CssClass="NoResults" />
                        <AlternatingRowStyle CssClass="AltRowStyle" />
                        <FooterStyle />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <div class="Keywords">
                                        <asp:ImageButton ID="btnToggle" runat="server" ImageUrl="~/Images/Plus.gif" />
                                        <asp:LinkButton runat="server" ID="lnkKeyword">keyword</asp:LinkButton>
                                        <asp:Label runat="server" ID="lblTotalCount" CssClass="TotalCount">total count</asp:Label>
                                    </div>
                                    <!-- ===============nested view ================= -->
                                    <asp:Literal ID="litGridLinks" runat="server"></asp:Literal>
                                    <asp:Literal runat="server" ID="lit1" Text="<div id='trCollapseGrid' class='RowStyle' style='display:none' >" />
                                    <asp:GridView ID="gvLinks" runat="server" AutoGenerateColumns="false" ShowHeader="false"
                                        EnableViewState="False" CssClass="GridLinks" AllowPaging="True" GridLines="None">
                                        <PagerStyle CssClass="PagerStyleLinks" />
                                        <RowStyle CssClass="RowStyleLinks" />
                                        <EmptyDataRowStyle CssClass="NoResults" />
                                        <AlternatingRowStyle CssClass="AltRowStyleLinks" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="LinkTitle">
                                                        <asp:HyperLink runat="server" ID="lnkLinkTitle">title - follow me</asp:HyperLink>
                                                    </div>
                                                    <div class="LinkDescription">
                                                        <asp:Label runat="server" ID="lblLinkDescription"></asp:Label>
                                                    </div>
                                                    <div class="LinkLink">
                                                        <asp:HyperLink runat="server" ID="lnkLink">link follow me</asp:HyperLink>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerTemplate>
                                            <div style="height: 20px;">
                                                <div class="command">
                                                    <asp:ImageButton ID="btnFirst" runat="server" CommandName="First" ImageUrl="~/Images/btnFirst.PNG"
                                                        AlternateText="First Page" ToolTip="First Page" />
                                                    <asp:ImageButton ID="btnPrevious" runat="server" CommandName="Previous" ImageUrl="~/Images/btnPrev.PNG"
                                                        AlternateText="Previous Page" ToolTip="Previous Page" />
                                                </div>
                                                <div class="command">
                                                    <asp:TextBox ID="txtSlide" runat="server" AutoPostBack="true" OnTextChanged="txtSlideLinks_Changed" />
                                                    <ajaxToolkit:SliderExtender ID="ajaxSlider" runat="server" TargetControlID="txtSlide"
                                                        RaiseChangeOnlyOnMouseUp="true" Orientation="Horizontal" Minimum="1" />
                                                </div>
                                                <div class="command">
                                                    <asp:ImageButton ID="btnNext" runat="server" CommandName="Next" ImageUrl="~/Images/btnNext.PNG"
                                                        AlternateText="Next Page" ToolTip="Next Page" />
                                                    <asp:ImageButton ID="btnLast" runat="server" CommandName="Last" ImageUrl="~/Images/btnLast.PNG"
                                                        AlternateText="Last Page" ToolTip="Last Page" />
                                                </div>
                                                <div class="PagerInfo">
                                                    <asp:Label ID="lblPage" CssClass="PagerInfo" runat="server" />
                                                </div>
                                            </div>
                                        </PagerTemplate>
                                    </asp:GridView>
                                    <asp:Literal runat="server" ID="lit2" Text="</div>" />
                                    <!-- ===============nested view end ============== -->
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerTemplate>
                            <div style="height: 30px;">
                                <div class="command">
                                    <asp:ImageButton ID="btnFirst" runat="server" CommandName="First" ImageUrl="~/Images/btnFirst.PNG"
                                        AlternateText="First Page" ToolTip="First Page" />
                                    <asp:ImageButton ID="btnPrevious" runat="server" CommandName="Previous" ImageUrl="~/Images/btnPrev.PNG"
                                        AlternateText="Previous Page" ToolTip="Previous Page" />
                                </div>
                                <div class="command">
                                    <asp:TextBox ID="txtSlide" runat="server" Text='<%# gvKeywords.PageIndex + 1 %>'
                                        AutoPostBack="true" OnTextChanged="txtSlide_Changed" />
                                    <ajaxToolkit:SliderExtender ID="ajaxSlider" runat="server" TargetControlID="txtSlide"
                                        RaiseChangeOnlyOnMouseUp="true" Orientation="Horizontal" Minimum="1" Steps='<%# gvKeywords.PageCount %>'
                                        Maximum='<%# ((GridView)Container.NamingContainer).PageCount %>' />
                                </div>
                                <div class="command">
                                    <asp:ImageButton ID="btnNext" runat="server" CommandName="Next" ImageUrl="~/Images/btnNext.PNG"
                                        AlternateText="Next Page" ToolTip="Next Page" />
                                    <asp:ImageButton ID="btnLast" runat="server" CommandName="Last" ImageUrl="~/Images/btnLast.PNG"
                                        AlternateText="Last Page" ToolTip="Last Page" />
                                </div>
                                <div class="PagerInfo">
                                    <asp:Label ID="lblPage" CssClass="PagerInfo" runat="server" Text='<%# "Page " + (gvKeywords.PageIndex + 1) + " of " + gvKeywords.PageCount %>' />
                                </div>
                            </div>
                        </PagerTemplate>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="FetchInfo">
                <asp:Label runat="server" ID="lblSummary"></asp:Label>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
