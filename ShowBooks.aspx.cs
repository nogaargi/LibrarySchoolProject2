using LibraryClient.MyProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LibraryClient
{
    public partial class ShowBooks1 : System.Web.UI.Page
    {
        // פונקציה חדשה שמרכזת את טעינת כל הספרים
        //public void ShowAllBooks()
        //{
        //    try
        //    {
        //        // תשתמשי ב-Namespace שקראת לו MyProxy, 
        //        // ואחריו את שם המחלקה כפי שהיא מופיעה בתוך ה-Web Reference
        //        MyProxy.LibraryService myServer = new MyProxy.LibraryService();
        //        System.Data.DataSet ds = myServer.GetAllBooks();
        //        gvBooks.DataSource = ds.Tables[0];
        //        gvBooks.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write("שגיאה בטעינת הספרים: " + ex.Message);
        //    }
        //}

        public void ShowAllBooks()
        {
            // 1. מוודאים שהפאנל של הקטלוג מוצג
            pnlCatalog.Visible = true;
            pnlSearchResults.Visible = false; // מסתירים את החיפוש אם הוא היה פתוח

            try
            {
                MyProxy.LibraryService myServer = new MyProxy.LibraryService();
                DataSet ds = myServer.GetAllBooks(); // מביאים את כל הספרים בבת אחת

                // 2. ממלאים את כל ה-DataList-ים
                // את פשוט מבצעת DataBind לכל אחד מהם בהתאם למה שיש לך ב-HTML
                dlFantasy.DataSource = ds.Tables[0]; // או סינון לפי ז'אנר אם יש לך פונקציה מסננת
                dlFantasy.DataBind();

                dlDrama.DataSource = ds.Tables[0];
                dlDrama.DataBind();

                dlNonFiction.DataSource = ds.Tables[0];
                dlNonFiction.DataBind();

                dlSciFi.DataSource = ds.Tables[0];
                dlSciFi.DataBind();

                dlAutoBio.DataSource = ds.Tables[0];
                dlAutoBio.DataBind();

                dlHistRoman.DataSource = ds.Tables[0];
                dlHistRoman.DataBind();

                dlThriller.DataSource = ds.Tables[0];
                dlThriller.DataBind();

                dlScience.DataSource = ds.Tables[0];
                dlScience.DataBind();

                // וכך הלאה לכל ה-DataList-ים שבתוך ה-pnlCatalog
            }
            catch (Exception ex)
            {
                Response.Write("שגיאה בטעינת הקטלוג: " + ex.Message);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // אבטחה תמיד רצה
            if (Session["userName"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            // טעינה ראשונית בלבד
            if (!IsPostBack)
            {
                ShowAllBooks();

                MyProxy.LibraryService proxy = new MyProxy.LibraryService();

                // טעינת ספרי פנטזיה
                dlFantasy.DataSource = proxy.GetBooksByGenre("פנטזיה");
                dlFantasy.DataBind();

                // טעינת ספרי דרמה
                dlDrama.DataSource = proxy.GetBooksByGenre("דרמה");
                dlDrama.DataBind();

                // טעינת ספרי עיון
                dlNonFiction.DataSource = proxy.GetBooksByGenre("עיון");
                dlNonFiction.DataBind();

                // טעינת ספרי מדע בדיוני
                dlSciFi.DataSource = proxy.GetBooksByGenre("מדע בדיוני");
                dlSciFi.DataBind();

                // טעינת אוטוביוגרפיה
                dlAutoBio.DataSource = proxy.GetBooksByGenre("אוטוביוגרפיה");
                dlAutoBio.DataBind();

                // טעינת רומן היסטורי
                dlHistRoman.DataSource = proxy.GetBooksByGenre("רומן היסטורי");
                dlHistRoman.DataBind();

                // טעינת מתח
                dlThriller.DataSource = proxy.GetBooksByGenre("מתח");
                dlThriller.DataBind();

                // טעינת מדעי
                dlScience.DataSource = proxy.GetBooksByGenre("מדע");
                dlScience.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            MyProxy.LibraryService proxy = new MyProxy.LibraryService();
            string field = ddlSearchType.SelectedValue;
            string value = txtSearch.Text;

            DataSet ds = proxy.GetBooksBySearch(field, value);

            // מציגים רק את תוצאות החיפוש ומסתירים את הקטלוג הראשי
            dlSearchResults.DataSource = ds.Tables[0];
            dlSearchResults.DataBind();

            pnlSearchResults.Visible = true;
            pnlCatalog.Visible = false;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            // מחזירים למצב הקטלוג הרגיל
            pnlSearchResults.Visible = false;
            pnlCatalog.Visible = true;
        }

        protected void dlBooks_ItemCommand(object source, DataListCommandEventArgs e)
        {
            // 1. בדיקת הגבלה ל-4 ספרים
            int loanedCount = 0;
            if (Session["LoanedCount"] != null)
            {
                loanedCount = (int)Session["LoanedCount"];
            }

            if (loanedCount >= 4)
            {
                string scriptLimit = "alert('לא ניתן להשאיל יותר מ-4 ספרים!');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", scriptLimit, true);
                return;
            }

            // 2. בדיקה קריטית: האם המשתמש מחובר? (מונע את שגיאת ה-NullReference)
            if (Session["UserID"] == null)
            {
                string scriptLogin = "alert('עליך להתחבר למערכת כדי להשאיל ספרים'); window.location='Login.aspx';";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", scriptLogin, true);
                return;
            }

            //Response.Write("<script>alert('The UserID in Session is: " + Session["UserID"] + "');</script>");
            // 3. אם הכל תקין – הוספה לסלסלת ההשאלות הזמנית ב-Session
            int bookID = Convert.ToInt32(e.CommandArgument);
            MyProxy.LibraryService proxy = new MyProxy.LibraryService();

            // 🌟 שליפת ה-DataSet מהסרוויס ומשיכת הטבלה הראשונה מתוכו [Tables[0]]
            System.Data.DataSet dsBooks = proxy.GetAllBooks();
            System.Data.DataTable dtAllBooks = dsBooks.Tables[0];

            string bookName = "ספר כלשהו";
            string author = "סופר כלשהו";

            // רצים על הטבלה כדי למצוא את השם והסופר של הספר שנבחר
            foreach (System.Data.DataRow bookRow in dtAllBooks.Rows)
            {
                if (Convert.ToInt32(bookRow["MyBookID"]) == bookID)
                {
                    bookName = bookRow["MyBookName"].ToString();
                    author = bookRow["MyAuthor"].ToString();
                    break;
                }
            }

            // אשף יצירת הסלסלה ב-Session (אם היא עדיין לא קיימת)
            if (Session["Basket"] == null)
            {
                DataTable basket = new DataTable();
                basket.Columns.Add("MyBookID", typeof(int));
                basket.Columns.Add("MyBookName", typeof(string));
                basket.Columns.Add("MyAuthor", typeof(string));

                Session["Basket"] = basket;
            }

            DataTable currentBasket = (DataTable)Session["Basket"];

            // בדיקה שהספר לא קיים כבר בעגלה
            bool alreadyInBasket = false;
            foreach (DataRow row in currentBasket.Rows)
            {
                if (Convert.ToInt32(row["MyBookID"]) == bookID)
                {
                    alreadyInBasket = true;
                    break;
                }
            }

            if (alreadyInBasket)
            {
                string scriptExists = "alert('הספר הזה כבר נמצא בסלסלת ההשאלות שלך!');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", scriptExists, true);
                return;
            }

            // הכנסת הנתונים המלאים של הספר לשורה בסלסלה
            DataRow dr = currentBasket.NewRow();
            dr["MyBookID"] = bookID;
            dr["MyBookName"] = bookName;
            dr["MyAuthor"] = author;
            currentBasket.Rows.Add(dr);

            // עדכון ה-Session והמונים
            // עדכון ה-Session והמונים
            Session["Basket"] = currentBasket;
            Session["LoanedCount"] = loanedCount + 1;
        }
    }
}