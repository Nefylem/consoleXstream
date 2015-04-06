using System.Windows.Forms;

namespace consoleXstream.Menu.VR
{
    public class Resize
    {
        public Resize(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void SetResizeMode()
        {
            _class.Nav.SetOkWait(5);

            _class.Var.IsResizeVr = true;
            _class.Base.Menu.Visible = false;
        }

        private void CloseResizeMode()
        {
            if (_class.Nav._moveOkWait != 0) return;
            _class.Nav.SetOkWait(3);
            _class.Var.IsResizeVr = false;
            _class.Base.Menu.Visible = true;
        }

        public void GetCommand(string command)
        {
            command = command.ToLower();
            switch (command)
            {
                case "ok": CloseResizeMode(); return;
                case "up": DecreaseHeightOffset(); return;
                case "down": IncreaseHeightOffset(); return;
                case "left": IncreaseWidthOffset(); return;
                case "right": DecreaseWidthOffset(); return;
                case "back": ResetOffsets(); return;
            }
        }

        private void DecreaseHeightOffset()
        {
            if (_class.Base.System.Class.Vr.HeightOffset <= 0) return;

            _class.Base.System.Class.Vr.HeightOffset -= 5;
            _class.Base.System.Class.Set.Add("VR_Height", _class.Base.System.Class.Vr.HeightOffset.ToString());

            _class.Base.VideoCapture.Class.Display.Setup();
        }

        private void IncreaseHeightOffset()
        {
            var centerHeight = (Screen.PrimaryScreen.Bounds.Height / 2);

            if (_class.Base.System.Class.Vr.HeightOffset >= centerHeight - 10) return;

            _class.Base.System.Class.Vr.HeightOffset += 5;
            _class.Base.System.Class.Set.Add("VR_Height", _class.Base.System.Class.Vr.HeightOffset.ToString());

            _class.Base.VideoCapture.Class.Display.Setup();
        }

        private void IncreaseWidthOffset()
        {
            var centerWidth = (Screen.PrimaryScreen.Bounds.Width / 4);

            if (_class.Base.System.Class.Vr.WidthOffset >= centerWidth - 10) return;

            _class.Base.System.Class.Vr.WidthOffset += 5;
            _class.Base.System.Class.Set.Add("VR_Width", _class.Base.System.Class.Vr.WidthOffset.ToString());

            _class.Base.VideoCapture.Class.Display.Setup();
        }

        private void DecreaseWidthOffset()
        {
            if (_class.Base.System.Class.Vr.WidthOffset <= 0) return;

            _class.Base.System.Class.Vr.WidthOffset -= 5;
            _class.Base.System.Class.Set.Add("VR_Width", _class.Base.System.Class.Vr.WidthOffset.ToString());

            _class.Base.VideoCapture.Class.Display.Setup();
        }

        private void ResetOffsets()
        {
            _class.Base.System.Class.Vr.WidthOffset = 0;
            _class.Base.System.Class.Set.Add("VR_Width", _class.Base.System.Class.Vr.WidthOffset.ToString());
            _class.Base.System.Class.Vr.HeightOffset = 0;
            _class.Base.System.Class.Set.Add("VR_Height", _class.Base.System.Class.Vr.HeightOffset.ToString());

            _class.Base.VideoCapture.Class.Display.Setup();
        }
    }
}
