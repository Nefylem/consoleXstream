using System.Linq;
using consoleXstream.Config;
using consoleXstream.Menu.Data;
using consoleXstream.Output;

namespace consoleXstream.Menu.SubMenu
{
    class Action
    {
        private Form1 _form1;
        private Configuration _system;
        private Interaction _data;
        private Navigation _nav;
        private User _user;
        private Variables _var;
        private MainMenu.Action _action;
        private SubMenuOptions.Display _display;
        private Shutter _shutter;
        private VideoCapture.VideoCapture _videoCapture;
        private SubMenuOptions.Remap _remap;

        private readonly ListDevices _listTo = new ListDevices();

        public void GetActionHandle(MainMenu.Action action) { _action = action; }
        public void GetForm1Handle(Form1 form1) { _form1 = form1; }
        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetNavigationHandle(Navigation nav) { _nav = nav; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVariableHandle(Variables var) { _var = var; }
        public void GetShutterHandle(Shutter shutter) { _shutter = shutter; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture videoCapture) { _videoCapture = videoCapture; }

        public void ProcessSubMenu(string command)
        {
            switch (_user.Menu)
            {
                case "console select": LoadProfile(command); break;
                case "video input": ChangeCrossbar(command); break;
                case "video device": ChangeVideoDevice(command); break;
                case "safe profile": SaveProfile(command); break;
                case "controller output": ChangeSetting(command); break;
                case "device": ChangeSetting(command); break;
                case "video settings": ChangeSetting(command); break;
                case "resolution": ChangeResolution(command); break;
                case "video display": ChangeVideoDisplay(command); break;
                case "remap": ChangeRemapScreen(command); break;
                case "video resolution": ChangeVideoResolution(command); break;
                case "video refresh": ChangeVideoRefresh(command); break;
                case "exit": Exit(command); break;
            }
        }

        public void AddSubItem(string strCommand, string strTitle)
        {
            _data.SubItems.Add(new DisplayItem());
            var intId = _data.SubItems.Count - 1;

            _data.SubItems[intId].Command = strCommand;
            _data.SubItems[intId].Display = strTitle;
            _data.SubItems[intId].DisplayOption = "";
            _data.SubItems[intId].ActiveWatcher = "";
        }

        public void AddSubItem(string command, string title, string subtitle)
        {
            _data.SubItems.Add(new DisplayItem());
            var id = _data.SubItems.Count - 1;

            _data.SubItems[id].Command = command;
            _data.SubItems[id].Display = title;
            _data.SubItems[id].DisplayOption = subtitle;
            _data.SubItems[id].ActiveWatcher = "";
        }

        public void AddSubItem(string strCommand, string strTitle, bool isCheck)
        {
            _data.SubItems.Add(new DisplayItem());
            var id = _data.SubItems.Count - 1;

            _data.SubItems[id].Command = strCommand;
            _data.SubItems[id].Display = strTitle;
            _data.SubItems[id].DisplayOption = "";
            _data.SubItems[id].ActiveWatcher = "";

            if (isCheck)
                _data.Checked.Add(strTitle);
        }

        public void AddSubItem(string strCommand, string strTitle, string subTitle, bool isCheck)
        {
            _data.SubItems.Add(new DisplayItem());
            var id = _data.SubItems.Count - 1;

            _data.SubItems[id].Command = strCommand;
            _data.SubItems[id].Display = strTitle;
            _data.SubItems[id].DisplayOption = "";
            _data.SubItems[id].ActiveWatcher = "";

            if (isCheck)
                _data.Checked.Add(strTitle);
        }

        public void AddSubItemFolder(string command, string title, string setData)
        {
            _data.SubItems.Add(new DisplayItem());
            var id = _data.SubItems.Count - 1;

            _data.SubItems[id].Command = command;
            _data.SubItems[id].Display = title;
            _data.SubItems[id].IsFolder = true;
            _data.SubItems[id].DisplayOption = FindSubOption(setData);
            _data.SubItems[id].ActiveWatcher = "";
        }

        public string FindSubOption(string command)
        {
            command = command.ToLower();

            switch (command)
            {
                case "capture resolution": return _system.strCurrentResolution;
                case "graphics card": return _system.getGraphicsCard();
                case "screen refresh": return _system.getRefreshRate();
                case "display resolution": return _system.getResolution();
                case "volume": return _system.getVolume();
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
            foreach (var t in _data.SubItems.Where(t => t.Display == title))
                t.IsFolder = set;
        }

        private void LoadProfile(string command)
        {
            var profile = new SubMenuOptions.Profiles();
            profile.Load(command);
        }

        private void ChangeCrossbar(string command)
        {
            var crossbar = new SubMenuOptions.Crossbar();
            crossbar.Change(command);
        }

        private void ChangeVideoDevice(string command)
        {
            var videoDevice = new SubMenuOptions.CaptureDevice();
            videoDevice.Change(command);
        }

        private void SaveProfile(string command)
        {
            var profile = new SubMenuOptions.Profiles();
            profile.Save(command);            
        }

        private void ChangeSetting(string strCommand)
        {
            strCommand = strCommand.ToLower();
            if (strCommand == "ds4 emulation") _system.changeDS4Emulation();
            if (strCommand == "normalize") _system.changeNormalizeGamepad();
            if (strCommand == "controllermax") _system.changeControllerMax();
            if (strCommand == "titanone") ChangeTitanOne(); 
            if (strCommand == "resolution") ListCaptureResolution();
            if (strCommand == "avirender") _system.changeAVIRender();
            if (strCommand == "checkcaptureres") _system.changeCaptureAutoRes();

            _action.CheckDisplaySettings();
        }

        private void ChangeTitanOne()
        {
            if (_listTo.GetToCount("TitanOne") > 1)
                ListAllTitanOne();
            else
                _system.changeTitanOne();
        }

        private void ListAllTitanOne()
        {
            var toList = _listTo.FindDevices("TitanOne");
            _var.ShowSubSelection = true;
        }

        private void ChangeVideoDisplay(string command)
        {
            _display = new SubMenuOptions.Display();
            _display.GetSystemHandle(_system);
            _display.GetDataHandle(_data);
            _display.GetShutterHandle(_shutter);
            _display.GetUserHandle(_user);
            _display.GetSubActionHandle(this);
            _display.ChangeVideoDisplay(command);
        }

        private void Exit(string command)
        {
            command = command.ToLower();
            if (command == "exit") _form1.closeSystem();
            if (command == "back") _nav.MenuBack();
        }

        private void ChangeResolution(string command)
        {
            _display = new SubMenuOptions.Display();
            _display.GetSystemHandle(_system);
            _display.GetVideoCaptureHandle(_videoCapture);

            _display.ChangeResolution(command);
        }

        private void ChangeVideoRefresh(string command)
        {
            _display = new SubMenuOptions.Display();
            _display.GetSystemHandle(_system);
            _display.GetDataHandle(_data);

            _display.ChangeVideoRefresh(command);
        }

        private void ChangeVideoResolution(string command)
        {
            _display = new SubMenuOptions.Display();
            _display.GetSystemHandle(_system);
            _display.GetDataHandle(_data);

            _display.ChangeVideoResolution(command);
        }

        private void ChangeRemapScreen(string command)
        {
            _remap = new SubMenuOptions.Remap();
            _remap.GetDataHandle(_data);
            _remap.GetVariableHandle(_var);
            _remap.GetUserHandle(_user);

            _remap.ChangeRemapScreen(command);
        }

        private void ListCaptureResolution()
        {
            var videoDevice = new SubMenuOptions.CaptureDevice();
            videoDevice.GetUserHandle(_user);
            videoDevice.ListCaptureResolution();

        }


    }
}
