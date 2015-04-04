using System;
using System.ComponentModel;
using System.Drawing;
using consoleXstream.Config;
using consoleXstream.Input;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.Remap
{
    public class Remap
    {
        public Remap(Classes classes) { _class = classes; }
        private Classes _class;

        public void ChangeRemapScreen(string command)
        {
            command = command.ToLower();
            if (command == "gamepad") _class.RemapGamepad.Setup();
        }
    }
}
