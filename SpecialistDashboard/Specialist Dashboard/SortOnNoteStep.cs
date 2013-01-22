using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class SortOnNoteStep : IComparer<Note>
    {
        public int Compare(Note a, Note b)
        {
            return string.Compare(a.Step, b.Step, true);
        }
    }
}
