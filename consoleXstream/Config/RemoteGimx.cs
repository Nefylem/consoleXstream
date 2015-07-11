using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Config
{
    public class RemoteGimx
    {
        public RemoteGimx(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Change()
        {
            _class.System.UseTitanOne = false;
            _class.System.UseControllerMax = false;
            _class.System.UseGimxRemote = !_class.System.UseGimxRemote;
        }
    }
}
