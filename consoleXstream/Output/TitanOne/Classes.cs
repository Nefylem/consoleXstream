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
        public Configuration System;
        public KeyboardInterface KeyboardInterface;
        public Gamepad Gamepad;

        public Init Init;
        public Define Define;
        public Write Write;

        public GCMAPI.Init MInit;
        public GCMAPI.Define MDefine;
        public GCMAPI.Write MWrite;
        public GCMAPI.Devices MDevices;

        public void Create()
        {
            Init = new Init(this);
            Define = new Define();

            MInit = new GCMAPI.Init(this);
            MDefine = new GCMAPI.Define();
            MDevices = new GCMAPI.Devices(this);
            MWrite = new GCMAPI.Write(this);
        }
    }
}
