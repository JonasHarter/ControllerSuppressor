using System;
using System.IO;
using System.Threading;
using ControllerMapper.Source;
using NLog;
using NLog.Targets;
using NLog.Config;
using System.Security.Principal;
using ControllerSupressor.Source.Input;
using System.Diagnostics;
using SharpDX.XInput;
using SharpDX.RawInput;
using ControllerMapper.Source.Mouse;

namespace ControllerMapper
{
    class Program
    {
        internal static string configurationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ControllerSupressor");
        internal static string configurationFilePath = Path.Combine(Program.configurationFolder, "configuration.xml");

        static void Main(string[] args)
        {
            Directory.CreateDirectory(configurationFolder);
            ConfigLogger();
            if (!IsAdministrator())
                throw new Exception("Not launched as admin");
            foreach (String hid in Configuration.GetInstance().HidList)
            {
                DeviceBlacklister.GetInstance().BlacklistDevice(hid);
            }
            ProcessWhitelister processWhitelister = ProcessWhitelister.GetInstance();
            processWhitelister.PurgeWhitelist();
            processWhitelister.AddToWhitelist(Process.GetCurrentProcess().Id);
            ControllerManager directInputManager = ControllerManager.GetInstance();
            //MouseControl mouseControl = MouseControl.GetInstance();
            // Suspend main thread
            Thread.Sleep(Timeout.Infinite);
        }

        static void MainForJoystick()
        {
            return;
            Console.WriteLine("Start XGamepadApp");
            // Initialize XInput
            var controllers = new[] { new SharpDX.XInput.Controller(UserIndex.One), new SharpDX.XInput.Controller(UserIndex.Two), new SharpDX.XInput.Controller(UserIndex.Three), new SharpDX.XInput.Controller(UserIndex.Four) };

            // Get 1st controller available
            SharpDX.XInput.Controller controller = null;
            foreach (var selectControler in controllers)
            {
                if (selectControler.IsConnected)
                {
                    controller = selectControler;
                    break;
                }
            }

            if (controller == null)
            {
                Console.WriteLine("No XInput controller installed");
            }
            else
            {

                Console.WriteLine("Found a XInput controller available");
                Console.WriteLine("Press buttons on the controller to display events or escape key to exit... ");

                // Poll events from joystick
                var previousState = controller.GetState();
                while (controller.IsConnected)
                {
                    var state = controller.GetState();
                    if (previousState.PacketNumber != state.PacketNumber)
                        Console.WriteLine(state.Gamepad);
                    Thread.Sleep(10);
                    previousState = state;
                }
            }
            Console.WriteLine("End XGamepadApp");
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
