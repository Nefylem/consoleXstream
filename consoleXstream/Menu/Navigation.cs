using System.Collections.Generic;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu
{
    class Navigation
    {
        private MainMenu.Action _act;
        private SubMenu.Action _subAct;
        private Variables _var;
        private ShowMenu _menu;
        private Interaction _data;
        private User _user;
        private FrameCount _fps;
        private Mouse _mouse;
        private SubMenu.Shutter _shutter;

        public List<string> ListHistory;

        private int _moveUpWait;
        private int _moveDownWait;
        private int _moveLeftWait;
        private int _moveRightWait;
        private int _moveOkWait;
        private int _menuBackWait;

        public void GetActionVariable(MainMenu.Action action) { _act = action; }
        public void GetSubActionVariable(SubMenu.Action action) { _subAct = action; }
        public void GetVariableHandle(Variables inVar) { _var = inVar; }
        public void GetMenuHandle(ShowMenu inMenu) { _menu = inMenu; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetFpsHandle(FrameCount fps) { _fps = fps; }
        public void GetMouseHandle(Mouse mouse) { _mouse = mouse; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }

        public void CheckCommand(string command)
        {
            command = command.ToLower();
            if (command == "up" && _moveUpWait == 0) MenuUp();
            if (command == "down" && _moveDownWait == 0) MenuDown();
            if (command == "left" && _moveLeftWait == 0) MenuLeft();
            if (command == "right" && _moveRightWait == 0) MenuRight();
            if (command == "ok") { if (_moveOkWait == 0) MenuOk(); else _moveOkWait = 5; }
            if (command == "back") { if (_menuBackWait == 0) MenuBack(); else _menuBackWait = 5; }
        }

        public void MenuUp()
        {
            foreach (var t in _data.Buttons)
            {
                if (t.Command != _user.Selected && t.Command != _user.SubSelected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(intX, intY - _var.CellHeight))
                    _moveUpWait = SetMoveWait();
                break;
            }
        }

        public void MenuDown()
        {
            foreach (var t in _data.Buttons)
            {
                if (t.Command != _user.Selected && t.Command != _user.SubSelected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(intX, intY + _var.CellHeight))
                    _moveDownWait = SetMoveWait();
                break;
            }
        }

        public void MenuLeft()
        {
            foreach (var t in _data.Buttons)
            {
                if (t.Command != _user.Selected && t.Command != _user.SubSelected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(intX - _var.CellWidth, intY))
                {
                    _moveLeftWait = SetMoveWait();
                    if (_shutter.Open)
                    {
                        var index = _shutter.FindScrollIndex();
                        if (index > -1)
                        {
                            if (index - _shutter.Scroll < 1)
                                _shutter.Scroll = index - 1;

                            if (_shutter.Scroll < 0)
                                _shutter.Scroll = 0;
                        }
                    }
                }
                break;
            }
        }

        public void MenuRight()
        {
            for (var intCount = _shutter.Scroll; intCount < _data.Buttons.Count; intCount++)
            {
                if (_data.Buttons[intCount].Command != _user.Selected &&
                    _data.Buttons[intCount].Command != _user.SubSelected) continue;
                var intX = _data.Buttons[intCount].Rect.Left + (_data.Buttons[intCount].Rect.Width / 2);
                var intY = _data.Buttons[intCount].Rect.Top + (_data.Buttons[intCount].Rect.Height / 2);

                if (FindNewLocation(intX + _var.CellWidth, intY))
                {
                    _moveRightWait = SetMoveWait();
                    if (_shutter.Open)
                    {
                        var index = _shutter.FindScrollIndex();
                        if (index > -1)
                        {
                            if (index > _shutter.Scroll + 2)
                            {
                                _shutter.Scroll = index - 2;
                                if (_shutter.Scroll + 4 > _data.Buttons.Count)
                                    _shutter.Scroll = _data.Buttons.Count - 4;
                            }
                        }
                    }
                }
                break;
            }
        }

        public void MenuBack()
        {
            if (_var.Setup)
            {
                _var.Setup = false;
                _var.SetupGamepad = false;
            }
            else
            {
                if (ListHistory.Count == 0)
                    _menu.ClosePanel();
                else
                {
                    if (_var.ShowSubSelection)
                    {
                        _var.ShowSubSelection = false;
                        return;
                    }
                    _menuBackWait = 5;
                    ListHistory.RemoveAt(ListHistory.Count - 1);
                    if (ListHistory.Count == 0)
                        _shutter.Hide = true;
                }
            }
        }

        public void MenuOk()
        {
            _moveOkWait = 5;
            if (_var.IsMainMenu)
                _act.MainMenu(_user.Selected);
            else
            {
                _subAct.ProcessSubMenu(_user.SubSelected);
            }
        }


        public bool FindNewLocation(int intX, int intY)
        {
            foreach (var t in _data.Buttons)
            {
                if (intY < t.Rect.Top || intY > t.Rect.Bottom) continue;
                if (intX < t.Rect.Left || intX > t.Rect.Right) continue;
                if (_mouse.Hover == t.Command) return true;
                if (_var.IsMainMenu)
                    _user.Selected = t.Command;
                else
                    _user.SubSelected = t.Command;
                return true;
            }
            return false;
        }

        //Sets motion if moved by mouse
        public bool FindNewLocation(int intX, int intY, bool boolMouseMove)
        {
            foreach (var t in _data.Buttons)
            {
                if (intY < t.Rect.Top || intY > t.Rect.Bottom) continue;
                if (intX < t.Rect.Left || intX > t.Rect.Right) continue;

                if (_mouse.Hover == t.Command) return true;
                _mouse.Hover = t.Command;

                if (_var.IsMainMenu)
                    _user.Selected = t.Command;
                else
                    _user.SubSelected = t.Command;
                return true;
            }
            return false;
        }


        public int SetMoveWait()
       {
            if (_fps.Frames > 20)
                return _fps.Frames/6;
            else
                return 3;
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

        public void SetBackWait(int wait) { _menuBackWait = wait; }
    }
}
