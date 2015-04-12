using System.Drawing;
using consoleXstream.Input;
using consoleXstream.Properties;

namespace consoleXstream.Menu.VR
{
    public class HeadTracking
    {
        public HeadTracking(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void ShowMovementSubmenu()
        {
            _class.Data.ClearButtons();

            _class.Shutter.Scroll = 0;

            _class.Data.SubItems.Clear();
            _class.Data.Checked.Clear();
            _class.Shutter.Error = "";
            _class.Shutter.Explain = "";
            _class.User.Menu = "headtracking";

            //_class.SubAction.AddSubItem("Mouse", "Mouse", true);
            //_class.SubAction.AddSubItem("Accelerometer", "Accelerometer", true);
            _class.SubAction.AddSubItem("Modifier", "Modifier");

            if (_class.Data.SubItems.Count > 0)
                _class.User.SubSelected = _class.Data.SubItems[0].Command;
        }

        public void SetOption(string command)
        {
            command = command.ToLower();
            if (command == "modifier")
            {
                _class.Nav.SetOkWait(6);
                _class.Base.System.IsCalibrateHeadMotion = true;
                _class.Var.ShowTrackingInstructions = true;
            }
        }

        public void DrawInstructionGuide()
        {
            if (_class.Shutter.Height == 0)
            {
                _class.Var.ShowTrackingInstructions = false;
                return;                
            }
            var bmpExplain = new Bitmap(Resources.imgSubMenu.Width, _class.Shutter.Height);

            _class.DrawGui.drawImage(bmpExplain, 0, 0, Resources.imgSubMenu);

            _class.DrawGui.setOutline(true);
            _class.DrawGui.setFontSize(25);
            _class.DrawGui.CenterText(bmpExplain, new Rectangle(0, 0, 600, 40), _class.Base.System.MouseModifierX + " /" + _class.Base.System.MouseModifierY);
            _class.DrawGui.setOutline(false);
            _class.DrawGui.setFontSize(12);
            _class.DrawGui.CenterText(bmpExplain, new Rectangle(0, 40, 600, 10), "DPad up = increase vertical modifier");
            _class.DrawGui.CenterText(bmpExplain, new Rectangle(0, 55, 600, 10), "DPad down = decrease vertical modifier");
            _class.DrawGui.CenterText(bmpExplain, new Rectangle(0, 70, 600, 10), "DPad right = increase horizontal modifier");
            _class.DrawGui.CenterText(bmpExplain, new Rectangle(0, 85, 600, 10), "DPad left = decrease horizontal modifier");
            _class.DrawGui.CenterText(bmpExplain, new Rectangle(0, 100, 600, 10), "(A) to close");
            _class.DrawGui.setFontSize(12);

            _class.DrawGui.DrawImage(new Rectangle(8, 250, 581, _class.Shutter.Height), bmpExplain);
        }

        public void CheckControls()
        {
            var controls = GamePad.GetState(PlayerIndex.One);
            if (controls.DPad.Up) _class.Base.System.MouseModifierY++;
            if (controls.DPad.Down) _class.Base.System.MouseModifierY--;
            if (controls.DPad.Left) _class.Base.System.MouseModifierX--;
            if (controls.DPad.Right) _class.Base.System.MouseModifierX++;
            if (controls.Buttons.A) CloseRecalibrate();

            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadUp) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadUp)) _class.Base.System.MouseModifierY++;
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadDown) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadDown)) _class.Base.System.MouseModifierY--;
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadLeft) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadLeft)) _class.Base.System.MouseModifierX--;
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.DpadRight) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.DpadRight)) _class.Base.System.MouseModifierX++;
            if (_class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyDef.ButtonA) || _class.Base.Keyboard.GetKey(_class.Base.Keymap.KeyAltDef.ButtonA)) CloseRecalibrate();            
        }

        public void CloseRecalibrate()
        {
            if (!_class.Nav.CheckOk()) return;
            _class.Base.System.Set("VR_Modifier_X", _class.Base.System.MouseModifierX.ToString());
            _class.Base.System.Set("VR_Modifier_Y", _class.Base.System.MouseModifierY.ToString());
            _class.Nav.SetOkWait(6);
            _class.Var.ShowTrackingInstructions = false;
            _class.Base.System.IsCalibrateHeadMotion = false;
        }
    }
}
