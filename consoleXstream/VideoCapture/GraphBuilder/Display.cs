﻿using System;
using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class Display
    {
        public Display(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Setup()
        {
            try
            {
                var videoHandle = _class.FrmMain.imgDisplay.Handle;
                _class.Graph.VideoWindow.put_Owner(videoHandle);
                    _class.Graph.VideoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);

                if (_class.Graph.VideoWindow != null)
                    _class.Graph.VideoWindow.SetWindowPosition(0, 0, _class.FrmMain.imgDisplay.Width, _class.FrmMain.imgDisplay.Height);

                _class.Graph.VideoWindow.put_Visible(OABool.True);
                
                if (!_class.System.IsVr) return;
                var videoHandle2 = _class.FrmMain.imgDisplayVr.Handle;
                _class.Graph.VideoWindowVr.put_Owner(videoHandle2);
                _class.Graph.VideoWindowVr.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);

                if (_class.Graph.VideoWindow != null)
                    _class.Graph.VideoWindow.SetWindowPosition(0, _class.System.VrVerticalOffset,
                        _class.FrmMain.imgDisplay.Width,
                        _class.FrmMain.imgDisplay.Height - (_class.System.VrVerticalOffset*2));

                if (_class.Graph.VideoWindowVr != null)
                    _class.Graph.VideoWindowVr.SetWindowPosition(0, _class.System.VrVerticalOffset,
                        _class.FrmMain.imgDisplayVr.Width,
                        _class.FrmMain.imgDisplayVr.Height - (_class.System.VrVerticalOffset*2));

                _class.Graph.VideoWindowVr.put_Visible(OABool.True);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
