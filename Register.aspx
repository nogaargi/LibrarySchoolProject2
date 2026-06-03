<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="LibraryClient.Register" MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="direction: rtl; text-align: center; margin-top: 50px;">
        <h2>דף הרשמה</h2>
        <table style="margin: 0 auto; background-color: #f9f9f9; padding: 20px; border-radius: 10px; width: 450px;">
            <tr>
                <td style="text-align: right; padding: 10px;">שם פרטי:</td>
                <td>
                    <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                        ControlToValidate="txtFirstName" ErrorMessage="שדה חובה!" ForeColor="Red" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revFirstName" runat="server"
                        ControlToValidate="txtFirstName"
                        ErrorMessage="ניתן להזין אותיות בלבד"
                        ValidationExpression="^[a-zA-Z\u0590-\u05FF\s]+$"
                        ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right; padding: 10px;">שם משפחה:</td>
                <td>
                    <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
                        ControlToValidate="txtLastName" ErrorMessage="שדה חובה!" ForeColor="Red" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revLastName" runat="server"
                        ControlToValidate="txtLastName"
                        ErrorMessage="ניתן להזין אותיות בלבד"
                        ValidationExpression="^[a-zA-Z\u0590-\u05FF\s]+$"
                        ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right; padding: 10px;">שם משתמש:</td>
                <td>
                    <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvUserName" runat="server"
                        ControlToValidate="txtUserName" ErrorMessage="שדה חובה!" ForeColor="Red" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revUserName" runat="server"
                        ControlToValidate="txtUserName"
                        ErrorMessage="ניתן להזין עד 10 תווים, ואותיות ומספרים בלבד"
                        ValidationExpression="^[a-zA-Z\u0590-\u05FF0-9]{1,10}$"
                        ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">עיר מגורים:</td>
                <td>
                    <asp:DropDownList ID="ddlCities" runat="server" Width="100%" Style="direction: rtl;"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; padding: 10px;">סיסמה:</td>
                <td>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                        ControlToValidate="txtPassword" ErrorMessage="שדה חובה!" ForeColor="Red" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revPassword" runat="server"
                        ControlToValidate="txtPassword"
                        ErrorMessage="חובה להזין לפחות 6 תווים, אות אחת, מספר אחד ותו מיוחד אחד"
                        ValidationExpression="^(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-zA-Z\u0590-\u05FF]).{6,}$"
                        ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right; padding: 10px;">טלפון:</td>
                <td>
                    <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvPhone" runat="server"
                        ControlToValidate="txtPhone" ErrorMessage="שדה חובה!" ForeColor="Red" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revPhone" runat="server"
                        ControlToValidate="txtPhone"
                        ErrorMessage="מספר טלפון לא תקין"
                        ValidationExpression="^0\d{8,9}$"
                        ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right; padding: 10px;">תאריך לידה:</td>
                <td>
                    <asp:TextBox ID="txtBirthDate" runat="server" placeholder="DD/MM/YYYY"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvBirthDate" runat="server"
                        ControlToValidate="txtBirthDate" ErrorMessage="שדה חובה!" ForeColor="Red" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revDate" runat="server"
                        ControlToValidate="txtBirthDate"
                        ErrorMessage="תאריך לא תקין. הפורמט צריך להיות DD/MM/YYYY"
                        ValidationExpression="^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\d{4}$"
                        ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="padding-top: 20px;">
                    <asp:Button ID="btnRegister" runat="server" Text="הירשם" OnClick="btnRegister_Click" Width="100px" />
                </td>
            </tr>
        </table>
        <br />
        <asp:Label ID="lblMsg" runat="server"></asp:Label>
    </div>
</asp:Content>
