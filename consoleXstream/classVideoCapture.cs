using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DirectShowLib;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing;

namespace consoleXstream
{
    public class classVideoCapture
    {
        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
 
        private Form1 frmMain;
        private classSystem system;

        public classVideoCapture(Form1 mainForm) { frmMain = mainForm; }

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

        public IAMCrossbar XBar = null;

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

        private IGraphBuilder _graph = null;
        private IMediaControl _mediaControl = null;
        private IMediaEvent _mediaEvent = null;
        private IVideoWindow _videoWindow = null;
        private IAMAnalogVideoDecoder _iamAVD = null;
        private IBasicVideo _iVideoDef = null;
        private IVideoWindow _iVideoWindow = null;
        private IVideoWindow _videoPreview = null;

        private async Task WriteTextAsync(string strWrite)
        {
            strWrite = strWrite.Trim();
            string strCurrentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            StreamWriter txtOut = new StreamWriter("video.log", true);
            if (strWrite.Length > 0)
                strWrite = strCurrentTime + " - " + strWrite;
            await txtOut.WriteLineAsync(strWrite);
            txtOut.Close();
        }

        public async void debugVideo(string strWrite)
        {
            if (system != null)
            {
                int intLevel = _intLastDebugLevel;
                int intSysLevel = system.intDebugLevel;

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

        public void getSystemHandle(classSystem inSystem)
        {
            system = inSystem;
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
            if (system.checkUserSetting("Crossbar") == "true") boolUseCrossbar = true; else boolCrossbar = false;
            if (system.checkUserSetting("Preview") == "true") boolCreateSmartTee = true; else boolCreateSmartTee = false;
            if (system.checkUserSetting("AVIRender") == "true") boolCreateAVIRender = true; else boolCreateAVIRender = false;
            
            //Check as user setting
            boolSampleGrabber = true;

            strVideoPin = system.checkUserSetting("crossbarVideoPin");
            strAudioPin = system.checkUserSetting("crossbarAudioPin");

            setVideoCaptureDevice(system.checkUserSetting("VideoCaptureDevice"));

            if (boolCreateSmartTee)
                debugVideo("Using smartTee");
            else
                debugVideo("SmartTee disabled");

            if (boolCreateAVIRender)
                debugVideo("Using AVI Renderer");
            else
                debugVideo("AVI Rendered disabled");
        }

        #region Video Capture Information
        public void setPreviewWindow(bool boolSet)
        {
            _boolShowPreviewWindow = boolSet;
        }

        public void setVideoCaptureDevice(string strTitle)
        {
            debugVideo("[0] Looking for " + strTitle);
            int intIndex = listVideoCaptureName.FindIndex(x => x.Equals(strTitle, StringComparison.OrdinalIgnoreCase));
            debugVideo("[0] DeviceID: " + intIndex.ToString());
            if (intIndex > -1)
                _intVideoDevice = intIndex;
            else
                debugVideo("[0] [ERR] Cant find " + strTitle + " VCD");
        }

        //Caches currently connected video capture devices
        private void findVideoDevices()
        {
            listVideoCapture = new List<string>();
            listVideoCaptureName = new List<string>();


            debugVideo("[0] Listing video capture devices");
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
                    debugVideo("->" + strTitle);
                }
            }
            else
            {
                listVideoCapture.Add("*NF*");
                listVideoCaptureName.Add("");
                debugVideo("[Err] No capture devices found");
            }
            debugVideo("");
        }

        private void findVideoResolution()
        {
            try
            {
                debugVideo("[0] Find video device resolution");
                
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
                        debugVideo("->" + strRes);
                    }
                }
                
                system.strCurrentResolution = _listVideoResolution[_intVideoResolution];

                debugVideo("");
            }
            catch (Exception e)
            {
                debugVideo("[ERR] fail find video resolution : " + e.ToString());
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
            debugVideo("[0] Find audio devices");
            _listAudioDevice = new List<string>();

            DsDevice[] devObject = DsDevice.GetDevicesOfCat(FilterCategory.AudioRendererCategory);

            for (int intCount = 0; intCount < devObject.Length; intCount++)
            {
                if (_listAudioDevice.IndexOf(devObject[intCount].Name) == -1)
                {
                    _listAudioDevice.Add(devObject[intCount].Name.ToString());
                    debugVideo("->" + devObject[intCount].Name);
                    //If nothing set, assume this
                    if (_intAudioDevice == -1)
                    {
                        if (devObject[intCount].Name == "Default WaveOut Device")
                            _intAudioDevice = intCount;
                    }
                }
            }

            debugVideo("");
        }

        public string showCrossbarOutput(int intID, string strType)
        {
            string strReturn = "";
            if (listCrossbarInput.Count == 0) { findCrossbarOutput(false); }
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
            debugVideo("[0] Build capture graph");
            if (_intVideoDevice > -1 && _intVideoDevice < listVideoCaptureName.Count)
            {
                int hr = 0;

                _boolBuildingGraph = true;
                debugVideo("Using : " + listVideoCaptureName[_intVideoDevice]);
                debugVideo("");

                if (_mediaControl != null) _mediaControl.StopWhenReady();
                if (_listVideoResolution.Count == 0) { findVideoResolution();  }

                _graph = null;
                _mediaControl = null;
                _mediaEvent = null;
                _videoWindow = null;
                _iVideoDef = null;
                _iVideoWindow = null;
                XBar = null;

                _graph = (IGraphBuilder)new FilterGraph();

                if (buildGraph())
                {
                    if (!_boolShowPreviewWindow)
                        setupVideoWindow();
                    else
                        setupPreviewWindow();

                    _mediaControl = (IMediaControl)_graph;
                    _mediaEvent = (IMediaEvent)_graph;

                    debugVideo("");
                    debugVideo("Run compiled graph");
                    hr = _mediaControl.Run();
                    debugVideo("[2] " + DsError.GetErrorText(hr));

                    //TODO: Check for major / minor errors before declaring video run
                    boolActiveVideo = true;
                }

                _boolBuildingGraph = false;

                if (XBar != null)
                    if (listCrossbarInput.Count == 0)
                        findCrossbarOutput(false);
                
                if (_boolRestartGraph)
                    _intRestartGraph = 3;
            }
            else
                debugVideo("[ERR] Unknown capture device");
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

                debugVideo("[2] VCD: " + strVideoDevice);
                debugVideo("[2] VCDID: " + strShortName);
                debugVideo("[2] RES: " + _listVideoResolution[_intVideoResolution]);
                debugVideo("[2] AOD: " + _listAudioDevice[_intAudioDevice]);
                
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
                debugVideo("");
                debugVideo("[0] Create new graph");
                ICaptureGraphBuilder2 pBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                hr = pBuilder.SetFiltergraph(_graph);
                debugVideo("[2] [OK] " + DsError.GetErrorText(hr));
                debugVideo("");

                _videoWindow = (IVideoWindow)_graph;            //Open the window

                //Primary Capture Device
                IBaseFilter pCaptureDevice = setFilter(FilterCategory.VideoInputDevice, strVideoDevice, out strTempOut);
                debugVideo("");
                if (pCaptureDevice == null)
                {
                    debugVideo("[ERR] Cant create capture device. Graph cannot continue");
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

                debugVideo("[0]");
                debugVideo("<Video Out>" + strCaptureVideoOut);
                debugVideo("<Audio Out>" + strCaptureAudioOut);
                debugVideo("<Video In>" + strCaptureVideoIn);
                debugVideo("<Audio In>" + strCaptureAudioIn);
                debugVideo("");

                _iamAVD = pCaptureDevice as IAMAnalogVideoDecoder;

                //Create user crossbar if needed
                if (boolUseCrossbar == true)
                    if (createCrossbar(ref strCrossAudioOut, ref strCrossVideoOut, strCaptureVideoIn, strCaptureAudioIn, strShortName, pCaptureDevice))
                        checkCrossbar();

                debugVideo("");

                //Set resolution
                debugVideo("[0] Checking capture resolution");
                if (_intVideoResolution == 0)       
                    lookupAutoResolution();

                if (_intVideoResolution > 0)
                    setVideoResolution(pCaptureDevice, strCaptureVideoOut);
                else
                    debugVideo("[0] [WARN] Cant find capture resolution - no input or unknown resolution type");

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
                debugVideo("");
                debugVideo("[0]***   Create Video Renderer");
                Guid CLSID_ActiveVideo = new Guid("{B87BEB7B-8D29-423F-AE4D-6582C10175AC}");

                IBaseFilter pVideoRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ActiveVideo));
                hr = _graph.AddFilter(pVideoRenderer, "Video Renderer");
                if (hr == 0) { debugVideo("[1] [OK] Created video renderer"); }
                else
                {
                    debugVideo("[1] [FAIL] Cant create video renderer");
                    debugVideo("-> " + DsError.GetErrorText(hr));
                }

                debugVideo("");
                debugVideo("***   Listing Video Renderer pins");
                listPin(pVideoRenderer);
                strVideoIn = assumePinIn("Input");
                debugVideo("<Video>" + strVideoIn);
                debugVideo("");

                debugVideo("***   Connect AVI Decompressor (" + strPinOut + ") to Video Renderer (" + strVideoIn + ")");
                hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pVideoRenderer, strVideoIn), null);
                if (hr == 0) { debugVideo("[OK] Connected AVI to video renderer"); }
                else
                {
                    debugVideo("[FAIL] Can't connect AVI to video renderer");
                    debugVideo("-> " + DsError.GetErrorText(hr));
                }

                _iVideoDef = pVideoRenderer as IBasicVideo;
                _iVideoWindow = pVideoRenderer as IVideoWindow;
                
                //Audio device
                if (_intAudioDevice > -1 && _intAudioDevice < _listAudioDevice.Count)
                {
                    _intDeviceID = 0;               //Dont need multiple devices, set back to 0

                    debugVideo("[0]");
                    debugVideo("***   Create " + _listAudioDevice[_intAudioDevice] + " audio device");
                    IBaseFilter pAudio = null;

                    pAudio = setFilter(FilterCategory.AudioRendererCategory, _listAudioDevice[_intAudioDevice], out strTempOut);
                    hr = _graph.AddFilter(pAudio, "Audio Device");
                    debugVideo("-> " + DsError.GetErrorText(hr));

                    if (pAudio != null)
                    {
                        debugVideo("[1]");
                        debugVideo("***   Listing " + _listAudioDevice[_intAudioDevice] + " pins");

                        listPin(pAudio);
                        strAudioIn = assumePinIn("Audio");
                        debugVideo("<Audio>" + strAudioIn);
                        debugVideo("");

                        //connect Capture Device and Audio Device
                        debugVideo("***   Connect " + strVideoDevice + " (" + strCaptureAudioOut + ") to " + _listAudioDevice[_intAudioDevice] + " [Audio] (" + strAudioIn + ")");
                        hr = _graph.ConnectDirect(GetPin(pCaptureDevice, strCaptureAudioOut), GetPin(pAudio, strAudioIn), null);
                        debugVideo("-> " + DsError.GetErrorText(hr));
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
                debugVideo("[3] set resolution " + _listVideoResolution[_intVideoResolution]);
                hr = ((IAMStreamConfig)GetPin(pCaptureDevice, strCaptureVideoOut)).SetFormat(_listVideoRes[_intVideoResolution]);
                if (hr == 0)
                {
                    debugVideo("[OK] Set resolution " + _listVideoResolution[_intVideoResolution]);
                    _intCurrentVideoResolution = _intVideoResolution;
                    _strCurrentResolution = _listVideoResolution[_intVideoResolution];

                    if (_strCurrentResolution.IndexOf('[') > -1)
                        _strCurrentResolution = _strCurrentResolution.Substring(0, _strCurrentResolution.IndexOf('['));
                }
                else
                {
                    debugVideo("[NG] Can't set resolution " + _listVideoResolution[_intVideoResolution]);
                    debugVideo("-> " + DsError.GetErrorText(hr));
                }
            }
            else
                debugVideo("[0] [ERR] cant find resolution " + _intVideoResolution.ToString());
        }

        //Finds the closest resolution from device output
        private void lookupAutoResolution()
        {
            if (_iamAVD != null && _intSetResolution == 0)
            {
                int intLineCount = 0;
                _iamAVD.get_NumberOfLines(out intLineCount);

                if (intLineCount > 0)
                {
                    string strLineCount = intLineCount.ToString();
                    system.autoChangeRes(intLineCount);

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
            debugVideo("");
            debugVideo("[2]  Create " + strTitle + " (" + strCategory + ")");

            pFilter = createFilter(category, strTitle, out strTempOut);
            intHR = _graph.AddFilter(pFilter, strTempOut);
            if (intHR == 0)
            {
                debugVideo("[OK] Added " + strTitle + " (" + strCategory + ") to graph");
            }
            else
            {
                debugVideo("[FAIL] Can't add " + strTitle + " (" + strCategory + ") to graph");
                debugVideo("-> " + DsError.GetErrorText(intHR));
            }

            return pFilter;
        }

        private IBaseFilter createFilter(Guid category, string strName, out string strTitle)
        {
            debugVideo("[3] Create filter: name>" + strName);
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

                debugVideo("FilterName: " + strName);
                debugVideo("DevID: " + strID);
                try
                {
                    intID = Convert.ToInt32(strID);
                }
                catch { }
                debugVideo("Confirm DevID: " + intID.ToString());
            }

            int intDevID = 0;

            DsDevice[] devices = DsDevice.GetDevicesOfCat(category);
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(category))
            {
                string strDevice = device.Name.ToLower();
                debugVideo("device>" + strDevice);
                if (strDevice.IndexOf(strName) > -1 || strDevice.ToLower() == "oem crossbar")
                {
                    bool boolCreate = true;
                    debugVideo("TargetID=" + intID.ToString() + " [] + DevID=" + intDevID.ToString());

                    if (intDevID == intID) 
                    { 
                        boolCreate = true;
                        _intDeviceID = intID;
                    } 
                    else 
                        boolCreate = false; 

                    if (boolCreate == true)
                    {
                        debugVideo("SetDeviceID (" + intDevID.ToString() + ")");
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
                    else { debugVideo(intDevID.ToString() + " skipped"); }
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
            debugVideo("[3] listing Pins [" + pOut.achName + "]");

            try
            {
                IEnumPins epins;
                int hr = filter.EnumPins(out epins);
                if (hr < 0) { debugVideo("[0] [NG] Can't find pins"); }
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
                    debugVideo("-" + _listPinIn[intCount] + " (Input)");
                }

                for (int intCount = 0; intCount < _listPinOut.Count; intCount++)
                {
                    debugVideo("-" + _listPinOut[intCount] + " (Output)");
                }
            }
            catch
            {
                debugVideo("[0] [FAIL] Error listing pins");
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

            debugVideo("");
            debugVideo("[1] Looking for crossbar " + _intDeviceID);

            IBaseFilter pCrossbar = createFilter(FilterCategory.AMKSCrossbar, strShortName, out strTempOut);
            if (strTempOut.ToLower() == "*nf*")
            {
                debugVideo("[FAIL] No crossbar found. Will not interrupt operation");
                return false;
            }
            else
            {
                hr = _graph.AddFilter(pCrossbar, strTempOut);
                if (hr == 0)
                {
                    debugVideo("[OK] Create crossbar");
                    boolCrossbar = true;

                    listPin(pCrossbar);
                    strCrossAudioOut = assumePinOut("Audio");
                    strCrossVideoOut = assumePinOut("Video");
                    debugVideo("<Audio>" + strCrossAudioOut);
                    debugVideo("<Video>" + strCrossVideoOut);

                    debugVideo("");
                    debugVideo("Connect Crossbar (" + strCrossVideoOut + ") to Capture (" + strCaptureVideoIn + ")");

                    hr = _graph.ConnectDirect(GetPin(pCrossbar, strCrossVideoOut), GetPin(pCaptureDevice, strCaptureVideoIn), null);
                    debugVideo("Crossbar Video -> " + DsError.GetErrorText(hr));

                    hr = _graph.ConnectDirect(GetPin(pCrossbar, strCrossAudioOut), GetPin(pCaptureDevice, strCaptureAudioIn), null);
                    debugVideo("Crossbar Audio -> " + DsError.GetErrorText(hr));

                    XBar = (IAMCrossbar)pCrossbar;
                    debugVideo("");

                    return true;
                }
                else
                {
                    debugVideo("[FAIL] Can't add " + strShortName + " Crossbar to graph");
                    debugVideo("-> " + DsError.GetErrorText(hr));
                    debugVideo("");

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
            if (XBar != null)
            {
                if ((_strCrossVideo.Length > 0) || (_strCrossAudio.Length > 0))
                {
                    string strXBarChange = "";
                    if (_strCrossVideo.Length > 0 && _strCrossVideo != "none")
                    {
                        debugVideo("check cross video " + _strCrossVideo);

                        strXBarChange = findCrossbarSettings(_strCrossVideo, "");
                        debugVideo("Change crossbar command (video): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _strCrossVideo = "";
                    }

                    if (_strCrossAudio.Length > 0 && _strCrossAudio != "none")
                    {
                        debugVideo("check cross audio " + _strCrossAudio);

                        strXBarChange = findCrossbarSettings("", _strCrossAudio);
                        debugVideo("Change crossbar command (audio): " + strXBarChange);
                        changeCrossbarInput(strXBarChange);
                        _strCrossAudio = "";
                    }
                }
            }
        }

        private string findCrossbarSettings(string strVideo, string strAudio)
        {
            debugVideo("find crossbar Settings " + strVideo + " / " + strAudio);
            string strReturn = "";
            int intType = 0;
            int intPin = 0;

            if (listCrossbarInput.Count == 0) findCrossbarOutput(false); 
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
            debugVideo("[changeCrossbarInput] " + strInput);
            int hr = 0;
            if (XBar != null)
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
                debugVideo("intPinType:" + intPinType);
                debugVideo("intPinID:" + intPinID);

                hr = XBar.Route(intPinType, intPinID);
                if (hr != 0) { debugVideo("[ERR] " + DsError.GetErrorText(hr)); }
            }
            else { debugVideo("xbar null " + strInput); }
        }

        private void findCrossbarOutput(bool boolReturn)
        {
            if (XBar != null)
            {
                listCrossbarInput = new List<string>();

                int inPin = 0;
                int outPin = 0;
                int hr = 0;

                XBar.get_PinCounts(out inPin, out outPin);

                for (int intCount = 0; intCount < outPin; intCount++)
                {
                    int intRouted = -1;
                    int intPinID = 0;
                    PhysicalConnectorType pinType;

                    hr = XBar.get_CrossbarPinInfo(true, intCount, out intPinID, out pinType);
                    hr = XBar.get_IsRoutedTo(intCount, out intRouted);

                    listCrossbarInput.Add(pinType.ToString());

                    debugVideo(intCount.ToString() + " / " + pinType.ToString());
                }
            }
            else { debugVideo("No crossbar found"); }
        }

        #endregion

        private void createSmartTee(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            int hr = 0;
            debugVideo("");
            debugVideo("Creating SmartTee Preview Filter");

            IBaseFilter pSmartTee2 = (IBaseFilter)new SmartTee();
            hr = _graph.AddFilter(pSmartTee2, "Smart Tee");
            debugVideo(DsError.GetErrorText(hr)); 
            debugVideo("");

            listPin(pSmartTee2);
            strPreviewIn = assumePinIn("Input");
            strPreviewOut = assumePinOut("Preview");

            debugVideo("");
            debugVideo("***   Connect " + strDevice + " (" + strPinOut + ") to SmartTee Preview Filter (" + strPreviewIn + ")");
            hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pSmartTee2, strPreviewIn), null);
            if (hr == 0)
            {
                debugVideo("[OK] Connected " + strDevice + " to SmartTee Preview Filter");
                strDevice = "SmartTee Preview Filter";
                pRen = pSmartTee2;
                strPinOut = strPreviewOut;
            }
            else
            {
                debugVideo("[NG] cant Connect " + strDevice + " to Preview Filter. Attempting to continue without preview");
                debugVideo("-> " + DsError.GetErrorText(hr));
            }
        }

        private void createSampleGrabber(ref string strPreviewIn, ref string strPreviewOut, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll

            int hr = 0;
            debugVideo("");
            debugVideo("Creating SampleGrabber");

            //add SampleGrabber
            IBaseFilter pSampleGrabber = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_SampleGrabber));
            hr = _graph.AddFilter(pSampleGrabber, "SampleGrabber");
            debugVideo("-> " + DsError.GetErrorText(hr));

            listPin(pSampleGrabber);
            string strSampleIn = assumePinIn("Input");
            string strSampleOut = assumePinOut("Output");

            debugVideo("Set samplegrabber resolution feed");
            if (_listVideoRes.Count > 0)
            {

                hr = ((ISampleGrabber)pSampleGrabber).SetMediaType(_listVideoRes[_intVideoResolution]);
                debugVideo("-> " + DsError.GetErrorText(hr));
            }
            else
                debugVideo("[ERR] failure in video resolution list");

            debugVideo("");
            debugVideo("***   Connect " + strDevice + " (" + strPinOut + ") to SampleGrabber (" + strSampleIn + ")");
            //hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pSampleGrabber, strPreviewIn), null);
            hr = _graph.ConnectDirect(GetPin(pRen, "Capture"), GetPin(pSampleGrabber, "Input"), null);
            if (hr == 0)
            {
                SampleGrabberCallback cb = new SampleGrabberCallback();
                cb.getForm1Handle(frmMain);

                ISampleGrabber sampleGrabber = (ISampleGrabber)pSampleGrabber;
                sampleGrabber.SetCallback(cb, 1);

                debugVideo("[OK] Connected " + strDevice + " to SampleGrabber");
                strDevice = "Sample Grabber";
                pRen = pSampleGrabber;
                strPinOut = strSampleOut;
            }
            else
            {
                debugVideo("[NG] Cant connect SampleGrabber to video Capture feed. Attempting to continue.");
                debugVideo("-> " + DsError.GetErrorText(hr));
            }
        }

        private void createAVIRender(ref string strAVIin, ref string strAVIout, ref string strDevice, ref string strPinOut, ref IBaseFilter pRen)
        {
            int hr = 0;
            debugVideo("");
            debugVideo("Creating AVI renderer");
            IBaseFilter pAVIDecompressor = (IBaseFilter)new AVIDec();
            hr = _graph.AddFilter(pAVIDecompressor, "AVI Decompressor");
            debugVideo("-> " + DsError.GetErrorText(hr));

            listPin(pAVIDecompressor);
            strAVIin = assumePinIn("XForm");
            strAVIout = assumePinOut("XForm");

            debugVideo("");
            debugVideo("***   Connect " + strDevice + " (" + strPinOut + ") to AVI Decompressor (" + strAVIin + ")");
            hr = _graph.ConnectDirect(GetPin(pRen, strPinOut), GetPin(pAVIDecompressor, strAVIin), null);
            if (hr == 0)
            {
                debugVideo("[OK] Connected " + strDevice + " to AVI Decompressor");
                pRen = pAVIDecompressor;
                strDevice = "AVI Decompressor";
                strPinOut = strAVIout;
            }
            else
            {
                debugVideo("[FAIL] Can't connected " + strDevice + " to AVI Decompressor. May interrupt operation");

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
            debugVideo("[3]***   Attaching running graph to preview window");

            int hr = 0;

            _boolPreviewFail = false;
            try
            {
                debugVideo("-> putOwner");

                hr = _videoPreview.put_Owner(_previewWindow);
                if (hr != 0) { debugVideo("-> " + DsError.GetErrorText(hr)); }

                debugVideo("-> putWindowStyle");
                hr = _videoPreview.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                if (hr != 0) { debugVideo("-> " + DsError.GetErrorText(hr)); }

                if (_videoWindow != null)
                {
                    debugVideo("-> setBounds (" + _previewBounds.X.ToString() + ", " + _previewBounds.Y.ToString() + ")");
                    //Point ptReturn = frmMain.setVideoWindowBounds();
                    debugVideo("-> setWindowPosition");
                    _videoPreview.SetWindowPosition(0, 0, _previewBounds.X, _previewBounds.Y);
                }
                debugVideo("-> putVisible");
                hr = _videoPreview.put_Visible(OABool.True);
                if (hr != 0) { debugVideo("-> " + DsError.GetErrorText(hr)); }
            }
            catch (Exception e)
            {
                debugVideo("[ERR] *setupVideoWindow* " + e.ToString());
                _boolPreviewFail = true;
                return;
            }
        }

        public void setupVideoWindow()
        {
            debugVideo("[0]***   Attaching running graph to video window");

            int hr = 0;

            IntPtr videoHandle = frmMain.returnVideoHandle();

            _boolVideoFail = false;
            try
            {
                debugVideo("-> putOwner");
                hr = _videoWindow.put_Owner(videoHandle);
                if (hr != 0) { debugVideo("-> " + DsError.GetErrorText(hr)); }

                debugVideo("-> putWindowStyle");
                hr = _videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                if (hr != 0) { debugVideo("-> " + DsError.GetErrorText(hr)); }

                if (_videoWindow != null)
                {
                    debugVideo("-> setBounds");
                    Point ptReturn = frmMain.setVideoWindowBounds();
                    debugVideo("-> setWindowPosition");
                    _videoWindow.SetWindowPosition(0, 0, ptReturn.X, ptReturn.Y);
                }
                debugVideo("-> putVisible");
                hr = _videoWindow.put_Visible(OABool.True);
                if (hr != 0) { debugVideo("-> " + DsError.GetErrorText(hr)); }
            }
            catch (Exception e)
            {
                debugVideo("[ERR] *setupVideoWindow* " + e.ToString());
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
                debugVideo("[3] Update graph");
                runGraph();
            }

            if (!_boolVideoFail && system.boolAutoSetCaptureResolution)
                checkVideoResolution();

            if (_boolRestartGraph)
            {
                if (_intRestartGraph > 0) 
                    _intRestartGraph--; 
                else 
                { 
                    debugVideo("[3] Restart graph"); 
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

            if (_iamAVD != null)
                _iamAVD.get_NumberOfLines(out intLineCount);

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
                frmMain.intLineSample = intLineCount;
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
                                debugVideo("[1] Change res 720");
                                runGraph();
                            }
                        }

                        //Compares video output to display output
                        if (intLineCount != intVideoHeight)
                        {
                            _boolBuildingGraph = true;
                            if (intLineCount == 720) { _boolRerunGraph = true; debugVideo("[0] &FindMe& Rerun graph * 720p"); }
                            _intVideoResolution = 0;
                            if (_boolVideoFail == false)
                            {
                                debugVideo("[1] Change Res : " + intLineCount.ToString());
                                
                                system.autoChangeRes(intLineCount);

                                runGraph();
                            }
                        }
                    }
                    else
                    {
                        //Forces a second display change if moving to 720 (mainly PS3 needs this)
                        if (intLineCount == 720)
                        {
                            debugVideo("[1] Rerun Graph : 720");
                            _boolRerunGraph = false;        
                            runGraph();
                        }
                    }
                }
            }
        }

        public void forceRebuildAfterResolution()
        {
            _IsChangedDisplayResolution = true;
        }

        public void closeGraph()
        {
            debugVideo("[0]");
            debugVideo("[TRY] Gracefully closing graph");

            if (_mediaControl != null) _mediaControl.StopWhenReady();

            _graph = null;
            _mediaControl = null;
            _mediaEvent = null;
            _videoWindow = null;
            _iVideoDef = null;
            _iVideoWindow = null;
            _iamAVD = null;
            XBar = null;

            debugVideo("[OK] close ok");
        }
        #endregion
    }

    public class SampleGrabberCallback : ISampleGrabberCB
    {
        private Form1 form1;
        private string _strFPSCheck;
        private int intSampleFrame;

        public void getForm1Handle(Form1 inForm) { form1 = inForm; }

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (_strFPSCheck != DateTime.Now.ToString("ss"))
            {
                form1.intSampleFPS = intSampleFrame;
                intSampleFrame = 0;
                _strFPSCheck = DateTime.Now.ToString("ss");
            }
            else
            {
                intSampleFrame++;
            }

            //form1.intSampleGrabberFrame++;
            return 0;
        }
     
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            return 0;
        }
    }
}
