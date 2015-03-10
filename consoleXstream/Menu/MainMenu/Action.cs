using System;
using consoleXstream.Config;
using consoleXstream.Menu.Data;
using consoleXstream.Menu.SubMenuOptions;
using consoleXstream.Output;

namespace consoleXstream.Menu.MainMenu
{
    public class Action
    {
        private readonly Classes _class;

        public Action(Classes inClass) { _class = inClass; }

        private readonly CountDevices _listTo = new CountDevices();

        public void MainMenu(string command)
        {
            command = command.ToLower();

            var rowTop = -10;
            var currentRow = -1;

            //Find where the button is
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != _class.User.Selected) continue;
                rowTop = t.Rect.Top;
                break;
            }

            _class.Data.ClearButtons();                     //Stop the mouse from being able to select main menu buttons

            //Find what row it belongs too
            for (var intCount = 0; intCount < _class.Data.Row.Count; intCount++)
            {
                if (_class.Data.Row[intCount] == rowTop)
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
            _class.User.Menu = command;
            _class.Nav.ListHistory.Add(command);            
        }

        private void ClearSub()
        {
            _class.Var.IsMainMenu = false;

            _class.Data.SubItems.Clear();

            _class.Shutter.Scroll = 0;

            _class.Shutter.Error = "";
            _class.Shutter.Explain = "";
        }

        private void ConsoleSelect(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            var profiles = new Profiles();
            profiles.GetDataHandle(_class.Data);
            profiles.GetSubActionHandle(_class.SubAction);
            profiles.GetShutterHandle(_class.Shutter);

            var listData = profiles.List();

            _class.Data.SubItems.Clear();            
            foreach (var profile in listData)
            {
                _class.SubAction.AddSubItem(profile, profile);
                if (String.Equals(_class.User.ConnectProfile, profile, StringComparison.CurrentCultureIgnoreCase)) _class.Data.Checked.Add(profile);
            }
            
            if (_class.Data.SubItems.Count == 0)
            {
                _class.Shutter.Error = "No profiles found";
                _class.Shutter.Explain = "Please set up your console display then use Save Profile";
            }
            else
            {
                SelectSubItem();
                _class.Shutter.SetActive(currentRow + 1);
            }
        }

        private void SaveProfile(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.SubAction.AddSubItem("PlayStation3", "PlayStation3");
            _class.SubAction.AddSubItem("PlayStation4", "PlayStation4");
            _class.SubAction.AddSubItem("Xbox360", "Xbox360");
            _class.SubAction.AddSubItem("XboxOne", "XboxOne");

            SelectSubItem();
            _class.Shutter.SetActive(currentRow + 1);
        }

        private void VideoInput(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();
            
            var crossbar = new Crossbar();
            crossbar.GetDataHandle(_class.Data);
            crossbar.GetShutterHandle(_class.Shutter);
            crossbar.GetSubActionHandle(_class.SubAction);
            crossbar.GetSystemHandle(_class.System);
            crossbar.GetVideoCapture(_class.VideoCapture);
            
            crossbar.Find();
            SelectSubItem();
            _class.Shutter.SetActive(currentRow + 1);
        }

        private void VideoDevice(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            var capture = new CaptureDevice(_class);

            capture.Find();
            SelectSubItem();
            _class.Shutter.SetActive(currentRow + 1);
        }

        private void VideoSettings(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.SubAction.AddSubItem("Crossbar", "Crossbar", CheckSystemSetting("Crossbar"));
            _class.SubAction.AddSubItem("AVIRender", "AVI Renderer", CheckSystemSetting("AVIRender"));
            _class.SubAction.AddSubItem("CheckCaptureRes", "Check Capture", CheckSystemSetting("CheckCaptureRes"));
            _class.SubAction.AddSubItemFolder("Resolution", "Resolution", "Capture Resolution");

            SelectSubItem();
            _class.Shutter.SetActive(currentRow + 1);
        }

        private void VideoDisplay(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.SubAction.AddSubItem("AutoSet", "Auto Set", _class.System.boolAutoSetResolution);
            _class.SubAction.AddSubItemFolder("Device", "Graphics Device", "Graphics Card");
            _class.SubAction.AddSubItemFolder("Resolution", "Resolution", "Display Resolution");
            _class.SubAction.AddSubItemFolder("Refresh", "Refresh Rate", "Screen Refresh");
            _class.SubAction.AddSubItemFolder("Volume", "Volume", "Volume");
            _class.SubAction.AddSubItem("StayOnTop", "Stay On Top", _class.System.boolAutoSetResolution);

            SelectSubItem();
            _class.Shutter.SetActive(currentRow + 1);
        }

        private void Device(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.SubAction.AddSubItem("DS4 Emulation", "DS4 Emulation");
            _class.SubAction.AddSubItem("Normalize", "Normalize");

            CheckDisplaySettings();

            SelectSubItem();
            _class.Shutter.SetActive(currentRow - 1);
        }

        private void ControllerOutput(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.SubAction.AddSubItem("ControllerMax", "ControllerMax");

            _class.SubAction.AddSubItem("TitanOne", "TitanOne", _listTo.GetToCount("TitanOne").ToString());
            RegisterWatcher("TitanOne");

            _class.SubAction.AddSubItem("GIMX", "GIMX");
            _class.SubAction.AddSubItem("Remote GIMX", "Remote GIMX");
            _class.SubAction.AddSubItem("McShield", "McShield");
            _class.SubAction.AddSubItem("Control VJOY", "Control VJOY");

            CheckDisplaySettings();
            SelectSubItem();
            _class.Shutter.SetActive(currentRow - 1);
        }

        private void Remap(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.SubAction.AddSubItem("Gamepad", "Gamepad");
            _class.SubAction.AddSubItem("Keyboard", "Keyboard");
            _class.SubAction.AddSubItem("Mouse", "Mouse");
            _class.SubAction.AddSubItem("Touch", "Touch");

            SelectSubItem();
            _class.Shutter.SetActive(currentRow - 1);
        }

        private void Config(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.Shutter.SetActive(currentRow - 1);
        }

        private void Exit(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();

            _class.SubAction.AddSubItem("exit", "Yes");
            _class.SubAction.AddSubItem("back", "No");

            _class.User.SubSelected = _class.Data.SubItems[1].Command;
            _class.Shutter.SetActive(currentRow - 1);
        }

        public void SelectSubItem()
        {
            if (_class.Data.SubItems.Count > 0)
            {
                _class.User.SubSelected = _class.Data.SubItems[0].Command;
            }
        }

        private bool CheckSystemSetting(string strCommand)
        {
            return _class.System.checkUserSetting(strCommand.ToLower()).ToLower() == "true";
        }

        private void RegisterWatcher(string title)
        {
            foreach (var t in _class.Data.SubItems)
                if (t.Display == title) t.ActiveWatcher = title;
        }

        public void CheckDisplaySettings()
        {
            _class.Data.Checked.Clear();

            foreach (var t in _class.Data.SubItems)
            {
                if (t.Command.ToLower() == "ds4 emulation")
                    if (_class.System.boolPS4ControllerMode) _class.Data.Checked.Add("DS4 Emulation");

                if (t.Command.ToLower() == "normalize")
                    if (_class.System.boolNormalizeControls) _class.Data.Checked.Add("Normalize");

                if (t.Command.ToLower() == "controllermax")
                    if (_class.System.boolControllerMax) _class.Data.Checked.Add("ControllerMax");

                if (t.Command.ToLower() == "titanone")
                    if (_class.System.boolTitanOne) _class.Data.Checked.Add("TitanOne");

                if (t.Command.ToLower() == "gimx")
                    if (_class.System.boolGIMX) _class.Data.Checked.Add("GIMX");

                if (t.Command.ToLower() == "remote gimx")
                    if (_class.System.boolRemoteGIMX) _class.Data.Checked.Add("remote gimx");

                if (t.Command.ToLower() == "McShield")
                    if (_class.System.boolMcShield) _class.Data.Checked.Add("McShield");

                if (t.Command.ToLower() == "control vjoy")
                    if (_class.System.boolControlVJOY) _class.Data.Checked.Add("Control VJOY");

                if (t.Command.ToLower() == "crossbar")
                    if (CheckSystemSetting("Crossbar")) _class.Data.Checked.Add("Crossbar");

                if (t.Command.ToLower() == "avirender")
                    if (CheckSystemSetting("AVIRender")) _class.Data.Checked.Add("AVI Renderer");

                if (t.Command.ToLower() == "checkcaptureres")
                    if (CheckSystemSetting("CheckCaptureRes")) _class.Data.Checked.Add("Check Capture");
            }
        }


    }
}
