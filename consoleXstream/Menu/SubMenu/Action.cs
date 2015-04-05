using System.Collections.Generic;
using System.Linq;
using consoleXstream.Menu.Data;
using consoleXstream.Output;

namespace consoleXstream.Menu.SubMenu
{
    public class Action
    {

        public Action(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        private readonly CountDevices _listTo = new CountDevices();

        public void ProcessSubMenu(string command)
        {
            switch (_class.User.Menu)
            {
                case "console select": LoadProfile(command); break;
                case "save profile": SaveProfile(command); break;
                case "vr": _class.ConfigVr.SetVrOptions(command); break;
                case "video input": ChangeCrossbar(command); break;
                case "video device": ChangeVideoDevice(command); break;
                case "controller output": ChangeSetting(command); break;
                case "device": ChangeSetting(command); break;
                case "video settings": ChangeSetting(command); break;
                case "resolution": ChangeResolution(command); break;
                case "video display": ChangeVideoDisplay(command); break;
                case "remap": ChangeRemapScreen(command); break;
                case "videoresolution": ChangeVideoResolution(command); break;      //Video display sub menu
                case "videorefresh": ChangeVideoRefresh(command); break;
                case "exit": Exit(command); break;
            }
        }

        public void AddSubItem(string strCommand, string strTitle)
        {
            _class.Data.SubItems.Add(new DisplayItem());
            var intId = _class.Data.SubItems.Count - 1;

            _class.Data.SubItems[intId].Command = strCommand;
            _class.Data.SubItems[intId].Display = strTitle;
            _class.Data.SubItems[intId].DisplayOption = "";
            _class.Data.SubItems[intId].ActiveWatcher = "";
        }

        public void AddSubItem(string command, string title, string subtitle)
        {
            _class.Data.SubItems.Add(new DisplayItem());
            var id = _class.Data.SubItems.Count - 1;

            _class.Data.SubItems[id].Command = command;
            _class.Data.SubItems[id].Display = title;
            _class.Data.SubItems[id].DisplayOption = subtitle;
            _class.Data.SubItems[id].ActiveWatcher = "";
        }

        public void AddSubItem(string strCommand, string strTitle, bool isCheck)
        {
            _class.Data.SubItems.Add(new DisplayItem());
            var id = _class.Data.SubItems.Count - 1;

            _class.Data.SubItems[id].Command = strCommand;
            _class.Data.SubItems[id].Display = strTitle;
            _class.Data.SubItems[id].DisplayOption = "";
            _class.Data.SubItems[id].ActiveWatcher = "";

            if (isCheck)
                _class.Data.Checked.Add(strTitle);
        }

        public void AddSubItem(string strCommand, string strTitle, string subTitle, bool isCheck)
        {
            _class.Data.SubItems.Add(new DisplayItem());
            var id = _class.Data.SubItems.Count - 1;

            _class.Data.SubItems[id].Command = strCommand;
            _class.Data.SubItems[id].Display = strTitle;
            _class.Data.SubItems[id].DisplayOption = "";
            _class.Data.SubItems[id].ActiveWatcher = "";

            if (isCheck)
                _class.Data.Checked.Add(strTitle);
        }

        public void AddSubItemFolder(string command, string title, string setData)
        {
            _class.Data.SubItems.Add(new DisplayItem());
            var id = _class.Data.SubItems.Count - 1;

            _class.Data.SubItems[id].Command = command;
            _class.Data.SubItems[id].Display = title;
            _class.Data.SubItems[id].IsFolder = true;
            _class.Data.SubItems[id].DisplayOption = FindSubOption(setData);
            _class.Data.SubItems[id].ActiveWatcher = "";
        }

        public string FindSubOption(string command)
        {
            command = command.ToLower();

            switch (command)
            {
                case "capture resolution": return _class.Base.System.strCurrentResolution;
                case "graphics card": return _class.Base.System.GetGraphicsCard();
                case "screen refresh": return _class.Base.System.GetRefreshRate();
                case "display resolution": return _class.Base.System.GetResolution();
                case "volume": return _class.Base.System.GetVolume();
                case "titanone":
                    var toCount = _listTo.GetToCount("TitanOne");
                    ChangeToFolder("TitanOne", toCount > 1);

                    var ret = toCount + " device";
                    if (toCount != 1) ret += "s";
                    ret += "\nconnected";

                    return ret;
            }
            return "";
        }

        private void ChangeToFolder(string title, bool set)
        {
            foreach (var t in _class.Data.SubItems.Where(t => t.Display == title))
                t.IsFolder = set;
        }

        private void LoadProfile(string command)
        {
            /*
            var profile = new SubMenuOptions.Profiles();
            profile.GetUserHandle(_class.User);
            profile.GetDataHandle(_class.Data);
            profile.GetVideoCaptureHandle(_class.VideoCapture);
            profile.GetSystemHandle(_class.System);
            profile.Load(command);
             */
            _class.Profiles.Load(command);
        }

        private void ChangeCrossbar(string command)
        {
            var crossbar = new SubMenuOptions.Crossbar();
            crossbar.GetVideoCapture(_class.Base.VideoCapture);
            crossbar.GetSystemHandle(_class.Base.System);
            crossbar.Change(command);
        }

        private void ChangeVideoDevice(string command)
        {
            var videoDevice = new SubMenuOptions.CaptureDevice(_class);

            videoDevice.Change(command);
        }

        private void SaveProfile(string command)
        {
            /*
            var profile = new SubMenuOptions.Profiles();
            profile.GetDataHandle(_class.Data);
            profile.GetUserHandle(_class.User);
            profile.GetSystemHandle(_class.System);
            profile.GetVideoCaptureHandle(_class.VideoCapture);
            profile.Save(command);
             */
            _class.Profiles.Save(command);
            _class.Shutter.Close();
        }

        private void ChangeSetting(string command)
        {
            command = command.ToLower();
            if (command == "ds4 emulation") _class.Base.System.ChangeDs4Emulation();
            if (command == "normalize") _class.Base.System.ChangeNormalizeGamepad();
            if (command == "rumble") _class.Base.System.Class.Gamepad.ChangeRumble();
            if (command == "controllermax") _class.Base.System.ChangeControllerMax();
            if (command == "titanone") ChangeTitanOne();
            if (command == "resolution") ListCaptureResolution();
            if (command == "crossbar") _class.Base.System.ChangeCrossbar();
            if (command == "avirender") _class.Base.System.ChangeAviRender();
            if (command == "checkcaptureres") _class.Base.System.ChangeCaptureAutoRes();

            _class.Action.CheckDisplaySettings();
        }

        private void ChangeTitanOne()
        {
            _class.Data.ClearButtons();

            if (_listTo.GetToCount("TitanOne") > 1)
                ListAllTitanOne();
            else
            {
                _class.Base.System.ChangeTitanOne();
                //_class.Form1.InitTitanOne();
                //_class.Form1.SetTitanOneDevice();
            }
        }

        private void ListAllTitanOne()
        {
            if (_class.Base.Home.ListToDevices == null)
                _class.Base.Home.ListToDevices = new List<string>();

            if (_class.Base.Home.ListToDevices.Count > 0)
            {
                _class.SubSelectVar.TitanSerial = _class.Base.System.TitanOneDevice;
                _class.SubSelectVar.ListData = _class.Base.Home.ListToDevices;
                _class.Base.Home.SetTitanOneMode("Multi");

                _class.SubNav.SetMenuOkWait();
                _class.Var.ShowSubSelection = true;
            }
            else
            {
                _class.SubSelectVar.DisplayTitle = "Finding Devices";
                _class.SubSelectVar.DisplayMessage = "Please wait";

                _class.SubSelectVar.ListData.Clear();
                _class.SubSelectVar.TitanSerial = _class.Base.System.TitanOneDevice;

                _class.Base.Home.SetTitanOneMode("Multi");
                _class.Base.Home.ListTitanOneDevices();

                if (_class.Base.Home.GetTitanOne().Length > 0)
                    _class.Data.Checked.Add("TitanOne");

                _class.SubNav.SetMenuOkWait();
                _class.Var.ShowSubSelection = true;
            }
        }

        private void ChangeVideoDisplay(string command)
        {
            _class.Display.ChangeVideoDisplay(command);
        }

        private void Exit(string command)
        {
            command = command.ToLower();
            if (command == "exit") _class.Base.Home.CloseSystem();
            if (command == "back") _class.Nav.MenuBack();
        }

        private void ChangeResolution(string command)
        {
            var display = new SubMenuOptions.Display(_class);

            display.ChangeResolution(command);
        }

        private void ChangeVideoRefresh(string command)
        {
            _class.Display.ChangeVideoRefresh(command);
        }

        private void ChangeVideoResolution(string command)
        {
            _class.Display.ChangeVideoResolution(command);
        }

        private void ChangeRemapScreen(string command)
        {
            _class.RemapNav._moveOkWait = 5;
            _class.RemapMenu.ChangeRemapScreen(command);
        }

        private void ListCaptureResolution()
        {
            _class.VideoDevice.ListCaptureResolution();
        }
    }
}
