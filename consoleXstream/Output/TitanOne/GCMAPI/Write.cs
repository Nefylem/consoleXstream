using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Write
    {
        public Write(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void setDevice(string device)
        {
            
        }

        public void Send()
        {
            if (_class.MDefine.GcmapiConnect != null)
                _class.MDefine.GcmapiConnect((ushort)_class.Write.DevId);

            if (_class.MDefine.GcmapiIsConnected(0) == 1)
            {
                _class.MDefine.GcmapiWrite(0, _class.Gamepad.Output);
            }
        }
    }
}
