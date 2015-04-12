using consoleXstream.Config;
using consoleXstream.Input;

namespace consoleXstream.Home.Loop
{
    public class MainLoop
    {
        public MainLoop(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private void SetupClasses()
        {
            /*
            _setup = true;

            _altKeyMap = _class.BaseClass.Keymap.KeyAltDef;
            _keymap = _class.BaseClass.Keymap.KeyDef;

            _checkTitan = _class.CheckTitanDevices;
            _keyboard = _class.BaseClass.Keyboard;
            _keyboardInterface = _class.BaseClass.KeyboardInterface;
            _gamepad = _class.Controller;
            _menu = _class.Menu;
            _mouse = _class.Mouse;
            _system = _class.BaseClass.System;
            _var = _class.Var;
            _videoCapture = _class.BaseClass.VideoCapture;
             */
        }

        //Delete me
        public void Run()
        {
            /*
            if (_class.Var.IsUpdatingTitanOneList) _checkTitan.ConnectionList();
            if (_class.Var.BlockMenuCount > 0) _class.Var.BlockMenuCount--;

            if (_class.BaseClass.System.UseInternalCapture && _videoCapture.BoolActiveVideo)
                _videoCapture.CheckVideoOutput();

            if (_class.BaseClass.System.boolMenu)
                return;


            if (_class.BaseClass.System.IsEnableKeyboard)
            {
                if (_keyboard.GetKey(_keymap.ButtonBack) || _keyboard.GetKey(_altKeyMap.ButtonBack))
                    if (_var.BlockMenuCount == 0) _menu.Open(); else _var.BlockMenuCount = 3;

                _keyboardInterface.Check();
            }

            if (_system.IsEnableMouse || _system.IsVr)
                _mouse.Check();

            _gamepad.Check();
             */
        }

        //Delete me
        public void CheckLoop()
        {
            /*
            if (_class.Var.RetrySetTitanOne != null) _class.CheckTitanDevices.Confirm();

            Run();

            if (_class.BaseClass.System.CheckFps) 
                _class.CheckFps.Read();
             */
        }

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
            
            if (_class.BaseClass.System.IsEnableKeyboard)
            {
                if (_class.BaseClass.Keyboard.GetKey(keymap.ButtonBack) || _class.BaseClass.Keyboard.GetKey(altKeyMap.ButtonBack))
                    if (_class.Var.BlockMenuCount == 0) _class.Menu.Open(); else _class.Var.BlockMenuCount = 3;

                _class.BaseClass.KeyboardInterface.Check();
            }

            if (_class.BaseClass.System.IsEnableMouse || _class.BaseClass.System.IsVr)
                _class.Mouse.Check();

            _class.Controller.Check(true);
        }
    }
}
