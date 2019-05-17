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

        private DeviceBlacklister()
        { }

        public static DeviceBlacklister GetInstance()
        {
            if (instance == null)
                instance = new DeviceBlacklister();
            return instance;
        }

        public void BlacklistDevice(string hid)
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
                    logger.Debug($"Blacklisted {hid}");
            }
        }
    }
}
