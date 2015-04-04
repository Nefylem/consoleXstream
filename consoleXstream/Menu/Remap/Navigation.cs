using System.Windows.Forms;

namespace consoleXstream.Menu.Remap
{
    public class Navigation
    {
        public Navigation(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public string Selected;
        public string ShowCommand;
        private string SetMap;

        public int CellHeight = 20;
        public int CellSpace = 5;
        public int CellWidth = 450;

        private int _moveUpWait;
        private int _moveDownWait;
        private int _moveLeftWait;
        private int _moveRightWait;
        public int _moveOkWait;
        private int _menuBackWait;

        public bool WaitInput = false;

        public void GetCommand(string command)
        {
            if (WaitInput) return;

            if (command == "back") CloseRemapMenu();
            if (command == "down" && _moveDownWait == 0) MenuDown();
            if (command == "up" && _moveUpWait == 0) MenuUp();
            if (command == "right" && _moveRightWait == 0) MenuRight();
            if (command == "left" && _moveLeftWait == 0) MenuLeft();
            if (command == "ok" && _moveOkWait == 0) MenuOk();
        }

        private void CloseRemapMenu()
        {
            _class.Action.Remap("remap", 2);
            _class.Nav.SetBackWait(1);
            _class.Var.Setup = false;
            _class.Var.SetupGamepad = false;
        }

        public void MenuLeft()
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != Selected) continue;
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(150, intY))
                    _moveLeftWait = SetMoveWait();
                break;
            }
        }

        public void MenuRight()
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != Selected) continue;
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(470, intY))
                    _moveRightWait = SetMoveWait();
                break;
            }
        }

        private void MenuDown()
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != Selected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(intX, intY + CellHeight))
                    _moveDownWait = SetMoveWait();
                break;
            }
        }

        private void MenuUp()
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != Selected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);

                if (FindNewLocation(intX, intY - CellHeight))
                    _moveUpWait = SetMoveWait();
                break;
            }
        }

        public bool FindNewLocation(int intX, int intY)
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (intY < t.Rect.Top || intY > t.Rect.Bottom) continue;
                if (intX < t.Rect.Left || intX > t.Rect.Right) continue;

                if (_class.System.CheckLog("Menu")) _class.System.Debug("menu.log", ">Move>" + t.Command);

                Selected = t.Command;
                return true;
            }
            return false;
        }

        private void MenuOk()
        {
            _moveOkWait = 5;
            ShowCommand = "Press new button for " + Selected;
            SetMap = Selected;
            WaitInput = true;
        }

        public int SetMoveWait()
        {
            if (_class.Fps.Frames > 20)
                return _class.Fps.Frames / 12;
            
            return 1;
        }

        public void CheckDelays()
        {
            if (_moveRightWait > 0) _moveRightWait--;
            if (_moveLeftWait > 0) _moveLeftWait--;
            if (_moveUpWait > 0) _moveUpWait--;
            if (_moveDownWait > 0) _moveDownWait--;
            if (_moveOkWait > 0) _moveOkWait--;
            if (_menuBackWait > 0) _menuBackWait--;
        }

    }
}
