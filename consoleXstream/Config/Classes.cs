namespace consoleXstream.Config
{
    public class Classes
    {
        public Classes(Configuration sys) { System = sys; }

        public Debug Debug { get; set; }
        public Form1 Main { get; set; }
        public Profile Profile { get; set; }
        public Var Var { get; set; }
        public Set Set { get; set; }
        public Configuration System { get; set; }
        public VideoCapture.VideoCapture VideoCapture { get; set; }
        public Output.ControllerMax ControllerMax { get; set; }
        public Output.TitanOne.Write TitanOne { get; set; }
        public VideoResolution VideoResolution { get; set; }
        public XmlData Xml { get; set; }
        public Log Log { get; set; }
        public Settings Settings { get; set; }
        public Gamepad Gamepad { get; set; }
        public Display Display { get; set; }
        public TitanOne TitanOneConfig { get; set; }
        public ControllerMax ControllerMaxConfig { get; set; }
        public BaseClass BaseClass { get; set; }
        public Vr Vr { get; private set; }

        public void DeclareClasses(BaseClass baseClass)
        {
            BaseClass = baseClass;
            Main = baseClass.Home;

            Debug = new Debug();
            Profile = new Profile(this);
            Set = new Set(this);
            Var = new Var(this);
            Xml = new XmlData(this);
            Log = new Log();
            Settings = new Settings(this);
            Gamepad = new Gamepad(this);
            Display = new Display(this);
            TitanOneConfig = new TitanOne(this);
            ControllerMaxConfig = new ControllerMax(this);
            Vr = new Vr(this);
        }
    }
}
