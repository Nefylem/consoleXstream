using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class Close
    {
        public Close(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void CloseGraph()
        {
            _class.Debug.Log("[0]");
            _class.Debug.Log("[TRY] Gracefully closing graph");

            if (_class.Graph.MediaControl != null) _class.Graph.MediaControl.StopWhenReady();

            _class.Graph.CaptureGraph = null;
            _class.Graph.MediaControl = null;
            _class.Graph.MediaEvent = null;
            _class.Graph.VideoWindow = null;
            _class.Graph.VideoDef = null;
            //_class.Graph.IVideoWindow = null;
            _class.Graph.IamAvd = null;
            _class.Graph.XBar = null;

            _class.Debug.Log("[OK] close ok");
        }

    }
}
