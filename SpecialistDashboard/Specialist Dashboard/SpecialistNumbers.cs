using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class SpecialistNumbers : IComparable<SpecialistNumbers>
    {
        public string RollName { get; set; }
        public int RollsNum { get; set; }
        public int ImagesNum { get; set; }
        public double RollsPerHour { get; set; }
        public double ImagesPerHour { get; set; }

        public SpecialistNumbers(string rollName, int rollsNum, int imagesNum, double rollsPerHour, double imagesPerHour)
        {
            RollName = rollName;
            RollsNum = rollsNum;
            ImagesNum = imagesNum;
            RollsPerHour = rollsPerHour;
            ImagesPerHour = imagesPerHour;
        }

        public int CompareTo(SpecialistNumbers b)
        {
            // Alphabetic sort name[A to Z]
            return this.RollName.CompareTo(b.RollName);
        }
    }
}
