using System.Drawing;
using System.Windows.Forms;

namespace consoleXstream.Menu.SubSelectMenu
{
    public class Menu
    {
        public Menu(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private int _moveLeftWait;
        private int _moveRightWait;
        private int _menuOkWait;

        public void Draw()
        {
            CheckDelays();

            var bmpMenu = new Bitmap(Properties.Resources.imgSubMenu.Width, Properties.Resources.imgSubMenu.Height);
            var bmpSerial = new Bitmap(Properties.Resources.imgSubMenu.Width, Properties.Resources.imgSubMenu.Height);

            _class.DrawGui.drawImage(bmpMenu, 0, 0, Properties.Resources.imgSubMenu);
            _class.DrawGui.drawImage(bmpSerial, 0, 0, Properties.Resources.imgSubMenu);

            var intX = 0;
            if (_class.SubSelectVar.ListData.Count < 4)
            {
                var intSetWidth = _class.SubSelectVar.ListData.Count * (_class.Var.CellWidth + 5);
                intX = ((Properties.Resources.imgSubMenu.Width - 20) / 2) - (intSetWidth / 2);
            }

            for (int count = 0; count < _class.SubSelectVar.ListData.Count; count++)
            {
                var displayRect = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 4);                  //Outline
                var displayRectText = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 34);             //Text
                var logoRect = new Rectangle(intX + 25, 17, _class.Var.CellWidth - 50, _class.Var.CellHeight - 38);                  //Outline

                _class.DrawGui.drawImage(bmpMenu, logoRect, Properties.Resources.imgTitanOneLogo);

                if (count == _class.SubSelectVar.Selected)
                    _class.DrawGui.drawImage(bmpMenu, displayRect, Properties.Resources.imgSubGlow);

                var displayRectTextOption = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 60);
                _class.DrawGui.centerText(bmpMenu, displayRectText, "Unnamed");
                _class.DrawGui.centerText(bmpMenu, displayRectTextOption, "TitanOne");

                if (_class.SubSelectVar.Selected < _class.SubSelectVar.ListData.Count)
                    if (_class.SubSelectVar.ListData[count] == _class.SubSelectVar.TitanSerial)
                        _class.DrawGui.drawImage(bmpMenu, intX + _class.Var.CellWidth - 40, 17, 25, 25, Properties.Resources.imgTick);

                intX += _class.Var.CellWidth + 5;
            }

            if (_class.SubSelectVar.Selected < _class.SubSelectVar.ListData.Count)
                _class.DrawGui.centerText(bmpSerial, new Rectangle(0, bmpSerial.Height - 25, bmpSerial.Width, 20), "SN# " + _class.SubSelectVar.ListData[_class.SubSelectVar.Selected]);

            _class.DrawGui.drawImage(new Rectangle(8, 275, Properties.Resources.imgSubMenu.Width + 6, Properties.Resources.imgSubMenu.Height), bmpSerial);
            _class.DrawGui.drawImage(new Rectangle(8, 250, Properties.Resources.imgSubMenu.Width + 6, Properties.Resources.imgSubMenu.Height), bmpMenu);
        }

        public void GetCommand(string command)
        {
            if (command == "left" && _moveRightWait == 0) MenuLeft();
            if (command == "right" && _moveRightWait == 0) MenuRight();
            if (command == "ok" && _menuOkWait == 0) MenuOk();
        }

        public void MenuLeft()
        {
            _moveLeftWait = SetMoveWait();
            if (_class.SubSelectVar.Selected > 0)
                _class.SubSelectVar.Selected--;
        }

        public void MenuRight()
        {
            _moveRightWait = SetMoveWait();
            if (_class.SubSelectVar.Selected < _class.SubSelectVar.ListData.Count - 1)
                _class.SubSelectVar.Selected++;
        }

        public void MenuOk()
        {
            _menuOkWait = SetMoveWait();

            if (_class.SubSelectVar.Selected < _class.SubSelectVar.ListData.Count)
            {
                _class.SubSelectVar.TitanSerial = _class.SubSelectVar.ListData[_class.SubSelectVar.Selected];
                _class.Form1.SetTitanOne(_class.SubSelectVar.ListData[_class.SubSelectVar.Selected]);
            }
        }

        public int SetMoveWait()
        {
            if (_class.Fps.Frames > 20)
                return _class.Fps.Frames / 6;
            else
                return 3;
        }

        public void CheckDelays()
        {
            if (_moveRightWait > 0) _moveRightWait--;
            if (_moveLeftWait > 0) _moveLeftWait--;
            if (_menuOkWait > 0) _menuOkWait--;
        }
    }
}
     