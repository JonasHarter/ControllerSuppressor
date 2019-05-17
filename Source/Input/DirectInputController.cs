using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;

namespace ControllerSupressor.Source.Input
{
    abstract class DirectInputController : Controller
    {
        private static DirectInput directInput = new DirectInput();
        protected Joystick directInputController { get; set; }

        protected DirectInputController(DeviceInstance deviceInstance) : base(deviceInstance.InstanceGuid)
        {
            directInputController = new Joystick(directInput, deviceInstance.InstanceGuid);
            directInputController.Properties.BufferSize = 128;
            directInputController.Acquire();
        }

        protected override Xbox360Report MapToXInputDevice()
        {
            directInputController.Poll();
            JoystickState directInputState = directInputController.GetCurrentState();
            return MapToXInputDevice(directInputState);
        }

        protected abstract Xbox360Report MapToXInputDevice(JoystickState directInputState);
    }
}