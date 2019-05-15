using System.Collections.Generic;
using SharpDX.DirectInput;
using System.Timers;
using NLog;
using System.Speech.Synthesis;

namespace ControllerSupressor.Source.Input
{
    // https://codereview.stackexchange.com/questions/68711/joystick-helper-class
    // https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectInput/JoystickApp/Program.cs
    // https://forums.vigem.org/topic/6/use-vigem-to-create-xbox-360-controller-in-c/20
    class DirectInputControllerManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static DirectInputControllerManager instance;
        private static Timer detectTimer;
        private static Timer updateTimer;
        private readonly DirectInput directInput;
        //TODO threadsafe list
        private List<DirectInputController> controllers = new List<DirectInputController>();


        private DirectInputControllerManager()
        {
            directInput = new DirectInput();
            detectTimer = new Timer();
            detectTimer.Elapsed += DetectDevices;
            detectTimer.Interval = 5000;
            detectTimer.Enabled = true;
            updateTimer = new Timer();
            updateTimer.Elapsed += UpdateDevices;
            updateTimer.Interval = 15; // ~60FPS
            updateTimer.Enabled = true;
        }

        internal static DirectInputControllerManager GetInstance()
        {
            if (instance == null)
                instance = new DirectInputControllerManager();
            return instance;
        }

        private void UpdateDevices(object source, ElapsedEventArgs ee)
        {
            if (controllers.Count == 0)
                return;
            foreach(DirectInputController controller in controllers)
            {
                controller.Update();
            }
        }

        private void DetectDevices(object source, ElapsedEventArgs ee)
        {
            foreach (DeviceInstance deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly))
            {
                if (deviceInstance.InstanceName.ToLower().Contains("xbox") || controllers.Find(item => item.guid == deviceInstance.InstanceGuid) != null)
                    continue;
                var test = DirectInputController.Create(directInput, deviceInstance);
                controllers.Add(test);
                logger.Debug($"Detected controller {deviceInstance.InstanceName} with GUID {deviceInstance.InstanceGuid}");
            }
        }

    }
}
