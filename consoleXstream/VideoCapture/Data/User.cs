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
            _class.Var.UseCrossbar = _class.System.CheckData("Crossbar") == "true";
            _class.Var.CreateSmartTee = _class.System.CheckData("Preview") == "true";
            _class.Var.CreateAviRender = _class.System.CheckData("AVIRender") == "true";

            _class.Var.UseSampleGrabber = true;

            _class.Var.SetVideoPin = _class.System.CheckData("crossbarVideoPin");
            _class.Var.SetAudioPin = _class.System.CheckData("crossbarAudioPin");

            _class.Capture.Set(_class.System.CheckData("VideoCaptureDevice"));
            _class.Debug.Log(_class.Var.CreateSmartTee ? "Using smartTee" : "SmartTee disabled");
            _class.Debug.Log(_class.Var.CreateAviRender ? "Using AVI Renderer" : "AVI Rendered disabled");
        }

    }
}
