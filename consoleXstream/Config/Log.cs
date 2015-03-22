namespace consoleXstream.Config
{
    public class Log
    {
        private bool _menu;
        private bool _titanOne;
        private bool _controllerMax;

        public bool CheckLog(string title)
        {
            title = title.ToLower();
            switch (title)
            {
                case "menu": return _menu; 
                case "titanone": return _titanOne;
                case "controllermax": return _controllerMax;
            }
            return false;
        }

        public void SetValue(string title, bool set)
        {
            title = title.ToLower();
            switch (title)
            {
                case "menu": _menu = set; break;
                case "titanone": _titanOne = set; break;
                case "controllermax": _controllerMax = set; break;
            }            
        }
    }
}
