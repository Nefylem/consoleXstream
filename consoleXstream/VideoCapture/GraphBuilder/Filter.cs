using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.VideoCapture.Data;
using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class Filter
    {
        public Filter(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public class ActiveFilterList
        {
            public IBaseFilter filter;
            public string name;
            public List<string> PinIn = new List<string>();
            public List<string> PinOut = new List<string>();
            public string UsePinIn;
            public string UsePinOut;
        }

        private List<ActiveFilterList> filterId;
        private List<string> filterName;
 
        public IBaseFilter Set(Guid category, string strTitle, out string strTempOut)
        {
            IBaseFilter pFilter = null;

            string strCategory = "";
            int intHR = 0;
            strTempOut = "";

            if (category == FilterCategory.VideoInputDevice) { strCategory = "Video Capture Device"; }
            if (category == FilterCategory.AudioRendererCategory) { strCategory = "Audio Render Device"; }
            _class.Debug.Log("");
            _class.Debug.Log("[2]  Create " + strTitle + " (" + strCategory + ")");

            pFilter = Create(category, strTitle, out strTempOut);
            intHR = _class.Graph.CaptureGraph.AddFilter(pFilter, strTempOut);
            if (intHR == 0)
            {
                _class.Debug.Log("[OK] Added " + strTitle + " (" + strCategory + ") to graph");
            }
            else
            {
                _class.Debug.Log("[FAIL] Can't add " + strTitle + " (" + strCategory + ") to graph");
                _class.Debug.Log("-> " + DsError.GetErrorText(intHR));
            }

            return pFilter;
        }

        public IBaseFilter Create(Guid category, string strName, out string strTitle)
        {
            _class.Debug.Log("[3] Create filter: name>" + strName);
            IBaseFilter filter = null;
            strTitle = "*NF*";
            strName = strName.ToLower();
            int hr = 0;

            int intID = _class.Var.DeviceId;
            if (strName.IndexOf('(') > -1)
            {
                string strID = strName.Substring(strName.IndexOf('(') + 1);
                if (strID.IndexOf(')') > -1) { strID = strID.Substring(0, strID.IndexOf(')')); }
                strName = strName.Substring(0, strName.IndexOf('(')).Trim();

                _class.Debug.Log("FilterName: " + strName);
                _class.Debug.Log("DevID: " + strID);
                try
                {
                    intID = Convert.ToInt32(strID);
                }
                catch { }
                _class.Debug.Log("Confirm DevID: " + intID.ToString());
            }

            int intDevID = 0;

            DsDevice[] devices = DsDevice.GetDevicesOfCat(category);
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(category))
            {
                string strDevice = device.Name.ToLower();
                _class.Debug.Log("device>" + strDevice);
                if (strDevice.IndexOf(strName) > -1 || strDevice.ToLower() == "oem crossbar")
                {
                    bool boolCreate = true;
                    _class.Debug.Log("TargetID=" + intID.ToString() + " [] + DevID=" + intDevID.ToString());

                    if (intDevID == intID)
                    {
                        boolCreate = true;
                        _class.Var.DeviceId = intID;
                    }
                    else
                        boolCreate = false;

                    if (boolCreate == true)
                    {
                        _class.Debug.Log("SetDeviceID (" + intDevID.ToString() + ")");
                        strTitle = device.Name;
                        IBindCtx bindCtx = null;
                        try
                        {
                            hr = Variables.CreateBindCtx(0, out bindCtx);
                            DsError.ThrowExceptionForHR(hr);
                            Guid guid = typeof(IBaseFilter).GUID;
                            object obj;
                            device.Mon.BindToObject(bindCtx, null, ref guid, out obj);
                            filter = (IBaseFilter)obj;
                        }
                        finally
                        {
                            if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);
                        }
                    }
                    else { _class.Debug.Log(intDevID.ToString() + " skipped"); }
                    intDevID++;
                }
            }
            return filter;
        }

        public void CreateFilterList()
        {
            filterId = new List<ActiveFilterList>();
            filterName = new List<string>();
        }
    }
}
