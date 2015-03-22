using System;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Define
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        public struct GcmapiConstants
        {
            public const int GcapiInputTotal = 30;
            public const int GcapiOutputTotal = 36;
        }

        public struct GcmapiStatus
        {
            public byte Value; // Current value - Range: [-100 ~ 100] %
            public byte PrevValue; // Previous value - Range: [-100 ~ 100] %
            public int PressTv; // Time marker for the button press event
        }

        public struct GcmapiReport
        {
            public byte Console; // Receives values established by the #defines CONSOLE_*
            public byte Controller; // Values from #defines CONTROLLER_* and EXTENSION_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Led; // Four LED - #defines LED_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]        //XBOX ONE TRIGGER RUMBLE
            public byte[] Rumble; // Two rumbles - Range: [0 ~ 100] %
            public byte BatteryLevel; // Battery level - Range: [0 ~ 10] 0 = empty, 10 = full

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GcmapiConstants.GcapiInputTotal, ArraySubType = UnmanagedType.Struct)]
            public GcmapiStatus[] Input;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate byte GCMAPI_LOAD();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GCMAPI_UNLOAD();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GCMAPI_CONNECT(ushort devPID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GCMAPI_GETSERIALNUMBER(int devId);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GCMAPI_ISCONNECTED(int m);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int GCMAPI_WRITE(int device, byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr GCMAPI_READ(int device, [In, Out] ref GcmapiReport gcapiReport);
        /*
typedef  uint8_t (_stdcall *GCMAPI_Load)();
typedef     void (_stdcall *GCMAPI_Unload)();
typedef      int (_stdcall *GCMAPI_Connect)(uint16_t);
typedef  uint8_t (_stdcall *GCMAPI_IsConnected)(int);
typedef uint8_t* (_stdcall *GCMAPI_GetSerialNumber)(int);
typedef uint16_t (_stdcall *GCMAPI_GetFWVer)(int);
typedef  uint8_t (_stdcall *GCMAPI_Read)(int, GCAPI_REPORT *);
typedef  uint8_t (_stdcall *GCMAPI_ReadEX)(int, GPPAPI_REPORT_EX *);
typedef  uint8_t (_stdcall *GCMAPI_Write)(int, int8_t *);
typedef  uint8_t (_stdcall *GCMAPI_WriteEX)(int, uint8_t *, uint8_t);
typedef uint32_t (_stdcall *GCMAPI_GetTimeVal)();
typedef uint32_t (_stdcall *GCMAPI_CalcPressTime)(uint32_t);
        */
        public GCMAPI_LOAD GcmapiLoad = null;
        public GCMAPI_UNLOAD GcmapiUnload = null;
        public GCMAPI_CONNECT GcmapiConnect = null;
        public GCMAPI_GETSERIALNUMBER GcmapiGetSerialNumber = null;
        public GCMAPI_ISCONNECTED GcmapiIsConnected = null;
        public GCMAPI_WRITE GcmapiWrite = null;
        public GCMAPI_READ GcmapiRead = null;
    }
}
