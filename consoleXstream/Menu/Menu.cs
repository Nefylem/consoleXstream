﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace consoleXstream.Menu
{
    public partial class ShowMenu : Form
    {
        public ShowMenu(BaseClass baseClass)
        {
            _class = new Classes(baseClass, this);
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

            if (_class.Base.System.IsStayOnTop)
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
            if (!_class.Base.System.IsVr)
            {
                Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (Properties.Resources.imgMainMenu.Width / 2);
                Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (Properties.Resources.imgMainMenu.Height / 2);

                //Enlarge once in the right spot
                Width = Properties.Resources.imgMainMenu.Width;
                Height = Properties.Resources.imgMainMenu.Height;                            
            }
            else
            {
                Left = 0;
                Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (Properties.Resources.imgMainMenu.Height / 2);

                Width = Screen.PrimaryScreen.Bounds.Width;
                Height = Properties.Resources.imgMainMenu.Height;                            
            }

        }

        private void tmrMenu_Tick(object sender, EventArgs e)
        {
            if (_class.Var.IsResizeVr)
            {
                CheckControls();
                return;
            }

            _class.Nav.CheckDelays();
            if (_class.Var.SetupGamepad) _class.RemapNav.CheckDelays();

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
                if (_class.Var.SetupGamepad)
                    _class.RemapGamepad.Draw();
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

            if (_class.Base.System.CheckFps)
                _class.DrawGui.drawText(5, 500, "FPS: " + _class.Fps.Frames);

            if (_class.Base.System.IsVr)
            {
                Image menu = _class.DrawGui.drawGraph();
                Image showMenu = new Bitmap(imgDisplay.Width, menu.Height);
                using (var g = Graphics.FromImage(showMenu))
                {
                    int centerPoint = Screen.PrimaryScreen.Bounds.Width/2;
                    int quaterPoint = centerPoint/2;
                    g.DrawImage(menu, new Rectangle(quaterPoint - 300, 0, 600, 500));
                    g.DrawImage(menu, new Rectangle(centerPoint + quaterPoint - 300, 0, 600, 500));
                }
                imgDisplay.Image = showMenu;
            }
            else
            {
                imgDisplay.Image = _class.DrawGui.drawGraph();
            }
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
            _class.Base.Home.CloseMenuForm();
        }
    }
}
