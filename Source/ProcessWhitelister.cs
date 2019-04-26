using System.Collections.Generic;
using Microsoft.Win32;

namespace ControllerMapper.Source
{
    /// <summary>
    /// Add process id to HidGuardians whitelist, preventing it from filtering hid devices to that process
    /// </summary>
    class ProcessWhitelister
    {
        private static string RegistryKeyBase => @"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters\Whitelist";
        private static ProcessWhitelister instance;

        private ProcessWhitelister() {}

        public static ProcessWhitelister GetInstance()
        {
            if (instance == null)
                instance = new ProcessWhitelister();
            return instance;
        }

        public void AddToWhitelist(List<int> processIds)
        {
            foreach(int processId in processIds)
                AddToWhitelist(processId);
        }

        public void AddToWhitelist(int processId)
        {
            Registry.LocalMachine.CreateSubKey($"{RegistryKeyBase}\\{processId}");
        }

        public void RemoveFromWhitelist(int processId)
        {
            Registry.LocalMachine.DeleteSubKey($"{RegistryKeyBase}\\{processId}");
        }

        public void PurgeWhitelist()
        {
            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(RegistryKeyBase);
            }
            catch { }
        }
    }
}
