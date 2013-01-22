using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace Specialist_Dashboard
{
    public class AllJobSpecUpdates : JobSpecUpdates
    {
        public string ProjectId { get; set; }
        public string FileName { get; set; }
        public AllJobSpecUpdates()
        {
        }

        public void Initialize(string projectId, string fileName)
        {
            ProjectId = projectId;
            FileName = fileName;
            MyRollPaths = new RollPaths();
            JobSpecPath = MyRollPaths.GetJobSpec() + @"\" + ProjectId + @"\" + FileName;
            if (File.Exists(JobSpecPath)) RootElement = XElement.Load(JobSpecPath);

            AutoCropElement = RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("AutoCrop");
            DeskewElement = RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("Deskew");

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
