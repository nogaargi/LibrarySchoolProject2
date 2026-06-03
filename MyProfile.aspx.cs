using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LibraryClient
{
    public partial class MyProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. בדיקת אבטחה: אם המשתמש לא מחובר, זרוק אותו ללוגין
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            // 2. ביצוע הפעולות רק בפעם הראשונה שהדף נטען
            if (!IsPostBack)
            {
                // טעינת הלוואות (הקוד המקורי שלך)
                ShowMyLoans();

                // בדיקת הסל (הקוד המקורי שלך)
                if (Session["Basket"] != null)
                {
                    DataTable basket = (DataTable)Session["Basket"];
                    gvBasket.DataSource = basket;
                    gvBasket.DataBind();
                    btnCheckout.Visible = (gvBasket.Rows.Count > 0);
                }
                else
                {
                    gvBasket.DataSource = null;
                    gvBasket.DataBind();
                    btnCheckout.Visible = false;
                }

                // 3. טעינת הנתונים החדשים למודל עריכת פרופיל
                // קודם ממלאים את רשימת הערים, אח"כ מושכים את פרטי המשתמש
                BindCitiesList();
                LoadMemberDetails();
            }
        }

        private void BindCitiesList()
        {
            ddlCities.Items.Clear();

            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                string sql = "SELECT MyCityID, MyCityName FROM MyCitiesTbl";
                OleDbCommand cmd = new OleDbCommand(sql, conn);

                try
                {
                    conn.Open();
                    // ... שאר הקוד נשאר בדיוק אותו דבר ...
                    OleDbDataReader reader = cmd.ExecuteReader();
                    ddlCities.DataSource = reader;
                    ddlCities.DataTextField = "MyCityName";
                    ddlCities.DataValueField = "MyCityID";
                    ddlCities.DataBind();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    // נדפיס את השגיאה האמיתית כדי להבין מה קורה
                    ddlCities.Items.Add(new ListItem("Error: " + ex.Message, "0"));
                }
            }
        }

        private void LoadMemberDetails()
        {
            if (Session["userName"] != null)
            {
                MyProxy.LibraryService proxy = new MyProxy.LibraryService();
                string user = Session["userName"].ToString();
                string[] details = proxy.GetMemberDetails(user);

                txtEditFName.Text = details[0];
                txtEditLName.Text = details[1];
                txtEditPhone.Text = details[3];

                // כאן התיקון:
                // נקה בחירה קודמת
                ddlCities.ClearSelection();
                // בחר את העיר לפי ה-ID שהגיע מה-DB (זה details[2])
                if (ddlCities.Items.FindByValue(details[2]) != null)
                {
                    ddlCities.SelectedValue = details[2];
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            MyProxy.LibraryService proxy = new MyProxy.LibraryService();
            string user = Session["userName"].ToString();

            // הוספת השורה הזו פותרת את השגיאה:
            string selectedCity = ddlCities.SelectedValue;

            // עכשיו משתמשים במשתנה שהגדרנו:
            bool success = proxy.UpdateMemberDetails(user, txtEditFName.Text, txtEditLName.Text, selectedCity, txtEditPhone.Text);

            if (success)
            {
                Response.Write("<script>alert('הפרטים עודכנו בהצלחה!');</script>");
                // אופציונלי: סגירת המודל אחרי שמירה
                Response.Write("<script>hideModal();</script>");
            }
            else
            {
                Response.Write("<script>alert('שגיאה בעדכון.');</script>");
            }
        }

        private void ShowMyLoans()
        {
            try
            {
                // התחברות לסרוויס באמצעות הפרוקסי
                MyProxy.LibraryService proxy = new MyProxy.LibraryService();

                // שליפת ה-ID של המשתמש המחובר מה-Session
                string memberID = Session["UserID"].ToString();

                // קבלת הנתונים המעודכנים מהסרוויס (רק ספרים שטרם הוחזרו)
                DataSet ds = proxy.GetUserLoans(memberID);

                // מקשרים את הנתונים לטבלה
                if (ds != null && ds.Tables["UserLoans"] != null)
                {
                    gvLoans.DataSource = ds.Tables["UserLoans"];
                    gvLoans.DataBind();
                }
                else
                {
                    // אם ה-DataSet ריק, נותנים לטבלה null כדי שתפעיל את ה-EmptyDataText
                    gvLoans.DataSource = null;
                    gvLoans.DataBind();
                }
            }
            catch (Exception ex)
            {
                // במקרה של שגיאה חריגה אפשר להקפיץ alert או להשאיר ריק
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"alert('שגיאה בטעינת הנתונים: {ex.Message}');", true);
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            // 1. משיכת הסלסלה מה-Session
            DataTable basket = (DataTable)Session["Basket"];

            // הגנה: אם הסלסלה ריקה - מקפיצים התראה, מעלימים כפתור ועוצרים
            if (basket == null || basket.Rows.Count == 0)
            {
                string scriptEmpty = "alert('הסלסלה שלך ריקה, אין מה להשאיל!');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", scriptEmpty, true);
                btnCheckout.Visible = false;
                return;
            }

            // 2. הגבלת כמות הספרים ל-4 בסך הכל בו זמנית
            int currentlyLoaned = gvLoans.Rows.Count; // כמה מושאלים כבר יש בטבלה למעלה
            int tryingToLoan = basket.Rows.Count;     // כמה ספרים יש כרגע בסלסלה

            if (currentlyLoaned + tryingToLoan > 4)
            {
                string scriptLimit = "alert('שגיאה: מותר להשאיל עד 4 ספרים בסך הכל בו זמנית!');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", scriptLimit, true);
                return; // עוצר כאן ולא ממשיך להשאיל!
            }

            // 3. התחברות ל-Web Service ושליפת ה-ID של המשתמש
            MyProxy.LibraryService proxy = new MyProxy.LibraryService();
            string memberID = Session["UserID"].ToString();

            // 4. לולאה שמבצעת את ההשאלה עבור כל ספר בסלסלה
            // 🌟 4. לולאה שמבצעת את ההשאלה עבור כל ספר בסלסלה (עם חסימת כפילויות)
            bool hadDuplicateLoans = false;
            string blockedBooksNames = "";

            foreach (DataRow row in basket.Rows)
            {
                int bookID = Convert.ToInt32(row["MyBookID"]);

                // בדיקה מול הסרוויס: האם הספר כבר מושאל אצלו כרגע?
                if (proxy.IsBookAlreadyLoaned(bookID, memberID))
                {
                    hadDuplicateLoans = true;
                    // נאסוף את שם הספר (ודאי שזה שם העמודה של השם ב-DataTable של הסלסלה שלך)
                    blockedBooksNames += row["MyBookName"].ToString() + ", ";
                    continue; // דילוג! לא משאילים את הספר הזה שוב
                }

                // אם הכל תקין והספר לא אצלו - מבצעים השאלה רגילה
                proxy.LoanBook(bookID, memberID);
            }

            // אם גילינו ספרים כפולים, נקפיץ התראה מרוכזת
            if (hadDuplicateLoans)
            {
                // מנקים את הפסיק המיותר שנשאר בסוף המחרוזת
                blockedBooksNames = blockedBooksNames.TrimEnd(',', ' ');

                string scriptDuplicate = $"alert('שים לב: הספרים הבאים לא הושאלו שוב מאחר והם כבר מושאלים לך: {blockedBooksNames}');";
                ClientScript.RegisterStartupScript(this.GetType(), "alertDuplicate", scriptDuplicate, true);
            }

            // 5. חישוב תאריך הגעה משוער (עוד 3 ימים) והצגת פאנל הסיכום
            DateTime arrivalDate = DateTime.Now.AddDays(3);

            lblDate.Text = "<b>📅 תאריך ההזמנה:</b> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            lblDestination.Text = "<b>📍 יעד משלוח:</b> נשלח אל עיר מגוריך הרשומה במערכת";
            lblArrival.Text = "<b>🚚 זמן הגעה משוער:</b> " + arrivalDate.ToString("dd/MM/yyyy") + " (עד השעה 16:00)";

            pnlSummary.Visible = true;

            // 6. התיקון המנצח: ריקון הסלסלה ורענון מיידי של המסך
            basket.Rows.Clear();             // מוחק את השורות מתוך ה-DataTable
            Session["Basket"] = basket;      // שומר את ה-DataTable הריק חזרה ב-Session
            Session["LoanedCount"] = 0;

            gvBasket.DataSource = basket;    // מקשר את ה-DataTable הריק לטבלה במסך
            gvBasket.DataBind();             // גורם לטבלה להתרענן מיד ולהציג "הסלסלה שלך ריקה כרגע"
            btnCheckout.Visible = false;     // מעלים את הכפתור הירוק

            // 7. עדכון אוטומטי של הטבלה העליונה (כדי שהספרים החדשים יופיעו שם מיד)
            ShowMyLoans();
        }

        protected void gvLoans_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // בודקים אם הלחיצה הגיעה מכפתור ההחזר שלנו
            if (e.CommandName == "ReturnBook")
            {
                // שליפת קוד ההשאלה מה-CommandArgument שהגדרנו ב-HTML
                int loanID = Convert.ToInt32(e.CommandArgument);

                // חיבור ל-Web Service שלך
                MyProxy.LibraryService proxy = new MyProxy.LibraryService();

                // קריאה לפונקציית ההחזרה מהסרוויס שלך (תוודאי שזה השם המדויק אצלך בסרוויס, למשל ReturnBook)
                proxy.ReturnBook(loanID);

                // הקפצת הודעה שהפעולה הצליחה
                string scriptSuccess = "alert('הספר הוחזר בהצלחה למלאי!');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", scriptSuccess, true);

                // רענון מיידי של הטבלה העליונה כדי שהספר שהוחזר יתעדכן או ייעלם (לפי הלוגיקה של השאילתה שלך)
                ShowMyLoans();
            }
        }

        protected void gvBasket_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // בודקים אם הלחיצה הגיעה מכפתור המחיקה שלנו
            if (e.CommandName == "RemoveFromBasket")
            {
                // 1. מוציאים את אינדקס השורה שנלחצה
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                // 2. מושכים את הסלסלה הנוכחית מה-Session
                DataTable basket = (DataTable)Session["Basket"];

                if (basket != null && basket.Rows.Count > rowIndex)
                {
                    // 3. מוחקים את השורה הספציפית מה-DataTable
                    basket.Rows[rowIndex].Delete();

                    // שמירה של השינוי בתוך ה-DataTable
                    basket.AcceptChanges();

                    // 4. שומרים את הסלסלה המעודכנת חזרה ב-Session
                    Session["Basket"] = basket;

                    // 5. מרעננים את ה-GridView במסך כדי שהשורה תיעלם מיד
                    gvBasket.DataSource = basket;
                    gvBasket.DataBind();

                    // הגנה קטנה: אם הסלסלה התרוקנה לגמרי בעקבות המחיקה, נעלים את כפתור הסיום
                    if (basket.Rows.Count == 0)
                    {
                        btnCheckout.Visible = false;
                        // אם יש לך פאנל שמקיף את הסלסלה, אפשר לטפל גם בו כאן
                    }
                }
            }
        }
    }
}