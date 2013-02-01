using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class DriveSpace
    {
        public double UsedSpace { get; set; }
        public double TotalSpace { get; set; }

        public DriveSpace(double usedSpace, double totalSpace)
        {
            this.UsedSpace = usedSpace;
            this.TotalSpace = totalSpace;
        }
    }
}
