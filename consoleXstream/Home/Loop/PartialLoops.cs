using System.Windows.Forms;
using consoleXstream.Config;
using consoleXstream.Input;

namespace consoleXstream.Home.Loop
{
    public class PartialLoops
    {
        public PartialLoops(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void ControllerThread(object state)
        {
            if (_class.BaseClass.System.boolMenu) return;

            _class.Controller.Check(false);
        }
    }
}
