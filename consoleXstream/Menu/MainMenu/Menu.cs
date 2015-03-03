using System;
using System.Drawing;
using consoleXstream.DrawGui;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.MainMenu
{
    public class Menu
    {
        private readonly Classes _class;

        public Menu(Classes inClass) { _class = inClass; }

        public void Draw()
        {
            var intX = 10 + _class.Var.MenuOffsetX;
            var intY = 10 + _class.Var.MenuOffsetY;

            _class.Var.CellHeight = Properties.Resources.imgTileLow.Height;
            _class.Var.CellWidth = Properties.Resources.imgTileLow.Width;

            _class.DrawGui.setFontSize(18f);
            _class.DrawGui.setOutline(true);
            _class.DrawGui.setCenterBottom(true);
            _class.Data.Row.Clear();

            foreach (var t in _class.Data.Items)
            {
                _class.Data.Row.Add(intY);

                foreach (var t1 in t)
                {
                    var displayRect = new Rectangle(intX, intY, _class.Var.CellWidth, _class.Var.CellHeight);

                    if (_class.Var.IsMainMenu)
                        _class.Button.Create(displayRect, t1.Command);
                    else
                        _class.Button.Create(displayRect, t1.Command, false);       //Add to the inactive list incase its needed - mainly by mouse move

                    if (_class.User.Selected == t1.Command)
                        _class.DrawGui.drawImage(intX - 6, intY - 7, 108, 115, Properties.Resources.imgSubGlow);
                    else
                        _class.DrawGui.drawImage(intX, intY, Properties.Resources.imgTileLow);

                    _class.DrawGui.centerText(displayRect, t1.Display);

                    intX += _class.Var.CellWidth + 5;
                }
                intY += _class.Var.CellHeight + 5;
                intX = 10;
            }
            _class.DrawGui.setOutline(true);
            _class.DrawGui.setFontSize(12f);
        }
    }
}
