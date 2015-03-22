using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using consoleXstream.Config;
using consoleXstream.Input;
using consoleXstream.Input.Mouse;
using consoleXstream.Menu;
using consoleXstream.Output;
using consoleXstream.Output.TitanOne;
using consoleXstream.Remap;
using consoleXstream.Scripting;

namespace consoleXstream
{
    public partial class Form1 : Form
    {
        #region Definitions
        #region development
        string strPath = @"\\gamer-pc\shield\consoleXstream\";
        public bool boolIDE = false;
        #endregion

        private Configuration _system;
        private ExternalScript _external;

        //private GamepadXInput _gamepad;
        private Output.Gamepad _gamepad;

        private Input.Mouse.Hook _mouse;
        private KeyboardHook _keyboard;
        private KeyboardInterface _keyboardInterface;

        private ControllerMax _controllerMax;
        private Output.TitanOne.Write _titanOne;
        private Gimx _gimx;
        private VideoCapture.VideoCapture _videoCapture;
        private VideoResolution _videoResolution;
        private Remap.Remapping _remap;
        private Remap.Keymap _keymap;

        private ShowMenu formMenu;

        public int intSampleFPS;
        public int intLineSample;
        public int intReplaceX;
        public int intReplaceY;

        private int _intWatchFPS;
        private int _intCurrentFPS;
        private string _strFPSCheck;
        private int _intBlockMenu;

        private int _MouseScreenBounds;
        private int _intMouseX;
        private int _intMouseY;
        private bool _boolChangeFullscreen;

        private bool IsUpdatingTitanOneList;

        public string RetrySetTitanOne;
        public int RetryTimeOut;

        public List<string> ListToDevices;
        public List<string> ListBackupToDevices;
 
        private int TitanOneListRefresh;
        private bool TitanOneListRefreshFail;

        //private int _intScreenWidth;
        //private int _intScreenHeight;
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //captureImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            _MouseScreenBounds = 25;
            DeleteLogs();

            if (System.Diagnostics.Debugger.IsAttached)
                boolIDE = true;

            DeclareClasses();
            CheckDevelopment();

            RunStartup();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseSystem();
        }

        //Definitions for calling from classes
        private void DeclareClasses()
        {
            _remap = new Remapping();
            _keyboardInterface = new KeyboardInterface(this);
            _system = new Configuration(this);
            _gamepad = new Output.Gamepad(this, _remap, _system, _keyboardInterface);
            _titanOne = new Write(this, _system, _gamepad);

            _external = new ExternalScript(this);

            //_gamepad = new GamepadXInput(this);
            _keyboard = new KeyboardHook(this);

            _controllerMax = new ControllerMax(this);
            _gimx = new Gimx(this);

            _videoResolution = new VideoResolution(this);
            _keymap = new Keymap();

            _mouse = new Hook(this, _gamepad);
            _videoCapture = new VideoCapture.VideoCapture(this, _system);

            formMenu = new ShowMenu(this, _system, _keyboard, _videoCapture, _remap, _keymap);

            //Pass to subforms as needed
            _system.GetClassHandles(_videoCapture, _controllerMax, _titanOne, _videoResolution);

            _controllerMax.getSystemHandle(_system);
            _controllerMax.getKeyboardInterfaceHandle(_keyboardInterface);

            _keyboardInterface.getSystemHandle(_system);
            _keyboardInterface.getRemapHangle(_remap);
            _keyboardInterface.getKeymapHandle(_keymap);

            _mouse.GetSystemHandle(_system);
            _mouse.GetKeyboardInterfaceHandle(_keyboardInterface);
            _mouse.GetMenuHandle(formMenu);

            _keymap.InitializeKeyboardDefaults();

        }

        //Deletes the log files on startup so only shows latest information
        private void DeleteLogs()
        {
            if (File.Exists("system.log")) File.Delete("system.log"); 
            if (File.Exists("video.log")) File.Delete("video.log"); 
            if (File.Exists("titanOne.log")) File.Delete("titanOne.log"); 
            if (File.Exists("controllerMax.log")) File.Delete("controllerMax.log");
            if (File.Exists("connectTo.log")) File.Delete("connectTo.log");
            if (File.Exists("listAll.log")) File.Delete("listAll.log");
            if (File.Exists("VideoResolution.log")) File.Delete("VideoResolution.log");
            if (File.Exists("video.log")) File.Delete("video.log");
            if (File.Exists("menu.log")) File.Delete("Menu.log");
        }
        //Copies files to test environment 
        private void CheckDevelopment()
        {
            if (!boolIDE) return;
            if (!Directory.Exists(strPath)) return;
            try
            {
                if (File.Exists(strPath + @"\consoleXstream.exe"))
                    File.Delete(strPath + @"\consoleXstream.exe");

                File.Copy("consoleXstream.exe", strPath + @"\consoleXstream.exe");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //Kill active functions
        public void CloseSystem()
        {
            if (_system != null)
            {
                _system.IsOverrideOnExit = true;
                _system.setInitialDisplay();

                if (_system.boolControllerMax)
                    _controllerMax.closeControllerMaxInterface();

                if (_system.boolInternalCapture)
                    _videoCapture.CloseGraph();
            }
            Application.Exit();
        }

        private void tmrSystem_Tick(object sender, EventArgs e)
        {
            if (RetrySetTitanOne != null)
            {
                if (RetrySetTitanOne.Length > 0)
                {
                    RetryTimeOut--;
                    if (RetryTimeOut <= 0)
                    {
                        RetrySetTitanOne = "";
                    }

                    int result = _titanOne.CheckDevices();
                    if (result > 0)
                    {
                        string serial = RetrySetTitanOne;
                        RetrySetTitanOne = "";
                        _system.DisableTitanOneRetry = true;
                        _titanOne.SetTitanOneDevice(serial);
                    }
                }
            }

            RunMainLoop();

            if (!_system.boolFPS) return;
            if (_strFPSCheck != DateTime.Now.ToString("ss"))
            {
                _intWatchFPS = _intCurrentFPS;
                _intCurrentFPS = 0;
                _strFPSCheck = DateTime.Now.ToString("ss");

                label1.Text = @"control: " + _intWatchFPS + "ups";
                label2.Text = @"capture: " + intSampleFPS + "fps";
            }
            else
                _intCurrentFPS++;
        }

        #region Startup Configuration
        //Configures system on startup
        private void RunStartup()
        {
            _system.getInitialDisplay();

            _system.Debug("[3] runStartup");
            BackColor = Color.Green;

            if (!boolIDE)
                FormBorderStyle = FormBorderStyle.None;

            WindowState = FormWindowState.Maximized;


            _intMouseX = Cursor.Position.X;
            _intMouseY = Cursor.Position.Y;
            //_intScreenHeight = (int)(Screen.PrimaryScreen.Bounds.Height / 2);
            //_intScreenWidth = (int)(Screen.PrimaryScreen.Bounds.Width / 2);
            loadBackground();                           //If theres a background in the resource file, splash that
            loadUserConfig();

            _remap.setDefaultGamepad();
            _remap.loadGamepadRemap();
            //remap.saveGamepadRemap();

            if (_system.IsVr)
            {
                imgDisplayVr.Visible = true;
                imgDisplayVr.BackColor = Color.Black;

                imgDisplay.Visible = true;
                imgDisplay.BackColor = Color.Black;

                imgDisplay.Left = 0;
                imgDisplay.Top = 0;
                imgDisplay.Width = Screen.PrimaryScreen.Bounds.Width / 2;
                imgDisplay.Height = Screen.PrimaryScreen.Bounds.Height;

                imgDisplayVr.Left = Screen.PrimaryScreen.Bounds.Width / 2;
                imgDisplayVr.Top = 0;
                imgDisplayVr.Height = Screen.PrimaryScreen.Bounds.Height;
                imgDisplayVr.Width = Screen.PrimaryScreen.Bounds.Width / 2;
            }
            else
            {
                imgDisplay.Dock = DockStyle.Fill;
                imgDisplay.BackColor = Color.Black;
            }

            if (boolIDE)
                _system.BoolStayOnTop = false;

            if (_system.BoolStayOnTop)
                TopMost = true;

            if (_system.boolEnableKeyboard)
            {
                _system.Debug("[3] Init keyboard hook");
                _keyboardInterface.getKeyboardHandle(_keyboard);
                _keyboard.enableKeyboardHook();
            }

            if (_system.boolEnableMouse)
            {
                _system.Debug("[3] Init mouse event hook");
                _mouse.enableMouseHook();
            }

            if (_system.boolInternalCapture)
            {
                _system.Debug("[3] Init video capture variables");
                configureVideoCapture();

                if (_system.strSetResolution != null)
                {
                    _system.Debug("[3] set user res ");
                    if (_system.strSetResolution.Length > 0)
                    {
                        _system.Debug("[3] set user res [" + _system.strSetResolution + "]");
                        _system.changeCaptureResolution(_system.strSetResolution);
                    }
                }
            }

            if (_system.boolControllerMax)
            {
                if (!_system.useTitanOneAPI)
                {
                    _system.Debug("[3] Configure ControllerMax API");
                    _controllerMax.initControllerMax();
                }
                else
                {
                    //_system.Debug("[3] Configure ControllerMax using TitanOne API");
                    //_titanOne.setTOInterface(Output.TitanOne.DevPID.ControllerMax);
                    //_titanOne.initTitanOne();
                }
            }

            if (_system.boolTitanOne) InitializeTitanOne();

            if (_system.boolFPS)
            {
                label1.Visible = true;
                label2.Visible = true;
            }

            if (_system.boolHideMouse)
                Cursor.Hide();

            tmrSystem.Enabled = true;
        }

        public void InitializeTitanOne()
        {
            if (ListToDevices == null)
                ListToDevices = new List<string>();

            _system.Debug("[3] Configure TitanOne API");
            _titanOne.SetToInterface(Define.DevPid.TitanOne);
            
            if (_system.TitanOneDevice != null)
            {
                if (_system.TitanOneDevice.Length > 0)
                {
                    _titanOne.SetApiMethod(Define.ApiMethod.Multi);
                    _titanOne.SetTitanOneDevice(_system.TitanOneDevice);
                }
            }

            _titanOne.Initialize();
        }

        //Loads a background - if present in the resource file
        private void loadBackground()
        {
            Assembly _assembly = Assembly.GetExecutingAssembly();

            List<string> filenames = new List<string>();
            filenames = _assembly.GetManifestResourceNames().ToList<string>();
            //Check for background
        }

        private void loadUserConfig()
        {
            _system.loadDefaults();
            
            _keymap.InitializeKeyboardDefaults();
            _keymap.LoadKeyboardInputs();

            _system.LoadSetup();
            _system.CheckUserSettings();
        }

        //Sends the settings into the video capture class. User settings already sent to class
        private void configureVideoCapture()
        {
            _videoCapture.InitialzeCapture();            
            _videoCapture.RunGraph();
        }

        public int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void CheckMouse()
        {
            /*
            label4.Text = Cursor.Position.ToString();
            const int intModifierX = -35;
            const int intModifierY = -25;

            intReplaceX = Clamp((_intMouseX - Cursor.Position.X) * intModifierX, -100, 100);
            intReplaceY = Clamp((_intMouseY - Cursor.Position.Y) * intModifierY, -100, 100);

            _intMouseX = Cursor.Position.X;
            _intMouseY = Cursor.Position.Y;

            label5.Text = intReplaceX + @" / " + intReplaceY;

            if (Cursor.Position.Y > Screen.PrimaryScreen.Bounds.Height - _MouseScreenBounds)
                CenterMouseY();

            if (Cursor.Position.Y < _MouseScreenBounds)
                CenterMouseY();

            if (Cursor.Position.X < _MouseScreenBounds)
                CenterMouseX();

            if (Cursor.Position.X > Screen.PrimaryScreen.Bounds.Width - _MouseScreenBounds)
                CenterMouseX();
             */
        }

        private void CenterMouseY()
        {
            Cursor.Position = new Point(Cursor.Position.X, Screen.PrimaryScreen.Bounds.Height / 2);
            _intMouseY = Cursor.Position.Y;
        }

        private void CenterMouseX()
        {
            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Cursor.Position.Y);
            _intMouseX = Cursor.Position.X;
        }

        #endregion

        //Main control loop
        private void RunMainLoop()
        {
            if (IsUpdatingTitanOneList) CheckTitanOneConnectionList();

            if (_intBlockMenu > 0)
                _intBlockMenu--;

            if (_system.boolInternalCapture)
                CheckRunningGraph();

            if (_system.boolMenu)
                return;

            if (_system.boolEnableKeyboard)
            {
                if (_keyboard.getKey(_keymap.KeyDef.ButtonBack))
                {
                    if (_intBlockMenu == 0)
                        OpenMenu();
                    else
                        _intBlockMenu = 3;
                }
                _keyboardInterface.checkKeys();
            }

            if (_system.boolEnableMouse)
                CheckMouse();

            CheckControllerInput();
        }

        private void CheckControllerInput()
        {
            _gamepad.Check();
            if (_system.useTitanOneAPI) _titanOne.Send();
            else
            {
                if (_system.boolControllerMax)
                    _controllerMax.checkControllerInput();

                if (_system.boolTitanOne)
                    _titanOne.Send();
            }
        }

        #region Video capture links to main form
        private void CheckRunningGraph()
        {
            if (_videoCapture.boolActiveVideo)
                _videoCapture.checkVideoOutput();
        }

        public IntPtr ReturnVideoHandle()
        {
            return imgDisplay.Handle;
        }

        public IntPtr ReturnVrHandle()
        {
            return imgDisplayVr.Handle;
        }

        public void ShowVrWindow()
        {
            imgDisplayVr.Visible = true;
        }

        public void ShowVideoWindow()
        {
            imgDisplay.Visible = true;
        }

        public Point SetVideoWindowBounds()
        {
            var ptReturn = new Point(0, 0);
            if (_system.IsVr)
            {
                ptReturn = new Point(ClientSize.Width / 2, ClientSize.Height);
                imgDisplay.SetBounds(0, 0, ClientSize.Width / 2, ClientSize.Height);
            }
            else
            {
                ptReturn = new Point(ClientSize.Width, ClientSize.Height);
                imgDisplay.SetBounds(0, 0, ClientSize.Width, ClientSize.Height);
            }

            return ptReturn;
        }

        public Point SetVideoWindowBoundsVr()
        {
            var ptReturn = new Point(0, 0);
            if (_system.IsVr)
            {
                ptReturn = new Point(ClientSize.Width / 2, ClientSize.Height);
                imgDisplayVr.SetBounds(ClientSize.Width / 2, 0, ClientSize.Width / 2, ClientSize.Height);
            }

            return ptReturn;
        }

        //Resizes the display video after resolution change
        public void ChangeDisplayRes()
        {
            /*
            if (_system.IsOverrideOnExit) return;

            Application.DoEvents();
            //_videoCapture.DebugVideo("[0] Reset after display change: " + Screen.PrimaryScreen.Bounds.Width + " / " + Screen.PrimaryScreen.Bounds.Height);

            BringToFront();
            Focus();

            imgDisplay.Dock = DockStyle.None;
            Application.DoEvents();
            imgDisplay.Dock = DockStyle.Fill;

            _videoCapture.setupVideoWindow();

            imgDisplay.BringToFront();
            imgDisplay.Focus();

            //_videoCapture.ForceRebuildAfterResolution();
             */
        }
        #endregion

        public void OpenMenu()
        {
            if (_intBlockMenu != 0) return;

            _system.boolMenu = true;

            Cursor.Show();

            if (_system.BoolStayOnTop)
                TopMost = false;

            formMenu.ShowPanel();
        }

        public void CloseMenuForm()
        {
            if (_system.BoolStayOnTop)
                TopMost = true;

            _system.boolMenu = false;
            
            if (_system.boolHideMouse)
                Cursor.Hide();

            _intBlockMenu = 3;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Alt || e.KeyCode.ToString().ToLower() != "return" || _boolChangeFullscreen) return;

            _boolChangeFullscreen = true;
            if (FormBorderStyle != FormBorderStyle.None)
            {
                FormBorderStyle = FormBorderStyle.None;
                if (_system.boolHideMouse)
                    Cursor.Hide();
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Fixed3D;
                Cursor.Show();
            }

            Top = 0;           //Reset position
            Left = 0;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode.ToString().ToLower() == "return" && _boolChangeFullscreen)
                _boolChangeFullscreen = false;
        }

        public void ListTitanOneDevices()
        {
            ListBackupToDevices = new List<string>();

            foreach (var item in ListToDevices)
            {
                ListBackupToDevices.Add(item);
            }

            //_system.Debug("listAll.log", "clearList");

            ListToDevices = new List<string>();

            //_system.Debug("listAll.log", "check controllerMax");

            if (_controllerMax._gcapi_Unload != null)
                _controllerMax.closeControllerMaxInterface();


            //_system.Debug("listAll.log", "setup update true");
            IsUpdatingTitanOneList = true;
            TitanOneListRefresh = 10;
            TitanOneListRefreshFail = false;
            //_system.Debug("listAll.log", "list devices");
            _titanOne.ListDevices(); 
        }

        private void CheckTitanOneConnectionList()
        {
            //_system.Debug("listAll.log", "_titanOne.CheckDevices()");
            var result = _titanOne.CheckDevices();
            if (result == 0)
            {
                if (TitanOneListRefresh > 0)
                {
                    TitanOneListRefresh--;
                }
                else
                {
                    if (!TitanOneListRefreshFail)
                    {
                        TitanOneListRefreshFail = true;
                        TitanOneListRefresh = 10;
                        _titanOne.ListDevices();                        
                    }
                    else
                    {
                        IsUpdatingTitanOneList = false;
                        TitanOneListRefresh = 0;
                        TitanOneListRefreshFail = false;
                        //_system.Debug("listAll.log", "Cant find TitanOnes, passing from backup list");
                        ListToDevices.Clear();

                        foreach (var item in ListBackupToDevices) { ListToDevices.Add(item); }
                        IsUpdatingTitanOneList = false;
                        formMenu.PassToSubSelect(ListToDevices);

                    }
                }
                return;
            }

            if (result > 0)
            {
                //_system.Debug("listAll.log", "found " + result + " results");
                IsUpdatingTitanOneList = false;
                formMenu.PassToSubSelect(ListToDevices);
            }
        }

        public void ChangeVr()
        {
            if (_system.IsVr)
            {
                imgDisplay.Dock = DockStyle.None;
                
                imgDisplayVr.Visible = true;
                imgDisplayVr.BackColor = Color.Black;

                imgDisplay.Visible = true;
                imgDisplay.BackColor = Color.Black;

                imgDisplay.Left = 0;
                imgDisplay.Top = 50;
                imgDisplay.Width = Screen.PrimaryScreen.Bounds.Width / 2;
                imgDisplay.Height = Screen.PrimaryScreen.Bounds.Height - 100;

                imgDisplayVr.Left = Screen.PrimaryScreen.Bounds.Width / 2;
                imgDisplayVr.Top = 50;
                imgDisplayVr.Height = Screen.PrimaryScreen.Bounds.Height - 100;
                imgDisplayVr.Width = Screen.PrimaryScreen.Bounds.Width / 2;
            }
            else
            {
                imgDisplayVr.Visible = false;
                imgDisplay.Dock = DockStyle.Fill;
            }
        }

        public void SetTitanOne(string serial)
        {
            _titanOne.SetTitanOneDevice(serial); 
            _system.AddData("ControllerMax", "False");
            _system.AddData("TitanOne", "True");
        }
        public string GetTitanOne() { return _titanOne.GetTitanOneDevice(); }

        public void SetTitanOneMode(string type)
        {
            _titanOne.SetApiMethod(type.ToLower() == "single" ? Define.ApiMethod.Single : Define.ApiMethod.Multi);
        }

        public void InitControllerMax()
        {
            _controllerMax.initControllerMax();
        }

        public void FocusWindow()
        {
            imgDisplay.BringToFront();
            imgDisplay.Focus();
        }
    }
}
