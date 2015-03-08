using System.Windows.Forms;
using consoleXstream.Config;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenuOptions
{
    public class Display
    {
        private Configuration _system;
        private Interaction _data;
        private User _user;
        private SubMenu.Action _subAction;
        private SubMenu.Shutter _shutter;
        private VideoCapture.VideoCapture _videoCapture;
        private ShowMenu _menu;

        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture video) { _videoCapture = video; }
        public void GetMenuHandle(ShowMenu menu) { _menu = menu; }

        public void ChangeResolution(string resolution)
        {
            _system.strCurrentResolution = resolution;
            resolution = resolution.ToLower();
            if (resolution == "resolution")
                return;

            var listRes = _videoCapture.GetVideoResolution();
            for (var count = 0; count < listRes.Count; count++)
            {
                if (resolution != listRes[count].ToLower())
                    continue;

                _videoCapture.SetVideoResolution(count);
                _videoCapture.runGraph();

                _system.addUserData("CaptureResolution", resolution);

                break;
            }
        }

        private void ListDisplayRefresh()
        {
            _data.ClearButtons();

            _shutter.Scroll = 0;

            _data.SubItems.Clear();
            _data.Checked.Clear();
            _shutter.Error = "";
            _shutter.Explain = "";
            _user.Menu = "videorefresh";

            var listDisplayRef = _system.getDisplayRefresh();
            var currentRef = _system.getRefreshRate().ToLower();

            foreach (var title in listDisplayRef)
            {
                if (title.ToLower() == currentRef)
                    _subAction.AddSubItem(title, title, true);
                else
                    _subAction.AddSubItem(title, title);
            }

            SelectSubItem();
        }

        private void ListDisplayResolution()
        {
            _data.ClearButtons();

            _shutter.Scroll = 0;

            _data.SubItems.Clear();
            _data.Checked.Clear();
            _shutter.Error = "";
            _shutter.Explain = "";
            _user.Menu = "videoresolution";

            var listDisplayRes = _system.getDisplayResolutionList();
            var currentRes = _system.getResolution().ToLower();

            foreach (var title in listDisplayRes)
            {
                if (title.ToLower() == currentRes)
                    _subAction.AddSubItem(title, title, true);
                else
                    _subAction.AddSubItem(title, title);
            }

            SelectSubItem();
        }

        public void ChangeVideoResolution(string command)
        {
            if (command.ToLower() == "resolution")
                return;

            _system.setDisplayResolution(command);
            _data.Checked.Clear();
            _data.Checked.Add(command);
            
            _menu.PositionMenu();
        }

        public void ChangeVideoRefresh(string command)
        {
            if (command.ToLower() == "refresh")
                return;

            _system.setDisplayRefresh(command);
            _data.Checked.Clear();
            _data.Checked.Add(command);

            _menu.PositionMenu();
        }

        private void ChangeAutoRes()
        {
            if (_data.Checked.IndexOf("Auto Set") > -1)
                _data.Checked.RemoveAt(_data.Checked.IndexOf("Auto Set"));
            else
                _data.Checked.Add("Auto Set");

            _system.setAutoChangeDisplay();
        }

        private void ChangeStayOnTop()
        {
            if (_data.Checked.IndexOf("Stay On Top") > -1)
                _data.Checked.RemoveAt(_data.Checked.IndexOf("Stay On Top"));
            else
                _data.Checked.Add("Stay On Top");

            _system.setStayOnTop();
        }

        public void ChangeVideoDisplay(string command)
        {
            command = command.ToLower();
            if (command == "autoset") ChangeAutoRes();
            if (command == "resolution") ListDisplayResolution();
            if (command == "refresh") ListDisplayRefresh();
            if (command == "stayontop") ChangeStayOnTop();
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
