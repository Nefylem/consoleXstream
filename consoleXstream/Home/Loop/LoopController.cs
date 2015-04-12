namespace consoleXstream.Home.Loop
{
    public class LoopController
    {
        public LoopController(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void RunSystemLoop()
        {
            _class.MainLoop.RunSystemLoop();
        }

        public void RunMasterControlLoop()
        {
            if (_class.BaseClass.System.IsCalibrateHeadMotion) _class.MainLoop.RunMasterControlLoopPartial();

            if (_class.BaseClass.System.boolMenu) return;

            _class.MainLoop.RunMasterControlLoop();
        }

        public void RunControlLoop()
        {
            /*
            if (_class.BaseClass.System.boolMenu) 
                return;

            _class.PartialLoop.CheckControls();
             */
        }
    }
}
