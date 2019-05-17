using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;
using System;

namespace ControllerSupressor.Source.Input
{
    class HoriBattlepad : DirectInputController
    {
        internal HoriBattlepad(DeviceInstance deviceInstance) : base(deviceInstance)
        { }

        protected override Xbox360Report MapToXInputDevice(JoystickState directInputState)
        {
            Xbox360Report report = new Xbox360Report();
            report.SetButtonState(Xbox360Buttons.A, directInputState.Buttons[4]);
            report.SetButtonState(Xbox360Buttons.B, directInputState.Buttons[3]);
            report.SetButtonState(Xbox360Buttons.X, directInputState.Buttons[2]);
            report.SetButtonState(Xbox360Buttons.Y, directInputState.Buttons[1]);

            report.SetButtonState(Xbox360Buttons.Back, directInputState.Buttons[13]);
            report.SetButtonState(Xbox360Buttons.Guide, false);
            report.SetButtonState(Xbox360Buttons.Start, directInputState.Buttons[0]);

            report.SetButtonState(Xbox360Buttons.Up, directInputState.Buttons[8]);
            report.SetButtonState(Xbox360Buttons.Down, directInputState.Buttons[9]);
            report.SetButtonState(Xbox360Buttons.Left, directInputState.Buttons[11]);
            report.SetButtonState(Xbox360Buttons.Right, directInputState.Buttons[10]);

            report.SetButtonState(Xbox360Buttons.LeftShoulder, directInputState.Buttons[12]);
            report.SetButtonState(Xbox360Buttons.RightShoulder, directInputState.Buttons[7]);

            report.SetButtonState(Xbox360Buttons.LeftThumb, false);
            report.SetButtonState(Xbox360Buttons.RightThumb, false);

            report.LeftTrigger = (byte)(directInputState.Buttons[5] ? 0xFF : 0x00);
            report.RightTrigger = (byte)(directInputState.Buttons[6] ? 0xFF : 0x00);

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
