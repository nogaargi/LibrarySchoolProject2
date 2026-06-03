<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LibraryClient.Login1" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="direction: rtl; text-align: center; margin-top: 50px;">
        <h2>דף התחברות</h2>
        <table style="margin: 0 auto; background-color: #f9f9f9; padding: 20px; border-radius: 10px; width: 400px;">
            <tr>
                <td style="text-align: right; padding: 10px;">שם משתמש:</td>
                <td>
                    <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox></td>
            </tr>
            <tr style="border-top: 1px solid #ddd;">
                <td style="text-align: right; padding: 10px;">סיסמה:</td>
                <td>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2" style="padding-top: 20px;">
                    <asp:Button ID="btnLogin" runat="server" Text="התחבר" OnClick="btnLogin_Click" Width="100px" />
                </td>
            </tr>
        </table>
        <br />
        <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
    </div>
</asp:Content>
