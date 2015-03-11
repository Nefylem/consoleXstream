using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.VideoCapture.Sampling;
using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class SampleGrabber
    {
        public SampleGrabber(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void createSampleGrabber(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll

            int hr = 0;
            _class.Debug.Log("");
            _class.Debug.Log("Creating SampleGrabber");

            //add SampleGrabber
            IBaseFilter pSampleGrabber = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber));
            hr = _class.Graph.CaptureGraph.AddFilter(pSampleGrabber, "SampleGrabber");
            _class.Debug.Log("-> " + DsError.GetErrorText(hr));

            _class.GraphPin.ListPin(pSampleGrabber);
            string strSampleIn = _class.GraphPin.AssumePinIn("Input");
            string strSampleOut = _class.GraphPin.AssumePinOut("Output");

            _class.Debug.Log("Set samplegrabber resolution feed");
            if (_class.Resolution.List.Count > 0)
            {

                hr = ((ISampleGrabber)pSampleGrabber).SetMediaType(_class.Resolution.Type[_class.Var.VideoResolutionIndex]);
                _class.Debug.Log("-> " + DsError.GetErrorText(hr));
            }
            else
                _class.Debug.Log("[ERR] failure in video resolution list");

            _class.Debug.Log("");
            _class.Debug.Log("***   Connect " + strDevice + " (" + strPinOut + ") to SampleGrabber (" + strSampleIn + ")");
            //hr = CaptureGraph.ConnectDirect(_class.GraphPin.GetPin(pRen, strPinOut), _class.GraphPin.GetPin(pSampleGrabber, strPreviewIn), null);
            hr = _class.Graph.CaptureGraph.ConnectDirect(_class.GraphPin.GetPin(pRen, "Capture"), _class.GraphPin.GetPin(pSampleGrabber, "Input"), null);
            if (hr == 0)
            {
                var cb = new SampleGrabberCallback();
                cb.GetForm1Handle(_class.FrmMain);

                var sampleGrabber = (ISampleGrabber)pSampleGrabber;
                sampleGrabber.SetCallback(cb, 1);

                _class.Debug.Log("[OK] Connected " + strDevice + " to SampleGrabber");
                strDevice = "Sample Grabber";
                pRen = pSampleGrabber;
                strPinOut = strSampleOut;
            }
            else
            {
                _class.Debug.Log("[NG] Cant connect SampleGrabber to video Capture feed. Attempting to continue.");
                _class.Debug.Log("-> " + DsError.GetErrorText(hr));
            }
        }

    }
}
