namespace consoleXstream.Config
{
    public class TitanOne
    {
        public TitanOne(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Change()
        {
            if (_class.System.UseTitanOneApi)
            {
                changeTitanOne_TOAPI();
                return;
            }

            if (_class.System.UseControllerMax && _class.System.UseTitanOne)      //Stop infinite loops
                _class.System.UseControllerMax = false;

            if (_class.System.UseControllerMax) _class.ControllerMaxConfig.Change();       //Disable if running

            _class.System.UseTitanOne = !_class.System.UseTitanOne;
            _class.Set.Add("TitanOne", _class.System.UseTitanOne.ToString());

            if (_class.System.UseTitanOne)
                _class.TitanOne.Initialize();
        }

        public void Change(bool set)
        {
            if (_class.System.UseTitanOneApi)
            {
                changeTitanOne_TOAPI();
                return;
            }

            if (_class.System.UseControllerMax && _class.System.UseTitanOne)      //Stop infinite loops
                _class.System.UseControllerMax = false;

            if (_class.System.UseControllerMax) _class.ControllerMaxConfig.Change();        //Disable if running

            _class.System.UseTitanOne = set;
            _class.Set.Add("TitanOne", _class.System.UseTitanOne.ToString());

            if (_class.System.UseTitanOne)
                _class.TitanOne.Initialize();
            else
                _class.TitanOne.Close();
        }

        public void changeTitanOne_TOAPI()
        {
            _class.TitanOne.Close();

            _class.System.UseTitanOne = !_class.System.UseTitanOne;
            _class.Set.Add("TitanOne", _class.System.UseTitanOne.ToString());

            if (_class.System.UseTitanOne)
            {
                _class.System.UseControllerMax = false;
                _class.Set.Add("ControllerMax", _class.System.UseControllerMax.ToString());
            }

            if (!_class.System.UseTitanOne) return;
            _class.TitanOne.SetToInterface(Output.TitanOne.Define.DevPid.TitanOne);
            _class.TitanOne.Initialize();
        }

    }
}
