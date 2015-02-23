using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace consoleXstream.Input
{
    public class KeyboardHook
    {
        readonly KeyboardHookDef _keyboardHook = new KeyboardHookDef();

        private Form1 frmMain;

        private bool _boolKeyHookActive;
        private List<string> _listKey;

        public KeyboardHook(Form1 mainForm) { frmMain = mainForm; }

        public void enableKeyboardHook()
        {
            if (_boolKeyHookActive == false)
            {
                _boolKeyHookActive = true;
                _keyboardHook.KeyDown += new KeyboardHookDef.KeyboardHookCallback(keyboardHook_KeyDown);
                _keyboardHook.KeyUp += new KeyboardHookDef.KeyboardHookCallback(keyboardHook_KeyUp);

                _listKey = new List<string>();

                _keyboardHook.Install();
            }
        }

        public void disableKeyHook()
        {
            _keyboardHook.Uninstall();
        }

        void keyboardHook_KeyUp(KeyboardHookDef.VKeys key) { setKey(key, false); }
        void keyboardHook_KeyDown(KeyboardHookDef.VKeys key) { setKey(key, true); }

        private void setKey(KeyboardHookDef.VKeys key, bool boolSet)
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
