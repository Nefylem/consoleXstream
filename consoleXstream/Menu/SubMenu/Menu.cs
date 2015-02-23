using System.Drawing;
using consoleXstream.Config;
using consoleXstream.DrawGui;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenu
{
    class Menu
    {
        private ShowMenu _menu;
        private Action _action;
        private ButtonItem _button;
        private Interaction _data;
        private DrawGraph _drawGui;
        private User _user;
        private Shutter _shutter;
        private Variables _var;
        private Configuration _system;
        private VideoCapture.VideoCapture _videoCapture;

        public void GetActionHandle(Action action) { _action = action; }
        public void GetButtonHandle(ButtonItem button) { _button = button; }
        public void GetDataHandle(Interaction data) { _data = data; }
        public void GetDrawGuiHandle(DrawGraph drawGui) { _drawGui = drawGui; }
        public void GetUserHandle(User user) { _user = user; }
        public void GetShutterHandle(Shutter shutter) { _shutter = shutter; }
        public void GetVariableHandle(Variables var) { _var = var; }
        public void GetSystemHandle(Configuration system) { _system = system; }
        public void GetVideoCaptureHandle(VideoCapture.VideoCapture videoCapture) { _videoCapture = videoCapture; }
        public void GetFormMenuHandle(ShowMenu menu) { _menu = menu; }

        public void Draw()
        {
            if (_shutter.Open)
            {
                var intPinAudio = -1;
                var intPinVideo = -1;

                if (_system.boolInternalCapture && _videoCapture._xBar != null && _user.Menu == "video input")
                {
                    _videoCapture._xBar.get_IsRoutedTo(0, out intPinVideo);
                    _videoCapture._xBar.get_IsRoutedTo(1, out intPinAudio);
                }

                var bmpShutter = new Bitmap(_menu.Width - 20, _shutter.Height);

                _drawGui.drawImage(bmpShutter, 0, 0, Properties.Resources.imgSubMenu);

                if (_shutter.Error.Length == 0)
                {
                    var intX = 0;
                    if (_data.SubItems.Count < 4)
                    {
                        var intSetWidth = _data.SubItems.Count * (_var.CellWidth + 5);
                        intX = ((_menu.Width - 20) / 2) - (intSetWidth / 2);
                    }
                    for (var intCount = _shutter.Scroll; intCount < _data.SubItems.Count; intCount++)
                    {
                        var displayRect = new Rectangle(intX, 2, _var.CellWidth, _var.CellHeight - 4);                  //Outline
                        var displayRectText = new Rectangle(intX, 2, _var.CellWidth, _var.CellHeight - 24);             //Text
                        var buttonRect = new Rectangle(8 + intX, _shutter.Start, _var.CellWidth, _var.CellHeight);    //Mouse over location

                        _button.Create(buttonRect, _data.SubItems[intCount].Command);

                        if (_user.SubSelected == _data.SubItems[intCount].Command)
                            _drawGui.drawImage(bmpShutter, displayRect, Properties.Resources.imgSubGlow);

                        var boolDrawCheck = false;

                        if (_system.boolInternalCapture && _videoCapture._xBar != null && _user.Menu == "video input")
                            if (intCount == intPinVideo || intCount == intPinAudio)
                                boolDrawCheck = true;

                        if (_data.Checked.IndexOf(_data.SubItems[intCount].Display) > -1 || boolDrawCheck)
                            _drawGui.drawImage(bmpShutter, intX + _var.CellWidth - 40, 17, 25, 25, Properties.Resources.imgTick);

                        /*
                        if (_boolShowPreview)
                            DrawPreviewWindow(bmpShutter, displayRect, _data.SubItems[intCount].Display);
                        */
                        if (_data.SubItems[intCount].IsFolder)
                        {
                            //draw folder option (three dots)
                            _drawGui.drawImage(bmpShutter, intX + _var.CellWidth - 50, 90, 60, 60, Properties.Resources.ThreeDots);
                            var displayRectTextOption = new Rectangle(intX, 2, _var.CellWidth, _var.CellHeight - 60);
                            var displayRectTextDisplay = new Rectangle(intX, 2, _var.CellWidth, _var.CellHeight - 34);
                            _drawGui.centerText(bmpShutter, displayRectTextOption, _data.SubItems[intCount].DisplayOption);
                            _drawGui.centerText(bmpShutter, displayRectTextDisplay, _data.SubItems[intCount].Display);
                        }
                        else
                        {
                            if (_data.SubItems[intCount].DisplayOption.Length == 0)
                                _drawGui.centerText(bmpShutter, displayRectText, _data.SubItems[intCount].Display);
                            else
                            {
                                var displayRectTextOption = new Rectangle(intX, 2, _var.CellWidth, _var.CellHeight - 60);
                                //var displayRectTextDisplay = new Rectangle(intX, 2, _var.CellWidth, _var.CellHeight - 34);
                                _drawGui.centerText(bmpShutter, displayRectText, _data.SubItems[intCount].Display);
                                _drawGui.centerText(bmpShutter, displayRectTextOption, _data.SubItems[intCount].DisplayOption);
                            }
                        }
                        //Check for changing information
                        if (_data.SubItems[intCount].ActiveWatcher.Length > 0)
                            _data.SubItems[intCount].DisplayOption = _action.FindSubOption(_data.SubItems[intCount].Display);

                        intX += _var.CellWidth + 5;
                    }
                }
                else
                {
                    _drawGui.setCenter();
                    _drawGui.setFontSize(24f);
                    _drawGui.centerText(bmpShutter, new Rectangle(0, 0, _menu.Width - 20, _shutter.Height), _shutter.Error);
                    if (_shutter.Explain.Length > 0)
                    {
                        _drawGui.setFontSize(14f);
                        _drawGui.setCenterBottom(true);
                        _drawGui.centerText(bmpShutter, new Rectangle(0, 0, _menu.Width - 20, _shutter.Height - 20), _shutter.Explain);
                    }
                }

                _drawGui.drawImage(new Rectangle(8, _shutter.Start, 581, _shutter.Height), bmpShutter);
            }
            _drawGui.setFontSize(12f);
        }

    }
}
