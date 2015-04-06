using consoleXstream.Home.Startup;

namespace consoleXstream.Home
{
    public class Classes
    {
        public Classes(Form1 frmMain, BaseClass baseClass)
        {
            Home = frmMain;
            BaseClass = baseClass;
            DeclareClasses();
        }

        public BaseClass BaseClass { get; private set; }
        public Form1 Home { get; private set; }

        public CheckFps CheckFps { get; private set; }
        public CheckTitanDevices CheckTitanDevices { get; private set; }
        public ControllerInput Controller { get; private set; }
        public Declaration Declaration { get; private set; }
        public Development Development { get; private set; }
        public MainLoop MainLoop { get; private set; }
        public MenuHandler Menu { get; private set; }
        public Mouse Mouse { get; private set; }
        public Initial Startup { get; private set; }
        public Display StartupDisplay { get; private set; }
        public TitanOne StartupTitanOne { get; private set; }
        public System System { get; private set; }
        public Var Var { get; private set; }

        private void DeclareClasses()
        {
            CheckFps = new CheckFps(this);
            CheckTitanDevices = new CheckTitanDevices(this);
            Controller = new ControllerInput(this);
            Declaration = new Declaration(this);
            Development = new Development(this);
            MainLoop = new MainLoop(this);
            Menu = new MenuHandler(this);
            Mouse = new Mouse(this);
            Startup = new Initial(this);
            StartupDisplay = new Display(this);
            StartupTitanOne = new TitanOne(this);
            System = new System(this);
            Var = new Var();
        }
    }
}
