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
        public List<Roll> GetRolls(Specialist user, DateTime min, DateTime max, string step, string rollname = null)
        {
            //work on making this query faster and more accurate
            string sql = MyRollsSQLString(user, step, rollname);
            SqlDataReader reader = Data_Context.RunSelectSQLQuery(sql, 120, min, max);
            
            if (reader.HasRows)
            {
                List<Roll> rollList = new List<Roll>();
                while (reader.Read())
                {
                    //var id = reader.GetInt32(0);
                    var i = reader["WorkItemID"];
                    int id = Convert.ToInt32(i);
                    string pId = reader["ProjectID"] as string;
                    //string pId = Convert.ToString(pi);
                    string rName = reader["Rollname"] as string;
                    //string qHist = reader["MyQueue"] as string;
                    string qName = reader["CurrentQueue"] as string;
                    string st = reader["State"] as string;
                    var lUpdate = reader.GetDateTime(5);

                    var roll = new RollDetails(id, pId, rName, user, st, qName, step, lUpdate);
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
				                            ,R.RollName Rollname
                                            ,R.ProjectId
				                            ,WI.State
				                            ,Q.QueueName CurrentQueue
				                            ,WI.LastUpdateTime
                            FROM Roll R (NOLOCK)
                            LEFT JOIN RollBatch RB (NOLOCK) ON R.RollID = RB.RollID
                            LEFT JOIN SOX.DetailedResult DR (NOLOCK) ON RB.RollBatchID = DR.RollBatchID
                            LEFT JOIN JWF_Live..WorkItem WI (NOLOCK) ON R.RollName = WI.WorkItemName
                            LEFT JOIN JWF_Live..Queue Q (NOLOCK) ON WI.QueueId = Q.QueueId";

                               sql += @" WHERE ";
            if (step != "")
                               sql += @"          DR.StepTypeID = '" + step + "'";
            
            if (rollname != "" && step != "")
                               sql += @"      AND R.RollName LIKE '%" + rollname + "%'";
            else if (rollname != "" && step == "")
                               sql += @"          R.RollName LIKE '%" + rollname + "%'";
            
            if (step != "" || rollname != "")
                               sql += @"      AND ";
                               sql += @"          DR.Operator = 'myfamily\" + user.Username + "'";
                               sql += @"      AND DR.LastDocDate BETWEEN CONVERT(DATETIME, @min) AND CONVERT(DATETIME, @max)
                                        ORDER BY R.RollName";

            return sql;
        }
    }
}
