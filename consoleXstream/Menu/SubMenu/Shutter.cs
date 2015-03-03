using System;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenu
{
    public class Shutter
    {
        private readonly Classes _class;

        public Shutter(Classes inClass) { _class = inClass; }

        public int Height { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        private int _slide = 7;

        public bool Open;
        public bool Hide;

        public string Error;
        public string Explain;

        public int Scroll;

        public int FindScrollIndex()
        {
            for (var count = 0; count < _class.Data.Buttons.Count; count++)
            {
                if (String.Equals(_class.User.SubSelected, _class.Data.Buttons[count].Command, StringComparison.CurrentCultureIgnoreCase))
                    return count;
            }
            return -1;
        }

        public void SetActive(int targetRow)
        {
            if (targetRow < 0 || targetRow > _class.Data.Row.Count)
                return;

            Start = _class.Data.Row[targetRow];
            End = _class.Data.Row[targetRow] + _class.Var.CellHeight;

            Height = 0;
            if (_class.Fps.Frames > 20)
                _slide = _class.Fps.Frames / 3;
            else
                _slide = 10;

            Hide = false;
            Open = true;
        }

        public void CheckDisplay()
        {
            if (Open && !Hide)
            {
                if (Height < End - Start)
                {
                    Height += _slide;

                    if (Height > End - Start)
                        Height = End - Start;
                }
            }

            if (!Hide) return;
            if (Height > 0)
                Height -= _slide;

            if (Height > 0) return;

            Height = 0;
            Hide = false;
            Open = false;
            _class.Var.IsMainMenu = true;
            _class.Data.ClearButtons();
        }

        public bool IsOpen()
        {
            return !_class.Var.IsMainMenu;
        }

        public void Close()
        {
            Hide = true;
        }


    }
}
