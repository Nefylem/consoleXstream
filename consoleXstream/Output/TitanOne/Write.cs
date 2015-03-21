using System;
using System.Collections.Generic;
using consoleXstream.Config;
using consoleXstream.Input;

namespace consoleXstream.Output.TitanOne
{
    public class Write
    {
        private readonly Classes _class;

        public Write(Form1 mainForm, Configuration system, Gamepad gamepad)
        {
            _class = new Classes(this, mainForm, system, gamepad);
            _class.Create();
        }

        public Define.DevPid DevId = Define.DevPid.Any;
        public Define.ApiMethod ApiMethod = Define.ApiMethod.Single;

        private bool _isToDisconnected;

        public void SetToInterface(Define.DevPid devId)
        {
            DevId = devId;
            _class.System.Debug("titanOne.log", "[0] using DevID: " + DevId);
            _class.System.Debug("titanOne.log", "");
        }

        public void SetApiMethod(Define.ApiMethod setType)
        {
            setType = Define.ApiMethod.Multi; 
            ApiMethod = setType;
            _class.System.Debug("titanOne.log", "[0] using API: " + setType);
            _class.System.Debug("titanOne.log", "");
        }

        public void Initialize() { _class.Init.Open(); }
        public void Close() { _class.Init.Close(); }

        public void Send()
        {
            if (ApiMethod == Define.ApiMethod.Multi)
            {
                _class.MWrite.Send();
                return;
            }

            if (_class.Define.Write == null) return;

            var boolOverride = _class.FrmMain.boolIDE;

            if ((_class.Define.IsConnected() == 1) || boolOverride)
            {

                //Block gamepad rumble
                //gcapi_WriteEX(uint8_t *outpacket, uint8_t size)
                /*
                [0xFF,0x01 : 2 byte, Packet Signature]
    [Update LED Command (0,1) : 1 byte]
        [LED 1 Status : 1 byte]
        [LED 2 Status : 1 byte]
        [LED 3 Status : 1 byte]
        [LED 4 Status : 1 byte]
    [Reset LEDs Command (0,1) : 1 byte]
    [Update Rumble Command (0,1) : 1 byte]
        [Rumble 1 Value : 1 byte]
        [Rumble 2 Value : 1 byte]
        [Rumble 3 Value : 1 byte]
        [Rumble 4 Value : 1 byte]
    [Reset Rumble Command (0,1) : 1 byte]
    [Block Rumble Command (0,1) : 1 byte]
    [Turn Off Controller Command (0,1) : 1 byte]
    [Button States : 36 bytes - same format as gcapi_Write]
                _gcapi_WriteEx(output);
                 */

                _class.Define.Write(_class.Gamepad.Output);

                if (!_class.System.boolUseRumble) return;
                if (DevId == Define.DevPid.TitanOne)
                {
                    var report = new Define.GcapiReportTitanone();
                    if (_class.Define.Read(ref report) != IntPtr.Zero)
                        GamePad.SetState(PlayerIndex.One, report.Rumble[0], report.Rumble[1]);
                }
                else
                {
                    var report = new Define.GcapiReportControllermax();
                    if (_class.Define.ReadCm(ref report) != IntPtr.Zero)
                        GamePad.SetState(PlayerIndex.One, report.Rumble[0], report.Rumble[1]);
                }
            }
            else
            {
                if (_isToDisconnected) return;
                _class.System.Debug("titanOne.log", "[NOTE] TitanOne is disconnected");
                _isToDisconnected = true;
            }
        }

        public void ReloadGcmapi() { _class.MInit.Close(); }
        public void ListDevices() { _class.MDevices.List(); }
        public int CheckDevices() { return _class.MDevices.Check(); }

        public void SetTitanOneDevice(string serial) { _class.MWrite.SetDevice(serial); }
        public string GetTitanOneDevice() { return _class.MWrite.ReturnActiveDevice();  }
    }
}
