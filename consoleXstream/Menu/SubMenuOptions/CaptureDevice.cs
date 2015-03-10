using System;
using System.Collections.Generic;
using consoleXstream.Config;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenuOptions
{
    public class CaptureDevice
    {
        public CaptureDevice(Classes classes) { _class = classes; }
        private Classes _class;

        public void Find()
        {
            if (_class.System.boolInternalCapture)
            {
                var videoList = _class.VideoCapture.GetVideoCaptureByName();
                if (videoList != null && videoList.Count > 0)
                {
                    foreach (var device in videoList)
                    {
                        _class.SubAction.AddSubItem(device, device);
                    }

                    _class.Data.Checked.Clear();
                    _class.Data.Checked.Add(_class.VideoCapture.strVideoCaptureDevice);
                }
                else
                {
                    _class.Shutter.Error = "No capture devices found";
                    _class.Shutter.Explain = "";                    
                }
            }
            else
            {
                _class.Shutter.Error = "No implied control for external capture";
                _class.Shutter.Explain = "Please use your applications device select options";
            }
        }

        public void Change(string strSet)
        {
            if (!_class.System.boolInternalCapture)
                return;

            var videoList = _class.VideoCapture.GetVideoCaptureByName();
            
            if (videoList == null)
                return;

            if (videoList.Count == 0 || videoList.IndexOf(strSet) == -1)
                return;

            _class.System.addUserData("VideoCaptureDevice", strSet);
            _class.VideoCapture.SetVideoCaptureDevice(strSet);
            _class.VideoCapture.runGraph();
            _class.Data.Checked.Clear();
            _class.Data.Checked.Add(_class.VideoCapture.strVideoCaptureDevice);
        }

        public void ListCaptureResolution()
        {
            _class.Data.ClearButtons();

            _class.Shutter.Scroll = 0;

            _class.Data.SubItems.Clear();
            _class.Data.Checked.Clear();
            _class.Shutter.Error = "";
            _class.Shutter.Explain = "";
            _class.User.Menu = "resolution";

            var listAdded = new List<string>();

            var listVideoRes = _class.VideoCapture.GetVideoResolution();
            var currentResolution = _class.VideoCapture.GetVideoResolutionCurrent();

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
                    _class.System.strCurrentResolution = listVideoRes[count];
                    _class.SubAction.AddSubItem(listVideoRes[count], "*" + listVideoRes[count], true);
                }
                else
                    _class.SubAction.AddSubItem(listVideoRes[count], listVideoRes[count]);
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

            var listRes = _class.VideoCapture.getVideoResolution();
            for (var count = 0; count < listRes.Count; count++)
            {
                if (resolution != listRes[count].ToLower())
                    continue;

                _class.VideoCapture.setVideoResolution(count);
                _class.VideoCapture.runGraph();

                _system.addUserData("CaptureResolution", resolution);

                break;
            }
             */
        }

        private void SelectSubItem()
        {
            if (_class.Data.SubItems.Count > 0)
            {
                _class.User.SubSelected = _class.Data.SubItems[0].Command;
            }
        }


    }
}
