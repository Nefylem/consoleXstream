using System.Collections.Generic;
using DirectShowLib;

namespace consoleXstream.VideoCapture.Analyse
{
    class Audio
    {
        public Audio(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        public List<string> Devices; 
        public int Output = -1;

        public void Find()
        {
            Devices = new List<string>();
            _class.Debug.Log("[0] Find audio devices");
            Devices = new List<string>();

            var devObject = DsDevice.GetDevicesOfCat(FilterCategory.AudioRendererCategory);

            for (var intCount = 0; intCount < devObject.Length; intCount++)
            {
                if (Devices.IndexOf(devObject[intCount].Name) != -1) continue;
                Devices.Add(devObject[intCount].Name);
                _class.Debug.Log("->" + devObject[intCount].Name);
                //If nothing set, assume this
                if (Output != -1) continue;
                if (devObject[intCount].Name == "Default WaveOut Device")
                    Output = intCount;
            }

            _class.Debug.Log("");
        }

    }
}
