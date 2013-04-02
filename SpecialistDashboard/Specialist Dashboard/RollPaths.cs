using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace Specialist_Dashboard
{
    public class RollPaths
    {
        public DataContext Data_Context { get; set; }
        public Roll MyRoll { get; set; }
        public XElement RootElement { get; set; }
        public string JobSpec { get; set; }

        public RollPaths(Roll roll)
        {
            Data_Context = new DataContext("epdb01", "jwf_live");
            MyRoll = roll;
            
            if (File.Exists(GetJobSpec() + @"\" + MyRoll.ProjectId + @"\" + MyRoll.RollName + ".xml"))
            {
                JobSpec = GetJobSpec() + @"\" + MyRoll.ProjectId + @"\" + MyRoll.RollName + ".xml";
                RootElement = XElement.Load(JobSpec);
            }
            else if (File.Exists(GetJobSpec() + @"\" + MyRoll.RollName.Substring(0, 5) + @"\" + MyRoll.RollName + ".xml"))
            {
                JobSpec = GetJobSpec() + @"\" + MyRoll.RollName.Substring(0, 5) + @"\" + MyRoll.RollName + ".xml";
                RootElement = XElement.Load(JobSpec);
            }
        }

        public RollPaths()
        {
            Data_Context = new DataContext("epdb01", "jwf_live");
        }

        private string _rootPath;
        public string GetRootPath()
        {
            if (_rootPath == null)
                _rootPath = GetPath("RootPath", MyRoll.ProjectId, MyRoll.RollName);
            return _rootPath;
        }

        private string _rootImagesPath;
        public string GetRootImagesPath()
        {
            if (_rootImagesPath == null)
                _rootImagesPath = GetPath("RootImagesPath", MyRoll.ProjectId, MyRoll.RollName);
            return _rootImagesPath;
        }

        private string _imageProcessPath;
        public string GetImgProcessPath()
        {
            if (_imageProcessPath == null)
                _imageProcessPath = GetPath("ImgProcessPath", MyRoll.ProjectId, MyRoll.RollName);
            return _imageProcessPath;
        }

        private string _idxPath;
        public string GetIdxPath()
        {
            if (_idxPath == null)
                _idxPath = GetPath("IdxPath", MyRoll.ProjectId, MyRoll.RollName);
            return _idxPath;
        }

        private string _rollsPath;
        public string GetSiteSuitePath()
        {
            if (_rollsPath == null)
                _rollsPath = GetPath("RollsPath", MyRoll.ProjectId, MyRoll.RollName);
            return _rollsPath;
        }

        private string GetPath(string pathType, string project, string rollName)
        {
            //opens the jobspec and finds the specified node
            if (pathType == "RootPath")
            {
                if (File.Exists(JobSpec))
                {
                    return RootElement.Element("Roll").Attribute("RootFolder").Value;
                }
                return null;
            }
            else if (pathType == "RootImagesPath")
            {
                if (File.Exists(JobSpec))
                {
                    if (RootElement.Attribute("ImagingBatchClass").Value == "Mekel")
                    {
                        return GetPath("RootPath", project, rollName) + @"\frames";
                    }
                    else
                    {
                        return GetPath("RootPath", project, rollName);
                    }
                }
                return null;
            }
            else if (pathType == "ImgProcessPath")
            {
                if (File.Exists(JobSpec))
                {
                    if (RootElement.Descendants("Roll")
                        .Where(roll => roll.Elements("ImageQA").Any()).Any())
                    {
                        var imgProcPath = RootElement.Element("Roll").Element("ImageQA").Attribute("InputFolder").Value;
                        if (imgProcPath.Substring(0, 14).ToLower() == "%dynamicshare%")
                            imgProcPath = @"\\dpfs02\imaging\" + project + @"\" + rollName;

                        return imgProcPath;
                    }
                }
                return null;
            }
            else if (pathType == "IdxPath")
            {
                string idxPath;
                if (File.Exists(JobSpec))
                {
                    if (RootElement.Descendants("Roll")
                        .Where(roll => roll.Elements("ImageConversion").Any()).Any())
                    {
                        var compressionOutputElements = RootElement.Element("Roll").Element("ImageConversion").Element("Compression").Elements("CompressionOutput");
                        foreach (var element in compressionOutputElements)
                        {
                            if (element.Attribute("TypeCodeID").Value.ToLower() == "j2k_1" || element.Attribute("TypeCodeID").Value.ToLower() == "tif_bitonal")
                            {
                                idxPath = element.Attribute("OutputFolder").Value;
                                return idxPath;
                            }
                        }
                    }
                }
                return null;
            }
            else if (pathType == "RollsPath")
            {
                string rollsPath;
                if (File.Exists(JobSpec))
                {
                    if (RootElement.Descendants("Roll")
                        .Where(roll => roll.Elements("ImageConversion").Any()).Any())
                    {
                        var compressionOutputElements = RootElement.Element("Roll").Element("ImageConversion").Element("Compression").Elements("CompressionOutput");
                        foreach (var element in compressionOutputElements)
                        {
                            if (element.Attribute("TypeCodeID").Value.ToLower() == "j2k_hq")
                            {
                                rollsPath = element.Attribute("OutputFolder").Value;
                                return rollsPath;
                            }
                        }
                    }
                }
                return null;
            }

            return null;
        }

        private string _jobSpec;
        public string GetJobSpec()
        {
            if (_jobSpec == null)
            {
                //query to find the jobspec location
                string sql = @"SELECT SettingValue
                            FROM WorkFlowSettings WFS (NOLOCK)
                            WHERE WorkFlowSettingsName = 'JobSpecFolderBase'";

                _jobSpec = Data_Context.ExecuteScalar(sql, "epdb01", "JWF_Live");

                Data_Context.CloseConnection();
            }
            return _jobSpec;
        }
    }
}
