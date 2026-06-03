<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="LibraryClient.MyProfile" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* קופסאות המסגרת העגולות לשני החלקים */
        .my-custom-box {
            background-color: #ffffff;
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            margin-bottom: 25px;
        }

        /* עיצוב הכותרות החדשות בתוך הקופסאות */
        .my-custom-title {
            font-size: 20px;
            color: #1a237e !important;
            margin-top: 0;
            margin-bottom: 15px;
            font-weight: bold;
        }

        /* עיצוב הטבלאות (הכותרת הכחולה של ה-GridView) */
        .myGrid {
            border-collapse: collapse;
        }

            .myGrid th {
                background-color: #2b82c9 !important;
                color: white !important;
                padding: 10px;
                font-weight: bold;
                text-align: center;
            }

            .myGrid td {
                padding: 10px;
                border-bottom: 1px solid #eee;
            }

        .my-return-button {
            background-color: #f44336;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
        }

            .my-return-button:hover {
                background-color: #d32f2f;
            }

        /* עיצוב הכפתור הירוק */
        .my-green-button {
            background-color: #4CAF50;
            color: white;
            border: none;
            padding: 10px 24px;
            font-size: 16px;
            border-radius: 5px;
            cursor: pointer;
            font-weight: bold;
            transition: background-color 0.2s ease;
        }

            /* אפקט כהה כשעומדים על הכפתור עם העכבר */
            .my-green-button:hover {
                background-color: #388E3C;
            }

        /* שכבת הרקע שמחשיכה את הדף */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            z-index: 999;
        }

        /* עיצוב החלון עצמו */
        .edit-modal-box {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: white;
            padding: 30px;
            border-radius: 15px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            z-index: 1000;
            width: 400px; /* רוחב נורמלי וקבוע */
        }

        /* עיצוב אחיד לתיבות טקסט */
        .my-text-input {
            width: 100%; /* ממלא את התא של הטבלה */
            padding: 8px;
            box-sizing: border-box; /* חשוב כדי שה-padding לא ינפח את השדה */
            margin-bottom: 10px;
        }

        .my-custom-button {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-weight: bold;
            color: white;
        }

            .my-custom-button:hover {
                filter: brightness(110%); /* הכפתור יבהיר מעט כשעוברים עליו */
                transform: translateY(-2px); /* הכפתור יקפוץ פיקסל אחד למעלה */
            }

            /* אם רוצים להוסיף אפקט לחיצה */
            .my-custom-button:active {
                transform: translateY(0); /* הכפתור יחזור למקומו בלחיצה */
            }

        .btn-green {
            background-color: #4CAF50;
        }

        .btn-blue {
            background-color: #007bff;
        }

            .btn-blue:hover {
                filter: brightness(85%);
                transform: none;
            }

        .btn-gray {
            background-color: #6c757d;
        }
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Button ID="btnOpenEdit" CssClass="my-custom-button btn-blue" runat="server" Text="עדכון פרטים אישיים" OnClientClick="showModal(); return false;" />

    <div id="overlay" class="modal-overlay"></div>
    <div id="editModal" class="edit-modal-box">
        <h3 style="text-align: center;">עריכת פרופיל</h3>
        <table style="width: 100%;">
            <tr>
                <td>שם פרטי:</td>
                <td>
                    <asp:TextBox ID="txtEditFName" runat="server" CssClass="my-text-input"></asp:TextBox></td>
            </tr>
            <tr>
                <td>שם משפחה:</td>
                <td>
                    <asp:TextBox ID="txtEditLName" runat="server" CssClass="my-text-input"></asp:TextBox></td>
            </tr>
            <tr>
                <td>עיר:</td>
                <td>
                    <asp:DropDownList ID="ddlCities" runat="server" CssClass="my-text-input"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>טלפון:</td>
                <td>
                    <asp:TextBox ID="txtEditPhone" runat="server" CssClass="my-text-input"></asp:TextBox></td>
            </tr>
        </table>

        <div style="text-align: center; margin-top: 20px;">
            <asp:Button ID="btnSave" runat="server" Text="שמור שינויים" CssClass="my-custom-button btn-green" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="ביטול" CssClass="my-custom-button btn-gray" />
            <%--<button type="button" onclick="hideModal()">ביטול</button>--%>
        </div>
    </div>

    <div style="direction: rtl; text-align: right; font-family: Arial; padding: 20px;">

        <asp:Label ID="lblMessage" runat="server" ForeColor="Blue" Font-Size="Large"></asp:Label>

        <div class="my-custom-box">
            <h2 class="my-custom-title">📚 ספרים מושאלים:</h2>
            <asp:GridView ID="gvLoans" runat="server" AutoGenerateColumns="False" CssClass="myGrid" Width="100%" OnRowCommand="gvLoans_RowCommand" EmptyDataText="עדיין לא השאלת ספרים מהספרייה שלנו!">
                <Columns>
                    <asp:BoundField DataField="MyLoanID" HeaderText="קוד השאלה" HeaderStyle-Width="10%" />
                    <asp:BoundField DataField="MyBookName" HeaderText="שם הספר" HeaderStyle-Width="30%" />
                    <asp:BoundField DataField="MyLoanDate" HeaderText="תאריך השאלה" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" HeaderStyle-Width="20%" />
                    <asp:BoundField DataField="MyReturnDate" HeaderText="תאריך החזרה מבוקש" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" HeaderStyle-Width="20%" />

                    <%-- 🌟 העמודה החדשה: כפתור החזרה ייעודי לכל שורה --%>
                    <asp:TemplateField HeaderText="פעולה" HeaderStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Button ID="btnReturn" runat="server" Text="↩️ החזר"
                                CommandName="ReturnBook"
                                CommandArgument='<%# Eval("MyLoanID") %>'
                                CssClass="my-return-button" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="my-custom-box">
            <h2 class="my-custom-title">🛒 סלסלת ההשאלות הזמנית שלך:</h2>
            <asp:GridView ID="gvBasket" runat="server" AutoGenerateColumns="False" CssClass="myGrid" Width="100%" EmptyDataText="הסלסלה שלך ריקה כרגע." OnRowCommand="gvBasket_RowCommand">
                <Columns>
                    <asp:BoundField DataField="MyBookID" HeaderText="קוד ספר" HeaderStyle-Width="15%" />
                    <asp:BoundField DataField="MyBookName" HeaderText="שם הספר" HeaderStyle-Width="45%" />
                    <asp:BoundField DataField="MyAuthor" HeaderText="מאת (סופר)" HeaderStyle-Width="25%" />
                    <asp:TemplateField HeaderText="פעולה">
                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" Text="🗑️ מחק מהסל"
                                CommandName="RemoveFromBasket"
                                CommandArgument='<%# Container.DataItemIndex %>'
                                CssClass="your-delete-button-class" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div style="text-align: center; margin-top: 15px;">
                <asp:Button ID="btnCheckout" runat="server" Text="✓ בצע את ההשאלה כעת!" OnClick="btnCheckout_Click" CssClass="my-green-button" />
            </div>
        </div>

        <asp:Panel ID="pnlSummary" runat="server" Visible="False" Style="border: 2px dashed #4CAF50; padding: 15px; background-color: #e8f5e9; border-radius: 5px;">
            <h3 style="color: #2e7d32;">🎉 ההזמנה בוצעה בהצלחה!</h3>
            <p>
                <asp:Label ID="lblDate" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label ID="lblDestination" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label ID="lblArrival" runat="server"></asp:Label>
            </p>
        </asp:Panel>

    </div>

    <script type="text/javascript">
        function showModal() {
            document.getElementById('editModal').style.display = 'block';
            document.getElementById('overlay').style.display = 'block';
        }
        function hideModal() {
            document.getElementById('editModal').style.display = 'none';
            document.getElementById('overlay').style.display = 'none';
        }
    </script>

</asp:Content>
