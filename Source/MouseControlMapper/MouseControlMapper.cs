using ControllerMapper.Source.Mouse;
using SharpDX.XInput;
using System.Collections.Generic;
using System.Timers;

namespace ControllerMapper.Source.MouseControlMapper
{
    class MouseControlMapper
    {
        private static MouseControlMapper instance;
        private static Timer updateTimer;
        private List<MouseControlMap> controllerMaps = new List<MouseControlMap>();

        private MouseControlMapper()
        {
            Controller[] controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };
            foreach (Controller controller in controllers)
            {
                controllerMaps.Add(new MouseControlMap(controller));
            }

            updateTimer = new Timer();
            updateTimer.Elapsed += Update;
            updateTimer.Interval = 15; // ~60FPS
            updateTimer.AutoReset = false;
            updateTimer.Enabled = true;
        }

        public void Update(object source, ElapsedEventArgs ee)
        {
            foreach(MouseControlMap map in controllerMaps)
            {
                map.Update();
            }
            updateTimer.Start();
        }

        public static MouseControlMapper GetInstance()
        {
            if (instance == null)
                instance = new MouseControlMapper();
            return instance;
        }
    }
}
