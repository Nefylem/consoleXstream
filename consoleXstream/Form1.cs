using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace consoleXstream
{
    public partial class Form1 : Form
    {
        #region Definitions
        #region development
        string strPath = @"\\gamer-pc\shield\consoleXstream\";
        public bool boolIDE = false;
        #endregion
        #region Classes
        private classSystem system;
        private classExternalScripting external;

        private classGamepadXInput gamepad;
        private classMouseHook mouse;
        private classKeyboardHook keyboard;
        private classKeyboardInterface keyboardInterface;

        private classControllerMax controllerMax;
        private classTitanOne titanOne;
        private classGimx gimx;
        private classVideoCapture videoCapture;
        private classVideoResolution videoResolution;
        private classRemap remap;

        private frmMenu formMenu;
        #endregion
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
        //private int _intScreenWidth;
        //private int _intScreenHeight;

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _MouseScreenBounds = 25;
            deleteLogs();

            if (System.Diagnostics.Debugger.IsAttached)
                boolIDE = true;

            declareClasses();
            checkDevelopment();

            runStartup();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeSystem();
        }

        //Definitions for calling from classes
        private void declareClasses()
        {
            formMenu = new frmMenu();

            system = new classSystem(this);
            external = new classExternalScripting(this);

            gamepad = new classGamepadXInput(this);
            keyboard = new classKeyboardHook(this);

            keyboardInterface = new classKeyboardInterface(this);

            mouse = new classMouseHook(this);

            controllerMax = new classControllerMax(this);
            titanOne = new classTitanOne(this);
            gimx = new classGimx(this);

            videoCapture = new classVideoCapture(this);
            videoResolution = new classVideoResolution(this);
            remap = new classRemap();

            //Pass to subforms as needed
            system.getVideoCaptureHandle(videoCapture);
            system.getControllerMaxHandle(controllerMax);
            system.getTitanOneHandle(titanOne);
            system.getVideoResolutionHandle(videoResolution);

            controllerMax.getSystemHandle(system);
            controllerMax.getKeyboardInterfaceHandle(keyboardInterface);

            titanOne.getSystemHandle(system);
            titanOne.getKeyboardInterfaceHandle(keyboardInterface);
            titanOne.getRemapHandle(remap);

            keyboardInterface.getSystemHandle(system);

            mouse.getSystemHandle(system);
            mouse.getKeyboardInterfaceHandle(keyboardInterface);
            mouse.getMenuHandle(formMenu);

            videoCapture.getSystemHandle(system);

            formMenu.getRemapHandle(remap);
        }
        //Deletes the log files on startup so only shows latest information
        private void deleteLogs()
        {
            if (File.Exists("system.log") == true) File.Delete("system.log"); 
            if (File.Exists("video.log") == true) File.Delete("video.log"); 
            if (File.Exists("menu.log") == true) File.Delete("menu.log"); 
            if (File.Exists("titanOne.log") == true) File.Delete("titanOne.log"); 
        }
        //Copies files to test environment 
        private void checkDevelopment()
        {
            if (boolIDE)
            {
                //Go do a dns check on the name. No need at this stage 
                //Also consider a manifest to check if things have changed. 
                if (Directory.Exists(strPath))
                {
                    try
                    {
                        if (File.Exists(strPath + @"\consoleXstream.exe"))
                            File.Delete(strPath + @"\consoleXstream.exe");

                        File.Copy("consoleXstream.exe", strPath + @"\consoleXstream.exe");
                    }
                    catch { }
                }
            }
        }
        //Kill active functions
        public void closeSystem()
        {
            if (system != null)
            {
                system.IsOverrideOnExit = true;
                system.setInitialDisplay();

                if (system.boolControllerMax)
                    controllerMax.closeControllerMaxInterface();

                if (system.boolInternalCapture)
                    videoCapture.closeGraph();
            }
            Application.Exit();
        }

        private void tmrSystem_Tick(object sender, EventArgs e)
        {
            runMainLoop();

            if (system.boolFPS)
            {
                if (_strFPSCheck != DateTime.Now.ToString("ss"))
                {
                    _intWatchFPS = _intCurrentFPS;
                    _intCurrentFPS = 0;
                    _strFPSCheck = DateTime.Now.ToString("ss");

                    label1.Text = "control: " + _intWatchFPS.ToString() + "ups";
                    label2.Text = "capture: " + intSampleFPS.ToString() + "fps";
                }
                else
                    _intCurrentFPS++;
            }

        }

        #region Startup Configuration
        //Configures system on startup
        private void runStartup()
        {
            system.getInitialDisplay();

            system.debug("[3] runStartup");
            this.BackColor = Color.Black;

            if (!boolIDE)
                this.FormBorderStyle = FormBorderStyle.None;

            this.WindowState = FormWindowState.Maximized;


            imgDisplay.Dock = DockStyle.Fill;
            imgDisplay.BackColor = Color.Black;

            _intMouseX = Cursor.Position.X;
            _intMouseY = Cursor.Position.Y;
            //_intScreenHeight = (int)(Screen.PrimaryScreen.Bounds.Height / 2);
            //_intScreenWidth = (int)(Screen.PrimaryScreen.Bounds.Width / 2);
            loadBackground();                           //If theres a background in the resource file, splash that
            loadUserConfig();

            remap.setDefaultGamepad();
            remap.loadGamepadRemap();
            //remap.saveGamepadRemap();

            if (boolIDE)
                system.boolStayOnTop = false;

            if (system.boolStayOnTop)
                this.TopMost = true;

            if (system.boolEnableKeyboard)
            {
                system.debug("[3] Init keyboard hook");
                keyboardInterface.getKeyboardHandle(keyboard);
                keyboard.enableKeyboardHook();
            }

            if (system.boolEnableMouse)
            {
                system.debug("[3] Init mouse event hook");
                mouse.enableMouseHook();
            }

            if (system.boolInternalCapture)
            {
                system.debug("[3] Init video capture variables");
                configureVideoCapture();

                if (system.strSetResolution != null)
                {
                    system.debug("[3] set user res ");
                    if (system.strSetResolution.Length > 0)
                    {
                        system.debug("[3] set user res [" + system.strSetResolution + "]");
                        system.changeCaptureResolution(system.strSetResolution);
                    }
                }
            }

            if (system.boolControllerMax)
            {
                if (!system.useTitanOneAPI)
                {
                    system.debug("[3] Configure ControllerMax API");
                    controllerMax.initControllerMax();
                    configureControllerMax();
                }
                else
                {
                    system.debug("[3] Configure ControllerMax using TitanOne API");
                    titanOne.setTOInterface(classTitanOne.DevPID.ControllerMax);
                    titanOne.initTitanOne();
                    configureTitanOne();
                }
            }

            if (system.boolTitanOne)
            {
                system.debug("[3] Configure TitanOne API");
                titanOne.setTOInterface(classTitanOne.DevPID.TitanOne);
                titanOne.initTitanOne();
                configureTitanOne();
            }

            if (system.boolFPS)
            {
                label1.Visible = true;
                label2.Visible = true;
            }

            if (system.boolHideMouse)
                Cursor.Hide();

            tmrSystem.Enabled = true;
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
            system.initializeUserData();
            system.loadDefaults();
            system.initializeKeyboardDefaults();
            system.loadSetupXML();
            system.checkUserSettings();
        }

        //Sends the settings into the video capture class. User settings already sent to class
        private void configureVideoCapture()
        {
            videoCapture.initialzeCapture();            //List everything so the user settings can pass into it
            videoCapture.runGraph();
        }

        private void configureControllerMax()
        {
            mouse.getControllerMaxHandle(controllerMax);
            mouse.enableMouseHook();
        }

        private void configureTitanOne()
        {
            
            mouse.getTitanOneHandle(titanOne);
            mouse.enableMouseHook();
        }

        public int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void checkMouse()
        {

            label4.Text = Cursor.Position.ToString();
            int intModifierX = -35;
            int intModifierY = -25;

            intReplaceX = Clamp((_intMouseX - Cursor.Position.X) * intModifierX, -100, 100);
            intReplaceY = Clamp((_intMouseY - Cursor.Position.Y) * intModifierY, -100, 100);

            _intMouseX = Cursor.Position.X;
            _intMouseY = Cursor.Position.Y;

            label5.Text = intReplaceX.ToString() + " / " + intReplaceY.ToString();

            if (Cursor.Position.Y > Screen.PrimaryScreen.Bounds.Height - _MouseScreenBounds)
                centerMouseY();

            if (Cursor.Position.Y < _MouseScreenBounds)
                centerMouseY();

            if (Cursor.Position.X < _MouseScreenBounds)
                centerMouseX();

            if (Cursor.Position.X > Screen.PrimaryScreen.Bounds.Width - _MouseScreenBounds)
                centerMouseX();
        }

        private void centerMouseY()
        {
            Cursor.Position = new Point(Cursor.Position.X, Screen.PrimaryScreen.Bounds.Height / 2);
            _intMouseY = Cursor.Position.Y;
        }

        private void centerMouseX()
        {
            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Cursor.Position.Y);
            _intMouseX = Cursor.Position.X;
        }

        #endregion

        //Main control loop
        private void runMainLoop()
        {
            if (_intBlockMenu > 0)
                _intBlockMenu--;

            if (system.boolInternalCapture)
                checkRunningGraph();

            if (system.boolMenu)
                return;

            if (system.boolEnableKeyboard)
            {
                if (keyboard.getKey(system.keyDef.strButtonBack))
                {
                    if (_intBlockMenu == 0)
                        openMenu();
                    else
                        _intBlockMenu = 3;
                }
                keyboardInterface.checkKeys();
            }

            if (system.boolEnableMouse)
                checkMouse();

            checkControllerInput();
        }

        private void checkControllerInput()
        {
            if (system.useTitanOneAPI)
            {
                titanOne.checkControllerInput();
            }
            else
            {
                if (system.boolControllerMax)
                    controllerMax.checkControllerInput();

                if (system.boolTitanOne)
                    titanOne.checkControllerInput();
            }
        }

        #region Video capture links to main form
        private void checkRunningGraph()
        {
            if (videoCapture.boolActiveVideo)
                videoCapture.checkVideoOutput();
        }

        public IntPtr returnVideoHandle()
        {
            return imgDisplay.Handle;
        }

        public void showVideoWindow()
        {
            imgDisplay.Visible = true;
        }

        public Point setVideoWindowBounds()
        {
            Point ptReturn = new Point(this.ClientSize.Width, this.ClientSize.Height);
            imgDisplay.SetBounds(0, 0, this.ClientSize.Width, this.ClientSize.Height);

            return ptReturn;
        }

        //Resizes the display video after resolution change
        public void changeDisplayRes()
        {
            if (!system.IsOverrideOnExit)
            {
                Application.DoEvents();
                videoCapture.debugVideo("[0] Reset after display change: " + Screen.PrimaryScreen.Bounds.Width + " / " + Screen.PrimaryScreen.Bounds.Height);

                this.BringToFront();
                this.Focus();

                imgDisplay.Dock = DockStyle.None;
                Application.DoEvents();
                imgDisplay.Dock = DockStyle.Fill;

                videoCapture.setupVideoWindow();

                imgDisplay.BringToFront();
                imgDisplay.Focus();

                videoCapture.forceRebuildAfterResolution();
            }
        }
        #endregion

        public void openMenu()
        {
            if (_intBlockMenu == 0)
            {
                system.boolMenu = true;

                Cursor.Show();

                if (system.boolStayOnTop)
                    this.TopMost = false;

                //Pass in various handles it needs
                formMenu.getSystemHandle(system);
                formMenu.getKeyboardHandle(keyboard);
                formMenu.getVideoCaptureHandle(videoCapture);

                formMenu.showPanel();
            }
        }

        public void closeMenuForm()
        {
            if (system.boolStayOnTop)
                this.TopMost = true;

            system.boolMenu = false;
            
            if (system.boolHideMouse)
                Cursor.Hide();

            _intBlockMenu = 3;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode.ToString().ToLower() == "return" && _boolChangeFullscreen == false)
            {
                _boolChangeFullscreen = true;
                if (this.FormBorderStyle != FormBorderStyle.None)
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                    if (system.boolHideMouse)
                        Cursor.Hide();
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.Fixed3D;
                    Cursor.Show();
                }

                this.Top = 0;           //Reset position
                this.Left = 0;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode.ToString().ToLower() == "return" && _boolChangeFullscreen == true)
                _boolChangeFullscreen = false;
        }
    }
}
