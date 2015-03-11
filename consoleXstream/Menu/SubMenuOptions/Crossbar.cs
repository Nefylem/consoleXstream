using consoleXstream.Config;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenuOptions
{
    class Crossbar
    {
        private Interaction _data;
        private SubMenu.Shutter _shutter;
        private SubMenu.Action _subAction;
        private Configuration _system;
        private VideoCapture.VideoCapture _videoCapture;

        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }
        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetVideoCapture(VideoCapture.VideoCapture videoCapture) { _videoCapture = videoCapture; }

        public void Find()
        {
            if (_system.boolInternalCapture)
            {
                var crossbarList = _videoCapture.GetCrossbarList();
                foreach (var t in crossbarList)
                {
                    var strTitle = t;
                    if (strTitle.ToLower() == "video_serialdigital") { strTitle = "HDMI"; }
                    if (strTitle.ToLower() == "video_yryby") { strTitle = "Component"; }
                    if (strTitle.ToLower() == "audio_spdifdigital") { strTitle = "Digital Audio"; }
                    if (strTitle.ToLower() == "audio_line") { strTitle = "Line Audio"; }

                    _subAction.AddSubItem(t, strTitle);
                }

                if (_data.SubItems.Count != 0) return;
                _shutter.Error = "No connections found";
                _shutter.Explain = "Your capture device has no available crossbar information";
            }
            else
            {
                _shutter.Error = "No implied control for external capture";
                _shutter.Explain = "Please use your applications input select options";
            }
        }

        public void Change(string strSet)
        {
            var crossbarList = _videoCapture.GetCrossbarList();
            var intIndex = crossbarList.IndexOf(strSet);

            if (intIndex <= -1)
                return;
            if (strSet.Length <= "video_".Length) return;

            if (strSet.Substring(0, "video_".Length).ToLower() == "video_")
            {
                _system.addUserData("crossbarVideoPin", strSet);

                //Set the audio pin if selecting HDMI
                if (strSet.ToLower() == "video_serialdigital")
                {
                    _system.addUserData("crossbarAudioPin", "audio_spdifdigital");
                    _videoCapture.SetCrossbar("audio_spdifdigital");
                }

                _videoCapture.SetCrossbar(strSet);
                _videoCapture.runGraph();
            }

            if (strSet.Substring(0, "audio_".Length).ToLower() != "audio_")
                return;
            _system.addUserData("crossbarAudioPin", strSet);
            _videoCapture.SetCrossbar(strSet);
            _videoCapture.runGraph();
        }


    }
}
