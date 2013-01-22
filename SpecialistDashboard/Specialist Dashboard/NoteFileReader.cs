using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class NoteFileReader
    {
        public DataContext Data_Context { get; set; }

        public NoteFileReader()
        {
            Data_Context = new DataContext("epdb02", "DexterImaging");
        }

        public List<ImageFileNote> GetNotes(string rollName)
        {
            string sql = ImageNotesSql(rollName);
            var reader = Data_Context.RunSelectSQLQuery(sql, 30);

            var imgNotes = new List<ImageFileNote>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string uName = reader["Author"] as string;
                    string n = reader["Note"] as string;
                    int imgNum = reader.GetInt32(0);

                    var imgNote = new ImageFileNote(uName, n, imgNum);
                    imgNotes.Add(imgNote);
                }
                Data_Context.CloseConnection();
                return imgNotes;
            }
            else return null;
        }

        private string ImageNotesSql(string rollName)
        {
            string sql = @"SELECT DISTINCT 
				                            IMN.ImageNumber
				                            ,IMN.Note
                                            ,IMN.Author
                            FROM ImageNote IMN (NOLOCK)
	                            LEFT JOIN RollNote RN (NOLOCK) ON RN.RollNoteID = IMN.RollNoteID
                            WHERE RN.RollName = '" + rollName + @"'
                            ORDER BY ImageNumber";

            return sql;
        }
    }
}
