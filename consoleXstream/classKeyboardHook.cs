using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace consoleXstream
{
    public class classKeyboardHook
    {
        KeyboardHook keyboardHook = new KeyboardHook();

        private Form1 frmMain;

        private bool _boolKeyHookActive;
        private List<string> _listKey;

        public classKeyboardHook(Form1 mainForm) { frmMain = mainForm; }

        public void enableKeyboardHook()
        {
            if (_boolKeyHookActive == false)
            {
                _boolKeyHookActive = true;
                keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
                keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);

                _listKey = new List<string>();

                keyboardHook.Install();
            }
        }

        public void disableKeyHook()
        {
            keyboardHook.Uninstall();
        }

        void keyboardHook_KeyUp(KeyboardHook.VKeys key) { setKey(key, false); }
        void keyboardHook_KeyDown(KeyboardHook.VKeys key) { setKey(key, true); }

        private void setKey(KeyboardHook.VKeys key, bool boolSet)
        {
            string strKey = key.ToString();

            int intIndex = _listKey.IndexOf(strKey);

            if (boolSet)
            {
                if (intIndex == -1) 
                    _listKey.Add(strKey);
            }
            else
            {
                if (intIndex > -1)
                    _listKey.RemoveAt(intIndex);
            }
        }

        public bool getKey(string strKey)
        {
            if (_listKey.IndexOf(strKey) > -1)
                return true;

            return false;
        }
        
    }
}
