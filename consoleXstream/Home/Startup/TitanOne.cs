using System.Collections.Generic;
using consoleXstream.Output.TitanOne;

namespace consoleXstream.Home.Startup
{
    public class TitanOne
    {
        public TitanOne(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void InitializeTitanOne()
        {
            var var = _class.Var;
            var system = _class.BaseClass.System;
            var titanOne = _class.BaseClass.TitanOne;

            if (var.ListToDevices == null)
                var.ListToDevices = new List<string>();

            system.Debug("[3] Configure TitanOne API");
            titanOne.SetToInterface(Define.DevPid.TitanOne);

            if (system.TitanOneDevice != null)
            {
                if (system.TitanOneDevice.Length > 0)
                {
                    titanOne.SetApiMethod(Define.ApiMethod.Multi);
                    titanOne.SetTitanOneDevice(system.TitanOneDevice);
                }
            }

            titanOne.Initialize();
        }
    }
}
