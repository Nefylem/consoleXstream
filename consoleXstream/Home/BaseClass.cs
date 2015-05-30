using consoleXstream.Config;
using consoleXstream.Output;
using consoleXstream.Remap;
using consoleXstream.Scripting;

namespace consoleXstream.Home
{
    public class BaseClass
    {
        public BaseClass(Form1 form)
        {
            Home = form;
            Declare();
        }

        public Form1 Home                                               { get; private set; }
        public Configuration System                                     { get; private set; }
        public Remapping Remap                                          { get; private set; }
        public VideoResolution VideoResolution                          { get; private set; }
        public Keymap Keymap                                            { get; private set; }
        public Mousemap Mousemap                                        { get; private set; }
        public Menu.ShowMenu Menu                                       { get; private set; }
        public Home.Classes HomeClass                                   { get; set; }
        public Output.ControllerMax ControllerMax                       { get; private set; }
        public Output.TitanOne.Write TitanOne                           { get; private set; }
        public Output.Gamepad Gamepad                                   { get; private set; }
        private Gimx Gimx                                               { get; set; }

        public Input.KeyboardHook Keyboard                              { get; private set; }
        public Input.KeyboardInterface KeyboardInterface                { get; private set; }
        public Input.Mouse.Hook Mouse                                   { get; private set; }
        private ExternalScript External                                 { get; set; }
        public VideoCapture.VideoCapture VideoCapture                   { get; set; }


        private void Declare()
        {
            ControllerMax = new Output.ControllerMax(this);
            Gamepad = new Output.Gamepad(this);
            Gimx = new Gimx(this);
            Keyboard = new Input.KeyboardHook(this);
            KeyboardInterface = new Input.KeyboardInterface(this);
            Mousemap = new Mousemap(this);
            Keymap = new Keymap(this);
            Menu = new Menu.ShowMenu(this);
            Mouse = new Input.Mouse.Hook(this);
            Remap = new Remapping();
            System = new Configuration(this);
            TitanOne = new Output.TitanOne.Write(this);
            VideoResolution = new VideoResolution(this);

            External = new ExternalScript(Home);

            VideoCapture = new VideoCapture.VideoCapture(this);
        }
    }
}
