using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.Input;
using consoleXstream.Remap;

namespace consoleXstream.Menu
{
    public class Keyboard
    {
        private readonly Classes _class;

        public Keyboard(Classes inClass) { _class = inClass; }

        public void CheckInput()
        {
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.DpadUp) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.DpadUp)) _class.Nav.CheckCommand("up");
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.DpadDown) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.DpadDown)) _class.Nav.CheckCommand("down");
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.DpadLeft) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.DpadLeft)) _class.Nav.CheckCommand("left");
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.DpadRight) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.DpadRight)) _class.Nav.CheckCommand("right");
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.ButtonB) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.ButtonB)) _class.Nav.CheckCommand("back");
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.ButtonBack) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.ButtonBack)) _class.Nav.CheckCommand("back");
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.ButtonA) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.ButtonA)) _class.Nav.CheckCommand("ok");
            if (_class.KeyboardHook.getKey(_class.Keymap.KeyDef.ButtonStart) || _class.KeyboardHook.getKey(_class.Keymap.KeyAltDef.ButtonStart)) _class.Nav.CheckCommand("ok");
        }
    }
}
