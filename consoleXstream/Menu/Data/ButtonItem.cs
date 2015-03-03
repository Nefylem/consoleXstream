using System;
using System.Drawing;
using System.Linq;

namespace consoleXstream.Menu.Data
{
    public class ButtonItem
    {
        private readonly Classes _class;

        public ButtonItem(Classes inClass) { _class = inClass; }

        public string Command;
        public Rectangle Rect;

        public void Create(Rectangle rect, string strCommand)
        {
            if (_class.Data.Buttons.Any(t => String.Equals(t.Command, strCommand, StringComparison.CurrentCultureIgnoreCase)))
                return;

            _class.Data.Buttons.Add(new ButtonItem(_class));
            var intIndex = _class.Data.Buttons.Count - 1;

            _class.Data.Buttons[intIndex].Command = strCommand;
            _class.Data.Buttons[intIndex].Rect = rect;
        }

        public void Create(Rectangle rect, string strCommand, bool boolInactive)
        {
            foreach (var t in _class.Data.InactiveButtons)
            {
                if (String.Equals(t.Command, strCommand, StringComparison.CurrentCultureIgnoreCase))
                    return;
            }

            _class.Data.InactiveButtons.Add(new ButtonItem(_class));
            var intIndex = _class.Data.InactiveButtons.Count - 1;

            _class.Data.InactiveButtons[intIndex].Command = strCommand;
            _class.Data.InactiveButtons[intIndex].Rect = rect;
        }
    }
}
