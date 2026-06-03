<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ShowBooks.aspx.cs" Inherits="LibraryClient.ShowBooks1" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .search-container {
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 10px;
            display: flex;
            justify-content: flex-start;
            align-items: center;
            gap: 10px;
            border: 1px solid #ddd;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            width: fit-content;
        }

            .search-container input, .search-container select {
                padding: 8px 12px;
                border: 1px solid #ccc;
                border-radius: 5px;
                font-size: 14px;
            }

        .btn-search {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 8px 20px;
            border-radius: 5px;
            cursor: pointer;
            transition: 0.3s;
        }

            .btn-search:hover {
                filter: brightness(85%);
            }

        .btn-clear {
            background-color: #6c757d;
            color: white;
            border: none;
            padding: 8px 15px;
            border-radius: 5px;
            cursor: pointer;
        }

            .btn-clear:hover {
                filter: brightness(85%);
            }

        .scroll-wrapper {
            position: relative;
            display: flex;
            align-items: center;
        }

        .books-scroll-container {
            display: flex;
            overflow-x: auto; /* מאפשר את הגלילה */
            scroll-behavior: smooth; /* גלילה חלקה */
            flex-grow: 1;
            scrollbar-width: none; /* מסתיר את פס הגלילה המכוער */
        }

        .scroll-btn {
            background: rgba(0,0,0,0.5);
            color: white;
            border: none;
            cursor: pointer;
            padding: 10px;
            z-index: 10;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>

        <h3>מה מתחשק לך לקרוא היום?</h3>

        <div class="search-container" style="margin-bottom: 20px;">
            <asp:DropDownList ID="ddlSearchType" runat="server">
                <asp:ListItem Value="MyBookName" Text="שם הספר" />
                <asp:ListItem Value="MyAuthor" Text="שם הסופר" />
                <asp:ListItem Value="MyGenre" Text="ז'אנר" />
            </asp:DropDownList>
            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
            <asp:Button CssClass="btn-search" ID="btnSearch" runat="server" Text="חיפוש" OnClick="btnSearch_Click" />
            <asp:Button CssClass="btn-clear" ID="btnClear" runat="server" Text="נקה" OnClick="btnClear_Click" />
        </div>

        <br />

        <asp:Panel ID="pnlSearchResults" runat="server" Visible="false">
            <h2>תוצאות חיפוש</h2>
            <asp:DataList ID="dlSearchResults" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                <ItemTemplate>
                    <div class="book-item" style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; box-shadow: 2px 2px 8px rgba(0,0,0,0.1); width: 200px; background-color: #fff;">
                        <!-- עיצוב התמונה -->
                        <img src='<%# "Images/" + Eval("MyBookImage") + ".jpg" %>' style="width: 140px; height: 200px; border-radius: 5px; object-fit: cover;" />

                        <!-- עיצוב שם הספר - גובה קבוע כדי שהכל יהיה באותו קו -->
                        <h4 style="margin: 12px 0 5px 0; font-family: Arial; font-size: 1.1em; height: 45px; overflow: hidden; display: flex; align-items: center; justify-content: center;">
                            <%# Eval("MyBookName") %>
                        </h4>

                        <!-- שם הסופר -->
                        <p style="font-size: 0.9em; color: #555; margin-bottom: 15px;"><%# Eval("MyAuthor") %></p>

                        <!-- כפתור מעוצב -->
                        <asp:Button ID="btnLoan" runat="server"
                            Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                            Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                            CommandArgument='<%# Eval("MyBookID") %>'
                            OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                            Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ?
                            "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </asp:Panel>

        <asp:Panel ID="pnlCatalog" runat="server">
            <h2>רשימת הספרים</h2>

            <div style="direction: rtl; text-align: right; font-family: Arial;">

                <h2>ספרי פנטזיה</h2>

                <div class="scroll-wrapper">
                    <button class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div id="fantasyList" class="books-scroll-container">
                        <asp:DataList ID="dlFantasy" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; box-shadow: 2px 2px 8px rgba(0,0,0,0.1); width: 200px; background-color: #fff;">
                                    <!-- עיצוב התמונה -->
                                    <img src='<%# "Images/" + Eval("MyBookImage") + ".jpg" %>' style="width: 140px; height: 200px; border-radius: 5px; object-fit: cover;" />

                                    <!-- עיצוב שם הספר - גובה קבוע כדי שהכל יהיה באותו קו -->
                                    <h4 style="margin: 12px 0 5px 0; font-family: Arial; font-size: 1.1em; height: 45px; overflow: hidden; display: flex; align-items: center; justify-content: center;">
                                        <%# Eval("MyBookName") %>
                                    </h4>

                                    <!-- שם הסופר -->
                                    <p style="font-size: 0.9em; color: #555; margin-bottom: 15px;"><%# Eval("MyAuthor") %></p>

                                    <!-- כפתור מעוצב -->
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ?
                                "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>

                <h2>ספרי דרמה</h2>

                <div class="scroll-wrapper">
                    <button class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div id="darmaList" class="books-scroll-container">
                        <asp:DataList ID="dlDrama" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <!-- אותו מבנה בדיוק כמו בפנטזיה -->
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; box-shadow: 2px 2px 8px rgba(0,0,0,0.1); width: 200px; background-color: #fff;">
                                    <!-- עיצוב התמונה -->
                                    <img src='<%# "Images/" + Eval("MyBookImage") + ".jpg" %>' style="width: 140px; height: 200px; border-radius: 5px; object-fit: cover;" />

                                    <!-- עיצוב שם הספר - גובה קבוע כדי שהכל יהיה באותו קו -->
                                    <h4 style="margin: 12px 0 5px 0; font-family: Arial; font-size: 1.1em; height: 45px; overflow: hidden; display: flex; align-items: center; justify-content: center;">
                                        <%# Eval("MyBookName") %>
                                    </h4>

                                    <!-- שם הסופר -->
                                    <p style="font-size: 0.9em; color: #555; margin-bottom: 15px;"><%# Eval("MyAuthor") %></p>

                                    <!-- כפתור מעוצב -->
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>

                <h2>ספרי עיון</h2>

                <div class="scroll-wrapper">
                    <button class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div id="nonfictionList" class="books-scroll-container">
                        <asp:DataList ID="dlNonFiction" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; box-shadow: 2px 2px 8px rgba(0,0,0,0.1); width: 200px; background-color: #fff;">
                                    <!-- עיצוב התמונה -->
                                    <img src='<%# "Images/" + Eval("MyBookImage") + ".jpg" %>' style="width: 140px; height: 200px; border-radius: 5px; object-fit: cover;" />

                                    <!-- עיצוב שם הספר - גובה קבוע כדי שהכל יהיה באותו קו -->
                                    <h4 style="margin: 12px 0 5px 0; font-family: Arial; font-size: 1.1em; height: 45px; overflow: hidden; display: flex; align-items: center; justify-content: center;">
                                        <%# Eval("MyBookName") %>
                                    </h4>

                                    <!-- שם הסופר -->
                                    <p style="font-size: 0.9em; color: #555; margin-bottom: 15px;"><%# Eval("MyAuthor") %></p>

                                    <!-- כפתור מעוצב -->
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>

                <h2>ספרי מדע בדיוני</h2>

                <div class="scroll-wrapper">
                    <button class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div id="scifiList" class="books-scroll-container">
                        <asp:DataList ID="dlSciFi" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; box-shadow: 2px 2px 8px rgba(0,0,0,0.1); width: 200px; background-color: #fff;">
                                    <!-- עיצוב התמונה -->
                                    <img src='<%# "Images/" + Eval("MyBookImage") + ".jpg" %>' style="width: 140px; height: 200px; border-radius: 5px; object-fit: cover;" />

                                    <!-- עיצוב שם הספר - גובה קבוע כדי שהכל יהיה באותו קו -->
                                    <h4 style="margin: 12px 0 5px 0; font-family: Arial; font-size: 1.1em; height: 45px; overflow: hidden; display: flex; align-items: center; justify-content: center;">
                                        <%# Eval("MyBookName") %>
                                    </h4>

                                    <!-- שם הסופר -->
                                    <p style="font-size: 0.9em; color: #555; margin-bottom: 15px;"><%# Eval("MyAuthor") %></p>

                                    <!-- כפתור מעוצב -->
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>

                <h2>ספרי אוטוביוגרפיה</h2>

                <div class="scroll-wrapper">
                    <button class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div id="autobioList" class="books-scroll-container">
                        <asp:DataList ID="dlAutoBio" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; width: 200px;">
                                    <img src='Images/<%# Eval("MyBookImage") %>.jpg' style="width: 140px; height: 200px; object-fit: cover;" />
                                    <h4><%# Eval("MyBookName") %></h4>
                                    <p><%# Eval("MyAuthor") %></p>
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>

                <h2>ספרי רומן היסטורי</h2>

                <div class="scroll-wrapper">
                    <button class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div id="histromanList" class="books-scroll-container">
                        <asp:DataList ID="dlHistRoman" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; width: 200px;">
                                    <img src='Images/<%# Eval("MyBookImage") %>.jpg' style="width: 140px; height: 200px; object-fit: cover;" />
                                    <h4><%# Eval("MyBookName") %></h4>
                                    <p><%# Eval("MyAuthor") %></p>
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>

                <h2>ספרי מתח</h2>

                <div class="scroll-wrapper">
                    <button id="thrillerList" class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div class="books-scroll-container">
                        <asp:DataList ID="dlThriller" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; width: 200px;">
                                    <img src='Images/<%# Eval("MyBookImage") %>.jpg' style="width: 140px; height: 200px; object-fit: cover;" />
                                    <h4><%# Eval("MyBookName") %></h4>
                                    <p><%# Eval("MyAuthor") %></p>
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>

                <h2>ספרי מדע</h2>

                <div class="scroll-wrapper">
                    <button class="scroll-btn prev" onclick="scrollContent(this, 1, event)">&#10094;</button>

                    <div id="scienceList" class="books-scroll-container">
                        <asp:DataList ID="dlScience" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" OnItemCommand="dlBooks_ItemCommand">
                            <ItemTemplate>
                                <div style="border: 1px solid #ddd; margin: 10px; padding: 15px; text-align: center; border-radius: 10px; width: 200px;">
                                    <img src='Images/<%# Eval("MyBookImage") %>.jpg' style="width: 140px; height: 200px; object-fit: cover;" />
                                    <h4><%# Eval("MyBookName") %></h4>
                                    <p><%# Eval("MyAuthor") %></p>
                                    <asp:Button ID="btnLoan" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 ? "הוסף לסל" : "לא זמין" %>'
                                        Enabled='<%# Convert.ToInt32(Eval("MyAvailable")) > 0 %>'
                                        CommandArgument='<%# Eval("MyBookID") %>'
                                        OnClientClick="alert('הספר נוסף לסלסלת ההשאלות שלך בהצלחה! תוכל לאשר את ההשאלה באזור האישי.');"
                                        Style='<%# "width: 100%; padding: 8px; border-radius: 5px; border: 1px solid #ccc; cursor: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "pointer": "not-allowed") + "; background-color: " + (Convert.ToInt32(Eval("MyAvailable")) > 0 ? "#f8f9fa" : "#e9ecef") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>

                    <button class="scroll-btn next" onclick="scrollContent(this, -1, event)">&#10095;</button>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        function scrollContent(btn, direction, event) {
            // מניעת רענון הדף (ה-Postback)
            if (event) {
                event.preventDefault();
            }

            var wrapper = btn.closest('.scroll-wrapper');
            var container = wrapper.querySelector('.books-scroll-container');

            if (container) {
                container.scrollBy({
                    left: direction * 300,
                    behavior: 'smooth'
                });
            }
        }
</script>
</asp:Content>
