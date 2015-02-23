using System;
using System.Collections.Generic;
using System.Linq;
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


        private readonly ListDevices _listTo = new ListDevices();

        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetConfigHandle(Configuration config) { _system = config; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetNavHandle(Navigation nav) { _nav = nav; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVariableHandle(Variables inVar) { _var = inVar; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }

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
            crossbar.Find();
            //FindVideoCrossbar();
            //ActivateShutter(intCurrentRow + 1);
        }

        private void VideoDevice(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();
            var capture = new CaptureDevice();
            capture.Find();
            //ClearSub(strCommand);
            //FindVideoDevice();
            //ActivateShutter(intCurrentRow + 1);
        }

        private void VideoSettings(string command, int currentRow)
        {
            SetMenu(command);
            ClearSub();
            /*
            ClearSub(strCommand);
            ActivateShutter(intCurrentRow + 1);

            AddSubItem("Crossbar", "Crossbar", CheckSystemSetting("Crossbar"));
            AddSubItem("AVIRender", "AVI Renderer", CheckSystemSetting("AVIRender"));
            AddSubItem("CheckCaptureRes", "Check Capture", CheckSystemSetting("CheckCaptureRes"));
            AddSubItemFolder("Resolution", "Resolution", "Capture Resolution");

            SelectSubItem();
            */
        }

        private void VideoDisplay(string command, int currentRow)
        {
            /*
            ClearSub(strCommand);
            ActivateShutter(intCurrentRow + 1);

            AddSubItem("AutoSet", "Auto Set", _system.boolAutoSetResolution);
            AddSubItemFolder("Device", "Graphics Device", "Graphics Card");
            AddSubItemFolder("Resolution", "Resolution", "Display Resolution");
            AddSubItemFolder("Refresh", "Refresh Rate", "Screen Refresh");
            AddSubItemFolder("Volume", "Volume", "Volume");

            AddSubItem("StayOnTop", "Stay On Top", _system.boolAutoSetResolution);

            SelectSubItem();
            */
        }

        private void Device(string command, int currentRow)
        {
            /*
            ClearSub(strCommand);
            ActivateShutter(intCurrentRow + 1);

            AddSubItem("AutoSet", "Auto Set", _system.boolAutoSetResolution);
            AddSubItemFolder("Device", "Graphics Device", "Graphics Card");
            AddSubItemFolder("Resolution", "Resolution", "Display Resolution");
            AddSubItemFolder("Refresh", "Refresh Rate", "Screen Refresh");
            AddSubItemFolder("Volume", "Volume", "Volume");

            AddSubItem("StayOnTop", "Stay On Top", _system.boolAutoSetResolution);

            SelectSubItem();
            */
        }

        private void ControllerOutput(string command, int currentRow)
        {
            /*
            ClearSub(strCommand);

            AddSubItem("ControllerMax", "ControllerMax");

            AddSubItem("TitanOne", "TitanOne", _listTo.GetToCount("TitanOne").ToString());
            RegisterWatcher("TitanOne");

            AddSubItem("GIMX", "GIMX");
            AddSubItem("Remote GIMX", "Remote GIMX");
            AddSubItem("McShield", "McShield");
            AddSubItem("Control VJOY", "Control VJOY");

            CheckDisplaySettings();
            _user.SubSelected = _listSubItems[0].Command;

            ActivateShutter(intCurrentRow - 1);
            */
        }

        private void Remap(string command, int currentRow)
        {
            /*
            ClearSub(strCommand);

            AddSubItem("Gamepad", "Gamepad");
            AddSubItem("Keyboard", "Keyboard");
            AddSubItem("Mouse", "Mouse");
            //addSubItem("Touch", "Touch");

            _user.SubSelected = _listSubItems[0].Command;
            ActivateShutter(intCurrentRow - 1);
            */
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

    }
}
