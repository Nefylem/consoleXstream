using System;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.TitanOne
{
    public class Define
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        public struct GcapiConstants
        {
            public const int GcapiInputTotal = 30;
            public const int GcapiOutputTotal = 36;
        }
        public struct GcapiStatus
        {
            public byte Value; // Current value - Range: [-100 ~ 100] %
            public byte PrevValue; // Previous value - Range: [-100 ~ 100] %
            public int PressTv; // Time marker for the button press event
        }
        public struct GcapiReportTitanone
        {
            public byte Console; // Receives values established by the #defines CONSOLE_*
            public byte Controller; // Values from #defines CONTROLLER_* and EXTENSION_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Led; // Four LED - #defines LED_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]        //XBOX ONE TRIGGER RUMBLE
            public byte[] Rumble; // Two rumbles - Range: [0 ~ 100] %
            public byte BatteryLevel; // Battery level - Range: [0 ~ 10] 0 = empty, 10 = full

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GcapiConstants.GcapiInputTotal, ArraySubType = UnmanagedType.Struct)]
            public GcapiStatus[] Input;
        }

        public struct GcapiReportControllermax
        {
            public byte Console; // Receives values established by the #defines CONSOLE_*
            public byte Controller; // Values from #defines CONTROLLER_* and EXTENSION_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Led; // Four LED - #defines LED_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Rumble; // Two rumbles - Range: [0 ~ 100] %
            public byte BatteryLevel; // Battery level - Range: [0 ~ 10] 0 = empty, 10 = full

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GcapiConstants.GcapiInputTotal, ArraySubType = UnmanagedType.Struct)]
            public GcapiStatus[] Input;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte GcapiLoad();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate byte GcapiLoaddevice(ushort devPid);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate byte GcapiIsconnected();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint GcapiGettimeval();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint GcapiGetfwver();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate byte GcapiWrite(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate byte GcapiWriteEx(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate byte GcapiWriteref(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GcapiCalcpresstime(byte time);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GcapiUnload();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GcapiReadCm([In, Out] ref GcapiReportControllermax gcapiReport);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GcapiReadTo([In, Out] ref GcapiReportTitanone gcapiReport);

        public GcapiLoad Load = null;
        public GcapiLoaddevice LoadDevice = null;
        public GcapiIsconnected IsConnected = null;
        public GcapiGettimeval GetTimeVal = null;
        public GcapiGetfwver GetFwVer = null;
        public GcapiWrite Write = null;
        public GcapiWriteEx WriteEx = null;
        public GcapiWriteref WriteRef = null;
        public GcapiReadTo Read = null;
        public GcapiReadCm ReadCm = null;
        public GcapiCalcpresstime CalcPressTime = null;
        public GcapiUnload Unload = null;

        public enum DevPid
        {
            Any = 0x000,
            ControllerMax = 0x001,
            Cronus = 0x002,
            TitanOne = 0x003
        };

        public enum ApiMethod
        {
            Single = 0,
            Multi = 1
        };

    }
}
