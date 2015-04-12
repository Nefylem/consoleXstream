namespace consoleXstream.Home.Loop
{
    public class MainLoop
    {
        public MainLoop(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void RunSystemLoop()
        {
            if (_class.Var.RetrySetTitanOne != null) _class.CheckTitanDevices.Confirm();
            if (_class.Var.IsUpdatingTitanOneList) _class.CheckTitanDevices.ConnectionList();

            if (_class.Var.BlockMenuCount > 0) _class.Var.BlockMenuCount--;

            if (_class.BaseClass.System.UseInternalCapture && _class.BaseClass.VideoCapture.BoolActiveVideo) _class.BaseClass.VideoCapture.CheckVideoOutput();
        }

        public void RunMasterControlLoop()
        {
            var keymap = _class.BaseClass.Keymap.KeyDef;
            var altKeyMap = _class.BaseClass.Keymap.KeyAltDef;
            
            if (_class.BaseClass.System.IsEnableKeyboard || _class.BaseClass.System.IsVr)
            {
                if (_class.BaseClass.Keyboard.GetKey(keymap.ButtonBack) || _class.BaseClass.Keyboard.GetKey(altKeyMap.ButtonBack))
                    if (_class.Var.BlockMenuCount == 0) _class.Menu.Open(); else _class.Var.BlockMenuCount = 3;

                if (_class.BaseClass.System.IsEnableMouse || _class.BaseClass.System.IsVr)
                    _class.Mouse.Check();

                _class.BaseClass.KeyboardInterface.Check();
            }

            _class.Controller.Check(true);
        }

        //Only allows for mouse motion
        public void RunMasterControlLoopPartial()
        {
            _class.Mouse.Check();
            _class.BaseClass.KeyboardInterface.Check();
            _class.Controller.Send(_class.BaseClass.KeyboardInterface.Output);
        }
    }
}
