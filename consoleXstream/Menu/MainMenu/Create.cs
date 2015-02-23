using System.Collections.Generic;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.MainMenu
{
    class Create
    {
        private Interaction _data;
        private User _user;

        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetUserHandle(User user) { _user = user; }

        private int NewMenu()
        {
            _data.Items.Add(new List<DisplayItem>());
            return _data.Items.Count - 1;
        }

        private void AddMenuItem(int index, string command, string title)
        {
            _data.Items[index].Add(new DisplayItem());

            var id = _data.Items[index].Count - 1;

            _data.Items[index][id].Command = command;
            _data.Items[index][id].Display = title;
        }

        public void Menu()
        {
            _data.Items.Clear();
            //Console
            int intIndex = NewMenu();
            AddMenuItem(intIndex, "Console Select", "Connect To");
            AddMenuItem(intIndex, "Save Profile", "Save Profile");
            AddMenuItem(intIndex, "Power On", "Power On");
            AddMenuItem(intIndex, "Graph", "Graph");

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

            _user.Selected = "Console Select";
        }

    }
}
