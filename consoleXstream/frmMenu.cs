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
using System.Xml;
using System.Reflection;

namespace consoleXstream
{
    public partial class frmMenu : Form
    {
        private Form1 form1;
        private classSystem system;
        private classKeyboardHook keyboard;
        private classGUIDraw drawGUI;
        private classVideoCapture videoCapture;
        private classRemap remap;

        private class previewVideoCapture
        {
            public string strTitle;
            public string strCrossbarVideo;
            public string strCrossbarAudio;
            public classVideoCapture videoPreview;
            public Bitmap bmpDisplay;
        }
        private class menuItem
        {
            public string strDisplay;
            public string strCommand;
        }
        private class subMenuItem
        {
            public string strDisplay;
            public string strCommand;
            public string strDisplayOption;
            public bool IsFolder;
        }
        private class menuButtons
        {
            public string strCommand;
            public Rectangle rect;
        }

        private List<previewVideoCapture> _previewVideo = new List<previewVideoCapture>();
        private List<List<menuItem>> _listItems = new List<List<menuItem>>();
        private List<subMenuItem> _listSubItems = new List<subMenuItem>();
        private List<menuButtons> _listButtons = new List<menuButtons>();
        private List<menuButtons> _listInactiveButtons = new List<menuButtons>();
        private List<string> _listChecked = new List<string>();
        
        private List<int> _listRow = new List<int>();

        private int _intWatchFPS;
        private int _intCurrentFPS;

        private int _intMoveWaitRight;
        private int _intMoveWaitLeft;
        private int _intMoveWaitUp;
        private int _intMoveWaitDown;
        private int _intMenuBack;
        private int _intMenuOK;

        private int _intMenuOffsetX;
        private int _intMenuOffsetY;
        private int _intCellHeight;
        private int _intCellWidth;

        private int _intShutterStart;
        private int _intShutterEnd;
        private int _intShutterMark;
        private int _intShutterScroll;
        private int _intShutterSlide = 7;

        private bool _boolHideShutter;
        private bool _boolShowShutter;
        private bool _boolFindFPS;
        private bool _boolSetup;
        private bool _boolMainMenu;
        private bool _boolEnableMouseMove;
        private bool _boolShowPreview;
        private bool _boolSetupGamepad;

        private string _strFPSCheck;
        private string _strSelected;
        private string _strSubSelected;
        private string _strSubError;
        private string _strSubExplain;
        private string _strCurrentMenu;
        private string _strCurrentProfile;
        private string _strMouseCurrent;
        private string _strRemapSelected;

        private List<string> _listHistory;

        private GamePadState _controls;

        public frmMenu()
        {
            InitializeComponent();
        }

        public void getSystemHandle(classSystem inSystem) { system = inSystem; }
        public void getKeyboardHandle(classKeyboardHook inKey) { keyboard = inKey; }
        public void getVideoCaptureHandle(classVideoCapture inVideo) { videoCapture = inVideo; }
        public void getRemapHandle(classRemap inMap) { remap = inMap; }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            _strCurrentProfile = "";
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

        public bool isShutterOpen()
        {
            return !_boolMainMenu;
        }

        public void closeShutter()
        {
            _boolHideShutter = true;
        }

        public void closePanel()
        {
            tmrMenu.Enabled = false;
            this.Hide();
            form1.closeMenuForm();
        }

        public void showPanel()
        {
            if (form1 == null) { form1 = (Form1)Application.OpenForms["Form1"]; }
            drawGUI = new classGUIDraw();
            _listHistory = new List<string>();

            createMainMenu();

            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            this.TransparencyKey = Color.BlanchedAlmond;
            this.BackColor = Color.BlanchedAlmond;

            imgDisplay.Dock = DockStyle.Fill;

            //Make as close to invisible as possible, to stop the menu "flashing" into view
            this.Width = 1;
            this.Height = 1;
            this.Opacity = 0.1;

            if (system.boolStayOnTop)
                this.TopMost = true;

            //_boolSetup = true;
            //_boolSetupGamepad = true;
            //changeToGamepadRemap();

            this.Show();

            this.Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (consoleXstream.Properties.Resources.imgMainMenu.Width / 2);
            this.Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (consoleXstream.Properties.Resources.imgMainMenu.Height / 2);
            
            //Enlarge once in the right spot
            this.Width = consoleXstream.Properties.Resources.imgMainMenu.Width;
            this.Height = consoleXstream.Properties.Resources.imgMainMenu.Height;
            
            this.Opacity = 0.90;
            _boolMainMenu = true;

            _boolFindFPS = true;

            _intMenuBack = 5;
            tmrMenu.Enabled = true;
        }

        private void tmrMenu_Tick(object sender, EventArgs e)
        {
            if (_intMenuOK > 0) _intMenuOK--;
            if (_intMenuBack > 0) _intMenuBack--;
            if (_intMoveWaitRight > 0) _intMoveWaitRight--;
            if (_intMoveWaitLeft > 0) _intMoveWaitLeft--;
            if (_intMoveWaitUp > 0) _intMoveWaitUp--;
            if (_intMoveWaitDown > 0) _intMoveWaitDown--;

            if (_boolFindFPS)
            {
                if (_strFPSCheck != DateTime.Now.ToString("ss"))
                {
                    _intWatchFPS = _intCurrentFPS;
                    _intCurrentFPS = 0;
                    _strFPSCheck = DateTime.Now.ToString("ss");
                }
                else
                    _intCurrentFPS++;
            }

            if (_boolShowShutter && !_boolHideShutter)
            {
                if (_intShutterMark < _intShutterEnd - _intShutterStart)
                {
                    _intShutterMark += _intShutterSlide;

                    if (_intShutterMark > _intShutterEnd - _intShutterStart)
                        _intShutterMark = _intShutterEnd - _intShutterStart;
                }
            }

            if (_boolHideShutter)
            {
                if (_intShutterMark > 1)
                    _intShutterMark -= _intShutterSlide;
                else
                {
                    _intShutterMark = 0;
                    _boolHideShutter = false;
                    _boolShowShutter = false;
                    _boolMainMenu = true;
                    clearButtons();
                }
            }

            drawPanel();

            checkControls();

            this.BringToFront();
            this.Focus();
        }

        private void drawPanel()
        {
            
            drawGUI.clearGraph(consoleXstream.Properties.Resources.imgMainMenu);
            
            if (_boolSetup)
            {
                if (_boolSetupGamepad)
                    drawGamepadRemap();
            }
            else
            {
                drawMainMenu();
                
                if (!_boolMainMenu)
                    drawSubmenu();

                //drawFooter();
            }

            drawGUI.setOutline(false);

            if (system.boolFPS)
                drawGUI.drawText(5, 500, "FPS: " + _intWatchFPS.ToString());

            imgDisplay.Image = drawGUI.drawGraph();
        }

        private void drawMainMenu()
        {
            int intX = 10 + _intMenuOffsetX;
            int intY = 10 + _intMenuOffsetY;

            //These calls take time. Do not run during a loop (approx 20fps loss)
            _intCellHeight = consoleXstream.Properties.Resources.imgTileLow.Height;
            _intCellWidth = consoleXstream.Properties.Resources.imgTileLow.Width;

            drawGUI.setFontSize(18f);
            drawGUI.setOutline(true);
            drawGUI.setCenterBottom(true);
            _listRow.Clear();

            for (int intRow = 0; intRow < _listItems.Count; intRow++)
            {
                _listRow.Add(intY);

                for (int intCell = 0; intCell < _listItems[intRow].Count; intCell++ )
                {
                    Rectangle displayRect = new Rectangle(intX, intY, _intCellWidth, _intCellHeight);

                    if (_boolMainMenu)          
                        createButton(displayRect, _listItems[intRow][intCell].strCommand);
                    else
                        createButton(displayRect, _listItems[intRow][intCell].strCommand, false);       //Add to the inactive list incase its needed - mainly by mouse move


                    if (_strSelected == _listItems[intRow][intCell].strCommand)
                        drawGUI.drawImage(intX - 6, intY - 7, 108, 115, consoleXstream.Properties.Resources.imgSubGlow);
                    else
                        drawGUI.drawImage(intX, intY, consoleXstream.Properties.Resources.imgTileLow);

                    drawGUI.centerText(displayRect, _listItems[intRow][intCell].strDisplay);

                    intX += _intCellWidth + 5;
                }
                intY += _intCellHeight + 5;
                intX = 10;
            }
            drawGUI.setOutline(true);
            drawGUI.setFontSize(12f);
        }

        private void drawSubmenu()
        {
            if (_boolShowShutter)
            {
                int intPinAudio = -1;
                int intPinVideo = -1;

                if (system.boolInternalCapture && videoCapture.XBar != null && _strCurrentMenu == "video input")
                {
                    videoCapture.XBar.get_IsRoutedTo(0, out intPinVideo);
                    videoCapture.XBar.get_IsRoutedTo(1, out intPinAudio);
                }

                Bitmap bmpShutter = new Bitmap(this.Width - 20, _intShutterEnd - _intShutterStart);
                int intShutterHeight = _intShutterEnd - _intShutterStart;

                drawGUI.drawImage(bmpShutter, 0, 0, consoleXstream.Properties.Resources.imgSubMenu);

                if (_strSubError.Length == 0)
                {
                    int intX = 0;
                    if (_listSubItems.Count < 4)
                    {
                        int intSetWidth = _listSubItems.Count * (_intCellWidth + 5);
                        intX = ((this.Width - 20) / 2) - (intSetWidth / 2);
                    }
                    for (int intCount = _intShutterScroll; intCount < _listSubItems.Count; intCount++)
                    {
                        Rectangle displayRect = new Rectangle(intX, 2, _intCellWidth, _intCellHeight - 4);                  //Outline
                        Rectangle displayRectText = new Rectangle(intX, 2, _intCellWidth, _intCellHeight - 24);             //Text
                        Rectangle buttonRect = new Rectangle(8 + intX, _intShutterStart, _intCellWidth, _intCellHeight);    //Mouse over location

                        createButton(buttonRect, _listSubItems[intCount].strCommand);
                        
                        if (_strSubSelected == _listSubItems[intCount].strCommand)
                            drawGUI.drawImage(bmpShutter, displayRect, consoleXstream.Properties.Resources.imgSubGlow); 


                        bool boolDrawCheck = false;

                        if (system.boolInternalCapture && videoCapture.XBar != null && _strCurrentMenu == "video input")
                            if (intCount == intPinVideo || intCount == intPinAudio) 
                                boolDrawCheck = true;

                        if (_listChecked.IndexOf(_listSubItems[intCount].strDisplay) > -1 || boolDrawCheck)
                            drawGUI.drawImage(bmpShutter, intX + _intCellWidth - 40, 17, 25, 25, consoleXstream.Properties.Resources.imgTick);

                        if (_boolShowPreview)
                            drawPreviewWindow(bmpShutter, displayRect, _listSubItems[intCount].strDisplay);

                        if (_listSubItems[intCount].IsFolder)
                        {
                            //draw folder option (three dots)
                            drawGUI.drawImage(bmpShutter, intX + _intCellWidth - 50, 90, 60, 60, consoleXstream.Properties.Resources.ThreeDots);
                            Rectangle displayRectTextOption = new Rectangle(intX, 2, _intCellWidth, _intCellHeight - 60);
                            Rectangle displayRectTextDisplay = new Rectangle(intX, 2, _intCellWidth, _intCellHeight - 34);             
                            drawGUI.centerText(bmpShutter, displayRectTextOption, _listSubItems[intCount].strDisplayOption);
                            drawGUI.centerText(bmpShutter, displayRectTextDisplay, _listSubItems[intCount].strDisplay);
                        }
                        else
                            drawGUI.centerText(bmpShutter, displayRectText, _listSubItems[intCount].strDisplay);


                        intX += _intCellWidth + 5;
                    }
                }
                else
                {
                    drawGUI.setCenter();
                    drawGUI.setFontSize(24f);
                    drawGUI.centerText(bmpShutter, new Rectangle(0, 0, this.Width - 20, _intShutterEnd - _intShutterStart), _strSubError);
                    if (_strSubExplain.Length > 0)
                    {
                        drawGUI.setFontSize(14f);
                        drawGUI.setCenterBottom(true);
                        drawGUI.centerText(bmpShutter, new Rectangle(0, 0, this.Width - 20, _intShutterEnd - _intShutterStart - 20), _strSubExplain);
                    }
                }

                drawGUI.drawImage(new Rectangle(8, _intShutterStart, 581, _intShutterMark), bmpShutter);
            }
            drawGUI.setFontSize(12f);
        }

        private void drawPreviewWindow(Bitmap bmpShutter, Rectangle displayRect, string strCommand)
        {
            int intIndex = -1;
            for (int intCount = 0; intCount < _previewVideo.Count; intCount++)
            {
                if (_previewVideo[intCount].strTitle.ToLower() == strCommand.ToLower())
                    intIndex = intCount;
            }

            if (intIndex > -1)
                drawGUI.drawImage(bmpShutter, displayRect, _previewVideo[intIndex].bmpDisplay);
        }

        private void clearButtons()
        {
            _listInactiveButtons.Clear();
            _listButtons.Clear();
        }

        private void createButton(Rectangle rect, string strCommand)
        {
            for (int intCount = 0; intCount < _listButtons.Count; intCount++)
            {
                if (_listButtons[intCount].strCommand.ToLower() == strCommand.ToLower())
                    return;
            }

            _listButtons.Add(new menuButtons());
            int intIndex = _listButtons.Count - 1;

            _listButtons[intIndex].strCommand = strCommand;
            _listButtons[intIndex].rect = rect;
        }

        private void createButton(Rectangle rect, string strCommand, bool boolInactive)
        {
            for (int intCount = 0; intCount < _listInactiveButtons.Count; intCount++)
            {
                if (_listInactiveButtons[intCount].strCommand.ToLower() == strCommand.ToLower())
                    return;
            }

            _listInactiveButtons.Add(new menuButtons());
            int intIndex = _listInactiveButtons.Count - 1;

            _listInactiveButtons[intIndex].strCommand = strCommand;
            _listInactiveButtons[intIndex].rect = rect;
        }

        #region Create Menu Items
        private int newMenu()
        {
            _listItems.Add(new List<menuItem>());
            return _listItems.Count - 1;
        }

        private void addSubItem(string strCommand, string strTitle)
        {
            _listSubItems.Add(new subMenuItem());
            int intID = _listSubItems.Count - 1;

            _listSubItems[intID].strCommand = strCommand;
            _listSubItems[intID].strDisplay = strTitle;
        }

        private void addSubItem(string strCommand, string strTitle, bool IsCheck)
        {
            _listSubItems.Add(new subMenuItem());
            int intID = _listSubItems.Count - 1;

            _listSubItems[intID].strCommand = strCommand;
            _listSubItems[intID].strDisplay = strTitle;

            if (IsCheck == true)
                _listChecked.Add(strTitle);
        }

        private void addSubItemFolder(string command, string title, string setData)
        {
            _listSubItems.Add(new subMenuItem());
            int intID = _listSubItems.Count - 1;

            _listSubItems[intID].strCommand = command;
            _listSubItems[intID].strDisplay = title;
            _listSubItems[intID].IsFolder = true;
            _listSubItems[intID].strDisplayOption = findSubOption(setData);
        }

        private string findSubOption(string command)
        {
            command = command.ToLower();

            if (command == "capture resolution") return system.strCurrentResolution;
            if (command == "graphics card") return system.getGraphicsCard();
            if (command == "screen refresh") return system.getRefreshRate();
            if (command == "display resolution") return system.getResolution();
            if (command == "volume") return system.getVolume();

            return "";
        }

        private void addMenuItem(int intIndex, string strCommand, string strTitle)
        {
            _listItems[intIndex].Add(new menuItem());

            int intID = _listItems[intIndex].Count - 1;

            _listItems[intIndex][intID].strCommand = strCommand;
            _listItems[intIndex][intID].strDisplay = strTitle;
        }

        private void createMainMenu()
        {
            _listItems.Clear();
            //Console
            int intIndex = newMenu();
            addMenuItem(intIndex, "Console Select", "Connect To");
            addMenuItem(intIndex, "Save Profile", "Save Profile");
            addMenuItem(intIndex, "Power On", "Power On");
            addMenuItem(intIndex, "Graph", "Graph");

            //Video
            intIndex = newMenu();
            addMenuItem(intIndex, "Video Input", "Video Input");
            addMenuItem(intIndex, "Video Device", "Device");
            addMenuItem(intIndex, "Video Settings", "Capture \nSettings");
            addMenuItem(intIndex, "Video Display", "Display");

            //Controller
            intIndex = newMenu();
            addMenuItem(intIndex, "Controller Output", "Output");
            addMenuItem(intIndex, "Device", "Controller \n Settings");
            addMenuItem(intIndex, "Remap", "Remap \nInputs");
            addMenuItem(intIndex, "Profile", " Input \nProfile");

            //System
            intIndex = newMenu();
            addMenuItem(intIndex, "System Status", "Status");
            addMenuItem(intIndex, "Config", "Configuration");
            addMenuItem(intIndex, "Exit", "Exit");

            _strSelected = "Console Select";
        }
        #endregion
        #region Keyboard / Gamepad controls
        private void checkControls()
        {
            checkKeyboardControls();
            checkGamepadControls();
        }

        //Enable keyboard controls in the menu, even if keyboard disabled for gameplay
        private void checkKeyboardControls()
        {
            if (keyboard.getKey(system.keyDef.strDpadDown) && _intMoveWaitDown == 0) menuDown();
            if (keyboard.getKey(system.keyDef.strDpadUp) && _intMoveWaitUp == 0) menuUp();
            if (keyboard.getKey(system.keyDef.strDpadLeft) && _intMoveWaitLeft == 0) menuLeft();
            if (keyboard.getKey(system.keyDef.strDpadRight) && _intMoveWaitRight == 0) menuRight();

            if (keyboard.getKey(system.keyDef.strButtonB) || keyboard.getKey(system.keyDef.strButtonBack))
            {
                if (_intMenuBack == 0)
                    menuBack();
                else
                    _intMenuBack = 5;
            }

            if (keyboard.getKey(system.keyDef.strButtonA) || keyboard.getKey(system.keyDef.strButtonStart) || keyboard.getKey(system.keyAltDef.strButtonA) )
            {
                if (_intMenuOK <= 0)
                    menuOK();
                else
                    _intMenuOK = 5;
            }

        }

        private void checkGamepadControls()
        {
            _controls = GamePad.GetState(PlayerIndex.One);
            if (_controls.DPad.Down && _intMoveWaitDown == 0) menuDown();
            if (_controls.DPad.Up && _intMoveWaitUp == 0) menuUp(); 
            if (_controls.DPad.Left && _intMoveWaitLeft == 0) menuLeft(); 
            if (_controls.DPad.Right && _intMoveWaitRight == 0) menuRight();

            if (_controls.Buttons.B || _controls.Buttons.Back)
            {
                if (_intMenuBack == 0)
                    menuBack();
                else
                    _intMenuBack = 5;
            }

            if (_controls.Buttons.A || _controls.Buttons.Start)
            {
                if (_intMenuOK <= 0)
                    menuOK();
                else
                    _intMenuOK = 5;
            }
        }

        private void menuDown()
        {
            for (int intCount = 0; intCount < _listButtons.Count; intCount++)
            {
                if (_listButtons[intCount].strCommand == _strSelected || _listButtons[intCount].strCommand == _strSubSelected)
                {
                    int intX = _listButtons[intCount].rect.Left + (_listButtons[intCount].rect.Width / 2);
                    int intY = _listButtons[intCount].rect.Top + (_listButtons[intCount].rect.Height / 2);
                    if (findNewLocation(intX, intY + _intCellHeight))
                        _intMoveWaitDown = setMoveWait();
                    break;
                }
            }
        }

        private void menuUp()
        {
            for (int intCount = 0; intCount < _listButtons.Count; intCount++)
            {
                if (_listButtons[intCount].strCommand == _strSelected || _listButtons[intCount].strCommand == _strSubSelected)
                {
                    int intX = _listButtons[intCount].rect.Left + (_listButtons[intCount].rect.Width / 2);
                    int intY = _listButtons[intCount].rect.Top + (_listButtons[intCount].rect.Height / 2);
                    if (findNewLocation(intX, intY - _intCellHeight))
                    {
                        _intMoveWaitUp = setMoveWait();
                    }
                    break;
                }
            }
        }

        private void menuLeft()
        {
            for (int intCount = 0; intCount < _listButtons.Count; intCount++)
            {
                if (_listButtons[intCount].strCommand == _strSelected || _listButtons[intCount].strCommand == _strSubSelected)
                {
                    int intX = _listButtons[intCount].rect.Left + (_listButtons[intCount].rect.Width / 2);
                    int intY = _listButtons[intCount].rect.Top + (_listButtons[intCount].rect.Height / 2);
                    if (findNewLocation(intX - _intCellWidth, intY))
                    {
                        _intMoveWaitLeft = setMoveWait();
                        if (_boolShowShutter)
                        {
                            int index = findScrollIndex();
                            if (index > -1)
                            {
                                if (index - _intShutterScroll < 1)
                                {
                                    _intShutterScroll = index - 1;
                                }

                                if (_intShutterScroll < 0)
                                    _intShutterScroll = 0;
                            }
                        }
                    }
                    break;
                }
            }
        }

        private void menuRight()
        {
            for (int intCount = _intShutterScroll; intCount < _listButtons.Count; intCount++)
            {
                if (_listButtons[intCount].strCommand == _strSelected || _listButtons[intCount].strCommand == _strSubSelected)
                {
                    int intX = _listButtons[intCount].rect.Left + (_listButtons[intCount].rect.Width / 2);
                    int intY = _listButtons[intCount].rect.Top + (_listButtons[intCount].rect.Height / 2);

                    if (findNewLocation(intX + _intCellWidth, intY))
                    {
                        _intMoveWaitRight = setMoveWait();
                        if (_boolShowShutter)
                        {
                            int index = findScrollIndex();
                            if (index > -1)
                            {
                                if (index > _intShutterScroll + 2)
                                {
                                    _intShutterScroll = index - 2;

                                    if (_intShutterScroll + 4 > _listButtons.Count)
                                    {
                                        _intShutterScroll = _listButtons.Count - 4;
                                    }
                                }

                            }
                        }
                    }
                    break;
                }
            }
        }


        private int findScrollIndex()
        {
            for (int count = 0; count < _listButtons.Count; count++)
            {
                if (_strSubSelected.ToLower() == _listButtons[count].strCommand.ToLower())
                {
                    return count;
                }
            }
            return -1;
        }

        private void menuBack()
        {
            if (_boolSetup == true)
            {
                _boolSetup = false;
                _boolSetupGamepad = false;
            }
            else
            {
                if (_listHistory.Count == 0)
                    closePanel();
                else
                {
                    _intMenuBack = 5;
                    _listHistory.RemoveAt(_listHistory.Count - 1);
                    if (_listHistory.Count == 0)
                        _boolHideShutter = true;
                }
            }
        }

        private void menuOK()
        {
            _intMenuOK = 5;

            if (_boolMainMenu)
                processMainMenu(_strSelected);
            else
                processSubMenu(_strSubSelected);
        }

        //Slow down movement. At 60fps, can move extremely fast
        private int setMoveWait()
        {
            if (_intWatchFPS > 20 )
                return _intWatchFPS / 6;
            else
                return 3;
        }

        private bool findNewLocation(int intX, int intY)
        {
            for (int intCount = 0; intCount < _listButtons.Count; intCount++)
            {
                if (intY >= _listButtons[intCount].rect.Top && intY <= _listButtons[intCount].rect.Bottom)
                {
                    if (intX >= _listButtons[intCount].rect.Left && intX <= _listButtons[intCount].rect.Right)
                    {
                        if (_strMouseCurrent != _listButtons[intCount].strCommand)
                        {
                            if (_boolMainMenu)
                                _strSelected = _listButtons[intCount].strCommand;
                            else
                                _strSubSelected = _listButtons[intCount].strCommand;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        //Sets motion if moved by mouse
        private bool findNewLocation(int intX, int intY, bool boolMouseMove)
        {
            for (int intCount = 0; intCount < _listButtons.Count; intCount++)
            {
                if (intY >= _listButtons[intCount].rect.Top && intY <= _listButtons[intCount].rect.Bottom)
                {
                    if (intX >= _listButtons[intCount].rect.Left && intX <= _listButtons[intCount].rect.Right)
                    {
                        if (_boolSetupGamepad == true)
                        {
                            int a = 0;
                            a = a;
                        }
                        if (_strMouseCurrent != _listButtons[intCount].strCommand)
                        {
                            _strMouseCurrent = _listButtons[intCount].strCommand;

                            if (_boolMainMenu)
                                _strSelected = _listButtons[intCount].strCommand;
                            else
                                _strSubSelected = _listButtons[intCount].strCommand;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
        #region Mouse Control
        private void imgDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            int mouseX = e.Location.X;
            int mouseY = e.Location.Y;

            if (_boolEnableMouseMove)
                findNewLocation(mouseX, mouseY, true);
        }

        private void imgDisplay_MouseEnter(object sender, EventArgs e)
        {
            _boolEnableMouseMove = true;
        }

        private void imgDisplay_MouseLeave(object sender, EventArgs e)
        {
            _boolEnableMouseMove = false;
        }

        private void imgDisplay_Click(object sender, EventArgs e)
        {
            //TODO: Check if mouse is inside display box, otherwise jump out of menu
            if (_boolEnableMouseMove)
            {
                int intMouseX = Cursor.Position.X - this.Left;
                int intMouseY = Cursor.Position.Y - this.Top;

                if (_boolMainMenu)
                    menuOK();
                else
                {
                    for (int intCount = 0; intCount < _listButtons.Count; intCount++)
                    {
                        if (_listButtons[intCount].strCommand == _strSubSelected)
                        {
                            if (intMouseX >= _listButtons[intCount].rect.Left && intMouseX <= _listButtons[intCount].rect.Right)
                            {
                                if (intMouseY >= _listButtons[intCount].rect.Top && intMouseY <= _listButtons[intCount].rect.Bottom)
                                {
                                    menuOK();
                                    return;
                                }
                            }
                        }
                    }

                    if (intMouseY < _intShutterStart || intMouseY > _intShutterEnd)
                    {
                        _boolHideShutter = true;
                        for (int intCount = 0; intCount < _listInactiveButtons.Count; intCount++)
                        {
                            if (intMouseY >= _listInactiveButtons[intCount].rect.Top && intMouseY <= _listInactiveButtons[intCount].rect.Bottom)
                                if (intMouseX >= _listInactiveButtons[intCount].rect.Left && intMouseX <= _listInactiveButtons[intCount].rect.Right)
                                    _strSelected = _listInactiveButtons[intCount].strCommand;
                        }
                    }
                }
            }
        }

        #endregion
        #region Action Handlers
        private void processMainMenu(string strCommand)
        {
            strCommand = strCommand.ToLower();

            int intRowTop = -10;
            int intCurrentRow = -1;

            //Find where the button is
            for (int intCount = 0; intCount < _listButtons.Count; intCount++)
            {
                if (_listButtons[intCount].strCommand == _strSelected)
                {
                    intRowTop = _listButtons[intCount].rect.Top;
                    break;
                }
            }

            clearButtons();                     //Stop the mouse from being able to select main menu buttons

            //Find what row it belongs too
            for (int intCount = 0; intCount < _listRow.Count; intCount++)
            {
                if (_listRow[intCount] == intRowTop)
                    intCurrentRow = intCount;
            }
            #region Console
            if (strCommand == "console select")
            {
                //TODO: preview connection screens. Preload connection details
                clearSub(strCommand);
                listProfiles();
                activateShutter(intCurrentRow, intCurrentRow + 1);
            }
            if (strCommand == "save profile")
            {
                //TODO: lookup for additional connections
                clearSub(strCommand);

                addSubItem("PlayStation3", "PlayStation3");
                addSubItem("PlayStation4", "PlayStation4");
                addSubItem("Xbox360", "Xbox360");
                addSubItem("XboxOne", "XboxOne");

                selectSubItem();

                activateShutter(intCurrentRow, intCurrentRow + 1);
            }
            #endregion
            #region Video
            if (strCommand == "video input")
            {
                clearSub(strCommand);
                findVideoCrossbar();
                activateShutter(intCurrentRow, intCurrentRow + 1);
            }

            if (strCommand == "video device")
            {
                clearSub(strCommand);
                findVideoDevice();
                activateShutter(intCurrentRow, intCurrentRow + 1);

            }

            if (strCommand == "video settings")
            {
                clearSub(strCommand);
                activateShutter(intCurrentRow, intCurrentRow + 1);
                
                addSubItem("Crossbar", "Crossbar", checkSystemSetting("Crossbar"));
                addSubItem("AVIRender", "AVI Renderer", checkSystemSetting("AVIRender"));
                addSubItem("CheckCaptureRes", "Check Capture", checkSystemSetting("CheckCaptureRes"));
                addSubItemFolder("Resolution", "Resolution", "Capture Resolution");

                selectSubItem();
            }

            if (strCommand == "video display")
            {
                clearSub(strCommand);
                activateShutter(intCurrentRow, intCurrentRow + 1);

                addSubItem("AutoSet", "Auto Set", system.boolAutoSetResolution);
                addSubItemFolder("Device", "Graphics Device", "Graphics Card");
                addSubItemFolder("Resolution", "Resolution", "Display Resolution");
                addSubItemFolder("Refresh", "Refresh Rate", "Screen Refresh");
                addSubItemFolder("Volume", "Volume", "Volume");
                addSubItem("StayOnTop", "Stay On Top", system.boolAutoSetResolution);

                selectSubItem();
            }
            #endregion
            #region Controller
            if (strCommand == "device")
            {
                clearSub(strCommand);
                addSubItem("DS4 Emulation", "DS4 Emulation");
                addSubItem("Normalize", "Normalize");

                checkDisplaySettings();

                _strSubSelected = _listSubItems[0].strCommand;
                activateShutter(intCurrentRow - 2, intCurrentRow - 1);

            }

            if (strCommand == "controller output")
            {
                clearSub(strCommand);

                addSubItem("ControllerMax", "ControllerMax");
                addSubItem("TitanOne", "TitanOne");
                addSubItem("GIMX", "GIMX");
                addSubItem("Remote GIMX", "Remote GIMX");
                addSubItem("McShield", "McShield");
                addSubItem("Control VJOY", "Control VJOY");

                checkDisplaySettings();
                _strSubSelected = _listSubItems[0].strCommand;

                activateShutter(intCurrentRow - 2, intCurrentRow - 1);
            }

            if (strCommand == "remap")
            {
                clearSub(strCommand);

                addSubItem("Gamepad", "Gamepad");
                addSubItem("Keyboard", "Keyboard");
                addSubItem("Mouse", "Mouse");
                //addSubItem("Touch", "Touch");

                _strSubSelected = _listSubItems[0].strCommand;
                activateShutter(intCurrentRow - 2, intCurrentRow - 1);
            }
            #endregion
            #region System
            if (strCommand == "config")
            {
                clearSub(strCommand);
                activateShutter(0, intCurrentRow);
            }
            if (strCommand == "exit") 
            {
                clearSub(strCommand);
                addSubItem("exit", "Yes");
                addSubItem("back", "No");
                _strSubSelected = _listSubItems[1].strCommand;
                activateShutter(intCurrentRow - 2, intCurrentRow - 1);
            }
            #endregion
        }

        private bool checkSystemSetting(string strCommand)
        {
            if (system.checkUserSetting(strCommand.ToLower()).ToLower() == "true")
                return true;
            else
                return false;
        }

        private void processSubMenu(string strCommand)
        {
            if (_strCurrentMenu == "console select") loadProfile(strCommand);
            if (_strCurrentMenu == "video input") changeCrossbar(strCommand);
            if (_strCurrentMenu == "video device") changeVideoDevice(strCommand);
            if (_strCurrentMenu == "save profile") saveConnectProfile(strCommand);
            if (_strCurrentMenu == "controller output") changeSetting(strCommand);
            if (_strCurrentMenu == "device") changeSetting(strCommand);
            if (_strCurrentMenu == "video settings") changeSetting(strCommand);
            if (_strCurrentMenu == "resolution") changeResolution(strCommand);
            if (_strCurrentMenu == "video display") changeVideoDisplay(strCommand);
            if (_strCurrentMenu == "remap") changeRemapScreen(strCommand);

            if (_strCurrentMenu == "videoresolution") changeVideoResolution(strCommand);
            if (_strCurrentMenu == "videorefresh") changeVideoRefresh(strCommand);
            
            if (_strCurrentMenu == "exit")
            {
                if (strCommand.ToLower() == "exit") form1.closeSystem();
                if (strCommand.ToLower() == "back") menuBack(); 
            }
        }

        private void clearSub(string strCommand)
        {
            _boolMainMenu = false;

            _strCurrentMenu = strCommand;
            _listHistory.Add(strCommand);

            _listSubItems.Clear();
            _listChecked.Clear();
            _strSubError = "";
            _strSubExplain = "";

            _intShutterScroll = 0;
        }

        //TODO: fix row select. Hangover from old code
        private void activateShutter(int intCurrentRow, int intTargetRow)
        {
            _intShutterStart = _listRow[intTargetRow];
            _intShutterEnd = _listRow[intTargetRow] + _intCellHeight;

            _intShutterMark = 0;
            if (_intWatchFPS > 20)
                _intShutterSlide = _intWatchFPS / 3;
            else
                _intShutterSlide = 10;

            _boolHideShutter = false;
            _boolShowShutter = true;
        }

        //TODO: look into list for last selected object / scroll. For now, just revert to 1
        private void selectSubItem()
        {
            if (_listSubItems.Count > 0)
            {
                _strSubSelected = _listSubItems[0].strCommand;
            }
        }
        #endregion

        #region Menu Actions
        private void listProfiles()
        {
            _listSubItems.Clear();
            if (Directory.Exists("Profiles") == true)
            {
                string[] listDir = Directory.GetFiles(@"Profiles", "*.connectProfile");
                if (listDir.Count() > 0)
                {
                    for (int intCount = 0; intCount < listDir.Length; intCount++)
                    {
                        string strName = Path.GetFileNameWithoutExtension(listDir[intCount].ToString());
                        addSubItem(strName, strName);

                        if (_strCurrentProfile.ToLower() == strName.ToLower())
                            _listChecked.Add(strName);
                    }
                }
            }

            if (_listSubItems.Count == 0)
            {
                _strSubError = "No profiles found";
                _strSubExplain = "Please set up your console display then use Save Profile";
            }
            else
                selectSubItem();
        }

        private void loadProfile(string strFile)
        {
            string strDevice = "";
            string strAudio = "";
            string strVideoPin = "";
            string strAudioPin = "";

            string strSetting = "";
            if (Directory.Exists("Profiles") == true)
            {
                if (File.Exists(@"Profiles\" + strFile + ".connectProfile") == true)
                {
                    XmlTextReader reader = new XmlTextReader(@"Profiles\" + strFile + ".connectProfile");
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: 
                                break;
                            case XmlNodeType.Text: //Display the text in each element.
                                strSetting = reader.Value;
                                break;
                            case XmlNodeType.EndElement: 
                                if (strSetting.Length > 0)
                                {
                                    if (reader.Name.ToLower() == "device") { strDevice = strSetting; }
                                    if (reader.Name.ToLower() == "audio") { strAudio = strSetting; }
                                    if (reader.Name.ToLower() == "videopin") { strVideoPin = strSetting; }
                                    if (reader.Name.ToLower() == "audiopin") { strAudioPin = strSetting; }
                                }
                                strSetting = "";
                                break;
                        }
                    }
                    reader.Close();

                    _strCurrentProfile = strFile;
                    system.addUserData("CurrentProfile", strFile);
                    system.addUserData("VideoCaptureDevice", strDevice);
                    system.addUserData("AudioPlaybackDevice", strAudio);
                    if (strVideoPin.Length > 0) system.addUserData("crossbarVideoPin", strVideoPin);
                    if (strAudio.Length > 0) system.addUserData("crossbarAudioPin", strAudioPin);
                    
                    _listChecked.Clear();
                    _listChecked.Add(strFile);

                    videoCapture.setVideoCaptureDevice(strDevice);
                    //TODO: set Audio device
                    videoCapture.setCrossbar(strVideoPin);
                    videoCapture.setCrossbar(strAudioPin);
                    videoCapture.runGraph();
                }
            }
        }

        private void findVideoCrossbar()
        {
            if (system.boolInternalCapture)
            {
                for (int intCount = 0; intCount < videoCapture.listCrossbarInput.Count; intCount++)
                {
                    string strTitle = videoCapture.listCrossbarInput[intCount];
                    if (strTitle.ToLower() == "video_serialdigital") { strTitle = "HDMI"; }
                    if (strTitle.ToLower() == "video_yryby") { strTitle = "Component"; }
                    if (strTitle.ToLower() == "audio_spdifdigital") { strTitle = "Digital Audio"; }
                    if (strTitle.ToLower() == "audio_line") { strTitle = "Line Audio"; }

                    addSubItem(videoCapture.listCrossbarInput[intCount], strTitle);
                }

                if (_listSubItems.Count == 0)
                {
                    _strSubError = "No connections found";
                    _strSubExplain = "Your capture device has no available crossbar information";
                }
                else
                    selectSubItem();
            }
            else
            {
                _strSubError = "No implied control for external capture";
                _strSubExplain = "Please use your applications input select options";
            }
        }

        private void findVideoDevice()
        {
            if (system.boolInternalCapture)
            {
                for (int intCount = 0; intCount < videoCapture.listVideoCapture.Count; intCount++)
                {
                    addSubItem(videoCapture.listVideoCaptureName[intCount], videoCapture.listVideoCaptureName[intCount]);
                    //addNewVideoCapture(videoCapture.listVideoCaptureName[intCount]);
                    //open up a preview window for this
                    //videoCapture = new classVideoCapture(this);
                }

                if (_listSubItems.Count == 0)
                {
                    _strSubError = "No capture devices found";
                    _strSubExplain = "";
                }
                else
                {
                    selectSubItem();
                    _listChecked.Clear();
                    _listChecked.Add(videoCapture.strVideoCaptureDevice);
                }
            }
            else
            {
                _strSubError = "No implied control for external capture";
                _strSubExplain = "Please use your applications device select options";
            }
        }
        //TODO: create second device by crossbar
        private void addNewVideoCapture(string strVideoDevice)
        {
            int intIndex = -1;
            for (int intCount = 0; intCount < _previewVideo.Count; intCount++)
            {
                if (strVideoDevice.ToLower() == _previewVideo[intCount].strTitle.ToLower())
                    intIndex = intCount;
            }

            //If the device is already running in a graph, dont create another one.
            //if (strVideoDevice == form1.videoCapture) intIndex = -1;
            
            if (intIndex == -1)
            {
                _previewVideo.Add(new previewVideoCapture());
                intIndex = _previewVideo.Count - 1;
                _previewVideo[intIndex].strTitle = strVideoDevice;
                //Add crossbar here
                _previewVideo[intIndex].videoPreview = new classVideoCapture(form1);
                _previewVideo[intIndex].videoPreview.getSystemHandle(system);
                _previewVideo[intIndex].videoPreview.initialzeCapture();
                _previewVideo[intIndex].videoPreview.setVideoCaptureDevice(strVideoDevice);
                //Find video device crossbar
                //Send crossbar command here
                //Dynamically check display bound values
                _previewVideo[intIndex].bmpDisplay = new Bitmap(150, 110);
                Rectangle displayRect = new Rectangle(0, 0, 150, 110);
                drawGUI.setCenterTop(true);
                drawGUI.drawTextJustify(_previewVideo[intIndex].bmpDisplay, displayRect, "No preview");

                _previewVideo[intIndex].videoPreview.setPreviewWindow(true);
                _previewVideo[intIndex].videoPreview.setPreviewWindowHandle(_previewVideo[intIndex].bmpDisplay.GetHbitmap());
                _previewVideo[intIndex].videoPreview.setPreviewWindowBounds(new Point(150, 110));
                _previewVideo[intIndex].videoPreview.runGraph();

                _boolShowPreview = true;
            }
        }
        private void changeCrossbar(string strSet)
        {
            int intIndex = videoCapture.listCrossbarInput.IndexOf(strSet);
            if (intIndex > -1)
            {
                if (strSet.Length > "video_".Length)
                {
                    if (strSet.Substring(0, "video_".Length).ToLower() == "video_")
                    {
                        system.addUserData("crossbarVideoPin", strSet);

                        //Set the audio pin if selecting HDMI
                        if (strSet.ToLower() == "video_serialdigital")
                        {
                            system.addUserData("crossbarAudioPin", "audio_spdifdigital");
                            videoCapture.setCrossbar("audio_spdifdigital");
                        }

                        videoCapture.setCrossbar(strSet);
                        videoCapture.runGraph();
                    }
                    if (strSet.Substring(0, "audio_".Length).ToLower() == "audio_")
                    {
                        system.addUserData("crossbarAudioPin", strSet);
                        videoCapture.setCrossbar(strSet);
                        videoCapture.runGraph();
                    }

                }
            }
        }

        private void changeVideoDevice(string strSet)
        {
            if (system.boolInternalCapture)
            {
                int intIndex = videoCapture.listVideoCaptureName.IndexOf(strSet);
                if (intIndex > -1)
                {
                    system.addUserData("VideoCaptureDevice", strSet);
                    videoCapture.setVideoCaptureDevice(strSet);
                    videoCapture.runGraph();
                    _listChecked.Clear();
                    _listChecked.Add(videoCapture.strVideoCaptureDevice);
                }
            }

        }

        private void saveConnectProfile(string strCommand)
        {
            _listChecked.Clear();
            _listChecked.Add(strCommand);

            _strCurrentProfile = strCommand;
            system.addUserData("CurrentProfile", strCommand);

            string strTitle = strCommand;
            strCommand = strCommand.Replace(" ", String.Empty);

            if (Directory.Exists("Profiles") == false) { Directory.CreateDirectory("Profiles"); }
            if (File.Exists(@"Profiles\" + strCommand + ".connectProfile") == true) { File.Delete(@"Profiles\" + strCommand + ".connectProfile"); }

            string strDev = videoCapture.strVideoCaptureDevice;
            string strAud = videoCapture.strAudioPlaybackDevice;
            string strCrossVideo = "";
            string strCrossAudio = "";

            if (videoCapture.XBar != null)
            {
                int intPinVideo = -1;
                int intPinAudio = -1;
                videoCapture.XBar.get_IsRoutedTo(0, out intPinVideo);
                videoCapture.XBar.get_IsRoutedTo(1, out intPinAudio);
                strCrossVideo = videoCapture.showCrossbarOutput(intPinVideo, "Video");
                strCrossAudio = videoCapture.showCrossbarOutput(intPinAudio, "Audio");
            }

            //Control method

            string strSave = "<Profile>";
            strSave += "<Title>" + strTitle + "</Title>";
            strSave += "<videoCaptureSettings>";
            strSave += "<device>" + strDev + "</device>";
            strSave += "<audio>" + strAud + "</audio>";
            strSave += "</videoCaptureSettings>";
            if ((strCrossAudio.Length > 0) || (strCrossVideo.Length > 0))
            {
                strSave += "<videoInput>";
                if (strCrossVideo.Length > 0) { strSave += "<videoPin>" + strCrossVideo + "</videoPin>"; }
                if (strCrossAudio.Length > 0) { strSave += "<audioPin>" + strCrossAudio + "</audioPin>"; }
                strSave += "</videoInput>";
            }
            strSave += "</Profile>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strSave);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(@"Profiles\" + strCommand + ".connectProfile", settings);
            doc.Save(writer);
            writer.Close();
        }

        private void changeSetting(string strCommand)
        {
            strCommand = strCommand.ToLower();
            if (strCommand == "ds4 emulation") system.changeDS4Emulation();
            if (strCommand == "normalize") system.changeNormalizeGamepad();
            if (strCommand == "controllermax") system.changeControllerMax();
            if (strCommand == "titanone") system.changeTitanOne();

            if (strCommand == "resolution") listCaptureResolution();
            if (strCommand == "avirender") system.changeAVIRender();
            if (strCommand == "checkcaptureres") system.changeCaptureAutoRes();

            /*
            addSubItem("GIMX", "GIMX");
            addSubItem("Remote GIMX", "Remote GIMX");
            addSubItem("McShield", "McShield");
            addSubItem("Control VJOY", "Control VJOY");
            */
            checkDisplaySettings();
        }

        private void checkDisplaySettings()
        {
            _listChecked.Clear();

            for (int intCount = 0; intCount < _listSubItems.Count; intCount++)
            {
                if (_listSubItems[intCount].strCommand.ToLower() == "ds4 emulation")
                    if (system.boolPS4ControllerMode) _listChecked.Add("DS4 Emulation");
              
                if (_listSubItems[intCount].strCommand.ToLower() == "normalize")
                    if (system.boolNormalizeControls) _listChecked.Add("Normalize");

                if (_listSubItems[intCount].strCommand.ToLower() == "controllermax")
                    if (system.boolControllerMax) _listChecked.Add("ControllerMax");

                if (_listSubItems[intCount].strCommand.ToLower() == "titanone")
                    if (system.boolTitanOne) _listChecked.Add("TitanOne");

                if (_listSubItems[intCount].strCommand.ToLower() == "gimx")
                    if (system.boolGIMX) _listChecked.Add("GIMX");

                if (_listSubItems[intCount].strCommand.ToLower() == "remote gimx")
                    if (system.boolRemoteGIMX) _listChecked.Add("remote gimx");

                if (_listSubItems[intCount].strCommand.ToLower() == "McShield")
                    if (system.boolMcShield) _listChecked.Add("McShield");

                if (_listSubItems[intCount].strCommand.ToLower() == "control vjoy")
                    if (system.boolControlVJOY) _listChecked.Add("Control VJOY");

                if (_listSubItems[intCount].strCommand.ToLower() == "crossbar")
                    if (checkSystemSetting("Crossbar")) _listChecked.Add("Crossbar");

                if (_listSubItems[intCount].strCommand.ToLower() == "avirender")
                    if (checkSystemSetting("AVIRender")) _listChecked.Add("AVI Renderer");

                if (_listSubItems[intCount].strCommand.ToLower() == "checkcaptureres")
                    if (checkSystemSetting("CheckCaptureRes")) _listChecked.Add("Check Capture");
            }
        }

        private void listCaptureResolution()
        {
            clearButtons();

            _intShutterScroll = 0;

            _listSubItems.Clear();
            _listChecked.Clear();
            _strSubError = "";
            _strSubExplain = "";
            _strCurrentMenu = "resolution";

            List<string> listAdded = new List<string>();

            List<string> listVideoRes = videoCapture.getVideoResolution();
            int currentResolution = videoCapture.getVideoResolutionCurrent();

            for (int count = 0; count < listVideoRes.Count; count++)
            {
                string videoRes = listVideoRes[count];
                if (videoRes.IndexOf("[") > -1)
                    videoRes = videoRes.Substring(0, videoRes.IndexOf("[")).Trim();

                if (listAdded.IndexOf(videoRes) == -1)
                {
                    listAdded.Add(videoRes);

                    if (count == currentResolution)
                    {
                        system.strCurrentResolution = listVideoRes[count];
                        addSubItem(listVideoRes[count], "*" + listVideoRes[count], true);
                    }
                    else
                        addSubItem(listVideoRes[count], listVideoRes[count]);
                }
            }

            selectSubItem();
        }

        private void changeResolution(string resolution)
        {
            system.strCurrentResolution = resolution;
            resolution = resolution.ToLower();
            if (resolution != "resolution")
            {
                List<string> listRes = videoCapture.getVideoResolution();
                for (int count = 0; count < listRes.Count; count++)
                {
                    if (resolution == listRes[count].ToLower())
                    {
                        videoCapture.setVideoResolution(count);
                        videoCapture.runGraph();

                        system.addUserData("CaptureResolution", resolution);
                        
                        break;
                    }
                }
            }
        }

        private void changeVideoDisplay(string command)
        {
            command = command.ToLower();
            if (command == "autoset") changeAutoRes();
            if (command == "resolution") listDisplayResolution();
            if (command == "refresh") listDisplayRefresh();
            if (command == "stayontop") changeStayOnTop();
        }

        private void listDisplayRefresh()
        {
            clearButtons();

            _intShutterScroll = 0;

            _listSubItems.Clear();
            _listChecked.Clear();
            _strSubError = "";
            _strSubExplain = "";
            _strCurrentMenu = "videorefresh";

            List<string> listDisplayRef = system.getDisplayRefresh();
            string currentRef = system.getRefreshRate().ToLower();

            for (int count = 0; count < listDisplayRef.Count; count++)
            {
                string title = listDisplayRef[count];
                if (title.ToLower() == currentRef)
                    addSubItem(title, title, true);
                else
                    addSubItem(title, title);
            }

            selectSubItem();
        }

        private void listDisplayResolution()
        {
            clearButtons();

            _intShutterScroll = 0;

            _listSubItems.Clear();
            _listChecked.Clear();
            _strSubError = "";
            _strSubExplain = "";
            _strCurrentMenu = "videoresolution";

            List<string> listDisplayRes = system.getDisplayResolutionList();
            string currentRes = system.getResolution().ToLower();

            for (int count = 0; count < listDisplayRes.Count; count++)
            {
                string title = listDisplayRes[count];
                if (title.ToLower() == currentRes)
                    addSubItem(title, title, true);
                else
                    addSubItem(title, title);
            }

            selectSubItem();
            //Set scroll position
            //Set most used to front of list
        }

        private void changeVideoResolution(string command)
        {
            if (command.ToLower() != "resolution")
            {
                system.setDisplayResolution(command);
                _listChecked.Clear();
                _listChecked.Add(command);

                //save set res
                this.Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (consoleXstream.Properties.Resources.imgMainMenu.Width / 2);
                this.Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (consoleXstream.Properties.Resources.imgMainMenu.Height / 2);
            }
        }

        private void changeVideoRefresh(string command)
        {
            if (command.ToLower() != "refresh")
            {
                system.setDisplayRefresh(command);
                _listChecked.Clear();
                _listChecked.Add(command);

                this.Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (consoleXstream.Properties.Resources.imgMainMenu.Width / 2);
                this.Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (consoleXstream.Properties.Resources.imgMainMenu.Height / 2);
            }
        }

        private void changeAutoRes()
        {
            if (_listChecked.IndexOf("Auto Set") > -1)
                _listChecked.RemoveAt(_listChecked.IndexOf("Auto Set"));
            else
                _listChecked.Add("Auto Set");

            system.setAutoChangeDisplay();
        }

        private void changeStayOnTop()
        {
            if (_listChecked.IndexOf("Stay On Top") > -1)
                _listChecked.RemoveAt(_listChecked.IndexOf("Stay On Top"));
            else
                _listChecked.Add("Stay On Top");

            system.setStayOnTop();
        }
        #endregion
        #region change Remap Screens
        private void changeRemapScreen(string command)
        {
            command = command.ToLower();
            if (command == "gamepad") setupGamepadRemap();
        }

        private void setupGamepadRemap()
        {
            clearButtons();
            _boolSetup = true;
            _boolSetupGamepad = true;
            _strSubSelected = "";
        }
        #endregion
        #region Draw Gamepad Remap
        private int _gamepadCount;
        //private int _remapCount;

        private void drawGamepadRemap()
        {
            if (_gamepadCount == 0)
                _gamepadCount = Enum.GetNames(typeof(xbox)).Length;

            drawButtonRemap(new Rectangle(10, 10, 100, 20), "Save Profile", false);
            //drawGUI.drawText(10, 10, "Save Profile");
            //drawGUI.drawText(100, 10, "Load Profile");
            //drawGUI.drawText(200, 10, "Reset to Default");
            //drawGUI.drawText(300, 10, "Wizard");

            int start = 50;

            for (int count = 0; count < _gamepadCount;count++)
            {
                bool IsSelected = false;
                string title = findGamepadValue(count);
                string set = findRemapValue(title);
                if (_strSubSelected == "") _strSelected = set;

                if (set.ToLower() == _strSubSelected.ToLower()) IsSelected = true;
                drawGUI.drawText(400, 50, _strSubSelected);
                drawGUI.drawText(10, start, title);
                drawButtonRemap(new Rectangle(120, start - 1, 75, 15), set, IsSelected);
                //drawGUI.drawText(125, start, findRemapValue(title));
                start += 15;
            }
        }

        private void drawButtonRemap(Rectangle rect, string write, bool IsHigh)
        {
            createButton(rect, write);

            //if (IsHigh)
                drawGUI.drawImage(rect.X, rect.Y, rect.Width, rect.Height, consoleXstream.Properties.Resources.imgTileLow);

            drawGUI.drawText(rect.X + 10, rect.Y + 1, write);
            //drawGUI.centerText(rect, write);
            /*
            if (IsHigh)
                drawGUI.drawImage(rect.X, rect.Y, rect.Width, rect.Height, consoleXstream.Properties.Resources.imgSubGlow);
            else
                drawGUI.drawImage(rect.X, rect.Y, rect.Width, rect.Height, consoleXstream.Properties.Resources.imgTileLow);
            */
        }

        private string findRemapValue(string title)
        {
            int find = remap.findRemapName(title);
            return findGamepadValue(find);
        }

        private string findGamepadValue(int value)
        {
            xbox xboxValue = (xbox)value;
            return GetEnumDescription(xboxValue);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                    return attributes[0].Description;
                else
                    return value.ToString();
            }

            return value.ToString();
        }

        #endregion
    }
}
