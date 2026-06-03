<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="LibraryClient.AdminPage" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--    <style>
        .admin-grid {
            width: 100%;
            border-collapse: collapse; /* גורם לטבלה להיראות כגוש אחד */
            margin-bottom: 30px; /* מרווח בין טבלה לטבלה */
            box-shadow: 0 2px 5px rgba(0,0,0,0.1); /* צל עדין למראה מקצועי */
            font-family: Arial, sans-serif;
        }

            .admin-grid th {
                background-color: #343a40; /* צבע כהה ואחיד לכותרות */
                color: white;
                padding: 12px;
                text-align: center;
            }

            .admin-grid td {
                border: 1px solid #dee2e6;
                padding: 10px;
                text-align: center;
            }

            /* אפקט של "פסים" בשורות לשיפור הקריאות */
            .admin-grid tr:nth-child(even) {
                background-color: #f8f9fa;
            }

            .admin-grid tr:hover {
                background-color: #e9ecef;
            }
        /* אפקט כשהעכבר עובר על שורה */
    </style>--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3>בחירת ספר להמלצת השבוע</h3>
    <asp:GridView ID="gvBooks" runat="server" AutoGenerateColumns="False" OnRowCommand="gvBooks_RowCommand" DataKeyNames="MyBookID" CssClass="admin-grid">
        <Columns>
            <asp:BoundField DataField="MyBookName" HeaderText="שם הספר" />
            <asp:TemplateField HeaderText="פעולות">
                <ItemTemplate>
                    <asp:Button ID="btnSetRec" runat="server" Text="הפוך להמלצה" CommandName="SetRec" CommandArgument='<%# Eval("MyBookID") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <hr />

    <h3>ניהול תגובות</h3>
    <asp:GridView ID="gvComments" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvComments_RowDeleting" DataKeyNames="MyCommentID" CssClass="admin-grid">
        <Columns>
            <asp:CommandField ShowDeleteButton="True" DeleteText="מחק" />
            <asp:BoundField DataField="MyName" HeaderText="שם" />
            <asp:BoundField DataField="MyComment" HeaderText="תגובה" />
        </Columns>
    </asp:GridView>

    <hr />

    <h3>ניהול משתמשים</h3>
    <asp:GridView ID="gvMembers" runat="server" AutoGenerateColumns="false" OnRowDeleting="gvMembers_RowDeleting" DataKeyNames="MyMemberID" CssClass="admin-grid">
        <Columns>
            <asp:CommandField ShowDeleteButton="True" DeleteText="מחק" />
            <asp:BoundField DataField="MyMemberID" HeaderText="מספר משתמש" />
            <asp:BoundField DataField="MyFirstName" HeaderText="שם פרטי" />
            <asp:BoundField DataField="MyLastName" HeaderText="שם משפחה" />
            <asp:BoundField DataField="IsAdmin" HeaderText="האם מנהל" />
        </Columns>
    </asp:GridView>

    <hr />

    <h3>ניהול השאלות</h3>
    <asp:GridView ID="gvLoans" runat="server" AutoGenerateColumns="false" OnRowDeleting="gvLoans_RowDeleting" DataKeyNames="MyLoanID" CssClass="admin-grid">
        <Columns>
            <asp:CommandField ShowDeleteButton="True" DeleteText="מחק" />
            <asp:BoundField DataField="MyLoanID" HeaderText="מספר השאלה" />
            <asp:BoundField DataField="MyMemberID" HeaderText="מספר משתמש" />
            <asp:BoundField DataField="MyBookID" HeaderText="מספר ספר" />
            <asp:BoundField DataField="IsReturned" HeaderText="האם הוחזר" />
        </Columns>
    </asp:GridView>

</asp:Content>
