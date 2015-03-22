using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Devices
    {
        public Devices(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private int _timeOut = 5000;
        private int _deviceCount;
        private int _deviceLoaded;

        public void List()
        {
            //if (_class.System.)
            //_class.System.Debug("listAll.log", "MWrite.listing devices");
            _class.System.ChangeTitanOne(true);

            _class.Write.SetApiMethod(TitanOne.Define.ApiMethod.Multi);
            _class.Write.SetToInterface(TitanOne.Define.DevPid.TitanOne);

            if (_class.MDefine.GcmapiConnect == null)
                _class.MInit.Open();

            if (_deviceLoaded == 0)
                _deviceLoaded = _class.MDefine.GcmapiLoad();

            //_class.System.Debug("listAll.log", "Load: " + _class.MDefine.GcmapiLoad());
            //_class.System.Debug("listAll.log", "Device: " + _class.Write.DevId);

            if (_class.MDefine.GcmapiConnect != null)
            {
                //_class.System.Debug("listAll.log", "GcmapiConnect " + _class.Write.DevId);
                _deviceCount = _class.MDefine.GcmapiConnect((ushort) _class.Write.DevId);
            }
            else
            {
                //_class.System.Debug("listAll.log", "gcmapiconnect == null");
            }
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
            {
                //_class.System.Debug("listAll.log", "result : " + result + " now reading serials");
                ReadSerials();
            }

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
                
                if (_class.FrmMain.ListToDevices == null)
                    _class.FrmMain.ListToDevices = new List<string>();

                _class.FrmMain.ListToDevices.Add(disp);
            }

            if (_class.FrmMain.RetrySetTitanOne == null) return;
            if (_class.FrmMain.RetrySetTitanOne.Length <= 0) return;
            _class.MWrite.SetDevice(_class.FrmMain.RetrySetTitanOne);
            _class.FrmMain.RetrySetTitanOne = "";
        }
    }
}
