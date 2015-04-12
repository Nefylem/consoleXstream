using consoleXstream.DrawGui;
using consoleXstream.Home;
using consoleXstream.Menu.Data;
using consoleXstream.Menu.MainMenu;
using consoleXstream.Menu.SubMenu;
using consoleXstream.Menu.SubMenuOptions;
using consoleXstream.Menu.VR;

namespace consoleXstream.Menu
{
    public class Classes
    {
        public Classes(BaseClass baseClass, ShowMenu main)
        {
            DisplayMenu = main;
            Base = baseClass;
            Declare();
        }

        public BaseClass Base { get; private set; }
        public ShowMenu DisplayMenu { get; private set; }
        public MainMenu.Action Action { get; private set; }
        public SubMenu.Action SubAction { get; private set; }
        public ButtonItem Button { get; private set; }
        public Create CreateMain { get; private set; }
        public MainMenu.Menu MainMenu { get; private set; }
        public SubMenu.Menu SubMenu { get; private set; }
        public Navigation Nav { get; private set; }
        public Variables Var { get; private set; }
        public Interaction Data { get; private set; }
        public FrameCount Fps { get; private set; }
        public Mouse Mouse { get; private set; }
        public Shutter Shutter { get; private set; }
        public Gamepad Gamepad { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public User User { get; private set; }
        public Display Display { get; private set; }
        public SubSelectMenu.Navigation SubNav { get; private set; }
        public CaptureDevice VideoDevice { get; private set; }
        public DrawGraph DrawGui { get; private set; }
        public Profiles Profiles { get; private set; }
        public SubSelectMenu.Menu SubSelectMenu { get; private set; }
        public SubSelectMenu.Var SubSelectVar { get; private set; }
        public Remap.Remap RemapMenu { get; private set; }
        public Remap.Gamepad RemapGamepad { get; private set; }
        public Remap.Navigation RemapNav { get; private set; }
        
        public VR.Config ConfigVr { get; private set; }
        public Resize ResizeVr { get; private set; }
        public Reposition RepositionVr { get; private set; }
        public HeadTracking HeadTracking { get; private set; }

        private void Declare()
        {
            Action = new MainMenu.Action(this);
            Button = new ButtonItem(this);
            CreateMain = new Create(this);
            Keyboard = new Keyboard(this);
            MainMenu = new MainMenu.Menu(this);
            Mouse = new Mouse(this);
            Nav = new Navigation(this);
            Shutter = new Shutter(this);
            SubAction = new SubMenu.Action(this);
            SubMenu = new SubMenu.Menu(this);
            
            SubSelectMenu = new SubSelectMenu.Menu(this);
            SubSelectVar = new SubSelectMenu.Var();
            SubNav = new SubSelectMenu.Navigation(this);

            Var = new Variables();
            Data = new Interaction();
            Fps = new FrameCount();
            Gamepad = new Gamepad(this);
            User = new User();  
            DrawGui = new DrawGraph();
            Display = new Display(this);
            VideoDevice = new CaptureDevice(this);
            Profiles = new Profiles(this);

            RemapMenu = new Remap.Remap(this);
            RemapGamepad = new Remap.Gamepad(this);
            RemapNav = new Remap.Navigation(this);

            ConfigVr = new VR.Config(this);
            ResizeVr = new Resize(this);
            RepositionVr = new Reposition(this);
            HeadTracking = new HeadTracking(this);
        }
    }
}
