using System.Linq;
using System.Windows.Forms;
using consoleXstream.Config;
using consoleXstream.Menu.Data;
using consoleXstream.Output;
using DirectShowLib.BDA;

namespace consoleXstream.Menu.SubMenu
{
    public class Action
    {

        public Action(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        /*
        private Form1 _form1;
        private ShowMenu _menu;
        private Configuration _class.System;
        private Interaction _class.Data;
        private Navigation _nav;
        private User _class.User;
        private Variables _class.Var;
        private MainMenu.Action _class.Action;
        private SubMenuOptions.Display _display;
        private Shutter _class.Shutter;
        private VideoCapture.VideoCapture _class.VideoCapture;
        private SubMenuOptions.Remap _remap;
        */
        private readonly CountDevices _listTo = new CountDevices();
        /*
        public void GetMenuHandle(ShowMenu menu) { _menu = menu; }
        public void GetActionHandle(MainMenu.Action action) { _class.Action = action; }
        public void GetForm1Handle(Form1 form1) { _form1 = form1; }
        public void GetSystemHandle(Configuration system) { _class.System = system; }
        public void GetDataHandle(Interaction data) { _class.Data = data; }
        public void GetNavigationHandle(Navigation nav) { _nav = nav; }
        public void GetUserHandle(User user) { _class.User = user; }
        public void GetVariableHandle(Variables var) { _class.Var = var; }
        public void GetShutterHandle(Shutter shutter) { _class.Shutter = shutter; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture videoCapture) { _class.VideoCapture = videoCapture; }
        */
        public void ProcessSubMenu(string command)
        {
            switch (_class.User.Menu)
            {
                case "console select": LoadProfile(command); break;
                case "save profile": SaveProfile(command); break;
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
                case "capture resolution": return _class.System.strCurrentResolution;
                case "graphics card": return _class.System.getGraphicsCard();
                case "screen refresh": return _class.System.getRefreshRate();
                case "display resolution": return _class.System.getResolution();
                case "volume": return _class.System.getVolume();
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
            var profile = new SubMenuOptions.Profiles();
            profile.GetUserHandle(_class.User);
            profile.GetDataHandle(_class.Data);
            profile.GetVideoCaptureHandle(_class.VideoCapture);
            profile.GetSystemHandle(_class.System);
            profile.Load(command);
        }

        private void ChangeCrossbar(string command)
        {
            var crossbar = new SubMenuOptions.Crossbar();
            crossbar.GetVideoCapture(_class.VideoCapture);
            crossbar.GetSystemHandle(_class.System);
            crossbar.Change(command);
        }

        private void ChangeVideoDevice(string command)
        {
            var videoDevice = new SubMenuOptions.CaptureDevice(_class);

            videoDevice.Change(command);
        }

        private void SaveProfile(string command)
        {
            var profile = new SubMenuOptions.Profiles();
            profile.GetDataHandle(_class.Data);
            profile.GetUserHandle(_class.User);
            profile.GetSystemHandle(_class.System);
            profile.GetVideoCaptureHandle(_class.VideoCapture);
            profile.Save(command);
            _class.Shutter.Close();
        }

        private void ChangeSetting(string command)
        {
            command = command.ToLower();
            if (command == "ds4 emulation") _class.System.changeDS4Emulation();
            if (command == "normalize") _class.System.changeNormalizeGamepad();
            if (command == "controllermax") _class.System.changeControllerMax();
            if (command == "titanone") ChangeTitanOne();
            if (command == "resolution") ListCaptureResolution();
            if (command == "crossbar") _class.System.ChangeCrossbar();
            if (command == "avirender") _class.System.ChangeAviRender();
            if (command == "checkcaptureres") _class.System.changeCaptureAutoRes();

            _class.Action.CheckDisplaySettings();
        }

        private void ChangeTitanOne()
        {
            _class.System.changeTitanOne();
            /*
            if (_listTo.GetToCount("TitanOne") > 1)
                ListAllTitanOne();
            else
                _class.System.changeTitanOne();
             */
        }

        private void ListAllTitanOne()
        {
            _class.Var.ShowSubSelection = true;
            _class.SubSelectVar.DisplayTitle = "Finding Devices";
            _class.SubSelectVar.DisplayMessage = "Please wait";

            _class.Form1.ListTitanOneDevices();
        }

        private void ChangeVideoDisplay(string command)
        {
            _class.Display.ChangeVideoDisplay(command);
        }

        private void Exit(string command)
        {
            command = command.ToLower();
            if (command == "exit") _class.Form1.CloseSystem();
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
            //_class.SubRemap.ChangeRemapScreen(command);
        }

        private void ListCaptureResolution()
        {
            _class.VideoDevice.ListCaptureResolution();
        }
    }
}
