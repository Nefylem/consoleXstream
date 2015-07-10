using consoleXstream.Config;
using consoleXstream.Output;
using consoleXstream.Remap;
using consoleXstream.Scripting;

namespace consoleXstream
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
        public Menu.ShowMenu Menu                                       { get; private set; }

        public Output.ControllerMax ControllerMax                       { get; private set; }
        public Output.TitanOne.Write TitanOne                           { get; private set; }
        public Output.Gamepad Gamepad                                   { get; private set; }
        public Output.Gimx.Gimx Gimx                                    { get; private set; }
        public Output.Gimx.GimxRemote GimxRemote                        { get; private set; }

        public Input.KeyboardHook Keyboard                              { get; private set; }
        public Input.KeyboardInterface KeyboardInterface                { get; private set; }
        public Input.Mouse.Hook Mouse                                   { get; private set; }
        private ExternalScript External                                 { get; set; }
        public VideoCapture.VideoCapture VideoCapture                   { get; private set; }


        private void Declare()
        {
            ControllerMax = new Output.ControllerMax(this);
            Gamepad = new Output.Gamepad(this);
            Gimx = new Output.Gimx.Gimx(this);
            GimxRemote = new Output.Gimx.GimxRemote(this);
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
