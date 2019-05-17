using System.Collections.Generic;
using SharpDX.DirectInput;
using System.Timers;
using NLog;
using System.Speech.Synthesis;
using System;
using ControllerMapper.Source;

namespace ControllerSupressor.Source.Input
{
    // https://codereview.stackexchange.com/questions/68711/joystick-helper-class
    // https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectInput/JoystickApp/Program.cs
    // https://forums.vigem.org/topic/6/use-vigem-to-create-xbox-360-controller-in-c/20
    class ControllerManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static ControllerManager instance;
        private static Timer detectTimer;
        private static Timer updateTimer;
        private readonly DirectInput directInput;
        //TODO threadsafe list
        private List<DirectInputController> controllers = new List<DirectInputController>();


        private ControllerManager()
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

        internal static ControllerManager GetInstance()
        {
            if (instance == null)
                instance = new ControllerManager();
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
                // Check if controller is already detected
                if (controllers.Find(item => item.guid == deviceInstance.InstanceGuid) != null)
                    continue;
                // Check if controller is one of the blacklisted hid
                if (!Configuration.GetInstance().Contains(ConvertProductGuidToHid(deviceInstance.ProductGuid)))
                    continue;
                var newController = DirectInputController.Create(deviceInstance);
                controllers.Add(newController);
                logger.Debug($"Detected controller {deviceInstance.InstanceName} as Type {newController.GetType().Name}");
            }
        }

        private string ConvertProductGuidToHid(Guid guid)
        {
            // 00dc0f0d-0000-0000-0000-504944564944 to HID\VID_045E&PID_02FF&IG_00
            string s = guid.ToString();
            string vid = s.Substring(4, 4).ToUpper();
            string pid = s.Substring(0, 4).ToUpper();
            return $"VID_{vid}&PID_{pid}";
        }

    }
}
