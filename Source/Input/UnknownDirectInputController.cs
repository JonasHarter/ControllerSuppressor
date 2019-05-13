using SharpDX.DirectInput;
using System;

namespace ControllerSupressor.Source.Input
{
    class UnknownDirectInputController : DirectInputController
    {
        internal UnknownDirectInputController(DirectInput directInput, DeviceInstance deviceInstance) : base(directInput, deviceInstance)
        {

        }

        internal override bool checkForActivation()
        {
            return false;
        }

        internal override void getXInputState()
        {
            throw new NotImplementedException();
        }
    }
}
