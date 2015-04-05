namespace consoleXstream.Menu.VR
{
    public class Reposition
    {
        public Reposition(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void SetRepositionMode()
        {
            _class.Nav.SetOkWait(3);
            _class.Var.IsRepositionVr = true;
            _class.Base.Menu.Visible = false;
        }

        public void CloseRepositionMode()
        {
            if (_class.Nav._moveOkWait != 0) return;
            _class.Nav.SetOkWait(2);
            _class.Var.IsRepositionVr = false;
            _class.Base.Menu.Visible = true;
        }

        public void GetCommand(string command)
        {
            command = command.ToLower();
            switch (command)
            {
                case "ok": CloseRepositionMode(); return;
                case "up": MoveDisplayUp(); return;
                case "down": MoveDisplayDown(); return;
                case "left": MoveDisplayLeft(); return;
                case "right": MoveDisplayRight(); return;
                case "back": ResetDisplay(); return;
            }
        }

        private void ResetDisplay()
        {
            _class.Base.System.Class.Vr.VideoHeightOffset = 0;
            _class.Base.System.Class.Vr.VideoWidthOffset = 0;
            _class.Base.System.Class.Set.Add("VR_OffsetWidth", _class.Base.System.Class.Vr.VideoWidthOffset.ToString());
            _class.Base.System.Class.Set.Add("VR_OffsetHeight", _class.Base.System.Class.Vr.HeightOffset.ToString());

            _class.Base.Home.MoveVrDisplay();            
        }

        private void MoveDisplayRight()
        {
            _class.Base.System.Class.Vr.VideoWidthOffset += 5;
            _class.Base.System.Class.Set.Add("VR_OffsetWidth", _class.Base.System.Class.Vr.VideoWidthOffset.ToString());

            _class.Base.Home.MoveVrDisplay();            
        }

        private void MoveDisplayLeft()
        {
            _class.Base.System.Class.Vr.VideoWidthOffset -= 5;
            _class.Base.System.Class.Set.Add("VR_OffsetWidth", _class.Base.System.Class.Vr.VideoWidthOffset.ToString());

            _class.Base.Home.MoveVrDisplay();
        }

        private void MoveDisplayUp()
        {
            _class.Base.System.Class.Vr.VideoHeightOffset -= 5;
            _class.Base.System.Class.Set.Add("VR_OffsetHeight", _class.Base.System.Class.Vr.VideoHeightOffset.ToString());

            _class.Base.Home.MoveVrDisplay();
        }

        private void MoveDisplayDown()
        {
            _class.Base.System.Class.Vr.VideoHeightOffset += 5;
            _class.Base.System.Class.Set.Add("VR_OffsetHeight", _class.Base.System.Class.Vr.VideoHeightOffset.ToString());

            _class.Base.Home.MoveVrDisplay();
        }
    }
}
