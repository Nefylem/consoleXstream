using consoleXstream.Config;
using consoleXstream.DrawGui;
using consoleXstream.Input;
using consoleXstream.Menu.Data;
using consoleXstream.Menu.MainMenu;
using consoleXstream.Menu.SubMenu;
using consoleXstream.Menu.SubMenuOptions;
using consoleXstream.Remap;
using Display = consoleXstream.Menu.SubMenuOptions.Display;

namespace consoleXstream.Menu
{
    public class Classes
    {
        public Classes(ShowMenu men) { DisplayMenu = men; }

        public Form1 Form1 { get; set; }
        public ShowMenu DisplayMenu { get; set; }
        public MainMenu.Action Action { get; set; }
        public SubMenu.Action SubAction { get; set; }
        public ButtonItem Button { get; set; }
        public Create CreateMain { get; set; }
        public MainMenu.Menu MainMenu { get; set; }
        public SubMenu.Menu SubMenu { get; set; }
        public Navigation Nav { get; set; }
        public Variables Var { get; set; }
        public Interaction Data { get; set; }
        public FrameCount Fps { get; set; }
        public Mouse Mouse { get; set; }
        public Shutter Shutter { get; set; }
        public Gamepad Gamepad { get; set; }
        public Keyboard Keyboard { get; set; }
        public KeyboardHook KeyboardHook { get; set; }
        public Keymap Keymap { get; set; }
        public User User { get; set; }
        public Configuration System { get; set; }
        public VideoCapture.VideoCapture VideoCapture { get; set; }
        public Display Display { get; set; }
        public SubMenuOptions.Remap SubRemap { get; set; }
        public SubSelectMenu.Navigation SubNav { get; set; }
        public Remapping Remap { get; set; }
        public CaptureDevice VideoDevice { get; set; }
        public DrawGraph DrawGui { get; set; }
        public Profiles Profiles { get; set; }
        
        public SubSelectMenu.Menu SubSelectMenu { get; set; }
        public SubSelectMenu.Var SubSelectVar { get; set; }


        public void DeclareClasses()
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
        }
    }
}
