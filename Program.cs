using System;
using System.IO;
using System.Threading;
using ControllerMapper.Source;
using NLog;
using NLog.Targets;
using NLog.Config;
using System.Security.Principal;

namespace ControllerMapper
{
    class Program
    {
        internal static string configurationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ControllerSupressor");
        internal static string configurationFilePath = Path.Combine(Program.configurationFolder, "configuration.xml");

        static void Main(string[] args)
        {
            //TODO check for  others controller
            Directory.CreateDirectory(configurationFolder);
            ConfigLogger();
            if (!IsAdministrator())
                throw new Exception("Not launched as admin");
            Configuration configuration = Configuration.GetInstance();
            ProcessWhitelister processWhitelister = ProcessWhitelister.GetInstance();
            DeviceBlacklister blacklist = DeviceBlacklister.GetInstance();
            ProcessWatcher watcher = new ProcessWatcher(configuration, processWhitelister);
            // Suspend main thread
            Thread.Sleep(Timeout.Infinite);
        }
        
        private static void ConfigLogger()
        {
            var config = new LoggingConfiguration();

            string layout = @"[${date:format=HH\:mm\:ss}] [${level}] ${message} ${exception}";
            var consoleTarget = new ColoredConsoleTarget("ConsoleTarget")
            {
                Layout = layout
            };
            config.AddTarget(consoleTarget);
            var fileTarget = new FileTarget("FileTarget")
            {
                FileName = Path.Combine(Program.configurationFolder, "log.txt"),
                Layout = layout
            };
            config.AddTarget(fileTarget);

            config.AddRuleForOneLevel(LogLevel.Warn, fileTarget);
            config.AddRuleForAllLevels(consoleTarget);

            LogManager.Configuration = config;
        }

        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
