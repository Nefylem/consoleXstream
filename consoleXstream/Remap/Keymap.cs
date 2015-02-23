namespace consoleXstream.Remap
{
    public class Keymap
    {
        public class KeyboardKeys
        {
            public string DpadDown { get; set; }
            public string DpadUp { get; set; }
            public string DpadLeft { get; set; }
            public string DpadRight { get; set; }

            public string ButtonA { get; set; }
            public string ButtonB { get; set; }
            public string ButtonX { get; set; }
            public string ButtonY { get; set; }

            public string LXleft { get; set; }
            public string LXright { get; set; }
            public string LYup { get; set; }
            public string LYdown { get; set; }

            public string RXleft { get; set; }
            public string RXright { get; set; }
            public string RYup { get; set; }
            public string RYdown { get; set; }

            public string Modifier { get; set; }
            public string RightModifier { get; set; }

            public string ButtonBack { get; set; }
            public string ButtonStart { get; set; }
            public string ButtonHome { get; set; }
        }

        public KeyboardKeys KeyDef;
        public KeyboardKeys KeyAltDef;

        public void InitializeKeyboardDefaults()
        {
            KeyDef = new KeyboardKeys();
            KeyAltDef = new KeyboardKeys();

            KeyDef.DpadDown = "DOWN";
            KeyDef.DpadUp = "UP";
            KeyDef.DpadLeft = "LEFT";
            KeyDef.DpadRight = "RIGHT";

            KeyDef.ButtonY = "KEY_I";
            KeyDef.ButtonX = "KEY_J";
            KeyDef.ButtonA = "KEY_K";
            KeyDef.ButtonB = "KEY_L";
            
            KeyDef.LXleft = "KEY_A";
            KeyDef.LXright = "KEY_D";
            KeyDef.LYup = "KEY_W";
            KeyDef.LYdown = "KEY_S";

            KeyDef.RXleft = "NUMPAD4";
            KeyDef.RXright = "NUMPAD6";
            KeyDef.RYup = "NUMPAD8";
            KeyDef.RYdown = "NUMPAD2";

            KeyDef.Modifier = "LSHIFT";
            KeyDef.RightModifier = "RSHIFT";

            KeyDef.ButtonBack = "ESCAPE";
            KeyDef.ButtonHome = "F2";
            KeyDef.ButtonStart = "F3";

            KeyAltDef.ButtonBack = "F4";

            KeyAltDef.ButtonA = "RETURN";
            KeyAltDef.ButtonB = "BACK";
        }

        public void LoadKeyboardInputs()
        {
            
        }

    }
}
