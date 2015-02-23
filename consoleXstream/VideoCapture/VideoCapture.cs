using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using DirectShowLib;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing;
using consoleXstream.Config;

namespace consoleXstream.VideoCapture
{
    public class VideoCapture
    {
        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
 
        private readonly Form1 _frmMain;
        private Configuration _system;

        public VideoCapture(Form1 mainForm) { _frmMain = mainForm; }

        //References from the main menu
        public bool boolUseCrossbar { get; private set; }
        public bool boolCreateSmartTee { get; private set; }
        public bool boolCreateAVIRender { get; private set; }
        public bool boolActiveVideo { get; private set; }
        public bool boolCrossbar { get; private set; }
        public bool boolSampleGrabber { get; private set; }
        public string strVideoPin = "";
        public string strAudioPin = "";
        public string strVideoCaptureDevice { get; private set; }
        public string strAudioPlaybackDevice { get; private set; }

        public List<string> listCrossbarInput { get; private set; }
        public List<string> listVideoCapture { get; private set; }
        public List<string> listVideoCaptureName { get; private set; }

        public IMediaEvent MediaEvent
        {
            get { return _mediaEvent; }
            set { _mediaEvent = value; }
        }


        private List<string> _listAudioDevice;
        private List<AMMediaType> _listVideoRes;
        private List<string> _listVideoResolution;

        private List<string> _listPinIn;
        private List<string> _listPinOut;

        private string _strCurrentResolution;
        private string _strCrossVideo;
        private string _strCrossAudio;

        private int _intLastDebugLevel = 0;
        private int _intRestartGraph = 0;
        private int _intDeviceID = 0;

        private int _intVideoDevice = 0;
        private int _intVideoResolution = 0;
        private int _intCurrentVideoResolution = 0;
        private int _intSetResolution = 0;
        private int _intAudioDevice = -1;

        private bool _boolBuildingGraph;
        private bool _boolRestartGraph;
        private bool _boolVideoFail;
        private bool _boolRerunGraph;
        private bool _boolInitializeGraph;
        private bool _boolShowPreviewWindow;
        private bool _IsChangedDisplayResolution;

        private IGraphBuilder _graph;
        private IMediaControl _mediaControl;
        private IMediaEvent _mediaEvent;
        private IVideoWindow _videoWindow;
        private IAMAnalogVideoDecoder _iamAvd;
        private IBasicVideo _iVideoDef;
        private IVideoWindow _iVideoWindow;
        private IVideoWindow _videoPreview;
        public IAMCrossbar _xBar;

        protected virtual async Task WriteTextAsync(string strWrite)
        {
            strWrite = strWrite.Trim();
            var strCurrentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            var txtOut = new StreamWriter("video.log", true);
            if (strWrite.Length > 0)
                strWrite = strCurrentTime + " - " + strWrite;
            await txtOut.WriteLineAsync(strWrite);
            txtOut.Close();
        }

        public async void DebugVideo(string strWrite)
        {
            if (_system != null)
            {
                int intLevel = _intLastDebugLevel;
                int intSysLevel = _system.intDebugLevel;

                if (strWrite.IndexOf(']') == 2)
                {
                    string strTest = strWrite.Substring(1, strWrite.IndexOf(']') - 1);
                    strWrite = strWrite.Substring(strWrite.IndexOf(']') + 1);
                    try
                    {
                        intLevel = Convert.ToInt32(strTest);
                    }
                    catch { }       //Dont care if this errors. Last system debug level will still apply
                }

                _intLastDebugLevel = intLevel;

                if (intLevel <= intSysLevel)
                {
                    await WriteTextAsync(strWrite);
                }
            }
        }

        public void getSystemHandle(Config.Configuration inSystem)
        {
            _system = inSystem;
        }

        public void initialzeCapture()
        {
            _strCrossAudio = "none";
            _strCrossVideo = "none";

            //Caches build information
            findVideoDevices();
            findAudioDevices();

            loadUserSettings();
            findVideoResolution();

            _boolInitializeGraph = true;
            _boolRestartGraph = true;
        }

        //Loads all user settings for graph
        public void loadUserSettings()
        {
            //Load user settings
            if (_system.checkUserSetting("Crossbar") == "true") boolUseCrossbar = true; else boolCrossbar = false;
            if (_system.checkUserSetting("Preview") == "true") boolCreateSmartTee = true; else boolCreateSmartTee = false;
            if (_system.checkUserSetting("AVIRender") == "true") boolCreateAVIRender = true; else boolCreateAVIRender = false;
            
            //Check as user setting
            boolSampleGrabber = true;

            strVideoPin = _system.checkUserSetting("crossbarVideoPin");
            strAudioPin = _system.checkUserSetting("crossbarAudioPin");

            setVideoCaptureDevice(_system.checkUserSetting("VideoCaptureDevice"));

            if (boolCreateSmartTee)
                DebugVideo("Using smartTee");
            else
                DebugVideo("SmartTee disabled");

            if (boolCreateAVIRender)
                DebugVideo("Using AVI Renderer");
            else
                DebugVideo("AVI Rendered disabled");
        }

        #region Video Capture Information
        public void setPreviewWindow(bool boolSet)
        {
            _boolShowPreviewWindow = boolSet;
        }

        public void setVideoCaptureDevice(string strTitle)
        {
            DebugVideo("[0] Looking for " + strTitle);
            int intIndex = listVideoCaptureName.FindIndex(x => x.Equals(strTitle, StringComparison.OrdinalIgnoreCase));
            DebugVideo("[0] DeviceID: " + intIndex.ToString());
            if (intIndex > -1)
                _intVideoDevice = intIndex;
            else
                DebugVideo("[0] [ERR] Cant find " + strTitle + " VCD");
        }

        //Caches currently connected video capture devices
        private void findVideoDevices()
        {
            listVideoCapture = new List<string>();
            listVideoCaptureName = new List<string>();


            DebugVideo("[0] Listing video capture devices");
            DsDevice[] devObjects = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            if (devObjects.Length > 0)
            {
                for (int intCount = 0; intCount < devObjects.Length; intCount++)
                {
                    string strTitle = devObjects[intCount].Name;

                    listVideoCapture.Add(strTitle);
                    int intDevID = 1;

                    if (listVideoCaptureName.IndexOf(strTitle) > -1)
                    {
                        bool boolFound = true;
                        while (boolFound == true)
                        {
                            string strSet = strTitle + " (" + intDevID + ")";
                            if (listVideoCaptureName.IndexOf(strSet) == -1) { boolFound = false; strTitle = strSet; } else { intDevID++; }
                        }
                    }
                    listVideoCaptureName.Add(strTitle);
                    DebugVideo("->" + strTitle);
                }
            }
            else
            {
                listVideoCapture.Add("*NF*");
                listVideoCaptureName.Add("");
                DebugVideo("[Err] No capture devices found");
            }
            DebugVideo("");
        }

        private void findVideoResolution()
        {
            try
            {
                DebugVideo("[0] Find video device resolution");
                
                _listVideoResolution = new List<string>();
                _listVideoResolution.Add("Auto");

                _listVideoRes = new List<AMMediaType>();
                _listVideoRes.Add(null);

                DsDevice dev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)[_intVideoDevice];
                IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
                IBaseFilter baseDev;

                filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out baseDev);
                IPin pin = DsFindPin.ByCategory(baseDev, PinCategory.Capture, 0);
                IAMStreamConfig streamConfig = pin as IAMStreamConfig;
                AMMediaType media;
                IntPtr ptr;
                int iC = 0, iS = 0;
                streamConfig.GetNumberOfCapabilities(out iC, out iS);
                ptr = Marshal.AllocCoTaskMem(iS);

                for (int i = 0; i < iC; i++)
                {
                    streamConfig.GetStreamCaps(i, out media, ptr);
                    VideoInfoHeader v;
                    v = new VideoInfoHeader();
                    Marshal.PtrToStructure(media.formatPtr, v);

                    if (v.BmiHeader.Width != 0)
                    {
                        string strRes = v.BmiHeader.Width + " x " + v.BmiHeader.Height;
                        strRes += "  [" + checkMediaType(media) + "]";
                        _listVideoRes.Add(media);
                        _listVideoResolution.Add(strRes);
                        DebugVideo("->" + strRes);
                    }
                }
                
                _system.strCurrentResolution = _listVideoResolution[_intVideoResolution];

                DebugVideo("");
            }
            catch (Exception e)
            {
                DebugVideo("[ERR] fail find video resolution : " + e.ToString());
            }
        }

        public List<string> getVideoResolution()
        {
            return _listVideoResolution;
        }

        public int getVideoResolutionCurrent()
        {
            return _intCurrentVideoResolution;
        }

        public void setVideoResolution(int setRes)
        {
            _intVideoResolution = setRes;
            _intSetResolution = setRes;
        }

        //Return human readable text instead of GUID
        private string checkMediaType(AMMediaType media)
        {
            if (media.subType == MediaSubType.A2B10G10R10) { return "A2B10G10R10"; }
            if (media.subType == MediaSubType.A2R10G10B10) { return "A2R10G10B10"; }
            if (media.subType == MediaSubType.AI44) { return "AI44"; }
            if (media.subType == MediaSubType.AIFF) { return "AI44"; }
            if (media.subType == MediaSubType.AnalogVideo_NTSC_M) { return "AnalogVideo_NTSC_M"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_B) { return "AnalogVideo_PAL_B"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_D) { return "AnalogVideo_PAL_D"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_G) { return "AnalogVideo_PAL_G"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_H) { return "AnalogVideo_PAL_H"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_I) { return "AnalogVideo_PAL_I"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_M) { return "AnalogVideo_PAL_M"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_N) { return "AnalogVideo_PAL_N"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_N_COMBO) { return "AnalogVideo_PAL_N_COMBO"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_B) { return "AnalogVideo_SECAM_B"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_D) { return "AnalogVideo_SECAM_D"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_G) { return "AnalogVideo_SECAM_G"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_H) { return "AnalogVideo_SECAM_H"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_K) { return "AnalogVideo_SECAM_K"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_K1) { return "AnalogVideo_SECAM_K1"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_L) { return "AnalogVideo_SECAM_L"; }
            if (media.subType == MediaSubType.ARGB1555) { return "ARGB1555"; }
            if (media.subType == MediaSubType.ARGB1555_D3D_DX7_RT) { return "ARGB1555_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.ARGB1555_D3D_DX9_RT) { return "ARGB1555_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.ARGB32) { return "ARGB32"; }
            if (media.subType == MediaSubType.ARGB32_D3D_DX7_RT) { return "ARGB32_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.ARGB32_D3D_DX9_RT) { return "ARGB32_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.ARGB4444) { return "ARGB4444"; }
            if (media.subType == MediaSubType.ARGB4444_D3D_DX7_RT) { return "ARGB4444_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.ARGB4444_D3D_DX9_RT) { return "ARGB4444_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.Asf) { return "Asf"; }
            if (media.subType == MediaSubType.AtscSI) { return "AtscSI"; }
            if (media.subType == MediaSubType.AU) { return "AU"; }
            if (media.subType == MediaSubType.Avi) { return "Avi"; }
            if (media.subType == MediaSubType.AYUV) { return "AYUV"; }
            if (media.subType == MediaSubType.CFCC) { return "CFCC"; }
            if (media.subType == MediaSubType.CLJR) { return "CLJR"; }
            if (media.subType == MediaSubType.CPLA) { return "CPLA"; }
            if (media.subType == MediaSubType.Data708_608) { return "Data708_608"; }
            if (media.subType == MediaSubType.DOLBY_AC3_SPDIF) { return "DOLBY_AC3_SPDIF"; }
            if (media.subType == MediaSubType.DolbyAC3) { return "DolbyAC3"; }
            if (media.subType == MediaSubType.DRM_Audio) { return "DRM_Audio"; }
            if (media.subType == MediaSubType.DssAudio) { return "DssAudio"; }
            if (media.subType == MediaSubType.DssVideo) { return "DssVideo"; }
            if (media.subType == MediaSubType.DtvCcData) { return "DtvCcData"; }
            if (media.subType == MediaSubType.dv25) { return "dv25"; }
            if (media.subType == MediaSubType.dv50) { return "dv50"; }
            if (media.subType == MediaSubType.DvbSI) { return "DvbSI"; }
            if (media.subType == MediaSubType.DVCS) { return "DVCS"; }
            if (media.subType == MediaSubType.dvh1) { return "dvh1"; }
            if (media.subType == MediaSubType.dvhd) { return "dvhd"; }
            if (media.subType == MediaSubType.DVSD) { return "DVSD"; }
            if (media.subType == MediaSubType.dvsl) { return "dvsl"; }
            if (media.subType == MediaSubType.H264) { return "H264"; }
            if (media.subType == MediaSubType.I420) { return "I420"; }
            if (media.subType == MediaSubType.IA44) { return "IA44"; }
            if (media.subType == MediaSubType.IEEE_FLOAT) { return "IEEE_FLOAT"; }
            if (media.subType == MediaSubType.IF09) { return "IF09"; }
            if (media.subType == MediaSubType.IJPG) { return "IJPG"; }
            if (media.subType == MediaSubType.IMC1) { return "IMC1"; }
            if (media.subType == MediaSubType.IMC2) { return "IMC2"; }
            if (media.subType == MediaSubType.IMC3) { return "IMC3"; }
            if (media.subType == MediaSubType.IMC4) { return "IMC4"; }
            if (media.subType == MediaSubType.IYUV) { return "IYUV"; }
            if (media.subType == MediaSubType.Line21_BytePair) { return "Line21_BytePair"; }
            if (media.subType == MediaSubType.Line21_GOPPacket) { return "Line21_GOPPacket"; }
            if (media.subType == MediaSubType.Line21_VBIRawData) { return "Line21_VBIRawData"; }
            if (media.subType == MediaSubType.MDVF) { return "MDVF"; }
            if (media.subType == MediaSubType.MJPG) { return "MJPG"; }
            if (media.subType == MediaSubType.MPEG1AudioPayload) { return "MPEG1AudioPayload"; }
            if (media.subType == MediaSubType.MPEG1Packet) { return "MPEG1Packet"; }
            if (media.subType == MediaSubType.MPEG1Payload) { return "MPEG1Payload"; }
            if (media.subType == MediaSubType.MPEG1System) { return "MPEG1System"; }
            if (media.subType == MediaSubType.MPEG1SystemStream) { return "MPEG1SystemStream"; }
            if (media.subType == MediaSubType.MPEG1Video) { return "MPEG1Video"; }
            if (media.subType == MediaSubType.MPEG1VideoCD) { return "MPEG1VideoCD"; }
            if (media.subType == MediaSubType.Mpeg2Audio) { return "Mpeg2Audio"; }
            if (media.subType == MediaSubType.Mpeg2Data) { return "Mpeg2Data"; }
            if (media.subType == MediaSubType.Mpeg2Program) { return "Mpeg2Program"; }
            if (media.subType == MediaSubType.Mpeg2Transport) { return "Mpeg2Transport"; }
            if (media.subType == MediaSubType.Mpeg2TransportStride) { return "Mpeg2TransportStride"; }
            if (media.subType == MediaSubType.Mpeg2Video) { return "Mpeg2Video"; }
            if (media.subType == MediaSubType.None) { return "None"; }
            if (media.subType == MediaSubType.Null) { return "Null"; }
            if (media.subType == MediaSubType.NV12) { return "NV12"; }
            if (media.subType == MediaSubType.NV24) { return "NV24"; }
            if (media.subType == MediaSubType.Overlay) { return "Overlay"; }
            if (media.subType == MediaSubType.PCM) { return "PCM"; }
            if (media.subType == MediaSubType.PCMAudio_Obsolete) { return "PCMAudio_Obsolete"; }
            if (media.subType == MediaSubType.PLUM) { return "PLUM"; }
            if (media.subType == MediaSubType.QTJpeg) { return "QTJpeg"; }
            if (media.subType == MediaSubType.QTMovie) { return "QTMovie"; }
            if (media.subType == MediaSubType.QTRle) { return "QTRle"; }
            if (media.subType == MediaSubType.QTRpza) { return "QTRpza"; }
            if (media.subType == MediaSubType.QTSmc) { return "QTSmc"; }
            if (media.subType == MediaSubType.RAW_SPORT) { return "RAW_SPORT"; }
            if (media.subType == MediaSubType.RGB1) { return "RGB1"; }
            if (media.subType == MediaSubType.RGB16_D3D_DX7_RT) { return "RGB16_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.RGB16_D3D_DX9_RT) { return "RGB16_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.RGB24) { return "RGB24"; }
            if (media.subType == MediaSubType.RGB32) { return "RGB32"; }
            if (media.subType == MediaSubType.RGB32_D3D_DX7_RT) { return "RGB32_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.RGB32_D3D_DX9_RT) { return "RGB32_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.RGB4) { return "RGB4"; }
            if (media.subType == MediaSubType.RGB555) { return "RGB555"; }
            if (media.subType == MediaSubType.RGB565) { return "RGB565"; }
            if (media.subType == MediaSubType.RGB8) { return "RGB8"; }
            if (media.subType == MediaSubType.S340) { return "S340"; }
            if (media.subType == MediaSubType.S342) { return "S342"; }
            if (media.subType == MediaSubType.SPDIF_TAG_241h) { return "SPDIF_TAG_241h"; }
            if (media.subType == MediaSubType.TELETEXT) { return "TELETEXT"; }
            if (media.subType == MediaSubType.TVMJ) { return "TVMJ"; }
            if (media.subType == MediaSubType.UYVY) { return "UYVY"; }
            if (media.subType == MediaSubType.VideoImage) { return "VideoImage"; }
            if (media.subType == MediaSubType.VPS) { return "VPS"; }
            if (media.subType == MediaSubType.VPVBI) { return "VPVBI"; }
            if (media.subType == MediaSubType.VPVideo) { return "VPVideo"; }
            if (media.subType == MediaSubType.WAKE) { return "WAKE"; }
            if (media.subType == MediaSubType.WAVE) { return "WAVE"; }
            if (media.subType == MediaSubType.WebStream) { return "WebStream"; }
            if (media.subType == MediaSubType.WSS) { return "WSS"; }
            if (media.subType == MediaSubType.Y211) { return "Y211"; }
            if (media.subType == MediaSubType.Y411) { return "Y411"; }
            if (media.subType == MediaSubType.Y41P) { return "Y41P"; }
            if (media.subType == MediaSubType.YUY2) { return "YUY2"; }
            if (media.subType == MediaSubType.YUYV) { return "YUYV"; }
            if (media.subType == MediaSubType.YV12) { return "YV12"; }
            if (media.subType == MediaSubType.YVU9) { return "YVU9"; }
            if (media.subType == MediaSubType.YVYU) { return "YVYU"; }

            return "";
        }

        //Lists available audio output. Default to WaveOut for simplicity
        private void findAudioDevices()
        {
            DebugVideo("[0] Find audio devices");
            _listAudioDevice = new List<string>();

            DsDevice[] devObject = DsDevice.GetDevicesOfCat(FilterCategory.AudioRendererCategory);

            for (int intCount = 0; intCount < devObject.Length; intCount++)
            {
                if (_listAudioDevice.IndexOf(devObject[intCount].Name) == -1)
                {
                    _listAudioDevice.Add(devObject[intCount].Name.ToString());
                    DebugVideo("->" + devObject[intCount].Name);
                    //If nothing set, assume this
                    if (_intAudioDevice == -1)
                    {
                        if (devObject[intCount].Name == "Default WaveOut Device")
                            _intAudioDevice = intCount;
                    }
                }
            }

            DebugVideo("");
        }

        public string showCrossbarOutput(int intID, string strType)
        {
            string strReturn = "";
            if (listCrossbarInput.Count == 0) { FindCrossbarOutput(false); }
            if (listCrossbarInput.Count > 0)
            {
                int intConnector = 0;
                for (int intCount = 0; intCount < listCrossbarInput.Count; intCount++)
                {
                    string strTemp = listCrossbarInput[intCount];
                    if (strTemp.Length > "video_".Length)
                    {
                        if (strType.ToLower() == "video")
                        {
                            if (strTemp.Substring(0, "video_".Length).ToLower() == "video_")
                            {
                                if (intConnector == intID) { strReturn = strTemp; }
                            }
                        }
                        if (strType.ToLower() == "audio")
                        {
                            if (strTemp.Substring(0, "audio".Length).ToLower() == "audio")
                            {
                                if (intConnector == intID) { strReturn = strTemp; }
                            }
                        }
                        intConnector++;
                    }
                }
            }
            return strReturn;
        }

        #endregion

        #region DirectShow Graph
        public void runGraph()
        {
            DebugVideo("[0] Build capture graph");
            if (_intVideoDevice > -1 && _intVideoDevice < listVideoCaptureName.Count)
            {
                int hr = 0;

                _boolBuildingGraph = true;
                DebugVideo("Using : " + listVideoCaptureName[_intVideoDevice]);
                DebugVideo("");

                if (_mediaControl != null) _mediaControl.StopWhenReady();
                if (_listVideoResolution.Count == 0) { findVideoResolution();  }

                _graph = null;
                _mediaControl = null;
                _mediaEvent = null;
                _videoWindow = null;
                _iVideoDef = null;
                _iVideoWindow = null;
                _xBar = null;

                _graph = (IGraphBuilder)new FilterGraph();

                if (buildGraph())
                {
                    if (!_boolShowPreviewWindow)
                        setupVideoWindow();
                    else
                        setupPreviewWindow();

                    _mediaControl = (IMediaControl)_graph;
                    _mediaEvent = (IMediaEvent)_graph;

                    DebugVideo("");
                    DebugVideo("Run compiled graph");
                    hr = _mediaControl.Run();
                    DebugVideo("[2] " + DsError.GetErrorText(hr));

                    //TODO: Check for major / minor errors before declaring video run
                    boolActiveVideo = true;
                }

                _boolBuildingGraph = false;

                if (_xBar != null)
                    if (listCrossbarInput.Count == 0)
                        FindCrossbarOutput(false);
                
                if (_boolRestartGraph)
                    _intRestartGraph = 3;
            }
            else
                DebugVideo("[ERR] Unknown capture device");
        }

        private bool buildGraph()
        {
            int hr = 0;
            
            if (_intVideoDevice > -1 && _intVideoDevice < listVideoCapture.Count)
            {
                string strVideoDevice = listVideoCaptureName[_intVideoDevice];
                string strShortName = findCaptureName(strVideoDevice);

                if (_intVideoResolution > _listVideoResolution.Count)
                    _intVideoResolution = 0;

                if (_intAudioDevice > _listAudioDevice.Count)
                {
                    _intAudioDevice = -1;
                    findAudioDevices();
                }

                DebugVideo("[2] VCD: " + strVideoDevice);
                DebugVideo("[2] VCDID: " + strShortName);
                DebugVideo("[2] RES: " + _listVideoResolution[_intVideoResolution]);
                DebugVideo("[2] AOD: " + _listAudioDevice[_intAudioDevice]);
                
                strVideoCaptureDevice = strVideoDevice;
                strAudioPlaybackDevice = _listAudioDevice[_intAudioDevice];

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
                DebugVideo("");
                DebugVideo("[0] Create new graph");
                ICaptureGraphBuilder2 pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                hr = pBuilder.SetFiltergraph(_graph);
                DebugVideo("[2] [OK] " + DsError.GetErrorText(hr));
                DebugVideo("");

                _videoWindow = (IVideoWindow)_graph;            //Open the window

                //Primary Capture Device
                IBaseFilter pCaptureDevice = setFilter(FilterCategory.VideoInputDevice, strVideoDevice, out strTempOut);
                DebugVideo("");
                if (pCaptureDevice == null)
                {
                    DebugVideo("[ERR] Cant create capture device. Graph cannot continue");
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

                DebugVideo("[0]");
                DebugVideo("<Video Out>" + strCaptureVideoOut);
                DebugVideo("<Audio Out>" + strCaptureAudioOut);
                DebugVideo("<Video In>" + strCaptureVideoIn);
                DebugVideo("<Audio In>" + strCaptureAudioIn);
                DebugVideo("");

                _iamAvd = pCaptureDevice as IAMAnalogVideoDecoder;

                //Create user crossbar if needed
                if (boolUseCrossbar == true)
                    if (createCrossbar(ref strCrossAudioOut, ref strCrossVideoOut, strCaptureVideoIn, strCaptureAudioIn, strShortName, pCaptureDevice))
                        checkCrossbar();

                DebugVideo("");

                //Set resolution
                DebugVideo("[0] Checking capture resolution");
                if (_intVideoResolution == 0)       
                    lookupAutoResolution();

                if (_intVideoResolution > 0)
                    setVideoResolution(pCaptureDevice, strCaptureVideoOut);
                else
                    DebugVideo("[0] [WARN] Cant find capture resolution - no input or unknown resolution type");

                IBaseFilter pRen = pCaptureDevice;
                string strPinOut = strCaptureVideoOut;
                string strDevice = strVideoDevice;

                if (boolSampleGrabber)
                    createSampleGrabber(ref strPreviewIn, ref strPreviewOut, ref strDevice, ref strPinOut, ref pRen);

                if (boolCreateSmartTee)
                    createSmartTee(ref strPreviewIn, ref strPreviewOut, ref strDevice, ref strPinOut, ref pRen);

                if (boolCreateAVIRender)
                    createAVIRender(ref strAVIin, ref strAVIout, ref strDevice, ref strPinOut, ref pRen);

                //Video renderer
                DebugVideo("");
                DebugVideo("[0]***   Create Video Renderer");
                Guid CLSID_ActiveVideo = new Guid("{B87BEB7B-8D29-423F-AE4D-6582C10175AC}");

                IBaseFilter pVideoRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ActiveVideo));
                hr = _graph.AddFilter(pVideoRenderer, "Video Renderer");
                if (hr == 0) { DebugVideo("[1] [OK] Created video renderer"); }
                else
                {
                    DebugVideo("[1] [FAIL] Cant create video renderer");
                    DebugVideo("-> " + DsError.GetErrorText(hr));
                }

                DebugVideo("");
                DebugVideo("***   Listing Video Renderer pins");
                listPin(pVideoRenderer);
                strVideoIn = assumePinIn("Input");
                DebugVideo("<Video>" + strVideoIn);
                DebugVideo("");

                DebugVideo("***   Connect AVI Decompressor (" + strPinOut + ") to Video Renderer (" + strVideoIn + ")");
                hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pVideoRenderer, strVideoIn), null);
                if (hr == 0) { DebugVideo("[OK] Connected AVI to video renderer"); }
                else
                {
                    DebugVideo("[FAIL] Can't connect AVI to video renderer");
                    DebugVideo("-> " + DsError.GetErrorText(hr));
                }

                _iVideoDef = pVideoRenderer as IBasicVideo;
                _iVideoWindow = pVideoRenderer as IVideoWindow;
                
                //Audio device
                if (_intAudioDevice > -1 && _intAudioDevice < _listAudioDevice.Count)
                {
                    _intDeviceID = 0;               //Dont need multiple devices, set back to 0

                    DebugVideo("[0]");
                    DebugVideo("***   Create " + _listAudioDevice[_intAudioDevice] + " audio device");
                    IBaseFilter pAudio = null;

                    pAudio = setFilter(FilterCategory.AudioRendererCategory, _listAudioDevice[_intAudioDevice], out strTempOut);
                    hr = _graph.AddFilter(pAudio, "Audio Device");
                    DebugVideo("-> " + DsError.GetErrorText(hr));

                    if (pAudio != null)
                    {
                        DebugVideo("[1]");
                        DebugVideo("***   Listing " + _listAudioDevice[_intAudioDevice] + " pins");

                        listPin(pAudio);
                        strAudioIn = assumePinIn("Audio");
                        DebugVideo("<Audio>" + strAudioIn);
                        DebugVideo("");

                        //connect Capture Device and Audio Device
                        DebugVideo("***   Connect " + strVideoDevice + " (" + strCaptureAudioOut + ") to " + _listAudioDevice[_intAudioDevice] + " [Audio] (" + strAudioIn + ")");
                        hr = _graph.ConnectDirect(GetPin(pCaptureDevice, strCaptureAudioOut), GetPin(pAudio, strAudioIn), null);
                        DebugVideo("-> " + DsError.GetErrorText(hr));
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
            if (_intVideoResolution < _listVideoResolution.Count)
            {
                DebugVideo("[3] set resolution " + _listVideoResolution[_intVideoResolution]);
                hr = ((IAMStreamConfig)GetPin(pCaptureDevice, strCaptureVideoOut)).SetFormat(_listVideoRes[_intVideoResolution]);
                if (hr == 0)
                {
                    DebugVideo("[OK] Set resolution " + _listVideoResolution[_intVideoResolution]);
                    _intCurrentVideoResolution = _intVideoResolution;
                    _strCurrentResolution = _listVideoResolution[_intVideoResolution];

                    if (_strCurrentResolution.IndexOf('[') > -1)
                        _strCurrentResolution = _strCurrentResolution.Substring(0, _strCurrentResolution.IndexOf('['));
                }
                else
                {
                    DebugVideo("[NG] Can't set resolution " + _listVideoResolution[_intVideoResolution]);
                    DebugVideo("-> " + DsError.GetErrorText(hr));
                }
            }
            else
                DebugVideo("[0] [ERR] cant find resolution " + _intVideoResolution.ToString());
        }

        //Finds the closest resolution from device output
        private void lookupAutoResolution()
        {
            if (_iamAvd != null && _intSetResolution == 0)
            {
                int intLineCount = 0;
                _iamAvd.get_NumberOfLines(out intLineCount);

                if (intLineCount > 0)
                {
                    string strLineCount = intLineCount.ToString();
                    _system.autoChangeRes(intLineCount);

                    for (int intCount = 0; intCount < _listVideoResolution.Count; intCount++)
                    {
                        string strRes = _listVideoResolution[intCount];
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
            DebugVideo("");
            DebugVideo("[2]  Create " + strTitle + " (" + strCategory + ")");

            pFilter = createFilter(category, strTitle, out strTempOut);
            intHR = _graph.AddFilter(pFilter, strTempOut);
            if (intHR == 0)
            {
                DebugVideo("[OK] Added " + strTitle + " (" + strCategory + ") to graph");
            }
            else
            {
                DebugVideo("[FAIL] Can't add " + strTitle + " (" + strCategory + ") to graph");
                DebugVideo("-> " + DsError.GetErrorText(intHR));
            }

            return pFilter;
        }

        private IBaseFilter createFilter(Guid category, string strName, out string strTitle)
        {
            DebugVideo("[3] Create filter: name>" + strName);
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

                DebugVideo("FilterName: " + strName);
                DebugVideo("DevID: " + strID);
                try
                {
                    intID = Convert.ToInt32(strID);
                }
                catch { }
                DebugVideo("Confirm DevID: " + intID.ToString());
            }

            int intDevID = 0;

            DsDevice[] devices = DsDevice.GetDevicesOfCat(category);
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(category))
            {
                string strDevice = device.Name.ToLower();
                DebugVideo("device>" + strDevice);
                if (strDevice.IndexOf(strName) > -1 || strDevice.ToLower() == "oem crossbar")
                {
                    bool boolCreate = true;
                    DebugVideo("TargetID=" + intID.ToString() + " [] + DevID=" + intDevID.ToString());

                    if (intDevID == intID) 
                    { 
                        boolCreate = true;
                        _intDeviceID = intID;
                    } 
                    else 
                        boolCreate = false; 

                    if (boolCreate == true)
                    {
                        DebugVideo("SetDeviceID (" + intDevID.ToString() + ")");
                        strTitle = device.Name;
                        IBindCtx bindCtx = null;
                        try
                        {
                            hr = CreateBindCtx(0, out bindCtx);
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
                    else { DebugVideo(intDevID.ToString() + " skipped"); }
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
            DebugVideo("[3] listing Pins [" + pOut.achName + "]");

            try
            {
                IEnumPins epins;
                int hr = filter.EnumPins(out epins);
                if (hr < 0) { DebugVideo("[0] [NG] Can't find pins"); }
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
                    DebugVideo("-" + _listPinIn[intCount] + " (Input)");
                }

                for (int intCount = 0; intCount < _listPinOut.Count; intCount++)
                {
                    DebugVideo("-" + _listPinOut[intCount] + " (Output)");
                }
            }
            catch
            {
                DebugVideo("[0] [FAIL] Error listing pins");
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

            listCrossbarInput = new List<string>();

            DebugVideo("");
            DebugVideo("[1] Looking for crossbar " + _intDeviceID);

            IBaseFilter pCrossbar = createFilter(FilterCategory.AMKSCrossbar, strShortName, out strTempOut);
            if (strTempOut.ToLower() == "*nf*")
            {
                DebugVideo("[FAIL] No crossbar found. Will not interrupt operation");
                return false;
            }
            else
            {
                hr = _graph.AddFilter(pCrossbar, strTempOut);
                if (hr == 0)
                {
                    DebugVideo("[OK] Create crossbar");
                    boolCrossbar = true;

                    listPin(pCrossbar);
                    strCrossAudioOut = assumePinOut("Audio");
                    strCrossVideoOut = assumePinOut("Video");
                    DebugVideo("<Audio>" + strCrossAudioOut);
                    DebugVideo("<Video>" + strCrossVideoOut);

                    DebugVideo("");
                    DebugVideo("Connect Crossbar (" + strCrossVideoOut + ") to Capture (" + strCaptureVideoIn + ")");

                    hr = _graph.ConnectDirect(GetPin(pCrossbar, strCrossVideoOut), GetPin(pCaptureDevice, strCaptureVideoIn), null);
                    DebugVideo("Crossbar Video -> " + DsError.GetErrorText(hr));

                    hr = _graph.ConnectDirect(GetPin(pCrossbar, strCrossAudioOut), GetPin(pCaptureDevice, strCaptureAudioIn), null);
                    DebugVideo("Crossbar Audio -> " + DsError.GetErrorText(hr));

                    _xBar = (IAMCrossbar)pCrossbar;
                    DebugVideo("");

                    return true;
                }
                else
                {
                    DebugVideo("[FAIL] Can't add " + strShortName + " Crossbar to graph");
                    DebugVideo("-> " + DsError.GetErrorText(hr));
                    DebugVideo("");

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
            if (_xBar != null)
            {
                if ((_strCrossVideo.Length > 0) || (_strCrossAudio.Length > 0))
                {
                    string strXBarChange = "";
                    if (_strCrossVideo.Length > 0 && _strCrossVideo != "none")
                    {
                        DebugVideo("check cross video " + _strCrossVideo);

                        strXBarChange = findCrossbarSettings(_strCrossVideo, "");
                        DebugVideo("Change crossbar command (video): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _strCrossVideo = "";
                    }

                    if (_strCrossAudio.Length > 0 && _strCrossAudio != "none")
                    {
                        DebugVideo("check cross audio " + _strCrossAudio);

                        strXBarChange = findCrossbarSettings("", _strCrossAudio);
                        DebugVideo("Change crossbar command (audio): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _strCrossAudio = "";
                    }
                }
            }
        }

        private string findCrossbarSettings(string strVideo, string strAudio)
        {
            DebugVideo("find crossbar Settings " + strVideo + " / " + strAudio);
            string strReturn = "";
            int intType = 0;
            int intPin = 0;

            if (listCrossbarInput.Count == 0) FindCrossbarOutput(false); 
            if (listCrossbarInput.Count > 0)
            {
                for (int intCount = 0; intCount < listCrossbarInput.Count; intCount++)
                {
                    if (strVideo.ToLower() == listCrossbarInput[intCount].ToLower()) { intType = 0; intPin = intCount; }
                    if (strAudio.ToLower() == listCrossbarInput[intCount].ToLower()) { intType = 1; intPin = intCount; }
                }
            }

            strReturn = intType.ToString() + ", " + intPin.ToString();
            return strReturn;
        }

        public void changeCrossbarInput(string strInput)
        {
            DebugVideo("[changeCrossbarInput] " + strInput);
            int hr = 0;
            if (_xBar != null)
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
                DebugVideo("intPinType:" + intPinType);
                DebugVideo("intPinID:" + intPinID);

                hr = _xBar.Route(intPinType, intPinID);
                if (hr != 0) { DebugVideo("[ERR] " + DsError.GetErrorText(hr)); }
            }
            else { DebugVideo("xbar null " + strInput); }
        }

        private void FindCrossbarOutput(bool boolReturn)
        {
            if (_xBar != null)
            {
                listCrossbarInput = new List<string>();

                var inPin = 0;
                var outPin = 0;

                _xBar.get_PinCounts(out inPin, out outPin);

                for (var intCount = 0; intCount < outPin; intCount++)
                {
                    int intRouted;
                    int intPinId;
                    PhysicalConnectorType pinType;

                    _xBar.get_CrossbarPinInfo(true, intCount, out intPinId, out pinType);
                    _xBar.get_IsRoutedTo(intCount, out intRouted);

                    listCrossbarInput.Add(pinType.ToString());

                    DebugVideo(intCount + " / " + pinType);
                }
            }
            else { DebugVideo("No crossbar found"); }
        }

        #endregion

        private void createSmartTee(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            int hr = 0;
            DebugVideo("");
            DebugVideo("Creating SmartTee Preview Filter");

            IBaseFilter pSmartTee2 = (IBaseFilter)new SmartTee();
            hr = _graph.AddFilter(pSmartTee2, "Smart Tee");
            DebugVideo(DsError.GetErrorText(hr)); 
            DebugVideo("");

            listPin(pSmartTee2);
            strPreviewIn = assumePinIn("Input");
            strPreviewOut = assumePinOut("Preview");

            DebugVideo("");
            DebugVideo("***   Connect " + strDevice + " (" + strPinOut + ") to SmartTee Preview Filter (" + strPreviewIn + ")");
            hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pSmartTee2, strPreviewIn), null);
            if (hr == 0)
            {
                DebugVideo("[OK] Connected " + strDevice + " to SmartTee Preview Filter");
                strDevice = "SmartTee Preview Filter";
                pRen = pSmartTee2;
                strPinOut = strPreviewOut;
            }
            else
            {
                DebugVideo("[NG] cant Connect " + strDevice + " to Preview Filter. Attempting to continue without preview");
                DebugVideo("-> " + DsError.GetErrorText(hr));
            }
        }

        private void createSampleGrabber(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll

            int hr = 0;
            DebugVideo("");
            DebugVideo("Creating SampleGrabber");

            //add SampleGrabber
            IBaseFilter pSampleGrabber = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber));
            hr = _graph.AddFilter(pSampleGrabber, "SampleGrabber");
            DebugVideo("-> " + DsError.GetErrorText(hr));

            listPin(pSampleGrabber);
            string strSampleIn = assumePinIn("Input");
            string strSampleOut = assumePinOut("Output");

            DebugVideo("Set samplegrabber resolution feed");
            if (_listVideoRes.Count > 0)
            {

                hr = ((ISampleGrabber)pSampleGrabber).SetMediaType(_listVideoRes[_intVideoResolution]);
                DebugVideo("-> " + DsError.GetErrorText(hr));
            }
            else
                DebugVideo("[ERR] failure in video resolution list");

            DebugVideo("");
            DebugVideo("***   Connect " + strDevice + " (" + strPinOut + ") to SampleGrabber (" + strSampleIn + ")");
            //hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pSampleGrabber, strPreviewIn), null);
            hr = _graph.ConnectDirect(GetPin(pRen, "Capture"), GetPin(pSampleGrabber, "Input"), null);
            if (hr == 0)
            {
                SampleGrabberCallback cb = new SampleGrabberCallback();
                cb.GetForm1Handle(_frmMain);

                ISampleGrabber sampleGrabber = (ISampleGrabber)pSampleGrabber;
                sampleGrabber.SetCallback(cb, 1);

                DebugVideo("[OK] Connected " + strDevice + " to SampleGrabber");
                strDevice = "Sample Grabber";
                pRen = pSampleGrabber;
                strPinOut = strSampleOut;
            }
            else
            {
                DebugVideo("[NG] Cant connect SampleGrabber to video Capture feed. Attempting to continue.");
                DebugVideo("-> " + DsError.GetErrorText(hr));
            }
        }

        private void createAVIRender(ref string strAVIin, ref string strAVIout, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            int hr = 0;
            DebugVideo("");
            DebugVideo("Creating AVI renderer");
            IBaseFilter pAVIDecompressor = (IBaseFilter)new AVIDec();
            hr = _graph.AddFilter(pAVIDecompressor, "AVI Decompressor");
            DebugVideo("-> " + DsError.GetErrorText(hr));

            listPin(pAVIDecompressor);
            strAVIin = assumePinIn("XForm");
            strAVIout = assumePinOut("XForm");

            DebugVideo("");
            DebugVideo("***   Connect " + strDevice + " (" + strPinOut + ") to AVI Decompressor (" + strAVIin + ")");
            hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pAVIDecompressor, strAVIin), null);
            if (hr == 0)
            {
                DebugVideo("[OK] Connected " + strDevice + " to AVI Decompressor");
                pRen = pAVIDecompressor;
                strDevice = "AVI Decompressor";
                strPinOut = strAVIout;
            }
            else
            {
                DebugVideo("[FAIL] Can't connected " + strDevice + " to AVI Decompressor. May interrupt operation");

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
            DebugVideo("[3]***   Attaching running graph to preview window");

            int hr = 0;

            _boolPreviewFail = false;
            try
            {
                DebugVideo("-> putOwner");

                hr = _videoPreview.put_Owner(_previewWindow);
                if (hr != 0) { DebugVideo("-> " + DsError.GetErrorText(hr)); }

                DebugVideo("-> putWindowStyle");
                hr = _videoPreview.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                if (hr != 0) { DebugVideo("-> " + DsError.GetErrorText(hr)); }

                if (_videoWindow != null)
                {
                    DebugVideo("-> setBounds (" + _previewBounds.X.ToString() + ", " + _previewBounds.Y.ToString() + ")");
                    //Point ptReturn = frmMain.setVideoWindowBounds();
                    DebugVideo("-> setWindowPosition");
                    _videoPreview.SetWindowPosition(0, 0, _previewBounds.X, _previewBounds.Y);
                }
                DebugVideo("-> putVisible");
                hr = _videoPreview.put_Visible(OABool.True);
                if (hr != 0) { DebugVideo("-> " + DsError.GetErrorText(hr)); }
            }
            catch (Exception e)
            {
                DebugVideo("[ERR] *setupVideoWindow* " + e.ToString());
                _boolPreviewFail = true;
                return;
            }
        }

        public void setupVideoWindow()
        {
            DebugVideo("[0]***   Attaching running graph to video window");

            int hr = 0;

            IntPtr videoHandle = _frmMain.returnVideoHandle();

            _boolVideoFail = false;
            try
            {
                DebugVideo("-> putOwner");
                hr = _videoWindow.put_Owner(videoHandle);
                if (hr != 0) { DebugVideo("-> " + DsError.GetErrorText(hr)); }

                DebugVideo("-> putWindowStyle");
                hr = _videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                if (hr != 0) { DebugVideo("-> " + DsError.GetErrorText(hr)); }

                if (_videoWindow != null)
                {
                    DebugVideo("-> setBounds");
                    Point ptReturn = _frmMain.setVideoWindowBounds();
                    DebugVideo("-> setWindowPosition");
                    _videoWindow.SetWindowPosition(0, 0, ptReturn.X, ptReturn.Y);
                }
                DebugVideo("-> putVisible");
                hr = _videoWindow.put_Visible(OABool.True);
                if (hr != 0) { DebugVideo("-> " + DsError.GetErrorText(hr)); }
            }
            catch (Exception e)
            {
                DebugVideo("[ERR] *setupVideoWindow* " + e.ToString());
                _boolVideoFail = true;
                return;
            }
        }

        public void checkVideoOutput()
        {
            //Reruns the graph once, find this needs to happen after quick resolution changes (PS3)
            if (_boolRestartGraph && !_boolVideoFail)           
            {
                _boolRestartGraph = false;
                DebugVideo("[3] Update graph");
                runGraph();
            }

            if (!_boolVideoFail && _system.boolAutoSetCaptureResolution)
                checkVideoResolution();

            if (_boolRestartGraph)
            {
                if (_intRestartGraph > 0) 
                    _intRestartGraph--; 
                else 
                { 
                    DebugVideo("[3] Restart graph"); 
                    _boolRestartGraph = false; 
                    _boolVideoFail = false; 
                    runGraph(); 
                }
            }
        }

        //Compares video capture resolution with video out resolution, adjusts graph if needed
        private void checkVideoResolution()
        {
            int intVideoWidth = 0;
            int intVideoHeight = 0;
            int intLineCount = 0;

            if (_iVideoDef != null)
                _iVideoDef.GetVideoSize(out intVideoWidth, out intVideoHeight);

            if (_iamAvd != null)
                _iamAvd.get_NumberOfLines(out intLineCount);

            int intGetHeight = 0;
            int intGetWidth = 0;


            //May need to point directly to imgDisplay
            if (_iVideoWindow != null)
            {
                _iVideoWindow.get_Height(out intGetHeight);
                _iVideoWindow.get_Width(out intGetWidth);
            }

            if (_IsChangedDisplayResolution)
            {
                _IsChangedDisplayResolution = false;
                runGraph();
            }

            if (intLineCount > 0)
            {
                _frmMain.intLineSample = intLineCount;
                //frmMain.Text = intLineCount.ToString();

                if (_boolBuildingGraph == false)
                {
                    if (!_boolRerunGraph)
                    {
                        if (_boolInitializeGraph == true)           //Do another swap if running on PS3. Counters freezing display
                        {
                            _boolInitializeGraph = false;
                            if (intLineCount == 720)
                            {
                                _intVideoResolution = 0;
                                DebugVideo("[1] Change res 720");
                                runGraph();
                            }
                        }

                        //Compares video output to display output
                        if (intLineCount != intVideoHeight)
                        {
                            _boolBuildingGraph = true;
                            if (intLineCount == 720) { _boolRerunGraph = true; DebugVideo("[0] &FindMe& Rerun graph * 720p"); }
                            _intVideoResolution = 0;
                            if (_boolVideoFail == false)
                            {
                                DebugVideo("[1] Change Res : " + intLineCount.ToString());
                                
                                _system.autoChangeRes(intLineCount);

                                runGraph();
                            }
                        }
                    }
                    else
                    {
                        //Forces a second display change if moving to 720 (mainly PS3 needs this)
                        if (intLineCount == 720)
                        {
                            DebugVideo("[1] Rerun Graph : 720");
                            _boolRerunGraph = false;        
                            runGraph();
                        }
                    }
                }
            }
        }

        public void ForceRebuildAfterResolution()
        {
            _IsChangedDisplayResolution = true;
        }

        public void CloseGraph()
        {
            DebugVideo("[0]");
            DebugVideo("[TRY] Gracefully closing graph");

            if (_mediaControl != null) _mediaControl.StopWhenReady();

            _graph = null;
            _mediaControl = null;
            _mediaEvent = null;
            _videoWindow = null;
            _iVideoDef = null;
            _iVideoWindow = null;
            _iamAvd = null;
            _xBar = null;

            DebugVideo("[OK] close ok");
        }
        #endregion
    }

    public class SampleGrabberCallback : ISampleGrabberCB
    {
        private Form1 _form1;
        private string _strFpsCheck;
        private int _intSampleFrame;

        public void GetForm1Handle(Form1 inForm) { _form1 = inForm; }

        public int BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
        {
            if (_strFpsCheck != DateTime.Now.ToString("ss"))
            {
                _form1.intSampleFPS = _intSampleFrame;
                _intSampleFrame = 0;
                _strFpsCheck = DateTime.Now.ToString("ss");
            }
            else
            {
                _intSampleFrame++;
            }

            return 0;
        }
     
        public int SampleCB(double sampleTime, IMediaSample pSample)
        {
            return 0;
        }
    }
}
