using System;
using System.IO;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.CronusPlus
{
    class Init
    {
        public Init(Classes classes) { _class = classes; }
        private readonly Classes _class;

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        public void Open()
        {
            _class.BaseClass.System.Debug("ControllerMax.log", "[0] Opening ControllerMax api");
            var dir = Directory.GetCurrentDirectory() + @"\";

            var api = "controllerMax_gcdapi.dll";

            //TODO: check API To confirm which one we're loading
            if (File.Exists(dir + "gcdapi.dll")) api = "gcdapi.dll";

            if (File.Exists(dir + api) == false)
            {
                _class.BaseClass.System.Debug("[0] [FAIL] Unable to find ControllerMax API");
                return;
            }

            _class.BaseClass.System.Debug("ControllerMax.log", "[TRY] Attempting to open ControllerMax Device Interface");

            var ptrDll = LoadLibrary(dir + api);
            if (ptrDll == IntPtr.Zero)
            {
                _class.BaseClass.System.Debug("ControllerMax.log", "[0] [FAIL] Unable to allocate Device API");
                return;
            }

            var ptrLoad = LoadExternalFunction(ptrDll, "gcdapi_Load");
            if (ptrLoad == IntPtr.Zero) { _class.BaseClass.System.Debug("ControllerMax.log", "[0] [FAIL] gcapi_Load"); return; }

            var ptrIsConnected = LoadExternalFunction(ptrDll, "gcapi_IsConnected");
            if (ptrIsConnected == IntPtr.Zero) { _class.BaseClass.System.Debug("ControllerMax.log", "[0] [FAIL] gcapi_IsConnected"); return; }

            var ptrUnload = LoadExternalFunction(ptrDll, "gcdapi_Unload");
            if (ptrUnload == IntPtr.Zero) { _class.BaseClass.System.Debug("ControllerMax.log", "[0] [FAIL] gcapi_Unload"); return; }

            var ptrGetTimeVal = LoadExternalFunction(ptrDll, "gcapi_GetTimeVal");
            if (ptrGetTimeVal == IntPtr.Zero) { _class.BaseClass.System.Debug("ControllerMax.log", "[0] [FAIL] gcapi_GetTimeVal"); return; }
            
            var ptrGetFwVer = LoadExternalFunction(ptrDll, "gcapi_GetFWVer");
            if (ptrGetFwVer == IntPtr.Zero) { _class.BaseClass.System.Debug("ControllerMax.log", "[0] [FAIL] gcapi_GetFWVer"); return; }

            var ptrWrite = LoadExternalFunction(ptrDll, "gcapi_Write");
            if (ptrWrite == IntPtr.Zero) return;
            
            var ptrRead = LoadExternalFunction(ptrDll, "gcapi_Read");
            if (ptrRead == IntPtr.Zero) return;
            
            var ptrCalcPressTime = LoadExternalFunction(ptrDll, "gcapi_CalcPressTime");
            if (ptrCalcPressTime == IntPtr.Zero) { _class.BaseClass.System.Debug("ControllerMax.log", "[0] [FAIL] gcapi_CalcPressTime"); return; }

            try
            {
                _class.Define.Load = (Define.GcapiLoadPtr)Marshal.GetDelegateForFunctionPointer(ptrLoad, typeof(Define.GcapiLoadPtr));
                _class.Define.IsConnected = (Define.GcapiIsconnectedPtr)Marshal.GetDelegateForFunctionPointer(ptrIsConnected, typeof(Define.GcapiIsconnectedPtr));
                _class.Define.Unload = (Define.GcapiUnloadPtr)Marshal.GetDelegateForFunctionPointer(ptrUnload, typeof(Define.GcapiUnloadPtr));
                _class.Define.GetTimeVal = (Define.GcapiGettimevalPtr)Marshal.GetDelegateForFunctionPointer(ptrGetTimeVal, typeof(Define.GcapiGettimevalPtr));
                _class.Define.GetFwVer = (Define.GcapiGetfwverPtr)Marshal.GetDelegateForFunctionPointer(ptrGetFwVer, typeof(Define.GcapiGetfwverPtr));
                _class.Define.Write = (Define.GcapiWritePtr)Marshal.GetDelegateForFunctionPointer(ptrWrite, typeof(Define.GcapiWritePtr));
                _class.Define.Read = (Define.GcapiReadCmPtr)Marshal.GetDelegateForFunctionPointer(ptrRead, typeof(Define.GcapiReadCmPtr));
                _class.Define.CalcPressTime = (Define.GcapiCalcpresstimePtr)Marshal.GetDelegateForFunctionPointer(ptrCalcPressTime, typeof(Define.GcapiCalcpresstimePtr));
            }
            catch (Exception ex)
            {
                _class.BaseClass.System.Debug("ControllerMax.log", "[0] Fail -> " + ex);
            }


            _class.Define.Load();
            _class.BaseClass.System.Debug("ControllerMax.log", "[0] Initialize ControllerMax GCAPI ok");
        }

        private IntPtr LoadExternalFunction(IntPtr ptrDll, string strFunction)
        {
            var function = GetProcAddress(ptrDll, strFunction);
            if (function == IntPtr.Zero)
            {
                _class.BaseClass.System.Debug("ControllerMax.log", "[0] [NG] " + strFunction + " alloc fail");
            }
            else
            {
                _class.BaseClass.System.Debug("ControllerMax.log", "[5] [OK] " + strFunction);
            }
            return function;
        }

        public void Close()
        {
            if (_class.Define.Unload != null)
                _class.Define.Unload();

            _class.Define.Load = null;
            _class.Define.IsConnected = null;
            _class.Define.GetTimeVal = null;
            _class.Define.GetFwVer = null;
            _class.Define.Write = null;
            _class.Define.WriteEx = null;
            _class.Define.WriteRef = null;
            _class.Define.Read = null;
            _class.Define.CalcPressTime = null;
            _class.Define.Unload = null;

            _class.BaseClass.System.Debug("ControllerMax.log", "[OK] Closed ControllerMax API");
        }

    }
}
