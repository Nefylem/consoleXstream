using System;
using consoleXstream.Remap;
using DirectShowLib.BDA;

namespace consoleXstream.Input
{
    public class KeyboardInterface
    {
        /*
        public KeyboardInterface(BaseClass classes)
        {
            _class = classes;
            system = _class.System;
            keyboard = _class.Keyboard;
            _remap = _class.Remap;
            _keymap = _class._keymap;
        }
        private BaseClass _class;
        */

        private Form1 frmMain;
        private Config.Configuration system;
        private KeyboardHook keyboard;
        private Remapping _remap;
        private Keymap _keymap;

        public byte[] output;
        public bool boolLeftMouse;
        public bool boolRightMouse;

        private int _intXboxCount;


        public KeyboardInterface(Form1 mainForm) { frmMain = mainForm; }
        public void getSystemHandle(Config.Configuration inSystem) { system = inSystem; }
        public void getKeyboardHandle(KeyboardHook inKey) { keyboard = inKey; }
        public void getRemapHangle(Remapping remap) { _remap = remap; }
        public void getKeymapHandle(Keymap keymap) { _keymap = keymap; }

        public void checkKeys()
        {
            
            if (_intXboxCount == 0) { _intXboxCount = Enum.GetNames(typeof(Xbox)).Length; }
            output = new byte[_intXboxCount];
            if (keyboard.GetKey(_keymap.KeyDef.DpadDown)) output[_remap.RemapGamepad.Down] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.DpadUp)) output[_remap.RemapGamepad.Up] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.DpadLeft)) output[_remap.RemapGamepad.Left] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.DpadRight)) output[_remap.RemapGamepad.Right] = Convert.ToByte(100);

            if (keyboard.GetKey(_keymap.KeyAltDef.DpadDown)) output[_remap.RemapGamepad.Down] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyAltDef.DpadUp)) output[_remap.RemapGamepad.Up] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyAltDef.DpadLeft)) output[_remap.RemapGamepad.Left] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyAltDef.DpadRight)) output[_remap.RemapGamepad.Right] = Convert.ToByte(100);

            if (keyboard.GetKey(_keymap.KeyDef.ButtonA)) output[_remap.RemapGamepad.A] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.ButtonB)) output[_remap.RemapGamepad.B] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.ButtonX)) output[_remap.RemapGamepad.X] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.ButtonY)) output[_remap.RemapGamepad.Y] = Convert.ToByte(100);

            if (keyboard.GetKey(_keymap.KeyAltDef.ButtonA)) output[_remap.RemapGamepad.A] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyAltDef.ButtonB)) output[_remap.RemapGamepad.B] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyAltDef.ButtonX)) output[_remap.RemapGamepad.X] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyAltDef.ButtonY)) output[_remap.RemapGamepad.Y] = Convert.ToByte(100);

            int intModified = 100;

            if (keyboard.GetKey(_keymap.KeyDef.Modifier)) intModified = 50;

            if (keyboard.GetKey(_keymap.KeyDef.LXleft)) output[_remap.RemapGamepad.LeftX] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.GetKey(_keymap.KeyDef.LXright)) output[_remap.RemapGamepad.LeftX] = (byte)Convert.ToSByte((int)(intModified));
            if (keyboard.GetKey(_keymap.KeyDef.LYup)) output[_remap.RemapGamepad.LeftY] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.GetKey(_keymap.KeyDef.LYdown)) output[_remap.RemapGamepad.LeftY] = (byte)Convert.ToSByte((int)(intModified));

            if (keyboard.GetKey(_keymap.KeyDef.RXleft)) output[_remap.RemapGamepad.RightX] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.GetKey(_keymap.KeyDef.RXright)) output[_remap.RemapGamepad.RightX] = (byte)Convert.ToSByte((int)(intModified));
            if (keyboard.GetKey(_keymap.KeyDef.RYup)) output[_remap.RemapGamepad.RightY] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.GetKey(_keymap.KeyDef.RYdown)) output[_remap.RemapGamepad.RightY] = (byte)Convert.ToSByte((int)(intModified));

            if (keyboard.GetKey(_keymap.KeyDef.ButtonBack)) output[_remap.RemapGamepad.Back] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.ButtonStart)) output[_remap.RemapGamepad.Start] = Convert.ToByte(100);
            if (keyboard.GetKey(_keymap.KeyDef.ButtonHome)) output[_remap.RemapGamepad.Home] = Convert.ToByte(100);

            if (system.IsEnableMouse)
            {
                int intReplaceX = frmMain.intReplaceX;
                int intReplaceY = frmMain.intReplaceY;

                //frmMain.intReplaceX = (int)(intReplaceX / 1.5);        
                //frmMain.intReplaceY = (int)(intReplaceX / 1.5);

                //TODO: mouse config
                if (intReplaceX != 0)
                    output[_remap.RemapGamepad.RightX] = (byte)Convert.ToSByte((int)(intReplaceX));
                if (intReplaceY != 0)
                    output[_remap.RemapGamepad.RightY] = (byte)Convert.ToSByte((int)(intReplaceY));

                //TODO: mouse button map
                if (boolLeftMouse)
                    output[_remap.RemapGamepad.RightTrigger] = Convert.ToByte(100);

                if (boolRightMouse)
                    output[_remap.RemapGamepad.LeftTrigger] = Convert.ToByte(100);
            }
        }
    }
}
