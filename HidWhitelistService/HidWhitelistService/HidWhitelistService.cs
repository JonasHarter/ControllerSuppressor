using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Management;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace InputMapperCerberusWhitelister
{
    public partial class HidWhitelistService : ServiceBase
    {
        private static string HidWhitelistRegistryKeyBase => @"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters\Whitelist";
        private static string[] TrustedApplications = { "AutoHotkey.exe", "AutoHotkeyA32.exe", "AutoHotkeyU32.exe", "AutoHotkeyU64.exe"};
        
        private ManagementEventWatcher startWatch;
        private ManagementEventWatcher stopWatch;

        static void Main(string[] args)
        {
#if (!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new HidWhitelistService() };
            ServiceBase.Run(ServicesToRun);
# else
            HidWhitelistService hid = new HidWhitelistService();
            hid.OnStart(new String[0]);
#endif           
        }

        public HidWhitelistService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // lowercase process names
            for (int i = 0; i < TrustedApplications.Length; i++)
            {
                TrustedApplications[i] = TrustedApplications[i].ToLower();
            }
            // purge registry
            Registry.LocalMachine.DeleteSubKeyTree($"{HidWhitelistRegistryKeyBase}");

            startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            startWatch.Start();

            stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
            stopWatch.Start();

            while(true)
            {
                Console.WriteLine("In Loop");
                Thread.Sleep(1000);
            }
        }

        protected override void OnStop()
        {
            startWatch.Stop();
            stopWatch.Stop();
        }

        static void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString().ToLower();
            int processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value.ToString() ?? "0");

            if (TrustedApplications.Contains(processName))
            {
                Registry.LocalMachine.CreateSubKey($"{HidWhitelistRegistryKeyBase}\\{processID}");
            }
        }

        private void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString().ToLower();
            int processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value.ToString() ?? "0");

            if (TrustedApplications.Contains(processName) || Registry.LocalMachine.OpenSubKey($"{HidWhitelistRegistryKeyBase}\\{processID}") != null)
            {
                Registry.LocalMachine.DeleteSubKey($"{HidWhitelistRegistryKeyBase}\\{processID}");
            }
        }
    }
}