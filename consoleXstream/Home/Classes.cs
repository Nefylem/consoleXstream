
using consoleXstream.Home.Loop;

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
        public Startup.Declaration Declaration { get; private set; }
        public Development Development { get; private set; }
        public MenuHandler Menu { get; private set; }
        public Mouse Mouse { get; private set; }
        public MainSystem System { get; private set; }
        public Var Var { get; private set; }

        public Loop.MainLoop MainLoop { get; private set; }
        public Loop.PartialLoops PartialLoop { get; private set; }
        public Loop.LoopController LoopController { get; private set; }

        public Startup.Initial Startup { get; private set; }
        public Startup.Display StartupDisplay { get; private set; }
        public Startup.TitanOne StartupTitanOne { get; private set; }
        public Startup.Timers Timers { get; private set; }

        private void DeclareClasses()
        {
            CheckFps = new CheckFps(this);
            CheckTitanDevices = new CheckTitanDevices(this);
            Controller = new ControllerInput(this);
            Declaration = new Startup.Declaration(this);
            Development = new Development(this);            
            Menu = new MenuHandler(this);
            Mouse = new Mouse(this);
            System = new MainSystem(this);
            Var = new Var();

            Startup = new Startup.Initial(this);
            StartupDisplay = new Startup.Display(this);
            StartupTitanOne = new Startup.TitanOne(this);
            Timers = new Startup.Timers(this);

            MainLoop = new MainLoop(this);
            PartialLoop = new PartialLoops(this);
            LoopController = new LoopController(this);
        }
    }
}
