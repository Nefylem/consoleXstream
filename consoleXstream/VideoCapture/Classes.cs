using consoleXstream.Config;
using consoleXstream.VideoCapture.Analyse;
using consoleXstream.VideoCapture.Data;
using DirectShowLib;

namespace consoleXstream.VideoCapture
{
    class Classes
    {
        public Classes(VideoCapture video) { VideoCapture = video; }

        public Audio Audio { get; set; }
        public Capture Capture { get; set; }
        public Debug Debug { get; set; }
        public Resolution Resolution { get; set; }
        public Variables Var { get; set; }

        public Graph Graph { get; set; }
        public VideoCapture VideoCapture { get; set; }

        public Configuration System { get; set; }

        public void DeclareClasses()
        {
            Audio = new Audio(this);
            Capture = new Capture(this);
            Debug = new Debug(this);
            Resolution = new Resolution(this);
            Var = new Variables(this);
            Graph = new Graph();
        }
    }
}
