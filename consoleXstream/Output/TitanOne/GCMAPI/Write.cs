using System;
using System.Collections.Generic;
using consoleXstream.Input;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Write
    {
        public Write(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private readonly List<string> _listDevices = new List<string>();
        private int _activeDevice;
        private bool _isConnected;

        public void SetDevice(string device)
        {
            int index = _listDevices.IndexOf(device);

            if (index > -1)
            {
                _activeDevice = index;
                _class.System.SetTitanOneDevice(device);
            }
            else
            {
                if (!_class.System.DisableTitanOneRetry)
                {
                    _class.MDevices.List();
                    _class.FrmMain.RetrySetTitanOne = device;
                    _class.FrmMain.RetryTimeOut = 5000;                    
                }
                _class.System.DisableTitanOneRetry = true;
            }
        }

        public void AddDevice(string serial)
        {
            if (_listDevices.IndexOf(serial) == -1)
                _listDevices.Add(serial);
        }

        public string ReturnActiveDevice()
        {
            return _listDevices.Count > 0 ? _listDevices[_activeDevice] : "";
        }

        public void Send()
        {
            //_class.System.Debug("TOCheck.log", _activeDevice + " / " + _listDevices[_activeDevice]);
            if (!_isConnected)
            {
                if (_class.MDefine.GcmapiConnect != null)
                    _class.MDefine.GcmapiConnect((ushort) _class.Write.DevId);
                _isConnected = true;
            }

            if (_class.MDefine.GcmapiIsConnected(_activeDevice) == 1)
            {
                _class.MDefine.GcmapiWrite(_activeDevice, _class.Gamepad.Output);

                if (!_class.System.boolUseRumble) return;

                var report = new Define.GcmapiReport();

                if (_class.MDefine.GcmapiRead(_activeDevice, ref report) != IntPtr.Zero)
                    GamePad.SetState(PlayerIndex.One, report.Rumble[0], report.Rumble[1]);
            }
        }
    }
}
