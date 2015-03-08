using consoleXstream.Config;
using consoleXstream.VideoCapture.Analyse;
using consoleXstream.VideoCapture.Data;

namespace consoleXstream.VideoCapture
{
    class Classes
    {
        public Classes(VideoCapture video, Form1 main, Configuration sys) { VideoCapture = video; FrmMain = main; System = sys; }

        public Form1 FrmMain { get; set; }
        public Audio Audio { get; set; }
        public Capture Capture { get; set; }
        public Crossbar Crossbar { get; set; }
        public Debug Debug { get; set; }
        public Resolution Resolution { get; set; }
        public Variables Var { get; set; }
        public Graph Graph { get; set; }
        public VideoCapture VideoCapture { get; set; }
        public User User { get; set; }
        public Configuration System { get; set; }

        public void DeclareClasses()
        {
            Audio = new Audio(this);
            Capture = new Capture(this);
            Crossbar = new Crossbar(this);
            Debug = new Debug(this);
            Resolution = new Resolution(this);
            Var = new Variables(this);
            User = new User(this);
            Graph = new Graph();

        }
    }
}
