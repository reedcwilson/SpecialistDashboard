using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Specialist_Dashboard
{
    class QueuesRollsReader
    {
        public DataContext Data_Context { get; set; }

        public QueuesRollsReader (DataContext data_context)
        {
            Data_Context = data_context;
        }

        public List<Roll> GetRolls(string step, string state, string priority, Specialist user, string project, string rollname)
        {
            string sql = MyRollsSQLString(step, state, priority, user, project, rollname);
            var reader = Data_Context.RunSelectSQLQuery(sql, 60);
            
            if (reader.HasRows)
            {
                List<Roll> rollList = new List<Roll>();
                while (reader.Read())
                {
                    var roll = RollReader(reader);
                    rollList.Add(roll);
                }
                Data_Context.CloseConnection();
                return rollList;
            }
            else return null;
        }

        public Roll GetRoll(string rollName)
        {
            string sql = MyRollsSQLString("", "", "", null, "", rollName);
            var reader = Data_Context.RunSelectSQLQuery(sql, 60);

            if (reader.HasRows)
            {
                var roll = RollReader(reader);
                return roll;
            }
            return null;
        }

        private Roll RollReader(SqlDataReader reader)
        {
            var id = reader["WorkItemID"];
            int i = Convert.ToInt32(id);
            DateTime date = reader.GetDateTime(7);

            string pId = reader["ProjectID"] as string;
            string rName = reader["Rollname"] as string;

            int? p = reader["Priority"] as int?;
            int prior = Convert.ToInt32(p);

            string uName = reader["UserName"] as string;
            string qName = reader["QueueName"] as string;
            string st = reader["State"] as string;

            var roll = new Roll(i, pId, rName, prior, uName, st, qName, date);

            return roll;
        }
        
        /// <summary>
        /// Finds the correct sql string based on caller
        /// </summary>
        private string MyRollsSQLString(string step, string state, string priority, Specialist user, string project, string rollname)
        {
            string sql = @"SELECT
                                    WI.WorkItemID
                                    ,R.ProjectID
                                    ,WI.WorkItemName Rollname
                                    ,WI.Priority
                                    ,WI.UserName
                                    ,Q.QueueName
                                    ,WI.State
                                    ,WI.LastUpdateTime
                        FROM WorkItem WI (NOLOCK)
                              LEFT JOIN Queue Q (NOLOCK) ON WI.QueueId = Q.QueueId
                              LEFT JOIN Dexter_DeeDee..Roll R (NOLOCK) ON R.RollName = WI.WorkItemName
                        WHERE WI.Type = 'Roll'";
            if (project != "")
                sql += @"     AND R.ProjectID = '" + project + "'";
            if (rollname != "")
                sql += @"     AND WI.WorkItemName like '%" + rollname + "%'";
            if (step != "")
            {
                if (step.ToLower() == "auditing")
                {
                    sql += @" AND (Q.QueueName = 'ImageQA'
                              OR Q.QueueName = 'ImageQE')";
                }
                else if (step.ToLower() == "incomplete")
                {
                    sql += @" AND Q.QueueName != 'BatchExporting'";
                }
                else
                    sql += @" AND Q.QueueName = '" + step + "'";
            }
            if (user != null)
                sql += @"     AND WI.UserName = 'myfamily\" + user.Username + "'";
            if (priority != "")
                sql += @"     AND WI.Priority = " + priority;
            if (state != "")
                sql += @"     AND WI.State = '" + state + "'";

            return sql;
        }
    }
}
