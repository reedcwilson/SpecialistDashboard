using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class SortOnProject : IComparer<Roll>
    {
        public int Compare(Roll a, Roll b)
        {
            return string.Compare(a.ProjectId, b.ProjectId, true);
        }
    }
}
