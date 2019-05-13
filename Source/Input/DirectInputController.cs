using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerSupressor.Source.Input
{
    abstract class DirectInputController
    {
        protected Joystick joystick { get; set; }
        private string name { get; set; }
        internal Guid guid { get; private set; }
        internal abstract bool checkForActivation();
        internal abstract void getXInputState();

        protected DirectInputController(DirectInput directInput, DeviceInstance deviceInstance)
        {
            joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
            joystick.Properties.BufferSize = 128;
            joystick.Acquire();
            this.name = deviceInstance.InstanceName;
            this.guid = deviceInstance.InstanceGuid;
        }

        internal static DirectInputController create(DirectInput directInput, DeviceInstance deviceInstance)
        {
            string name = deviceInstance.InstanceName;
            Console.WriteLine(name);
            if (name.Contains("HORIPAD S"))
                return new HoriBattlepadSwitch(directInput, deviceInstance);
            return new UnknownDirectInputController(directInput, deviceInstance);
        }

    }
}
