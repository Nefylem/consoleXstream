using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace consoleXstream.Home.Startup
{
    public class Initial
    {
        public Initial(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Run()
        {
            Var var = _class.Var;
            //_class.Var.MouseScreenBounds = 25;
            var.MouseScreenBounds = 25;

            _class.BaseClass.System.GetInitialDisplay();

            _class.BaseClass.System.Debug("[3] runStartup");
            _class.Home.BackColor = Color.Black;

            if (!_class.Var.IsIde)
                _class.Home.FormBorderStyle = FormBorderStyle.None;

            _class.Home.WindowState = FormWindowState.Maximized;

            _class.Var.MouseX = 0;
            _class.Var.MouseY = 0;

            LoadBackground();                           
            LoadUserConfig();

            if (_class.BaseClass.System.IsVr)
                _class.StartupDisplay.ConfigureDisplayWindowsVrMode();
            else
                _class.StartupDisplay.ConfigureDisplayWindow();

            if (_class.Var.IsIde)
                _class.BaseClass.System.IsStayOnTop = false;

            if (_class.BaseClass.System.IsStayOnTop)
                _class.Home.TopMost = true;

            if (_class.BaseClass.System.IsEnableKeyboard)
            {
                _class.BaseClass.System.Debug("[3] Init keyboard hook");
                _class.BaseClass.Keyboard.EnableKeyboardHook();
            }

            _class.BaseClass.System.Debug("[3] Init mouse event hook");
            _class.BaseClass.Mouse.enableMouseHook();

            if (_class.BaseClass.System.UseInternalCapture)
            {
                _class.BaseClass.System.Debug("[3] Init video capture variables");
                _class.StartupDisplay.ConfigureVideoCapture();

                if (_class.BaseClass.System.strSetResolution != null)
                {
                    _class.BaseClass.System.Debug("[3] set user res ");
                    if (_class.BaseClass.System.strSetResolution.Length > 0)
                    {
                        _class.BaseClass.System.Debug("[3] set user res [" + _class.BaseClass.System.strSetResolution + "]");
                        _class.BaseClass.System.changeCaptureResolution(_class.BaseClass.System.strSetResolution);
                    }
                }
                if (_class.BaseClass.System.IsVr)
                {
                    _class.Home.MoveVrDisplay();
                }
            }

            if (_class.BaseClass.System.UseControllerMax)
            {
                if (!_class.BaseClass.System.UseTitanOneApi)
                {
                    _class.BaseClass.System.Debug("[3] Configure ControllerMax API");
                    _class.BaseClass.ControllerMax.initControllerMax();
                }
                else
                {
                    //_class.System.Debug("[3] Configure ControllerMax using TitanOne API");
                    //_class.TitanOne.setTOInterface(Output.TitanOne.DevPID.ControllerMax);
                    //_class.TitanOne.initTitanOne();
                }
            }

            _class.StartupTitanOne.InitializeTitanOne();

            if (_class.BaseClass.System.CheckFps)
            {
                _class.Home.label1.Visible = true;
                _class.Home.label2.Visible = true;
            }

            if (_class.BaseClass.System.IsHideMouse)
                Cursor.Hide();

            //EndBuild();
        }

        //Loads a background - if present in the resource file
        private void LoadBackground()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            List<string> filenames = new List<string>();
            filenames = assembly.GetManifestResourceNames().ToList<string>();
            //Check for background
        }

        private void LoadUserConfig()
        {
            var system = _class.BaseClass.System;
            var keymap = _class.BaseClass.Keymap;
            
            system.loadDefaults();

            keymap.InitializeKeyboardDefaults();
            //keymap.LoadKeyboardInputs();

            system.LoadSetup();
            system.CheckUserSettings();
        }

        public void EndBuild()
        {
            _class.Timers.Create();
        }
    }
}
