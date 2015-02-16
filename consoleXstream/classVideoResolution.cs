using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace consoleXstream
{
    public class classVideoResolution
    {
        [DllImport("User32.dll")]
        private static extern bool EnumDisplayDevices(IntPtr lpDevice, int iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

        [DllImport("User32.dll")]
        private static extern bool EnumDisplaySettings(string devName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAY_DEVICE
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

            public DISPLAY_DEVICE(int flags)
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
        public struct DEVMODE
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

        private Form1 frmMain;

        public classVideoResolution(Form1 mainForm) { frmMain = mainForm; }

        private List<string> _listDisplayDevices = new List<string>();
        private List<string> _listRefreshRate = new List<string>();
        private List<string> _listResolution = new List<string>();
        private List<string> _listDisplay = new List<string>();

        public string getVideoCard(int cardID)
        {
            if (_listDisplayDevices.Count == 0) EnumDevices();

            if (cardID < _listDisplayDevices.Count)
                return _listDisplayDevices[cardID];
            else
                return _listDisplayDevices[0];
        }

        public string getRefreshRate(int cardID)
        {
            //EnumModes(cardID);

            DEVMODE current = GetDevmode(cardID, -1);

            return current.dmDisplayFrequency.ToString() + " Hz";
        }

        public string getDisplayResolution(int cardID)
        {
            DEVMODE current = GetDevmode(cardID, -1);
            return current.dmPelsWidth.ToString() + " x " + current.dmPelsHeight.ToString();
        }

        public void setDisplayResolution(int cardID, string res)
        {
            if (_listDisplay.Count == 0)
                EnumModes(cardID);

            if (res != null)
            {
                res = res.ToLower();

                var setRes = -1;
                for (int count = 0; count < _listDisplay.Count; count++)
                {
                    if (res == _listDisplay[count].ToLower())
                    {
                        setRes = count;
                        break;
                    }
                }

                DEVMODE setDev = GetDevmode(cardID, setRes);
                if (setDev.dmBitsPerPel != 0 & setDev.dmPelsWidth != 0 & setDev.dmPelsHeight != 0)
                {
                    ChangeDisplaySettings(ref setDev, 0);
                    frmMain.changeDisplayRes();
                }
            }
        }

        public List<string> listDisplayResolutions(int cardID)
        {
            if (_listResolution.Count == 0)
                EnumModes(cardID);

            return _listResolution;
        }

        public List<string> listDisplayRefresh(int cardID)
        {
            if (_listRefreshRate.Count == 0)
                EnumModes(cardID);

            return _listRefreshRate;
        }

        private void EnumDevices()
        { 
            DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);

            int devNum = 0;
            bool result;
            do
            {
                result = EnumDisplayDevices(IntPtr.Zero, devNum, ref d, 0);

                if (result)
                {
                    string item = devNum.ToString() +
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

            string devName = GetDeviceName(devNum);
            DEVMODE devMode = new DEVMODE();
            int modeNum = 0;
            bool result = true;
            do
            {
                result = EnumDisplaySettings(devName,
                    modeNum, ref devMode);

                if (result)
                {
                    addRefresh(devMode);
                    addResolution(devMode);
                    addDisplay(devMode);
                }
                modeNum++;
            } while (result);
        }

        private string GetDeviceName(int devNum)
        {
            DISPLAY_DEVICE d = new DISPLAY_DEVICE(0);
            bool result = EnumDisplayDevices(IntPtr.Zero,
                devNum, ref d, 0);
            return (result ? d.DeviceName.Trim() : "#error#");
        }

        private void addRefresh(DEVMODE devMode)
        {
            string set = devMode.dmDisplayFrequency.ToString() + " Hz";
            if (_listRefreshRate.IndexOf(set) == -1)
                _listRefreshRate.Add(set);
        }

        private void addResolution(DEVMODE devMode)
        {
            string set = devMode.dmPelsWidth.ToString() + " x " + devMode.dmPelsHeight.ToString();

            if (_listResolution.IndexOf(set) == -1)
                _listResolution.Add(set);
        }

        private void addDisplay(DEVMODE devMode)
        {
            string set = devMode.dmPelsWidth.ToString() + " x " + devMode.dmPelsHeight.ToString() + " - " + devMode.dmDisplayFrequency.ToString() + " Hz";
            _listDisplay.Add(set);
        }

        private string DevmodeToString(DEVMODE devMode)
        {
            return devMode.dmPelsWidth.ToString() +
                " x " + devMode.dmPelsHeight.ToString() +
                ", " + devMode.dmBitsPerPel.ToString() +
                " bits, " +
                devMode.dmDisplayFrequency.ToString() + " Hz";
        }

        private DEVMODE GetDevmode(int devNum, int modeNum)
        { //populates DEVMODE for the specified device and mode
            DEVMODE devMode = new DEVMODE();
            string devName = GetDeviceName(devNum);
            EnumDisplaySettings(devName, modeNum, ref devMode);
            return devMode;
        }
    }
}
