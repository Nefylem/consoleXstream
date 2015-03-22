namespace consoleXstream.Config
{
    public class Settings
    {
        public Settings(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Check()
        {
            if (_class.Set.Check("keyboard").ToLower() == "true") _class.System.IsEnableKeyboard = true;
            if (_class.Set.Check("mouse").ToLower() == "true") _class.System.IsEnableMouse = true;
            if (_class.Set.Check("hidemouse").ToLower() == "true") _class.System.IsHideMouse = true;

            if (_class.Set.Check("internalCapture").ToLower() == "true") _class.System.UseInternalCapture = true;

            if (_class.Set.Check("controllerrumble").ToLower() == "true") _class.System.UseRumble = true;
            if (_class.Set.Check("ds4emulation").ToLower() == "true") _class.System.IsPs4ControllerMode = true;
            if (_class.Set.Check("normalize").ToLower() == "true") _class.System.IsNormalizeControls = true;

            if (_class.Set.Check("showfps").ToLower() == "true") _class.System.CheckFps = true;

            if (_class.Set.Check("stayontop").ToLower() == "true") _class.System.IsStayOnTop = true;

            if (_class.Set.Check("checkcaptureres").ToLower() == "true") _class.System.IsAutoSetCaptureResolution = true;
            if (_class.Set.Check("AutoResolution").ToLower() == "true") _class.System.IsAutoSetDisplayResolution = true;
            if (_class.Set.Check("VR_Video").ToLower() == "true") _class.System.IsVr = true;
            
            if (_class.Set.Check("MenuLog").ToLower() == "true") _class.Log.SetValue("Menu", true);

            if (_class.Set.Check("controllermax").ToLower() == "true") _class.System.UseControllerMax = true;
            if (_class.Set.Check("titanone").ToLower() == "true") _class.System.UseTitanOne = true;
            if (_class.Set.Check("UseTitanOne").Length > 0)
            {
                _class.System.TitanOneDevice = _class.Set.Check("UseTitanOne");
                _class.Main.SetTitanOne(_class.Set.Check("UseTitanOne"));
            }

            _class.System.RefreshRate = _class.Set.Check("RefreshRate");
            _class.System.DisplayResolution = _class.Set.Check("Resolution");

            if (!_class.System.IsAutoSetDisplayResolution && (_class.System.RefreshRate.Length > 0 || _class.System.DisplayResolution.Length > 0))
            {
                if (_class.System.DisplayResolution.Length == 0)
                    _class.Display.SetRefresh(_class.System.RefreshRate);
                else
                    _class.Display.SetResolution(_class.System.DisplayResolution);
            }

            if (_class.Set.Check("CaptureResolution").Length > 0) _class.Display.SetResolution(_class.Set.Check("CaptureResolution"));

            _class.System.CaptureProfile = _class.Set.Check("CurrentProfile");

            if (_class.Main.boolIDE)
                _class.System.IsHideMouse = false;
        }

    }
}
