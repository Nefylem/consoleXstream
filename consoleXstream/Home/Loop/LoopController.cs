namespace consoleXstream.Home.Loop
{
    public class LoopController
    {
        public LoopController(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void RunSystemLoop()
        {
            if (_class.BaseClass.System.MainThreads == 1)
                _class.MainLoop.CheckLoop();
            else
                _class.PartialLoop.SystemLoop();
        }

        public void RunControlLoop()
        {
            if (_class.BaseClass.System.boolMenu) 
                return;

            _class.PartialLoop.CheckControls();
        }
    }
}
