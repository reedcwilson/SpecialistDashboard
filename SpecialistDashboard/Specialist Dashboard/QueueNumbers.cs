using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specialist_Dashboard
{
    class QueueNumbers
    {
        public int Scan { get; set; }
        public int Framing { get; set; }
        public int MekelExtracting { get; set; }
        public int BatchValidating { get; set; }
        public int ImageProcessing { get; set; }
        public int ImageQA { get; set; }
        public int ImageQE { get; set; }
        public int GridlinesQA { get; set; }

        public double ScanningUsed { get; set; }
        public double ScanningTotal { get; set; }
        public double ImagingUsed { get; set; }
        public double ImagingTotal { get; set; }
        public double StagingUsed { get; set; }
        public double StagingTotal { get; set; }

        public QueueNumbers(int scan, int framing, int mekelExtracting, int batchValidating, int imageProcessing, int imageQA, int imageQE, int gridlinesQA)
        {
            this.Scan = scan;
            this.Framing = framing;
            this.MekelExtracting = mekelExtracting;
            this.BatchValidating = batchValidating;
            this.ImageProcessing = imageProcessing;
            this.ImageQA = imageQA;
            this.ImageQE = imageQE;
            this.GridlinesQA = gridlinesQA;
        }
    }
}
