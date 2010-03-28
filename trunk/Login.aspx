<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Courier Management - LogIn</title>
    <link href="Styles/Main.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="Scripts/LoginValidation.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div class="title">
        <asp:Label runat="server" ID="lblPageTitle" Text="Courier Management"></asp:Label>
    </div>
    <table class="frame" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <asp:Panel runat="server" ID="pnlPageContent" DefaultButton="btnLogin">
                    <div class="sideBorders">
                        <div class="stepContent">
                            <div class="formInputSection">
                                <asp:Panel runat="server" ID="pnlErrorMessage" CssClass="errorMessage" Visible="false">
                                    <asp:Literal runat="server" ID="lblErrorMessage"></asp:Literal>
                                </asp:Panel>
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td class="formInputRow">
                                            <asp:Label ID="lblSalesLocation" runat="server" CssClass="formInputCaption" Text="Operator:"></asp:Label><span
                                                class="requiredField">*</span><br />
                                            <asp:DropDownList ID="ddlOperators" runat="server" Width="300px" OnSelectedIndexChanged="ddlOperators_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formInputRow">
                                            <asp:Label ID="lblPassword" runat="server" CssClass="formInputCaption" Text="Password:"></asp:Label><span
                                                class="requiredField">*</span><br />
                                            <asp:TextBox ID="tbPassword" runat="server" Columns="20" CssClass="formInputText"
                                                MaxLength="150" TextMode="Password" Width="300px"></asp:TextBox>&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <asp:Panel runat="server" ID="pnlButtonsRow">
                                <div class="formInputButtonsRow">
                                    &nbsp;
                                    <asp:Button ID="btnLogin" runat="server" CssClass="formInputButton" OnClick="btnLogin_Click"
                                        Text="Login" ValidationGroup="validationGroup" />
                                </div>
                            </asp:Panel>
                            &nbsp;
                        </div>
                    </div>
                </asp:Panel>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
