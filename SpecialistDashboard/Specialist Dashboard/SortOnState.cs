using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class SortOnState : IComparer<Roll>
    {
        public int Compare(Roll a, Roll b)
        {
            return string.Compare(a.State, b.State, true);
        }
    }
}
