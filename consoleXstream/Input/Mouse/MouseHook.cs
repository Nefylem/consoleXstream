using System;
using System.Windows.Forms;
using consoleXstream.Home;

namespace consoleXstream.Input.Mouse
{
    public class Hook
    {
        public Hook(BaseClass baseClass) { _class = baseClass; }
        private BaseClass _class;

        /*
        public Hook(Form1 mainForm, Output.Gamepad game) { frmMain = mainForm; _gamepad = game; }

        private Output.Gamepad _gamepad;
        private Form1 frmMain;
        private Menu.ShowMenu formMenu;
        private Config.Configuration system;
        private Input.KeyboardInterface keyboard;
        */

        public void enableMouseHook()
        {
            MouseDownFilter mouseFilter = new MouseDownFilter(_class.Home);
            mouseFilter.LeftClick += mouseFilter_leftClick;
            mouseFilter.LeftRelease += mouseFilter_leftRelease;
            mouseFilter.mouseMoved += mouseFilter_mouseMovement;
            mouseFilter.RightClick += mouseFilter_rightClick;
            mouseFilter.RightRelease += mouseFilter_rightRelease;

            Application.AddMessageFilter(mouseFilter);
        }
        /*
        public void GetSystemHandle(Config.Configuration inSystem) { system = inSystem; }
        public void GetKeyboardInterfaceHandle(Input.KeyboardInterface inKey) { keyboard = inKey; }
        public void GetMenuHandle(Menu.ShowMenu inMenu) { formMenu = inMenu; }
          */  
        void mouseFilter_leftClick(object sender, EventArgs e)
        {
            if (!_class.System.boolMenu)
            {
                if (!_class.System.IsEnableMouse || _class.System.IsVr)
                    _class.Gamepad.Ps4Touchpad = true;
                else
                    _class.KeyboardInterface.BoolLeftMouse = true;
            }
            else
            {
                _class.Menu.BringToFront();
                _class.Menu.Focus();                
            }
        }

        void mouseFilter_leftRelease(object sender, EventArgs e)
        {
            if (!_class.System.boolMenu)
            {
                if (_class.System.IsEnableMouse && !_class.System.IsVr)
                    _class.KeyboardInterface.BoolLeftMouse = false;
                else
                    _class.Gamepad.Ps4Touchpad = false;
            }
        }

        void mouseFilter_rightClick(object sender, EventArgs e)
        {
            if (!_class.System.boolMenu)
            {
                if (_class.System.IsEnableMouse)
                    _class.KeyboardInterface.BoolRightMouse = true;
            }
        }

        void mouseFilter_rightRelease(object sender, EventArgs e)
        {
            if (!_class.System.boolMenu)
            {
                if (_class.System.IsEnableMouse)
                    _class.KeyboardInterface.BoolRightMouse = false;
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
