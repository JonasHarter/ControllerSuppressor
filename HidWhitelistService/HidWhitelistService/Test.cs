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

namespace ASDF
{
    public class ASDF
    {
        private static string HidWhitelistRegistryKeyBase => @"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters\Whitelist";
        private static string[] TrustedApplications = { "autohotkey.exe", "autohotkeya32.exe", "autohotkeyu32.exe", "autohotkeyu64.exe", "notepad++.exe" };

        public static void Main()
        {
            ManagementEventWatcher startWatch = null;
            ManagementEventWatcher stopWatch = null;
            try
            {
                //Eventwatcher initilisieren
                startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                startWatch.EventArrived += new EventArrivedEventHandler(processStart);
                startWatch.Start();
                stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                stopWatch.EventArrived += new EventArrivedEventHandler(processStop);
                stopWatch.Start();
                //while (!Console.KeyAvailable) Thread.Sleep(50);//Auf Benutzereingabe warten, um das Programm zu beenden.
                while (true) Thread.Sleep(50);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Leider ist ein Fehler aufgetreten.");
                Console.WriteLine("Fehlermeldung: {0}", ex.Message);
            }
            finally
            {
                //Eventwatcher wieder beenden
                if (startWatch != null)
                    startWatch.Stop();
                if (stopWatch != null)
                    stopWatch.Stop();
            }
        }

        static void processStart(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString().ToLower();
            int processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value.ToString() ?? "0");

            if (TrustedApplications.Contains(processName))
            {
                try
                {
                    Registry.LocalMachine.CreateSubKey($"{HidWhitelistRegistryKeyBase}\\{processID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error adding " + processName + ", " + processID);
                }
            }
        }

        static void processStop(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString().ToLower();
            int processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value.ToString() ?? "0");

            if (TrustedApplications.Contains(processName) || Registry.LocalMachine.OpenSubKey($"{HidWhitelistRegistryKeyBase}\\{processID}") != null)
            {
                try
                {
                    Registry.LocalMachine.DeleteSubKey($"{HidWhitelistRegistryKeyBase}\\{processID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error removing " + processName + ", " + processID);
                }
            }
        }
    }
}