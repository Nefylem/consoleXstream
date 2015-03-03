namespace consoleXstream.Config
{
    class VrMode
    {
        private readonly Configuration _system;

        public VrMode(Configuration config) { _system = config; }

        public void InitializeVr()
        {
            _system.IsVr = true;
            _system.SetupMouse(1);
        }
    }
}
