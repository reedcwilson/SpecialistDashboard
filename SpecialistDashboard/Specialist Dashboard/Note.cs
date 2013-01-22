using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class Note : IComparable<Note>
    {
        public Specialist Spec { get; set; }
        public string NoteMessage { get; set; }
        public DateTime Date { get; set; }
        public string Step { get; set; }

        public Note(Specialist spec, string note, DateTime date, string step)
        {
            Spec = spec;
            NoteMessage = note;
            Date = date;
            Step = step;
        }

        public int CompareTo(Note b)
        {
            // DateTime sort
            return this.Date.CompareTo(b.Date);
        }
    }
}
