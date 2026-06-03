using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LibraryClient
{
    public partial class AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // שמירת זמן ההתחברות - בודקים אם המשתמש מחובר ואם הוא מנהל
            if (Session["IsAdmin"] == null || (bool)Session["IsAdmin"] == false)
            {
                Response.Redirect("Home.aspx"); // אם הוא לא מנהל – זורקים אותו חזרה לבית
            }

            if (!IsPostBack)
            {
                BindGrid();                // טעינת תגובות
                BindBooksGrid();           // טעינת ספרים
                BindMembersAndLoans();
            }
        }

        //private void BindWeeklyRecommendation()
        //{
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project\LibraryWebService\App_Data\NogaDataBase.accdb";

        //    using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
        //    {
        //        // שליפת הספר המסומן
        //        string query = "SELECT MyBookName, MyBookImage FROM MyBooksTbl WHERE IsRecommend = True";
        //        System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(query, conn);
        //        System.Data.DataTable dt = new System.Data.DataTable();
        //        da.Fill(dt);
        //    }
        //}

        private void BindMembersAndLoans()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                conn.Open();

                // משתמשים
                System.Data.OleDb.OleDbDataAdapter daM = new System.Data.OleDb.OleDbDataAdapter("SELECT MyMemberID, MyFirstName, MyLastName, IsAdmin FROM MyMembersTbl", conn);
                System.Data.DataTable dtM = new System.Data.DataTable();
                daM.Fill(dtM);
                gvMembers.DataSource = dtM;
                gvMembers.DataBind();

                // השאלות
                System.Data.OleDb.OleDbDataAdapter daL = new System.Data.OleDb.OleDbDataAdapter("SELECT MyLoanID, MyMemberID, MyBookID, IsReturned FROM MyLoansTbl", conn);
                System.Data.DataTable dtL = new System.Data.DataTable();
                daL.Fill(dtL);
                gvLoans.DataSource = dtL;
                gvLoans.DataBind();
            }
        }

        private void BindBooksGrid()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";
            string query = "SELECT MyBookID, MyBookName FROM MyBooksTbl";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(query, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                gvBooks.DataSource = dt;
                gvBooks.DataBind();
            }
        }

        private void BindGrid()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";
            string query = "SELECT * FROM MyCommentsTbl";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(query, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                gvComments.DataSource = dt;
                gvComments.DataBind();
            }
        }

        protected void gvMembers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int memberID = Convert.ToInt32(gvMembers.DataKeys[e.RowIndex].Value);

            // זה הנתיב המלא מהמחשב שלך - וודאי שהוא נכון ב-100%
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    // פקודה ישירה
                    string sql = "DELETE FROM MyMembersTbl WHERE MyMemberID = " + memberID;
                    System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(sql, conn);
                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    if (result > 0)
                    {
                        BindGrid();                // טעינת תגובות
                        BindBooksGrid();           // טעינת ספרים
                        BindMembersAndLoans();
                    }
                    else
                    {
                        // המחיקה רצה אבל לא מצאה שורה למחוק
                        Response.Write("<script>alert('לא נמצאה רשומה למחיקה!');</script>");
                    }
                }
                catch (Exception ex)
                {
                    // אם יש שגיאת הרשאות או נתיב לא נכון
                    Response.Write("<script>alert('שגיאה: " + ex.Message + "');</script>");
                }
            }
        }

        protected void gvLoans_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // קבלת ה-ID של ההשאלה למחיקה
            int loanID = Convert.ToInt32(gvLoans.DataKeys[e.RowIndex].Value);

            // מחיקה ממסד הנתונים
            DeleteFromDB("MyLoansTbl", "MyLoanID", loanID);

            BindGrid();                // טעינת תגובות
            BindBooksGrid();           // טעינת ספרים
            BindMembersAndLoans();
        }

        // פונקציית עזר למחיקה (אם לא קיימת, תוסיפי גם אותה)
        private void DeleteFromDB(string tableName, string idColumn, int idValue)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                string query = $"DELETE FROM {tableName} WHERE {idColumn} = ?";
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", idValue);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery(); // כאן אנחנו בודקים כמה שורות נמחקו

                if (rowsAffected == 0)
                {
                    // אם הגענו לכאן, המחיקה לא עבדה - נסי לבדוק שוב את שם הטבלה או ה-ID
                    throw new Exception("לא נמחקה אף שורה! בדקי את שם הטבלה או את ה-ID.");
                }
            }
        }

        protected void gvComments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // שולפים את ה-ID של התגובה מהטבלה
            int commentID = Convert.ToInt32(gvComments.DataKeys[e.RowIndex].Value);

            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";
            string query = "DELETE FROM MyCommentsTbl WHERE MyCommentID = ?";

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", commentID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // מרעננים את הטבלה אחרי המחיקה
            BindGrid();
        }

        protected void gvBooks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SetRec")
            {
                string bookId = e.CommandArgument.ToString();
                string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

                using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
                {
                    conn.Open();
                    // 1. מבטלים את ה-IsRecommend לכל הספרים
                    string resetQuery = "UPDATE MyBooksTbl SET IsRecommend = False";
                    new System.Data.OleDb.OleDbCommand(resetQuery, conn).ExecuteNonQuery();

                    // 2. מסמנים רק את הספר שנבחר כ-True
                    string setQuery = "UPDATE MyBooksTbl SET IsRecommend = True WHERE MyBookID = ?";
                    System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(setQuery, conn);
                    cmd.Parameters.AddWithValue("?", bookId);
                    cmd.ExecuteNonQuery();
                }
                BindBooksGrid(); // רענון הטבלה
            }
        }
    }
}