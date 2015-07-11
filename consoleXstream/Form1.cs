using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using consoleXstream.Output.TitanOne;

namespace consoleXstream
{
    public partial class Form1 : Form
    {
        string strPath = @"\\gamer-pc\shield\consoleXstream\";
        public bool boolIDE = false;

        private BaseClass _class;

        public int intSampleFPS;
        public int intLineSample;
        public int intReplaceX;
        public int intReplaceY;

        private int _intWatchFPS;
        private int _intCurrentFPS;
        private string _strFPSCheck;
        private int _intBlockMenu;

        private int MouseScreenBounds;
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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _class = new BaseClass(this);

            //captureImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            MouseScreenBounds = 25;
            DeleteLogs();

            if (System.Diagnostics.Debugger.IsAttached)
                boolIDE = true;

            CheckDevelopment();

            RunStartup();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseSystem();
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
            if (_class.System != null)
            {
                _class.System.IsOverrideOnExit = true;
                _class.System.SetInitialDisplay();

                if (_class.System.UseControllerMax)
                    _class.ControllerMax.closeControllerMaxInterface();

                if (_class.System.UseInternalCapture)
                    _class.VideoCapture.CloseGraph();
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

                    int result = _class.TitanOne.CheckDevices();
                    if (result > 0)
                    {
                        string serial = RetrySetTitanOne;
                        RetrySetTitanOne = "";
                        _class.System.DisableTitanOneRetry = true;
                        _class.TitanOne.SetTitanOneDevice(serial);
                    }
                }
            }

            RunMainLoop();

            if (!_class.System.CheckFps) return;
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
            _class.System.GetInitialDisplay();

            _class.System.Debug("[3] runStartup");
            BackColor = Color.Black;

            if (!boolIDE)
                FormBorderStyle = FormBorderStyle.None;

            WindowState = FormWindowState.Maximized;


            _intMouseX = Cursor.Position.X;
            _intMouseY = Cursor.Position.Y;
            //_intScreenHeight = (int)(Screen.PrimaryScreen.Bounds.Height / 2);
            //_intScreenWidth = (int)(Screen.PrimaryScreen.Bounds.Width / 2);
            loadBackground();                           //If theres a background in the resource file, splash that
            loadUserConfig();

            _class.Remap.SetDefaultGamepad();
            _class.Remap.LoadGamepadRemap();
            //remap.saveGamepadRemap();

            if (_class.System.IsVr)
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
                _class.System.IsStayOnTop = false;

            if (_class.System.IsStayOnTop)
                TopMost = true;

            if (_class.System.IsEnableKeyboard)
            {
                _class.System.Debug("[3] Init keyboard hook");
                _class.KeyboardInterface.getKeyboardHandle(_class.Keyboard);
                _class.Keyboard.EnableKeyboardHook();
            }

            _class.System.Debug("[3] Init mouse event hook");
            _class.Mouse.enableMouseHook();

            if (_class.System.UseInternalCapture)
            {
                _class.System.Debug("[3] Init video capture variables");
                configureVideoCapture();

                if (_class.System.strSetResolution != null)
                {
                    _class.System.Debug("[3] set user res ");
                    if (_class.System.strSetResolution.Length > 0)
                    {
                        _class.System.Debug("[3] set user res [" + _class.System.strSetResolution + "]");
                        _class.System.changeCaptureResolution(_class.System.strSetResolution);
                    }
                }
            }

            if (_class.System.UseControllerMax)
            {
                if (!_class.System.UseTitanOneApi)
                {
                    _class.System.Debug("[3] Configure ControllerMax API");
                    _class.ControllerMax.initControllerMax();
                }
                else
                {
                    //_class.System.Debug("[3] Configure ControllerMax using TitanOne API");
                    //_class.TitanOne.setTOInterface(Output.TitanOne.DevPID.ControllerMax);
                    //_class.TitanOne.initTitanOne();
                }
            }

            if (_class.System.UseGimxRemote) _class.GimxRemote.initGimxRemote();

            if (_class.System.UseTitanOne) InitializeTitanOne();

            if (_class.System.CheckFps)
            {
                label1.Visible = true;
                label2.Visible = true;
            }

            if (_class.System.IsHideMouse)
                Cursor.Hide();

            tmrSystem.Enabled = true;
        }

        public void InitializeTitanOne()
        {
            if (ListToDevices == null)
                ListToDevices = new List<string>();

            _class.System.Debug("[3] Configure TitanOne API");
            _class.TitanOne.SetToInterface(Define.DevPid.TitanOne);
            
            if (_class.System.TitanOneDevice != null)
            {
                if (_class.System.TitanOneDevice.Length > 0)
                {
                    _class.TitanOne.SetApiMethod(Define.ApiMethod.Multi);
                    _class.TitanOne.SetTitanOneDevice(_class.System.TitanOneDevice);
                }
            }

            _class.TitanOne.Initialize();
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
            _class.System.loadDefaults();
            
            _class.Keymap.InitializeKeyboardDefaults();
            _class.Keymap.LoadKeyboardInputs();

            _class.System.LoadSetup();
            _class.System.CheckUserSettings();
        }

        //Sends the settings into the video capture class. User settings already sent to class
        private void configureVideoCapture()
        {
            _class.VideoCapture.InitialzeCapture();            
            _class.VideoCapture.RunGraph();
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

            if (Cursor.Position.Y > Screen.PrimaryScreen.Bounds.Height - _class.MouseScreenBounds)
                CenterMouseY();

            if (Cursor.Position.Y < _class.MouseScreenBounds)
                CenterMouseY();

            if (Cursor.Position.X < _class.MouseScreenBounds)
                CenterMouseX();

            if (Cursor.Position.X > Screen.PrimaryScreen.Bounds.Width - _class.MouseScreenBounds)
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

            if (_class.System.UseInternalCapture)
                CheckRunningGraph();

            if (_class.System.boolMenu)
                return;

            if (_class.System.IsEnableKeyboard)
            {
                if (_class.Keyboard.GetKey(_class.Keymap.KeyDef.ButtonBack))
                {
                    if (_intBlockMenu == 0)
                        OpenMenu();
                    else
                        _intBlockMenu = 3;
                }
                _class.KeyboardInterface.checkKeys();
            }

            if (_class.System.IsEnableMouse)
                CheckMouse();

            CheckControllerInput();
        }

        private void CheckControllerInput()
        {
            _class.Gamepad.Check();

            if (_class.System.UseTitanOne)
            {
                _class.TitanOne.Send();
                return;
            }

            if (_class.System.UseControllerMax)
            {
                if (_class.System.UseTitanOneApi)
                    _class.TitanOne.Send();
                else
                    _class.ControllerMax.Send();
                return;
            }

            if (_class.System.UseGimxRemote)
                _class.GimxRemote.CheckControllerInput();
        }

        #region Video capture links to main form
        private void CheckRunningGraph()
        {
            if (_class.VideoCapture.BoolActiveVideo)
                _class.VideoCapture.checkVideoOutput();
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
            if (_class.System.IsVr)
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
            if (_class.System.IsVr)
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
            if (_class.System.IsOverrideOnExit) return;

            Application.DoEvents();
            //_class._videoCapture.DebugVideo("[0] Reset after display change: " + Screen.PrimaryScreen.Bounds.Width + " / " + Screen.PrimaryScreen.Bounds.Height);

            BringToFront();
            Focus();

            imgDisplay.Dock = DockStyle.None;
            Application.DoEvents();
            imgDisplay.Dock = DockStyle.Fill;

            _class._videoCapture.setupVideoWindow();

            imgDisplay.BringToFront();
            imgDisplay.Focus();

            //_class._videoCapture.ForceRebuildAfterResolution();
             */
        }
        #endregion

        public void OpenMenu()
        {
            if (_intBlockMenu != 0) return;

            _class.System.boolMenu = true;

            Cursor.Show();

            if (_class.System.IsStayOnTop)
                TopMost = false;

            _class.Menu.ShowPanel();
        }

        public void CloseMenuForm()
        {
            if (_class.System.IsStayOnTop)
                TopMost = true;

            _class.System.boolMenu = false;
            
            if (_class.System.IsHideMouse)
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
                if (_class.System.IsHideMouse)
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

            //_class.System.Debug("listAll.log", "clearList");

            ListToDevices = new List<string>();

            //_class.System.Debug("listAll.log", "check controllerMax");

            if (_class.ControllerMax._gcapi_Unload != null)
                _class.ControllerMax.closeControllerMaxInterface();


            //_class.System.Debug("listAll.log", "setup update true");
            IsUpdatingTitanOneList = true;
            TitanOneListRefresh = 10;
            TitanOneListRefreshFail = false;
            //_class.System.Debug("listAll.log", "list devices");
            _class.TitanOne.ListDevices(); 
        }

        private void CheckTitanOneConnectionList()
        {
            //_class.System.Debug("listAll.log", "_class.TitanOne.CheckDevices()");
            var result = _class.TitanOne.CheckDevices();
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
                        _class.TitanOne.ListDevices();                        
                    }
                    else
                    {
                        IsUpdatingTitanOneList = false;
                        TitanOneListRefresh = 0;
                        TitanOneListRefreshFail = false;
                        //_class.System.Debug("listAll.log", "Cant find TitanOnes, passing from backup list");
                        ListToDevices.Clear();

                        foreach (var item in ListBackupToDevices) { ListToDevices.Add(item); }
                        IsUpdatingTitanOneList = false;
                        _class.Menu.PassToSubSelect(ListToDevices);

                    }
                }
                return;
            }

            if (result > 0)
            {
                //_class.System.Debug("listAll.log", "found " + result + " results");
                IsUpdatingTitanOneList = false;
                _class.Menu.PassToSubSelect(ListToDevices);
            }
        }

        public void ChangeVr()
        {
            if (_class.System.IsVr)
            {
                imgDisplay.Dock = DockStyle.None;
                
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
                imgDisplayVr.Visible = false;
                imgDisplay.Dock = DockStyle.Fill;
            }
        }

        public void SetTitanOne(string serial)
        {
            _class.TitanOne.SetTitanOneDevice(serial); 
            _class.System.AddData("ControllerMax", "False");
            _class.System.AddData("TitanOne", "True");
        }
        public string GetTitanOne() { return _class.TitanOne.GetTitanOneDevice(); }

        public void SetTitanOneMode(string type)
        {
            _class.TitanOne.SetApiMethod(type.ToLower() == "single" ? Define.ApiMethod.Single : Define.ApiMethod.Multi);
        }

        public void InitControllerMax()
        {
            _class.ControllerMax.initControllerMax();
        }

        public void InitTitanOne()
        {
            _class.System.Debug("titanone.log", "InitializeTitanOne");
            _class.TitanOne.Initialize();
        }

        public void SetTitanOneDevice()
        {
            _class.TitanOne.SetTitanOneDevice();   
        }

        public void FocusWindow()
        {
            imgDisplay.BringToFront();
            imgDisplay.Focus();
        }

        public void SetDisplayRecord(string res)
        {
            _class.VideoCapture.SetDisplay(res);
        }

        public void MoveVrDisplay()
        {
            var centerWidth = (Screen.PrimaryScreen.Bounds.Width/2);
            imgDisplay.Top = _class.System.Class.Vr.VideoHeightOffset;
            imgDisplayVr.Top = _class.System.Class.Vr.VideoHeightOffset;
            imgDisplay.Left = _class.System.Class.Vr.VideoWidthOffset;
            imgDisplayVr.Left = centerWidth + _class.System.Class.Vr.VideoWidthOffset;
        }
    }
}
