using System.Collections.Generic;

namespace consoleXstream.Menu.Data
{
    public class Interaction
    {
        public List<ButtonItem> Buttons = new List<ButtonItem>();
        public List<ButtonItem> InactiveButtons = new List<ButtonItem>();
        
        public List<List<DisplayItem>> Items = new List<List<DisplayItem>>();
        public List<DisplayItem> SubItems = new List<DisplayItem>();

        public List<string> Checked = new List<string>();

        public List<int> Row = new List<int>();

        public void ClearButtons()
        {
            InactiveButtons.Clear();
            Buttons.Clear();
        }

    }
}
