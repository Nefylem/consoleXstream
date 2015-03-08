using System;
using System.Collections.Generic;
using consoleXstream.Config;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenuOptions
{
    public class CaptureDevice
    {
        private Interaction _data;
        private SubMenu.Shutter _shutter;
        private SubMenu.Action _subAction;
        private Configuration _system;
        private User _user;
        private VideoCapture.VideoCapture _videoCapture;

        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }
        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVideoCapture(VideoCapture.VideoCapture videoCapture) { _videoCapture = videoCapture; }

        public void Find()
        {
            if (_system.boolInternalCapture)
            {
                for (var intCount = 0; intCount < _videoCapture.listVideoCapture.Count; intCount++)
                {
                    _subAction.AddSubItem(_videoCapture.listVideoCaptureName[intCount], _videoCapture.listVideoCaptureName[intCount]);
                    //addNewVideoCapture(videoCapture.listVideoCaptureName[intCount]);
                    //open up a preview window for this
                    //videoCapture = new classVideoCapture(this);
                }

                if (_data.SubItems.Count == 0)
                {
                    _shutter.Error = "No capture devices found";
                    _shutter.Explain = "";
                }
                else
                {
                    _data.Checked.Clear();
                    _data.Checked.Add(_videoCapture.strVideoCaptureDevice);
                }
            }
            else
            {
                _shutter.Error = "No implied control for external capture";
                _shutter.Explain = "Please use your applications device select options";
            }
        }

        public void Change(string strSet)
        {
            if (!_system.boolInternalCapture)
                return;
            var intIndex = _videoCapture.listVideoCaptureName.IndexOf(strSet);

            if (intIndex <= -1)
                return;

            _system.addUserData("VideoCaptureDevice", strSet);
            _videoCapture.SetVideoCaptureDevice(strSet);
            _videoCapture.runGraph();
            _data.Checked.Clear();
            _data.Checked.Add(_videoCapture.strVideoCaptureDevice);
        }

        public void ListCaptureResolution()
        {
            _data.ClearButtons();

            _shutter.Scroll = 0;

            _data.SubItems.Clear();
            _data.Checked.Clear();
            _shutter.Error = "";
            _shutter.Explain = "";
            _user.Menu = "resolution";

            var listAdded = new List<string>();

            var listVideoRes = _videoCapture.GetVideoResolution();
            var currentResolution = _videoCapture.GetVideoResolutionCurrent();

            for (var count = 0; count < listVideoRes.Count; count++)
            {
                var videoRes = listVideoRes[count];
                if (videoRes.IndexOf("[", StringComparison.Ordinal) > -1)
                    videoRes = videoRes.Substring(0, videoRes.IndexOf("[", StringComparison.Ordinal)).Trim();

                if (listAdded.IndexOf(videoRes) != -1)
                    continue;

                listAdded.Add(videoRes);

                if (count == currentResolution)
                {
                    _system.strCurrentResolution = listVideoRes[count];
                    _subAction.AddSubItem(listVideoRes[count], "*" + listVideoRes[count], true);
                }
                else
                    _subAction.AddSubItem(listVideoRes[count], listVideoRes[count]);
            }

            SelectSubItem();
        }

        private void ChangeResolution(string resolution)
        {
            /*
            _system.strCurrentResolution = resolution;
            resolution = resolution.ToLower();
            if (resolution == "resolution")
                return;

            var listRes = _videoCapture.getVideoResolution();
            for (var count = 0; count < listRes.Count; count++)
            {
                if (resolution != listRes[count].ToLower())
                    continue;

                _videoCapture.setVideoResolution(count);
                _videoCapture.runGraph();

                _system.addUserData("CaptureResolution", resolution);

                break;
            }
             */
        }

        private void SelectSubItem()
        {
            if (_data.SubItems.Count > 0)
            {
                _user.SubSelected = _data.SubItems[0].Command;
            }
        }


    }
}
