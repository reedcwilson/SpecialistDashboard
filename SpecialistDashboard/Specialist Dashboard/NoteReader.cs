using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class NoteReader
    {
        public DataContext Data_Context { get; set; }

        public NoteReader()
        {
            Data_Context = new DataContext("epdb02", "DexterImaging");
        }

        public List<Note> GetNotes(Roll roll)
        {
            string sql = NotesSql(roll);
            var reader = Data_Context.RunSelectSQLQuery(sql, 30);

            var notes = new List<Note>();

            if (reader.HasRows)
            {
                var sReader = new SpecialistsReader();
                while (reader.Read())
                {
                    string uName = reader["Author"] as string;
                    var spec = sReader.GetSpecialist(uName);
                    string n = reader["Note"] as string;
                    DateTime date = reader.GetDateTime(4);
                    string step = reader["StepTypeID"] as string;

                    var note = new Note(spec, n, date, step);
                    notes.Add(note);
                }
                return notes;
            }
            else return null;
        }

        private string NotesSql(Roll roll)
        {
            string sql = @"SELECT RN.RollName
		                        ,RN.StepTypeID
		                        ,RN.Author
		                        ,RN.Note
		                        ,RN.UpdateTime
                        FROM RollNote RN (NOLOCK)
                        WHERE RN.RollName = '" + roll.RollName + @"'
	                        AND RN.NOTE != ''
                        ORDER BY RN.UpdateTime DESC";

            return sql;
        }
    }
}
