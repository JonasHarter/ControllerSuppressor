using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;
using System;

namespace ControllerSupressor.Source.Input
{
    class UnknownDirectInputController : DirectInputController
    {
        internal UnknownDirectInputController(DeviceInstance deviceInstance) : base(deviceInstance)
        { }

        protected override Xbox360Report MapToXInputDevice(JoystickState directInputState)
        {
            throw new NotImplementedException();
        }
    }
}
