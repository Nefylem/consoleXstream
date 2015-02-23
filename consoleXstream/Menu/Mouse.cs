using System;
using System.Linq;
using System.Windows.Forms;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu
{
    class Mouse
    {
        private Interaction _data;
        private Navigation _nav;
        private SubMenu.Shutter _subMenu;
        private User _user;
        private Variables _var;

        public string Hover;
        public bool Enable { get; private set; }

        private int _menuHeight;
        private int _menuWidth;

        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetMenuSize(int width, int height) { _menuHeight = height; _menuWidth = width; }
        public void GetNavHandle(Navigation nav) { _nav = nav; }
        public void GetSubMenuHandle(SubMenu.Shutter subMenu) { _subMenu = subMenu; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVariableHandle(Variables var) { _var = var; }

        public void MouseMove(MouseEventArgs e)
        {
            var mouseX = e.Location.X;
            var mouseY = e.Location.Y;

            if (Enable)
                _nav.FindNewLocation(mouseX, mouseY, true);

        }

        public void Click(EventArgs e)
        {
            //TODO: Check if mouse is inside display box, otherwise jump out of menu
            if (!Enable) return;

            var intMouseX = Cursor.Position.X - _menuWidth;
            var intMouseY = Cursor.Position.Y - _menuHeight;

            if (_var.IsMainMenu)
            {
                _nav.MenuOk();
                return;
            }

            if (_data.Buttons
                .Where(t => t.Command == _user.SubSelected)
                .Where(t => intMouseX >= t.Rect.Left && intMouseX <= t.Rect.Right)
                .Any(t => intMouseY >= t.Rect.Top && intMouseY <= t.Rect.Bottom))
            {
                _nav.MenuOk();
                return;
            }

            if (intMouseY >= _subMenu.Start && intMouseY <= _subMenu.Height) return;

            _subMenu.Hide = true;

            foreach (var t in _data.InactiveButtons)
            {
                if (intMouseY < t.Rect.Top || intMouseY > t.Rect.Bottom) continue;
                if (intMouseX >= t.Rect.Left && intMouseX <= t.Rect.Right)
                    _user.Selected = t.Command;
            }
        }

        public void Set(bool set) { Enable = set; }
    }
}
