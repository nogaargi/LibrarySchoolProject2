<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="LibraryClient.Home1" MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="lblWelcome" runat="server" Text=""></asp:Label>
    <div class="container" style="direction: rtl; padding: 20px; font-family: sans-serif;">

        <div style="background-color: #f8f9fa; border-right: 5px solid #1e3c72; padding: 15px; margin-bottom: 30px; border-radius: 4px; box-shadow: 0 2px 5px rgba(0,0,0,0.05);">
            <h3 id="greetingText" style="margin: 0; color: #1e3c72; font-size: 1.5rem;">שלום קורא אורח!</h3>
        </div>

        <div style="display: flex; justify-content: space-between; gap: 20px; margin-top: 30px; flex-wrap: wrap;">
            <div style="flex: 1; min-width: 200px; background: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; text-align: center; box-shadow: 0 4px 10px rgba(0,0,0,0.05);">
                <div style="font-size: 2.5rem; font-weight: bold; color: #1e3c72; margin-bottom: 5px;" id="countBooks">0</div>
                <div style="color: #4a5568; font-weight: bold; font-size: 1.1rem;">📚 ספרים דיגיטליים במלאי</div>
            </div>

            <div style="flex: 1; min-width: 200px; background: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; text-align: center; box-shadow: 0 4px 10px rgba(0,0,0,0.05);">
                <div style="font-size: 2.5rem; font-weight: bold; color: #1e3c72; margin-bottom: 5px;" id="countUsers">0</div>
                <div style="color: #4a5568; font-weight: bold; font-size: 1.1rem;">👥 מנויים פעילים באתר</div>
            </div>

            <div style="flex: 1; min-width: 200px; background: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; text-align: center; box-shadow: 0 4px 10px rgba(0,0,0,0.05);">
                <div style="font-size: 2.5rem; font-weight: bold; color: #1e3c72; margin-bottom: 5px;" id="countDays">0</div>
                <div style="color: #4a5568; font-weight: bold; font-size: 1.1rem;">⏱️ ימי השאלה מותרים לספר</div>
            </div>
        </div>
        <br />
        <div style="background: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; padding: 25px; box-shadow: 0 4px 12px rgba(0,0,0,0.08); display: flex; flex-direction: column; align-items: center; text-align: center;">
            <span style="background: #ffc107; color: #000; padding: 6px 18px; font-weight: bold; border-radius: 20px; font-size: 1rem; margin-bottom: 15px;">המלצת השבוע של נגה ⭐</span>
            
            <asp:Repeater ID="rptWeeklyRec" runat="server">
                <ItemTemplate>
                    <h4 style="margin: 1px 0; font-size: 1.3rem;"><%# Eval("MyBookName") %></h4>
                    <p style="color: #666; margin-bottom: 15px;">מאת: <%# Eval("MyAuthor") %></p>

                    <img src='Images/<%# Eval("MyBookImage") %>.jpg'
                        alt="book"
                        style="width: 190px; height: 300px; object-fit: cover; border-radius: 8px; margin-bottom: 20px;" />
                </ItemTemplate>
            </asp:Repeater>

            <a href="ShowBooks.aspx" style="background: #1e3c72; color: white; padding: 12px 25px; text-decoration: none; font-weight: bold; border-radius: 6px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);">מעבר לקטלוג והשאלה עכשיו 📖</a>
        </div>
        <div style="text-align: center; margin: 40px 0 20px 0;">
            <h2 style="color: #1e3c72; font-size: 25px; font-weight: bold; position: relative; display: inline-block; padding-bottom: 10px;">מה הקוראים שלנו מספרים?
       
                <span style="position: absolute; bottom: 0; right: 25%; left: 25%; height: 4px; background: #ffc107; border-radius: 2px;"></span>
            </h2>
        </div>

        <div style="display: flex; gap: 20px; justify-content: center; flex-wrap: wrap;">
            <asp:Repeater ID="rptComments" runat="server">
                <ItemTemplate>
                    <div style="width: 250px; background: #fff; border: 1px solid #ccc; padding: 20px; text-align: center;">

                        <div style="background: #1e3c72; color: #fff; width: 50px; height: 50px; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin: 0 auto 10px;">
                            <%# Eval("MyName").ToString().Length > 0 ? Eval("MyName").ToString().Substring(0, 1) : "?" %>
                        </div>

                        <p>"<%# Eval("MyComment") %>"</p>
                        <strong><%# Eval("MyName") %></strong><br />
                        <small><%# Eval("MyNickname") %></small>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <br />
        <br />

        <div style="background: #f0f4f8; padding: 30px; border-radius: 12px; border: 1px solid #d1d9e6; max-width: 600px; margin: 0 auto 40px auto; box-shadow: 0 4px 15px rgba(0,0,0,0.1);">
            <h4 style="color: #1e3c72; margin-top: 0;">גם לך יש המלצה? נשמח לשמוע!</h4>

            <div style="display: flex; flex-direction: column; gap: 10px;">
                <asp:TextBox ID="txtUserName" runat="server" placeholder="השם שלך" Style="padding: 12px; border: 1px solid #ccc; border-radius: 6px;"></asp:TextBox>
                <asp:TextBox ID="txtUserNickname" runat="server" placeholder="כינוי (למשל: תולעת ספרים)" Style="padding: 12px; border: 1px solid #ccc; border-radius: 6px;"></asp:TextBox>
                <asp:TextBox ID="txtUserComment" runat="server" placeholder="כתוב את ההמלצה כאן" Style="padding: 12px; border: 1px solid #ccc; border-radius: 6px;"></asp:TextBox>

                <asp:Button ID="btnSubmit" runat="server" Text="שלח המלצה" OnClick="btnSubmit_Click" Style="background: #1e3c72; color: white; padding: 12px; border: none; border-radius: 6px; font-weight: bold; cursor: pointer; transition: background 0.3s;" onmouseover="this.style.background='#162a56';" onmouseout="this.style.background='#1e3c72';" />
                <br />
                <asp:Label ID="lblStatus" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>
        </div>
    </div>

    <script>
        // 1. פונקציית ברכת השעה הקיימת שלך
        function setGreeting() {
            var hour = new Date().getHours();
            var greeting = "";

            if (hour >= 5 && hour < 12) {
                greeting = "☀️ בוקר טוב! זמן מעולה לקרוא משהו חדש עם הקפה ☕";
            } else if (hour >= 12 && hour < 18) {
                greeting = "✨ צהריים טובים! הפסקה קטנה עם ספר טוב תמיד עוזרת 📖";
            } else if (hour >= 18 && hour < 22) {
                greeting = "🌆 ערב טוב! מאחלים לך קריאה מהנה ומרגיעה";
            } else {
                greeting = "🌙 לילה טוב! אל תשכחו לקרוא פרק אחד לפני השינה";
            }

            document.getElementById("greetingText").innerHTML = greeting;
        }

        // 2. פונקציה חכמה שמריצה מספרים מאפס למעלה
        function animateCounter(elementId, targetValue, duration) {
            var obj = document.getElementById(elementId);
            var startValue = 0;
            var startTime = null;

            function step(timestamp) {
                if (!startTime) startTime = timestamp;
                var progress = timestamp - startTime;
                // חישוב הערך הנוכחי יחסית לזמן שעבר
                var currentValue = Math.min(Math.floor((progress / duration) * targetValue), targetValue);

                // הוספת סימן פלוס למספרים הגדולים
                obj.innerHTML = (targetValue > 50) ? "+" + currentValue : currentValue;

                if (progress < duration) {
                    window.requestAnimationFrame(step);
                }
            }
            window.requestAnimationFrame(step);
        }

        // 3. הפעלת הכל יחד כשהדף נטען
        window.onload = function () {
            setGreeting(); // מפעיל את הברכה החכמה

            // מריץ את שלושת הקאונטרים (מזהה אלמנט, מספר יעד, משך זמן במילישניות)
            animateCounter("countBooks", 1250, 1500); // ירוץ עד 1250 ספרים
            animateCounter("countUsers", 420, 1500);  // ירוץ עד 420 מנויים
            animateCounter("countDays", 60, 1500);    // ירוץ עד 60 ימים (חודשיים השאלה!)
        };

        function addComment() {
            var name = document.getElementById("userName").value;
            var text = document.getElementById("userComment").value;

            if (name === "" || text === "") {
                alert("נא למלא שם ותגובה");
                return;
            }

            // יצירת הקוביה החדשה בצורה דינמית
            var container = document.getElementById("newCommentsContainer");
            var newDiv = document.createElement("div");
            newDiv.style.cssText = "flex: 1; min-width: 250px; background: #fff59d; border-radius: 8px; padding: 20px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); margin-top: 20px;";

            newDiv.innerHTML = `
        <p style="font-style: italic;">"${text}"</p>
        <strong>- ${name}</strong>
    `;

            container.appendChild(newDiv);

            // ניקוי הטופס
            document.getElementById("userName").value = "";
            document.getElementById("userComment").value = "";
        }
    </script>
</asp:Content>
