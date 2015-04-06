using System;
using consoleXstream.Home;

namespace consoleXstream.Input
{
    public class KeyboardInterface
    {
        public KeyboardInterface(BaseClass baseClass) { _class = baseClass; }
        private readonly BaseClass _class;

        public byte[] Output;
        public bool BoolLeftMouse;
        public bool BoolRightMouse;

        private int _intXboxCount;

        public void Check()
        {
            
            if (_intXboxCount == 0) { _intXboxCount = Enum.GetNames(typeof(Xbox)).Length; }
            Output = new byte[_intXboxCount];

            CheckKeyMap();
            CheckAlternateKeyMap();           

            var intModified = 100;

            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.Modifier)) intModified = 50;

            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.LXleft)) Output[_class.Remap.RemapGamepad.LeftX] = (byte)Convert.ToSByte(-intModified);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.LXright)) Output[_class.Remap.RemapGamepad.LeftX] = (byte)Convert.ToSByte(intModified);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.LYup)) Output[_class.Remap.RemapGamepad.LeftY] = (byte)Convert.ToSByte(-intModified);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.LYdown)) Output[_class.Remap.RemapGamepad.LeftY] = (byte)Convert.ToSByte(intModified);

            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.RXleft)) Output[_class.Remap.RemapGamepad.RightX] = (byte)Convert.ToSByte(-intModified);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.RXright)) Output[_class.Remap.RemapGamepad.RightX] = (byte)Convert.ToSByte(intModified);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.RYup)) Output[_class.Remap.RemapGamepad.RightY] = (byte)Convert.ToSByte(-intModified);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.RYdown)) Output[_class.Remap.RemapGamepad.RightY] = (byte)Convert.ToSByte(intModified);

            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonBack)) Output[_class.Remap.RemapGamepad.Back] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonStart)) Output[_class.Remap.RemapGamepad.Start] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonHome)) Output[_class.Remap.RemapGamepad.Home] = Convert.ToByte(100);

            if (!_class.System.IsEnableMouse) return;
            var intReplaceX = _class.HomeClass.Var.MouseX;
            var intReplaceY = _class.HomeClass.Var.MouseY;

            //frmMain.intReplaceX = (int)(intReplaceX / 1.5);        
            //frmMain.intReplaceY = (int)(intReplaceX / 1.5);

            if (intReplaceX != 0)
                Output[_class.Remap.RemapGamepad.RightX] = (byte)Convert.ToSByte(intReplaceX);
            if (intReplaceY != 0)
                Output[_class.Remap.RemapGamepad.RightY] = (byte)Convert.ToSByte(intReplaceY);

            if (BoolLeftMouse)
                Output[_class.Remap.RemapGamepad.RightTrigger] = Convert.ToByte(100);

            if (BoolRightMouse)
                Output[_class.Remap.RemapGamepad.LeftTrigger] = Convert.ToByte(100);
        }

        private void CheckKeyMap()
        {
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.DpadDown)) Output[_class.Remap.RemapGamepad.Down] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.DpadUp)) Output[_class.Remap.RemapGamepad.Up] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.DpadLeft)) Output[_class.Remap.RemapGamepad.Left] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.DpadRight)) Output[_class.Remap.RemapGamepad.Right] = Convert.ToByte(100);

            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonA)) Output[_class.Remap.RemapGamepad.A] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonB)) Output[_class.Remap.RemapGamepad.B] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonX)) Output[_class.Remap.RemapGamepad.X] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonY)) Output[_class.Remap.RemapGamepad.Y] = Convert.ToByte(100);

        }

        private void CheckAlternateKeyMap()
        {
            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.DpadDown)) Output[_class.Remap.RemapGamepad.Down] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.DpadUp)) Output[_class.Remap.RemapGamepad.Up] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.DpadLeft)) Output[_class.Remap.RemapGamepad.Left] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.DpadRight)) Output[_class.Remap.RemapGamepad.Right] = Convert.ToByte(100);

            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.ButtonA)) Output[_class.Remap.RemapGamepad.A] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.ButtonB)) Output[_class.Remap.RemapGamepad.B] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.ButtonX)) Output[_class.Remap.RemapGamepad.X] = Convert.ToByte(100);
            if (_class.Keyboard.GetKey(_class.Keymap.KeyAltDef.ButtonY)) Output[_class.Remap.RemapGamepad.Y] = Convert.ToByte(100);            
        }
    }
}
