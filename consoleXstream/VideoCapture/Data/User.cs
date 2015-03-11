using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.VideoCapture.Data
{
    public class User
    {
        public User(Classes getclass) { _class = getclass; }
        private readonly Classes _class;

        public void LoadSettings()
        {
            _class.Var.UseCrossbar = _class.System.checkUserSetting("Crossbar") == "true";
            _class.Var.CreateSmartTee = _class.System.checkUserSetting("Preview") == "true";
            _class.Var.CreateAviRender = _class.System.checkUserSetting("AVIRender") == "true";

            _class.Var.UseSampleGrabber = true;

            _class.Var.SetVideoPin = _class.System.checkUserSetting("crossbarVideoPin");
            _class.Var.SetAudioPin = _class.System.checkUserSetting("crossbarAudioPin");

            _class.Capture.Set(_class.System.checkUserSetting("VideoCaptureDevice"));
            _class.Debug.Log(_class.Var.CreateSmartTee ? "Using smartTee" : "SmartTee disabled");
            _class.Debug.Log(_class.Var.CreateAviRender ? "Using AVI Renderer" : "AVI Rendered disabled");
        }

    }
}
