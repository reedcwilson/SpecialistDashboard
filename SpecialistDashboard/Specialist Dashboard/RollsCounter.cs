using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class RollsCounter
    {
        public DataContext Data_Context { get; set; }
        public QueueNumbers QueueNumbers { get; set; }

        public RollsCounter()
        {
            Data_Context = new DataContext("epdb01", "JWF_Live");
        }

        public QueueNumbers GetQueueNumbers(string projectId)
        {
            string sql = GetSQL(projectId);
            var reader = Data_Context.RunSelectSQLQuery(sql, 30);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int scan = reader.GetInt32(0);
                    int framing = reader.GetInt32(1);
                    int mekelExtracting = reader.GetInt32(2);
                    int imageQA = reader.GetInt32(3);
                    int imageQE = reader.GetInt32(4);
                    int imageProcess = reader.GetInt32(5);
                    int gridlines = reader.GetInt32(6);
                    int batchValidating = reader.GetInt32(7);

                    this.QueueNumbers = new QueueNumbers(scan, framing, mekelExtracting, batchValidating, imageProcess, imageQA, imageQE, gridlines);
                }
                Data_Context.CloseConnection();
                return this.QueueNumbers;
            }
            else return null;
        }

        public string GetSQL(string projectId)
        {
            string sql = @"SELECT DISTINCT
            
                                    
                            COUNT(WI2.WorkItemID) Scan
                                    ,COUNT(WI3.WorkItemID) Framing
                                    ,COUNT(WI4.WorkItemID) MekelExtract
                                    ,COUNT(WI5.WorkItemID) ImageQA
                                    ,COUNT(WI6.WorkItemID) ImageQE
                                    ,COUNT(WI7.WorkItemID) ImageProcess
                                    ,COUNT(WI8.WorkItemID) Gridlines
                                    ,COUNT(WI9.WorkItemID) BatchValidating ";
            if (projectId != "") sql += @"                    
                                    ,R.ProjectID ";

            sql += @"               FROM WorkItem WI1 (NOLOCK) 
	                                    LEFT JOIN WorkItem WI2 (NOLOCK) ON WI1.WorkItemID = WI2.WorkItemID AND WI2.QueueId = 6
	                                    LEFT JOIN WorkItem WI3 (NOLOCK) ON WI1.WorkItemID = WI3.WorkItemID AND WI3.QueueId = 32
	                                    LEFT JOIN WorkItem WI4 (NOLOCK) ON WI1.WorkItemID = WI4.WorkItemID AND WI4.QueueId = 31
	                                    LEFT JOIN WorkItem WI5 (NOLOCK) ON WI1.WorkItemID = WI5.WorkItemID AND WI5.QueueId = 8
	                                    LEFT JOIN WorkItem WI6 (NOLOCK) ON WI1.WorkItemID = WI6.WorkItemID AND WI6.QueueId = 14
	                                    LEFT JOIN WorkItem WI7 (NOLOCK) ON WI1.WorkItemID = WI7.WorkItemID AND WI7.QueueId = 3
	                                    LEFT JOIN WorkItem WI8 (NOLOCK) ON WI1.WorkItemID = WI8.WorkItemID AND WI8.QueueId = 89
                                        LEFT JOIN WorkItem WI9 (NOLOCK) ON WI1.WorkItemID = WI9.WorkItemID AND WI9.QueueId = 11 ";
            if (projectId != "") sql += @"
                                        LEFT JOIN Dexter_DeeDee..Roll R (NOLOCK) ON R.RollName = WI1.WorkItemName ";

            sql += @"               WHERE WI1.Type = 'ROLL'
	                                    AND (WI1.QueueId = 6
		                                    OR WI1.QueueId = 32
		                                    OR WI1.QueueId = 31
		                                    OR WI1.QueueId = 8
		                                    OR WI1.QueueId = 14
		                                    OR WI1.QueueId = 3
		                                    OR WI1.QueueId = 89
                                            OR WI1.QueueId = 11)";
            if (projectId != "") sql += @"	
                                        AND R.ProjectID = '" + projectId + @"'
                                    GROUP BY R.ProjectID";

            return sql;
        }
    }
}
