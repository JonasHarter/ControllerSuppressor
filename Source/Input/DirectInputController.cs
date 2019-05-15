using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace ControllerSupressor.Source.Input
{
    abstract class DirectInputController
    {
        private static ViGEmClient client = new ViGEmClient();
        private Xbox360Controller xInputController = new Xbox360Controller(client);
        private static SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        protected Joystick directInputController { get; set; }

        internal Guid guid { get; private set; }

        protected DirectInputController(DirectInput directInput, DeviceInstance deviceInstance)
        {
            directInputController = new Joystick(directInput, deviceInstance.InstanceGuid);
            directInputController.Properties.BufferSize = 128;
            directInputController.Acquire();
            this.guid = deviceInstance.InstanceGuid;
        }

        internal static DirectInputController Create(DirectInput directInput, DeviceInstance deviceInstance)
        {
            string name = deviceInstance.InstanceName;
            if (name.Contains("HORIPAD S"))
                return new HoriBattlepadSwitch(directInput, deviceInstance);
            return new UnknownDirectInputController(directInput, deviceInstance);
        }

        internal void Update()
        {
            if (activated)
            {
                UpdateXInputDevice();
            }
            else if(CheckForActivation())
            {
                try
                {
                    Activate();
                }
                catch (Exception ex)
                { }
            }
        }

        private bool activated = false;

        protected abstract bool CheckForActivation();

        private void Activate()
        {
            xInputController.Connect();
            activated = true;
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;  // 0...100
            synthesizer.Rate = -2;     // -10...10
            synthesizer.SpeakAsync("Player connected");
        }

        private void UpdateXInputDevice()
        {
            directInputController.Poll();
            Xbox360Report report = MapToXInputDevice(directInputController.GetCurrentState());
            xInputController.SendReport(report);
        }

        protected abstract Xbox360Report MapToXInputDevice(JoystickState directInputState);

    }
}
