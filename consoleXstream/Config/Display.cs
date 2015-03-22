using System;
using System.Collections.Generic;

namespace consoleXstream.Config
{
    public class Display
    {
        public Display(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void SetRefresh(string refresh)
        {
            if (string.IsNullOrEmpty(_class.System.DisplayResolution))
                _class.System.DisplayResolution = _class.VideoResolution.GetDisplayResolution(_class.System.GraphicsCardId);

            string set = _class.System.DisplayResolution + " - " + refresh;

            _class.VideoResolution.SetDisplayResolution(_class.System.GraphicsCardId, set);
            _class.Main.ChangeDisplayRes();

            _class.Set.Add("RefreshRate", refresh);

            _class.System.RefreshRate = refresh;
        }

        public void SetResolution(string video)
        {
            if (string.IsNullOrEmpty(_class.System.RefreshRate))
                _class.System.RefreshRate = _class.VideoResolution.GetRefreshRate(_class.System.GraphicsCardId);

            string set = video + " - " + _class.System.RefreshRate;

            _class.VideoResolution.SetDisplayResolution(_class.System.GraphicsCardId, set);

            _class.Set.Add("Resolution", video);

            _class.System.DisplayResolution = video;
        }

        public string GetGraphicsCard()
        {
            if (string.IsNullOrEmpty(_class.System.GraphicsCard))
                _class.System.GraphicsCard = _class.VideoResolution.GetVideoCard(_class.System.GraphicsCardId);

            return _class.System.GraphicsCard;
        }

        public string GetRefreshRate()
        {
            if (string.IsNullOrEmpty(_class.System.RefreshRate))
                _class.System.RefreshRate = _class.VideoResolution.GetRefreshRate(_class.System.GraphicsCardId);

            return _class.System.RefreshRate;
        }

        public string GetResolution()
        {
            if (string.IsNullOrEmpty(_class.System.DisplayResolution))
                _class.System.DisplayResolution = _class.VideoResolution.GetDisplayResolution(_class.System.GraphicsCardId);

            return _class.System.DisplayResolution;
        }

        public string GetVolume()
        {
            return "100%";
        }

        public List<string> GetDisplayResolutionList()
        {
            List<string> listRes = _class.VideoResolution.ListDisplayResolutions(_class.System.GraphicsCardId);

            return listRes;
        }

        public List<string> GetDisplayRefresh()
        {
            List<string> listRefresh = _class.VideoResolution.ListDisplayRefresh(_class.System.GraphicsCardId);

            return listRefresh;
        }

        public void AutoChangeRes(int height)
        {
            var res = GetResolution();
            if (res.IndexOf('x') > -1)
            {
                var check = res.Substring(res.IndexOf("x ", StringComparison.Ordinal) + 1).Trim();
                //_class.System.Debug("VideoSetResolution.log", "Check: [" + check + "]" + " equals setRes: [" + height + "]");

                if (check == height.ToString())
                {
                    //_class.System.Debug("VideoSetResolution.log", "resolution already set");
                    return;
                }
            }

            //_class.System.Debug("VideoSetResolution.log", "AutoSetResolution: " + IsAutoSetDisplayResolution);

            if (!_class.System.IsAutoSetDisplayResolution) return;
            List<string> listRes = _class.VideoResolution.ListDisplayResolutions(_class.System.GraphicsCardId);

            var set = "";

            //_class.System.Debug("VideoSetResolution.log", "listDisplayResolution: " + listRes);
            foreach (string t in listRes)
            {
                string title = t;

                if (title.IndexOf("x ", StringComparison.Ordinal) <= -1) continue;
                //_class.System.Debug("VideoSetResolution.log", "res: " + title);
                title = title.Substring(title.IndexOf("x ", StringComparison.Ordinal) + 1).Trim();
                //_class.System.Debug("VideoSetResolution.log", "height: " + height + " equals vcheight: " + height);

                if (title != height.ToString()) continue;
                set = t;
                //_class.System.Debug("VideoSetResolution.log", "set: " + set + " [] " + count);
                break;
            }


            if (String.Equals(set, GetResolution(), StringComparison.CurrentCultureIgnoreCase)) return;

            //_class.System.Debug("VideoSetResolution.log", "getResolution: " + getResolution());                    

            _class.Display.SetResolution(set);
        }

        public void GetInitialDisplay() { _class.System._initialDisplay = GetResolution(); }
        public void SetInitialDisplay() { _class.VideoResolution.SetDisplayResolution(_class.System.GraphicsCardId, _class.System._initialDisplay); }

        public void SetAutoChangeDisplay()
        {
            _class.System.IsAutoSetDisplayResolution = !_class.System.IsAutoSetDisplayResolution;

            _class.Set.Add("AutoResolution", _class.System.IsAutoSetDisplayResolution.ToString());
        }

        public void SetStayOnTop()
        {
            _class.System.IsStayOnTop = !_class.System.IsStayOnTop;
            _class.Set.Add("StayOnTop", _class.System.IsStayOnTop.ToString());
        }

        public void ChangeResolution(string resolution)
        {
            _class.System.strSetResolution = resolution;
        }
    }
}
