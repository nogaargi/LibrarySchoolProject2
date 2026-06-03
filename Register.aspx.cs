using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace LibraryClient
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ה-if הזה מוודא שהרשימה תיטען רק בפעם הראשונה שהדף עולה
            if (!IsPostBack)
            {
                LoadCities();
            }
        }

        private void LoadCities()
        {
            try
            {
                // התחברות לסרוויס באמצעות הפרוקסי
                MyProxy.LibraryService proxy = new MyProxy.LibraryService();

                // קריאה לפונקציה החדשה שיצרנו בסרוויס
                DataSet ds = proxy.GetCities();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    ddlCities.DataSource = ds.Tables[0];
                    ddlCities.DataValueField = "MyCityID";    // הקוד המספרי
                    ddlCities.DataTextField = "MyCityName";   // שם העיר להצגה
                    ddlCities.DataBind();
                }

                // הוספת שורת ברירת המחדל - שים לב שהיא בתוך הסוגריים של הפונקציה!
                ddlCities.Items.Insert(0, new ListItem("-- בחר עיר מגורים --", "0"));
            }
            catch (Exception ex)
            {
                //lblMsg.Text = "שגיאה בטעינת רשימת הערים: " + ex.Message;
                Response.Write(ex.ToString());
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            // 2. וולידציות נוספות בשרת (למקרה שעקפו את הלקוח)
            if (string.IsNullOrWhiteSpace(txtUserName.Text) || txtPassword.Text.Length < 6)
            {
                lblMsg.Text = "שם משתמש ריק או סיסמה קצרה מדי!";
                lblMsg.ForeColor = System.Drawing.Color.Red;
                return;
            }
            try
            {
                // בדיקה אבטחתית: ודאות שנבחרה עיר מגורים חוקית
                if (ddlCities.SelectedValue == "0")
                {
                    lblMsg.Text = "חובה לבחור עיר מגורים כדי להירשם!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // 1. איסוף הנתונים מהתיבות
                string fName = txtFirstName.Text;
                string lName = txtLastName.Text;
                string user = txtUserName.Text;
                string pass = txtPassword.Text;
                string phone = txtPhone.Text;
                string bDate = txtBirthDate.Text;

                // שליפת קוד העיר שנבחרה מה-DropDownList!
                string userCityID = ddlCities.SelectedValue;

                // 2. קריאה לסרוויס - הוספנו את המשתנה userCityID בסוף הפרמטרים!
                MyProxy.LibraryService proxy = new MyProxy.LibraryService();
                // במקום להסתמך רק על "גדול מ-0", נפריד את השגיאות:
                int newUserID = proxy.InsertUser(fName, lName, user, userCityID, pass, phone, bDate);

                if (newUserID > 0)
                {
                    // הצלחה
                    lblMsg.Text = "נרשמת בהצלחה! מעביר לדף הבית...";
                    lblMsg.ForeColor = System.Drawing.Color.Green;
                    Session["userName"] = user;
                    Session["userID"] = newUserID;
                    Response.Redirect("Home.aspx");
                }
                else if (newUserID == 0) // במקרה של שגיאת שרת (למשל נתון לא תקין)
                {
                    lblMsg.Text = "ייתכן ששם המשתמש כבר תפוס, או שאחד מהנתונים שהזנת היה שגוי.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "שגיאה: " + ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}