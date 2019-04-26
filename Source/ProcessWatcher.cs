using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ControllerMapper.Source
{
    class ProcessWatcher
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ManagementEventWatcher startProcessWatcher;
        private readonly ManagementEventWatcher stopProcessWatcher;
        private readonly Configuration configuration;
        private readonly ProcessWhitelister processWhitelister;

        public ProcessWatcher(Configuration configuration, ProcessWhitelister processWhitelister)
        {
            this.configuration = configuration;
            this.processWhitelister = processWhitelister;

            CheckAllCurrentProcesses();

            startProcessWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startProcessWatcher.EventArrived += new EventArrivedEventHandler(StartWatch_EventArrived);
            startProcessWatcher.Start();

            stopProcessWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopProcessWatcher.EventArrived += new EventArrivedEventHandler(StopWatch_EventArrived);
            stopProcessWatcher.Start();
        }

        private void CheckAllCurrentProcesses()
        {
            Process[] processes = Process.GetProcesses();
            foreach(Process process in processes)
            {
                // Probably a not optimal solution
                AddProcess(process.ProcessName + ".exe", process.Id);
            }
        }

        private void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString().ToLower();
            int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value.ToString() ?? "0");
            AddProcess(processName, processId);
        }

        private void AddProcess(string name, int id)
        {
            if (!configuration.Contains(name))
                return;
            processWhitelister.AddToWhitelist(id);
            logger.Debug($"Process started: {name} with pid {id}");
        }

        private void StopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString().ToLower();
            int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value.ToString() ?? "0");
            RemoveProcess(processName, processId);
        }

        private void RemoveProcess(string name, int id)
        {
            if (!configuration.Contains(name))
                return;
            processWhitelister.RemoveFromWhitelist(id);
            logger.Debug($"Process stopped: {name} with pid {id}");
        }
    }
}
