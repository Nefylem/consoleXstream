using consoleXstream.Config;
using consoleXstream.Input;

namespace consoleXstream.Output.TitanOne
{
    class Classes
    {
        public Classes(Write home, Form1 frmHome, Configuration system, Gamepad gamepad)
        {
            Write = home; 
            FrmMain = frmHome; 
            System = system;
            Gamepad = gamepad;
        }

        public Form1 FrmMain;
        public Write Write;
        public Configuration System;
        public KeyboardInterface KeyboardInterface;
        public Gamepad Gamepad;

        public Init Init;
        public Define Define;

        public void Create()
        {
            Init = new Init(this);
            Define = new Define();
        }
    }
}
