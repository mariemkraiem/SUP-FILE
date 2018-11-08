using SupFile2.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace SupFile2.Utilities
{
    public class StorageManager
    {
        public static StorageManager Instance;
        
        private Dictionary<string, string> drivesShares = new Dictionary<string, string>();

        public StorageManager()
        {
            if (Instance != null)
            {
                System.Diagnostics.Debug.WriteLine("Tried to create a second storage manager");
            }
            else
            {
                Instance = this;
                new Database();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Storage.config"));
                XmlNode root = xmlDoc.ChildNodes[1];
                XmlNode shares = root.FirstChild;
                foreach (XmlNode driveXml in shares.ChildNodes)
                {
                    string drive = "";
                    string share = "";
                    XmlAttributeCollection attrs = driveXml.Attributes;
                    foreach (XmlAttribute attr in attrs)
                    {
                        if (attr.Name == "drive") drive = attr.Value;
                        if (attr.Name == "share") share = attr.Value;
                    }

                    if (drive != "" && share != "") drivesShares.Add(drive, share);
                }
            }
        }

        public string GetLeastUsedStorage()
        {
            List<DriveInfo> drives = new List<DriveInfo>(DriveInfo.GetDrives());
            Regex rx = new Regex("^[a-zA-Z]:");

            string storageName = "";
            long storageSize = 0;

            foreach (KeyValuePair<string, string> driveShare in drivesShares)
            {
                if (rx.IsMatch(driveShare.Value)) // It's a drive
                {
                    DriveInfo drive = drives.Find(d => d.Name == driveShare.Value && d.DriveType != DriveType.CDRom);
                    if (drive == null) continue;

                    while (!drive.IsReady) ;

                    if (drive.AvailableFreeSpace > storageSize)
                    {
                        storageName = drive.Name;
                        storageSize = drive.AvailableFreeSpace;
                    }
                }
                else // It's a share
                {
                    using (PowerShell ps = PowerShell.Create())
                    {
                        ps.AddScript("$disk = Get-WmiObject Win32_LogicalDisk -ComputerName $remote -Filter \"DeviceID = '$drive'\" | Select FreeSpace");
                        ps.AddParameter("remote", driveShare.Value);
                        ps.AddParameter("drive", driveShare.Key);
                        //ps.AddScript("Get-WMIObject Win32_Logicaldisk -filter \"deviceid='C:'\" | Select FreeSpace");

                        Collection<PSObject> psOut = ps.Invoke();
                        try
                        {
                            long free = (long) psOut[0].Members["FreeSpace"].Value;
                            if (free > storageSize)
                            {
                                storageName = driveShare.Value;
                                storageSize = free;
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.Log($"Could not fetch free space for storage {driveShare.Value} (Disk {driveShare.Key})");
                            continue;
                        }
                    }
                }
            }
            return storageName;
        }

        public float KilobyteToGygabyte(long kilobyte)
        {
            return kilobyte / 1024f / 1024f;
        }

        public float KiloByteToMegaByte(long kilobyte)
        {
            return kilobyte / 1024f;
        }

        public long GygabyteToKilobyte(float gigabyte)
        {
            return (long) gigabyte * 1024 * 1024;
        }

        public long MegabyteToKilobyte(float gigabyte)
        {
            return (long) gigabyte * 1024;
        }

        public System.IO.DriveInfo GetLeastUsedDrive()
        {
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            if (drives.Length == 0)
                return null;

            System.IO.DriveInfo bestDrive = null;
            foreach(System.IO.DriveInfo drive in drives)
            {
                if (drive.DriveType == System.IO.DriveType.CDRom) continue;
                while (!drive.IsReady);

                if (bestDrive == null || drive.TotalFreeSpace > bestDrive.TotalFreeSpace)
                    bestDrive = drive;
            }
            return bestDrive;
        }

        public string GetLeastUsedDriveName()
        {
            DriveInfo result = GetLeastUsedDrive();
            if (result != null)
                return result.Name;
            else
                return null;
        }
    }
}