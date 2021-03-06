﻿using System;
using System.ComponentModel;
using System.Drawing;
using consoleXstream.Input;

namespace consoleXstream.Menu.Remap
{
    public class Gamepad
    {
        public Gamepad(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private int _gamepadCount;

        public void Setup()
        {
            _class.Data.ClearButtons();
            _class.Var.Setup = true;
            _class.Var.SetupGamepad = true;
            _class.RemapNav.Selected = "";
            _class.RemapNav.ShowCommand = null;
        }

        private string FindRemapValue(string title)
        {
            var gamepadCode = _class.Base.Remap.FindRemapName(title);
            return gamepadCode == -1 ? "Undefined" : FindGamepadValue(gamepadCode);
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

        //Each remap has its own draw so it can display the input properly
        public void Draw()
        {
            if (_class.RemapNav.WaitInput) CheckGamepad();

            if (_gamepadCount == 0) _gamepadCount = Enum.GetNames(typeof(Xbox)).Length;

            //DrawButton(new Rectangle(10, 20, 275, 20), "Save Profile", "Save Profile", "Save Profile" == _class.RemapNav.Selected);
            //DrawButton(new Rectangle(295, 20, 275, 20), "Load Profile", "Load Profile", "Load Profile" == _class.RemapNav.Selected);
            _class.DrawGui.setOutline(true);
            _class.DrawGui.setFontSize(25);
            _class.DrawGui.CenterText(new Rectangle(0, 0, 600, 40), "Remap Gamepad Inputs");
            _class.DrawGui.setOutline(false);
            _class.DrawGui.setFontSize(12);

            var start = 50;
            var leftTitle = 10;
            var leftButton = 120;
            for (var count = 0; count < _gamepadCount; count++)
            {
                var isSelected = false;
                var title = FindGamepadValue(count);
                var set = FindRemapValue(title);

                if (_class.RemapNav.Selected == "") _class.RemapNav.Selected = set;

                if (String.Equals(title, _class.RemapNav.Selected, StringComparison.CurrentCultureIgnoreCase))
                    isSelected = true;

                _class.DrawGui.drawText(leftTitle, start, title);

                DrawButton(new Rectangle(leftButton, start - 1, 125, _class.RemapNav.CellHeight), title, set, isSelected);
                start += _class.RemapNav.CellHeight + _class.RemapNav.CellSpace;

                if (start < 475) continue;
                leftTitle = 300;
                leftButton = 410;
                start = 50;
            }

            if (_class.RemapNav.ShowCommand != null)
                _class.DrawGui.CenterText(new Rectangle(0, 480, 500, 15), _class.RemapNav.ShowCommand);
        }

        private void DrawButton(Rectangle rect, string title, string write, bool isHigh)
        {
            _class.Button.Create(rect, title);
            
            if (isHigh) _class.DrawGui.SetPen(Color.White, 3); else _class.DrawGui.SetPen(Color.Black, 2);

            _class.DrawGui.DrawRectangle(rect);

            _class.DrawGui.CenterText(rect, write);
        }

        private void CheckGamepad()
        {
            var controls = GamePad.GetState(PlayerIndex.One);

            if (controls.DPad.Left) { SetGamepadButton("D-Pad Left"); }
            if (controls.DPad.Right) { SetGamepadButton("D-Pad Right"); }
            if (controls.DPad.Up) { SetGamepadButton("D-Pad Up"); }
            if (controls.DPad.Down) { SetGamepadButton("D-Pad Down"); }

            if (controls.Buttons.A && _class.RemapNav._moveOkWait == 0) { SetGamepadButton("A"); }
            if (controls.Buttons.B) { SetGamepadButton("B"); }
            if (controls.Buttons.X) { SetGamepadButton("X"); }
            if (controls.Buttons.Y) { SetGamepadButton("Y"); }

            if (controls.Buttons.Start) { SetGamepadButton("Start"); }
            if (controls.Buttons.Guide) { SetGamepadButton("Guide"); }
            if (controls.Buttons.Back) { SetGamepadButton("Back"); }

            if (controls.Buttons.LeftShoulder) { SetGamepadButton("LeftShoulder"); }
            if (controls.Buttons.RightShoulder) { SetGamepadButton("RightShoulder"); }
            if (controls.Buttons.LeftStick) { SetGamepadButton("LeftStick"); }
            if (controls.Buttons.RightStick) { SetGamepadButton("RightStick"); }

            if (controls.Triggers.Left > 0) { }
            if (controls.Triggers.Right > 0) { }
            //LX
            //LY
            //RX
            //RY
        }

        private void SetGamepadButton(string button)
        {
            _class.Base.Remap.SetRemapCode(_class.RemapNav.SetMap, button);
            _class.RemapNav.ShowCommand = "";
            _class.RemapNav.SetMap = "";
            _class.RemapNav.WaitInput = false;
        }
    }
}
