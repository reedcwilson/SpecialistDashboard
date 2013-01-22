using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class SortOnImagesPerHour : IComparer<RollNumbers>
    {
        public int Compare(RollNumbers a, RollNumbers b)
        {
            return Convert.ToInt32(a.ImagesPerHour - b.ImagesPerHour);
        }
    }
}
