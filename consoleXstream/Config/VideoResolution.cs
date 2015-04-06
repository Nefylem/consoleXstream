using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using consoleXstream.Home;

namespace consoleXstream.Config
{
    public class VideoResolution
    {
        public VideoResolution(BaseClass baseClass) { _class = baseClass; }
        private readonly BaseClass _class;


        [DllImport("User32.dll")]
        private static extern bool EnumDisplayDevices(IntPtr lpDevice, int iDevNum, ref DisplayDevice lpDisplayDevice, int dwFlags);

        [DllImport("User32.dll")]
        private static extern bool EnumDisplaySettings(string devName, int modeNum, ref Devmode devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref Devmode devMode, int flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct DisplayDevice
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;

            public DisplayDevice(int flags)
            {
                cb = 0;
                StateFlags = flags;
                DeviceName = new string((char)32, 32);
                DeviceString = new string((char)32, 128);
                DeviceID = new string((char)32, 128);
                DeviceKey = new string((char)32, 128);
                cb = Marshal.SizeOf(this);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Devmode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmUnusedPadding;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }


        private readonly List<string> _listDisplayDevices = new List<string>();
        private readonly List<string> _listRefreshRate = new List<string>();
        private readonly List<string> _listResolution = new List<string>();
        private readonly List<string> _listDisplay = new List<string>();

        public string GetVideoCard(int cardId)
        {
            if (_listDisplayDevices.Count == 0) EnumDevices();

            return cardId < _listDisplayDevices.Count ? _listDisplayDevices[cardId] : _listDisplayDevices[0];
        }

        public string GetRefreshRate(int cardId)
        {
            var current = GetDevmode(cardId, -1);
            return current.dmDisplayFrequency + " Hz";
        }

        public string GetDisplayResolution(int cardId)
        {
            var current = GetDevmode(cardId, -1);
            return current.dmPelsWidth + " x " + current.dmPelsHeight;
        }

        public void SetDisplayResolution(int cardId, string res)
        {
            if (_listDisplay.Count == 0)
                EnumModes(cardId);

            if (res == null) return;
            res = res.ToLower();

            var setRes = -1;
            for (var count = 0; count < _listDisplay.Count; count++)
            {
                if (res != _listDisplay[count].ToLower()) continue;
                setRes = count;
                break;
            }

            var setDev = GetDevmode(cardId, setRes);
            if (!(setDev.dmBitsPerPel != 0 & setDev.dmPelsWidth != 0 & setDev.dmPelsHeight != 0)) return;
            ChangeDisplaySettings(ref setDev, 0);
            //_class.Home.ChangeDisplayRes();
        }

        public List<string> ListDisplayResolutions(int cardId)
        {
            if (_listResolution.Count == 0)
                EnumModes(cardId);

            return _listResolution;
        }

        public List<string> ListDisplayRefresh(int cardId)
        {
            if (_listRefreshRate.Count == 0)
                EnumModes(cardId);

            return _listRefreshRate;
        }

        private void EnumDevices()
        { 
            var d = new DisplayDevice(0);

            var devNum = 0;
            bool result;
            do
            {
                result = EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0);

                if (result)
                {
                    var item = devNum +
                        ". " + d.DeviceString.Trim();
                    if ((d.StateFlags & 4) != 0) item += " - main";
                    _listDisplayDevices.Add(item);
                }
                devNum++;
            } while (result);
        }

        private void EnumModes(int devNum)
        {
            _listRefreshRate.Clear();

            var devName = GetDeviceName(devNum);
            var devMode = new Devmode();
            var modeNum = 0;
            bool result;
            do
            {
                result = EnumDisplaySettings(devName,
                    modeNum, ref devMode);

                if (result)
                {
                    AddRefresh(devMode);
                    AddResolution(devMode);
                    AddDisplay(devMode);
                }
                modeNum++;
            } while (result);
        }

        private static string GetDeviceName(int devNum)
        {
            var d = new DisplayDevice(0);
            var result = EnumDisplayDevices(IntPtr.Zero,
                devNum, ref d, 0);
            return (result ? d.DeviceName.Trim() : "#error#");
        }

        private void AddRefresh(Devmode devMode)
        {
            var set = devMode.dmDisplayFrequency + " Hz";
            if (_listRefreshRate.IndexOf(set) == -1)
                _listRefreshRate.Add(set);
        }

        private void AddResolution(Devmode devMode)
        {
            var set = devMode.dmPelsWidth + " x " + devMode.dmPelsHeight;

            if (_listResolution.IndexOf(set) == -1)
                _listResolution.Add(set);
        }

        private void AddDisplay(Devmode devMode)
        {
            var set = devMode.dmPelsWidth + " x " + devMode.dmPelsHeight + " - " + devMode.dmDisplayFrequency + " Hz";
            _listDisplay.Add(set);
        }

        private static Devmode GetDevmode(int devNum, int modeNum)
        { 
            var devMode = new Devmode();
            var devName = GetDeviceName(devNum);
            EnumDisplaySettings(devName, modeNum, ref devMode);
            return devMode;
        }
    }
}
