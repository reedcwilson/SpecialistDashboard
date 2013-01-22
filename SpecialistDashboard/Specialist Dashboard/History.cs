using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class History : IComparable<History>
    {
        public Specialist Spec { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Step { get; set; }

        public History(Specialist spec, string message, DateTime date, string step)
        {
            Spec = spec;
            Message = message;
            Date = date;
            Step = step;
        }

        public int CompareTo(History b)
        {
            // DateTime sort
            return this.Date.CompareTo(b.Date);
        }
    }
}
