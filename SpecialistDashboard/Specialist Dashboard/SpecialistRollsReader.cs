using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Specialist_Dashboard
{
    class SpecialistRollsReader
    {
        public DataContext Data_Context { get; set; }

        public SpecialistRollsReader (DataContext data_context)
        {
            Data_Context = data_context;
        }

        /// <summary>
        /// Returns rolls a specialist scanned/audited in a given time period
        /// </summary>
        public List<Roll> GetRolls(Specialist user, DateTime min, DateTime max, string step = null, string rollname = null)
        {
            //work on making this query faster and more accurate
            string sql = MyRollsSQLString(user, step, rollname);
            SqlDataReader reader = Data_Context.RunSelectSQLQuery(sql, 120, min, max);
            
            if (reader.HasRows)
            {
                List<Roll> rollList = new List<Roll>();
                while (reader.Read())
                {
                    var i = reader["WorkItemID"];
                    int id = Convert.ToInt32(i);
                    var pi = reader["ProjectID"];
                    string pId = Convert.ToString(pi);
                    string rName = reader["Rollname"] as string;
                    string qHist = reader["MyQueue"] as string;
                    string qName = reader["CurrentQueue"] as string;
                    string st = reader["State"] as string;
                    var lUpdate = reader.GetDateTime(6);

                    var roll = new RollDetails(id, pId, rName, user, st, qName, qHist, lUpdate);
                    rollList.Add(roll);
                }
                Data_Context.CloseConnection();
                return rollList;
            }
            else return null;
        }
        
        /// <summary>
        /// Finds the correct sql string based on caller
        /// </summary>
        private string MyRollsSQLString(Specialist user, string step, string rollname)
        {
            string sql = @"SELECT DISTINCT 
                                            WI.WorkItemID
                                            ,R.ProjectID
                                            ,WI.WorkItemName Rollname
                                            ,Q.QueueName CurrentQueue
                                            ,WI.State
                                            ,WIH.CurrentQueue MyQueue
                                            ,WI.LastUpdateTime
                                    FROM WorkItem WI (NOLOCK)
                                        LEFT JOIN Dexter_DeeDee..Roll R (NOLOCK) ON WI.WorkItemName = R.RollName
                                        LEFT JOIN WorkItemHistory WIH (NOLOCK) ON WI.WorkItemID = WIH.WorkItemID
                                        LEFT JOIN Queue Q (NOLOCK) ON WI.QueueId = Q.QueueId";

                               sql += @" WHERE ";
            if (step != "")
                               sql += @"          WIH.CurrentQueue = '" + step + "'";
            
            if (rollname != "" && step != "")
                               sql += @"      AND WI.WorkItemName LIKE '%" + rollname + "%'";
            else if (rollname != "" && step == "")
                               sql += @"          WI.WorkItemName LIKE '%" + rollname + "%'";
            
            if (step != "" || rollname != "")
                               sql += @"      AND ";
                               sql += @"          WIH.UserName = 'myfamily\" + user.Username + "'";
                               sql += @"      AND WIH.MessageDate BETWEEN CONVERT(DATETIME, @min) AND CONVERT(DATETIME, @max)
                                        ORDER BY WI.WorkItemName";

            return sql;
        }
    }
}
