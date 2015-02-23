using System;
using consoleXstream.Config;
using consoleXstream.Menu.Data;
using consoleXstream.Menu.SubMenuOptions;
using consoleXstream.Output;

namespace consoleXstream.Menu.MainMenu
{
    class Action
    {
        private SubMenu.Action _subAction;
        private Configuration _system;
        private Interaction _data;
        private Navigation _nav;
        private User _user;
        private Variables _var;
        private SubMenu.Shutter _shutter;
        private VideoCapture.VideoCapture _videoCapture;

        private readonly ListDevices _listTo = new ListDevices();

        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetConfigHandle(Configuration config) { _system = config; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetNavHandle(Navigation nav) { _nav = nav; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVariableHandle(Variables inVar) { _var = inVar; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }
        public void GetVideoCapture(VideoCapture.VideoCapture videoCapture) { _videoCapture = videoCapture; }

        public void MainMenu(string command)
        {
            command = command.ToLower();

            var rowTop = -10;
            var currentRow = -1;

            //Find where the button is
            foreach (var t in _data.Buttons)
            {
                if (t.Command != _user.Selected) continue;
                rowTop = t.Rect.Top;
                break;
            }

            _data.ClearButtons();                     //Stop the mouse from being able to select main menu buttons

            //Find what row it belongs too
            for (var intCount = 0; intCount < _data.Row.Count; intCount++)
            {
                if (_data.Row[intCount] == rowTop)
                    currentRow = intCount;
            }

            switch (command)
            {
                case "console select": ConsoleSelect(command, currentRow); break;
                case "save profile": SaveProfile(command, currentRow); break;
                case "video input": VideoInput(command, currentRow); break;
                case "video device": VideoDevice(command, currentRow); break;
                case "video settings": VideoSettings(command, currentRow); break;
                case "video display": VideoDisplay(command, currentRow); break;
                case "device": Device(command, currentRow); break;
                case "controller output": ControllerOutput(command, currentRow); break;
                case "remap": Remap(command, currentRow); break;
                case "config": Config(command, currentRow); break;
                case "exit": Exit(command, currentRow); break;
            }
        }

        private void SetMenu(string command)
        {
            _user.Menu = command;
            _nav.ListHistory.Add(command);            
        }

        private void ClearSub()
        {
            _var.IsMainMenu = false;

            _data.SubItems.Clear();

            _shutter.Scroll = 0;

            _shutter.Error = "";
            _shutter.Explain = "";
        }

        private void ConsoleSelect(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            var profiles = new Profiles();
            profiles.GetDataHandle(_data);
            profiles.GetSubActionHandle(_subAction);
            profiles.GetShutterHandle(_shutter);

            var listData = profiles.List();

            _data.SubItems.Clear();            
            foreach (var profile in listData)
            {
                _subAction.AddSubItem(profile, profile);
                if (String.Equals(_user.ConnectProfile, profile, StringComparison.CurrentCultureIgnoreCase)) _data.Checked.Add(profile);
            }
            
            if (_data.SubItems.Count == 0)
            {
                _shutter.Error = "No profiles found";
                _shutter.Explain = "Please set up your console display then use Save Profile";
            }
            else
            {
                SelectSubItem();
                _shutter.SetActive(currentRow + 1);
            }
        }

        private void SaveProfile(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _subAction.AddSubItem("PlayStation3", "PlayStation3");
            _subAction.AddSubItem("PlayStation4", "PlayStation4");
            _subAction.AddSubItem("Xbox360", "Xbox360");
            _subAction.AddSubItem("XboxOne", "XboxOne");

            SelectSubItem();
            _shutter.SetActive(currentRow + 1);
        }

        private void VideoInput(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();
            
            var crossbar = new Crossbar();
            crossbar.GetDataHandle(_data);
            crossbar.GetShutterHandle(_shutter);
            crossbar.GetSubActionHandle(_subAction);
            crossbar.GetSystemHandle(_system);
            crossbar.GetVideoCapture(_videoCapture);
            
            crossbar.Find();
            SelectSubItem();
            _shutter.SetActive(currentRow + 1);
        }

        private void VideoDevice(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            var capture = new CaptureDevice();

            capture.GetDataHandle(_data);
            capture.GetShutterHandle(_shutter);
            capture.GetSubActionHandle(_subAction);
            capture.GetSystemHandle(_system);
            capture.GetVideoCapture(_videoCapture);

            capture.Find();
            SelectSubItem();
            _shutter.SetActive(currentRow + 1);
        }

        private void VideoSettings(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _subAction.AddSubItem("Crossbar", "Crossbar", CheckSystemSetting("Crossbar"));
            _subAction.AddSubItem("AVIRender", "AVI Renderer", CheckSystemSetting("AVIRender"));
            _subAction.AddSubItem("CheckCaptureRes", "Check Capture", CheckSystemSetting("CheckCaptureRes"));
            _subAction.AddSubItemFolder("Resolution", "Resolution", "Capture Resolution");

            SelectSubItem();
            _shutter.SetActive(currentRow + 1);
        }

        private void VideoDisplay(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _subAction.AddSubItem("AutoSet", "Auto Set", _system.boolAutoSetResolution);
            _subAction.AddSubItemFolder("Device", "Graphics Device", "Graphics Card");
            _subAction.AddSubItemFolder("Resolution", "Resolution", "Display Resolution");
            _subAction.AddSubItemFolder("Refresh", "Refresh Rate", "Screen Refresh");
            _subAction.AddSubItemFolder("Volume", "Volume", "Volume");
            _subAction.AddSubItem("StayOnTop", "Stay On Top", _system.boolAutoSetResolution);

            SelectSubItem();
            _shutter.SetActive(currentRow + 1);
        }

        private void Device(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _subAction.AddSubItem("DS4 Emulation", "DS4 Emulation");
            _subAction.AddSubItem("Normalize", "Normalize");

            CheckDisplaySettings();

            SelectSubItem();
            _shutter.SetActive(currentRow - 1);
        }

        private void ControllerOutput(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _subAction.AddSubItem("ControllerMax", "ControllerMax");

            _subAction.AddSubItem("TitanOne", "TitanOne", _listTo.GetToCount("TitanOne").ToString());
            RegisterWatcher("TitanOne");

            _subAction.AddSubItem("GIMX", "GIMX");
            _subAction.AddSubItem("Remote GIMX", "Remote GIMX");
            _subAction.AddSubItem("McShield", "McShield");
            _subAction.AddSubItem("Control VJOY", "Control VJOY");

            CheckDisplaySettings();
            SelectSubItem();
            _shutter.SetActive(currentRow - 1);
        }

        private void Remap(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _subAction.AddSubItem("Gamepad", "Gamepad");
            _subAction.AddSubItem("Keyboard", "Keyboard");
            _subAction.AddSubItem("Mouse", "Mouse");
            _subAction.AddSubItem("Touch", "Touch");

            SelectSubItem();
            _shutter.SetActive(currentRow - 1);
        }

        private void Config(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _shutter.SetActive(currentRow - 1);
        }

        private void Exit(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _subAction.AddSubItem("exit", "Yes");
            _subAction.AddSubItem("back", "No");

            _user.SubSelected = _data.SubItems[1].Command;
            _shutter.SetActive(currentRow - 1);
        }

        public void SelectSubItem()
        {
            if (_data.SubItems.Count > 0)
            {
                _user.SubSelected = _data.SubItems[0].Command;
            }
        }

        private bool CheckSystemSetting(string strCommand)
        {
            return _system.checkUserSetting(strCommand.ToLower()).ToLower() == "true";
        }

        private void RegisterWatcher(string title)
        {
            foreach (var t in _data.SubItems)
                if (t.Display == title) t.ActiveWatcher = title;
        }

        public void CheckDisplaySettings()
        {
            _data.Checked.Clear();

            foreach (var t in _data.SubItems)
            {
                if (t.Command.ToLower() == "ds4 emulation")
                    if (_system.boolPS4ControllerMode) _data.Checked.Add("DS4 Emulation");

                if (t.Command.ToLower() == "normalize")
                    if (_system.boolNormalizeControls) _data.Checked.Add("Normalize");

                if (t.Command.ToLower() == "controllermax")
                    if (_system.boolControllerMax) _data.Checked.Add("ControllerMax");

                if (t.Command.ToLower() == "titanone")
                    if (_system.boolTitanOne) _data.Checked.Add("TitanOne");

                if (t.Command.ToLower() == "gimx")
                    if (_system.boolGIMX) _data.Checked.Add("GIMX");

                if (t.Command.ToLower() == "remote gimx")
                    if (_system.boolRemoteGIMX) _data.Checked.Add("remote gimx");

                if (t.Command.ToLower() == "McShield")
                    if (_system.boolMcShield) _data.Checked.Add("McShield");

                if (t.Command.ToLower() == "control vjoy")
                    if (_system.boolControlVJOY) _data.Checked.Add("Control VJOY");

                if (t.Command.ToLower() == "crossbar")
                    if (CheckSystemSetting("Crossbar")) _data.Checked.Add("Crossbar");

                if (t.Command.ToLower() == "avirender")
                    if (CheckSystemSetting("AVIRender")) _data.Checked.Add("AVI Renderer");

                if (t.Command.ToLower() == "checkcaptureres")
                    if (CheckSystemSetting("CheckCaptureRes")) _data.Checked.Add("Check Capture");
            }
        }


    }
}
