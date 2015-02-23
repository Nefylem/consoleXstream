using System;
using System.Drawing;
using System.Linq;

namespace consoleXstream.Menu.Data
{
    public class ButtonItem
    {
        private Interaction _data;

        public string Command;
        public Rectangle Rect;

        public void GetDataHandle(Interaction data) { _data = data; }

        public void Create(Rectangle rect, string strCommand)
        {
            if (_data.Buttons.Any(t => String.Equals(t.Command, strCommand, StringComparison.CurrentCultureIgnoreCase)))
                return;

            _data.Buttons.Add(new ButtonItem());
            var intIndex = _data.Buttons.Count - 1;

            _data.Buttons[intIndex].Command = strCommand;
            _data.Buttons[intIndex].Rect = rect;
        }

        public void Create(Rectangle rect, string strCommand, bool boolInactive)
        {
            foreach (var t in _data.InactiveButtons)
            {
                if (String.Equals(t.Command, strCommand, StringComparison.CurrentCultureIgnoreCase))
                    return;
            }

            _data.InactiveButtons.Add(new ButtonItem());
            var intIndex = _data.InactiveButtons.Count - 1;

            _data.InactiveButtons[intIndex].Command = strCommand;
            _data.InactiveButtons[intIndex].Rect = rect;
        }

    }
}
