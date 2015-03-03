using System.Drawing;
using consoleXstream.Config;
using consoleXstream.DrawGui;
using consoleXstream.Menu.Data;

namespace consoleXstream.Menu.SubMenu
{
    public class Menu
    {

        public Menu(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        public void Draw()
        {
            if (_class.Shutter.Open && _class.Shutter.Height > 0)
            {
                var intPinAudio = -1;
                var intPinVideo = -1;

                if (_class.System.boolInternalCapture && _class.VideoCapture._xBar != null && _class.User.Menu == "video input")
                {
                    _class.VideoCapture._xBar.get_IsRoutedTo(0, out intPinVideo);
                    _class.VideoCapture._xBar.get_IsRoutedTo(1, out intPinAudio);
                }

                var bmpShutter = new Bitmap(_class.DisplayMenu.Width - 20, _class.Shutter.Height);

                _class.DrawGui.drawImage(bmpShutter, 0, 0, Properties.Resources.imgSubMenu);

                if (_class.Shutter.Error.Length == 0)
                {
                    var intX = 0;
                    if (_class.Data.SubItems.Count < 4)
                    {
                        var intSetWidth = _class.Data.SubItems.Count * (_class.Var.CellWidth + 5);
                        intX = ((_class.DisplayMenu.Width - 20) / 2) - (intSetWidth / 2);
                    }
                    for (var intCount = _class.Shutter.Scroll; intCount < _class.Data.SubItems.Count; intCount++)
                    {
                        var displayRect = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 4);                  //Outline
                        var displayRectText = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 24);             //Text
                        var buttonRect = new Rectangle(8 + intX, _class.Shutter.Start, _class.Var.CellWidth, _class.Var.CellHeight);    //Mouse over location

                        _class.Button.Create(buttonRect, _class.Data.SubItems[intCount].Command);

                        if (_class.User.SubSelected == _class.Data.SubItems[intCount].Command)
                            _class.DrawGui.drawImage(bmpShutter, displayRect, Properties.Resources.imgSubGlow);

                        var boolDrawCheck = false;

                        if (_class.System.boolInternalCapture && _class.VideoCapture._xBar != null && _class.User.Menu == "video input")
                            if (intCount == intPinVideo || intCount == intPinAudio)
                                boolDrawCheck = true;

                        if (_class.Data.Checked.IndexOf(_class.Data.SubItems[intCount].Display) > -1 || boolDrawCheck)
                            _class.DrawGui.drawImage(bmpShutter, intX + _class.Var.CellWidth - 40, 17, 25, 25, Properties.Resources.imgTick);

                        /*
                        if (_boolShowPreview)
                            DrawPreviewWindow(bmpShutter, displayRect, _class.Data.SubItems[intCount].Display);
                        */
                        if (_class.Data.SubItems[intCount].IsFolder)
                        {
                            //draw folder option (three dots)
                            _class.DrawGui.drawImage(bmpShutter, intX + _class.Var.CellWidth - 50, 90, 60, 60, Properties.Resources.ThreeDots);
                            var displayRectTextOption = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 60);
                            var displayRectTextDisplay = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 34);
                            _class.DrawGui.centerText(bmpShutter, displayRectTextOption, _class.Data.SubItems[intCount].DisplayOption);
                            _class.DrawGui.centerText(bmpShutter, displayRectTextDisplay, _class.Data.SubItems[intCount].Display);
                        }
                        else
                        {
                            if (_class.Data.SubItems[intCount].DisplayOption.Length == 0)
                                _class.DrawGui.centerText(bmpShutter, displayRectText, _class.Data.SubItems[intCount].Display);
                            else
                            {
                                var displayRectTextOption = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 60);
                                //var displayRectTextDisplay = new Rectangle(intX, 2, _class.Var.CellWidth, _class.Var.CellHeight - 34);
                                _class.DrawGui.centerText(bmpShutter, displayRectText, _class.Data.SubItems[intCount].Display);
                                _class.DrawGui.centerText(bmpShutter, displayRectTextOption, _class.Data.SubItems[intCount].DisplayOption);
                            }
                        }
                        //Check for changing information
                        if (_class.Data.SubItems[intCount].ActiveWatcher.Length > 0)
                            _class.Data.SubItems[intCount].DisplayOption = _class.SubAction.FindSubOption(_class.Data.SubItems[intCount].Display);

                        intX += _class.Var.CellWidth + 5;
                    }
                }
                else
                {
                    _class.DrawGui.setCenter();
                    _class.DrawGui.setFontSize(24f);
                    _class.DrawGui.centerText(bmpShutter, new Rectangle(0, 0, _class.DisplayMenu.Width - 20, _class.Shutter.Height), _class.Shutter.Error);
                    if (_class.Shutter.Explain.Length > 0)
                    {
                        _class.DrawGui.setFontSize(14f);
                        _class.DrawGui.setCenterBottom(true);
                        _class.DrawGui.centerText(bmpShutter, new Rectangle(0, 0, _class.DisplayMenu.Width - 20, _class.Shutter.Height - 20), _class.Shutter.Explain);
                    }
                }

                _class.DrawGui.drawImage(new Rectangle(8, _class.Shutter.Start, 581, _class.Shutter.Height), bmpShutter);
            }
            _class.DrawGui.setFontSize(12f);
        }

    }
}
