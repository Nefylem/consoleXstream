using DirectShowLib;

namespace consoleXstream.VideoCapture.Data
{
    class Graph
    {
        public IGraphBuilder CaptureGraph { get; set; }
        public IMediaControl MediaControl { get; set; }
        public IMediaEvent MediaEvent { get; set; }
        public IVideoWindow VideoWindow { get; set; }
        public IAMAnalogVideoDecoder IamAvd { get; set; }
        public IBasicVideo VideoDef { get; set; }
        public IVideoWindow IVideoWindow { get; set; }
        public IVideoWindow VideoPreview { get; set; }
        public IAMCrossbar XBar { get; set; }

    }
}
