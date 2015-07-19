using System;
using consoleXstream.Input;

namespace consoleXstream.Output.CronusPlus
{
    public class Write
    {
        public Write(BaseClass baseClass) { _class = new Classes(baseClass, this); }

        private readonly Classes _class;

        public void Send(Gamepad.GamepadOutput player)
        {
            if (_class.Define.Write == null) return;

            var boolOverride = _class.BaseClass.Home.boolIDE;

            if ((_class.Define.IsConnected() != 1) && !boolOverride) return;

            _class.Define.Write(player.Output);

            var report = new Define.GcapiReportControllermax();
            if (_class.Define.Read(ref report) == IntPtr.Zero) return;

            if (_class.BaseClass.System.UseRumble)
                GamePad.SetState(player.PlayerIndex, report.Rumble[0], report.Rumble[1]);

            //TODO: Read report to see what authenticating controller is doing
        }

        public void Init()
        {
            _class.Init.Open();
        }

        public void Close()
        {
            _class.Init.Close();
        }


    }
}
