using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Write
    {
        public Write(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private readonly List<string> _listDevices = new List<string>();
        private string _serialToSet;
        private int _activeDevice;

        public void SetDevice(string device)
        {
            int index = _listDevices.IndexOf(device);
            if (index > -1)
            {
                _activeDevice = index;
                _class.System.SetTitanOneDevice(device);
                _class.System.Debug("titanone.log", "Set #" + _activeDevice + " device " + device);
            }
            else
            {
                if (!_class.System.DisableTitanOneRetry)
                {
                    _serialToSet = device;
                    _class.MDevices.List();
                    _class.FrmMain.RetrySetTitanOne = device;
                    _class.FrmMain.RetryTimeOut = 5000;                    
                }
                _class.System.DisableTitanOneRetry = false;

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
            if (_class.MDefine.GcmapiConnect != null)
                _class.MDefine.GcmapiConnect((ushort)_class.Write.DevId);

            if (_class.MDefine.GcmapiIsConnected(_activeDevice) == 1)
            {
                _class.MDefine.GcmapiWrite(_activeDevice, _class.Gamepad.Output);
            }
        }
    }
}
