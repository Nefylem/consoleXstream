using System.Collections.Generic;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.MainMenu
{
    public class Create
    {
        private readonly Classes _class;

        public Create(Classes inClass) { _class = inClass; }

        private int NewMenu()
        {
            _class.Data.Items.Add(new List<DisplayItem>());
            return _class.Data.Items.Count - 1;
        }

        private void AddMenuItem(int index, string command, string title)
        {
            _class.Data.Items[index].Add(new DisplayItem());

            var id = _class.Data.Items[index].Count - 1;

            _class.Data.Items[index][id].Command = command;
            _class.Data.Items[index][id].Display = title;
        }

        public void Menu()
        {
            _class.Data.Items.Clear();
            //Console
            int intIndex = NewMenu();
            AddMenuItem(intIndex, "Console Select", "Connect To");
            AddMenuItem(intIndex, "Save Profile", "Save Profile");
            AddMenuItem(intIndex, "Power On", "Power On");
            AddMenuItem(intIndex, "VR", "VR");

            //Video
            intIndex = NewMenu();
            AddMenuItem(intIndex, "Video Input", "Video Input");
            AddMenuItem(intIndex, "Video Device", "Device");
            AddMenuItem(intIndex, "Video Settings", "Capture \nSettings");
            AddMenuItem(intIndex, "Video Display", "Display");

            //Controller
            intIndex = NewMenu();
            AddMenuItem(intIndex, "Controller Output", "Output");
            AddMenuItem(intIndex, "Device", "Controller \n Settings");
            AddMenuItem(intIndex, "Remap", "Remap \nInputs");
            AddMenuItem(intIndex, "Profile", " Input \nProfile");

            //System
            intIndex = NewMenu();
            AddMenuItem(intIndex, "System Status", "Status");
            AddMenuItem(intIndex, "Config", "Configuration");
            AddMenuItem(intIndex, "Exit", "Exit");

            _class.User.Selected = "Console Select";
        }

    }
}
