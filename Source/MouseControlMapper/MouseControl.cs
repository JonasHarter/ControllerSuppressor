using System;
using System.Timers;
using SharpDX.XInput;
using NLog;
using System.Speech.Synthesis;
using WindowsInput;
using System.Collections.Generic;
using WindowsInput.Native;
using System.Threading;

namespace ControllerMapper.Source.Mouse
{
    class MouseControlMap
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private static readonly InputSimulator input = new InputSimulator();

        private readonly Controller Controller;
        private bool Active;
        private readonly Dictionary<string, bool> previousKeyStates = new Dictionary<string, bool>();
        private int mouseWheelLimiter = 0;

        static MouseControlMap()
        {
            synthesizer.SelectVoiceByHints(VoiceGender.Female);
        }

        internal MouseControlMap(Controller controller)
        {
            Controller = controller;
            Active = false;
            previousKeyStates.Add("A", false);
            previousKeyStates.Add("B", false);
            previousKeyStates.Add("X", false);
            previousKeyStates.Add("Y", false);
            previousKeyStates.Add("UP", false);
            previousKeyStates.Add("DOWN", false);
            previousKeyStates.Add("RIGHT", false);
            previousKeyStates.Add("LEFT", false);
            previousKeyStates.Add("LSHOULDER", false);
            previousKeyStates.Add("RSHOULDER", false);
            previousKeyStates.Add("LTHUMB", false);
            previousKeyStates.Add("RTHUMB", false);
            previousKeyStates.Add("LTRIGGER", false);
            previousKeyStates.Add("RTRIGGER", false);
            previousKeyStates.Add("BACK", false);
            previousKeyStates.Add("START", false);
        }

        internal void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!Controller.IsConnected)
                    continue;
                Gamepad state = Controller.GetState().Gamepad;
                if (!Active && CheckForActivate(state))
                {
                    Active = true;
                    Logger.Debug($"MouseControl activated for XInput controller #{i}");
                    synthesizer.Speak($"Activated");
                    continue;
                }
                if (Active && CheckForDeactivate(state))
                {
                    Active = false;
                    Logger.Debug($"MouseControl deactivated for XInput controller #{i}");
                    synthesizer.Speak($"Deactivated");
                    continue;
                }
                if (Active)
                {
                    try
                    {
                        updateController(state);
                    }
                    catch (Exception ex)
                    {
                        Active = false;
                        Logger.Debug($"MouseControl deactivated for XInput controller #{i} due to error {ex}");
                    }
                }
            }
        }

        private bool CheckForActivate(Gamepad state)
        {
            return state.Buttons.HasFlag(GamepadButtonFlags.Start) && state.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
        }

        private bool CheckForDeactivate(Gamepad state)
        {
            return state.Buttons.HasFlag(GamepadButtonFlags.Start) && state.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
        }

        private void updateController(Gamepad state)
        {
            // MouseMovement
            double deltaFactor = 8.0;
            if (state.Buttons.HasFlag(GamepadButtonFlags.B))
                deltaFactor /= 2;
            int deltaX = 0;
            if (state.LeftThumbX > 300 || state.LeftThumbX < -300)
            {
                deltaX = (int)((double)state.LeftThumbX / short.MaxValue * deltaFactor);
            }
            int deltaY = 0;
            if (state.LeftThumbY > 300 || state.LeftThumbY < -300)
            {
                deltaY = (int)((double)state.LeftThumbY / short.MaxValue * deltaFactor);
            }
            input.Mouse.MoveMouseBy(deltaX, -1 * deltaY);
            // ScrollWheel
            if (mouseWheelLimiter != 0)
            {
                mouseWheelLimiter--;
            }
            else
            {
                mouseWheelLimiter = 10;
                int vScroll = 0;
                if (state.RightThumbY > 300)
                {
                    vScroll = 1;
                }
                else if (state.RightThumbY < -300)
                {
                    vScroll = -1;
                }
                input.Mouse.VerticalScroll(vScroll);
            }
            // MouseButtons
            MapControllerButton("A", state, GamepadButtonFlags.A, () => input.Mouse.LeftButtonDown(), () => input.Mouse.LeftButtonUp());
            MapControllerButton("X", state, GamepadButtonFlags.X, () => input.Mouse.RightButtonDown(), () => input.Mouse.RightButtonUp());
            MapControllerButton("Y", state, GamepadButtonFlags.Y, () => input.Mouse.MiddleButtonDown(), () => input.Mouse.MiddleButtonUp());
            // ArrowButtons
            MapControllerButton("UP", state, GamepadButtonFlags.DPadUp, () => input.Keyboard.KeyDown(VirtualKeyCode.UP), () => input.Keyboard.KeyUp(VirtualKeyCode.UP));
            MapControllerButton("DOWN", state, GamepadButtonFlags.DPadDown, () => input.Keyboard.KeyDown(VirtualKeyCode.DOWN), () => input.Keyboard.KeyUp(VirtualKeyCode.DOWN));
            MapControllerButton("RIGHT", state, GamepadButtonFlags.DPadRight, () => input.Keyboard.KeyDown(VirtualKeyCode.RIGHT), () => input.Keyboard.KeyUp(VirtualKeyCode.RIGHT));
            MapControllerButton("LEFT", state, GamepadButtonFlags.DPadLeft, () => input.Keyboard.KeyDown(VirtualKeyCode.LEFT), () => input.Keyboard.KeyUp(VirtualKeyCode.LEFT));
            // Tab
            MapControllerButton("LSHOULDER", state, GamepadButtonFlags.LeftShoulder, null, () =>
            {
                input.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                input.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
                input.Keyboard.KeyPress(VirtualKeyCode.TAB);
                input.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
                input.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            });
            MapControllerButton("RSHOULDER", state, GamepadButtonFlags.RightShoulder, null, () =>
            {
                input.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                input.Keyboard.KeyPress(VirtualKeyCode.TAB);
                input.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            });
            // Control
            MapControllerButton("BACK", state, GamepadButtonFlags.Back, null, () =>
            {
                input.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                input.Keyboard.KeyPress(VirtualKeyCode.VK_W);
                input.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            });
            MapControllerButton("START", state, GamepadButtonFlags.Start, null, () =>
            {
                input.Keyboard.KeyDown(VirtualKeyCode.LMENU);
                input.Keyboard.KeyPress(VirtualKeyCode.F4);
                input.Keyboard.KeyUp(VirtualKeyCode.LMENU);
            });
            // Page
            if(state.LeftTrigger > 200 && !previousKeyStates["LTRIGGER"])
            {
                previousKeyStates["LTRIGGER"] = true;
                input.Keyboard.KeyDown(VirtualKeyCode.PRIOR);
            }
            else if (state.LeftTrigger < 50 && previousKeyStates["LTRIGGER"])
            {
                previousKeyStates["LTRIGGER"] = false;
                input.Keyboard.KeyUp(VirtualKeyCode.PRIOR);
                return;
            }
            if (state.RightTrigger > 200 && !previousKeyStates["RTRIGGER"])
            {
                previousKeyStates["RTRIGGER"] = true;
                input.Keyboard.KeyDown(VirtualKeyCode.NEXT);
            }
            else if (state.RightTrigger < 50 && previousKeyStates["RTRIGGER"])
            {
                previousKeyStates["RTRIGGER"] = false;
                input.Keyboard.KeyUp(VirtualKeyCode.NEXT);
                return;
            }
        }

        private void MapControllerButton(string marker, Gamepad state, GamepadButtonFlags button, Action pressedAction, Action releasedAction)
        {
            if (state.Buttons.HasFlag(button) && !previousKeyStates[marker])
            {
                previousKeyStates[marker] = true;
                if(pressedAction != null)
                    pressedAction.Invoke();
                return;
            }
            else if (!state.Buttons.HasFlag(button) && previousKeyStates[marker])
            {
                previousKeyStates[marker] = false;
                if (releasedAction != null)
                    releasedAction.Invoke();
                return;
            }
        }
    }
}
