using System;
using System.IO;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.TitanOne
{
    class Init
    {
        public Init(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Open()
        {
            if (_class.Write.ApiMethod == Define.ApiMethod.Multi)
                _class.MInit.Open();
            else
                OpenSingle();
        }

        private void OpenSingle()
        {
            _class.System.Debug("titanOne.log", "[0] Opening TitanOne api");
            var homeDir = Directory.GetCurrentDirectory() + @"\";

            var api = "titanOne_gcdapi.dll";

            if (File.Exists(homeDir + "gcdapi.dll")) api = "gcdapi.dll";

            if (File.Exists(homeDir + api) == false)
            {
                _class.System.Debug("titanOne.log", "[0] [FAIL] Unable to find TitanOne API (gcdapi.dll)");
                return;
            }

            _class.System.Debug("titanOne.log", "[TRY] Attempting to open TitanOne Device Interface");

            var ptrDll = Define.LoadLibrary(homeDir + api);
            if (ptrDll == IntPtr.Zero)
            {
                _class.System.Debug("titanOne.log", "[0] [FAIL] Unable to allocate Device API");
                return;
            }

            var ptrLoad = LoadExternalFunction(ptrDll, "gcdapi_Load");
            if (ptrLoad == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_Load"); return; }

            var ptrLoadDevice = LoadExternalFunction(ptrDll, "gcdapi_LoadDevice");
            if (ptrLoadDevice == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_LoadDevice"); return; }

            var ptrIsConnected = LoadExternalFunction(ptrDll, "gcapi_IsConnected");
            if (ptrIsConnected == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_IsConnected"); return; }

            var ptrUnload = LoadExternalFunction(ptrDll, "gcdapi_Unload");
            if (ptrUnload == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_Unload"); return; }

            var ptrGetTimeVal = LoadExternalFunction(ptrDll, "gcapi_GetTimeVal");
            if (ptrGetTimeVal == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_GetTimeVal"); return; }

            var ptrGetFwVer = LoadExternalFunction(ptrDll, "gcapi_GetFWVer");
            if (ptrGetFwVer == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_GetFWVer"); return; }

            var ptrWrite = LoadExternalFunction(ptrDll, "gcapi_Write");
            if (ptrWrite == IntPtr.Zero) return;

            var ptrRead = LoadExternalFunction(ptrDll, "gcapi_Read");
            if (ptrRead == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_WriteEX"); return; }

            var ptrWriteEx = LoadExternalFunction(ptrDll, "gcapi_WriteEX");
            if (ptrWriteEx == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_WriteEX"); return; }

            var ptrReadEx = LoadExternalFunction(ptrDll, "gcapi_ReadEX");
            if (ptrReadEx == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_ReadEX"); return; }

            var ptrCalcPressTime = LoadExternalFunction(ptrDll, "gcapi_CalcPressTime");
            if (ptrCalcPressTime == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcapi_CalcPressTime"); return; }

            try
            {
                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_Load");
                _class.Define.Load = (Define.GcapiLoad)Marshal.GetDelegateForFunctionPointer(ptrLoad, typeof(Define.GcapiLoad));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_LoadDevice");
                _class.Define.LoadDevice = (Define.GcapiLoaddevice)Marshal.GetDelegateForFunctionPointer(ptrLoadDevice, typeof(Define.GcapiLoaddevice));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_IsConnected");
                _class.Define.IsConnected = (Define.GcapiIsconnected)Marshal.GetDelegateForFunctionPointer(ptrIsConnected, typeof(Define.GcapiIsconnected));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_Unload");
                _class.Define.Unload = (Define.GcapiUnload)Marshal.GetDelegateForFunctionPointer(ptrUnload, typeof(Define.GcapiUnload));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_GetTimeVal");
                _class.Define.GetTimeVal = (Define.GcapiGettimeval)Marshal.GetDelegateForFunctionPointer(ptrGetTimeVal, typeof(Define.GcapiGettimeval));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_GetFwVer");
                _class.Define.GetFwVer = (Define.GcapiGetfwver)Marshal.GetDelegateForFunctionPointer(ptrGetFwVer, typeof(Define.GcapiGetfwver));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_Write");
                _class.Define.Write = (Define.GcapiWrite)Marshal.GetDelegateForFunctionPointer(ptrWrite, typeof(Define.GcapiWrite));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_CalcPressTime");
                _class.Define.CalcPressTime = (Define.GcapiCalcpresstime)Marshal.GetDelegateForFunctionPointer(ptrCalcPressTime, typeof(Define.GcapiCalcpresstime));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_WriteEx");
                _class.Define.WriteEx = (Define.GcapiWriteEx)Marshal.GetDelegateForFunctionPointer(ptrWriteEx, typeof(Define.GcapiWriteEx));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_Read_TO");
                _class.Define.Read = (Define.GcapiReadTo)Marshal.GetDelegateForFunctionPointer(ptrReadEx, typeof(Define.GcapiReadTo));

                _class.System.Debug("titanOne.log", "[5] Marshal _gcapi_Read_CM");
                _class.Define.ReadCm = (Define.GcapiReadCm)Marshal.GetDelegateForFunctionPointer(ptrReadEx, typeof(Define.GcapiReadCm));

            }
            catch (Exception ex)
            {
                _class.System.Debug("titanOne.log", "[0] Fail -> " + ex);
                _class.System.Debug("titanOne.log", "[0] [ERR] Critical failure loading TitanOne API.");
                return;
            }

            _class.System.Debug("titanOne.log", ">>>" + _class.Write.DevId);
            if (_class.Define.LoadDevice((ushort)_class.Write.DevId) == 1)
                _class.System.Debug("titanOne.log", "[0] Initialize TitanOne GCAPI ok");
            else
            {
                _class.System.Debug("titanOne.log", "[0] Initialize TitanOne failed");
                return;
            }

            //loadShortcutKeys();

            _class.System.Debug("titanOne.log", "");
        }

        //Finds the pointer for the dll function
        private IntPtr LoadExternalFunction(IntPtr ptrDll, string strFunction)
        {
            var ptrFunction = Define.GetProcAddress(ptrDll, strFunction);
            if (ptrFunction == IntPtr.Zero)
                _class.System.Debug("titanOne.log", "[0] [NG] " + strFunction + " alloc fail");
            else
                _class.System.Debug("titanOne.log", "[5] [OK] " + strFunction);

            return ptrFunction;
        }

        public void Close()
        {
            if (_class.Write.ApiMethod == Define.ApiMethod.Multi)
                _class.MInit.Close();
            else
                CloseSingle();
        }

        private void CloseSingle()
        {
            if (_class.Define.Unload != null) _class.Define.Unload();

            _class.Define.Load = null;
            _class.Define.LoadDevice = null;
            _class.Define.IsConnected = null;
            _class.Define.GetTimeVal = null;
            _class.Define.GetFwVer = null;
            _class.Define.Write = null;
            _class.Define.WriteEx = null;
            //_class.Define._gcapi_WriteRef = null;
            _class.Define.Read = null;
            _class.Define.ReadCm = null;
            _class.Define.CalcPressTime = null;
            _class.Define.Unload = null;

            _class.System.Debug("titanOne.log", "[OK] Closed TitanOne API");
        }


    }
}
