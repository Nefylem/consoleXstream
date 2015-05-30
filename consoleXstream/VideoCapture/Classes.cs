using consoleXstream.Config;
using consoleXstream.Home;
using consoleXstream.VideoCapture.Analyse;
using consoleXstream.VideoCapture.Data;
using consoleXstream.VideoCapture.GraphBuilder;
using Crossbar = consoleXstream.VideoCapture.Analyse.Crossbar;
using Display = consoleXstream.VideoCapture.GraphBuilder.Display;

namespace consoleXstream.VideoCapture
{
    public class Classes
    {
        public Classes(BaseClass baseClass)
        {
            BaseClass = baseClass;
            VideoCapture = baseClass.VideoCapture;
            FrmMain = baseClass.Home;
            System = baseClass.System;

            DeclareClasses();
        }

        public BaseClass BaseClass { get; private set; }
        public Form1 FrmMain { get; set; }
        public Audio Audio { get; set; }
        public Capture Capture { get; set; }
        public Crossbar Crossbar { get; set; }
        public Debug Debug { get; set; }
        public Resolution Resolution { get; set; }
        public Variables Var { get; set; }
        public Graph Graph { get; set; }
        public VideoCapture VideoCapture { get; private set; }
        public User User { get; set; }
        public Configuration System { get; set; }
        public GraphMap GraphBuild { get; set; }
        public Pin GraphPin { get; set; }
        public Filter GraphFilter { get; set; }
        public AviRender AviRender { get; set; }
        public GraphCrossbar GraphCrossbar { get; set; }
        public SampleGrabber SampleGrabber { get; set; }
        public GraphSmartTee SmartTee { get; set; }
        public InfiniteTee InfiniteTee { get; set; }
        public GraphResolution GraphResolution { get; set; }
        public Close Close { get; set; }
        public Display Display { get; set; }

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

            GraphPin = new Pin(this);
            GraphFilter = new Filter(this);
            AviRender = new AviRender(this);
            SampleGrabber = new SampleGrabber(this);
            SmartTee = new GraphSmartTee(this);
            InfiniteTee = new InfiniteTee(this);
            GraphCrossbar = new GraphCrossbar(this);
            Close = new Close(this);
            Display = new Display(this);
            GraphBuild = new GraphMap(this);
            GraphResolution = new GraphResolution(this);
        }
    }
}
