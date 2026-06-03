using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace LibraryWebService
{
    /// <summary>
    /// Summary description for LibraryService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]


    //string clientConnString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project\LibraryClient\App_Data\NogaDatabase.accdb;";

    //string serviceConnString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\pc\Desktop\LibraryProject\Library_Project (web service)\LibraryWebService\App_Data\NogaDatabase.accdb;";

    public class LibraryService : System.Web.Services.WebService
    {

        // מחרוזת התחברות לקובץ האקסס
        string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + HttpContext.Current.Server.MapPath("~/App_Data/NogaDatabase.accdb");
        Connect db = new Connect();

        [WebMethod]
        public bool IsUserAdmin(string username)
        {
            // נתיב לבסיס הנתונים
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
            {
                // אנחנו מחפשים את עמודת IsAdmin לפי שם המשתמש
                string query = "SELECT IsAdmin FROM MyMembersTbl WHERE MyFirstName = ?";

                using (System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", username);

                    conn.Open();
                    object result = cmd.ExecuteScalar(); // מחזיר את הערך של IsAdmin

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToBoolean(result); // מחזיר true או false
                    }
                }
            }
            return false; // אם לא מצאנו או שאין הרשאה, חוזר false כברירת מחדל
        }

        [WebMethod]
        public string CheckConnection()
        {
            OleDbConnection conn = new OleDbConnection(connString);
            try
            {
                conn.Open(); // מנסה לפתוח את הקשר לבסיס הנתונים
                return "החיבור ל-NogaDataBase הצליח!";
            }
            catch (Exception ex)
            {
                return "שגיאה בחיבור: " + ex.Message;
            }
            finally
            {
                conn.Close(); // תמיד סוגרים את החיבור בסוף
            }
        }

        [WebMethod]
        public bool UpdateMemberDetails(string username, string fName, string lName, string city, string phone)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

            // שאילתת עדכון - שמות השדות חייבים להיות זהים למה שראינו בטבלה
            string sql = "UPDATE MyMembersTbl SET MyFirstName = ?, MyLastName = ?, MyCity = ?, MyPhone = ? WHERE MyMemberName = ?";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);

                    // הוספת הפרמטרים לפי הסדר המדויק של סימני השאלה בשאילתה
                    cmd.Parameters.AddWithValue("?", fName);
                    cmd.Parameters.AddWithValue("?", lName);
                    cmd.Parameters.AddWithValue("?", city);
                    cmd.Parameters.AddWithValue("?", phone);
                    cmd.Parameters.AddWithValue("?", username); // זה ה-WHERE, הוא חייב להיות אחרון!

                    int rowsAffected = cmd.ExecuteNonQuery();

                    // אם עודכנה לפחות שורה אחת - החזרנו אמת
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // במקרה של שגיאה (למשל בעיית חיבור) נחזיר שקר
                return false;
            }
        }

        //[WebMethod]
        //public string[] GetMemberDetails(string username)
        //{
        //    // במקום הנתיב המלא:
        //    // זה יפתור את הבעיה כי זה מחשב את הנתיב בכל פעם מחדש בצורה בטוחה
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

        //    string sql = "SELECT MyFirstName, MyLastName, MyCity, MyPhone FROM MyMembersTbl WHERE MyMemberName = ?";

        //    string[] details = new string[4]; // יצירת מערך עם 4 מקומות

        //    using (OleDbConnection conn = new OleDbConnection(connString))
        //    {
        //        OleDbCommand cmd = new OleDbCommand(sql, conn);
        //        cmd.Parameters.AddWithValue("?", username);

        //        conn.Open();
        //        OleDbDataReader reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            details[0] = reader["MyFirstName"].ToString();
        //            details[1] = reader["MyLastName"].ToString();
        //            details[2] = reader["MyCity"].ToString();
        //            details[3] = reader["MyPhone"].ToString();
        //        }
        //        conn.Close();
        //    }
        //    return details;
        //}

        [WebMethod]
        public string[] GetMemberDetails(string username)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
            string sql = "SELECT MyFirstName, MyLastName, MyCity, MyPhone FROM MyMembersTbl WHERE MyMemberName = ?";

            // מאתחלים מערך ריק למקרה של שגיאה או אי-מציאת משתמש
            string[] details = new string[4];

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("?", username);
                        conn.Open();

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                details[0] = reader["MyFirstName"].ToString();
                                details[1] = reader["MyLastName"].ToString();
                                details[2] = reader["MyCity"].ToString();
                                details[3] = reader["MyPhone"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // כאן ניתן להוסיף לוגיקה לתיעוד השגיאה במידת הצורך
                return null;
            }

            return details;
        }

        //[WebMethod]
        //public DataSet GetAllBooks()
        //{
        //    // משתמשים באותו נתיב שעבד ב-Login
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
        //    OleDbConnection conn = new OleDbConnection(connString);
        //    string sql = "SELECT * FROM MyBooksTbl";
        //    OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
        //    DataSet ds = new DataSet();
        //    da.Fill(ds);
        //    return ds;
        //}

        [WebMethod]
        public DataSet GetAllBooks()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
            string sql = "SELECT * FROM MyBooksTbl";
            DataSet ds = new DataSet();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
                    {
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAllBooks: " + ex.Message);
            }

            return ds;
        }

        [WebMethod]
        public DataSet GetBooksBySearch(string field, string value)
        {
            // רשימה לבנה (White List) של שדות מורשים - זה מונע שגיאות SQL
            if (field != "MyBookName" && field != "MyAuthor" && field != "MyGenre")
            {
                field = "MyBookName"; // ברירת מחדל בטוחה
            }

            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

            // שימוש בשיטה בטוחה עם סוגריים מרובעים
            string sql = "SELECT * FROM [MyBooksTbl] WHERE [" + field + "] LIKE ?";

            DataSet ds = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    // חשוב: נוסיף את ה-% לתוך הפרמטר ולא לתוך ה-SQL עצמו
                    // תמחקי את מה שכתוב שם ותכתבי בדיוק את זה:
                    cmd.Parameters.AddWithValue("?", value + "%");

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    try
                    {
                        da.Fill(ds, "Books");
                    }
                    catch (Exception ex)
                    {
                        // אם עדיין יש שגיאה, נדע בדיוק מה היא
                        throw new Exception("SQL Error: " + sql + " | Details: " + ex.Message);
                    }
                }
            }
            return ds;
        }

        //[WebMethod]
        //public DataSet GetBooksByName(string bookName)
        //{
        //    OleDbConnection conn = new OleDbConnection(connString);
        //    // השאילתה משתמשת ב-LIKE כדי למצוא ספרים שגם רק מכילים את הטקסט
        //    string sql = "SELECT * FROM MyBooksTbl WHERE MyBookName LIKE '%" + bookName + "%'";
        //    OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        da.Fill(ds, "Books");
        //        return ds;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        ////[WebMethod]
        ////public DataSet GetBooksByName(string bookName)
        ////{
        ////    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
        ////    // שימוש ב-? כפרמטר למניעת SQL Injection
        ////    string sql = "SELECT * FROM MyBooksTbl WHERE MyBookName LIKE ?";
        ////    DataSet ds = new DataSet();

        ////    try
        ////    {
        ////        using (OleDbConnection conn = new OleDbConnection(connString))
        ////        {
        ////            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
        ////            {
        ////                // הוספת ה-LIKE לפרמטר עצמו
        ////                cmd.Parameters.AddWithValue("?", "%" + bookName + "%");

        ////                using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
        ////                {
        ////                    da.Fill(ds, "Books");
        ////                }
        ////            }
        ////        }
        ////    }
        ////    catch (Exception)
        ////    {
        ////        return null;
        ////    }

        ////    return ds;
        ////}

        [WebMethod]
        public string LoginUser(string user, string pass)
        {
            try
            {
                string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

                // שימוש בשמות המדויקים שקיימים אצלך באקסס: MyMemberName ו-MyPassword!
                string sql = "SELECT MyMemberID FROM MyMembersTbl WHERE MyMemberName = ? AND MyPassword = ?";

                using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
                {
                    System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(sql, conn);
                    cmd.Parameters.Clear();

                    // הדרך הבטוחה ביותר ב-OleDb היא להשתמש בסימן שאלה גם כאן
                    cmd.Parameters.AddWithValue("?", user);
                    cmd.Parameters.AddWithValue("?", pass);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString(); // יחזיר את ה-ID האמיתי של המשתמש שמתחבר
                    }
                    return "NotFound";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        //[WebMethod]
        //public int InsertUser(string fName, string lName, string user, string cityID, string pass, string phone, string bDate)
        //{
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

        //    // השאילתה עם סימני שאלה במקום המשתנים
        //    string sql = "INSERT INTO MyMembersTbl (MyFirstName, MyLastName, MyMemberName, MyCity, MyPassword, MyPhone, MyJoinDate, MyBirthDate, IsAdmin) " +
        //                 "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

        //    try
        //    {
        //        using (OleDbConnection conn = new OleDbConnection(connString))
        //        {
        //            conn.Open();
        //            OleDbCommand cmd = new OleDbCommand(sql, conn);

        //            string checkSql = "SELECT COUNT(*) FROM MyMembersTbl WHERE MyMemberName = ?";
        //            OleDbCommand checkCmd = new OleDbCommand(checkSql, conn);
        //            checkCmd.Parameters.AddWithValue("?", user);
        //            int count = (int)checkCmd.ExecuteScalar();

        //            // תוסיפי את השורה הזו כדי לראות אם הוא בכלל מגיע לבסיס הנתונים
        //            System.Diagnostics.Debug.WriteLine("Count is: " + count);

        //            if (count > 0) return 0; // מחזירים 0 אם השם כבר קיים

        //            // הוספת הפרמטרים לפי הסדר של סימני השאלה
        //            cmd.Parameters.AddWithValue("?", fName);
        //            cmd.Parameters.AddWithValue("?", lName);
        //            cmd.Parameters.AddWithValue("?", user);
        //            cmd.Parameters.AddWithValue("?", cityID);
        //            cmd.Parameters.AddWithValue("?", pass);
        //            cmd.Parameters.AddWithValue("?", phone);
        //            cmd.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString());
        //            cmd.Parameters.AddWithValue("?", bDate);
        //            cmd.Parameters.AddWithValue("?", false); // IsAdmin = false

        //            cmd.ExecuteNonQuery();

        //            // החזרת ה-ID שנוצר
        //            cmd.CommandText = "SELECT @@IDENTITY";
        //            return Convert.ToInt32(cmd.ExecuteScalar());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // כאן כדאי לכתוב ללוג את השגיאה, לא רק להחזיר 0
        //        return 0;
        //    }
        //}

        [WebMethod]
        public int InsertUser(string fName, string lName, string user, string cityID, string pass, string phone, string bDate)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    // בדיקת כפילות שם משתמש
                    string checkSql = "SELECT COUNT(*) FROM MyMembersTbl WHERE MyMemberName = ?";
                    using (OleDbCommand checkCmd = new OleDbCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("?", user);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0) return 0;
                    }

                    // הוספת משתמש חדש
                    string sql = "INSERT INTO MyMembersTbl (MyFirstName, MyLastName, MyMemberName, MyCity, MyPassword, MyPhone, MyJoinDate, MyBirthDate, IsAdmin) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("?", fName);
                        cmd.Parameters.AddWithValue("?", lName);
                        cmd.Parameters.AddWithValue("?", user);
                        cmd.Parameters.AddWithValue("?", cityID);
                        cmd.Parameters.AddWithValue("?", pass);
                        cmd.Parameters.AddWithValue("?", phone);
                        cmd.Parameters.AddWithValue("?", DateTime.Now.ToShortDateString());
                        cmd.Parameters.AddWithValue("?", bDate);
                        cmd.Parameters.AddWithValue("?", false);

                        cmd.ExecuteNonQuery();

                        // שליפת ה-ID שנוצר
                        cmd.CommandText = "SELECT @@IDENTITY";
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception)
            {
                // כאן מומלץ לתעד את השגיאה
                return 0;
            }
        }

        //[WebMethod]
        //public DataSet GetBooksByGenre(string genre)
        //{
        //    // הגדרת החיבור למסד הנתונים שמצאנו ב-App_Data
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
        //    OleDbConnection conn = new OleDbConnection(connString);

        //    // שאילתה שמושכת רק ספרים מז'אנר מסוים
        //    string sql = "SELECT * FROM MyBooksTbl WHERE MyGenre = '" + genre + "'";

        //    OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
        //    DataSet ds = new DataSet();
        //    da.Fill(ds);
        //    return ds;
        //}

        [WebMethod]
        public DataSet GetBooksByGenre(string genre)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
            string sql = "SELECT * FROM MyBooksTbl WHERE MyGenre = ?";
            DataSet ds = new DataSet();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("?", genre);

                        using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return ds;
        }

        //[WebMethod]
        //public bool LoanBook(int bookID, string memberID)
        //{
        //    // 1. עדכון המלאי בטבלת הספרים
        //    string sqlUpdate = "UPDATE MyBooksTbl SET MyAvailable = MyAvailable - 1 WHERE MyBookID = " + bookID + " AND MyAvailable > 0";

        //    // 2. הוספת שורת השאלה לטבלת ההשאלות
        //    string dateToday = DateTime.Now.ToString("dd/MM/yyyy");
        //    string dateReturn = DateTime.Now.AddMonths(2).ToString("dd/MM/yyyy"); // מחשב חודשיים קדימה

        //    string sqlInsert = "INSERT INTO MyLoansTbl (MyBookID, MyMemberID, MyLoanDate, MyReturnDate) " +
        //                       "VALUES (" + bookID + ", " + memberID + ", '" + dateToday + "', '" + dateReturn + "')";

        //    int rowsUpdated = db.ExecuteNonQuery(sqlUpdate);
        //    if (rowsUpdated > 0)
        //    {
        //        db.ExecuteNonQuery(sqlInsert); // מבצע את הרישום רק אם העדכון הצליח
        //        return true;
        //    }
        //    return false;
        //}

        [WebMethod]
        public bool LoanBook(int bookID, string memberID)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    // שימוש בטרנזקציה כדי להבטיח אמינות נתונים
                    using (OleDbTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. עדכון המלאי
                            string sqlUpdate = "UPDATE MyBooksTbl SET MyAvailable = MyAvailable - 1 WHERE MyBookID = ? AND MyAvailable > 0";
                            using (OleDbCommand cmdUpdate = new OleDbCommand(sqlUpdate, conn, trans))
                            {
                                cmdUpdate.Parameters.AddWithValue("?", bookID);
                                if (cmdUpdate.ExecuteNonQuery() == 0) return false; // אין מלאי
                            }

                            // 2. הוספת השאלה
                            string sqlInsert = "INSERT INTO MyLoansTbl (MyBookID, MyMemberID, MyLoanDate, MyReturnDate) VALUES (?, ?, ?, ?)";
                            using (OleDbCommand cmdInsert = new OleDbCommand(sqlInsert, conn, trans))
                            {
                                cmdInsert.Parameters.AddWithValue("?", bookID);
                                cmdInsert.Parameters.AddWithValue("?", memberID);
                                cmdInsert.Parameters.AddWithValue("?", DateTime.Now.ToString("dd/MM/yyyy"));
                                cmdInsert.Parameters.AddWithValue("?", DateTime.Now.AddMonths(2).ToString("dd/MM/yyyy"));
                                cmdInsert.ExecuteNonQuery();
                            }

                            trans.Commit(); // הכל עבר בהצלחה
                            return true;
                        }
                        catch
                        {
                            trans.Rollback(); // ביטול הכל אם משהו השתבש
                            return false;
                        }
                    }
                }
            }
            catch { return false; }
        }

        //[WebMethod]
        //public bool ReturnBook(int loanID)
        //{
        //    // 1. עדכון סטטוס ההחזרה ל-true בטבלת ההשאלות עבור קוד ההשאלה הספציפי
        //    string sqlUpdateLoan = "UPDATE MyLoansTbl SET IsReturned = true WHERE MyLoanID = " + loanID;
        //    int rowsUpdated = db.ExecuteNonQuery(sqlUpdateLoan);

        //    // אם העדכון הראשון הצליח, נעדכן גם את המלאי
        //    if (rowsUpdated > 0)
        //    {
        //        // 2. 🌟 שאילתה חכמה: מעדכנים את המלאי של הספר שמתאים ל-loanID שקיבלנו
        //        string sqlUpdateStock = "UPDATE MyBooksTbl " +
        //                               "INNER JOIN MyLoansTbl ON MyBooksTbl.MyBookID = MyLoansTbl.MyBookID " +
        //                               "SET MyBooksTbl.MyAvailable = MyBooksTbl.MyAvailable + 1 " +
        //                               "WHERE MyLoansTbl.MyLoanID = " + loanID;

        //        // הרצת עדכון המלאי באותה הדרך בדיוק!
        //        db.ExecuteNonQuery(sqlUpdateStock);

        //        return true;
        //    }
        //    return false;
        //}

        [WebMethod]
        public bool ReturnBook(int loanID)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    using (OleDbTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. עדכון סטטוס החזרה
                            string sqlUpdateLoan = "UPDATE MyLoansTbl SET IsReturned = true WHERE MyLoanID = ?";
                            using (OleDbCommand cmd1 = new OleDbCommand(sqlUpdateLoan, conn, trans))
                            {
                                cmd1.Parameters.AddWithValue("?", loanID);
                                if (cmd1.ExecuteNonQuery() == 0) return false;
                            }

                            // 2. עדכון המלאי חזרה (שימוש ב-JOIN בטוח)
                            string sqlUpdateStock = "UPDATE MyBooksTbl INNER JOIN MyLoansTbl ON MyBooksTbl.MyBookID = MyLoansTbl.MyBookID SET MyBooksTbl.MyAvailable = MyBooksTbl.MyAvailable + 1 WHERE MyLoansTbl.MyLoanID = ?";
                            using (OleDbCommand cmd2 = new OleDbCommand(sqlUpdateStock, conn, trans))
                            {
                                cmd2.Parameters.AddWithValue("?", loanID);
                                cmd2.ExecuteNonQuery();
                            }

                            trans.Commit();
                            return true;
                        }
                        catch
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch { return false; }
        }

        //[WebMethod]
        //public bool IsBookAlreadyLoaned(int bookID, string memberID)
        //{
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

        //    // שאילתה שבודקת אם יש השאלה פעילה (עדיין לא הוחזרה) של המשתמש לספר הזה
        //    string sql = "SELECT MyLoanID FROM MyLoansTbl " +
        //                 "WHERE MyMemberID = " + memberID + " " +
        //                 "AND MyBookID = " + bookID + " " +
        //                 "AND IsReturned = false";

        //    System.Data.DataSet ds = new System.Data.DataSet();

        //    try
        //    {
        //        using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
        //        {
        //            System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(sql, conn);
        //            da.Fill(ds, "CheckLoan");
        //        }

        //        // אם חזרה לפחות שורה אחת, זה אומר שהספר כבר אצלו!
        //        if (ds != null && ds.Tables["CheckLoan"].Rows.Count > 0)
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // במקרה של שגיאה נחזיר false כדי לא לחסום סתם
        //        return false;
        //    }

        //    return false; // הספר לא מושאל כרגע
        //}

        [WebMethod]
        public bool IsBookAlreadyLoaned(int bookID, string memberID)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
            // שימוש בפרמטרים (?) למניעת הזרקת SQL
            string sql = "SELECT MyLoanID FROM MyLoansTbl WHERE MyMemberID = ? AND MyBookID = ? AND IsReturned = false";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        // הוספת הפרמטרים בצורה מאובטחת
                        cmd.Parameters.AddWithValue("?", memberID);
                        cmd.Parameters.AddWithValue("?", bookID);

                        conn.Open();

                        // שימוש ב-ExecuteScalar לבדיקה מהירה אם קיימת שורה
                        object result = cmd.ExecuteScalar();
                        return result != null;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //[WebMethod]
        //public System.Data.DataSet GetUserLoans(string memberID)
        //{
        //    // נתיב לבסיס הנתונים (משתמש בחיבור המקומי שלך)
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

        //    // שאילתה שמחברת בין טבלת ההשאלות לטבלת הספרים כדי להציג את שם הספר ולא רק מספרים
        //    string sql = "SELECT L.MyLoanID, B.MyBookName, L.MyLoanDate, L.MyReturnDate, L.IsReturned " +
        //                 "FROM MyLoansTbl AS L " +
        //                 "INNER JOIN MyBooksTbl AS B ON L.MyBookID = B.MyBookID " +
        //                 "WHERE L.MyMemberID = " + memberID + " AND L.IsReturned = false";

        //    System.Data.DataSet ds = new System.Data.DataSet();

        //    try
        //    {
        //        using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
        //        {
        //            System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(sql, conn);
        //            da.Fill(ds, "UserLoans");
        //        }
        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null; // במקרה של שגיאה נחזיר ריק כדי שהאתר לא יתרסק
        //    }
        //}

        [WebMethod]
        public System.Data.DataSet GetUserLoans(string memberID)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
            string sql = "SELECT L.MyLoanID, B.MyBookName, L.MyLoanDate, L.MyReturnDate, L.IsReturned " +
                         "FROM MyLoansTbl AS L " +
                         "INNER JOIN MyBooksTbl AS B ON L.MyBookID = B.MyBookID " +
                         "WHERE L.MyMemberID = ? AND L.IsReturned = false";

            System.Data.DataSet ds = new System.Data.DataSet();

            try
            {
                using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
                {
                    using (System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(sql, conn))
                    {
                        // שימוש בפרמטר מאובטח
                        cmd.Parameters.AddWithValue("?", memberID);

                        using (System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(cmd))
                        {
                            da.Fill(ds, "UserLoans");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return ds;
        }

        //[WebMethod]
        //public DataSet GetCities()
        //{
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
        //    string sql = "SELECT MyCityID, MyCityName FROM MyCitiesTbl ORDER BY MyCityName";

        //    DataSet ds = new DataSet();
        //    using (OleDbConnection conn = new OleDbConnection(connString))
        //    {
        //        OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
        //        da.Fill(ds, "MyCitiesTbl");
        //    }
        //    return ds;
        //}

        [WebMethod]
        public DataSet GetCities()
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");
            string sql = "SELECT MyCityID, MyCityName FROM MyCitiesTbl ORDER BY MyCityName";

            DataSet ds = new DataSet();

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
                    {
                        da.Fill(ds, "MyCitiesTbl");
                    }
                }
            }
            catch (Exception ex)
            {
                // כאן השגיאה נתפסת. השירות לא יקרוס!
                // אפשר לכתוב ללוג: System.Diagnostics.Debug.WriteLine(ex.Message);
                return null; // או להחזיר DataSet ריק
            }

            return ds;
        }

        [WebMethod]
        public string GetFirstName(string username)
        {
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("~/App_Data/NogaDatabase.accdb");

            // בואי נשתמש בשם השדה המדויק כפי שמופיע ב-DB שלך
            string sql = "SELECT MyFirstName FROM MyMembersTbl WHERE MyMemberName = ?";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?", username);

                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "לא נמצא";
                }
            }
            catch (Exception ex)
            {
                return "שגיאת DB: " + ex.Message;
            }
        }

        //public class Connect
        //{
        //    // השתמשתי בנתיב המדויק שמופיע אצלך בתמונות
        //    string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + HttpContext.Current.Server.MapPath("~/App_Data/NogaDatabase.accdb");

        //    public int ExecuteNonQuery(string sql)
        //    {
        //        using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connString))
        //        {
        //            System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(sql, conn);
        //            conn.Open();
        //            int rowsAffected = cmd.ExecuteNonQuery();
        //            conn.Close();
        //            return rowsAffected;
        //        }
        //    }
        //}

        public class Connect
        {
            private string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + HttpContext.Current.Server.MapPath("~/App_Data/NogaDatabase.accdb");

            // מתודה להרצת שאילתות שמשנות נתונים (Insert, Update, Delete)
            public int ExecuteNonQuery(string sql, OleDbParameter[] parameters)
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        conn.Open();
                        return cmd.ExecuteNonQuery();
                    }
                }
            }

            // מתודה להרצת שאילתות שמחזירות ערך בודד (למשל Login או חישובים)
            public object ExecuteScalar(string sql, OleDbParameter[] parameters)
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        conn.Open();
                        return cmd.ExecuteScalar();
                    }
                }
            }
        }
    }
}
