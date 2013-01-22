using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    public class RollNumbers : IComparable<RollNumbers>
    {
        public string RollName { get; set; }
        public int ImageCount { get; set; }
        public double ImagesPerHour { get; set; }
        public int Seconds { get; set; }

        public RollNumbers(string rollName, int imageCount, double imagesPerHour, int seconds)
        {
            RollName = rollName;
            ImageCount = imageCount;
            ImagesPerHour = imagesPerHour;
            Seconds = seconds;
        }

        public int CompareTo(RollNumbers b)
        {
            // Alphabetic sort name[A to Z]
            return this.RollName.CompareTo(b.RollName);
        }
    }
}
