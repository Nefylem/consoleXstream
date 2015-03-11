using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class Pin
    {
        public Pin(Classes classes) { _class = classes; }
        private Classes _class;

        public IPin GetPin(IBaseFilter filter, string pinname)
        {
            IEnumPins epins;
            if (filter != null)
            {
                int hr = filter.EnumPins(out epins);

                IntPtr fetched = Marshal.AllocCoTaskMem(4);
                IPin[] pins = new IPin[1];
                while (epins.Next(1, pins, fetched) == 0)
                {
                    PinInfo pinfo;
                    pins[0].QueryPinInfo(out pinfo);
                    bool found = (pinfo.name == pinname);
                    DsUtils.FreePinInfo(pinfo);
                    if (found)
                        return pins[0];
                }

            }
            return null;
        }

        public void ListPin(IBaseFilter filter)
        {
            _class.Var.PinIn = new List<string>();
            _class.Var.PinOut = new List<string>();

            FilterInfo pOut;
            filter.QueryFilterInfo(out pOut);
            _class.Debug.Log("[3] listing Pins [" + pOut.achName + "]");

            try
            {
                IEnumPins epins;
                int hr = filter.EnumPins(out epins);
                if (hr < 0) { _class.Debug.Log("[0] [NG] Can't find pins"); }
                else
                {
                    IntPtr fetched = Marshal.AllocCoTaskMem(4);
                    IPin[] pins = new IPin[1];
                    while (epins.Next(1, pins, fetched) == 0)
                    {
                        PinInfo pinfo;
                        pins[0].QueryPinInfo(out pinfo);

                        if (pinfo.dir.ToString().ToLower() == "input") { _class.Var.PinIn.Add(pinfo.name); }
                        if (pinfo.dir.ToString().ToLower() == "output") { _class.Var.PinOut.Add(pinfo.name); }     

                        DsUtils.FreePinInfo(pinfo);
                    }
                }
                for (int intCount = 0; intCount < _class.Var.PinIn.Count; intCount++)
                {
                    _class.Debug.Log("-" + _class.Var.PinIn[intCount] + " (Input)");
                }

                for (int intCount = 0; intCount < _class.Var.PinOut.Count; intCount++)
                {
                    _class.Debug.Log("-" + _class.Var.PinOut[intCount] + " (Output)");
                }
            }
            catch
            {
                _class.Debug.Log("[0] [FAIL] Error listing pins");
            }
        }

        public string AssumePinIn(string strSearch)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            for (int intCount = 0; intCount < _class.Var.PinIn.Count; intCount++)
            {
                string strPinID = _class.Var.PinIn[intCount].ToLower();

                if (strPinID.IndexOf(strSearch) > -1) { strReturn = _class.Var.PinIn[intCount]; }
            }

            return strReturn;
        }

        public string AssumePinIn(string strSearch, string strType)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            for (int intCount = 0; intCount < _class.Var.PinIn.Count; intCount++)
            {
                string strPinID = _class.Var.PinIn[intCount].ToLower();
                if (strPinID.IndexOf(strSearch) > -1 || strPinID.IndexOf(strType) > -1)
                {
                    string strCheck = strType.ToLower();

                    if (strCheck == "vídeo")
                        strCheck = "video";

                    if (strCheck == "video")
                    {
                        if (strPinID.IndexOf("audio") == -1)
                            strReturn = _class.Var.PinOut[intCount];
                    }
                    else
                    {
                        if (strPinID.IndexOf("video") == -1)
                            strReturn = _class.Var.PinOut[intCount];
                    }
                }
            }

            return strReturn;
        }

        public string AssumePinOut(string strSearch)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            for (int intCount = 0; intCount < _class.Var.PinOut.Count; intCount++)
            {
                string strPinID = _class.Var.PinOut[intCount].ToLower();

                if (strPinID.IndexOf(strSearch) > -1) { strReturn = _class.Var.PinOut[intCount]; }
            }

            return strReturn;
        }

        public string AssumePinOut(string strSearch, string strType)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            strType = strType.ToLower();
            for (int intCount = 0; intCount < _class.Var.PinOut.Count; intCount++)
            {
                string strPinID = _class.Var.PinOut[intCount].ToLower();

                if (strPinID.IndexOf(strSearch) > -1 || strPinID.IndexOf(strType) > -1)
                {
                    string strCheck = strType.ToLower();

                    if (strCheck == "vídeo")
                        strCheck = "video";

                    if (strCheck == "video")
                    {
                        if (strPinID.IndexOf("audio") == -1)
                            strReturn = _class.Var.PinOut[intCount];
                    }
                    else
                    {
                        if (strPinID.IndexOf("video") == -1)
                            strReturn = _class.Var.PinOut[intCount];
                    }
                }
            }

            return strReturn;
        }


    }
}
