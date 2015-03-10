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
    }
}
