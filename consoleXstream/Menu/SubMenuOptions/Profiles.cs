using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using consoleXstream.Config;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenuOptions
{
    class Profiles
    {
        private Interaction _data;
        private SubMenu.Action _subAction;
        private SubMenu.Shutter _shutter;
        private User _user;
        private Configuration _system;
        private VideoCapture.VideoCapture _videoCapture;

        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture video) { _videoCapture = video; }

        public List<string> List()
        {
            var listData = new List<string>();

            if (Directory.Exists("Profiles") != true) return listData;

            var listDir = Directory.GetFiles(@"Profiles", "*.connectProfile");
            if (!listDir.Any()) return listData;
            listData.AddRange(listDir.Select(Path.GetFileNameWithoutExtension));
            
            return listData;
        }

        public void Save(string strCommand)
        {
            _data.Checked.Clear();
            _data.Checked.Add(strCommand);

            _user.ConnectProfile = strCommand;
            _system.AddData("CurrentProfile", strCommand);

            var strTitle = strCommand;
            strCommand = strCommand.Replace(" ", String.Empty);

            if (Directory.Exists("Profiles") == false) { Directory.CreateDirectory("Profiles"); }
            if (File.Exists(@"Profiles\" + strCommand + ".connectProfile")) { File.Delete(@"Profiles\" + strCommand + ".connectProfile"); }

            var strDev = _videoCapture.GetVideoDevice();
            var strAud = _videoCapture.GetAudioDevice();
            var strCrossVideo = "";
            var strCrossAudio = "";

            if (_videoCapture._xBar != null)
            {
                int intPinVideo;
                int intPinAudio;
                _videoCapture._xBar.get_IsRoutedTo(0, out intPinVideo);
                _videoCapture._xBar.get_IsRoutedTo(1, out intPinAudio);
                strCrossVideo = _videoCapture.GetCrossbarOutput(intPinVideo, "Video");
                strCrossAudio = _videoCapture.GetCrossbarOutput(intPinAudio, "Audio");
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
            strSave += "</Profile>";

            var doc = new XmlDocument();
            doc.LoadXml(strSave);
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(@"Profiles\" + strCommand + ".connectProfile", settings);
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
                        }
                        strSetting = "";
                        break;
                }
            }
            reader.Close();

            _user.ConnectProfile = strFile;
            _system.AddData("CurrentProfile", strFile);
            _system.AddData("VideoCaptureDevice", strDevice);
            _system.AddData("AudioPlaybackDevice", strAudio);
            if (strVideoPin.Length > 0) _system.AddData("crossbarVideoPin", strVideoPin);
            if (strAudio.Length > 0) _system.AddData("crossbarAudioPin", strAudioPin);

            _data.Checked.Clear();
            _data.Checked.Add(strFile);

            _videoCapture.SetVideoCaptureDevice(strDevice);
            //TODO: set Audio device
            _videoCapture.SetCrossbar(strVideoPin);
            _videoCapture.SetCrossbar(strAudioPin);
            _videoCapture.runGraph();
        }

    }
}
