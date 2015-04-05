using System.Collections.Generic;
using consoleXstream.Remap;

namespace consoleXstream.Config
{
    public class Configuration
    {
        public readonly Classes Class;
        
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
            Class = new Classes(this);
            Class.DeclareClasses(home);

            Class.Set.Title = new List<string>();
            Class.Set.Data = new List<string>();
        }

        public void GetClassHandles(VideoCapture.VideoCapture inVideo, Output.ControllerMax inMax, Output.TitanOne.Write inTo, VideoResolution inVid)
        {
            Class.VideoCapture = inVideo;
            Class.ControllerMax = inMax;
            Class.TitanOne = inTo;
            Class.VideoResolution = inVid;
        }

        public void loadDefaults()
        {
            Class.Var.IsReadData = true;
            Class.Set.Add("InternalCapture", "true");
            Class.Set.Add("Crossbar", "true");
            Class.Set.Add("Preview", "true");
            Class.Set.Add("AVIRender", "true");

            //_class.Set.Add("ControllerRumble", "true");
            Class.Set.Add("Keyboard", "true");
            Class.Set.Add("HideMouse", "true");
            //IsEnableMouse = true;
            IsStayOnTop = true;
            IsAutoSetCaptureResolution = true;
            EnableGcmapi = true;
            Class.Var.IsReadData = false;
        }

        public void ChangeCrossbar()
        {
            var set = Class.Set.Check("Crossbar").ToLower();
            Class.Set.Add("Crossbar", set == "true" ? "false" : "true");
            Class.VideoCapture.LoadUserSettings();
            Class.VideoCapture.RunGraph();
        }

        public void ChangeAviRender()
        {
            var set = Class.Set.Check("AVIRender").ToLower();
            Class.Set.Add("AVIRender", set == "true" ? "false" : "true");

            Class.VideoCapture.LoadUserSettings();
            Class.VideoCapture.RunGraph();
        }

        public void changeCaptureResolution(string resolution)
        {
            strCurrentResolution = resolution;
            resolution = resolution.ToLower();
            if (resolution != "resolution")
            {
                List<string> listRes = Class.VideoCapture.GetVideoResolution();
                for (int count = 0; count < listRes.Count; count++)
                {
                    if (resolution == listRes[count].ToLower())
                    {
                        Class.VideoCapture.SetVideoResolution(count);
                        break;
                    }
                }
            }
        }

        public void SetTitanOneDevice(string serial)
        {
            Class.Set.Add("UseTitanOne", serial);
            Class.Set.Add("TitanOne", "True");
        }

        public void ChangeCaptureAutoRes()
        {
            IsAutoSetCaptureResolution = !IsAutoSetCaptureResolution;

            Class.Set.Add("CheckCaptureRes", IsAutoSetCaptureResolution.ToString());
        }

        public void SetupMouse(int mouseType)
        {
            IsEnableMouse = true;
        }

        public void Debug(string file, string write) { Class.Debug.debug(file, write); }
        public void Debug(string write) { Class.Debug.debug(write); }
        public int GetDebugLevel() { return Class.Debug.intDebugLevel; }
        public void LoadSetup() { Class.Xml.Read(); }
        public void AddData(string title, string set) { Class.Set.Add(title, set); }
        public string CheckData(string title) { return Class.Set.Check(title);  }
        public bool CheckLog(string title) { return Class.Log.CheckLog(title); }
        public void CheckUserSettings() { Class.Settings.Check(); }
        public void ChangeControllerMax() { Class.ControllerMaxConfig.Change(); }
        public void ChangeDs4Emulation() { Class.Gamepad.ChangeDs4Emulation(); }
        public void ChangeNormalizeGamepad() { Class.Gamepad.ChangeNormalizeGamepad(); }
        public void ChangeTitanOne() { Class.TitanOneConfig.Change(); }
        public void ChangeTitanOne(bool set) { Class.TitanOneConfig.Change(set); }
        public void SetDisplayRefresh(string command) { Class.Display.SetRefresh(command); }
        public void SetDisplayResolution(string command) { Class.Display.SetResolution(command);}
        public string GetGraphicsCard() { return Class.Display.GetGraphicsCard(); }
        public string GetRefreshRate() { return Class.Display.GetRefreshRate(); }
        public string GetResolution() { return Class.Display.GetResolution(); }
        public List<string> GetDisplayRefresh() { return Class.Display.GetDisplayRefresh(); }
        public List<string> GetDisplayResolutionList() { return Class.Display.GetDisplayResolutionList(); }
        public string GetVolume() { return Class.Display.GetVolume(); }
        public void AutoChangeRes(int height) { Class.Display.AutoChangeRes(height); }
        public void GetInitialDisplay() { Class.Display.GetInitialDisplay(); }
        public void SetInitialDisplay() { Class.Display.SetInitialDisplay(); }
        private void ChangeResolution(string resolution) { Class.Display.ChangeResolution(resolution); }
    }
}
