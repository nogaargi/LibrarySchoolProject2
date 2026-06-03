using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LibraryClient
{
    public partial class Home1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. הלוגיקה הקיימת שלך לברכה
            if (Session["firstName"] != null)
            {
                lblWelcome.Text = "שלום " + Session["firstName"].ToString() + "!";
            }
            else
            {
                lblWelcome.Text = "ברוכים הבאים לספרייה של נגה!";
            }

            // 2. הוספת הלוגיקה החדשה ל-Repeater (רק אם זה לא ריענון דף)
            if (!IsPostBack)
            {
                BindComments();
                BindWeeklyRecommendation(); // קריאה לפונקציה החדשה
            }
        }

        private void BindWeeklyRecommendation()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                // שליפת הספר המסומן בלבד
                string query = "SELECT MyBookName, MyAuthor, MyBookImage FROM MyBooksTbl WHERE IsRecommend = True";
                System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(query, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);

                // חיבור הנתונים ל-Repeater
                rptWeeklyRec.DataSource = dt;
                rptWeeklyRec.DataBind();
            }
        }

        protected void btnGoToBooks_Click(object sender, EventArgs e)
        {
            Response.Redirect("ShowBooks.aspx");
        }

        private void BindComments()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                string query = "SELECT TOP 4 MyName, MyComment, MyNickname FROM MyCommentsTbl ORDER BY MyCommentID DESC";
                System.Data.OleDb.OleDbDataAdapter adapter = new System.Data.OleDb.OleDbDataAdapter(query, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                adapter.Fill(dt);
                rptComments.DataSource = dt;
                rptComments.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // 1. ניקוי הודעה קודמת
            lblStatus.Text = "";

            // 2. בדיקה אם השדות ריקים
            if (string.IsNullOrWhiteSpace(txtUserName.Text) ||
                string.IsNullOrWhiteSpace(txtUserNickname.Text) ||
                string.IsNullOrWhiteSpace(txtUserComment.Text))
            {
                lblStatus.Text = "נא למלא את כל השדות!";
                return; // עוצר כאן ולא מנסה לשמור כלום
            }

            // 3. שומרים לבסיס הנתונים
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                string query = "INSERT INTO MyCommentsTbl (MyName, MyNickname, MyComment, MyCommentDate) VALUES (?, ?, ?, ?)";
                using (System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", txtUserName.Text);
                    cmd.Parameters.AddWithValue("?", txtUserNickname.Text);
                    cmd.Parameters.AddWithValue("?", txtUserComment.Text);
                    cmd.Parameters.AddWithValue("?", DateTime.Now.ToString());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // 4. מרעננים את העמוד כדי לראות את התגובה החדשה
            Response.Redirect("Home.aspx");
        }
    }
}