using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class AviRender
    {
        public AviRender(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Create(ref string strAVIin, ref string strAVIout, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            var hr = 0;
            _class.Debug.Log("");
            _class.Debug.Log("Creating AVI renderer");
            var pAviDecompressor = (IBaseFilter) new AVIDec();
            hr = _class.Graph.CaptureGraph.AddFilter(pAviDecompressor, "AVI Decompressor");
            _class.Debug.Log("-> " + DsError.GetErrorText(hr));

            _class.GraphPin.ListPin(pAviDecompressor);
            strAVIin = _class.GraphPin.AssumePinIn("XForm");
            strAVIout = _class.GraphPin.AssumePinOut("XForm");

            _class.Debug.Log("");
            _class.Debug.Log("***   Connect " + strDevice + " (" + strPinOut + ") to AVI Decompressor (" + strAVIin + ")");
            hr = _class.Graph.CaptureGraph.ConnectDirect(_class.GraphPin.GetPin(pRen, strPinOut), _class.GraphPin.GetPin(pAviDecompressor, strAVIin), null);
            if (hr == 0)
            {
                _class.Debug.Log("[OK] Connected " + strDevice + " to AVI Decompressor");
                pRen = pAviDecompressor;
                strDevice = "AVI Decompressor";
                strPinOut = strAVIout;
            }
            else
            {
                _class.Debug.Log("[FAIL] Can't connected " + strDevice + " to AVI Decompressor. May interrupt operation");

            }
        }

    }
}
