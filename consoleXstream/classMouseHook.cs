using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace consoleXstream
{
    class classMouseHook
    {
        private Form1 frmMain;
        private frmMenu formMenu;
        private classSystem system;
        private classControllerMax controllerMax;
        private classTitanOne titanOne;
        private classKeyboardInterface keyboard;
        public classMouseHook(Form1 mainForm) { frmMain = mainForm; }

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

        public void getSystemHandle(classSystem inSystem) { system = inSystem; }
        public void getControllerMaxHandle(classControllerMax inMax) { controllerMax = inMax; }
        public void getTitanOneHandle(classTitanOne inTO) { titanOne = inTO; }
        public void getKeyboardInterfaceHandle(classKeyboardInterface inKey) { keyboard = inKey; }
        public void getMenuHandle(frmMenu inMenu) { formMenu = inMenu; }
            
        void mouseFilter_leftClick(object sender, EventArgs e)
        {
            if (!system.boolMenu)
            {
                if (system.boolEnableMouse)
                    keyboard.boolLeftMouse = true;
                else
                {
                    if (system.useTitanOneAPI && system.boolControllerMax)
                        titanOne.Ps4Touchpad = true;

                    if (system.boolControllerMax && !system.useTitanOneAPI)
                        controllerMax.boolPs4Touchpad = true;

                    if (system.boolTitanOne)
                        titanOne.Ps4Touchpad = true;
                }
            }
            else
            {
                formMenu.BringToFront();
                formMenu.Focus();

                if (formMenu.isShutterOpen())
                    formMenu.closeShutter();
                else
                    formMenu.closePanel();
            }
        }

        void mouseFilter_leftRelease(object sender, EventArgs e)
        {
            if (!system.boolMenu)
            {
                if (system.boolEnableMouse)
                    keyboard.boolLeftMouse = false;
                else
                {
                    if (system.boolControllerMax && !system.useTitanOneAPI)
                        controllerMax.boolPs4Touchpad = false;

                    if (system.boolControllerMax && system.useTitanOneAPI)
                        titanOne.Ps4Touchpad = false;

                    if (system.boolTitanOne)
                        titanOne.Ps4Touchpad = false;
                }
                
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
