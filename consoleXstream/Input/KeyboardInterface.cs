using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.Input;

namespace consoleXstream.Input
{
    public class KeyboardInterface
    {
        private Form1 frmMain;
        private Config.Configuration system;
        private KeyboardHook keyboard;

        public byte[] output;
        public bool boolLeftMouse;
        public bool boolRightMouse;

        private int _intXboxCount;

        public KeyboardInterface(Form1 mainForm) { frmMain = mainForm; }
        public void getSystemHandle(Config.Configuration inSystem) { system = inSystem; }
        public void getKeyboardHandle(KeyboardHook inKey) { keyboard = inKey; }

        public void checkKeys()
        {
            
            if (_intXboxCount == 0) { _intXboxCount = Enum.GetNames(typeof(xbox)).Length; }
            output = new byte[_intXboxCount];
            /*
            if (keyboard.getKey(system.keyDef.strDpadDown)) output[(int)xbox.down] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strDpadUp)) output[(int)xbox.up] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strDpadLeft)) output[(int)xbox.left] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strDpadRight)) output[(int)xbox.right] = Convert.ToByte(100);

            if (keyboard.getKey(system.keyAltDef.strDpadDown)) output[(int)xbox.down] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyAltDef.strDpadUp)) output[(int)xbox.up] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyAltDef.strDpadLeft)) output[(int)xbox.left] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyAltDef.strDpadRight)) output[(int)xbox.right] = Convert.ToByte(100);

            if (keyboard.getKey(system.keyDef.strButtonA)) output[(int)xbox.a] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strButtonB)) output[(int)xbox.b] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strButtonX)) output[(int)xbox.x] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strButtonY)) output[(int)xbox.y] = Convert.ToByte(100);

            if (keyboard.getKey(system.keyAltDef.strButtonA)) output[(int)xbox.a] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyAltDef.strButtonB)) output[(int)xbox.b] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyAltDef.strButtonX)) output[(int)xbox.x] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyAltDef.strButtonY)) output[(int)xbox.y] = Convert.ToByte(100);

            int intModified = 100;

            if (keyboard.getKey(system.keyDef.strModifier)) intModified = 50;
 
            if (keyboard.getKey(system.keyDef.strLXleft)) output[(int)xbox.leftX] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(system.keyDef.strLXright)) output[(int)xbox.leftX] = (byte)Convert.ToSByte((int)(intModified));
            if (keyboard.getKey(system.keyDef.strLYup)) output[(int)xbox.leftY] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(system.keyDef.strLYdown)) output[(int)xbox.leftY] = (byte)Convert.ToSByte((int)(intModified));

            if (keyboard.getKey(system.keyDef.strRXleft)) output[(int)xbox.rightX] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(system.keyDef.strRXright)) output[(int)xbox.rightX] = (byte)Convert.ToSByte((int)(intModified));
            if (keyboard.getKey(system.keyDef.strRYup)) output[(int)xbox.rightY] = (byte)Convert.ToSByte((int)(-intModified));
            if (keyboard.getKey(system.keyDef.strRYdown)) output[(int)xbox.rightY] = (byte)Convert.ToSByte((int)(intModified));

            if (keyboard.getKey(system.keyDef.strButtonBack)) output[(int)xbox.back] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strButtonStart)) output[(int)xbox.start] = Convert.ToByte(100);
            if (keyboard.getKey(system.keyDef.strButtonHome)) output[(int)xbox.home] = Convert.ToByte(100);

            if (system.boolEnableMouse)
            {
                int intReplaceX = frmMain.intReplaceX;
                int intReplaceY = frmMain.intReplaceY;

                //frmMain.intReplaceX = (int)(intReplaceX / 1.5);        
                //frmMain.intReplaceY = (int)(intReplaceX / 1.5);

                //TODO: mouse config
                if (intReplaceX != 0)
                    output[(int)xbox.rightX] = (byte)Convert.ToSByte((int)(intReplaceX));
                if (intReplaceY != 0)
                    output[(int)xbox.rightY] = (byte)Convert.ToSByte((int)(intReplaceY));

                //TODO: mouse button map
                if (boolLeftMouse)
                    output[(int)xbox.rightTrigger] = Convert.ToByte(100);

                if (boolRightMouse)
                    output[(int)xbox.leftTrigger] = Convert.ToByte(100);
            }
             */
        }
    }
}
