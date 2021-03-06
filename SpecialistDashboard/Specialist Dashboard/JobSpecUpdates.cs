﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace Specialist_Dashboard
{
    public class JobSpecUpdates
    {
        public string JobSpecPath { get; set; }
        public XElement RootElement { get; set; }
        public XElement AutoCropElement { get; set; }
        public XElement DeskewElement { get; set; }
        public Roll MyRoll { get; set; }
        public RollPaths MyRollPaths { get; set; }
        public string JobSpecDataReturn { get; set; }
        public bool AutoCropExists { get; set; }

        private bool _autoCrop;
        public bool AutoCrop
        {
            get
            {
                return _autoCrop;
            }
            set
            {
                if (RootElement.Descendants("Roll")
                                .Where(roll => roll.Elements("ImageProcessing").Any()).Any())
                {
                    if (RootElement.Element("Roll").Descendants("ImageProcessing")
                                .Where(roll => roll.Elements("ProcessSequence").Any()).Any())
                    {
                        if (RootElement.Element("Roll").Element("ImageProcessing").Descendants("ProcessSequence")
                                .Where(roll => roll.Elements("AutoCrop").Any()).Any())
                        {
                            RefreshRootElement();
                            _autoCrop = value;
                            RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("AutoCrop").Attribute("xDirection").SetValue(_autoCrop);
                            RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("AutoCrop").Attribute("yDirection").SetValue(_autoCrop);
                            RootElement.Save(JobSpecPath);
                        }
                    }
                }
            }
        }

        private bool _aggressiveFactor;
        public bool AggressiveFactor 
        { 
            get
            {
                return _aggressiveFactor;
            }
            set
            {
                RefreshRootElement();
                _aggressiveFactor = value;
                RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("AutoCrop").Attribute("AggressiveFactor").SetValue(_aggressiveFactor);
                //AutoCropElement.Attribute("AggressiveFactor").SetValue(_aggressiveFactor);
                RootElement.Save(JobSpecPath);
            }
        }

        private int _cropPadding;
        public int CropPadding
        {
            get
            {
                return _cropPadding;
            }
            set
            {
                RefreshRootElement();
                _cropPadding = value;
                RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("AutoCrop").Attribute("CropPadding").SetValue(_cropPadding);
                RootElement.Save(JobSpecPath);
            }
        }

        private int _deskewMaxAngle;
        public int DeskewMaxAngle
        {
            get
            {
                return _deskewMaxAngle;
            }
            set
            {
                RefreshRootElement();
                _deskewMaxAngle = value;
                RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("Deskew").Attribute("MaxAngle").SetValue(_deskewMaxAngle);
                RootElement.Save(JobSpecPath);
            }
        }

        public JobSpecUpdates()
        {
        }

        public JobSpecUpdates(Roll myRoll, RollPaths myRollPaths)
        {
            MyRoll = myRoll;
            MyRollPaths = myRollPaths;
            JobSpecDataReturn = MyRollPaths.GetJobSpec();

            if (File.Exists(JobSpecDataReturn + @"\" + MyRoll.ProjectId + @"\" + MyRoll.RollName + ".xml"))
                JobSpecPath = JobSpecDataReturn + @"\" + MyRoll.ProjectId + @"\" + MyRoll.RollName + ".xml";
            else if (File.Exists(JobSpecDataReturn + @"\" + MyRoll.RollName.Substring(0, 5) + @"\" + MyRoll.RollName + ".xml"))
                JobSpecPath = JobSpecDataReturn + @"\" + MyRoll.RollName.Substring(0, 5) + @"\" + MyRoll.RollName + ".xml";

            RefreshRootElement();

            if (RootElement != null)
            {
                if (RootElement.Descendants("Roll")
                                .Where(roll => roll.Elements("ImageProcessing").Any()).Any())
                {
                    if (RootElement.Element("Roll").Descendants("ImageProcessing")
                            .Where(roll => roll.Elements("ProcessSequence").Any()).Any())
                    {
                        if (RootElement.Element("Roll").Element("ImageProcessing").Descendants("ProcessSequence")
                                .Where(roll => roll.Elements("AutoCrop").Any()).Any())
                        {
                            AutoCropElement = RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("AutoCrop");
                            AutoCropExists = true;
                        }
                        else
                            AutoCropExists = false;

                        DeskewElement = RootElement.Element("Roll").Element("ImageProcessing").Element("ProcessSequence").Element("Deskew");
                    }
                }
                
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

        private void RefreshRootElement()
        {
            if (File.Exists(JobSpecPath)) RootElement = XElement.Load(JobSpecPath);
            else if (File.Exists(JobSpecDataReturn + @"\" + MyRoll.RollName.Substring(0, 5) + @"\" + MyRoll.RollName + ".xml"))
            {
                RootElement = XElement.Load(JobSpecDataReturn + @"\" + MyRoll.RollName.Substring(0, 5) + @"\" + MyRoll.RollName + ".xml");
            }
        }
    }
}
