using System.Windows.Forms;

namespace consoleXstream.Home
{
    public class System
    {
        public System(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Exit()
        {
            var system = _class.BaseClass.System;
            var controllerMax = _class.BaseClass.ControllerMax;
            var videoCapture = _class.BaseClass.VideoCapture;

            if (system != null)
            {
                system.IsOverrideOnExit = true;
                system.SetInitialDisplay();

                if (system.UseControllerMax) controllerMax.closeControllerMaxInterface();

                if (system.UseInternalCapture) videoCapture.CloseGraph();
            }
            Application.Exit();
        }

    }
}
