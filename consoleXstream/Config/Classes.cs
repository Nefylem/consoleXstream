using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Config
{
    class Classes
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

        public void DeclareClasses(Form1 mainForm)
        {
            Main = mainForm;

            Debug = new Debug();
            Profile = new Profile(this);
            Set = new Set(this);
            Var = new Var(this);
            Xml = new XmlData(this);
        }
    }
}
