using System.Collections.Generic;
using SharpDX.DirectInput;
using System.Timers;
using NLog;

namespace ControllerSupressor.Source.Input
{
    // https://codereview.stackexchange.com/questions/68711/joystick-helper-class
    //https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectInput/JoystickApp/Program.cs
    class DirectInputControllerManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static DirectInputControllerManager instance;
        private static Timer timer;
        private readonly DirectInput directInput;
        //TODO threadsafe list
        private List<DirectInputController> controllers = new List<DirectInputController>();


        private DirectInputControllerManager()
        {
            directInput = new DirectInput();
            timer = new Timer();
            timer.Elapsed += DetectDevices;
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        internal static DirectInputControllerManager GetInstance()
        {
            if (instance == null)
                instance = new DirectInputControllerManager();
            return instance;
        }

        public void DetectDevices(object source, ElapsedEventArgs ee)
        {
            foreach (DeviceInstance deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly))
            {
                if (deviceInstance.InstanceName.ToLower().Contains("xbox") || controllers.Find(item => item.guid == deviceInstance.InstanceGuid) != null)
                    continue;
                var test = DirectInputController.create(directInput, deviceInstance);
                test.checkForActivation();
                controllers.Add(test);
                logger.Debug($"Detected controller {deviceInstance.InstanceName} with GUID {deviceInstance.InstanceGuid}");
            }
        }

    }
}
