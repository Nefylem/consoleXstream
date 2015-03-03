using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.Config;
using consoleXstream.Input;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenuOptions
{
    public class Remap
    {
        private Configuration _system;
        private Interaction _data;
        private User _user;
        private SubMenu.Action _subAction;
        private SubMenu.Shutter _shutter;
        private Variables _var;
        private VideoCapture.VideoCapture _videoCapture;

        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetSubActionHandle(SubMenu.Action subAction) { _subAction = subAction; }
        public void GetShutterHandle(SubMenu.Shutter shutter) { _shutter = shutter; }
        public void GetVariableHandle(Variables var) { _var = var; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture video) { _videoCapture = video; }

        public void ChangeRemapScreen(string command)
        {
            command = command.ToLower();
            if (command == "gamepad") SetupGamepadRemap();
        }

        private void SetupGamepadRemap()
        {
            _data.ClearButtons();
            _var.Setup = true;
            _var.SetupGamepad = true;
            _user.SubSelected = "";
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
            var xboxValue = (Xbox)value;
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
