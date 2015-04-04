namespace consoleXstream.Menu
{
    public class Keyboard
    {
        private readonly Classes _class;

        public Keyboard(Classes inClass) { _class = inClass; }

        public void CheckInput()
        {
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadUp) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadUp)) _class.Nav.CheckCommand("up");
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadDown) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadDown)) _class.Nav.CheckCommand("down");
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadLeft) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadLeft)) _class.Nav.CheckCommand("left");
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadRight) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadRight)) _class.Nav.CheckCommand("right");
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.ButtonB) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.ButtonB)) _class.Nav.CheckCommand("back");
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.ButtonBack) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.ButtonBack)) _class.Nav.CheckCommand("back");
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.ButtonA) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.ButtonA)) _class.Nav.CheckCommand("ok");
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.ButtonStart) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.ButtonStart)) _class.Nav.CheckCommand("ok");
        }
    }
}
