using System.Collections.Generic;
using consoleXstream.Remap;

namespace consoleXstream.Config
{
    public class Configuration
    {
        private readonly Classes _class;
        
        public bool IsHideMouse { get; set; }
        public bool IsEnableMouse { get; set; }
        public bool IsEnableKeyboard { get; set; }

        public bool UseInternalCapture { get; set; }

        public bool UseRumble { get; set; }
        public bool IsPs4ControllerMode { get; set; }
        public bool IsNormalizeControls { get; set; }

        public bool CheckFps { get; set; }
        public bool IsStayOnTop { get; set; }
        public bool IsAutoSetDisplayResolution { get; set; }
        public bool IsAutoSetCaptureResolution { get; set; }
        public bool IsVr { get; set; }
        public string RefreshRate;
        public string DisplayResolution;

        public bool UseControllerMax { get; set; }
        public bool UseTitanOne { get; set; }
        public bool UseTitanOneApi { get; set; }
        public string TitanOneDevice { get; set; }

        public string CaptureProfile { get; set; }

        public bool boolGIMX { get; private set; }
        public bool boolRemoteGIMX { get; private set; }
        public bool boolMcShield { get; private set; }
        public bool boolControlVJOY { get; private set; }

        public bool boolBlockMenuButton { get; private set; }

        public bool EnableGcmapi { get; private set; }
        public bool DisableTitanOneRetry { get; set; }
        
        public bool boolMenu { get; set; }
        public bool IsOverrideOnExit { get; set; }
        
        public int VrVerticalOffset { get; set; }
        public int MouseMode { get; set; }


        public string strCurrentResolution { get; set; }
        public string strSetResolution { get; set; }

        public int GraphicsCardId;
        public string GraphicsCard;

        private int _refreshRateID;
        private int _displayResolutionID;
        public string _initialDisplay;
        public Keymap.KeyboardKeys KeyDef ;
        public Keymap.KeyboardKeys KeyAltDef;

        public Configuration(BaseClass home)
        {
            _class = new Classes(this);
            _class.DeclareClasses(home);

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

            //_class.Set.Add("ControllerRumble", "true");
            _class.Set.Add("Keyboard", "true");
            _class.Set.Add("HideMouse", "true");
            //IsEnableMouse = true;
            IsStayOnTop = true;
            IsAutoSetCaptureResolution = true;
            EnableGcmapi = true;
            VrVerticalOffset = 150;
            _class.Var.IsReadData = false;
        }

        public void ChangeCrossbar()
        {
            var set = _class.Set.Check("Crossbar").ToLower();
            _class.Set.Add("Crossbar", set == "true" ? "false" : "true");
            _class.VideoCapture.LoadUserSettings();
            _class.VideoCapture.RunGraph();
        }

        public void ChangeAviRender()
        {
            var set = _class.Set.Check("AVIRender").ToLower();
            _class.Set.Add("AVIRender", set == "true" ? "false" : "true");

            _class.VideoCapture.LoadUserSettings();
            _class.VideoCapture.RunGraph();
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

        public void ChangeCaptureAutoRes()
        {
            IsAutoSetCaptureResolution = !IsAutoSetCaptureResolution;

            _class.Set.Add("CheckCaptureRes", IsAutoSetCaptureResolution.ToString());
        }

        public void SetupMouse(int mouseType)
        {
            IsEnableMouse = true;
        }

        public void ChangeVrVideo()
        {
            IsVr = !IsVr;
            _class.Main.ChangeVr();
            _class.VideoCapture.RunGraph();            
            _class.Set.Add("VR_Video", IsVr.ToString());
        }

        public void Debug(string file, string write) { _class.Debug.debug(file, write); }
        public void Debug(string write) { _class.Debug.debug(write); }
        public int GetDebugLevel() { return _class.Debug.intDebugLevel; }
        public void LoadSetup() { _class.Xml.Read(); }
        public void AddData(string title, string set) { _class.Set.Add(title, set); }
        public string CheckData(string title) { return _class.Set.Check(title);  }
        public bool CheckLog(string title) { return _class.Log.CheckLog(title); }
        public void CheckUserSettings() { _class.Settings.Check(); }
        public void ChangeControllerMax() { _class.ControllerMaxConfig.Change(); }
        public void ChangeDs4Emulation() { _class.Gamepad.ChangeDs4Emulation(); }
        public void ChangeNormalizeGamepad() { _class.Gamepad.ChangeNormalizeGamepad(); }
        public void ChangeTitanOne() { _class.TitanOneConfig.Change(); }
        public void ChangeTitanOne(bool set) { _class.TitanOneConfig.Change(set); }
        public void SetDisplayRefresh(string command) { _class.Display.SetRefresh(command); }
        public void SetDisplayResolution(string command) { _class.Display.SetResolution(command);}
        public string GetGraphicsCard() { return _class.Display.GetGraphicsCard(); }
        public string GetRefreshRate() { return _class.Display.GetRefreshRate(); }
        public string GetResolution() { return _class.Display.GetResolution(); }
        public List<string> GetDisplayRefresh() { return _class.Display.GetDisplayRefresh(); }
        public List<string> GetDisplayResolutionList() { return _class.Display.GetDisplayResolutionList(); }
        public string GetVolume() { return _class.Display.GetVolume(); }
        public void AutoChangeRes(int height) { _class.Display.AutoChangeRes(height); }
        public void GetInitialDisplay() { _class.Display.GetInitialDisplay(); }
        public void SetInitialDisplay() { _class.Display.SetInitialDisplay(); }
        public void SetAutoChangeDisplay() {_class.Display.SetAutoChangeDisplay(); }
        public void SetStayOnTop() { _class.Display.SetStayOnTop(); }
        private void ChangeResolution(string resolution) { _class.Display.ChangeResolution(resolution); }
        public void ChangeRumble() { _class.Gamepad.ChangeRumble(); }
    }
}
