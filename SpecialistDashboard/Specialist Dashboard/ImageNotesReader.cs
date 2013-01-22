using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class ImageNotesReader
    {
        public DataContext Data_Context { get; set; }

        public ImageNotesReader()
        {
            Data_Context = new DataContext("epdb02", "DexterImaging");
        }

        public List<ImageNote> GetNotes(Roll roll)
        {
            string sql = ImageNotesSql(roll);
            var reader = Data_Context.RunSelectSQLQuery(sql, 30);

            var imgNotes = new List<ImageNote>();

            if (reader.HasRows)
            {
                var sReader = new SpecialistsReader();
                while (reader.Read())
                {
                    string uName = reader["Author"] as string;
                    var spec = sReader.GetSpecialist(uName);
                    string n = reader["Note"] as string;
                    DateTime date = reader.GetDateTime(3);
                    int imgNum = reader.GetInt32(0);

                    var imgNote = new ImageNote(spec, n, date, imgNum);
                    imgNotes.Add(imgNote);
                }
                Data_Context.CloseConnection();
                return imgNotes;
            }
            else return null;
        }

        private string ImageNotesSql(Roll roll)
        {
            string sql = @"SELECT DISTINCT 
				                            IMN.ImageNumber
				                            ,IMN.Author
				                            ,IMN.Note
				                            ,IMN.UpdateTime
                            FROM ImageNote IMN (NOLOCK)
	                            --LEFT JOIN epdb01.Dexter_DeeDee.dbo.Roll R (NOLOCK) ON IMN.RollID = R.RollID
	                            LEFT JOIN RollNote RN (NOLOCK) ON RN.RollNoteID = IMN.RollNoteID
                            WHERE RN.RollName = '" + roll.RollName + @"'
                            ORDER BY ImageNumber";

            return sql;
        }
    }
}
