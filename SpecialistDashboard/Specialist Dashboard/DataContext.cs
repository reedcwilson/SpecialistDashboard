using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;

namespace Specialist_Dashboard
{
    public class DataContext
    {
        private SqlConnection myCon { get; set; }
        public string path { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }

        public DataContext(string server, string database)
        {
            //p = "Data Source=MyDatabase.sqlite;Version=3;";
            //path = p;
            Server = server;
            Database = database;
            makeSQLConnection(Server, Database);
        }

        private void makeSQLConnection(string server, string database)
        {
            myCon = new SqlConnection(GetConnectionString(server, database));
            try
            {
                myCon.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public void CloseConnection()
        {
            myCon.Close();
        }

        public SqlDataReader RunSelectSQLQuery(string sql, int timeout, DateTime? min = null, DateTime? max = null)
        {
            makeSQLConnection(Server, Database);
            SqlCommand mycommand = new SqlCommand(sql, myCon);

            mycommand.CommandTimeout = timeout;

            if (min != null && max != null)
            {
                mycommand.Parameters.Add("@min", System.Data.SqlDbType.DateTime).Value = min.Value;
                mycommand.Parameters.Add("@max", System.Data.SqlDbType.DateTime).Value = max.Value;
            }
            else if (min != null && max == null)
            {
                mycommand.Parameters.Add("@min", System.Data.SqlDbType.DateTime).Value = min.Value;
                mycommand.Parameters.Add("@max", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
            }

            var reader = mycommand.ExecuteReader();

            return reader;
        }

//        public long GetWorkItemId(string WorkItemName)
//        {
//            string sql = @"SELECT WI.WorkItemId
//                            FROM WorkItem WI (NOLOCK)
//                            WHERE WI.WorkItemName = '" + WorkItemName + @"'";

//            makeSQLConnection(Server, Database);
//            SqlCommand mycommand = new SqlCommand(sql, myCon);
//            long id = (long)mycommand.ExecuteScalar();
//            CloseConnection();

//            return id;
//        }

        public string ExecuteScalar(string sql, string server, string database)
        {
            makeSQLConnection(server, database);
            SqlCommand mycommand = new SqlCommand(sql, myCon);
            string result = (string)mycommand.ExecuteScalar();
            CloseConnection();

            return result;
        }

        //public void RunNonQuery(string sql)
        //{
        //    makeSQLConnection(path);
        //    SqlCommand mycommand = new SqlCommand(sql, myCon);
        //    int status = mycommand.ExecuteNonQuery();
        //    CloseConnection();
        //    // You can check status if you want to know if it succeeds
        //}

        public string GetConnectionString(string server, string database)
        {
            string connectStr = "";
            if (server.ToLower() == "epdb01")
                connectStr = "Server=EPDB01;";
            else if (server.ToLower() == "epdb02")
                connectStr = "Server=EPDB02;";
            else return null;

            connectStr += "Trusted_Connection=yes;";

            if (database.ToLower() == "jwf_live")
                connectStr += "database=JWF_Live;";
            else if (database.ToLower() == "dexter_deedee")
                connectStr += "database=Dexter_DeeDee;";
            else if (database.ToLower() == "elabordump")
                connectStr += "database=eLaborDump;";
            else if (database.ToLower() == "dexterimaging")
                connectStr += "database=DexterImaging;";
            else return null;
            connectStr += "connection timeout=120;";

            return connectStr;
        }

        //public bool DatabaseExists()
        //{
        //    return System.IO.File.Exists("MyDatabase.sqlite");
        //}
    }
}