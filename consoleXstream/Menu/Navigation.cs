using System.Collections.Generic;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu
{
    public class Navigation
    {
        private readonly Classes _class;

        public Navigation(Classes inClass) { _class = inClass; }

        public List<string> ListHistory;

        private int _moveUpWait;
        private int _moveDownWait;
        private int _moveLeftWait;
        private int _moveRightWait;
        private int _moveOkWait;
        private int _menuBackWait;

        public void CheckCommand(string command)
        {
            command = command.ToLower();
            if (_class.Var.ShowSubSelection) { _class.SubNav.GetCommand(command); return; }
            
            if (_class.System.CheckLog("Menu")) _class.System.Debug("menu.log", command);   
            
            if (command == "back") { if (_menuBackWait == 0) MenuBack(); else _menuBackWait = 5; }
            if (command == "up" && _moveUpWait == 0) MenuUp();
            if (command == "down" && _moveDownWait == 0) MenuDown();
            if (command == "left" && _moveLeftWait == 0) MenuLeft();
            if (command == "right" && _moveRightWait == 0) MenuRight();
            if (command == "ok") { if (_moveOkWait == 0) MenuOk(); else _moveOkWait = 5; }
        }

        private void MenuUp()
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != _class.User.Selected && t.Command != _class.User.SubSelected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(intX, intY - _class.Var.CellHeight))
                    _moveUpWait = SetMoveWait();
                break;
            }
        }

        public void MenuDown()
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != _class.User.Selected && t.Command != _class.User.SubSelected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(intX, intY + _class.Var.CellHeight))
                    _moveDownWait = SetMoveWait();
                break;
            }
        }

        public void MenuLeft()
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (t.Command != _class.User.Selected && t.Command != _class.User.SubSelected) continue;
                var intX = t.Rect.Left + (t.Rect.Width / 2);
                var intY = t.Rect.Top + (t.Rect.Height / 2);
                if (FindNewLocation(intX - _class.Var.CellWidth, intY))
                {
                    _moveLeftWait = SetMoveWait();
                    if (_class.Shutter.Open)
                    {
                        var index = _class.Shutter.FindScrollIndex();
                        if (index > -1)
                        {
                            if (index - _class.Shutter.Scroll < 1)
                                _class.Shutter.Scroll = index - 1;

                            if (_class.Shutter.Scroll < 0)
                                _class.Shutter.Scroll = 0;
                        }
                    }
                }
                break;
            }
        }

        public void MenuRight()
        {
            for (var intCount = _class.Shutter.Scroll; intCount < _class.Data.Buttons.Count; intCount++)
            {
                if (_class.Data.Buttons[intCount].Command != _class.User.Selected &&
                    _class.Data.Buttons[intCount].Command != _class.User.SubSelected) continue;
                var intX = _class.Data.Buttons[intCount].Rect.Left + (_class.Data.Buttons[intCount].Rect.Width / 2);
                var intY = _class.Data.Buttons[intCount].Rect.Top + (_class.Data.Buttons[intCount].Rect.Height / 2);

                if (FindNewLocation(intX + _class.Var.CellWidth, intY))
                {
                    _moveRightWait = SetMoveWait();
                    if (_class.Shutter.Open)
                    {
                        var index = _class.Shutter.FindScrollIndex();
                        if (index > -1)
                        {
                            if (index > _class.Shutter.Scroll + 2)
                            {
                                _class.Shutter.Scroll = index - 2;
                                if (_class.Shutter.Scroll + 4 > _class.Data.Buttons.Count)
                                    _class.Shutter.Scroll = _class.Data.Buttons.Count - 4;
                            }
                        }
                    }
                }
                break;
            }
        }

        public void MenuBack()
        {
            if (_class.Var.Setup)
            {
                _class.Var.Setup = false;
                _class.Var.SetupGamepad = false;
            }
            else
            {
                if (ListHistory.Count == 0)
                    _class.DisplayMenu.ClosePanel();
                else
                {
                    if (_class.Var.ShowSubSelection)
                    {
                        _class.Var.ShowSubSelection = false;
                        return;
                    }
                    _menuBackWait = 5;
                    ListHistory.RemoveAt(ListHistory.Count - 1);
                    if (ListHistory.Count == 0)
                        _class.Shutter.Hide = true;
                }
            }
        }

        public void MenuOk()
        {
            _moveOkWait = 2;
            if (_class.System.CheckLog("Menu")) _class.System.Debug("menu.log", ">OK>" + _class.User.Selected);
            if (_class.System.CheckLog("Menu")) _class.System.Debug("menu.log", ">SUBOK>" + _class.User.SubSelected);

            if (_class.Var.IsMainMenu)
                _class.Action.MainMenu(_class.User.Selected);
            else
            {
                _class.SubAction.ProcessSubMenu(_class.User.SubSelected);
            }
        }


        public bool FindNewLocation(int intX, int intY)
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (intY < t.Rect.Top || intY > t.Rect.Bottom) continue;
                if (intX < t.Rect.Left || intX > t.Rect.Right) continue;
                //if (_class.Mouse.Hover == t.Command) return true;

                if (_class.System.CheckLog("Menu")) _class.System.Debug("menu.log", ">Move>" + t.Command);

                if (_class.Var.IsMainMenu)
                    _class.User.Selected = t.Command;
                else
                    _class.User.SubSelected = t.Command;
                return true;
            }
            return false;
        }

        //Sets motion if moved by mouse
        public bool FindNewLocation(int intX, int intY, bool boolMouseMove)
        {
            foreach (var t in _class.Data.Buttons)
            {
                if (intY < t.Rect.Top || intY > t.Rect.Bottom) continue;
                if (intX < t.Rect.Left || intX > t.Rect.Right) continue;

                if (_class.Mouse.Hover == t.Command) return true;
                _class.Mouse.Hover = t.Command;

                if (_class.Var.IsMainMenu)
                    _class.User.Selected = t.Command;
                else
                    _class.User.SubSelected = t.Command;
                return true;
            }
            return false;
        }


        public int SetMoveWait()
       {
            if (_class.Fps.Frames > 20)
                return _class.Fps.Frames/6;
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
