using System;
using System.Diagnostics;
using System.IO;

namespace consoleXstream.Home
{
    public class Development
    {
        public Development(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private const string DevPath = @"\\gamer-pc\shield\consoleXstream\";

        public void DeleteLogs()
        {
            if (File.Exists("system.log")) File.Delete("system.log");
            if (File.Exists("video.log")) File.Delete("video.log");
            if (File.Exists("titanOne.log")) File.Delete("titanOne.log");
            if (File.Exists("controllerMax.log")) File.Delete("controllerMax.log");
            if (File.Exists("connectTo.log")) File.Delete("connectTo.log");
            if (File.Exists("listAll.log")) File.Delete("listAll.log");
            if (File.Exists("VideoResolution.log")) File.Delete("VideoResolution.log");
            if (File.Exists("video.log")) File.Delete("video.log");
            if (File.Exists("menu.log")) File.Delete("Menu.log");
        }

        public void Setup()
        {
            if (Debugger.IsAttached)
                _class.Var.IsIde = true;

            CheckDevelopment();

            DeleteLogs();
        }

        //Copies files to test environment 
        private void CheckDevelopment()
        {
            if (!_class.Var.IsIde) return;
            if (!Directory.Exists(DevPath)) return;
            try
            {
                if (File.Exists(DevPath + @"\consoleXstream.exe"))
                    File.Delete(DevPath + @"\consoleXstream.exe");

                File.Copy("consoleXstream.exe", DevPath + @"\consoleXstream.exe");
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}
