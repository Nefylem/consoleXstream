using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Menu.SubMenuOptions
{
    class Crossbar
    {
        public void Find()
        {
            if (_system.boolInternalCapture)
            {
                foreach (var t in _videoCapture.listCrossbarInput)
                {
                    var strTitle = t;
                    if (strTitle.ToLower() == "video_serialdigital") { strTitle = "HDMI"; }
                    if (strTitle.ToLower() == "video_yryby") { strTitle = "Component"; }
                    if (strTitle.ToLower() == "audio_spdifdigital") { strTitle = "Digital Audio"; }
                    if (strTitle.ToLower() == "audio_line") { strTitle = "Line Audio"; }

                    _subAction.AddSubItem(t, strTitle);
                }

                if (_data.SubItems.Count == 0)
                {
                    _shutter.Error = "No connections found";
                    _shutter.Explain = "Your capture device has no available crossbar information";
                }
                else
                    _action.SelectSubItem();
            }
            else
            {
                _shutter.Error = "No implied control for external capture";
                _shutter.Explain = "Please use your applications input select options";
            }
        }

        private void Change(string strSet)
        {
            var intIndex = _videoCapture.listCrossbarInput.IndexOf(strSet);
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
                    _videoCapture.setCrossbar("audio_spdifdigital");
                }

                _videoCapture.setCrossbar(strSet);
                _videoCapture.runGraph();
            }

            if (strSet.Substring(0, "audio_".Length).ToLower() != "audio_")
                return;
            _system.addUserData("crossbarAudioPin", strSet);
            _videoCapture.setCrossbar(strSet);
            _videoCapture.runGraph();
        }


    }
}
