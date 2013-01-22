using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class SortOnImageNoteUser : IComparer<ImageNote>
    {
        public int Compare(ImageNote a, ImageNote b)
        {
            return string.Compare(a.Spec.Username, b.Spec.Username, true);
        }
    }
}
