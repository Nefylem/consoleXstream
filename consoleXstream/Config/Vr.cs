using System;

namespace consoleXstream.Config
{
    public class Vr
    {
        public Vr(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public int HeightOffset;
        public int WidthOffset;
        public int VideoHeightOffset;
        public int VideoWidthOffset;

        public void ChangeVrVideo()
        {
            _class.System.IsVr = !_class.System.IsVr;
            _class.Main.ChangeVr();
            _class.VideoCapture.RunGraph();
            _class.Set.Add("VR_Video", _class.System.IsVr.ToString());
        }

        public void SetWidth(string width)
        {
            try
            {
                int temp = Convert.ToInt32(width.Trim());
                WidthOffset = temp;
            }
            catch (Exception)
            {
                //Ignored
            }
        }

        public void SetHeight(string height)
        {
            try
            {
                int temp = Convert.ToInt32(height.Trim());
                HeightOffset = temp;
            }
            catch (Exception)
            {
                //Ignored
            }            
        }

        public void SetOffsetWidth(string width)
        {
            try
            {
                int temp = Convert.ToInt32(width.Trim());
                VideoWidthOffset = temp;
                _class.BaseClass.Home.MoveVrDisplay();
            }
            catch (Exception)
            {
                //Ignored
            }
        }

        public void SetOffsetHeight(string height)
        {
            try
            {
                int temp = Convert.ToInt32(height.Trim());
                VideoHeightOffset = temp;
                _class.BaseClass.Home.MoveVrDisplay();
            }
            catch (Exception)
            {
                //Ignored
            }
        }

    }
}
