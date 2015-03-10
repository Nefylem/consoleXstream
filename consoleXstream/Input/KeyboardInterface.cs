using System;
using consoleXstream.Remap;

namespace consoleXstream.Input
{
    public class KeyboardInterface
    {
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
            if (keyboard.getKey(_keymap.KeyDef.DpadDown)) output[_remap.remapGamepad.down] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.DpadUp)) output[_remap.remapGamepad.up] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.DpadLeft)) output[_remap.remapGamepad.left] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.DpadRight)) output[_remap.remapGamepad.right] = Convert.ToByte(100);

            if (keyboard.getKey(_keymap.KeyAltDef.DpadDown)) output[_remap.remapGamepad.down] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyAltDef.DpadUp)) output[_remap.remapGamepad.up] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyAltDef.DpadLeft)) output[_remap.remapGamepad.left] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyAltDef.DpadRight)) output[_remap.remapGamepad.right] = Convert.ToByte(100);

            if (keyboard.getKey(_keymap.KeyDef.ButtonA)) output[_remap.remapGamepad.a] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.ButtonB)) output[_remap.remapGamepad.b] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.ButtonX)) output[_remap.remapGamepad.x] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.ButtonY)) output[_remap.remapGamepad.y] = Convert.ToByte(100);

            if (keyboard.getKey(_keymap.KeyAltDef.ButtonA)) output[_remap.remapGamepad.a] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyAltDef.ButtonB)) output[_remap.remapGamepad.b] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyAltDef.ButtonX)) output[_remap.remapGamepad.x] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyAltDef.ButtonY)) output[_remap.remapGamepad.y] = Convert.ToByte(100);

            int intModified = 100;

            if (keyboard.getKey(_keymap.KeyDef.Modifier)) intModified = 50;

            if (keyboard.getKey(_keymap.KeyDef.LXleft)) output[_remap.remapGamepad.leftX] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(_keymap.KeyDef.LXright)) output[_remap.remapGamepad.leftX] = (byte)Convert.ToSByte((int)(intModified));
            if (keyboard.getKey(_keymap.KeyDef.LYup)) output[_remap.remapGamepad.leftY] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(_keymap.KeyDef.LYdown)) output[_remap.remapGamepad.leftY] = (byte)Convert.ToSByte((int)(intModified));

            if (keyboard.getKey(_keymap.KeyDef.RXleft)) output[_remap.remapGamepad.rightX] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(_keymap.KeyDef.RXright)) output[_remap.remapGamepad.rightX] = (byte)Convert.ToSByte((int)(intModified));
            if (keyboard.getKey(_keymap.KeyDef.RYup)) output[_remap.remapGamepad.rightY] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(_keymap.KeyDef.RYdown)) output[_remap.remapGamepad.rightY] = (byte)Convert.ToSByte((int)(intModified));

            if (keyboard.getKey(_keymap.KeyDef.ButtonBack)) output[_remap.remapGamepad.back] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.ButtonStart)) output[_remap.remapGamepad.start] = Convert.ToByte(100);
            if (keyboard.getKey(_keymap.KeyDef.ButtonHome)) output[_remap.remapGamepad.home] = Convert.ToByte(100);

            if (system.boolEnableMouse)
            {
                int intReplaceX = frmMain.intReplaceX;
                int intReplaceY = frmMain.intReplaceY;

                //frmMain.intReplaceX = (int)(intReplaceX / 1.5);        
                //frmMain.intReplaceY = (int)(intReplaceX / 1.5);

                //TODO: mouse config
                if (intReplaceX != 0)
                    output[_remap.remapGamepad.rightX] = (byte)Convert.ToSByte((int)(intReplaceX));
                if (intReplaceY != 0)
                    output[_remap.remapGamepad.rightY] = (byte)Convert.ToSByte((int)(intReplaceY));

                //TODO: mouse button map
                if (boolLeftMouse)
                    output[_remap.remapGamepad.rightTrigger] = Convert.ToByte(100);

                if (boolRightMouse)
                    output[_remap.remapGamepad.leftTrigger] = Convert.ToByte(100);
            }
        }
    }
}
