using System;
using System.Collections.Generic;
using DirectShowLib;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing;
using consoleXstream.Config;
using consoleXstream.VideoCapture.Data;
using consoleXstream.VideoCapture.Sampling;

namespace consoleXstream.VideoCapture
{
    public class VideoCapture
    {
        private readonly Classes _class;


        public bool boolActiveVideo { get; private set; }
        public bool boolCrossbar { get; private set; }
        public string strVideoCaptureDevice { get; private set; }
        public string strAudioPlaybackDevice { get; private set; }

        public List<string> listVideoCapture { get; private set; }

        public List<string> listVideoCaptureName { get; set; } 

        public IMediaEvent MediaEvent
        {
            get { return _class.Graph.MediaEvent; }
            set { _class.Graph.MediaEvent = value; }
        }

        public IAMCrossbar _xBar { get; set; }

        private List<string> _listPinIn;
        private List<string> _listPinOut;

        private string _strCurrentResolution;
        private string _strCrossVideo;
        private string _strCrossAudio;

        private int _intRestartGraph = 0;
        private int _intDeviceID = 0;

        private int _intVideoResolution = 0;
        private int _intCurrentVideoResolution = 0;
        private int _intSetResolution = 0;

        private bool _boolBuildingGraph;
        private bool _boolVideoFail;
        private bool _boolRerunGraph;
        
        private bool _IsChangedDisplayResolution;

        public VideoCapture(Form1 mainForm, Configuration System)
        {
            _class = new Classes(this, mainForm, System);
            _class.DeclareClasses();
        }

        public void InitialzeCapture()
        {
            _strCrossAudio = "none";
            _strCrossVideo = "none";

            //Caches build information
            _class.Capture.Find();
            _class.Audio.Find();

            _class.User.LoadSettings();
            _class.Resolution.Find();

            _class.Var.IsInitializeGraph = true;
            _class.Var.IsRestartGraph = true;
        }

        public void LoadUserSettings() { _class.User.LoadSettings(); }

        #region DirectShow Graph
        public void runGraph()
        {
            _class.Debug.Log("[0] Build capture graph");
            if (_class.Capture.CurrentDevice > -1 && _class.Capture.CurrentDevice < _class.Capture.Display.Count)
            {
                int hr = 0;

                _boolBuildingGraph = true;
                _class.Debug.Log("Using : " + _class.Capture.Display[_class.Capture.CurrentDevice]);
                _class.Debug.Log("");

                if (_class.Graph.MediaControl != null) _class.Graph.MediaControl.StopWhenReady();
                if (_class.Resolution.Type.Count == 0) { _class.Resolution.Find();  }

                _class.Graph.CaptureGraph = null;
                _class.Graph.MediaControl = null;
                _class.Graph.MediaEvent = null;
                _class.Graph.VideoWindow = null;
                _class.Graph.VideoDef = null;
                _class.Graph.IVideoWindow = null;
                _class.Graph.XBar = null;

                _class.Graph.CaptureGraph = (IGraphBuilder)new FilterGraph();

                if (buildGraph())
                {
                    if (!_class.Var.ShowPreviewWindow)
                        setupVideoWindow();
                    else
                        setupPreviewWindow();

                    _class.Graph.MediaControl = (IMediaControl)_class.Graph.CaptureGraph;
                    _class.Graph.MediaEvent = (IMediaEvent)_class.Graph.CaptureGraph;

                    _class.Debug.Log("");
                    _class.Debug.Log("Run compiled graph");
                    hr = _class.Graph.MediaControl.Run();
                    _class.Debug.Log("[2] " + DsError.GetErrorText(hr));

                    //TODO: Check for major / minor errors before declaring video run
                    boolActiveVideo = true;
                }

                _boolBuildingGraph = false;

                if (_class.Graph.XBar != null)
                    if (_class.Var.CrossbarInput.Count == 0)
                        _class.Crossbar.Output();

                if (_class.Var.IsRestartGraph)
                    _intRestartGraph = 3;
            }
            else
                _class.Debug.Log("[ERR] Unknown capture device");
        }

        private bool buildGraph()
        {
            int hr = 0;

            if (_class.Capture.CurrentDevice > -1 && _class.Capture.CurrentDevice < _class.Capture.Device.Count)
            {
                string strVideoDevice = _class.Capture.Display[_class.Capture.CurrentDevice];
                string strShortName = findCaptureName(strVideoDevice);

                if (_intVideoResolution > _class.Resolution.List.Count)
                    _intVideoResolution = 0;

                if (_class.Audio.Output > _class.Audio.Devices.Count)
                {
                    _class.Audio.Output = -1;
                    _class.Audio.Find();
                }

                _class.Debug.Log("[2] VCD: " + strVideoDevice);
                _class.Debug.Log("[2] VCDID: " + strShortName);
                _class.Debug.Log("[2] RES: " + _class.Resolution.List[_intVideoResolution]);
                _class.Debug.Log("[2] AOD: " + _class.Audio.Devices[_class.Audio.Output]);
                
                strVideoCaptureDevice = strVideoDevice;
                strAudioPlaybackDevice = _class.Audio.Devices[_class.Audio.Output];

                //Filter lists definitions
                string strCaptureVideoOut = "";
                string strCaptureAudioOut = "";
                string strCaptureVideoIn = "";
                string strCaptureAudioIn = "";
                string strCrossVideoOut = "";
                string strCrossAudioOut = "";
                string strAVIin = "";
                string strAVIout = "";
                string strVideoIn = "";
                string strAudioIn = "";
                string strPreviewIn = "";
                string strPreviewOut = "";
                string strTempOut;

                //graph builder
                _class.Debug.Log("");
                _class.Debug.Log("[0] Create new graph");
                ICaptureGraphBuilder2 pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                hr = pBuilder.SetFiltergraph(_class.Graph.CaptureGraph);
                _class.Debug.Log("[2] [OK] " + DsError.GetErrorText(hr));
                _class.Debug.Log("");

                _class.Graph.VideoWindow = (IVideoWindow)_class.Graph.CaptureGraph;            //Open the window

                //Primary Capture Device
                IBaseFilter pCaptureDevice = setFilter(FilterCategory.VideoInputDevice, strVideoDevice, out strTempOut);
                _class.Debug.Log("");
                if (pCaptureDevice == null)
                {
                    _class.Debug.Log("[ERR] Cant create capture device. Graph cannot continue");
                    return false;
                }

                //Video capture in/output
                listPin(pCaptureDevice);
                strCaptureVideoOut = assumePinOut("Capture", "Video");
                if (strCaptureVideoOut.Length == 0) strCaptureVideoOut = assumePinOut("Capturar", "vídeo");     //Alias for Deen0x spanish card
                strCaptureAudioOut = assumePinOut("Audio");

                strCaptureVideoIn = assumePinIn("Video");
                if (strCaptureVideoIn.Length == 0) strCaptureVideoIn = assumePinIn("Capturar", "vídeo");

                strCaptureAudioIn = assumePinIn("Audio");

                _class.Debug.Log("[0]");
                _class.Debug.Log("<Video Out>" + strCaptureVideoOut);
                _class.Debug.Log("<Audio Out>" + strCaptureAudioOut);
                _class.Debug.Log("<Video In>" + strCaptureVideoIn);
                _class.Debug.Log("<Audio In>" + strCaptureAudioIn);
                _class.Debug.Log("");

                _class.Graph.IamAvd = pCaptureDevice as IAMAnalogVideoDecoder;

                //Create user crossbar if needed
                if (_class.Var.UseCrossbar == true)
                    if (createCrossbar(ref strCrossAudioOut, ref strCrossVideoOut, strCaptureVideoIn, strCaptureAudioIn, strShortName, pCaptureDevice))
                        checkCrossbar();

                _class.Debug.Log("");

                //Set resolution
                _class.Debug.Log("[0] Checking capture resolution");
                if (_intVideoResolution == 0)       
                    lookupAutoResolution();

                if (_intVideoResolution > 0)
                    setVideoResolution(pCaptureDevice, strCaptureVideoOut);
                else
                    _class.Debug.Log("[0] [WARN] Cant find capture resolution - no input or unknown resolution type");

                IBaseFilter pRen = pCaptureDevice;
                string strPinOut = strCaptureVideoOut;
                string strDevice = strVideoDevice;

                if (_class.Var.UseSampleGrabber)
                    createSampleGrabber(ref strPreviewIn, ref strPreviewOut, ref strDevice, ref strPinOut, ref pRen);

                if (_class.Var.CreateSmartTee)
                    createSmartTee(ref strPreviewIn, ref strPreviewOut, ref strDevice, ref strPinOut, ref pRen);

                if (_class.Var.CreateAviRender)
                    createAVIRender(ref strAVIin, ref strAVIout, ref strDevice, ref strPinOut, ref pRen);

                //Video renderer
                _class.Debug.Log("");
                _class.Debug.Log("[0]***   Create Video Renderer");
                Guid CLSID_ActiveVideo = new Guid("{B87BEB7B-8D29-423F-AE4D-6582C10175AC}");

                IBaseFilter pVideoRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ActiveVideo));
                hr = _class.Graph.CaptureGraph.AddFilter(pVideoRenderer, "Video Renderer");
                if (hr == 0) { _class.Debug.Log("[1] [OK] Created video renderer"); }
                else
                {
                    _class.Debug.Log("[1] [FAIL] Cant create video renderer");
                    _class.Debug.Log("-> " + DsError.GetErrorText(hr));
                }

                _class.Debug.Log("");
                _class.Debug.Log("***   Listing Video Renderer pins");
                listPin(pVideoRenderer);
                strVideoIn = assumePinIn("Input");
                _class.Debug.Log("<Video>" + strVideoIn);
                _class.Debug.Log("");

                _class.Debug.Log("***   Connect AVI Decompressor (" + strPinOut + ") to Video Renderer (" + strVideoIn + ")");
                hr = _class.Graph.CaptureGraph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pVideoRenderer, strVideoIn), null);
                if (hr == 0) { _class.Debug.Log("[OK] Connected AVI to video renderer"); }
                else
                {
                    _class.Debug.Log("[FAIL] Can't connect AVI to video renderer");
                    _class.Debug.Log("-> " + DsError.GetErrorText(hr));
                }

                _class.Graph.VideoDef = pVideoRenderer as IBasicVideo;
                _class.Graph.IVideoWindow = pVideoRenderer as IVideoWindow;
                
                //Audio device
                if (_class.Audio.Output > -1 && _class.Audio.Output < _class.Audio.Devices.Count)
                {
                    _intDeviceID = 0;               //Dont need multiple devices, set back to 0

                    _class.Debug.Log("[0]");
                    _class.Debug.Log("***   Create " + _class.Audio.Devices[_class.Audio.Output] + " audio device");
                    IBaseFilter pAudio = null;

                    pAudio = setFilter(FilterCategory.AudioRendererCategory, _class.Audio.Devices[_class.Audio.Output], out strTempOut);
                    hr = _class.Graph.CaptureGraph.AddFilter(pAudio, "Audio Device");
                    _class.Debug.Log("-> " + DsError.GetErrorText(hr));

                    if (pAudio != null)
                    {
                        _class.Debug.Log("[1]");
                        _class.Debug.Log("***   Listing " + _class.Audio.Devices[_class.Audio.Output] + " pins");

                        listPin(pAudio);
                        strAudioIn = assumePinIn("Audio");
                        _class.Debug.Log("<Audio>" + strAudioIn);
                        _class.Debug.Log("");

                        //connect Capture Device and Audio Device
                        _class.Debug.Log("***   Connect " + strVideoDevice + " (" + strCaptureAudioOut + ") to " + _class.Audio.Devices[_class.Audio.Output] + " [Audio] (" + strAudioIn + ")");
                        hr = _class.Graph.CaptureGraph.ConnectDirect(GetPin(pCaptureDevice, strCaptureAudioOut), GetPin(pAudio, strAudioIn), null);
                        _class.Debug.Log("-> " + DsError.GetErrorText(hr));
                    }
                }
            }

            return true;
        }

        /*
         * Looks for a unique identifier (eg Avermedia U3 extremecap will return Avermedia)
         * If there's more than one avermedia card, will look for the next available unique name
         * Finall, if there's two avermedia u3 on there, will still only return Avermedia
         */
        private string findCaptureName(string device)
        {
            string output = "";
            for (int count = 0; count < device.Length; count++)
            {
                if (device[count] == ' ')
                {
                    if (findCaptureResults(output) == 1)
                        break;
                    else 
                        output += device[count];
                }
                else output += device[count];
            }

            return output;
        }

        private int findCaptureResults(string name)
        {
            int result = 0;
            List<string> listDev = new List<string>();

            foreach (DsDevice device in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                string devName = device.Name.ToLower();
                if (listDev.IndexOf(devName) == -1)
                {
                    listDev.Add(devName);

                    if (devName.Length > name.Length)
                        if (devName.Substring(0, name.Length) == name.ToLower())
                            result++;
                }
            }

            listDev.Clear();

            return result;
        }

        //Sets video capture pin out display resolution
        private void setVideoResolution(IBaseFilter pCaptureDevice, string strCaptureVideoOut)
        {
            int hr = 0;
            if (_intVideoResolution < _class.Resolution.List.Count)
            {
                _class.Debug.Log("[3] set resolution " + _class.Resolution.List[_intVideoResolution]);
                hr = ((IAMStreamConfig)GetPin(pCaptureDevice, strCaptureVideoOut)).SetFormat(_class.Resolution.Type[_intVideoResolution]);
                if (hr == 0)
                {
                    _class.Debug.Log("[OK] Set resolution " + _class.Resolution.List[_intVideoResolution]);
                    _intCurrentVideoResolution = _intVideoResolution;
                    _strCurrentResolution = _class.Resolution.List[_intVideoResolution];

                    if (_strCurrentResolution.IndexOf('[') > -1)
                        _strCurrentResolution = _strCurrentResolution.Substring(0, _strCurrentResolution.IndexOf('['));
                }
                else
                {
                    _class.Debug.Log("[NG] Can't set resolution " + _class.Resolution.List[_intVideoResolution]);
                    _class.Debug.Log("-> " + DsError.GetErrorText(hr));
                }
            }
            else
                _class.Debug.Log("[0] [ERR] cant find resolution " + _intVideoResolution.ToString());
        }

        //Finds the closest resolution from device output
        private void lookupAutoResolution()
        {
            if (_class.Graph.IamAvd != null && _intSetResolution == 0)
            {
                int intLineCount = 0;
                _class.Graph.IamAvd.get_NumberOfLines(out intLineCount);

                if (intLineCount > 0)
                {
                    string strLineCount = intLineCount.ToString();
                    _class.System.autoChangeRes(intLineCount);

                    for (int intCount = 0; intCount < _class.Resolution.List.Count; intCount++)
                    {
                        string strRes = _class.Resolution.List[intCount];
                        if (strRes.IndexOf('[') > -1)
                        {
                            strRes = strRes.Substring(0, strRes.IndexOf('['));
                        }
                        strRes = strRes.Trim();
                        string[] strSplit = strRes.Split('x');

                        if (strSplit.Length == 2)
                        {
                            if (strSplit[1].Trim() == strLineCount)
                            {
                                _intVideoResolution = intCount;
                            }
                        }
                    }
                }
            }
        }

        private IBaseFilter setFilter(Guid category, string strTitle, out string strTempOut)
        {
            IBaseFilter pFilter = null;

            string strCategory = "";
            int intHR = 0;
            strTempOut = "";

            if (category == FilterCategory.VideoInputDevice) { strCategory = "Video Capture Device"; }
            if (category == FilterCategory.AudioRendererCategory) { strCategory = "Audio Render Device"; }
            _class.Debug.Log("");
            _class.Debug.Log("[2]  Create " + strTitle + " (" + strCategory + ")");

            pFilter = createFilter(category, strTitle, out strTempOut);
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

        private IBaseFilter createFilter(Guid category, string strName, out string strTitle)
        {
            _class.Debug.Log("[3] Create filter: name>" + strName);
            IBaseFilter filter = null;
            strTitle = "*NF*";
            strName = strName.ToLower();
            int hr = 0;

            int intID = _intDeviceID;
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
                        _intDeviceID = intID;
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

        private void listPin(IBaseFilter filter)
        {
            _listPinIn = new List<string>();
            _listPinOut = new List<string>();

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

                        if (pinfo.dir.ToString().ToLower() == "input") { _listPinIn.Add(pinfo.name); }
                        if (pinfo.dir.ToString().ToLower() == "output") { _listPinOut.Add(pinfo.name); }

                        DsUtils.FreePinInfo(pinfo);
                    }
                }
                for (int intCount = 0; intCount < _listPinIn.Count; intCount++)
                {
                    _class.Debug.Log("-" + _listPinIn[intCount] + " (Input)");
                }

                for (int intCount = 0; intCount < _listPinOut.Count; intCount++)
                {
                    _class.Debug.Log("-" + _listPinOut[intCount] + " (Output)");
                }
            }
            catch
            {
                _class.Debug.Log("[0] [FAIL] Error listing pins");
            }
        }

        private string assumePinIn(string strSearch)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            for (int intCount = 0; intCount < _listPinIn.Count; intCount++)
            {
                string strPinID = _listPinIn[intCount].ToLower();

                if (strPinID.IndexOf(strSearch) > -1) { strReturn = _listPinIn[intCount]; }
            }

            return strReturn;
        }

        private string assumePinIn(string strSearch, string strType)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            for (int intCount = 0; intCount < _listPinIn.Count; intCount++)
            {
                string strPinID = _listPinIn[intCount].ToLower();
                if (strPinID.IndexOf(strSearch) > -1 || strPinID.IndexOf(strType) > -1)
                {
                    string strCheck = strType.ToLower();

                    if (strCheck == "vídeo")
                        strCheck = "video";

                    if (strCheck == "video")
                    {
                        if (strPinID.IndexOf("audio") == -1)
                            strReturn = _listPinOut[intCount];
                    }
                    else
                    {
                        if (strPinID.IndexOf("video") == -1)
                            strReturn = _listPinOut[intCount];
                    }
                }
            }

            return strReturn;
        }

        private string assumePinOut(string strSearch)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            for (int intCount = 0; intCount < _listPinOut.Count; intCount++)
            {
                string strPinID = _listPinOut[intCount].ToLower();

                if (strPinID.IndexOf(strSearch) > -1) { strReturn = _listPinOut[intCount]; }
            }

            return strReturn;
        }

        private string assumePinOut(string strSearch, string strType)
        {
            string strReturn = "";
            strSearch = strSearch.ToLower();
            strType = strType.ToLower();
            for (int intCount = 0; intCount < _listPinOut.Count; intCount++)
            {
                string strPinID = _listPinOut[intCount].ToLower();

                if (strPinID.IndexOf(strSearch) > -1 || strPinID.IndexOf(strType) > -1)
                {
                    string strCheck = strType.ToLower();

                    if (strCheck == "vídeo")
                        strCheck = "video";

                    if (strCheck == "video")
                    {
                        if (strPinID.IndexOf("audio") == -1)
                            strReturn = _listPinOut[intCount];
                    }
                    else
                    {
                        if (strPinID.IndexOf("video") == -1)
                            strReturn = _listPinOut[intCount];
                    }
                }
            }

            return strReturn;
        }

        private IPin GetPin(IBaseFilter filter, string pinname)
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

        #region Crossbar Support
        private bool createCrossbar(ref string strCrossAudioOut, ref string strCrossVideoOut, string strCaptureVideoIn, string strCaptureAudioIn, string strShortName, IBaseFilter pCaptureDevice)
        {
            int hr = 0;

            string strTempOut = "";

            _class.Var.CrossbarInput = new List<string>();

            _class.Debug.Log("");
            _class.Debug.Log("[1] Looking for crossbar " + _intDeviceID);

            IBaseFilter pCrossbar = createFilter(FilterCategory.AMKSCrossbar, strShortName, out strTempOut);
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
                    boolCrossbar = true;

                    listPin(pCrossbar);
                    strCrossAudioOut = assumePinOut("Audio");
                    strCrossVideoOut = assumePinOut("Video");
                    _class.Debug.Log("<Audio>" + strCrossAudioOut);
                    _class.Debug.Log("<Video>" + strCrossVideoOut);

                    _class.Debug.Log("");
                    _class.Debug.Log("Connect Crossbar (" + strCrossVideoOut + ") to Capture (" + strCaptureVideoIn + ")");

                    hr = _class.Graph.CaptureGraph.ConnectDirect(GetPin(pCrossbar, strCrossVideoOut), GetPin(pCaptureDevice, strCaptureVideoIn), null);
                    _class.Debug.Log("Crossbar Video -> " + DsError.GetErrorText(hr));

                    hr = _class.Graph.CaptureGraph.ConnectDirect(GetPin(pCrossbar, strCrossAudioOut), GetPin(pCaptureDevice, strCaptureAudioIn), null);
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
                    _strCrossVideo = strSet;

                if (strSet.Substring(0, "audio_".Length).ToLower() == "audio_")
                    _strCrossAudio = strSet;
            }
        }

        public void checkCrossbar()
        {
            if (_class.Graph.XBar != null)
            {
                if ((_strCrossVideo.Length > 0) || (_strCrossAudio.Length > 0))
                {
                    string strXBarChange = "";
                    if (_strCrossVideo.Length > 0 && _strCrossVideo != "none")
                    {
                        _class.Debug.Log("check cross video " + _strCrossVideo);

                        strXBarChange = findCrossbarSettings(_strCrossVideo, "");
                        _class.Debug.Log("Change crossbar command (video): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _strCrossVideo = "";
                    }

                    if (_strCrossAudio.Length > 0 && _strCrossAudio != "none")
                    {
                        _class.Debug.Log("check cross audio " + _strCrossAudio);

                        strXBarChange = findCrossbarSettings("", _strCrossAudio);
                        _class.Debug.Log("Change crossbar command (audio): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _strCrossAudio = "";
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


        #endregion

        private void createSmartTee(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            int hr = 0;
            _class.Debug.Log("");
            _class.Debug.Log("Creating SmartTee Preview Filter");

            IBaseFilter pSmartTee2 = (IBaseFilter)new SmartTee();
            hr = _class.Graph.CaptureGraph.AddFilter(pSmartTee2, "Smart Tee");
            _class.Debug.Log(DsError.GetErrorText(hr)); 
            _class.Debug.Log("");

            listPin(pSmartTee2);
            strPreviewIn = assumePinIn("Input");
            strPreviewOut = assumePinOut("Preview");

            _class.Debug.Log("");
            _class.Debug.Log("***   Connect " + strDevice + " (" + strPinOut + ") to SmartTee Preview Filter (" + strPreviewIn + ")");
            hr = _class.Graph.CaptureGraph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pSmartTee2, strPreviewIn), null);
            if (hr == 0)
            {
                _class.Debug.Log("[OK] Connected " + strDevice + " to SmartTee Preview Filter");
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

        private void createSampleGrabber(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll

            int hr = 0;
            _class.Debug.Log("");
            _class.Debug.Log("Creating SampleGrabber");

            //add SampleGrabber
            IBaseFilter pSampleGrabber = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber));
            hr = _class.Graph.CaptureGraph.AddFilter(pSampleGrabber, "SampleGrabber");
            _class.Debug.Log("-> " + DsError.GetErrorText(hr));

            listPin(pSampleGrabber);
            string strSampleIn = assumePinIn("Input");
            string strSampleOut = assumePinOut("Output");

            _class.Debug.Log("Set samplegrabber resolution feed");
            if (_class.Resolution.List.Count > 0)
            {

                hr = ((ISampleGrabber)pSampleGrabber).SetMediaType(_class.Resolution.Type[_intVideoResolution]);
                _class.Debug.Log("-> " + DsError.GetErrorText(hr));
            }
            else
                _class.Debug.Log("[ERR] failure in video resolution list");

            _class.Debug.Log("");
            _class.Debug.Log("***   Connect " + strDevice + " (" + strPinOut + ") to SampleGrabber (" + strSampleIn + ")");
            //hr = CaptureGraph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pSampleGrabber, strPreviewIn), null);
            hr = _class.Graph.CaptureGraph.ConnectDirect(GetPin(pRen, "Capture"), GetPin(pSampleGrabber, "Input"), null);
            if (hr == 0)
            {
                var cb = new SampleGrabberCallback();
                cb.GetForm1Handle(_class.FrmMain);

                var sampleGrabber = (ISampleGrabber) pSampleGrabber;
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

        private void createAVIRender(ref string strAVIin, ref string strAVIout, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            int hr = 0;
            _class.Debug.Log("");
            _class.Debug.Log("Creating AVI renderer");
            IBaseFilter pAVIDecompressor = (IBaseFilter)new AVIDec();
            hr = _class.Graph.CaptureGraph.AddFilter(pAVIDecompressor, "AVI Decompressor");
            _class.Debug.Log("-> " + DsError.GetErrorText(hr));

            listPin(pAVIDecompressor);
            strAVIin = assumePinIn("XForm");
            strAVIout = assumePinOut("XForm");

            _class.Debug.Log("");
            _class.Debug.Log("***   Connect " + strDevice + " (" + strPinOut + ") to AVI Decompressor (" + strAVIin + ")");
            hr = _class.Graph.CaptureGraph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pAVIDecompressor, strAVIin), null);
            if (hr == 0)
            {
                _class.Debug.Log("[OK] Connected " + strDevice + " to AVI Decompressor");
                pRen = pAVIDecompressor;
                strDevice = "AVI Decompressor";
                strPinOut = strAVIout;
            }
            else
            {
                _class.Debug.Log("[FAIL] Can't connected " + strDevice + " to AVI Decompressor. May interrupt operation");

            }
        }

        //TODO: add to main declaration list 
        private IntPtr _previewWindow;
        private Point _previewBounds;
        private bool _boolPreviewFail;

        public void setPreviewWindowHandle(IntPtr previewHandle)
        {
            _previewWindow = previewHandle;
        }

        public void setPreviewWindowBounds(Point videoBounds)
        {
            _previewBounds = videoBounds;
        }

        private void setupPreviewWindow()
        {
            _class.Debug.Log("[3]***   Attaching running graph to preview window");

            int hr = 0;

            _boolPreviewFail = false;
            try
            {
                _class.Debug.Log("-> putOwner");

                hr = _class.Graph.VideoPreview.put_Owner(_previewWindow);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }

                _class.Debug.Log("-> putWindowStyle");
                hr = _class.Graph.VideoPreview.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }

                if (_class.Graph.VideoWindow != null)
                {
                    _class.Debug.Log("-> setBounds (" + _previewBounds.X.ToString() + ", " + _previewBounds.Y.ToString() + ")");
                    //Point ptReturn = frmMain.setVideoWindowBounds();
                    _class.Debug.Log("-> setWindowPosition");
                    _class.Graph.VideoPreview.SetWindowPosition(0, 0, _previewBounds.X, _previewBounds.Y);
                }
                _class.Debug.Log("-> putVisible");
                hr = _class.Graph.VideoPreview.put_Visible(OABool.True);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }
            }
            catch (Exception e)
            {
                _class.Debug.Log("[ERR] *setupVideoWindow* " + e.ToString());
                _boolPreviewFail = true;
                return;
            }
        }

        public void setupVideoWindow()
        {
            _class.Debug.Log("[0]***   Attaching running graph to video window");

            int hr = 0;

            IntPtr videoHandle = _class.FrmMain.ReturnVideoHandle();

            _boolVideoFail = false;
            try
            {
                _class.Debug.Log("-> putOwner");
                hr = _class.Graph.VideoWindow.put_Owner(videoHandle);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }

                _class.Debug.Log("-> putWindowStyle");
                hr = _class.Graph.VideoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }

                if (_class.Graph.VideoWindow != null)
                {
                    _class.Debug.Log("-> setBounds");
                    Point ptReturn = _class.FrmMain.SetVideoWindowBounds();
                    _class.Debug.Log("-> setWindowPosition");
                    _class.Graph.VideoWindow.SetWindowPosition(0, 0, ptReturn.X, ptReturn.Y);
                }
                _class.Debug.Log("-> putVisible");
                hr = _class.Graph.VideoWindow.put_Visible(OABool.True);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }
            }
            catch (Exception e)
            {
                _class.Debug.Log("[ERR] *setupVideoWindow* " + e.ToString());
                _boolVideoFail = true;
                return;
            }
        }

        public void checkVideoOutput()
        {
            //Reruns the graph once, find this needs to happen after quick resolution changes (PS3)
            if (_class.Var.IsRestartGraph && !_boolVideoFail)           
            {
                _class.Var.IsRestartGraph = false;
                _class.Debug.Log("[3] Update graph");
                runGraph();
            }

            if (!_boolVideoFail && _class.System.boolAutoSetCaptureResolution)
                _class.Resolution.Check();

            if (_class.Var.IsRestartGraph)
            {
                if (_intRestartGraph > 0) 
                    _intRestartGraph--; 
                else 
                { 
                    _class.Debug.Log("[3] Restart graph");
                    _class.Var.IsRestartGraph = false; 
                    _boolVideoFail = false; 
                    runGraph(); 
                }
            }
        }

        //Compares video capture resolution with video out resolution, adjusts graph if needed

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
            _class.Graph.IVideoWindow = null;
            _class.Graph.IamAvd = null;
            _class.Graph.XBar = null;

            _class.Debug.Log("[OK] close ok");
        }
        #endregion

        public void SetVideoCaptureDevice(string device) { _class.Capture.Set(device); }
        public void SetPreviewWindow(bool set) { _class.Var.ShowPreviewWindow = set; }
        public List<string> GetVideoResolution() { return _class.Resolution.List; }
        public int GetVideoResolutionCurrent() { return _intCurrentVideoResolution; }
        public void SetVideoResolution(int setRes) { _intVideoResolution = setRes; _intSetResolution = setRes; }
        public List<string> GetCrossbarList() { return _class.Var.CrossbarInput; }
        public string GetCrossbarOutput(int id, string type) { return _class.Crossbar.List(id, type); }
        public void UpdateVideoCaptureList(List<string> data)
        {
            
        }
    }

}
