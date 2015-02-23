using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace consoleXstream.DrawGui
{
    public class DrawGraph
    {
        private Bitmap _imgDisplay;
        private Graphics _graph;

        private readonly Color _colorFont = Color.White;
        private readonly Color _colorFontMinor = Color.White;
        private readonly Color _colOutline = Color.Black;

        private readonly Brush _fontBrush = Brushes.Black;
        private const string FontName = "Tahoma";

        private float _floatFontSize = 12;
        private const int OutlineSize = 4;

        private bool _boolTextCenterVerticalLower = false;
        private bool _boolTextCenterVerticalUpper = false;
        private bool _boolTextCenterHorizontalNear = false;
        private bool _boolTextCenterHorizontalFar = false;
 
        private bool _boolOutline = false;
        private class OutlineText
        {
            public Bitmap bmpImage;
            public string strWrite;
            public string strFontName;
            public float floatSize;
        }
        private readonly List<OutlineText> _listOutline = new List<OutlineText>(); 

        public void ClearGraph(Bitmap imgBitmap)
        {
            _imgDisplay = imgBitmap;
            _graph = Graphics.FromImage(imgBitmap);
        }

        public void drawImage(int intX, int intY, Bitmap bmpDraw)
        {
            _graph.DrawImage(bmpDraw, new Point(intX, intY));
        }

        private void drawImage(int intX, int intY, int intZoom, Bitmap bmpIcon)
        {
            double dblSizeX = bmpIcon.Width;
            dblSizeX /= 100;
            dblSizeX *= intZoom;

            double dblSizeY = bmpIcon.Height;
            dblSizeY /= 100;
            dblSizeY *= intZoom;

            int intNewX = (int)dblSizeX - bmpIcon.Width;
            int intNewY = (int)dblSizeY - bmpIcon.Height;

            _graph.DrawImage(bmpIcon, new Rectangle(intX - (intNewX / 2), intY - (intNewY / 2), (int)dblSizeX, (int)dblSizeY));
        }

        public void drawImage(int intX, int intY, int intZoomX, int intZoomY, Bitmap bmpIcon)
        {
            double dblSizeX = bmpIcon.Width;
            dblSizeX /= 100;
            dblSizeX *= intZoomX;

            double dblSizeY = bmpIcon.Height;
            dblSizeY /= 100;
            dblSizeY *= intZoomY;

            int intNewX = (int)dblSizeX - bmpIcon.Width;
            int intNewY = (int)dblSizeY - bmpIcon.Height;

            _graph.DrawImage(bmpIcon, new Rectangle(intX, intY, (int)dblSizeX, (int)dblSizeY));
        }

        public void drawImage(Rectangle newRect, Bitmap bmpIcon)
        {
            _graph.DrawImage(bmpIcon, newRect);
        }

        public void centerText(Rectangle newRect, int intWrite) { centerText(newRect, intWrite.ToString()); }
        public void centerText(Rectangle newRect, float floatWrite) { centerText(newRect, floatWrite.ToString()); }
        public void centerText(Rectangle newRect, bool boolWrite) 
        {
            if (boolWrite)
                centerText(newRect, "true"); 
            else
                centerText(newRect, "false");
        }

        public void centerText(Rectangle newRect, string strWrite)
        {
            strWrite = strWrite.Trim();
            Font fontSet = new Font(FontName, _floatFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF textSize = _graph.MeasureString(strWrite, fontSet);

            int intX = (int)(newRect.Left + (newRect.Width / 2) - (textSize.Width / 3));
            int intY = (int)(newRect.Top + (newRect.Height / 2) - (textSize.Height / 2));

            if (_boolTextCenterVerticalUpper)
                intY = (int)(newRect.Top - textSize.Height);

            if (_boolTextCenterVerticalLower)
                intY = (int)(newRect.Bottom - textSize.Height);
            
            drawText(intX, intY, strWrite);
        }

        public void drawText(int intX, int intY, int intWrite) { drawText(intX, intY, intWrite.ToString()); }
        public void drawText(int intX, int intY, float floatWrite) { drawText(intX, intY, floatWrite.ToString()); }
        public void drawText(int intX, int intY, bool boolWrite) 
        {
            if (boolWrite)
                drawText(intX, intY, "true");
            else
                drawText(intX, intY, "false");
        }

        public void drawText(int intX, int intY, string strWrite)
        {
            if (_boolOutline)
            {
                outlineText(intX, intY, strWrite);
                return;
            }

            _graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            _graph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            SizeF textSize = _graph.MeasureString(strWrite, new Font(FontName, _floatFontSize));

            StringFormat setFormat = new StringFormat();
            setFormat.Alignment = StringAlignment.Center;

            RectangleF textRect = new RectangleF(intX, intY, intX + textSize.Width, intY + textSize.Height);

            _graph.DrawString(strWrite, new Font(FontName, _floatFontSize), _fontBrush, textRect);
        }

        private void outlineText(int intX, int intY, string strWrite)
        {
            if (_listOutline.Count > 0)
            {                
                for (int intCount = 0; intCount < _listOutline.Count; intCount++)
                {
                    bool boolFound = true;
                    if (_listOutline[intCount].strWrite != strWrite) boolFound = false;
                    if (_listOutline[intCount].strFontName != FontName) boolFound = false;
                    if (_listOutline[intCount].floatSize != _floatFontSize) boolFound = false;

                    if (boolFound)
                    {
                        drawImage(intX, intY, _listOutline[intCount].bmpImage);
                        return;
                    }
                }
            }
            
            Font fontSet = new Font(FontName, _floatFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF textSize = _graph.MeasureString(strWrite, fontSet);

            Bitmap bmpFontoutline = new Bitmap((int)textSize.Width + 20, (int)textSize.Height + 20);
            Graphics graph = Graphics.FromImage(bmpFontoutline); 

            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Near;

            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            Pen penOutline = new Pen(_colOutline, OutlineSize);
            penOutline.LineJoin = LineJoin.Round;

            Rectangle rectText = new Rectangle(intX, intY, (int)(intX + textSize.Width), (int)(intY + textSize.Height));
            LinearGradientBrush gradBrush = new LinearGradientBrush(rectText, _colorFont, _colorFontMinor, 90);

            RectangleF textRect = new RectangleF(0, 0, textSize.Width, textSize.Height);

            Brush fontBrush = Brushes.Black;

            GraphicsPath graphPath = new GraphicsPath();

            graphPath.AddString(strWrite, fontSet.FontFamily, (int)fontSet.Style, _floatFontSize, textRect, strFormat);

            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graph.DrawPath(penOutline, graphPath);
            graph.FillPath(gradBrush, graphPath);
            
            //Clean up
            graphPath.Dispose();
            gradBrush.Dispose();
            fontSet.Dispose();
            strFormat.Dispose();

            drawImage(intX, intY, bmpFontoutline);

            //Store the image
            _listOutline.Add(new OutlineText());
            
            int intIndex = _listOutline.Count - 1;
            _listOutline[intIndex].strWrite = strWrite;
            _listOutline[intIndex].strFontName = FontName;
            _listOutline[intIndex].floatSize = _floatFontSize;
            _listOutline[intIndex].bmpImage = bmpFontoutline;
        }

        public void setOutline(bool boolOutline)
        {
            _boolOutline = boolOutline;
        }

        public void setFontSize(float fontSize)
        {
            _floatFontSize = fontSize;
        }

        public void setCenter()
        {
            _boolTextCenterVerticalLower = false;
            _boolTextCenterVerticalUpper = false;
            _boolTextCenterHorizontalFar = false;
            _boolTextCenterHorizontalNear = false;
        }

        public void setCenterNear(bool boolSet)
        {
            _boolTextCenterHorizontalNear = boolSet;
            _boolTextCenterHorizontalFar = !boolSet;
        }

        public void setCenterFar(bool boolSet)
        {
            _boolTextCenterHorizontalNear = !boolSet;
            _boolTextCenterHorizontalFar = boolSet;
        }

        public void setCenterTop(bool boolSet)
        {
            _boolTextCenterVerticalUpper = boolSet;
            _boolTextCenterVerticalLower = !boolSet;
        }

        public void setCenterBottom(bool boolSet)
        {
            _boolTextCenterVerticalUpper = !boolSet;
            _boolTextCenterVerticalLower = boolSet;
        }

        public Bitmap drawGraph()
        {
            _graph.Flush();
            _graph.Dispose();

            return _imgDisplay;
        }

        //Processing for sub images. Leave the main canvas alone
        public void drawImage(Bitmap bmpSource, int intX, int intY, Bitmap bmpDraw)
        {
            Graphics graph = Graphics.FromImage(bmpSource); 
            graph.DrawImage(bmpDraw, new Point(intX, intY));
            graph.Dispose();
        }

        private void drawImage(Bitmap bmpSource, int intX, int intY, int intZoom, Bitmap bmpIcon)
        {
            Graphics _graph = Graphics.FromImage(bmpSource);

            double dblSizeX = bmpIcon.Width;
            dblSizeX /= 100;
            dblSizeX *= intZoom;

            double dblSizeY = bmpIcon.Height;
            dblSizeY /= 100;
            dblSizeY *= intZoom;

            int intNewX = (int)dblSizeX - bmpIcon.Width;
            int intNewY = (int)dblSizeY - bmpIcon.Height;

            _graph.DrawImage(bmpIcon, new Rectangle(intX - (intNewX / 2), intY - (intNewY / 2), (int)dblSizeX, (int)dblSizeY));
        }

        public void drawImage(Bitmap bmpSource, int intX, int intY, int intZoomX, int intZoomY, Bitmap bmpIcon)
        {
            Graphics _graph = Graphics.FromImage(bmpSource);

            double dblSizeX = bmpIcon.Width;
            dblSizeX /= 100;
            dblSizeX *= intZoomX;

            double dblSizeY = bmpIcon.Height;
            dblSizeY /= 100;
            dblSizeY *= intZoomY;

            int intNewX = (int)dblSizeX - bmpIcon.Width;
            int intNewY = (int)dblSizeY - bmpIcon.Height;

            _graph.DrawImage(bmpIcon, new Rectangle(intX, intY, (int)dblSizeX, (int)dblSizeY));
        }

        public void drawImage(Bitmap bmpSource, Rectangle newRect, Bitmap bmpIcon)
        {
            Graphics _graph = Graphics.FromImage(bmpSource);
            _graph.DrawImage(bmpIcon, newRect);
        }

        public void centerText(Bitmap bmpSource, Rectangle newRect, int intWrite) { centerText(bmpSource, newRect, intWrite.ToString()); }
        public void centerText(Bitmap bmpSource, Rectangle newRect, float floatWrite) { centerText(bmpSource, newRect, floatWrite.ToString()); }
        public void centerText(Bitmap bmpSource, Rectangle newRect, bool boolWrite)
        {
            if (boolWrite)
                centerText(bmpSource, newRect, "true");
            else
                centerText(bmpSource, newRect, "false");
        }

        public void centerText(Bitmap bmpSource, Rectangle newRect, string strWrite)
        {
            strWrite = strWrite.Trim();
            Font fontSet = new Font(FontName, _floatFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF textSize = _graph.MeasureString(strWrite, fontSet);
            if (textSize.Width > newRect.Width)
            {
                drawTextJustify(bmpSource, newRect, strWrite);
            }
            else
            {
                int intX = (int)(newRect.Left + (newRect.Width / 2) - (textSize.Width / 2));
                int intY = (int)(newRect.Top + (newRect.Height / 2) - (textSize.Height / 2));

                if (_boolTextCenterVerticalUpper)
                    intY = (int)(newRect.Top - textSize.Height);

                if (_boolTextCenterVerticalLower)
                    intY = (int)(newRect.Bottom - textSize.Height);

                drawText(bmpSource, intX, intY, strWrite);
            }
        }

        public void drawText(Bitmap bmpSource, int intX, int intY, int intWrite) { drawText(bmpSource, intX, intY, intWrite.ToString()); }
        public void drawText(Bitmap bmpSource, int intX, int intY, float floatWrite) { drawText(bmpSource, intX, intY, floatWrite.ToString()); }
        public void drawText(Bitmap bmpSource, int intX, int intY, bool boolWrite)
        {
            if (boolWrite)
                drawText(bmpSource, intX, intY, "true");
            else
                drawText(bmpSource, intX, intY, "false");
        }

        public void drawText(Bitmap bmpSource, int intX, int intY, string strWrite)
        {
            Graphics _graph = Graphics.FromImage(bmpSource);

            if (_boolOutline)
            {
                outlineText(bmpSource, intX, intY, strWrite);
                return;
            }

            _graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            _graph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            SizeF textSize = _graph.MeasureString(strWrite, new Font(FontName, _floatFontSize));

            StringFormat setFormat = new StringFormat();
            setFormat.Alignment = StringAlignment.Center;

            RectangleF textRect = new RectangleF(intX, intY, intX + textSize.Width, intY + textSize.Height);

            _graph.DrawString(strWrite, new Font(FontName, _floatFontSize), _fontBrush, textRect);
        }

        public void drawTextJustify(Bitmap bmpSource, Rectangle newRect, string strWrite)
        {
            Rectangle checkRect = new Rectangle(newRect.Left + 8, newRect.Top + 50, newRect.Width - 16, newRect.Height - 30);

            Graphics _graph = Graphics.FromImage(bmpSource);

            if (_boolOutline)
            {
                outlineTextJustify(bmpSource, newRect, strWrite);
                return;
            }

            _graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            _graph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            SizeF textSize = _graph.MeasureString(strWrite, new Font(FontName, _floatFontSize));

            StringFormat setFormat = new StringFormat();
            setFormat.Alignment = StringAlignment.Center;

            _graph.DrawString(strWrite, new Font(FontName, _floatFontSize), _fontBrush, checkRect, setFormat);
        }

        private void outlineText(Bitmap bmpSource, int intX, int intY, string strWrite)
        {
            if (_listOutline.Count > 0)
            {
                for (int intCount = 0; intCount < _listOutline.Count; intCount++)
                {
                    bool boolFound = true;
                    if (_listOutline[intCount].strWrite != strWrite) boolFound = false;
                    if (_listOutline[intCount].strFontName != FontName) boolFound = false;
                    if (_listOutline[intCount].floatSize != _floatFontSize) boolFound = false;

                    if (boolFound)
                    {
                        drawImage(bmpSource, intX, intY, _listOutline[intCount].bmpImage);
                        return;
                    }
                }
            }


            Font fontSet = new Font(FontName, _floatFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF textSize = _graph.MeasureString(strWrite, fontSet);

            Bitmap bmpFontoutline = new Bitmap((int)textSize.Width + 20, (int)textSize.Height + 20);
            Graphics graph = Graphics.FromImage(bmpFontoutline);

            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Near;

            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            Pen penOutline = new Pen(_colOutline, OutlineSize);
            penOutline.LineJoin = LineJoin.Round;

            Rectangle rectText = new Rectangle(intX, intY, (int)(intX + textSize.Width), (int)(intY + textSize.Height));
            LinearGradientBrush gradBrush = new LinearGradientBrush(rectText, _colorFont, _colorFontMinor, 90);

            RectangleF textRect = new RectangleF(0, 0, textSize.Width, textSize.Height);

            Brush fontBrush = Brushes.Black;

            GraphicsPath graphPath = new GraphicsPath();

            graphPath.AddString(strWrite, fontSet.FontFamily, (int)fontSet.Style, _floatFontSize, textRect, strFormat);

            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graph.DrawPath(penOutline, graphPath);
            graph.FillPath(gradBrush, graphPath);

            //Clean up
            graphPath.Dispose();
            gradBrush.Dispose();
            fontSet.Dispose();
            strFormat.Dispose();

            drawImage(bmpSource, intX, intY, bmpFontoutline);

            //Store the image
            _listOutline.Add(new OutlineText());

            int intIndex = _listOutline.Count - 1;
            _listOutline[intIndex].strWrite = strWrite;
            _listOutline[intIndex].strFontName = FontName;
            _listOutline[intIndex].floatSize = _floatFontSize;
            _listOutline[intIndex].bmpImage = bmpFontoutline;
        }

        private void outlineTextJustify(Bitmap bmpSource, Rectangle newRect, string strWrite)
        {
            if (_listOutline.Count > 0)
            {
                for (int intCount = 0; intCount < _listOutline.Count; intCount++)
                {
                    bool boolFound = true;
                    if (_listOutline[intCount].strWrite != strWrite) boolFound = false;
                    if (_listOutline[intCount].strFontName != FontName) boolFound = false;
                    if (_listOutline[intCount].floatSize != _floatFontSize) boolFound = false;

                    if (boolFound)
                    {
                        drawImage(bmpSource, newRect.Left, newRect.Top, _listOutline[intCount].bmpImage);
                        return;
                    }
                }
            }

            Font fontSet = new Font(FontName, _floatFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF textSize = _graph.MeasureString(strWrite, fontSet);

            //Bitmap bmpFontoutline = new Bitmap((int)textSize.Width + 20, (int)textSize.Height + 20);
            Bitmap bmpFontoutline = new Bitmap(500, 150);
            Graphics graph = Graphics.FromImage(bmpFontoutline);

            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;

            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            Pen penOutline = new Pen(_colOutline, OutlineSize);
            penOutline.LineJoin = LineJoin.Round;

            LinearGradientBrush gradBrush = new LinearGradientBrush(newRect, _colorFont, _colorFontMinor, 90);

            Rectangle checkRect = new Rectangle(newRect.Left + 8, newRect.Top, newRect.Width - 16, newRect.Height);
            Rectangle showRect = new Rectangle(0, 0, newRect.Width - 20, 150);

            Brush fontBrush = Brushes.Black;

            GraphicsPath graphPath = new GraphicsPath();

            graphPath.AddString(strWrite, fontSet.FontFamily, (int)fontSet.Style, _floatFontSize, showRect, strFormat);

            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graph.DrawPath(penOutline, graphPath);
            graph.FillPath(gradBrush, graphPath);

            //Clean up
            graphPath.Dispose();
            gradBrush.Dispose();
            fontSet.Dispose();
            strFormat.Dispose();

            drawImage(bmpSource, newRect.Left + 10, newRect.Top, bmpFontoutline);
        }
    }
}
