using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class SortOnHistoryStep : IComparer<History>
    {
        public int Compare(History a, History b)
        {
            return string.Compare(a.Step, b.Step, true);
        }
    }
}
