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

        private readonly ListDevices _listTo = new ListDevices();

        public void GetActionHandle(MainMenu.Action action) { _action = action; }
        public void GetForm1Handle(Form1 form1) { _form1 = form1; }
        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetNavigationHandle(Navigation nav) { _nav = nav; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVariableHandle(Variables var) { _var = var; }

        public void ProcessSubMenu(string command)
        {
            switch (_user.Menu)
            {
                case "console select": LoadProfile(command); break;
                case "video input": ChangeCrossbar(command); break;
                case "video device": ChangeVideoDevice(command); break;
                case "safe profile": SaveProfile(command); break;
                case "controller output": ChangeSetting(command); break;
                case "device": break;
                case "video settings": break;
                case "resolution": break;
                case "video display": break;
                case "remap": break;
                case "video resolution": break;
                case "video refresh": break;
                case "exit": Exit(command);break;
            }
            /*
            if (_user.Menu == "device") ChangeSetting(strCommand);
            if (_user.Menu == "video settings") ChangeSetting(strCommand);
            if (_user.Menu == "resolution") ChangeResolution(strCommand);
            if (_user.Menu == "video display") ChangeVideoDisplay(strCommand);
            if (_user.Menu == "remap") ChangeRemapScreen(strCommand);

            if (_user.Menu == "videoresolution") ChangeVideoResolution(strCommand);
            if (_user.Menu == "videorefresh") ChangeVideoRefresh(strCommand);

             */
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
            //if (strCommand == "titanone") ChangeTitanOne(); 

            //if (strCommand == "resolution") ListCaptureResolution();
            if (strCommand == "avirender") _system.changeAVIRender();
            if (strCommand == "checkcaptureres") _system.changeCaptureAutoRes();

            _action.CheckDisplaySettings();
        }

        private void ChangeTitanOne()
        {
            /*
            if (_listTo.GetToCount("TitanOne") > 1)
                ListAllTitanOne();
            else
                _system.changeTitanOne();
             */
        }

        private void ListAllTitanOne()
        {
            /*
            var toList = _listTo.FindDevices("TitanOne");
            _var.ShowSubSelection = true;
             */
        }

        private void ChangeVideoDisplay(string command)
        {
            /*
            command = command.ToLower();
            if (command == "autoset") ChangeAutoRes();
            if (command == "resolution") ListDisplayResolution();
            if (command == "refresh") ListDisplayRefresh();
            if (command == "stayontop") ChangeStayOnTop();
             */
        }

        private void Exit(string command)
        {
            command = command.ToLower();
            if (command == "exit") _form1.closeSystem();
            if (command == "back") _nav.MenuBack();
        }

    }
}
