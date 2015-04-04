namespace consoleXstream.Config
{
    public class Gamepad
    {
        public Gamepad(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void ChangeDs4Emulation()
        {
            _class.System.IsPs4ControllerMode = !_class.System.IsPs4ControllerMode;
            _class.Set.Add("DS4Emulation", _class.System.IsPs4ControllerMode.ToString());
        }

        public void ChangeNormalizeGamepad()
        {
            _class.System.IsNormalizeControls = !_class.System.IsNormalizeControls;
            _class.Set.Add("Normalize", _class.System.IsNormalizeControls.ToString());
        }

        public void ChangeRumble()
        {
            _class.System.UseRumble = !_class.System.UseRumble;
            _class.Set.Add("Rumble", _class.System.UseRumble.ToString());
        }


    }
}
