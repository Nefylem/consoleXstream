using System;
using System.Drawing;
using System.Windows.Forms;
using consoleXstream.Home;
using consoleXstream.Output.TitanOne;

namespace consoleXstream
{
    public partial class Form1 : Form
    {
        private BaseClass _class;
        private Home.Classes _homeClass;

        public int intSampleFPS;
        public int intLineSample;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Startup();
            _homeClass.Startup.Run();

            tmrSystem.Enabled = true;
            tmrMasterControl.Enabled = true;
            /*
            if (_class.System.MainThreads > 0)
            {
                _homeClass.Timers.Create();
                _homeClass.Timers.StartAll();
            }
             */
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _homeClass.System.Exit();
        }

        private void Startup()
        {
            _class = new BaseClass(this);

            _homeClass = new Home.Classes(this, _class);
            _homeClass.Development.Setup();

            _class.HomeClass = _homeClass;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Alt || e.KeyCode.ToString().ToLower() != "return" || _homeClass.Var.IsFullscreen) return;

            _homeClass.Var.IsFullscreen = true;
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
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode.ToString().ToLower() == "return" && _homeClass.Var.IsFullscreen)
                _homeClass.Var.IsFullscreen = false;
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
            //_class.System.AddData("ControllerMax", "False");
            //_class.System.AddData("TitanOne", "True");
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

        public void MoveVrDisplay()
        {
            var centerWidth = (Screen.PrimaryScreen.Bounds.Width/2);
            imgDisplay.Top = _class.System.Class.Vr.VideoHeightOffset;
            imgDisplayVr.Top = _class.System.Class.Vr.VideoHeightOffset;
            imgDisplay.Left = _class.System.Class.Vr.VideoWidthOffset;
            imgDisplayVr.Left = centerWidth + _class.System.Class.Vr.VideoWidthOffset;
        }

        private void tmrSystem_Tick(object sender, EventArgs e)
        {
            _homeClass.LoopController.RunSystemLoop();
        }

        private void tmrMasterControl_Tick(object sender, EventArgs e)
        {
            _homeClass.LoopController.RunMasterControlLoop();
        }
    }
}
