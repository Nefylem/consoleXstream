using System.Collections.Generic;

namespace consoleXstream.Input
{
    public class KeyboardHook
    {
        public KeyboardHook(BaseClass baseClass) { _class = baseClass; }
        private BaseClass _class;

        readonly KeyboardHookDef _keyboardHook = new KeyboardHookDef();

        private bool _boolKeyHookActive;
        private List<string> _listKey;

        public void EnableKeyboardHook()
        {
            if (_boolKeyHookActive) return;
            _boolKeyHookActive = true;
            _keyboardHook.KeyDown += keyboardHook_KeyDown;
            _keyboardHook.KeyUp += keyboardHook_KeyUp;

            _listKey = new List<string>();

            _keyboardHook.Install();
        }

        public void DisableKeyHook()
        {
            _keyboardHook.Uninstall();
        }

        void keyboardHook_KeyUp(KeyboardHookDef.VKeys key) { SetKey(key, false); }
        void keyboardHook_KeyDown(KeyboardHookDef.VKeys key) { SetKey(key, true); }

        private void SetKey(KeyboardHookDef.VKeys key, bool boolSet)
        {
            var strKey = key.ToString();

            var intIndex = _listKey.IndexOf(strKey);

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

        public bool GetKey(string strKey)
        {
            return _listKey.IndexOf(strKey) > -1;
        }
    }
}
