using System;
using System.Drawing;
using consoleXstream.DrawGui;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.MainMenu
{
    class Menu
    {
        private ButtonItem _button;
        private Interaction _data;
        private DrawGraph _drawGui;
        private User _user;
        private Variables _var;

        public void GetButtonHandle(ButtonItem button) { _button = button; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetDrawGuiHandle(DrawGraph drawGui) { _drawGui = drawGui; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVariableHandle(Variables var) { _var = var; }

        public void Draw()
        {
            var intX = 10 + _var.MenuOffsetX;
            var intY = 10 + _var.MenuOffsetY;

            _var.CellHeight = Properties.Resources.imgTileLow.Height;
            _var.CellWidth = Properties.Resources.imgTileLow.Width;

            _drawGui.setFontSize(18f);
            _drawGui.setOutline(true);
            _drawGui.setCenterBottom(true);
            _data.Row.Clear();

            foreach (var t in _data.Items)
            {
                _data.Row.Add(intY);

                foreach (var t1 in t)
                {
                    var displayRect = new Rectangle(intX, intY, _var.CellWidth, _var.CellHeight);

                    if (_var.IsMainMenu)
                        _button.Create(displayRect, t1.Command);
                    else
                        _button.Create(displayRect, t1.Command, false);       //Add to the inactive list incase its needed - mainly by mouse move

                    if (_user.Selected == t1.Command)
                        _drawGui.drawImage(intX - 6, intY - 7, 108, 115, Properties.Resources.imgSubGlow);
                    else
                        _drawGui.drawImage(intX, intY, Properties.Resources.imgTileLow);

                    _drawGui.centerText(displayRect, t1.Display);

                    intX += _var.CellWidth + 5;
                }
                intY += _var.CellHeight + 5;
                intX = 10;
            }
            _drawGui.setOutline(true);
            _drawGui.setFontSize(12f);
        }
    }
}
