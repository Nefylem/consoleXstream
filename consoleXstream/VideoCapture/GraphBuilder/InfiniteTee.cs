using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class InfiniteTee
    {
        public InfiniteTee(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void CreateInfiniteTee(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            int hr = 0;
            _class.Debug.Log("");
            _class.Debug.Log("Creating InfiniteTee Preview Filter");

            IBaseFilter pSmartTee2 = (IBaseFilter)new InfTee();
            hr = _class.Graph.CaptureGraph.AddFilter(pSmartTee2, "Infinite Pin Tee Filter");
            _class.Debug.Log(DsError.GetErrorText(hr));
            _class.Debug.Log("");

            _class.GraphPin.ListPin(pSmartTee2);
            strPreviewIn = _class.GraphPin.AssumePinIn("Input");
            strPreviewOut = _class.GraphPin.AssumePinOut("Preview");

            _class.Debug.Log("");
            _class.Debug.Log("***   Connect " + strDevice + " (" + strPinOut + ") to InfiniteTee Preview Filter (" + strPreviewIn + ")");
            hr = _class.Graph.CaptureGraph.ConnectDirect(_class.GraphPin.GetPin(pRen, strPinOut), _class.GraphPin.GetPin(pSmartTee2, strPreviewIn), null);
            if (hr == 0)
            {
                _class.Debug.Log("[OK] Connected " + strDevice + " to InfiniteTee Preview Filter");
                strDevice = "SmartTee Preview Filter";
                pRen = pSmartTee2;
                strPinOut = strPreviewOut;
            }
            else
            {
                _class.Debug.Log("[NG] cant Connect " + strDevice + " to Preview Filter. Attempting to continue without preview");
                _class.Debug.Log("-> " + DsError.GetErrorText(hr));
            }
        }

    }
}
