using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace consoleXstream.Menu.SubMenuOptions
{
    public class Profiles
    {
        public Profiles(Classes classes) { _class = classes; }
        private readonly Classes _class;

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
            _class.Base.System.AddData("CurrentProfile", command);

            var strTitle = command;
            command = command.Replace(" ", String.Empty);

            if (Directory.Exists("Profiles") == false) { Directory.CreateDirectory("Profiles"); }
            if (File.Exists(@"Profiles\" + command + ".connectProfile")) { File.Delete(@"Profiles\" + command + ".connectProfile"); }
            //_class.System.Debug("Profile.log", "saving " + command + " .profile");

            var strDev = _class.Base.VideoCapture.GetVideoDevice();
            var strAud = _class.Base.VideoCapture.GetAudioDevice();
            var strCrossVideo = _class.Base.VideoCapture.GetCrossbarSetting("video");
            var strCrossAudio = _class.Base.VideoCapture.GetCrossbarSetting("audio");
            //_class.System.Debug("Profile.log", "video " + strCrossVideo);
            //_class.System.Debug("Profile.log", "audio " + strCrossAudio);

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
            if (_class.Base.System.UseControllerMax)
                strSave += "<ControllerMax>True</ControllerMax>";

            if (_class.Base.System.UseTitanOne)
            {
                strSave += "<TitanOne>True</TitanOne>";
                if (_class.Base.System.TitanOneDevice.Length > 0)
                    strSave += "<TitanOneDevice>" + _class.Base.System.TitanOneDevice + "</TitanOneDevice>";
            }

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

            //_class.System.Debug("Profile.log", "loading " + strFile + ".profile");
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
                                    _class.Base.Home.InitControllerMax();
                                    _class.Base.System.UseControllerMax = true;
                                    _class.Base.System.UseTitanOne = false;
                                }
                            }
                            if (reader.Name.ToLower() == "titanone")
                            {
                                if (strSetting.ToLower() == "true")
                                {
                                    _class.Base.Home.InitializeTitanOne();
                                    _class.Base.System.UseControllerMax = false;
                                    _class.Base.System.UseTitanOne = true;
                                }
                            }
                            if (reader.Name.ToLower() == "titanonedevice")
                            {
                                _class.Base.Home.SetTitanOneMode("Multi");
                                _class.Base.Home.SetTitanOne(strSetting);
                                _class.Base.System.TitanOneDevice = strSetting;
                            }
                        }
                        strSetting = "";
                        break;
                }
            }
            reader.Close();
    
            _class.User.ConnectProfile = strFile;
            _class.Base.System.AddData("CurrentProfile", strFile);
            _class.Base.System.AddData("VideoCaptureDevice", strDevice);
            _class.Base.System.AddData("AudioPlaybackDevice", strAudio);
            if (strVideoPin.Length > 0) _class.Base.System.AddData("crossbarVideoPin", strVideoPin);
            if (strAudio.Length > 0) _class.Base.System.AddData("crossbarAudioPin", strAudioPin);

            _class.Data.Checked.Clear();
            _class.Data.Checked.Add(strFile);

            _class.Base.VideoCapture.SetVideoCaptureDevice(strDevice);
            //TODO: set Audio device
            _class.Base.VideoCapture.SetCrossbar(strVideoPin);
            _class.Base.VideoCapture.SetCrossbar(strAudioPin);
            _class.Base.VideoCapture.RunGraph();
        }

    }
}
