using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System.Speech.Synthesis;
using System;
using SharpDX.DirectInput;

namespace ControllerSupressor.Source.Input
{
    abstract class Controller
    {
        private static ViGEmClient client = new ViGEmClient();

        private bool activated = false;
        private Xbox360Controller emnulatedController = new Xbox360Controller(client);
        internal Guid guid { get; private set; }

        protected Controller(Guid guid)
        {
            this.guid = guid;
        }

        internal static DirectInputController Create(DeviceInstance deviceInstance)
        {
            string name = deviceInstance.InstanceName;
            if (name.Contains("WUSBmote"))
                return new HoriBattlepad(deviceInstance);
            //if (name.Contains("Xbox One For Windows"))
            //    return new XBoxOne(directInput, deviceInstance);
            return new UnknownDirectInputController(deviceInstance);
        }

        internal void Update()
        {
            if (activated)
            {
                UpdateXInputDevice();
            }
            else
            {
                try
                {
                    emnulatedController.Connect();
                    activated = true;
                }
                catch (Exception ex)
                { }
            }
        }

        private void UpdateXInputDevice()
        {
            Xbox360Report report = MapToXInputDevice();
            emnulatedController.SendReport(report);
        }

        protected abstract Xbox360Report MapToXInputDevice();
    }
}
