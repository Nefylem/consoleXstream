using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Config
{
    class Var
    {
        public Var(Classes classes) { _class = classes; }
        private Classes _class;

        public bool IsReadData { get; set; }

    }
}
