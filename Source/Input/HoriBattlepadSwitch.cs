using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;
using System;

namespace ControllerSupressor.Source.Input
{
    class HoriBattlepadSwitch : DirectInputController
    {
        internal HoriBattlepadSwitch(DirectInput directInput, DeviceInstance deviceInstance) : base(directInput, deviceInstance)
        { }

        protected override bool CheckForActivation()
        {
            directInputController.Poll();
            JoystickState state = directInputController.GetCurrentState();
            return state.Buttons[6] && state.Buttons[7];
        }

        protected override Xbox360Report MapToXInputDevice(JoystickState directInputState)
        {
            Xbox360Report report = new Xbox360Report();
            report.SetButtonState(Xbox360Buttons.A, directInputState.Buttons[0]);
            report.SetButtonState(Xbox360Buttons.B, directInputState.Buttons[1]);
            report.SetButtonState(Xbox360Buttons.X, directInputState.Buttons[2]);
            report.SetButtonState(Xbox360Buttons.Y, directInputState.Buttons[3]);

            report.SetButtonState(Xbox360Buttons.Back, false);
            report.SetButtonState(Xbox360Buttons.Guide, false);
            report.SetButtonState(Xbox360Buttons.Start, false);

            report.SetButtonState(Xbox360Buttons.Up, false);
            report.SetButtonState(Xbox360Buttons.Right, false);
            report.SetButtonState(Xbox360Buttons.Down, false);
            report.SetButtonState(Xbox360Buttons.Left, false);

            report.SetButtonState(Xbox360Buttons.LeftShoulder, false);
            report.SetButtonState(Xbox360Buttons.RightShoulder, false);

            report.SetButtonState(Xbox360Buttons.LeftThumb, false);
            report.SetButtonState(Xbox360Buttons.RightThumb, false);

            report.LeftTrigger = 0;
            report.RightTrigger = 0;
            //Console.WriteLine("###"); 65408
            //Console.WriteLine(directInputState.Z);

            report.LeftThumbX = ClampToShort(directInputState.X - 32768);
            report.LeftThumbY = ClampToShort(-1 * (directInputState.Y - 32768));
            report.RightThumbX = ClampToShort(directInputState.RotationX - 32768);
            report.RightThumbY = ClampToShort(-1 * (directInputState.RotationY - 32768));
            return report;
        }

        private static short ClampToShort(int val)
        {
            if (val.CompareTo(-short.MaxValue) < 0) return -short.MaxValue;
            else if (val.CompareTo(short.MaxValue) > 0) return short.MaxValue;
            else return Convert.ToInt16(val);
        }
    }
}
