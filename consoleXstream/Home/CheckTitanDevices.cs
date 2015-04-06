using System.Collections.Generic;

namespace consoleXstream.Home
{
    public class CheckTitanDevices
    {
        public CheckTitanDevices(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Confirm()
        {
            if (_class.Var.RetrySetTitanOne == null) return;
            if (_class.Var.RetrySetTitanOne.Length <= 0) return;
            _class.Var.RetryTimeOut--;
            
            if (_class.Var.RetryTimeOut <= 0)
                _class.Var.RetrySetTitanOne = "";

            int result = _class.BaseClass.TitanOne.CheckDevices();
            if (result <= 0) return;

            string serial = _class.Var.RetrySetTitanOne;
            _class.Var.RetrySetTitanOne = "";
            _class.BaseClass.System.DisableTitanOneRetry = true;
            _class.BaseClass.TitanOne.SetTitanOneDevice(serial);
        }

        public void ConnectionList()
        {
            var result = _class.BaseClass.TitanOne.CheckDevices();
            if (result == 0)
            {
                if (_class.Var.TitanOneListRefresh > 0)
                    _class.Var.TitanOneListRefresh--;
                else
                {
                    if (!_class.Var.TitanOneListRefreshFail)
                    {
                        _class.Var.TitanOneListRefreshFail = true;
                        _class.Var.TitanOneListRefresh = 10;
                        _class.BaseClass.TitanOne.ListDevices();
                    }
                    else
                    {
                        _class.Var.IsUpdatingTitanOneList = false;
                        _class.Var.TitanOneListRefresh = 0;
                        _class.Var.TitanOneListRefreshFail = false;

                        _class.Var.ListToDevices.Clear();

                        foreach (var item in _class.Var.ListBackupToDevices) { _class.Var.ListToDevices.Add(item); }
                        _class.Var.IsUpdatingTitanOneList = false;
                        _class.BaseClass.Menu.PassToSubSelect(_class.Var.ListToDevices);

                    }
                }
                return;
            }

            if (result <= 0) return;

            _class.Var.IsUpdatingTitanOneList = false;
            _class.BaseClass.Menu.PassToSubSelect(_class.Var.ListToDevices);
        }

        public void List()
        {
            _class.Var.ListBackupToDevices = new List<string>();

            foreach (var item in _class.Var.ListToDevices)
                _class.Var.ListBackupToDevices.Add(item);

            _class.Var.ListToDevices = new List<string>();

            if (_class.BaseClass.ControllerMax._gcapi_Unload != null)
                _class.BaseClass.ControllerMax.closeControllerMaxInterface();

            _class.Var.IsUpdatingTitanOneList = true;
            _class.Var.TitanOneListRefresh = 10;
            _class.Var.TitanOneListRefreshFail = false;
            _class.BaseClass.TitanOne.ListDevices();
        }

    }
}
