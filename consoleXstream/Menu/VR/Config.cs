namespace consoleXstream.Menu.VR
{
    public class Config
    {
        public Config(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void AddVrMenuOptions()
        {
            _class.SubAction.AddSubItem("VrVideo", "VR Mode\nVideo", _class.Base.System.IsVr);
            
            if (!_class.Base.System.IsVr) return;

            _class.SubAction.AddSubItem("Resize", "Resize");
            _class.SubAction.AddSubItem("Reposition", "Reposition");

        }

        public void SetVrOptions(string command)
        {
            command = command.ToLower();

            switch (command)
            {
                case "vrvideo": SwitchVrMode(); break;
                case "resize": _class.ResizeVr.SetResizeMode(); break;
                case "reposition": _class.RepositionVr.SetRepositionMode(); break;
            }
        }

        private void SwitchVrMode()
        {
            _class.Base.System.Class.Vr.ChangeVrVideo();
            _class.Action.CheckDisplaySettings();
            _class.DisplayMenu.PositionMenu();

            //Rebuild menu
            _class.Action.ClearSub();
            _class.ConfigVr.AddVrMenuOptions();
            _class.Action.SelectSubItem();
        }
    }
}
