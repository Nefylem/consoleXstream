using System;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.CronusPlus
{
    class Define
    {
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
        public delegate byte GcapiLoadPtr();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate byte GcapiIsconnectedPtr();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate uint GcapiGettimevalPtr();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate uint GcapiGetfwverPtr();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate byte GcapiWritePtr(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate byte GcapiWriteExPtr(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate byte GcapiWriterefPtr(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GcapiCalcpresstimePtr(byte time);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GcapiUnloadPtr();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate IntPtr GcapiReadCmPtr([In, Out] ref GcapiReportControllermax report);

        public GcapiLoadPtr Load = null;
        public GcapiIsconnectedPtr IsConnected = null;
        public GcapiGettimevalPtr GetTimeVal = null;
        public GcapiGetfwverPtr GetFwVer = null;
        public GcapiWritePtr Write = null;
        public GcapiWriteExPtr WriteEx = null;
        public GcapiWriterefPtr WriteRef = null;
        public GcapiReadCmPtr Read = null;
        public GcapiCalcpresstimePtr CalcPressTime = null;
        public GcapiUnloadPtr Unload = null;
    }
}
