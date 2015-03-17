using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Menu.SubSelectMenu
{
    public class Var
    {
        public string DisplayTitle;
        public string DisplayMessage;

        public List<string> ListData = new List<string>();

        public int Selected { get; set; }
        public string TitanSerial { get; set; }
    }
}
