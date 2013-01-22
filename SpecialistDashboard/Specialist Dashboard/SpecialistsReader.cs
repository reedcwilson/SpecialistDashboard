using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Specialist_Dashboard
{
    class SpecialistsReader
    {
        public DataContext Data_Context { get { return new DataContext("epdb01", "eLaborDump");} }
        
        public SpecialistsReader()
        {
        }

        public Specialist GetSpecialist(string username = null, string fullName = null)
        {
            if (username != null || fullName != null)
            {
                foreach (var spec in GetSpecialists())
                {
                    if (username != null)
                    {
                        if (spec.Username != null)
                        {
                            if (@"myfamily\" + spec.Username.ToLower() == username.ToLower()) return spec;
                        }
                        else
                        {
                            var newSpec = new Specialist("System", "Process", username.Substring(9));
                            return newSpec;
                        }
                    }
                    else if (fullName != null)
                    {
                        if (spec.SpecialistName != null)
                            if (fullName.ToLower() == spec.SpecialistName.ToLower()) return spec;
                    }
                }
                return null;
            }
            else return null;
        }

        private List<Specialist> _specialists;
        public List<Specialist> GetSpecialists()
        {
            if (_specialists == null)
            {
                string sql = SpecialistsSQLString();
                SqlDataReader reader = Data_Context.RunSelectSQLQuery(sql, 30);

                var specialists = new List<Specialist>();

                if (reader.HasRows)
                {
                    List<Roll> rollList = new List<Roll>();
                    while (reader.Read())
                    {
                        string i = reader["employee_id"] as string;
                        int id = Convert.ToInt32(i);

                        string fName = reader["employee_firstname"] as string;
                        string lName = reader["employee_lastname"] as string;
                        string uName = reader["username"] as string;

                        Specialist spec = new Specialist(fName, lName, uName);

                        specialists.Add(spec);
                    }

                    //return specialists;
                    _specialists = specialists;
                }
                else return null;
            }
            Data_Context.CloseConnection();
            return _specialists;
        }

        private string SpecialistsSQLString()
        {
            string sql = @"SELECT employee_firstname
		                        ,employee_lastname
		                        ,username
		                        ,employee_id
		                        ,shift
		                        ,supervisor_employee_id
                        FROM ELaborDump.dbo.Roster R (NOLOCK)
                        WHERE location = 'Provo'
	                        AND termdate > GETDATE()
                            AND shift IS NOT NULL 
                        ORDER BY employee_lastname ";
            return sql;
        }
    }
}
