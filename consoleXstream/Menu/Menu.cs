using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using consoleXstream.Config;
using consoleXstream.DrawGui;
using consoleXstream.Input;
using consoleXstream.Menu.Data;
using consoleXstream.Menu.MainMenu;
using consoleXstream.Remap;
using consoleXstream.Output;
using consoleXstream.VideoCapture;

namespace consoleXstream.Menu
{
    public partial class ShowMenu : Form
    {
        private Form1 _form1;
        private Configuration _system;
        private DrawGraph _drawGui;
        private VideoCapture.VideoCapture _videoCapture;
        private Remapping _remap;
        private KeyboardHook _keyboardHook;

        private MainMenu.Action _action;
        private SubMenu.Action _subAction;
        private ButtonItem _button;
        private Create _createMain;
        private MainMenu.Menu _mainMenu;
        private SubMenu.Menu _subMenu;
        private Navigation _nav;
        private Variables _var;
        private Interaction _data;
        private User _user;
        private FrameCount _fps;
        private Mouse _mouse;
        private SubMenu.Shutter _shutter;
        private Gamepad _gamepad;
        private Keyboard _keyboard;
        private Keymap _keymap;

        private readonly List<PreviewItem> _previewVideo = new List<PreviewItem>();

        private int _intMenuOk;

        private int _gamepadCount;

        private bool _boolShowPreview;

        private string _strRemapSelected;


        public ShowMenu(Form1 form1)
        {
            _form1 = form1;
            InitializeComponent();
        }

        public void GetSystemHandle(Configuration inSystem) { _system = inSystem; }
        public void GetKeyboardHookHandle(KeyboardHook inKey) { _keyboardHook = inKey; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture inVideo) { _videoCapture = inVideo; }
        public void GetRemapHandle(Remap.Remapping inMap) { _remap = inMap; }
        public void GetKeymapHandle(Keymap keymap) { _keymap = keymap; }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            _user.ConnectProfile = "";
        }

        //Hide from Alt-Tab / Task Manager
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;  // Turn on WS_EX_TOOLWINDOW
                return cp;
            }
        }

        public void ClosePanel()
        {
            tmrMenu.Enabled = false;
            _drawGui = null;
            Hide();
            _form1.closeMenuForm();
        }

        public void ShowPanel()
        {
            DeclareClasses();

            if (_form1 == null) { _form1 = (Form1)Application.OpenForms["Form1"]; }
            _nav.ListHistory = new List<string>();

            _createMain.Menu();

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;

            TransparencyKey = Color.BlanchedAlmond;
            BackColor = Color.BlanchedAlmond;

            imgDisplay.Dock = DockStyle.Fill;

            //Make as close to invisible as possible, to stop the menu "flashing" into view
            Width = 1;
            Height = 1;
            Opacity = 0.1;

            if (_system.boolStayOnTop)
                TopMost = true;

            Show();

            Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (Properties.Resources.imgMainMenu.Width / 2);
            Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (Properties.Resources.imgMainMenu.Height / 2);
            
            //Enlarge once in the right spot
            Width = Properties.Resources.imgMainMenu.Width;
            Height = Properties.Resources.imgMainMenu.Height;
            
            Opacity = 0.90;
            _var.IsMainMenu = true;

            _nav.SetBackWait(5);
            _mouse.GetMenuSize(Width, Height);

            tmrMenu.Enabled = true;
        }

        private void tmrMenu_Tick(object sender, EventArgs e)
        {
            _nav.CheckDelays();
            _fps.CheckFps();
            _shutter.CheckDisplay();

            DrawPanel();

            CheckControls();

            BringToFront();
            Focus();
        }

        private void DrawPanel()
        {
            
            _drawGui.ClearGraph(Properties.Resources.imgMainMenu);
            
            if (_var.Setup)
            {
                /*
                if (_var.SetupGamepad)
                    DrawGamepadRemap();
                 */
            }
            else
            {
                _mainMenu.Draw();

                if (!_var.IsMainMenu)
                {
                    _subMenu.Draw();
                    if (_var.ShowSubSelection) DrawSubSelectionMenu();
                }
                //drawFooter();
            }

            _drawGui.setOutline(false);

            if (_system.boolFPS)
                _drawGui.drawText(5, 500, "FPS: " + _fps.Frames);

            imgDisplay.Image = _drawGui.drawGraph();
        }

        private void DrawSubSelectionMenu()
        {
            var subSelect = new Bitmap(Properties.Resources.imgSubSelect);

            _drawGui.drawImage(new Rectangle(15, 240, 570, 100), subSelect);
        }


        private void RegisterWatcher(string title)
        {
            foreach (var t in _data.SubItems)
                if (t.Display == title) t.ActiveWatcher = title;
        }

        private void CheckControls()
        {
            _keyboard.CheckInput();
            _gamepad.CheckInput();
        }

        private void imgDisplay_MouseMove(object sender, MouseEventArgs e) { _mouse.MouseMove(e); }
        private void imgDisplay_MouseEnter(object sender, EventArgs e) { _mouse.Set(true); }
        private void imgDisplay_MouseLeave(object sender, EventArgs e) { _mouse.Set(false); }
        private void imgDisplay_Click(object sender, EventArgs e) { _mouse.Click(e); }

        #region Action Handlers

        private bool CheckSystemSetting(string strCommand)
        {
            return _system.checkUserSetting(strCommand.ToLower()).ToLower() == "true";
        }

        //TODO: look into list for last selected object / scroll. For now, just revert to 1
        #endregion

        #region Menu Actions






        private void ChangeSetting(string strCommand)
        {
            /*
            strCommand = strCommand.ToLower();
            if (strCommand == "ds4 emulation") _system.changeDS4Emulation();
            if (strCommand == "normalize") _system.changeNormalizeGamepad();
            if (strCommand == "controllermax") _system.changeControllerMax();
            
            if (strCommand == "titanone") ChangeTitanOne(); 

            if (strCommand == "resolution") ListCaptureResolution();
            if (strCommand == "avirender") _system.changeAVIRender();
            if (strCommand == "checkcaptureres") _system.changeCaptureAutoRes();

            CheckDisplaySettings();
             */
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

        private void CheckDisplaySettings()
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

        #endregion

        private void DeclareClasses()
        {
            CreateClasses();

            SetupAction();
            SetupSubAction();
            SetupButton();
            SetupCreate();
            SetupInterface();
            SetupMainMenu();
            SetupSubMenu();
            SetupNavigation();
            SetupShutter();
        }

        private void CreateClasses()
        {
            _action = new MainMenu.Action();
            _subAction = new SubMenu.Action();
            _button = new ButtonItem();
            _createMain = new Create();
            _data = new Interaction();
            _drawGui = new DrawGraph();
            _fps = new FrameCount();
            _gamepad = new Gamepad();
            _keyboard = new Keyboard();
            _mainMenu = new MainMenu.Menu();
            _subMenu = new SubMenu.Menu();
            _mouse = new Mouse();
            _nav = new Navigation();
            _shutter = new SubMenu.Shutter();
            _user = new User();
            _var = new Variables();
        }

        private void SetupAction()
        {
            _action.GetConfigHandle(_system);
            _action.GetDataHandle(_data);
            _action.GetNavHandle(_nav);
            _action.GetUserHandle(_user);
            _action.GetVariableHandle(_var);
            _action.GetShutterHandle(_shutter);
            _action.GetSubActionHandle(_subAction);
        }

        private void SetupSubAction()
        {
            _subAction.GetDataHandle(_data);
            _subAction.GetForm1Handle(_form1);
            _subAction.GetNavigationHandle(_nav);
            _subAction.GetSystemHandle(_system);
            _subAction.GetUserHandle(_user);
            _subAction.GetVariableHandle(_var);
        }

        private void SetupButton()
        {
            _button.GetDataHandle(_data);
        }

        private void SetupCreate()
        {
            _createMain.GetDataHandle(_data);
            _createMain.GetUserHandle(_user);
        }

        private void SetupInterface()
        {
            _gamepad.GetNavHandle(_nav);
            _keyboard.GetNavHandle(_nav);
            _keyboard.GetKeyHookHandle(_keyboardHook);
            _keyboard.GetKeymapHandle(_keymap);

            _mouse.GetDataHandle(_data);
            _mouse.GetNavHandle(_nav);
            _mouse.GetUserHandle(_user);
            _mouse.GetVariableHandle(_var);

            _mouse.GetSubMenuHandle(_shutter);
        }

        private void SetupMainMenu()
        {
            _mainMenu.GetButtonHandle(_button);
            _mainMenu.GetDataHandle(_data);
            _mainMenu.GetDrawGuiHandle(_drawGui);
            _mainMenu.GetUserHandle(_user);
            _mainMenu.GetVariableHandle(_var);
        }

        private void SetupSubMenu()
        {
            _subMenu.GetButtonHandle(_button);
            _subMenu.GetDataHandle(_data);
            _subMenu.GetDrawGuiHandle(_drawGui);
            _subMenu.GetShutterHandle(_shutter);
            _subMenu.GetUserHandle(_user);
            _subMenu.GetVariableHandle(_var);
            _subMenu.GetSystemHandle(_system);
            _subMenu.GetVideoCaptureHandle(_videoCapture);
            _subMenu.GetFormMenuHandle(this);
        }

        private void SetupNavigation()
        {
            _nav.GetMenuHandle(this);
            _nav.GetActionVariable(_action);
            _nav.GetSubActionVariable(_subAction);
            _nav.GetVariableHandle(_var);
            _nav.GetDataHandle(_data);
            _nav.GetUserHandle(_user);
            _nav.GetFpsHandle(_fps);
            _nav.GetMouseHandle(_mouse);
            _nav.GetShutterHandle(_shutter);
        }

        private void SetupShutter()
        {
            _shutter.GetDataHandle(_data);
            _shutter.GetFrameHandle(_fps);
            _shutter.GetUserHandle(_user);
            _shutter.GetVarHandle(_var);
        }
    }
}
