using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Management;
using System.Timers;
using System.Diagnostics;
using NLog;

namespace ControllerMapper.Source
{
    /// <summary>
    /// Adds hardware ids from a list to the HidGuardian blacklist
    /// </summary>
    class DeviceBlacklister
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static DeviceBlacklister instance;
        private static string RegistryKeyBase => @"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters";
        private static string RegistryValue => @"AffectedDevices";
        private static Timer timer;

        private DeviceBlacklister()
        {
            timer = new Timer();
            timer.Elapsed += BlacklistNamedDevices;
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        internal static DeviceBlacklister GetInstance()
        {
            if (instance == null)
                instance = new DeviceBlacklister();
            return instance;
        }

        private void BlacklistNamedDevices(object source, ElapsedEventArgs ee)
        {
            // Grab devices
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity where DeviceID Like ""HID%"""))
                collection = searcher.Get();

            // Blacklist controllers
            foreach (var device in collection)
            {
                var x = device.Properties;
                string devicedesc = device.GetPropertyValue("Description").ToString().ToLower();
                string deviceid = device.GetPropertyValue("DeviceID").ToString();
                deviceid = deviceid.Substring(0, deviceid.LastIndexOf("\\"));
                if (device["Status"].ToString() == "OK")
                {
                    if (!devicedesc.Contains("hid-compliant game controller") || IsByVigem(deviceid))
                        continue;
                    BlacklistDevice(deviceid);
                }
            }
            collection.Dispose();
        }

        // Check registry wether its a device creatd by vigem
        private static bool IsByVigem(string devid)
        {
            // HID\VID_0F0D&PID_00DC&IG_00 to VID_0F0D&PID_00DC
            string hid = devid.Substring(4, 17);
            string path = @"SYSTEM\CurrentControlSet\Enum\USB\" + hid;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
            {
                var subkeys = key.GetSubKeyNames();
                foreach (var subkey in subkeys)
                {
                    using (RegistryKey key2 = Registry.LocalMachine.OpenSubKey(path + "\\" + subkey))
                    {
                        string location = key2.GetValue("LocationInformation").ToString();
                        return location == "Virtual Gamepad Emulation Bus";
                    }
                }
            }
            return false;
        }

        // Replug a device so the user does not have to restart the pc or disconnect/reconnect.
        private static void ReplugDevice(string hid)
        {
            StartProcess(@"Ressources\devcon.exe", "disable" + " " + "\"" + hid + "\"");
            System.Threading.Thread.Sleep(1000);
            StartProcess(@"Ressources\devcon.exe", "enable" + " " + "\"" + hid + "\"");
        }

        private static void BlacklistDevice(string hid)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryKeyBase, true))
            {
                List<string> hidList = new List<string>();
                try
                {
                    hidList.AddRange((string[])key.GetValue(RegistryValue));
                } catch { }
                bool isNew = !hidList.Contains(hid);
                if (isNew)
                    hidList.Add(hid);
                key.SetValue(RegistryValue, hidList.ToArray());
                if (isNew)
                {
                    ReplugDevice(hid);
                    logger.Debug($"Blacklisted {hid}");
                }

            }
        }

        private static string StartProcess(string processname, string args)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = processname,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            return proc.StandardOutput.ReadToEnd();
        }
    }
}
