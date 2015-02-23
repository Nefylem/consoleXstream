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
    class Keyboard
    {
        private Navigation _nav;
        private KeyboardHook _keyboard;
        private Keymap _keymap;

        public void GetNavHandle(Navigation nav) { _nav = nav; }
        public void GetKeyHookHandle(KeyboardHook keyHook) { _keyboard = keyHook; }
        public void GetKeymapHandle(Keymap keymap) { _keymap = keymap; }

        public void CheckInput()
        {
            if (_keyboard.getKey(_keymap.KeyDef.DpadUp) || _keyboard.getKey(_keymap.KeyAltDef.DpadUp)) _nav.CheckCommand("up");
            if (_keyboard.getKey(_keymap.KeyDef.DpadDown) || _keyboard.getKey(_keymap.KeyAltDef.DpadDown)) _nav.CheckCommand("down");
            if (_keyboard.getKey(_keymap.KeyDef.DpadLeft) || _keyboard.getKey(_keymap.KeyAltDef.DpadLeft)) _nav.CheckCommand("left");
            if (_keyboard.getKey(_keymap.KeyDef.DpadRight) || _keyboard.getKey(_keymap.KeyAltDef.DpadRight)) _nav.CheckCommand("right");
            if (_keyboard.getKey(_keymap.KeyDef.ButtonB) || _keyboard.getKey(_keymap.KeyAltDef.ButtonB)) _nav.CheckCommand("back");
            if (_keyboard.getKey(_keymap.KeyDef.ButtonBack) || _keyboard.getKey(_keymap.KeyAltDef.ButtonBack)) _nav.CheckCommand("back");
            if (_keyboard.getKey(_keymap.KeyDef.ButtonA) || _keyboard.getKey(_keymap.KeyAltDef.ButtonA)) _nav.CheckCommand("ok");
            if (_keyboard.getKey(_keymap.KeyDef.ButtonStart) || _keyboard.getKey(_keymap.KeyAltDef.ButtonStart)) _nav.CheckCommand("ok");
        }
    }
}
