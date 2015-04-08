using consoleXstream.Config;
using consoleXstream.Input;

namespace consoleXstream.Home.Loop
{
    public class MainLoop
    {
        public MainLoop(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private Remap.Keymap.KeyboardKeys _altKeyMap;
        private Remap.Keymap.KeyboardKeys _keymap;

        private CheckTitanDevices _checkTitan;
        private KeyboardHook _keyboard;
        private KeyboardInterface _keyboardInterface;
        private ControllerInput _gamepad;
        private MenuHandler _menu;
        private Mouse _mouse;
        private Configuration _system;
        private Var _var;
        private VideoCapture.VideoCapture _videoCapture;

        private bool _setup;

        private void SetupClasses()
        {
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
        }

        public void Run()
        {
            if (!_setup) SetupClasses();
            if (_var.IsUpdatingTitanOneList) _checkTitan.ConnectionList();
            if (_var.BlockMenuCount > 0) _var.BlockMenuCount--;

            if (_system.UseInternalCapture && _videoCapture.BoolActiveVideo)
                _videoCapture.CheckVideoOutput();

            if (_system.boolMenu)
                return;

            
            if (_system.IsEnableKeyboard)
            {
                if (_keyboard.GetKey(_keymap.ButtonBack) || _keyboard.GetKey(_altKeyMap.ButtonBack))
                    if (_var.BlockMenuCount == 0) _menu.Open(); else _var.BlockMenuCount = 3;

                _keyboardInterface.Check();
            }

            if (_system.IsEnableMouse || _system.IsVr)
                _mouse.Check();

            _gamepad.Check();
        }

        public void CheckLoop()
        {
            if (_class.Var.RetrySetTitanOne != null) _class.CheckTitanDevices.Confirm();

            Run();

            if (_class.BaseClass.System.CheckFps) 
                _class.CheckFps.Read();
        }
    }
}
