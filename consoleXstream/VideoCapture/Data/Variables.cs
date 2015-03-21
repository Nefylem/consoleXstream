using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace consoleXstream.VideoCapture.Data
{
    public class Variables
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
        public bool IsCrossbar { get; set; }
        public bool IsChangedDisplayResolution { get; set; }
        public bool IsBuildingGraph { get; set; }
        public bool IsVideoFail { get; set; }

        public string SetVideoPin { get; set; }
        public string SetAudioPin { get; set; }
        public string CrossbarVideo { get; set; }
        public string CrossbarAudio { get; set; }
        public string VideoDevice { get; set; }
        public string AudioDevice { get; set; }
        public string CurrentResByName { get; set; }

        public int DeviceId { get; set; }
        public int VideoResolutionIndex { get; set; }
        public int SetResolution { get; set; }
        public int CurrentResolution { get; set; }


        public List<string> CrossbarInput { get; set; }
        public List<string> VideoCaptureDevice { get; set; }
        public List<string> VideoCaptureName { get; set; }
        public List<string> PinIn { get; set; }
        public List<string> PinOut { get; set; }



    }
}
