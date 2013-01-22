using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class DailyNumbersReader
    {
        public DataContext Data_Context { get; set; }

        public DailyNumbersReader()
        {
            Data_Context = new DataContext("epdb01", "Dexter_DeeDee");
        }

        public List<RollNumbers> GetNumbers(Specialist spec, string step)
        {
            string sql = GetSQL(spec, step);
            var reader = Data_Context.RunSelectSQLQuery(sql, 30);
            var rolls = new List<RollNumbers>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string rName = reader["RollName"] as string;
                    int imgCount = reader.GetInt32(1);
                    int sElapsed = reader.GetInt32(2);
                    int sPaused = reader.GetInt32(3);

                    double imgPerHour = imgCount / (((sElapsed - sPaused) / 60.0) / 60.0);
                    imgPerHour = Math.Round(imgPerHour, 2);

                    int seconds = sElapsed - sPaused;

                    var numbers = new RollNumbers(rName, imgCount, imgPerHour, seconds);
                    rolls.Add(numbers);
                }
                Data_Context.CloseConnection();
                return rolls;
            }
            else return null;
        }
        private string GetSQL(Specialist spec, string step)
        {
            string sql = @"SELECT DISTINCT 
				                            R.RollName
				                            ,R.ImageCount
				                            ,DR.SecondsElapsed
				                            ,DR.SecondsPaused
                            FROM Roll R (NOLOCK)
                            LEFT JOIN RollBatch RB (NOLOCK) ON R.RollID = RB.RollID
                            LEFT JOIN SOX.DetailedResult DR (NOLOCK) ON RB.RollBatchID = DR.RollBatchID
                            WHERE DR.Operator = 'myfamily\" + spec.Username + @"'
	                            AND DR.StepTypeID = '" + step + @"'
	                            AND DR.LastDocDate > '" + System.DateTime.Today + "'";
                   sql += @"ORDER BY R.RollName";

            return sql;
        }
    }
}
