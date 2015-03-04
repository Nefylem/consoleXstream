using System.Drawing;

namespace consoleXstream.Menu
{
    class PreviewVideo
    {
        //TODO: create second device by crossbar
        private void AddNewVideoCapture(string strVideoDevice)
        {
            /*
            var intIndex = -1;
            for (var intCount = 0; intCount < _previewVideo.Count; intCount++)
            {
                if (String.Equals(strVideoDevice, _previewVideo[intCount].Title, StringComparison.CurrentCultureIgnoreCase))
                    intIndex = intCount;
            }

            //If the device is already running in a graph, dont create another one.
            //if (strVideoDevice == form1.videoCapture) intIndex = -1;

            if (intIndex != -1)
                return;

            _previewVideo.Add(new PreviewItem());
            intIndex = _previewVideo.Count - 1;
            _previewVideo[intIndex].Title = strVideoDevice;
            //Add crossbar here
            _previewVideo[intIndex].VideoPreview = new VideoCapture.VideoCapture(_form1);
            _previewVideo[intIndex].VideoPreview.getSystemHandle(_system);
            _previewVideo[intIndex].VideoPreview.initialzeCapture();
            _previewVideo[intIndex].VideoPreview.setVideoCaptureDevice(strVideoDevice);
            //Find video device crossbar
            //Send crossbar command here
            //Dynamically check display bound values
            _previewVideo[intIndex].Display = new Bitmap(150, 110);
            var displayRect = new Rectangle(0, 0, 150, 110);
            _drawGui.setCenterTop(true);
            _drawGui.drawTextJustify(_previewVideo[intIndex].Display, displayRect, "No preview");

            _previewVideo[intIndex].VideoPreview.setPreviewWindow(true);
            _previewVideo[intIndex].VideoPreview.setPreviewWindowHandle(_previewVideo[intIndex].Display.GetHbitmap());
            _previewVideo[intIndex].VideoPreview.setPreviewWindowBounds(new Point(150, 110));
            _previewVideo[intIndex].VideoPreview.runGraph();

            _boolShowPreview = true;
             */
        }

        private void DrawPreviewWindow(Bitmap bmpShutter, Rectangle displayRect, string strCommand)
        {
            /*
            var intIndex = -1;
            for (var intCount = 0; intCount < _previewVideo.Count; intCount++)
            {
                if (String.Equals(_previewVideo[intCount].Title, strCommand, StringComparison.CurrentCultureIgnoreCase))
                    intIndex = intCount;
            }

            if (intIndex > -1)
                _drawGui.drawImage(bmpShutter, displayRect, _previewVideo[intIndex].Display);
             */
        }

    }
}
