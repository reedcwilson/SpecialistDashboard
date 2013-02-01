using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Specialist_Dashboard
{
    class DriveSpaceFinder
    {
        public DriveSpace DriveSpace { get; set; }
        /// <summary>
        /// maps required drives
        /// </summary>
        /// <param name="path"> string path</param>
        /// <returns>long usedSpace</returns>
        public DriveSpace NetDriveSpaceFinder(string path)
        {
            bool mappedDriveAvailable = true;
            int i = 70;
            char driveLetter = (char)i;
            while (!DriveSettings.MapNetworkDrive(Convert.ToString(driveLetter), path))
            {
                if (i == 91)
                {
                    mappedDriveAvailable = false;
                    break;
                }

                i++;
                driveLetter = (char)i;
            }
            //System.Diagnostics.Process.Start("net.exe", @"use " + driveName + ": " + path);
            if (!mappedDriveAvailable)
            {
                return this.DriveSpace = new DriveSpace(0.0, 0.0);
            }
            else
            {
                double usedSpace = 0.0;
                double totalSpace = 0.0;

                System.Threading.Thread.Sleep(250);

                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    if (drive.Name == driveLetter + ":\\")
                    {
                        if (drive.IsReady)
                        {
                            usedSpace = (drive.TotalSize - drive.AvailableFreeSpace) / 1024.0 / 1024.0 / 1024.0 / 1024.0;
                            totalSpace = drive.TotalSize / 1024.0 / 1024.0 / 1024.0 / 1024.0;
                            this.DriveSpace = new DriveSpace(usedSpace, totalSpace);

                            break;
                        }
                    }
                }

                //System.Diagnostics.Process.Start("net.exe", @"use J: /delete");
                DriveSettings.DisconnectNetworkDrive(Convert.ToString(driveLetter), true);

                return this.DriveSpace;
            }
        }
    }
}
