namespace consoleXstream.Config
{
    public class ControllerMax
    {
        public ControllerMax(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Change()
        {
            if (_class.System.UseTitanOneApi)
            {
                if (_class.Log.CheckLog("TitanOne")) _class.Debug.debug("TitanOne.Log", "Connecting ControllerMax using TO API");
                changeControllerMax_TOAPI();
                return;
            }

            if (_class.Log.CheckLog("ControllerMax")) _class.Debug.debug("ControllerMax.Log", "Using CM API");

            if (_class.System.UseControllerMax && _class.System.UseTitanOne)      //Stop infinite loops
                _class.System.UseTitanOne = false;

            if (_class.System.UseTitanOne) _class.TitanOneConfig.Change();         //Disable if running


            _class.System.UseControllerMax = !_class.System.UseControllerMax;
            _class.Set.Add("ControllerMax", _class.System.UseControllerMax.ToString());

            if (_class.System.UseControllerMax)
                _class.ControllerMax.initControllerMax();
            else
                _class.ControllerMax.closeControllerMaxInterface();
        }

        public void changeControllerMax_TOAPI()
        {
            _class.TitanOne.Close();

            _class.System.UseControllerMax = !_class.System.UseControllerMax;
            _class.Set.Add("ControllerMax", _class.System.UseControllerMax.ToString());

            if (_class.System.UseControllerMax)
            {
                _class.System.UseTitanOne = false;
                _class.Set.Add("TitanOne", _class.System.UseTitanOne.ToString());
            }

            if (!_class.System.UseControllerMax) return;
            _class.TitanOne.SetToInterface(Output.TitanOne.Define.DevPid.ControllerMax);
            _class.TitanOne.Initialize();
        }
    }
}
