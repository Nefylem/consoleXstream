using System;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenu
{
    class Shutter
    {
        private Interaction _data;
        private FrameCount _fps;
        private User _user;
        private Variables _var;

        public int Height { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        private int _slide = 7;

        public bool Open;
        public bool Hide;

        public string Error;
        public string Explain;

        public int Scroll;

        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetFrameHandle(FrameCount fps) { _fps = fps; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetVarHandle(Variables var) { _var = var; }

        public int FindScrollIndex()
        {
            for (var count = 0; count < _data.Buttons.Count; count++)
            {
                if (String.Equals(_user.SubSelected, _data.Buttons[count].Command, StringComparison.CurrentCultureIgnoreCase))
                    return count;
            }
            return -1;
        }

        public void SetActive(int targetRow)
        {
            if (targetRow < 0 || targetRow > _data.Row.Count)
                return;

            Start = _data.Row[targetRow];
            End = _data.Row[targetRow] + _var.CellHeight;

            Height = 0;
            if (_fps.Frames > 20)
                _slide = _fps.Frames / 3;
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
            if (Height > 1)
                Height -= _slide;

            if (Height > 0) return;

            Height = 0;
            Hide = false;
            Open = false;
            _var.IsMainMenu = true;
            _data.ClearButtons();
        }

        public bool IsOpen()
        {
            return !_var.IsMainMenu;
        }

        public void Close()
        {
            Hide = true;
        }


    }
}
