<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleMapForASPNet.ascx.cs"
    Inherits="GoogleMapForASPNet" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<asp:ScriptManagerProxy ID="ScriptManager1" runat="server">
    <Services>
        <asp:ServiceReference Path="~/GService.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<div id="GoogleMap_Div_Container">
    <div id="GoogleMap_Div" style="width: <%=GoogleMapObject.Width %>; height: <%=GoogleMapObject.Height %>;">
    </div>
    <%
if(ShowControls)
{
    %>
    <input type="button" id="btnFullScreen" value="Full Screen" onclick="ShowFullScreenMap();" />
    &nbsp&nbsp
    <input type="checkbox" id="chkIgnoreZero" onclick="IgnoreZeroLatLongs(this.checked);" />Ignore
    Zero Lat Longs
    <% } %>
</div>
<div id="directions_canvas">
</div>
<asp:UpdatePanel ID="UpdatePanelXXXYYY" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hidEventName" runat="server" />
        <asp:HiddenField ID="hidEventValue" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>

<script src="http://maps.google.com/maps?file=api&amp;v=<%=GoogleMapObject.APIVersion %>&amp;key=<%=GoogleMapObject.APIKey %>"
    type="text/javascript"></script>

<script type="text/javascript" src="Scripts/GoogleMapAPIWrapper.js">
</script>

<script language="javascript" type="text/javascript">
    //RaiseEvent('MovePushpin','pushpin2');
    function RaiseEvent(pEventName, pEventValue) {
        document.getElementById('<%=hidEventName.ClientID %>').value = pEventName;
        document.getElementById('<%=hidEventValue.ClientID %>').value = pEventValue;
        if (document.getElementById('<%=UpdatePanelXXXYYY.ClientID %>') != null) {
            __doPostBack('<%=UpdatePanelXXXYYY.ClientID %>', '');
        }
    }

</script>

