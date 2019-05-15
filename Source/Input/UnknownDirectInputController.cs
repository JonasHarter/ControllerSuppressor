using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;
using System;

namespace ControllerSupressor.Source.Input
{
    class UnknownDirectInputController : DirectInputController
    {
        internal UnknownDirectInputController(DirectInput directInput, DeviceInstance deviceInstance) : base(directInput, deviceInstance)
        { }

        protected override bool CheckForActivation()
        {
            return false;
        }

        protected override Xbox360Report MapToXInputDevice(JoystickState directInputState)
        {
            throw new NotImplementedException();
        }
    }
}
