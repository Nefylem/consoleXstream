using DirectShowLib;

namespace consoleXstream.VideoCapture.Data
{
    public class Graph
    {
        public IGraphBuilder CaptureGraph { get; set; }
        public IMediaControl MediaControl { get; set; }
        public IMediaEvent MediaEvent { get; set; }
        public IAMAnalogVideoDecoder IamAvd { get; set; }
        public IBasicVideo VideoDef { get; set; }
        public IVideoWindow VideoPreview { get; set; }
        public IAMCrossbar XBar { get; set; }

        public IVideoWindow VideoWindow { get; set; }
        public IVideoWindow VideoWindowVr { get; set; }
    }
}
