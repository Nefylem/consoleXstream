using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Menu.Data
{
    public class FrameCount
    {
        public int Frames;

        private int _currentFrames;
        private string _lastTimeCheck;

        public void CheckFps()
        {
            if (DateTime.Now.ToString("ss") == _lastTimeCheck) { _currentFrames++; return; }

            Frames = _currentFrames;
            _currentFrames = 0;
            _lastTimeCheck = DateTime.Now.ToString("ss");
        }
    }
}
