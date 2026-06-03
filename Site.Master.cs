using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LibraryClient
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. הגדרת ברירת מחדל: הכל מוסתר
            pnlGuest.Visible = false;
            pnlUser.Visible = false;
            pnlAdmin.Visible = false;

            // 2. בדיקה: האם המשתמש מחובר?
            if (Session["userName"] != null)
            {
                // אם מחובר, בודקים אם הוא מנהל
                if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"] == true)
                {
                    pnlAdmin.Visible = true; // מציגים את פאנל המנהל
                }
                else
                {
                    pnlUser.Visible = true; // מציגים פאנל משתמש רגיל
                }
            }
            else
            {
                // 3. אם לא מחובר - אורח
                pnlGuest.Visible = true;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // ניקוי המשתנים מהזיכרון של השרת
            Session.Clear();
            Session.Abandon();

            // שליחה לדף ההתחברות שלך
            Response.Redirect("Login.aspx");
        }
    }
}