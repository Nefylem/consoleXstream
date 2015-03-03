using System;
using System.Linq;
using System.Windows.Forms;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu
{
    public class Mouse
    {
        private readonly Classes _class;

        public Mouse(Classes inClass) { _class = inClass; }

        public string Hover;
        public bool Enable { get; private set; }

        private int _menuHeight;
        private int _menuWidth;

        public void MouseMove(MouseEventArgs e)
        {
            var mouseX = e.Location.X;
            var mouseY = e.Location.Y;

            if (Enable)
                _class.Nav.FindNewLocation(mouseX, mouseY, true);

        }

        public void Click(EventArgs e)
        {
            //TODO: Check if mouse is inside display box, otherwise jump out of menu
            if (!Enable) return;

            var intMouseX = Cursor.Position.X - _menuWidth;
            var intMouseY = Cursor.Position.Y - _menuHeight;

            if (_class.Var.IsMainMenu)
            {
                _class.Nav.MenuOk();
                return;
            }

            if (_class.Data.Buttons
                .Where(t => t.Command == _class.User.SubSelected)
                .Where(t => intMouseX >= t.Rect.Left && intMouseX <= t.Rect.Right)
                .Any(t => intMouseY >= t.Rect.Top && intMouseY <= t.Rect.Bottom))
            {
                _class.Nav.MenuOk();
                return;
            }

            if (intMouseY >= _class.Shutter.Start && intMouseY <= _class.Shutter.Height) return;

            _class.Shutter.Hide = true;

            foreach (var t in _class.Data.InactiveButtons)
            {
                if (intMouseY < t.Rect.Top || intMouseY > t.Rect.Bottom) continue;
                if (intMouseX >= t.Rect.Left && intMouseX <= t.Rect.Right)
                    _class.User.Selected = t.Command;
            }
        }

        public void Set(bool set) { Enable = set; }
    }
}
