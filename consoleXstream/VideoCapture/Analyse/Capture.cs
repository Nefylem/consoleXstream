using System;
using System.Collections.Generic;
using System.Linq;
using DirectShowLib;

namespace consoleXstream.VideoCapture.Analyse
{
    class Capture
    {
        public Capture(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        public List<string> Device { get; private set; }
        public List<string> Display { get; private set; }

        public int CurrentDevice;

        public void Find()
        {
            Device = new List<string>();
            Display = new List<string>();

            _class.Debug.Log("[0] Listing video capture devices");
            var devObjects = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            if (devObjects.Length > 0)
            {
                foreach (var title in devObjects.Select(t => t.Name))
                {
                    Device.Add(title);
                    var devId = 1;

                    if (Display.IndexOf(title) > -1)
                    {
                        var boolFound = true;
                        while (boolFound)
                        {
                            string strSet = title + " (" + devId + ")";
                            _class.Debug.Log(strSet);
                            if (Display.IndexOf(strSet) == -1)
                            {
                                boolFound = false;
                                Display.Add(strSet);
                            }
                            else
                            {
                                devId++;
                            }
                        }
                    }
                    else
                        Display.Add(title);

                    _class.Debug.Log("->" + title);
                }
            }
            else
            {
                Device.Add("*NF*");
                Display.Add("");
                _class.Debug.Log("[Err] No capture devices found");
            }
            _class.Debug.Log("");
            _class.VideoCapture.UpdateVideoCaptureList(Display);
        }

        public void Set(string title)
        {
            _class.Debug.Log("[0] Looking for " + title);
            var index = -1;
            _class.Debug.Log(Display.Count.ToString());
            for (var count = 0; count < Display.Count; count++)
            {
                _class.Debug.Log(Display[count] + "{}" + title);
                if (title.ToLower() == Display[count].ToLower())
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
