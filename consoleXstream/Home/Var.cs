using System.Collections.Generic;

namespace consoleXstream.Home
{
    public class Var
    {
        public bool IsIde { get; set; }
        public int MouseScreenBounds { get; set; }

        public int MouseX { get; set; }
        public int MouseY { get; set; }

        public List<string> ListToDevices { get; set; }
        public List<string> ListBackupToDevices { get; set; }
        public string RetrySetTitanOne { get; set; }
        public bool TitanOneListRefreshFail { get; set; }
        public bool IsUpdatingTitanOneList { get; set; }
        public int RetryTimeOut { get; set; }
        public int TitanOneListRefresh { get; set; }

        public int BlockMenuCount { get; set; }

        public bool IsFullscreen { get; set; }

    }
}
