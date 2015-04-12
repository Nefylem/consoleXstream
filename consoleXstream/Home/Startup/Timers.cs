using System.Collections.Generic;
using System.Threading;
using Timer = System.Timers.Timer;

namespace consoleXstream.Home.Startup
{
    public class Timers
    {
        public Timers(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private List<System.Threading.Timer> _sysTimer;
 
        public void Create()
        {
            _sysTimer = new List<System.Threading.Timer>();

            var system = _class.BaseClass.System;
            var main = _class.MainLoop;

            if (system.MainThreads <= 1) return;

            for (int count = 0; count < system.MainThreads - 1; count++)
            {
                var newTimer = new System.Threading.Timer(_class.PartialLoop.ControllerThread, null, Timeout.Infinite, 1000);
                _sysTimer.Add(newTimer);
            }
        }

        public void StartAll()
        {
            for (int count = 0; count < _sysTimer.Count; count++)
            {
                Start(count);
            }
        }

        public void StopAll()
        {
            for (int count = 0; count < _sysTimer.Count; count++)
            {
                Stop(count);
            } 
        }

        public void Start(int timer)
        {
            _sysTimer[timer].Change(0, 1);
        }

        public void Stop(int timer)
        {
            _sysTimer[timer].Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
