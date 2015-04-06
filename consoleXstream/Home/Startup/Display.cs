using System.Drawing;
using System.Windows.Forms;

namespace consoleXstream.Home.Startup
{
    public class Display
    {
        public Display(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void ConfigureDisplayWindow()
        {
            _class.Home.imgDisplay.Dock = DockStyle.Fill;
            _class.Home.imgDisplay.BackColor = Color.Black;
        }

        public void ConfigureDisplayWindowsVrMode()
        {
            _class.Home.imgDisplayVr.Visible = true;
            _class.Home.imgDisplayVr.BackColor = Color.Black;

            _class.Home.imgDisplay.Visible = true;
            _class.Home.imgDisplay.BackColor = Color.Black;

            _class.Home.imgDisplay.Left = 0;
            _class.Home.imgDisplay.Top = 0;
            _class.Home.imgDisplay.Width = Screen.PrimaryScreen.Bounds.Width / 2;
            _class.Home.imgDisplay.Height = Screen.PrimaryScreen.Bounds.Height;

            _class.Home.imgDisplayVr.Left = Screen.PrimaryScreen.Bounds.Width / 2;
            _class.Home.imgDisplayVr.Top = 0;
            _class.Home.imgDisplayVr.Height = Screen.PrimaryScreen.Bounds.Height;
            _class.Home.imgDisplayVr.Width = Screen.PrimaryScreen.Bounds.Width / 2;
        }

        public void ConfigureVideoCapture()
        {
            _class.BaseClass.VideoCapture.InitialzeCapture();
            _class.BaseClass.VideoCapture.RunGraph();
        }

    }
}
