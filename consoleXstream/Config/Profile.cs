using System.IO;
using System.Xml;

namespace consoleXstream.Config
{
    public class Profile
    {
        public Profile(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Load(string strFile)
        {
            string strDevice = "";
            string strAudio = "";
            string strVideoPin = "";
            string strAudioPin = "";

            string strSetting = "";
            if (Directory.Exists("Profiles"))
            {
                if (File.Exists(@"Profiles\" + strFile + ".connectProfile"))
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
                    _class.Set.Add("CurrentProfile", strFile);
                    _class.Set.Add("VideoCaptureDevice", strDevice);
                    _class.Set.Add("AudioPlaybackDevice", strAudio);
                    if (strVideoPin.Length > 0) _class.Set.Add("crossbarVideoPin", strVideoPin);
                    if (strAudio.Length > 0) _class.Set.Add("crossbarAudioPin", strAudioPin);

                    _class.VideoCapture.SetVideoCaptureDevice(strDevice);
                    //TODO: set Audio device
                    _class.VideoCapture.SetCrossbar(strVideoPin);
                    _class.VideoCapture.SetCrossbar(strAudioPin);
                    _class.VideoCapture.RunGraph();
                }
            }
        }

    }
}
