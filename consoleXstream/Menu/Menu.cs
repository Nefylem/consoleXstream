using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using consoleXstream.Config;
using consoleXstream.Input;
using consoleXstream.Remap;

namespace consoleXstream.Menu
{
    public partial class ShowMenu : Form
    {
        public ShowMenu(Form1 form1, Configuration system, KeyboardHook keyHook, VideoCapture.VideoCapture inVideo, Remapping inMap, Keymap keymap)
        {
            _class = new Classes(this);
            _class.DeclareClasses();

            _class.Form1 = form1;
            _class.System = system;
            _class.KeyboardHook = keyHook;
            _class.VideoCapture = inVideo;
            _class.Remap = inMap;
            _class.Keymap = keymap; 

            InitializeComponent();
        }
        private readonly Classes _class;

        private string _strRemapSelected;

        private void frmMenu_Load(object sender, EventArgs e)
        {
            _class.User.ConnectProfile = "";
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

        public void ShowPanel()
        {
            if (_class.Form1 == null) { _class.Form1 = (Form1)Application.OpenForms["Form1"]; }
            _class.Nav.ListHistory = new List<string>();

            _class.CreateMain.Menu();

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;

            TransparencyKey = Color.BlanchedAlmond;
            BackColor = Color.BlanchedAlmond;

            imgDisplay.Dock = DockStyle.Fill;

            //Make as close to invisible as possible, to stop the menu "flashing" into view
            Width = 1;
            Height = 1;
            Opacity = 0.1;

            if (_class.System.BoolStayOnTop)
                TopMost = true;

            Show();

            PositionMenu();

            Opacity = 0.90;
            _class.Var.IsMainMenu = true;
            _class.Nav.SetBackWait(5);
            //_class.SubMenu.GetMenuSize(Width, Height);

            tmrMenu.Enabled = true;
        }

        public void PositionMenu()
        {
            Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (Properties.Resources.imgMainMenu.Width / 2);
            Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (Properties.Resources.imgMainMenu.Height / 2);

            //Enlarge once in the right spot
            Width = Properties.Resources.imgMainMenu.Width;
            Height = Properties.Resources.imgMainMenu.Height;            
        }

        private void tmrMenu_Tick(object sender, EventArgs e)
        {
            _class.Nav.CheckDelays();
            _class.Fps.CheckFps();
            _class.Shutter.CheckDisplay();

            DrawPanel();

            CheckControls();

            BringToFront();
            Focus();
        }

        private void DrawPanel()
        {
            
            _class.DrawGui.ClearGraph(Properties.Resources.imgMainMenu);
            
            if (_class.Var.Setup)
            {
                /*
                if (_var.SetupGamepad)
                    DrawGamepadRemap();
                 */
            }
            else
            {
                _class.MainMenu.Draw();

                if (!_class.Var.IsMainMenu)
                {
                    _class.SubMenu.Draw();
                    if (_class.Var.ShowSubSelection) _class.SubSelectMenu.Draw();
                }
                //drawFooter();
            }

            _class.DrawGui.setOutline(false);

            if (_class.System.boolFPS)
                _class.DrawGui.drawText(5, 500, "FPS: " + _class.Fps.Frames);

            imgDisplay.Image = _class.DrawGui.drawGraph();
        }

        private void CheckControls()
        {
            _class.Keyboard.CheckInput();
            _class.Gamepad.CheckInput();
        }

        private void imgDisplay_MouseMove(object sender, MouseEventArgs e) { _class.Mouse.MouseMove(e); }
        private void imgDisplay_MouseEnter(object sender, EventArgs e) { _class.Mouse.Set(true); }
        private void imgDisplay_MouseLeave(object sender, EventArgs e) { _class.Mouse.Set(false); }
        private void imgDisplay_Click(object sender, EventArgs e) { _class.Mouse.Click(e); }

        public void PassToSubSelect(List<string> listData)
        {
            foreach (var t in listData)
                _class.SubSelectVar.ListData.Add(t);
        }

        public void ClosePanel()
        {
            tmrMenu.Enabled = false;
            Hide();
            _class.Form1.CloseMenuForm();
        }
    }
}
