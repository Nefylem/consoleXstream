using System.Collections.Generic;
using consoleXstream.Remap;

namespace consoleXstream.Config
{
    public class Configuration
    {
        private readonly Classes _class;
        
        public bool boolFPS { get; private set; }
        public bool boolHideMouse { get; private set; }

        public bool boolEnableMouse { get; private set; }
        public bool boolEnableKeyboard { get; private set; }
        public bool boolInternalCapture { get; private set; }

        public bool boolPS4ControllerMode { get; private set; }
        public bool boolNormalizeControls { get; private set; }

        public bool boolControllerMax { get; private set; }
        public bool boolTitanOne { get; private set; }
        public bool boolGIMX { get; private set; }
        public bool boolRemoteGIMX { get; private set; }
        public bool boolMcShield { get; private set; }
        public bool boolControlVJOY { get; private set; }
        public bool useTitanOneAPI { get; private set; }

        public bool boolBlockMenuButton { get; private set; }
        public bool boolUseRumble { get; private set; }

        public bool boolAutoSetResolution { get; private set; }
        public bool boolAutoSetCaptureResolution { get; private set; }
        public bool EnableGcmapi { get; private set; }
        public bool DisableTitanOneRetry { get; set; }
        
        public bool boolMenu { get; set; }
        public bool BoolStayOnTop { get; set; }
        public bool IsOverrideOnExit { get; set; }
        public bool IsVr { get; set; }
        public int VrVerticalOffset { get; set; }
        public int MouseMode { get; set; }


        public string strCaptureProfile { get; set; }
        public string strCurrentResolution { get; set; }
        public string strSetResolution { get; set; }
        public string TitanOneDevice { get; set; }

        private int _graphicsCardID;
        private int _refreshRateID;
        private int _displayResolutionID;
        private string _graphicsCard;
        private string _refreshRate;
        private string _displayResolution;
        private string _initialDisplay;
        public Keymap.KeyboardKeys keyDef ;
        public Keymap.KeyboardKeys keyAltDef;

        public Configuration(Form1 mainForm)
        {
            _class = new Classes(this);
            _class.DeclareClasses(mainForm);

            _class.Set.Title = new List<string>();
            _class.Set.Data = new List<string>();
        }

        public void GetClassHandles(VideoCapture.VideoCapture inVideo, Output.ControllerMax inMax, 
            Output.TitanOne.Write inTo, VideoResolution inVid)
        {
            _class.VideoCapture = inVideo;
            _class.ControllerMax = inMax;
            _class.TitanOne = inTo;
            _class.VideoResolution = inVid;
        }

        public void loadDefaults()
        {
            _class.Var.IsReadData = true;
            _class.Set.Add("InternalCapture", "true");
            _class.Set.Add("Crossbar", "true");
            _class.Set.Add("Preview", "true");
            _class.Set.Add("AVIRender", "true");

            _class.Set.Add("ControllerRumble", "true");
            _class.Set.Add("Keyboard", "true");
            _class.Set.Add("HideMouse", "true");
            boolEnableMouse = true;
            //useTitanOneAPI = true;
            BoolStayOnTop = true;
            boolAutoSetCaptureResolution = true;
            EnableGcmapi = true;
            VrVerticalOffset = 150;
            _class.Var.IsReadData = false;
        }

        public void checkUserSettings()
        {

            if (_class.Set.Check("keyboard").ToLower() == "true") boolEnableKeyboard = true;
            if (_class.Set.Check("mouse").ToLower() == "true") boolEnableMouse = true;
            if (_class.Set.Check("hidemouse").ToLower() == "true") boolHideMouse = true;

            if (_class.Set.Check("internalCapture").ToLower() == "true") boolInternalCapture = true;  
            if (_class.Set.Check("controllerrumble").ToLower() == "true") boolUseRumble = true;

            if (_class.Set.Check("ds4emulation").ToLower() == "true") boolPS4ControllerMode = true;
            if (_class.Set.Check("normalize").ToLower() == "true") boolNormalizeControls = true;

            if (_class.Set.Check("controllermax").ToLower() == "true") boolControllerMax = true;
            if (_class.Set.Check("titanone").ToLower() == "true") boolTitanOne = true;
            if (_class.Set.Check("showfps").ToLower() == "true") boolFPS = true;
            if (_class.Set.Check("stayontop").ToLower() == "true") BoolStayOnTop = true;
            if (_class.Set.Check("checkcaptureres").ToLower() == "true") boolAutoSetCaptureResolution = true;
            if (_class.Set.Check("AutoResolution").ToLower() == "true") boolAutoSetResolution = true;
            if (_class.Set.Check("VR_Video").ToLower() == "true") IsVr = true;
            if (_class.Set.Check("UseTitanOne").Length > 0) 
                TitanOneDevice = _class.Set.Check("UseTitanOne");
          
            _refreshRate = _class.Set.Check("RefreshRate");
            _displayResolution = _class.Set.Check("Resolution");

            if (!boolAutoSetResolution && (_refreshRate.Length > 0 || _displayResolution.Length > 0))
            {
                if (_displayResolution.Length == 0)
                    setDisplayRefresh(_refreshRate);
                else
                    setDisplayResolution(_displayResolution);
            }

            if (_class.Set.Check("CaptureResolution").Length > 0) changeResolution(_class.Set.Check("CaptureResolution"));

            strCaptureProfile = _class.Set.Check("CurrentProfile");

            if (_class.Main.boolIDE)
                boolHideMouse = false;
        }

        public void changeDS4Emulation()
        {
            boolPS4ControllerMode = !boolPS4ControllerMode;
            _class.Set.Add("DS4Emulation", boolPS4ControllerMode.ToString());
        }

        public void changeNormalizeGamepad()
        {
            boolNormalizeControls = !boolNormalizeControls;
            _class.Set.Add("Normalize", boolNormalizeControls.ToString());
        }

        public void changeControllerMax()
        {
            if (useTitanOneAPI)
            {
                changeControllerMax_TOAPI();
                return;
            }

            if (boolControllerMax && boolTitanOne)      //Stop infinite loops
                boolTitanOne = false;

            if (boolTitanOne) ChangeTitanOne();         //Disable if running


            boolControllerMax = !boolControllerMax;
            _class.Set.Add("ControllerMax", boolControllerMax.ToString());

            if (boolControllerMax)
                _class.ControllerMax.initControllerMax();
            else
                _class.ControllerMax.closeControllerMaxInterface();
        }

        public void changeControllerMax_TOAPI()
        {
            _class.TitanOne.Close();

            boolControllerMax = !boolControllerMax;
            _class.Set.Add("ControllerMax", boolControllerMax.ToString());

            if (boolControllerMax)
            {
                boolTitanOne = false;
                _class.Set.Add("TitanOne", boolTitanOne.ToString());
            }

            if (boolControllerMax)
            {
                _class.TitanOne.SetToInterface(Output.TitanOne.Define.DevPid.ControllerMax);
                _class.TitanOne.Initialize();
            }
        }

        public void ChangeTitanOne()
        {
            if (useTitanOneAPI)
            {
                changeTitanOne_TOAPI();
                return;
            }

            if (boolControllerMax && boolTitanOne)      //Stop infinite loops
                boolControllerMax = false;

            if (boolControllerMax) changeControllerMax();       //Disable if running

            boolTitanOne = !boolTitanOne;
            _class.Set.Add("TitanOne", boolTitanOne.ToString());

            if (boolTitanOne)
                _class.TitanOne.Initialize();
            else
                _class.TitanOne.Close();
        }

        public void changeTitanOne_TOAPI()
        {
            _class.TitanOne.Close();

            boolTitanOne = !boolTitanOne;
            _class.Set.Add("TitanOne", boolTitanOne.ToString());

            if (boolTitanOne)
            {
                boolControllerMax = false;
                _class.Set.Add("ControllerMax", boolControllerMax.ToString());
            }

            if (boolTitanOne)
            {
                _class.TitanOne.SetToInterface(Output.TitanOne.Define.DevPid.TitanOne);
                _class.TitanOne.Initialize();
            }
        }

        public void ChangeCrossbar()
        {
            var set = _class.Set.Check("Crossbar").ToLower();
            _class.Set.Add("Crossbar", set == "true" ? "false" : "true");
            _class.VideoCapture.LoadUserSettings();
            _class.VideoCapture.runGraph();
        }

        public void ChangeAviRender()
        {
            var set = _class.Set.Check("AVIRender").ToLower();
            _class.Set.Add("AVIRender", set == "true" ? "false" : "true");

            _class.VideoCapture.LoadUserSettings();
            _class.VideoCapture.runGraph();
        }

        public string getGraphicsCard()
        {
            if (_graphicsCard == null || _graphicsCard.Length == 0)
                _graphicsCard = _class.VideoResolution.GetVideoCard(_graphicsCardID);

            return _graphicsCard;
        }

        public string getRefreshRate()
        {
            if (_refreshRate == null || _refreshRate.Length == 0)
                _refreshRate = _class.VideoResolution.GetRefreshRate(_graphicsCardID);

            return _refreshRate;
        }

        public string getResolution()
        {
            if (_displayResolution == null || _displayResolution.Length == 0)
                _displayResolution = _class.VideoResolution.GetDisplayResolution(_graphicsCardID);

            return _displayResolution;
        }

        public string getVolume()
        {
            return "100%";
        }

        public List<string> getDisplayResolutionList()
        {
            List<string> listRes = _class.VideoResolution.ListDisplayResolutions(_graphicsCardID);

            return listRes;
        }

        public List<string> getDisplayRefresh()
        {
            List<string> listRefresh = _class.VideoResolution.ListDisplayRefresh(_graphicsCardID);
            
            return listRefresh;
        }

        public void setDisplayResolution(string video)
        {
            if (_refreshRate == null || _refreshRate.Length == 0)
                _refreshRate = _class.VideoResolution.GetRefreshRate(_graphicsCardID);

            string set = video + " - " + _refreshRate;

            _class.VideoResolution.SetDisplayResolution(_graphicsCardID, set);

            _class.Set.Add("Resolution", video);

            _displayResolution = video;
        }

        public void setDisplayRefresh(string refresh)
        {
            if (_displayResolution == null || _displayResolution.Length == 0)
                _displayResolution = _class.VideoResolution.GetDisplayResolution(_graphicsCardID);

            string set = _displayResolution + " - " + refresh;

            _class.VideoResolution.SetDisplayResolution(_graphicsCardID, set);
            _class.Main.ChangeDisplayRes();

            _class.Set.Add("RefreshRate", refresh);

            _refreshRate = refresh;
        }

        public void autoChangeRes(int height)
        {
            if (boolAutoSetResolution)
            {
                List<string> listRes = _class.VideoResolution.ListDisplayResolutions(_graphicsCardID);

                var set = "";

                for (int count = 0; count < listRes.Count; count++)
                {
                    string title = listRes[count];
                    if (title.IndexOf("x ") > -1)
                    {
                        title = title.Substring(title.IndexOf("x ") + 1).Trim();
                        if (title == height.ToString())
                        {
                            set = listRes[count];
                            break;
                        }
                    }
                }

                if (set.ToLower() != getResolution().ToLower())                
                    setDisplayResolution(set);
            }
        }

        public void getInitialDisplay() { _initialDisplay = getResolution(); }
        public void setInitialDisplay() { _class.VideoResolution.SetDisplayResolution(_graphicsCardID, _initialDisplay); }

        public void setAutoChangeDisplay()
        {
            boolAutoSetResolution = !boolAutoSetResolution;

            _class.Set.Add("AutoResolution", boolAutoSetResolution.ToString());
        }

        public void setStayOnTop()
        {
            BoolStayOnTop = !BoolStayOnTop;
            _class.Set.Add("StayOnTop", BoolStayOnTop.ToString());
        }

        private void changeResolution(string resolution)
        {
            strSetResolution = resolution;
        }

        public void changeCaptureResolution(string resolution)
        {
            strCurrentResolution = resolution;
            resolution = resolution.ToLower();
            if (resolution != "resolution")
            {
                List<string> listRes = _class.VideoCapture.GetVideoResolution();
                for (int count = 0; count < listRes.Count; count++)
                {
                    if (resolution == listRes[count].ToLower())
                    {
                        _class.VideoCapture.SetVideoResolution(count);
                        break;
                    }
                }
            }
        }

        public void SetTitanOneDevice(string serial)
        {
            _class.Set.Add("UseTitanOne", serial);
            _class.Set.Add("TitanOne", "True");
        }

        public void changeCaptureAutoRes()
        {
            boolAutoSetCaptureResolution = !boolAutoSetCaptureResolution;

            _class.Set.Add("CheckCaptureRes", boolAutoSetCaptureResolution.ToString());
        }

        private void setupVR()
        {
            //_vrMode.InitializeVr();
        }

        public void SetupMouse(int mouseType)
        {
            boolEnableMouse = true;
        }

        public void ChangeVrVideo()
        {
            IsVr = !IsVr;
            _class.Main.ChangeVr();
            _class.VideoCapture.runGraph();            
            _class.Set.Add("VR_Video", IsVr.ToString());
        }

        public void Debug(string file, string write) { _class.Debug.debug(file, write); }
        public void Debug(string write) { _class.Debug.debug(write); }
        public int GetDebugLevel() { return _class.Debug.intDebugLevel; }
        public void LoadSetup() { _class.Xml.Read(); }
        public void AddData(string title, string set) { _class.Set.Add(title, set); }
        public string CheckData(string title) { return _class.Set.Check(title);  }
    }
}
