using System;

namespace consoleXstream.Home
{
    public class CheckFps
    {
        public CheckFps(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public int WatchFps;
        private int _currentFps;
        private string _checkLastFps;

        public void Read()
        {
            if (_checkLastFps != DateTime.Now.ToString("ss"))
            {
                WatchFps = _currentFps;
                _currentFps = 0;
                _checkLastFps = DateTime.Now.ToString("ss");
            }
            else
                _currentFps++;
        }
    }
}
