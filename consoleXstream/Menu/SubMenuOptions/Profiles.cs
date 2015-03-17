using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using consoleXstream.Config;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenuOptions
{
    public class Profiles
    {
        public Profiles(Classes classes) { _class = classes; }
        private Classes _class;
        /*
        private Interaction _class.Data;
        private SubMenu.Action _subAction;
        private SubMenu.Shutter _shutter;
        private User _user;
        private Configuration _class.System;
        private VideoCapture.VideoCapture _class.VideoCapture;
        
        public void GetDataHandle(Interaction data) { _class.Data = data; }
        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetSystemHandle(Configuration system) { _class.System = system; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture video) { _class.VideoCapture = video; }
        */

        public List<string> List()
        {
            var listData = new List<string>();

            if (Directory.Exists("Profiles") != true) return listData;

            var listDir = Directory.GetFiles(@"Profiles", "*.connectProfile");
            if (!listDir.Any()) return listData;
            listData.AddRange(listDir.Select(Path.GetFileNameWithoutExtension));
            
            return listData;
        }

        public void Save(string command)
        {
            _class.Data.Checked.Clear();
            _class.Data.Checked.Add(command);

            _class.User.ConnectProfile = command;
            _class.System.AddData("CurrentProfile", command);

            var strTitle = command;
            command = command.Replace(" ", String.Empty);

            if (Directory.Exists("Profiles") == false) { Directory.CreateDirectory("Profiles"); }
            if (File.Exists(@"Profiles\" + command + ".connectProfile")) { File.Delete(@"Profiles\" + command + ".connectProfile"); }

            var strDev = _class.VideoCapture.GetVideoDevice();
            var strAud = _class.VideoCapture.GetAudioDevice();
            var strCrossVideo = "";
            var strCrossAudio = "";

            if (_class.VideoCapture._xBar != null)
            {
                int intPinVideo;
                int intPinAudio;
                _class.VideoCapture._xBar.get_IsRoutedTo(0, out intPinVideo);
                _class.VideoCapture._xBar.get_IsRoutedTo(1, out intPinAudio);
                strCrossVideo = _class.VideoCapture.GetCrossbarOutput(intPinVideo, "Video");
                strCrossAudio = _class.VideoCapture.GetCrossbarOutput(intPinAudio, "Audio");
            }

            //Control method

            var strSave = "<Profile>";
            strSave += "<Title>" + strTitle + "</Title>";
            strSave += "<videoCaptureSettings>";
            strSave += "<device>" + strDev + "</device>";
            strSave += "<audio>" + strAud + "</audio>";
            strSave += "</videoCaptureSettings>";
            if ((strCrossAudio.Length > 0) || (strCrossVideo.Length > 0))
            {
                strSave += "<videoInput>";
                if (strCrossVideo.Length > 0) { strSave += "<videoPin>" + strCrossVideo + "</videoPin>"; }
                if (strCrossAudio.Length > 0) { strSave += "<audioPin>" + strCrossAudio + "</audioPin>"; }
                strSave += "</videoInput>";
            }
            if (_class.System.boolControllerMax)
                strSave += "<ControllerMax>True</ControllerMax>";

            strSave += "</Profile>";

            var doc = new XmlDocument();
            doc.LoadXml(strSave);
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(@"Profiles\" + command + ".connectProfile", settings);
            doc.Save(writer);
            writer.Close();
        }

        public void Load(string strFile)
        {
            var strDevice = "";
            var strAudio = "";
            var strVideoPin = "";
            var strAudioPin = "";

            var strSetting = "";
            if (Directory.Exists("Profiles") != true) return;
            if (File.Exists(@"Profiles\" + strFile + ".connectProfile") != true) return;
            var reader = new XmlTextReader(@"Profiles\" + strFile + ".connectProfile");
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
                            if (reader.Name.ToLower() == "controllermax")
                            {
                                if (strSetting.ToLower() == "true")
                                {
                                    _class.Form1.InitControllerMax();
                                    _class.System.boolControllerMax = true;
                                    _class.System.boolTitanOne = false;
                                }
                            }
                        }
                        strSetting = "";
                        break;
                }
            }
            reader.Close();
    
            _class.User.ConnectProfile = strFile;
            _class.System.AddData("CurrentProfile", strFile);
            _class.System.AddData("VideoCaptureDevice", strDevice);
            _class.System.AddData("AudioPlaybackDevice", strAudio);
            if (strVideoPin.Length > 0) _class.System.AddData("crossbarVideoPin", strVideoPin);
            if (strAudio.Length > 0) _class.System.AddData("crossbarAudioPin", strAudioPin);

            _class.Data.Checked.Clear();
            _class.Data.Checked.Add(strFile);

            _class.VideoCapture.SetVideoCaptureDevice(strDevice);
            //TODO: set Audio device
            _class.VideoCapture.SetCrossbar(strVideoPin);
            _class.VideoCapture.SetCrossbar(strAudioPin);
            _class.VideoCapture.runGraph();
        }

    }
}
