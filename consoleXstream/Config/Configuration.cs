using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Reflection;
using consoleXstream.Remap;

namespace consoleXstream.Config
{
    public class Configuration
    {
        private Form1 frmMain;
        private VideoCapture.VideoCapture videoCapture;
        private Output.ControllerMax controllerMax;
        private Output.TitanOne.Write titanOne;
        private VideoResolution videoResolution;
        private VrMode _vrMode;

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
        public bool boolSaveData { get; private set; }

        public bool boolAutoSetResolution { get; private set; }
        public bool boolAutoSetCaptureResolution { get; private set; }
        public bool EnableGcmapi { get; private set; }
        
        public bool boolMenu { get; set; }
        public bool BoolStayOnTop { get; set; }
        public bool IsOverrideOnExit { get; set; }
        public bool IsVr { get; set; }
        public int MouseMode { get; set; }

        public int intDebugLevel = 5;           //All debug commands

        public string strCaptureProfile { get; set; }
        public string strCurrentResolution { get; set; }
        public string strSetResolution { get; set; }
        
        private List<string> _listUserTitle;
        private List<string> _listUserData;

        private int _intLastDebugLevel = 0;

        private int _graphicsCardID;
        private int _refreshRateID;
        private int _displayResolutionID;
        private string _graphicsCard;
        private string _refreshRate;
        private string _displayResolution;
        private string _initialDisplay;
        public Keymap.KeyboardKeys keyDef ;
        public Keymap.KeyboardKeys keyAltDef;

        public Configuration(Form1 mainForm) { frmMain = mainForm; }
        public void getVideoCaptureHandle(VideoCapture.VideoCapture inVideo) { videoCapture = inVideo; }
        public void getControllerMaxHandle(Output.ControllerMax inMax) { controllerMax = inMax; }
        public void getTitanOneHandle(Output.TitanOne.Write inTO) { titanOne = inTO; }
        public void getVideoResolutionHandle(Config.VideoResolution inVid) { videoResolution = inVid; }

        private async Task WriteTextAsync(string strWrite)
        {
            strWrite = strWrite.Trim();
            string strCurrentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            StreamWriter txtOut = new StreamWriter("system.log", true);
            if (strWrite.Length > 0)
                strWrite = strCurrentTime + " - " + strWrite;
            await txtOut.WriteLineAsync(strWrite);
            txtOut.Close();
        }

        private async Task WriteTextAsync(string strFile, string strWrite)
        {
            strWrite = strWrite.Trim();
            string strCurrentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            StreamWriter txtOut = new StreamWriter(strFile, true);
            if (strWrite.Length > 0)
                strWrite = strCurrentTime + " - " + strWrite;
            await txtOut.WriteLineAsync(strWrite);
            txtOut.Close();
        }

        private void taskTest()
        {

        }

        public async void debug(string strWrite)
        {
            int intLevel = _intLastDebugLevel;
            int intSysLevel = intDebugLevel;

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

        public async void debug(string strFile, string strWrite)
        {
            int intLevel = _intLastDebugLevel;
            int intSysLevel = intDebugLevel;

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
                await WriteTextAsync(strFile, strWrite);
            }
        }

        public void initializeUserData()
        {
            _listUserTitle = new List<string>();
            _listUserData = new List<string>();

            _vrMode = new VrMode(this);
        }

        public void loadDefaults()
        {
            addUserData("InternalCapture", "true");
            addUserData("Crossbar", "true");
            addUserData("Preview", "true");
            addUserData("AVIRender", "true");

            addUserData("ControllerRumble", "true");
            addUserData("Keyboard", "true");
            addUserData("HideMouse", "true");
            //boolEnableMouse = true;
            useTitanOneAPI = true;
            BoolStayOnTop = true;
            boolAutoSetCaptureResolution = true;
            EnableGcmapi = true;
        }

        public void loadSetupXML()
        {
            boolSaveData = false;
            string strSetting = "";
            if (File.Exists("config.xml") == true)
            {
                XmlTextReader reader = new XmlTextReader("config.xml");
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.
                            //MessageBox.Show("<" + reader.Name);
                            break;
                        case XmlNodeType.Text: //Display the text in each element.
                            strSetting = reader.Value;
                            break;
                        case XmlNodeType.EndElement: //Display the end of the element.
                            if (strSetting.Length > 0) { addUserData(reader.Name, strSetting); }
                            strSetting = "";
                            break;
                    }
                }
                reader.Close();
            }
            boolSaveData = true;
        }

        //Adds or replaces user data
        public void addUserData(string strTitle, string strSet)
        {
            if (strTitle.ToLower() != "title")
            {
                int intIndex = _listUserTitle.IndexOf(strTitle);
                if (intIndex > -1)             
                {
                    //Overwrite current setting
                    _listUserData[intIndex] = strSet;
                }
                else
                {
                    //Add new setting
                    _listUserTitle.Add(strTitle);
                    _listUserData.Add(strSet);
                }
            }
            
            if (boolSaveData == true)
                saveUserData();
        }

        public void checkUserSettings()
        {

            if (checkUserSetting("keyboard").ToLower() == "true") boolEnableKeyboard = true;
            if (checkUserSetting("mouse").ToLower() == "true") boolEnableMouse = true;
            if (checkUserSetting("hidemouse").ToLower() == "true") boolHideMouse = true;

            if (checkUserSetting("internalCapture").ToLower() == "true") boolInternalCapture = true;  
            if (checkUserSetting("controllerrumble").ToLower() == "true") boolUseRumble = true;

            if (checkUserSetting("ds4emulation").ToLower() == "true") boolPS4ControllerMode = true;
            if (checkUserSetting("normalize").ToLower() == "true") boolNormalizeControls = true;

            if (checkUserSetting("controllermax").ToLower() == "true") boolControllerMax = true;
            if (checkUserSetting("titanone").ToLower() == "true") boolTitanOne = true;
            if (checkUserSetting("showfps").ToLower() == "true") boolFPS = true;
            if (checkUserSetting("stayontop").ToLower() == "true") BoolStayOnTop = true;
            if (checkUserSetting("checkcaptureres").ToLower() == "true") boolAutoSetCaptureResolution = true;
            if (checkUserSetting("AutoResolution").ToLower() == "true") boolAutoSetResolution = true;
            if (checkUserSetting("VRMode").ToLower() == "true") setupVR();
          
            _refreshRate = checkUserSetting("RefreshRate");
            _displayResolution = checkUserSetting("Resolution");

            if (!boolAutoSetResolution && (_refreshRate.Length > 0 || _displayResolution.Length > 0))
            {
                if (_displayResolution.Length == 0)
                    setDisplayRefresh(_refreshRate);
                else
                    setDisplayResolution(_displayResolution);
            }

            if (checkUserSetting("CaptureResolution").Length > 0) changeResolution(checkUserSetting("CaptureResolution"));

            strCaptureProfile = checkUserSetting("CurrentProfile");

            if (frmMain.boolIDE)
                boolHideMouse = false;
        }

        public string checkUserSetting(string strTitle)
        {
            string strReturn = "";

            int intIndex = _listUserTitle.FindIndex(x => x.Equals(strTitle, StringComparison.OrdinalIgnoreCase));
            if (intIndex > -1)
            {
                strReturn = _listUserData[intIndex];
            }

            return strReturn;
        }

        public string checkUserDataSetting(string strTitle)
        {
            string strReturn = "";
            string strTemp = "";
            
            strTemp = findUserSettings(strTitle);
            
            if (strTemp.Length > 0) 
                strReturn += strTemp; 
            
            return strReturn;
        }

        private string findUserSettings(string strTitle)
        {
            for (int intCount = 0; intCount < _listUserTitle.Count; intCount++)
            {
                if (_listUserTitle[intCount].ToLower() == strTitle.ToLower())
                    return "<" + strTitle + ">" + _listUserData[intCount] + "</" + strTitle + ">";
            }
            return "";
        }

        public void saveUserData()
        {
            string strSave = "<Configuration>";
            strSave += "<Title>consoleXstream v.0.01</Title>";

            strSave += "<videoCaptureSettings>";
                strSave += findUserSettings("VideoCaptureDevice");
                strSave += findUserSettings("AudioPlaybackDevice");
                strSave += checkUserDataSetting("CaptureResolution");
                strSave += checkUserDataSetting("crossbarVideoPin");
                strSave += checkUserDataSetting("crossbarAudioPin");
                strSave += checkUserDataSetting("AVIRender");
                strSave += checkUserDataSetting("CheckCaptureRes");
            strSave += "</videoCaptureSettings>";

            strSave += "<DisplaySettings>";
                strSave += checkUserDataSetting("AutoResolution");
                strSave += checkUserDataSetting("RefreshRate");
                strSave += checkUserDataSetting("Resolution");
                strSave += checkUserDataSetting("StayOnTop");
            strSave += "</DisplaySettings>";

            strSave += checkUserDataSetting("CurrentProfile");
            strSave += checkUserDataSetting("DS4Emulation");
            strSave += checkUserDataSetting("Normalize");
            strSave += checkUserDataSetting("ControllerMax");
            strSave += checkUserDataSetting("TitanOne");

            strSave += "</Configuration>";

            // Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strSave);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create("config.xml", settings);
            doc.Save(writer);
            writer.Close();
        }

        public void loadProfile(string strFile)
        {
            string strDevice = "";
            string strAudio = "";
            string strVideoPin = "";
            string strAudioPin = "";

            string strSetting = "";
            if (Directory.Exists("Profiles") == true)
            {
                if (File.Exists(@"Profiles\" + strFile + ".connectProfile") == true)
                {
                    XmlTextReader reader = new XmlTextReader(@"Profiles\" + strFile + ".connectProfile");
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: 
                                break;
                            case XmlNodeType.Text: //Display the text in each element.
                                strSetting = reader.Value;
                                break;
                            case XmlNodeType.EndElement: 
                                if (strSetting.Length > 0)
                                {
                                    if (reader.Name.ToLower() == "device") { strDevice = strSetting; }
                                    if (reader.Name.ToLower() == "audio") { strAudio = strSetting; }
                                    if (reader.Name.ToLower() == "videopin") { strVideoPin = strSetting; }
                                    if (reader.Name.ToLower() == "audiopin") { strAudioPin = strSetting; }
                                }
                                strSetting = "";
                                break;
                        }
                    }
                    reader.Close();

                    //_strCurrentProfile = strFile;
                    addUserData("CurrentProfile", strFile);
                    addUserData("VideoCaptureDevice", strDevice);
                    addUserData("AudioPlaybackDevice", strAudio);
                    if (strVideoPin.Length > 0) addUserData("crossbarVideoPin", strVideoPin);
                    if (strAudio.Length > 0) addUserData("crossbarAudioPin", strAudioPin);
                    
                    videoCapture.SetVideoCaptureDevice(strDevice);
                    //TODO: set Audio device
                    videoCapture.setCrossbar(strVideoPin);
                    videoCapture.setCrossbar(strAudioPin);
                    videoCapture.runGraph();
                }
            }
        }

        public void changeDS4Emulation()
        {
            boolPS4ControllerMode = !boolPS4ControllerMode;
            addUserData("DS4Emulation", boolPS4ControllerMode.ToString());
        }

        public void changeNormalizeGamepad()
        {
            boolNormalizeControls = !boolNormalizeControls;
            addUserData("Normalize", boolNormalizeControls.ToString());
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

            if (boolTitanOne) changeTitanOne();         //Disable if running


            boolControllerMax = !boolControllerMax;
            addUserData("ControllerMax", boolControllerMax.ToString());

            if (boolControllerMax)
                controllerMax.initControllerMax();
            else
                controllerMax.closeControllerMaxInterface();
        }

        public void changeControllerMax_TOAPI()
        {
            titanOne.Close();

            boolControllerMax = !boolControllerMax;
            addUserData("ControllerMax", boolControllerMax.ToString());

            if (boolControllerMax)
            {
                boolTitanOne = false;
                addUserData("TitanOne", boolTitanOne.ToString());
            }

            if (boolControllerMax)
            {
                titanOne.SetToInterface(Output.TitanOne.Define.DevPid.ControllerMax);
                titanOne.Initialize();
            }
        }

        public void changeTitanOne()
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
            addUserData("TitanOne", boolTitanOne.ToString());

            if (boolTitanOne)
                titanOne.Initialize();
            else
                titanOne.Close();
        }

        public void changeTitanOne_TOAPI()
        {
            titanOne.Close();

            boolTitanOne = !boolTitanOne;
            addUserData("TitanOne", boolTitanOne.ToString());

            if (boolTitanOne)
            {
                boolControllerMax = false;
                addUserData("ControllerMax", boolControllerMax.ToString());
            }

            if (boolTitanOne)
            {
                titanOne.SetToInterface(Output.TitanOne.Define.DevPid.TitanOne);
                titanOne.Initialize();
            }
        }

        public void ChangeCrossbar()
        {
            var set = checkUserSetting("Crossbar").ToLower();
            addUserData("Crossbar", set == "true" ? "false" : "true");
            videoCapture.LoadUserSettings();
            videoCapture.runGraph();
        }

        public void ChangeAviRender()
        {
            var set = checkUserSetting("AVIRender").ToLower();
            addUserData("AVIRender", set == "true" ? "false" : "true");

            videoCapture.LoadUserSettings();
            videoCapture.runGraph();
        }

        public string getGraphicsCard()
        {
            if (_graphicsCard == null || _graphicsCard.Length == 0)
                _graphicsCard = videoResolution.GetVideoCard(_graphicsCardID);

            return _graphicsCard;
        }

        public string getRefreshRate()
        {
            if (_refreshRate == null || _refreshRate.Length == 0)
                _refreshRate = videoResolution.GetRefreshRate(_graphicsCardID);

            return _refreshRate;
        }

        public string getResolution()
        {
            if (_displayResolution == null || _displayResolution.Length == 0)
                _displayResolution = videoResolution.GetDisplayResolution(_graphicsCardID);

            return _displayResolution;
        }

        public string getVolume()
        {
            return "100%";
        }

        public List<string> getDisplayResolutionList()
        {
            List<string> listRes = videoResolution.ListDisplayResolutions(_graphicsCardID);

            return listRes;
        }

        public List<string> getDisplayRefresh()
        {
            List<string> listRefresh = videoResolution.ListDisplayRefresh(_graphicsCardID);
            
            return listRefresh;
        }

        public void setDisplayResolution(string video)
        {
            if (_refreshRate == null || _refreshRate.Length == 0)
                _refreshRate = videoResolution.GetRefreshRate(_graphicsCardID);

            string set = video + " - " + _refreshRate;

            videoResolution.SetDisplayResolution(_graphicsCardID, set);

            addUserData("Resolution", video);

            _displayResolution = video;
        }

        public void setDisplayRefresh(string refresh)
        {
            if (_displayResolution == null || _displayResolution.Length == 0)
                _displayResolution = videoResolution.GetDisplayResolution(_graphicsCardID);

            string set = _displayResolution + " - " + refresh;

            videoResolution.SetDisplayResolution(_graphicsCardID, set);
            frmMain.ChangeDisplayRes();

            addUserData("RefreshRate", refresh);

            _refreshRate = refresh;
        }

        public void autoChangeRes(int height)
        {
            if (boolAutoSetResolution)
            {
                List<string> listRes = videoResolution.ListDisplayResolutions(_graphicsCardID);

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
        public void setInitialDisplay() { videoResolution.SetDisplayResolution(_graphicsCardID, _initialDisplay); }

        public void setAutoChangeDisplay()
        {
            boolAutoSetResolution = !boolAutoSetResolution;

            addUserData("AutoResolution", boolAutoSetResolution.ToString());
        }

        public void setStayOnTop()
        {
            BoolStayOnTop = !BoolStayOnTop;
            addUserData("StayOnTop", BoolStayOnTop.ToString());
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
                List<string> listRes = videoCapture.GetVideoResolution();
                for (int count = 0; count < listRes.Count; count++)
                {
                    if (resolution == listRes[count].ToLower())
                    {
                        videoCapture.SetVideoResolution(count);
                        break;
                    }
                }
            }
        }

        public void changeCaptureAutoRes()
        {
            boolAutoSetCaptureResolution = !boolAutoSetCaptureResolution;

            addUserData("CheckCaptureRes", boolAutoSetCaptureResolution.ToString());
        }

        private void setupVR()
        {
            _vrMode.InitializeVr();
        }

        public void SetupMouse(int mouseType)
        {
            boolEnableMouse = true;
        }
    }
}
