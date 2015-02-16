using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace gcapiTitanOne
{
    public class classTitanOne
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        #region TitanOne Definitions
        public struct GCAPI_CONSTANTS
        {
            public const int GCAPI_INPUT_TOTAL = 30;
            public const int GCAPI_OUTPUT_TOTAL = 36;
        }
        public struct GCAPI_STATUS
        {
            public byte value; // Current value - Range: [-100 ~ 100] %
            public byte prev_value; // Previous value - Range: [-100 ~ 100] %
            public int press_tv; // Time marker for the button press event
        }
        public struct GCAPI_REPORT_TITANONE
        {
            public byte console; // Receives values established by the #defines CONSOLE_*
            public byte controller; // Values from #defines CONTROLLER_* and EXTENSION_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] led; // Four LED - #defines LED_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]        //XBOX ONE TRIGGER RUMBLE
            public byte[] rumble; // Two rumbles - Range: [0 ~ 100] %
            public byte battery_level; // Battery level - Range: [0 ~ 10] 0 = empty, 10 = full

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = GCAPI_CONSTANTS.GCAPI_INPUT_TOTAL, ArraySubType = UnmanagedType.Struct)]
            public GCAPI_STATUS[] input;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate byte GCAPI_LOAD();
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate byte GCAPI_LOADDEVICE(ushort devPID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate byte GCAPI_ISCONNECTED();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint GCAPI_GETTIMEVAL();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint GCAPI_GETFWVER();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate byte GCAPI_WRITE(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate byte GCAPI_WRITE_EX(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate byte GCAPI_WRITEREF(byte[] output);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GCAPI_CALCPRESSTIME(byte time);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GCAPI_UNLOAD();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr GCAPI_READ_TO([In, Out] ref GCAPI_REPORT_TITANONE gcapi_report);

        private GCAPI_LOAD _gcapi_Load = null;
        private GCAPI_LOADDEVICE _gcapi_LoadDevice = null;
        private GCAPI_ISCONNECTED _gcapi_IsConnected = null;
        private GCAPI_GETTIMEVAL _gcapi_GetTimeVal = null;
        private GCAPI_GETFWVER _gcapi_GetFwVer = null;
        private GCAPI_WRITE _gcapi_Write = null;
        private GCAPI_WRITE_EX _gcapi_WriteEx = null;
        private GCAPI_WRITEREF _gcapi_WriteRef = null;
        private GCAPI_READ_TO _gcapi_Read_TO = null;
        private GCAPI_CALCPRESSTIME _gcapi_CalcPressTime = null;
        private GCAPI_UNLOAD _gcapi_Unload = null;
        #endregion

        public enum DevPID
        {
            Any = 0x000,
            ControllerMax = 0x001,
            Cronus = 0x002,
            TitanOne = 0x003
        };
        
    public enum xbox : int
    {
        home = 0,
        back = 1,
        start = 2,
        rightShoulder = 3,
        rightTrigger = 4,
        rightStick = 5,
        leftShoulder = 6,
        leftTrigger = 7,
        leftStick = 8,
        rightX = 9,
        rightY = 10,
        leftX = 11,
        leftY = 12,
        up = 13,
        down = 14,
        left = 15,
        right = 16,
        y = 17,
        b = 18,
        a = 19,
        x = 20,
        accX = 21,      //rotate X. 90 = -25, 180 = 0, 270 = +25, 360 = 0 
        accY = 22,      //shake vertically. +25 (top) to -25 (bottom) 
        accZ = 23,      //tilt up
        gyroX = 24,     
        gyroY = 25,     
        gyroZ = 26,     
        touch = 27,             //touchpad, 100 = on    (works)
        touchX = 28,            //-100 to 100   (left to right)
        touchY = 29             //-100 to 100   (top to bottom)
    }                               

        private string _strTODevice;

        private bool _boolGCAPILoaded = false;

        private DevPID _devId = DevPID.Any;

        public void setTOInterface(DevPID devID)
        {
            _devId = devID;
        }
        
        public void initTitanOne()
        {
            var Dir = Directory.GetCurrentDirectory() + @"\";
            _strTODevice = strDevice;

            if (File.Exists(Dir + strAPI) == false)
                return;

            var ptrDll = LoadLibrary(strDir + strAPI);
            if (ptrDll == IntPtr.Zero) return;

            var ptrLoad = loadExternalFunction(ptrDll, "gcdapi_Load");
            if (ptrLoad == IntPtr.Zero) return; 

            var ptrLoadDevice = loadExternalFunction(ptrDll, "gcdapi_LoadDevice");
            if (ptrLoadDevice == IntPtr.Zero) return; 

            var ptrIsConnected = loadExternalFunction(ptrDll, "gcapi_IsConnected");
            if (ptrIsConnected == IntPtr.Zero) return; 

            var ptrUnload = loadExternalFunction(ptrDll, "gcdapi_Unload");
            if (ptrUnload == IntPtr.Zero) return; 

            var ptrGetTimeVal = loadExternalFunction(ptrDll, "gcapi_GetTimeVal");
            if (ptrGetTimeVal == IntPtr.Zero) return; 

            var ptrGetFwVer = loadExternalFunction(ptrDll, "gcapi_GetFWVer");
            if (ptrGetFwVer == IntPtr.Zero) return; 

            var ptrWrite = loadExternalFunction(ptrDll, "gcapi_Write");
            if (ptrWrite == IntPtr.Zero) return;

            var ptrRead = loadExternalFunction(ptrDll, "gcapi_Read");
            if (ptrRead == IntPtr.Zero) return;

            var ptrWriteEx = loadExternalFunction(ptrDll, "gcapi_WriteEX");
            if (ptrWriteEx == IntPtr.Zero) return; 

            var ptrReadEx = loadExternalFunction(ptrDll, "gcapi_ReadEX");
            if (ptrReadEx == IntPtr.Zero) return; 

            var ptrCalcPressTime = loadExternalFunction(ptrDll, "gcapi_CalcPressTime");
            if (ptrCalcPressTime == IntPtr.Zero) return; 

            try
            {
                _gcapi_Load = (GCAPI_LOAD)Marshal.GetDelegateForFunctionPointer(ptrLoad, typeof(GCAPI_LOAD));
                _gcapi_LoadDevice = (GCAPI_LOADDEVICE)Marshal.GetDelegateForFunctionPointer(ptrLoadDevice, typeof(GCAPI_LOADDEVICE));
                _gcapi_IsConnected = (GCAPI_ISCONNECTED)Marshal.GetDelegateForFunctionPointer(ptrIsConnected, typeof(GCAPI_ISCONNECTED));
                _gcapi_Unload = (GCAPI_UNLOAD)Marshal.GetDelegateForFunctionPointer(ptrUnload, typeof(GCAPI_UNLOAD));
                _gcapi_GetTimeVal = (GCAPI_GETTIMEVAL)Marshal.GetDelegateForFunctionPointer(ptrGetTimeVal, typeof(GCAPI_GETTIMEVAL));
                _gcapi_GetFwVer = (GCAPI_GETFWVER)Marshal.GetDelegateForFunctionPointer(ptrGetFwVer, typeof(GCAPI_GETFWVER));
                _gcapi_Write = (GCAPI_WRITE)Marshal.GetDelegateForFunctionPointer(ptrWrite, typeof(GCAPI_WRITE));
                _gcapi_CalcPressTime = (GCAPI_CALCPRESSTIME)Marshal.GetDelegateForFunctionPointer(ptrCalcPressTime, typeof(GCAPI_CALCPRESSTIME));
                _gcapi_Write = (GCAPI_WRITE)Marshal.GetDelegateForFunctionPointer(ptrWrite, typeof(GCAPI_WRITE));
                _gcapi_WriteEx = (GCAPI_WRITE_EX)Marshal.GetDelegateForFunctionPointer(ptrWriteEx, typeof(GCAPI_WRITE_EX));
                _gcapi_Read_TO = (GCAPI_READ_TO)Marshal.GetDelegateForFunctionPointer(ptrReadEx, typeof(GCAPI_READ_TO));
            }
            catch (Exception ex)
            {
                return;
            }


            if (_gcapi_LoadDevice((ushort)_devId) != 1)
                return;
        }

        //Finds the pointer for the dll function
        private IntPtr loadExternalFunction(IntPtr ptrDll, string strFunction)
        {
            IntPtr ptrFunction = IntPtr.Zero;
            ptrFunction = GetProcAddress(ptrDll, strFunction);
            if (ptrFunction == IntPtr.Zero)
            {
			//Handle fail
            }
            else
            {
			//Ptr load ok
            }
            return ptrFunction;
        }


        public void closeTitanOneInterface()
        {
            if (_gcapi_Unload != null)
                _gcapi_Unload();

            _gcapi_LoadDevice = null;
            _gcapi_Load = null;
            _gcapi_IsConnected = null;
            _gcapi_GetTimeVal = null;
            _gcapi_GetFwVer = null;
            _gcapi_Write = null;
            _gcapi_WriteEx = null;
            _gcapi_WriteRef = null;
            _gcapi_Read_TO = null;
            _gcapi_CalcPressTime = null;
            _gcapi_Unload = null;
        }

        public void checkControllerInput()
        {
            if (!_boolGCAPILoaded)
                return;
            if (_gcapi_IsConnected() == 1)
            {
                byte[] output = new byte[36];

                if (_controls.DPad.Left) { output[(int)xbox.left] = Convert.ToByte(100); }
                if (_controls.DPad.Right) { output[(int)xbox.right] = Convert.ToByte(100); }
                if (_controls.DPad.Up) { output[(int)xbox.up] = Convert.ToByte(100); }
                if (_controls.DPad.Down) { output[(int)xbox.down] = Convert.ToByte(100); }

                if (_controls.Buttons.A) { output[(int)xbox.a] = Convert.ToByte(100); }
                if (_controls.Buttons.B) { output[(int)xbox.b] = Convert.ToByte(100); }
                if (_controls.Buttons.X) { output[(int)xbox.x] = Convert.ToByte(100); }
                if (_controls.Buttons.Y) { output[(int)xbox.y] = Convert.ToByte(100); }

                if (_controls.Buttons.Start) { output[(int)xbox.start] = Convert.ToByte(100); }
                if (_controls.Buttons.Guide) { output[(int)xbox.home] = Convert.ToByte(100); }
                if (_controls.Buttons.Back) { output[(int)xbox.back] = Convert.ToByte(100); }

                if (_controls.Buttons.LeftShoulder) { output[(int)xbox.leftShoulder] = Convert.ToByte(100); }
                if (_controls.Buttons.RightShoulder) { output[(int)xbox.rightShoulder] = Convert.ToByte(100); }
                if (_controls.Buttons.LeftStick) { output[(int)xbox.leftStick] = Convert.ToByte(100); }
                if (_controls.Buttons.RightStick) { output[(int)xbox.rightStick] = Convert.ToByte(100); }

                if (_controls.Triggers.Left > 0) { output[(int)xbox.leftTrigger] = Convert.ToByte(_controls.Triggers.Left * 100); }
                if (_controls.Triggers.Right > 0) { output[(int)xbox.rightTrigger] = Convert.ToByte(_controls.Triggers.Right * 100); }

                double dblLX = _controls.ThumbSticks.Left.X * 100;
                double dblLY = _controls.ThumbSticks.Left.Y * -100;
                double dblRX = _controls.ThumbSticks.Right.X * 100;
                double dblRY = _controls.ThumbSticks.Right.Y * -100;


                if (dblLX != 0) { output[(int)xbox.leftX] = (byte)Convert.ToSByte((int)(dblLX)); }
                if (dblLY != 0) { output[(int)xbox.leftY] = (byte)Convert.ToSByte((int)(dblLY)); }
                if (dblRX != 0) { output[(int)xbox.rightX] = (byte)Convert.ToSByte((int)(dblRX)); }
                if (dblRY != 0) { output[(int)xbox.rightY] = (byte)Convert.ToSByte((int)(dblRY)); }


                if (Ps4Touchpad == true)
                    output[(int)xbox.touch] = Convert.ToByte(100);

                _gcapi_Write(output);


                if (system.UseRumble == true)
                {
                    GCAPI_REPORT_TITANONE report = new GCAPI_REPORT_TITANONE();
                    if (_gcapi_Read_TO(ref report) != IntPtr.Zero)
                    {
                        GamePad.SetState(PlayerIndex.One, report.rumble[0], report.rumble[1]);
                    }
                }
            }
        }

    }
}
