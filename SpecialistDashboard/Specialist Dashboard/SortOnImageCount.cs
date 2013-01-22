using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class SortOnImageCount : IComparer<RollNumbers>
    {
        public int Compare(RollNumbers a, RollNumbers b)
        {
            return a.ImageCount - b.ImageCount;
        }
    }
}
