using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace consoleXstream.Menu.SubMenuOptions
{
    class Display
    {
        private void ListDisplayRefresh()
        {
            _data.ClearButtons();

            _shutter.Scroll = 0;

            _data.SubItems.Clear();
            _data.Checked.Clear();
            _shutter.Error = "";
            _shutter.Explain = "";
            _user.Menu = "videorefresh";

            var listDisplayRef = _system.getDisplayRefresh();
            var currentRef = _system.getRefreshRate().ToLower();

            foreach (var title in listDisplayRef)
            {
                if (title.ToLower() == currentRef)
                    _subAction.AddSubItem(title, title, true);
                else
                    _subAction.AddSubItem(title, title);
            }

            _action.SelectSubItem();
        }

        private void ListDisplayResolution()
        {
            _data.ClearButtons();

            _shutter.Scroll = 0;

            _data.SubItems.Clear();
            _data.Checked.Clear();
            _shutter.Error = "";
            _shutter.Explain = "";
            _user.Menu = "videoresolution";

            var listDisplayRes = _system.getDisplayResolutionList();
            var currentRes = _system.getResolution().ToLower();

            foreach (var title in listDisplayRes)
            {
                if (title.ToLower() == currentRes)
                    _subAction.AddSubItem(title, title, true);
                else
                    _subAction.AddSubItem(title, title);
            }

            _action.SelectSubItem();
            //Set scroll position
            //Set most used to front of list
        }

        private void ChangeVideoResolution(string command)
        {
            if (command.ToLower() == "resolution")
                return;

            _system.setDisplayResolution(command);
            _data.Checked.Clear();
            _data.Checked.Add(command);

            //save set res
            Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (Properties.Resources.imgMainMenu.Width / 2);
            Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (Properties.Resources.imgMainMenu.Height / 2);
        }

        private void ChangeVideoRefresh(string command)
        {
            if (command.ToLower() == "refresh")
                return;

            _system.setDisplayRefresh(command);
            _data.Checked.Clear();
            _data.Checked.Add(command);

            Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (Properties.Resources.imgMainMenu.Width / 2);
            Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (Properties.Resources.imgMainMenu.Height / 2);
        }

        private void ChangeAutoRes()
        {
            if (_data.Checked.IndexOf("Auto Set") > -1)
                _data.Checked.RemoveAt(_data.Checked.IndexOf("Auto Set"));
            else
                _data.Checked.Add("Auto Set");

            _system.setAutoChangeDisplay();
        }

        private void ChangeStayOnTop()
        {
            if (_data.Checked.IndexOf("Stay On Top") > -1)
                _data.Checked.RemoveAt(_data.Checked.IndexOf("Stay On Top"));
            else
                _data.Checked.Add("Stay On Top");

            _system.setStayOnTop();
        }

    }
}
