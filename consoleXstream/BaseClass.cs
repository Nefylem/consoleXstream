using consoleXstream.Config;
using consoleXstream.Output;
using consoleXstream.Remap;
using consoleXstream.Scripting;

namespace consoleXstream
{
    public class BaseClass
    {
        public BaseClass(Form1 form ) { Home = form; }

        public Form1 Home { get; set; }
        public Configuration System { get; set; }
        public Remapping Remap { get; set; }
        public VideoResolution VideoResolution { get; set; }
        public Keymap Keymap { get; set; }
        public Menu.ShowMenu Menu { get; set; }


        public Output.ControllerMax ControllerMax { get; set; }
        public Output.TitanOne.Write TitanOne { get; set; }
        public Output.Gamepad Gamepad { get; set; }
        private Gimx Gimx { get; set; }

        public Input.KeyboardHook Keyboard { get; set; }
        public Input.KeyboardInterface KeyboardInterface { get; set; }
        public Input.Mouse.Hook Mouse { get; set; }

        private ExternalScript External { get; set; }

        public VideoCapture.VideoCapture VideoCapture;


        public void Declare()
        {
            ControllerMax = new Output.ControllerMax(this);
            Gamepad = new Output.Gamepad(this);
            Gimx = new Gimx(this);
            Keyboard = new Input.KeyboardHook(this);
            Keymap = new Keymap(this);
            Menu = new Menu.ShowMenu(this);
            Mouse = new Input.Mouse.Hook(this);
            Remap = new Remapping();
            System = new Configuration(this);
            VideoResolution = new VideoResolution(this);

            //Fix these later
            KeyboardInterface = new Input.KeyboardInterface(Home);
            KeyboardInterface.getSystemHandle(System);
            KeyboardInterface.getRemapHangle(Remap);
            KeyboardInterface.getKeymapHandle(Keymap);
            

            TitanOne = new Output.TitanOne.Write(Home, System, Gamepad);
            External = new ExternalScript(Home);


            VideoCapture = new VideoCapture.VideoCapture(Home, System);

            System.GetClassHandles(VideoCapture, ControllerMax, TitanOne, VideoResolution);
        }
    }
}
