using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace consoleXstream.Menu.SubSelectMenu
{
    public class Menu
    {
        public Menu(Classes classes)
        {
            _class = classes;
        }

        private readonly Classes _class;

        public void Draw()
        {
            var bmpMenu = new Bitmap(Properties.Resources.imgSubSelect.Width + 50,
                Properties.Resources.imgSubSelect.Height);
            _class.DrawGui.drawImage(bmpMenu, 0, 0, 101, 90, Properties.Resources.imgSubSelect);
            _class.DrawGui.drawText(bmpMenu, 10, 10, _class.SubSelectVar.ListData.Count.ToString());
            int start = 10;
            for (int count = 0; count < _class.SubSelectVar.ListData.Count; count++)
            {
                _class.DrawGui.drawText(bmpMenu, 50, start, _class.SubSelectVar.ListData[count]);
                start += 25;
            }
            _class.DrawGui.drawImage(
                new Rectangle(8, 250, Properties.Resources.imgSubSelect.Width + 50,
                    Properties.Resources.imgSubSelect.Height), bmpMenu);
        }
    }
}
     