using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.Input;

namespace consoleXstream.Menu.SubMenuOptions
{
    class Remap
    {
        private void ChangeRemapScreen(string command)
        {
            command = command.ToLower();
            if (command == "gamepad") SetupGamepadRemap();
        }

        private void SetupGamepadRemap()
        {
            /*
            _data.ClearButtons();
            _var.Setup = true;
            _var.SetupGamepad = true;
            _user.SubSelected = "";
             */
        }

        private void DrawGamepadRemap()
        {
            /*
            if (_gamepadCount == 0)
                _gamepadCount = Enum.GetNames(typeof(xbox)).Length;

            DrawButtonRemap(new Rectangle(10, 10, 100, 20), "Save Profile", false);

            var start = 50;

            for (var count = 0; count < _gamepadCount; count++)
            {
                var isSelected = false;
                var title = FindGamepadValue(count);
                var set = FindRemapValue(title);
                if (_user.SubSelected == "") _user.Selected = set;

                if (String.Equals(set, _user.SubSelected, StringComparison.CurrentCultureIgnoreCase))
                    isSelected = true;

                _drawGui.drawText(400, 50, _user.SubSelected);
                _drawGui.drawText(10, start, title);
                DrawButtonRemap(new Rectangle(120, start - 1, 75, 15), set, isSelected);
                start += 15;
            }
             */
        }

        private void DrawButtonRemap(Rectangle rect, string write, bool isHigh)
        {
            /*
            _button.Create(rect, write);

            _drawGui.drawImage(rect.X, rect.Y, rect.Width, rect.Height,
                isHigh ? Properties.Resources.imgTileHigh : Properties.Resources.imgTileLow);

            _drawGui.drawText(rect.X + 10, rect.Y + 1, write);
             */
        }

        private string FindRemapValue(string title)
        {
            /*
            //var find = _remap.findRemapName(title);
            return FindGamepadValue(find);
             */
            return "";
        }

        private static string FindGamepadValue(int value)
        {
            var xboxValue = (xbox)value;
            return GetEnumDescription(xboxValue);
        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi == null)
                return value.ToString();

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

    }
}
