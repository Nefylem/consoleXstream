using System.Windows.Forms;

namespace consoleXstream.Home
{
    public class MenuHandler
    {
        public MenuHandler(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Open()
        {
            var home = _class.BaseClass.Home;
            var menu = _class.BaseClass.Menu;
            var system = _class.BaseClass.System;
            var var = _class.Var;
            
            if (var.BlockMenuCount != 0) return;
            system.boolMenu = true;
            Cursor.Show();

            if (system.IsStayOnTop)
                home.TopMost = false;

            menu.ShowPanel();
        }

        public void Close()
        {
            var home = _class.BaseClass.Home;
            var system = _class.BaseClass.System;
            var var = _class.Var;

            if (system.IsStayOnTop)
                home.TopMost = true;

            system.boolMenu = false;

            if (system.IsHideMouse)
                Cursor.Hide();

            var.BlockMenuCount = 3;
        }

    }
}
