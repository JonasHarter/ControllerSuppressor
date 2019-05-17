using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SharpDX.XInput;

namespace ControllerMapper.Source.Mouse
{
    class MouseControl
    {
        private static MouseControl instance;
        private static Timer updateTimer;

        private MouseControl()
        {
            updateTimer = new Timer();
            updateTimer.Elapsed += Update;
            updateTimer.Interval = 15; // ~60FPS
            updateTimer.Enabled = true;
        }

        public static MouseControl GetInstance()
        {
            if (instance == null)
                instance = new MouseControl();
            return instance;
        }

        public void Update(object source, ElapsedEventArgs ee)
        {
            // Initialize XInput
            var controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };
            foreach (var selectControler in controllers)
            {
                if (selectControler.IsConnected)
                    updateController(selectControler);
            }
        }

        private void updateController(Controller controller)
        {
            //Console.WriteLine(controller.UserIndex);
            var state = controller.GetState();
            Console.WriteLine(state.Gamepad);
        }
    }
}
