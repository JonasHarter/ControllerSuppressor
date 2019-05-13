using SharpDX.DirectInput;
using System;

namespace ControllerSupressor.Source.Input
{
    class HoriBattlepadSwitch : DirectInputController
    {
        internal HoriBattlepadSwitch(DirectInput directInput, DeviceInstance deviceInstance) : base(directInput, deviceInstance)
        {

        }

        internal override bool checkForActivation()
        {
            while (true)
            {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                    Console.WriteLine(state);
            }
        }

        internal override void getXInputState()
        {
            throw new NotImplementedException();
        }
    }
}
