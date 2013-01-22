using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class HistoryReader
    {
        public DataContext Data_Context { get; set; }

        public HistoryReader()
        {
            Data_Context = new DataContext("epdb01", "JWF_Live");
        }

        public List<History> GetHistories(Roll roll)
        {
            string sql = HistorySql(roll);
            var reader = Data_Context.RunSelectSQLQuery(sql, 30);

            var histories = new List<History>();

            if (reader.HasRows)
            {
                var sReader = new SpecialistsReader();
                while (reader.Read())
                {
                    string uName = reader["UserName"] as string;
                    var spec = sReader.GetSpecialist(uName);
                    string m = reader["Message"] as string;
                    DateTime date = reader.GetDateTime(3);
                    string step = reader["CurrentQueue"] as string;

                    var note = new History(spec, m, date, step);
                    histories.Add(note);
                }
                Data_Context.CloseConnection();
                return histories;
            }
            else return null;
        }

        private string HistorySql(Roll roll)
        {
            string sql = @"SELECT WIH.CurrentQueue
		                            ,WIH.UserName
		                            ,WIH.Message
		                            ,WIH.MessageDate
                            FROM WorkItemHistory WIH (NOLOCK)
                            WHERE WIH.WorkItemID = " + roll.Id;
                  sql += @" ORDER BY MessageDate DESC";

            return sql;
        }
    }
}
