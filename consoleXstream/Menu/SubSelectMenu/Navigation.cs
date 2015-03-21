using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Menu.SubSelectMenu
{
    public class Navigation
    {
        public Navigation(Classes classes) { _class = classes; }
        private Classes _class;

        private int _moveLeftWait;
        private int _moveRightWait;
        private int _menuOkWait;
        private int _menuBackWait;

        public void GetCommand(string command)
        {
            if (command == "left" && _moveRightWait == 0) MenuLeft();
            if (command == "right" && _moveRightWait == 0) MenuRight();
            if (command == "ok") MenuOk();
            if (command == "back" && _menuBackWait == 0) CloseSubMenu();
        }

        public void CloseSubMenu()
        {
            _menuBackWait = 5;
            _class.Nav.SetBackWait(_menuBackWait);
            _class.Var.ShowSubSelection = false;
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
            if (_menuOkWait != 0)
            {
                _menuOkWait = SetMoveWait() * 3; return;
            }

            _menuOkWait = SetMoveWait();

            if (_class.SubSelectVar.Selected < _class.SubSelectVar.ListData.Count)
            {
                _class.SubSelectVar.TitanSerial = _class.SubSelectVar.ListData[_class.SubSelectVar.Selected];

                _class.Form1.SetTitanOne(_class.SubSelectVar.ListData[_class.SubSelectVar.Selected]);
                _class.System.TitanOneDevice = _class.SubSelectVar.ListData[_class.SubSelectVar.Selected];
                _class.System.boolControllerMax = false;
                _class.System.boolTitanOne = true;

                ChangeMenuSelections();
            }
        }

        private void ChangeMenuSelections()
        {
            _class.Data.Checked.Clear();
            _class.Data.Checked.Add("TitanOne");
        }

        public int SetMoveWait()
        {
            if (_class.Fps.Frames > 20)
                return _class.Fps.Frames / 6;
            else
                return 5;
        }

        public void CheckDelays()
        {
            if (_moveRightWait > 0) _moveRightWait--;
            if (_moveLeftWait > 0) _moveLeftWait--;
            if (_menuOkWait > 0) _menuOkWait--;
            if (_menuBackWait > 0) _menuBackWait--;
        }

        public void SetMenuOkWait()
        {
            if (_class.Fps.Frames > 20)
                _menuOkWait = _class.Fps.Frames / 6;
            else
                _menuOkWait = 5;
        }

        public void SetMenuOkWait(int wait)
        {
            _menuOkWait = wait;
        }

        public int showMenuWait()
        {
            return _menuBackWait;
        }

    }
}
