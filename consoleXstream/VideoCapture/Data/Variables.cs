using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace consoleXstream.VideoCapture.Data
{
    class Variables
    {
        public Variables(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        public bool ShowPreviewWindow;
        public bool UseCrossbar { get; set; }
        public bool CreateSmartTee { get; set; }
        public bool CreateAviRender { get; set; }
        public bool UseSampleGrabber { get; set; }
        public bool IsInitializeGraph { get; set; }
        public bool IsRestartGraph { get; set; }

        public string SetVideoPin { get; set; }
        public string SetAudioPin { get; set; }

        public List<string> CrossbarInput { get; set; }

        public List<string> VideoCaptureDevice { get; set; }
        public List<string> VideoCaptureName { get; set; } 


    }
}
