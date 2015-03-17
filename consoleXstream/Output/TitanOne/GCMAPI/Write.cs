using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Write
    {
        public Write(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private int _activeDevice;
        private readonly List<string> _listDevices = new List<string>(); 

        public void SetDevice(string device)
        {
            int index = _listDevices.IndexOf(device);
            if (index > -1)
            {
                _activeDevice = index;
                _class.System.SetTitanOneDevice(device);
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

            if (_class.MDefine.GcmapiIsConnected(0) == 1)
            {
                _class.MDefine.GcmapiWrite(0, _class.Gamepad.Output);
            }
        }
    }
}
