using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using consoleXstream.Input;

namespace consoleXstream.Output
{
    public class TitanOne
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

        public struct GCAPI_REPORT_CONTROLLERMAX
        {
            public byte console; // Receives values established by the #defines CONSOLE_*
            public byte controller; // Values from #defines CONTROLLER_* and EXTENSION_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] led; // Four LED - #defines LED_*

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
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
        private delegate IntPtr GCAPI_READ_CM([In, Out] ref GCAPI_REPORT_CONTROLLERMAX gcapi_report);

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
        private GCAPI_READ_CM _gcapi_Read_CM = null;
        private GCAPI_CALCPRESSTIME _gcapi_CalcPressTime = null;
        private GCAPI_UNLOAD _gcapi_Unload = null;
        #endregion

        private Form1 frmMain;
        private Config.Configuration system;
        private KeyboardInterface keyboardInterface;
        private Remap.Remapping remap;

        public int CMHomeCount { get; set; }
        public bool Ps4Touchpad { get; set; }

        public enum DevPID
        {
            Any = 0x000,
            ControllerMax = 0x001,
            Cronus = 0x002,
            TitanOne = 0x003
        };

        private Input.GamePadState _controls;
        
        private string _strTODevice;

        private bool _boolNoticeTODisconnected = false;
        private bool _boolHoldBack = false;
        private bool _boolLoadShortcuts = false;

        private int _XboxCount;
        private int _MenuWait;
        private int _MenuShow;

        private int[,] _intShortcut;
        private int _intShortcutCount;

        private DevPID _devId = DevPID.Any;

        public TitanOne(Form1 mainForm) { frmMain = mainForm; }
        public void getSystemHandle(Config.Configuration inSystem) { system = inSystem; }
        public void getKeyboardInterfaceHandle(KeyboardInterface inKeyInterface) { keyboardInterface = inKeyInterface; }
        public void getRemapHandle(Remap.Remapping inMap) { remap = inMap; }

        public void setTOInterface(DevPID devID)
        {
            _devId = devID;
            system.debug("titanOne.log", "[0] using " + _devId.ToString());
            system.debug("titanOne.log", "");
        }
        
        public void initTitanOne()
        {
            //TODO: Read from setup menu
            _MenuShow = 35;

            var strDevice = "TitanOne";
            var strRef = "TODI";
            var strAPI = "titanOne_gcdapi.dll";

            system.debug("titanOne.log", "[0] Opening " + strDevice + " api");
            var strDir = Directory.GetCurrentDirectory() + @"\";
            _strTODevice = strDevice;

            if (File.Exists(strDir + strAPI) == false)
            {
                system.debug("titanOne.log", "[0] [FAIL] Unable to find " + strDevice + " API (" + strAPI + ")");
                return;
            }

            system.debug("titanOne.log", "[TRY] Attempting to open " + strDevice + " Device Interface (" + strRef + ")");

            var ptrDll = LoadLibrary(strDir + strAPI);
            if (ptrDll == IntPtr.Zero)
            {
                system.debug("titanOne.log", "[0] [FAIL] Unable to allocate Device API");
                return;
            }

            var ptrLoad = loadExternalFunction(ptrDll, "gcdapi_Load");
            if (ptrLoad == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_Load"); return; }

            var ptrLoadDevice = loadExternalFunction(ptrDll, "gcdapi_LoadDevice");
            if (ptrLoadDevice == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_LoadDevice"); return; }

            var ptrIsConnected = loadExternalFunction(ptrDll, "gcapi_IsConnected");
            if (ptrIsConnected == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_IsConnected"); return; }

            var ptrUnload = loadExternalFunction(ptrDll, "gcdapi_Unload");
            if (ptrUnload == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_Unload"); return; }

            var ptrGetTimeVal = loadExternalFunction(ptrDll, "gcapi_GetTimeVal");
            if (ptrGetTimeVal == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_GetTimeVal"); return; }

            var ptrGetFwVer = loadExternalFunction(ptrDll, "gcapi_GetFWVer");
            if (ptrGetFwVer == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_GetFWVer"); return; }

            var ptrWrite = loadExternalFunction(ptrDll, "gcapi_Write");
            if (ptrWrite == IntPtr.Zero) return;

            var ptrRead = loadExternalFunction(ptrDll, "gcapi_Read");
            if (ptrRead == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_WriteEX"); return; }

            var ptrWriteEx = loadExternalFunction(ptrDll, "gcapi_WriteEX");
            if (ptrWriteEx == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_WriteEX"); return; }

            var ptrReadEx = loadExternalFunction(ptrDll, "gcapi_ReadEX");
            if (ptrReadEx == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_ReadEX"); return; }

            var ptrCalcPressTime = loadExternalFunction(ptrDll, "gcapi_CalcPressTime");
            if (ptrCalcPressTime == IntPtr.Zero) { system.debug("titanOne.log", "[0] [FAIL] gcapi_CalcPressTime"); return; }

            try
            {
                system.debug("titanOne.log", "[5] Marshal _gcapi_Load");
                _gcapi_Load = (GCAPI_LOAD)Marshal.GetDelegateForFunctionPointer(ptrLoad, typeof(GCAPI_LOAD));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_LoadDevice");
                _gcapi_LoadDevice = (GCAPI_LOADDEVICE)Marshal.GetDelegateForFunctionPointer(ptrLoadDevice, typeof(GCAPI_LOADDEVICE));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_IsConnected");
                _gcapi_IsConnected = (GCAPI_ISCONNECTED)Marshal.GetDelegateForFunctionPointer(ptrIsConnected, typeof(GCAPI_ISCONNECTED));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_Unload");
                _gcapi_Unload = (GCAPI_UNLOAD)Marshal.GetDelegateForFunctionPointer(ptrUnload, typeof(GCAPI_UNLOAD));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_GetTimeVal");
                _gcapi_GetTimeVal = (GCAPI_GETTIMEVAL)Marshal.GetDelegateForFunctionPointer(ptrGetTimeVal, typeof(GCAPI_GETTIMEVAL));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_GetFwVer");
                _gcapi_GetFwVer = (GCAPI_GETFWVER)Marshal.GetDelegateForFunctionPointer(ptrGetFwVer, typeof(GCAPI_GETFWVER));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_Write");
                _gcapi_Write = (GCAPI_WRITE)Marshal.GetDelegateForFunctionPointer(ptrWrite, typeof(GCAPI_WRITE));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_CalcPressTime");
                _gcapi_CalcPressTime = (GCAPI_CALCPRESSTIME)Marshal.GetDelegateForFunctionPointer(ptrCalcPressTime, typeof(GCAPI_CALCPRESSTIME));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_Write");
                _gcapi_Write = (GCAPI_WRITE)Marshal.GetDelegateForFunctionPointer(ptrWrite, typeof(GCAPI_WRITE));
                
                //TitanOne - modified DLL to accept LED / rumble feedback
                system.debug("titanOne.log", "[5] Marshal _gcapi_WriteEx");
                _gcapi_WriteEx = (GCAPI_WRITE_EX)Marshal.GetDelegateForFunctionPointer(ptrWriteEx, typeof(GCAPI_WRITE_EX));
                
                system.debug("titanOne.log", "[5] Marshal _gcapi_Read_TO");
                _gcapi_Read_TO = (GCAPI_READ_TO)Marshal.GetDelegateForFunctionPointer(ptrReadEx, typeof(GCAPI_READ_TO));

                system.debug("titanOne.log", "[5] Marshall _gcapi_read_CM");
                _gcapi_Read_CM = (GCAPI_READ_CM)Marshal.GetDelegateForFunctionPointer(ptrReadEx, typeof(GCAPI_READ_CM));
            }
            catch (Exception ex)
            {
                system.debug("titanOne.log", "[0] Fail -> " + ex.ToString());
                system.debug("titanOne.log", "[0] [ERR] Critical failure loading " + _strTODevice + " API.");
                return;
            }

            //TODO: _devID should be set by now on startup
            system.debug("titanOne.log", ">>>" + _devId.ToString());
            if (_gcapi_LoadDevice((ushort)_devId) == 1)
                system.debug("titanOne.log", "[0] Initialize " + _strTODevice + " GCAPI ok");
            else
            {
                system.debug("titanOne.log", "[0] Initialize " + _strTODevice + " failed");
                return;
            }

            loadShortcutKeys();

            system.debug("titanOne.log", "");

        }

        //Finds the pointer for the dll function
        private IntPtr loadExternalFunction(IntPtr ptrDll, string strFunction)
        {
            IntPtr ptrFunction = IntPtr.Zero;
            ptrFunction = GetProcAddress(ptrDll, strFunction);
            if (ptrFunction == IntPtr.Zero)
            {
                system.debug("titanOne.log", "[0] [NG] " + strFunction + " alloc fail");
            }
            else
            {
                system.debug("titanOne.log", "[5] [OK] " + strFunction);
            }
            return ptrFunction;
        }

        private void loadShortcutKeys()
        {
            _intShortcutCount = 0;

            string strInput = "";
            string[] strTemp;
            int intTemp1, intTemp2, intTemp3;
            _intShortcut = new int[3, 25];

            //Load these in incase there is no shortcut file
            //Home - normal mode
            _intShortcut[0, 0] = (int)Xbox.Back;
            _intShortcut[1, 0] = (int)Xbox.B;
            _intShortcut[2, 0] = (int)Xbox.Home;
            _intShortcutCount++;

            //Home - PS4 mode
            _intShortcut[0, 1] = (int)Xbox.Touch;
            _intShortcut[1, 1] = (int)Xbox.B;
            _intShortcut[2, 1] = (int)Xbox.Home;
            _intShortcutCount++;

            if (File.Exists(@"Data\shortcutGamepad.txt") == true)
            {
                int intID = 0;
                StreamReader txtIn = new StreamReader(@"Data\shortcutGamepad.txt");
                while ((strInput = txtIn.ReadLine()) != null)
                {
                    try
                    {
                        strTemp = strInput.Split(',');
                        if (strTemp.Length == 3)
                        {
                            intTemp1 = Convert.ToInt32(strTemp[0]);
                            intTemp2 = Convert.ToInt32(strTemp[1]);
                            intTemp3 = Convert.ToInt32(strTemp[2]);

                            _intShortcut[0, intID] = intTemp1;
                            _intShortcut[1, intID] = intTemp2;
                            _intShortcut[2, intID] = intTemp3;

                            intID++;
                            _intShortcutCount++;
                        }
                    }
                    catch { }
                }
                txtIn.Close();
            }
            _boolLoadShortcuts = true;
        }

        private void runScript()
        {

        }

        public void closeTitanOneInterface()
        {
            string strDevice = "TitanOne";
            string strRef = "TODI";

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

            system.debug("titanOne.log", "[OK] Closed " + strDevice + " (" + strRef + ")");
        }

        public void checkControllerInput()
        {
            if (_gcapi_Write == null)
                return;

            bool boolOverride = false;
            
            if (frmMain.boolIDE)
                boolOverride = true;

            if ((_gcapi_IsConnected() == 1) || (boolOverride == true))
            {
                //Update gamepad status
                _controls = GamePad.GetState(PlayerIndex.One);

                if (_XboxCount == 0) { _XboxCount = Enum.GetNames(typeof(Xbox)).Length; }
                byte[] output = new byte[_XboxCount];

                if (_controls.DPad.Left) { output[remap.remapGamepad.left] = Convert.ToByte(100); }
                if (_controls.DPad.Right) { output[remap.remapGamepad.right] = Convert.ToByte(100); }
                if (_controls.DPad.Up) { output[remap.remapGamepad.up] = Convert.ToByte(100); }
                if (_controls.DPad.Down) { output[remap.remapGamepad.down] = Convert.ToByte(100); }

                if (_controls.Buttons.A) { output[remap.remapGamepad.a] = Convert.ToByte(100); }
                if (_controls.Buttons.B) { output[remap.remapGamepad.b] = Convert.ToByte(100); }
                if (_controls.Buttons.X) { output[remap.remapGamepad.x] = Convert.ToByte(100); }
                if (_controls.Buttons.Y) { output[remap.remapGamepad.y] = Convert.ToByte(100); }

                if (_controls.Buttons.Start) { output[remap.remapGamepad.start] = Convert.ToByte(100); }
                if (_controls.Buttons.Guide) { output[remap.remapGamepad.home] = Convert.ToByte(100); }
                if (_controls.Buttons.Back)
                {
                    if (system.boolBlockMenuButton == false)
                    {
                        _MenuWait++;
                        if (system.boolMenu == false)
                            if (_MenuWait >= _MenuShow + 20)
                                openMenu();
                    }

                    //Remap back buton to touchpad
                    if (system.boolPS4ControllerMode)
                        output[remap.remapGamepad.touch] = Convert.ToByte(100);
                    else
                        output[remap.remapGamepad.back] = Convert.ToByte(100);
                }

                if (_controls.Buttons.LeftShoulder) { output[remap.remapGamepad.leftShoulder] = Convert.ToByte(100); }
                if (_controls.Buttons.RightShoulder) { output[remap.remapGamepad.rightShoulder] = Convert.ToByte(100); }
                if (_controls.Buttons.LeftStick) { output[remap.remapGamepad.leftStick] = Convert.ToByte(100); }
                if (_controls.Buttons.RightStick) { output[remap.remapGamepad.rightStick] = Convert.ToByte(100); }

                if (_controls.Triggers.Left > 0) { output[remap.remapGamepad.leftTrigger] = Convert.ToByte(_controls.Triggers.Left * 100); }
                if (_controls.Triggers.Right > 0) { output[remap.remapGamepad.rightTrigger] = Convert.ToByte(_controls.Triggers.Right * 100); }

                double dblLX = _controls.ThumbSticks.Left.X * 100;
                double dblLY = _controls.ThumbSticks.Left.Y * 100;
                double dblRX = _controls.ThumbSticks.Right.X * 100;
                double dblRY = _controls.ThumbSticks.Right.Y * 100;

                if (system.boolNormalizeControls == true)
                {
                    normalGamepad(ref dblLX, ref dblLY);
                    normalGamepad(ref dblRX, ref dblRY);
                }
                else
                {
                    dblLY = -dblLY;
                    dblRY = -dblRY;
                }

                if (dblLX != 0) { output[remap.remapGamepad.leftX] = (byte)Convert.ToSByte((int)(dblLX)); }
                if (dblLY != 0) { output[remap.remapGamepad.leftY] = (byte)Convert.ToSByte((int)(dblLY)); }
                if (dblRX != 0) { output[remap.remapGamepad.rightX] = (byte)Convert.ToSByte((int)(dblRX)); }
                if (dblRY != 0) { output[remap.remapGamepad.rightY] = (byte)Convert.ToSByte((int)(dblRY)); }

                if (CMHomeCount > 0)
                {
                    output[remap.remapGamepad.home] = Convert.ToByte(100);
                    CMHomeCount--;
                }

                if (Ps4Touchpad == true)
                    output[remap.remapGamepad.touch] = Convert.ToByte(100);


                if (_boolLoadShortcuts)
                    output = checkKeys(output);

                int intTarget = -1;
                if (system.boolPS4ControllerMode == false) { intTarget = remap.remapGamepad.back; } else { intTarget = remap.remapGamepad.touch; }
                /*
                //Back button. Wait until released as its also the menu button
                if (intTarget > -1)
                {
                    if (system.boolBlockMenuButton)
                    {
                        if (output[intTarget] == 100)
                        {
                            _boolHoldBack = true;
                            output[intTarget] = Convert.ToByte(0);
                            _MenuWait++;
                            if (!system.boolMenu)
                            {
                                if (_MenuWait >= _MenuShow)
                                {
                                    _boolHoldBack = false;
                                    openMenu();
                                }
                            }
                        }
                        else
                        {
                            if (_boolHoldBack == true)
                            {
                                _boolHoldBack = false;
                                output[intTarget] = Convert.ToByte(100);
                                _MenuWait = 0;
                            }
                            else
                                _MenuWait = 0;
                        }
                    }
                }
                */
                if (keyboardInterface != null && keyboardInterface.output != null)
                {
                    for (int intCount = 0; intCount < _XboxCount; intCount++)
                    {
                        if (keyboardInterface.output[intCount] != 0)
                            output[intCount] = keyboardInterface.output[intCount];
                    }
                }

                //Block gamepad rumble
                //gcapi_WriteEX(uint8_t *outpacket, uint8_t size)
                /*
                [0xFF,0x01 : 2 byte, Packet Signature]
    [Update LED Command (0,1) : 1 byte]
        [LED 1 Status : 1 byte]
        [LED 2 Status : 1 byte]
        [LED 3 Status : 1 byte]
        [LED 4 Status : 1 byte]
    [Reset LEDs Command (0,1) : 1 byte]
    [Update Rumble Command (0,1) : 1 byte]
        [Rumble 1 Value : 1 byte]
        [Rumble 2 Value : 1 byte]
        [Rumble 3 Value : 1 byte]
        [Rumble 4 Value : 1 byte]
    [Reset Rumble Command (0,1) : 1 byte]
    [Block Rumble Command (0,1) : 1 byte]
    [Turn Off Controller Command (0,1) : 1 byte]
    [Button States : 36 bytes - same format as gcapi_Write]
                _gcapi_WriteEx(output);
                 */

                _gcapi_Write(output);

                if (system.boolUseRumble == true)
                {
                    if (_devId == DevPID.TitanOne)
                    {
                        GCAPI_REPORT_TITANONE report = new GCAPI_REPORT_TITANONE();
                        if (_gcapi_Read_TO(ref report) != IntPtr.Zero)
                            GamePad.SetState(PlayerIndex.One, report.rumble[0], report.rumble[1]);
                    }
                    else
                    {
                        GCAPI_REPORT_CONTROLLERMAX report = new GCAPI_REPORT_CONTROLLERMAX();
                        if (_gcapi_Read_CM(ref report) != IntPtr.Zero)
                            GamePad.SetState(PlayerIndex.One, report.rumble[0], report.rumble[1]);

                    }
                }
            }
            else
            {
                //If device just disconnected open up notice to tell user
                if (_boolNoticeTODisconnected == false)
                {
                    system.debug("titanOne.log", "[NOTE] " + _strTODevice + " is disconnected");
                    _boolNoticeTODisconnected = true;
                }

                //Keep alive for opening the menu
                _controls = GamePad.GetState(PlayerIndex.One);
                if (_controls.Buttons.Back)
                {
                    if (system.boolBlockMenuButton == false)
                    {
                        _MenuWait++;
                        if (!system.boolMenu)
                        {
                            if (_MenuWait >= _MenuShow + 20)
                                frmMain.OpenMenu();
                        }
                    }
                }
            }
        }

        private void normalGamepad(ref double dblLX, ref double dblLY)
        {
            double dblNewX = dblLX;
            double dblNewY = dblLY;

            double dblLength = Math.Sqrt(Math.Pow(dblLX, 2) + Math.Pow(dblLY, 2));
            if (dblLength > 99.9)
            {
                double dblTheta = Math.Atan2(dblLY, dblLX);
                double dblAngle = (90 - ((dblTheta * 180) / Math.PI)) % 360;

                if ((dblAngle < 0) && (dblAngle >= -45)) { dblNewX = (int)(100 / Math.Tan(dblTheta)); dblNewY = -100; }
                if ((dblAngle >= 0) && (dblAngle <= 45)) { dblNewX = (int)(100 / Math.Tan(dblTheta)); dblNewY = -100; }
                if ((dblAngle > 45) && (dblAngle <= 135)) { dblNewY = -(int)(Math.Tan(dblTheta) * 100); dblNewX = 100; }
                if ((dblAngle > 135) && (dblAngle <= 225)) { dblNewX = -(int)(100 / Math.Tan(dblTheta)); dblNewY = 100; }
                if (dblAngle > 225) { dblNewY = (int)(Math.Tan(dblTheta) * 100); dblNewX = -100; }
                if (dblAngle < -45) { dblNewY = (int)(Math.Tan(dblTheta) * 100); dblNewX = -100; }
            }
            else
            {
                dblNewY = -dblNewY;
            }

            //Return values
            dblLX = dblNewX;
            dblLY = dblNewY;
        }

        private byte[] checkKeys(byte[] output)
        {
            int intData1;
            int intData2;
            int intTarget;

            for (int intCount = 0; intCount < _intShortcutCount; intCount++)
            {
                intData1 = _intShortcut[0, intCount];
                intData2 = _intShortcut[1, intCount];
                intTarget = _intShortcut[2, intCount];

                if ((output[intData1].ToString() == "100") && (output[intData2].ToString() == "100"))
                {
                    output[intData1] = Convert.ToByte(0);
                    output[intData2] = Convert.ToByte(0);

                    if (intTarget < 32)
                        output[intTarget] = Convert.ToByte(100);
                    else
                        runScript();
                }
            }

            return output;
        }

        private void openMenu()
        {
            _boolHoldBack = false;
            _MenuWait = 0;

            frmMain.OpenMenu();
        }


    }
}
