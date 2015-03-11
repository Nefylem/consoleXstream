using System;
using System.Collections.Generic;
using System.Linq;
using DirectShowLib;

namespace consoleXstream.VideoCapture.Analyse
{
    public class Capture
    {
        public Capture(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

       // public List<string> Device { get; private set; }
       // public List<string> Display { get; private set; }

        public int CurrentDevice;

        public void Find()
        {
            if (_class.Var.VideoCaptureDevice == null)
                _class.Var.VideoCaptureDevice = new List<string>();

            var device = new List<string>();

            _class.Debug.Log("[0] Listing video capture devices");
            var devObjects = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            if (devObjects.Length > 0)
            {
                foreach (var title in devObjects.Select(t => t.Name))
                {
                    device.Add(title);
                    var devId = 1;

                    if (_class.Var.VideoCaptureDevice.IndexOf(title) > -1)
                    {
                        var boolFound = true;
                        while (boolFound)
                        {
                            string strSet = title + " (" + devId + ")";
                            _class.Debug.Log(strSet);
                            if (_class.Var.VideoCaptureDevice.IndexOf(strSet) == -1)
                            {
                                boolFound = false;
                                _class.Var.VideoCaptureDevice.Add(strSet);
                            }
                            else
                            {
                                devId++;
                            }
                        }
                    }
                    else
                        _class.Var.VideoCaptureDevice.Add(title);

                    _class.Debug.Log("->" + title);
                }
            }
            else
            {
                device.Add("*NF*");
                _class.Var.VideoCaptureDevice.Add("");
                _class.Debug.Log("[Err] No capture devices found");
            }
            _class.Debug.Log("");
        }

        public void Set(string title)
        {
            _class.Debug.Log("[0] Looking for " + title);
            var index = -1;
            _class.Debug.Log(_class.Var.VideoCaptureDevice.Count.ToString());
            for (var count = 0; count < _class.Var.VideoCaptureDevice.Count; count++)
            {
                _class.Debug.Log(_class.Var.VideoCaptureDevice[count] + "{}" + title);
                if (title.ToLower() == _class.Var.VideoCaptureDevice[count].ToLower())
                    index = count;
            }
            _class.Debug.Log("[0] DeviceID: " + index);
            if (index > -1)
                _class.Capture.CurrentDevice = index;
            else
                _class.Debug.Log("[0] [ERR] Cant find " + title + " VCD");
        }

    }
}
