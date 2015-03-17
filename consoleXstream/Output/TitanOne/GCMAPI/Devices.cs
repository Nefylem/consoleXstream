using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Devices
    {
        public Devices(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private int _timeOut = 5000;
        private int _deviceCount;

        public void List()
        {
            _class.System.Debug("TitanOne.log", "listing devices");
            _class.System.ChangeTitanOne();

            _class.Write.SetApiMethod(TitanOne.Define.ApiMethod.Multi);
            _class.Write.SetToInterface(TitanOne.Define.DevPid.TitanOne);

            if (_class.MDefine.GcmapiConnect == null)
                _class.MInit.Open();

            _class.System.Debug("TitanOne.Log", "Load: " + _class.MDefine.GcmapiLoad().ToString());
            _class.System.Debug("TitanOne.Log", "Device: " + _class.Write.DevId.ToString());

            if (_class.MDefine.GcmapiConnect != null)
                _deviceCount = _class.MDefine.GcmapiConnect((ushort)_class.Write.DevId);
        }

        public int Check()
        {
            var result = _deviceCount;
            for (var count = 0; count < _deviceCount; count++)
            {
                if (_class.MDefine.GcmapiIsConnected(count) == 0) 
                    result = 0;
            }

            _timeOut--;
            if (_timeOut == 0)
                return -1;

            if (result != 0)
                ReadSerials();

            return result;
        }

        private void ReadSerials()
        {
            for (var count = 0; count < _deviceCount; count++)
            {
                var serial = new byte[20];
                var ret = _class.MDefine.GcmapiGetSerialNumber(count);
                Marshal.Copy(ret, serial, 0, 20);
                var disp = "";

                for (var counter = 0; counter < 20; counter++)
                {
                    disp += String.Format("{0:X2}", serial[counter]);
                }

                //Load config on each of these devices
                _class.MWrite.AddDevice(disp);
                _class.FrmMain.ListToDevices.Add(disp);
            }
        }
    }
}
