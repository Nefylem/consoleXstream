using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class GraphCrossbar
    {
        public GraphCrossbar(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public bool createCrossbar(ref string strCrossAudioOut, ref string strCrossVideoOut, string strCaptureVideoIn, string strCaptureAudioIn, string strShortName, IBaseFilter pCaptureDevice)
        {
            int hr = 0;

            string strTempOut = "";

            _class.Var.CrossbarInput = new List<string>();

            _class.Debug.Log("");
            _class.Debug.Log("[1] Looking for crossbar " + _class.Var.DeviceId);

            IBaseFilter pCrossbar = _class.GraphFilter.Create(FilterCategory.AMKSCrossbar, strShortName, out strTempOut);
            if (strTempOut.ToLower() == "*nf*")
            {
                _class.Debug.Log("[FAIL] No crossbar found. Will not interrupt operation");
                return false;
            }
            else
            {
                hr = _class.Graph.CaptureGraph.AddFilter(pCrossbar, strTempOut);
                if (hr == 0)
                {
                    _class.Debug.Log("[OK] Create crossbar");
                    _class.Var.IsCrossbar = true;

                    _class.GraphPin.ListPin(pCrossbar);
                    strCrossAudioOut = _class.GraphPin.AssumePinOut("Audio");
                    strCrossVideoOut = _class.GraphPin.AssumePinOut("Video");
                    _class.Debug.Log("<Audio>" + strCrossAudioOut);
                    _class.Debug.Log("<Video>" + strCrossVideoOut);

                    _class.Debug.Log("");
                    _class.Debug.Log("Connect Crossbar (" + strCrossVideoOut + ") to Capture (" + strCaptureVideoIn + ")");

                    hr = _class.Graph.CaptureGraph.ConnectDirect(_class.GraphPin.GetPin(pCrossbar, strCrossVideoOut), _class.GraphPin.GetPin(pCaptureDevice, strCaptureVideoIn), null);
                    _class.Debug.Log("Crossbar Video -> " + DsError.GetErrorText(hr));

                    hr = _class.Graph.CaptureGraph.ConnectDirect(_class.GraphPin.GetPin(pCrossbar, strCrossAudioOut), _class.GraphPin.GetPin(pCaptureDevice, strCaptureAudioIn), null);
                    _class.Debug.Log("Crossbar Audio -> " + DsError.GetErrorText(hr));

                    _class.Graph.XBar = (IAMCrossbar)pCrossbar;
                    _class.Debug.Log("");

                    return true;
                }
                else
                {
                    _class.Debug.Log("[FAIL] Can't add " + strShortName + " Crossbar to graph");
                    _class.Debug.Log("-> " + DsError.GetErrorText(hr));
                    _class.Debug.Log("");

                    return false;
                }
            }
        }

        public void setCrossbar(string strSet)
        {
            if (strSet.Length > "video_".Length)
            {
                if (strSet.Substring(0, "video_".Length).ToLower() == "video_")
                    _class.Var.CrossbarVideo = strSet;

                if (strSet.Substring(0, "audio_".Length).ToLower() == "audio_")
                    _class.Var.CrossbarAudio = strSet;
            }
        }

        public void checkCrossbar()
        {
            if (_class.Graph.XBar != null)
            {
                if ((_class.Var.CrossbarVideo.Length > 0) || (_class.Var.CrossbarAudio.Length > 0))
                {
                    string strXBarChange = "";
                    if (_class.Var.CrossbarVideo.Length > 0 && _class.Var.CrossbarVideo != "none")
                    {
                        _class.Debug.Log("check cross video " + _class.Var.CrossbarVideo);

                        strXBarChange = findCrossbarSettings(_class.Var.CrossbarVideo, "");
                        _class.Debug.Log("Change crossbar command (video): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _class.Var.CrossbarVideo = "";
                    }

                    if (_class.Var.CrossbarAudio.Length > 0 && _class.Var.CrossbarAudio != "none")
                    {
                        _class.Debug.Log("check cross audio " + _class.Var.CrossbarAudio);

                        strXBarChange = findCrossbarSettings("", _class.Var.CrossbarAudio);
                        _class.Debug.Log("Change crossbar command (audio): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _class.Var.CrossbarAudio = "";
                    }
                }
            }
        }

        private string findCrossbarSettings(string strVideo, string strAudio)
        {
            _class.Debug.Log("find crossbar Settings " + strVideo + " / " + strAudio);
            string strReturn = "";
            int intType = 0;
            int intPin = 0;

            if (_class.Var.CrossbarInput.Count == 0) _class.Crossbar.Output();
            if (_class.Var.CrossbarInput.Count > 0)
            {
                for (int intCount = 0; intCount < _class.Var.CrossbarInput.Count; intCount++)
                {
                    if (strVideo.ToLower() == _class.Var.CrossbarInput[intCount].ToLower()) { intType = 0; intPin = intCount; }
                    if (strAudio.ToLower() == _class.Var.CrossbarInput[intCount].ToLower()) { intType = 1; intPin = intCount; }
                }
            }

            strReturn = intType.ToString() + ", " + intPin.ToString();
            return strReturn;
        }

        public void changeCrossbarInput(string strInput)
        {
            _class.Debug.Log("[changeCrossbarInput] " + strInput);
            int hr = 0;
            if (_class.Graph.XBar != null)
            {
                int intPinType = 0;
                int intPinID = 0;
                if (strInput.IndexOf(',') > -1)
                {
                    string[] strTemp = strInput.Split(',');
                    if (strTemp.Length == 2)
                    {
                        try
                        {
                            intPinType = Convert.ToInt32(strTemp[0].Trim());
                            intPinID = Convert.ToInt32(strTemp[1].Trim());
                        }
                        catch { }
                    }
                }
                _class.Debug.Log("intPinType:" + intPinType);
                _class.Debug.Log("intPinID:" + intPinID);

                hr = _class.Graph.XBar.Route(intPinType, intPinID);
                if (hr != 0) { _class.Debug.Log("[ERR] " + DsError.GetErrorText(hr)); }
            }
            else { _class.Debug.Log("xbar null " + strInput); }
        }

    }
}
