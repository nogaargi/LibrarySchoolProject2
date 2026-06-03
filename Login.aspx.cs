using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LibraryClient
{
    public partial class Login1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // שימוש בשם הפרוקסי הנכון שלך
            //MyProxy.LibraryService myService = new MyProxy.LibraryService();
            MyProxy.LibraryService proxy = new MyProxy.LibraryService(); 

            // שליחת הערכים מתיבות הטקסט המלאות שלך
            string id = proxy.LoginUser(txtUsername.Text, txtPassword.Text);

            if (id.StartsWith("Error"))
            {
                lblMsg.Text = "שגיאה: " + id;
            }
            else if (id == "NotFound")
            {
                lblMsg.Text = "שם משתמש או סיסמה שגויים";
            }
            else
            {
                Session["UserID"] = id;
                Session["userName"] = txtUsername.Text;

                // אחרי שאימתת שהמשתמש קיים ב-DB:
                //MyProxy.LibraryService myService = new MyProxy.LibraryService();
                //MyProxy.LibraryService proxy = new MyProxy.LibraryService();
                string fName = proxy.GetFirstName(txtUsername.Text);
                //string fName = "בדיקה";

                // שמירה ב-Session
                Session["firstName"] = fName;

                bool isAdmin = proxy.IsUserAdmin(txtUsername.Text);
                Session["IsAdmin"] = isAdmin;
                Response.Redirect("Home.aspx");
            }
        }
    }
}