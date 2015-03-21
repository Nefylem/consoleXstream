namespace consoleXstream.Menu.SubMenuOptions
{
    public class Display
    {
        public Display(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void ChangeResolution(string resolution)
        {
            _class.System.strCurrentResolution = resolution;
            resolution = resolution.ToLower();
            if (resolution == "resolution")
                return;

            var listRes = _class.VideoCapture.GetVideoResolution();
            for (var count = 0; count < listRes.Count; count++)
            {
                if (resolution != listRes[count].ToLower())
                    continue;

                _class.VideoCapture.SetVideoResolution(count);
                _class.VideoCapture.RunGraph();

                _class.System.AddData("CaptureResolution", resolution);

                break;
            }
        }

        private void ListDisplayRefresh()
        {
            _class.Data.ClearButtons();

            _class.Shutter.Scroll = 0;

            _class.Data.SubItems.Clear();
            _class.Data.Checked.Clear();
            _class.Shutter.Error = "";
            _class.Shutter.Explain = "";
            _class.User.Menu = "videorefresh";

            var listDisplayRef = _class.System.getDisplayRefresh();
            var currentRef = _class.System.getRefreshRate().ToLower();

            foreach (var title in listDisplayRef)
            {
                if (title.ToLower() == currentRef)
                    _class.SubAction.AddSubItem(title, title, true);
                else
                    _class.SubAction.AddSubItem(title, title);
            }

            SelectSubItem();
        }

        private void ListDisplayResolution()
        {
            _class.Data.ClearButtons();

            _class.Shutter.Scroll = 0;

            _class.Data.SubItems.Clear();
            _class.Data.Checked.Clear();
            _class.Shutter.Error = "";
            _class.Shutter.Explain = "";
            _class.User.Menu = "videoresolution";

            var listDisplayRes = _class.System.getDisplayResolutionList();
            var currentRes = _class.System.getResolution().ToLower();

            foreach (var title in listDisplayRes)
            {
                if (title.ToLower() == currentRes)
                    _class.SubAction.AddSubItem(title, title, true);
                else
                    _class.SubAction.AddSubItem(title, title);
            }

            SelectSubItem();
        }

        public void ChangeVideoResolution(string command)
        {
            if (command.ToLower() == "resolution")
                return;

            _class.System.setDisplayResolution(command);
            _class.Data.Checked.Clear();
            _class.Data.Checked.Add(command);
            
            _class.DisplayMenu.PositionMenu();
        }
    
        public void ChangeVideoRefresh(string command)
        {
            if (command.ToLower() == "refresh")
                return;

            _class.System.setDisplayRefresh(command);
            _class.Data.Checked.Clear();
            _class.Data.Checked.Add(command);

            _class.DisplayMenu.PositionMenu();
        }

        private void ChangeAutoRes()
        {
            if (_class.Data.Checked.IndexOf("Auto Set") > -1)
                _class.Data.Checked.RemoveAt(_class.Data.Checked.IndexOf("Auto Set"));
            else
                _class.Data.Checked.Add("Auto Set");

            _class.System.setAutoChangeDisplay();
        }

        private void ChangeStayOnTop()
        {
            if (_class.Data.Checked.IndexOf("Stay On Top") > -1)
                _class.Data.Checked.RemoveAt(_class.Data.Checked.IndexOf("Stay On Top"));
            else
                _class.Data.Checked.Add("Stay On Top");

            _class.System.setStayOnTop();
        }

        public void ChangeVideoDisplay(string command)
        {
            command = command.ToLower();
            if (command == "autoset") ChangeAutoRes();
            if (command == "resolution") ListDisplayResolution();
            if (command == "refresh") ListDisplayRefresh();
            if (command == "stayontop") ChangeStayOnTop();
        }

        private void SelectSubItem()
        {
            if (_class.Data.SubItems.Count > 0)
            {
                _class.User.SubSelected = _class.Data.SubItems[0].Command;
            }
        }


    }
}
