using System;
using System.Windows.Forms;

namespace consoleXstream.Input.Mouse
{
    class Hook
    {

        private Output.Gamepad _gamepad;
        private Form1 frmMain;
        private Menu.ShowMenu formMenu;
        private Config.Configuration system;
        //private Output.ControllerMax controllerMax;
        //private Output.TitanOne.Write titanOne;
        private Input.KeyboardInterface keyboard;

        public Hook(Form1 mainForm, Output.Gamepad game) { frmMain = mainForm; _gamepad = game; }

        public void enableMouseHook()
        {
            MouseDownFilter mouseFilter = new MouseDownFilter(frmMain);
            mouseFilter.LeftClick += mouseFilter_leftClick;
            mouseFilter.LeftRelease += mouseFilter_leftRelease;
            mouseFilter.mouseMoved += mouseFilter_mouseMovement;
            mouseFilter.RightClick += mouseFilter_rightClick;
            mouseFilter.RightRelease += mouseFilter_rightRelease;

            Application.AddMessageFilter(mouseFilter);
        }

        public void GetSystemHandle(Config.Configuration inSystem) { system = inSystem; }
        //public void GetControllerMaxHandle(Output.ControllerMax inMax) { controllerMax = inMax; }
        //public void GetTitanOneHandle(Output.TitanOne.Write inTo) { titanOne = inTo; }
        public void GetKeyboardInterfaceHandle(Input.KeyboardInterface inKey) { keyboard = inKey; }
        public void GetMenuHandle(Menu.ShowMenu inMenu) { formMenu = inMenu; }
            
        void mouseFilter_leftClick(object sender, EventArgs e)
        {
            if (!system.boolMenu)
            {
                if (system.boolEnableMouse)
                    keyboard.boolLeftMouse = true;
                else
                    _gamepad.Ps4Touchpad = true;
            }
            else
            {
                formMenu.BringToFront();
                formMenu.Focus();
                /*
                if (formMenu.IsShutterOpen())
                    formMenu.CloseShutter();
                else
                    formMenu.ClosePanel();
                 */
            }
        }

        void mouseFilter_leftRelease(object sender, EventArgs e)
        {
            if (!system.boolMenu)
            {
                if (system.boolEnableMouse)
                    keyboard.boolLeftMouse = false;
                else
                    _gamepad.Ps4Touchpad = false;
            }
        }

        void mouseFilter_rightClick(object sender, EventArgs e)
        {
            if (!system.boolMenu)
            {
                if (system.boolEnableMouse)
                    keyboard.boolRightMouse = true;
            }
        }

        void mouseFilter_rightRelease(object sender, EventArgs e)
        {
            if (!system.boolMenu)
            {
                if (system.boolEnableMouse)
                    keyboard.boolRightMouse = false;
            }
        }

        void mouseFilter_mouseMovement(object sender, EventArgs e)
        {
            //Not in use. Main form uses mouse over instead
        }
    }

    class MouseDownFilter : IMessageFilter
    {
        public event EventHandler LeftClick;
        public event EventHandler LeftRelease;
        public event EventHandler mouseMoved;
        public event EventHandler RightClick;
        public event EventHandler RightRelease;
        private int
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205;

        private Form form = null;

        public MouseDownFilter(Form f)
        {
            form = f;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                if (Form.ActiveForm != null && Form.ActiveForm.Equals(form))
                    OnLeftClick();
            }

            if (m.Msg == WM_LBUTTONUP)
            {
                if (Form.ActiveForm != null && Form.ActiveForm.Equals(form))
                    OnLeftRelease();
            }

            if (m.Msg == WM_RBUTTONDOWN)
            {
                if (Form.ActiveForm != null && Form.ActiveForm.Equals(form))
                    OnRightClick();
            }

            if (m.Msg == WM_RBUTTONUP)
            {
                if (Form.ActiveForm != null && Form.ActiveForm.Equals(form))
                    OnRightRelease();
            }

            return false;
        }

        protected void OnLeftClick()
        {
            if (LeftClick != null)
                LeftClick(form, EventArgs.Empty);
        }

        protected void OnLeftRelease()
        {
            if (LeftClick != null)
                LeftRelease(form, EventArgs.Empty);
        }

        protected void OnRightClick()
        {
            if (RightClick != null)
                RightClick(form, EventArgs.Empty);
        }

        protected void OnRightRelease()
        {
            if (RightClick != null)
                RightRelease(form, EventArgs.Empty);
        }

    }
}
