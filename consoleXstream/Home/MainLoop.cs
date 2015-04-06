namespace consoleXstream.Home
{
    public class MainLoop
    {
        public MainLoop(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Run()
        {
            var altKeyMap = _class.BaseClass.Keymap.KeyAltDef;
            var keymap = _class.BaseClass.Keymap.KeyDef;

            var checkTitan = _class.CheckTitanDevices;
            var keyboard = _class.BaseClass.Keyboard;
            var keyboardInterface = _class.BaseClass.KeyboardInterface;
            var gamepad = _class.Controller;
            var menu = _class.Menu;
            var mouse = _class.Mouse;
            var system = _class.BaseClass.System;
            var var = _class.Var;
            var videoCapture = _class.BaseClass.VideoCapture;


            if (var.IsUpdatingTitanOneList) checkTitan.ConnectionList();
            if (var.BlockMenuCount > 0) var.BlockMenuCount--;

            if (system.UseInternalCapture && videoCapture.BoolActiveVideo)
                videoCapture.checkVideoOutput();

            if (system.boolMenu)
                return;

            if (system.IsEnableKeyboard)
            {
                if (keyboard.GetKey(keymap.ButtonBack) || keyboard.GetKey(altKeyMap.ButtonBack))
                    if (var.BlockMenuCount == 0) menu.Open(); else var.BlockMenuCount = 3;

                keyboardInterface.Check();
            }

            if (system.IsEnableMouse || system.IsVr)
                mouse.Check();

            gamepad.Check();
        }

    }
}
