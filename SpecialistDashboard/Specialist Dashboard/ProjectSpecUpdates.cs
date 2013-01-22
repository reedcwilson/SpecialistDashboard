using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace Specialist_Dashboard
{
    public class ProjectSpecUpdates : AllJobSpecUpdates
    {
        public void Initialize(string fileName)
        {
            FileName = fileName;
            MyRollPaths = new RollPaths();
            JobSpecPath = fileName;
            if (File.Exists(JobSpecPath)) RootElement = XElement.Load(JobSpecPath);

            AutoCropElement = RootElement.Element("ProcessingSteps").Element("ImageProcessing").Element("ProcessSequence").Element("AutoCrop");
            DeskewElement = RootElement.Element("ProcessingSteps").Element("ImageProcessing").Element("ProcessSequence").Element("Deskew");

            if (AutoCropElement != null)
            {
                if (AutoCropElement.Attribute("xDirection").Value == "true" && AutoCropElement.Attribute("yDirection").Value == "true")
                    AutoCrop = true;
                else AutoCrop = false;

                if (AutoCropElement.Attribute("AggressiveFactor").Value == "true")
                    AggressiveFactor = true;
                else AggressiveFactor = false;

                CropPadding = Convert.ToInt32(AutoCropElement.Attribute("CropPadding").Value);
            }

            if (DeskewElement != null)
                DeskewMaxAngle = Convert.ToInt32(DeskewElement.Attribute("MaxAngle").Value);
        }
    }
}
