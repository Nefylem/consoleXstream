using System;
using System.IO;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.TitanOne.GCMAPI
{
    class Init
    {
        public Init(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Open()
        {
            _class.System.Debug("titanOne.log", "[0] Opening TitanOne GCMAPI");
            var homeDir = Directory.GetCurrentDirectory() + @"\";

            var api = "titanOne_gcdapi.dll";

            if (File.Exists(homeDir + "gcdapi.dll")) api = "gcdapi.dll";

            if (File.Exists(homeDir + api) == false)
            {
                _class.System.Debug("titanOne.log", "[0] [FAIL] Unable to find TitanOne GCMAPI (gcdapi.dll)");
                return;
            }

            _class.System.Debug("titanOne.log", "[TRY] Attempting to open TitanOne Device Interface");

            var ptrDll = Define.LoadLibrary(homeDir + api);
            if (ptrDll == IntPtr.Zero)
            {
                _class.System.Debug("titanOne.log", "[0] [FAIL] Unable to allocate Device API");
                return;
            }

            var ptrMLoad = LoadExternalFunction(ptrDll, "gcmapi_Load");
            if (ptrMLoad == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcMapi_Load"); return; }

            _class.MDefine.GcmapiLoad = (Define.GCMAPI_LOAD)Marshal.GetDelegateForFunctionPointer(ptrMLoad, typeof(Define.GCMAPI_LOAD));

            var ptrMUnload = LoadExternalFunction(ptrDll, "gcmapi_Unload");
            if (ptrMUnload == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcMapi_Unload"); return; }

            _class.MDefine.GcmapiUnload = (Define.GCMAPI_UNLOAD)Marshal.GetDelegateForFunctionPointer(ptrMUnload, typeof(Define.GCMAPI_UNLOAD));

            var ptrMConnect = LoadExternalFunction(ptrDll, "gcmapi_Connect");
            if (ptrMConnect == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcmapi_Connect"); return; }

            _class.MDefine.GcmapiConnect = (Define.GCMAPI_CONNECT)Marshal.GetDelegateForFunctionPointer(ptrMConnect, typeof(Define.GCMAPI_CONNECT));

            var ptrMConnected = LoadExternalFunction(ptrDll, "gcmapi_IsConnected");
            if (ptrMConnected == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcmapi_IsConnected"); return; }

            _class.MDefine.GcmapiIsConnected =
                (Define.GCMAPI_ISCONNECTED)Marshal.GetDelegateForFunctionPointer(ptrMConnected, typeof(Define.GCMAPI_ISCONNECTED));

            var ptrSerial = LoadExternalFunction(ptrDll, "gcmapi_GetSerialNumber");
            if (ptrSerial == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcmapi_GetSerialNumber"); return; }

            _class.MDefine.GcmapiGetSerialNumber =
                (Define.GCMAPI_GETSERIALNUMBER)
                    Marshal.GetDelegateForFunctionPointer(ptrSerial, typeof (Define.GCMAPI_GETSERIALNUMBER));

            var ptrWrite = LoadExternalFunction(ptrDll, "gcmapi_Write");
            if (ptrWrite == IntPtr.Zero) { _class.System.Debug("titanOne.log", "[0] [FAIL] gcmapi_Write"); return; }

            _class.MDefine.GcmapiWrite = (Define.GCMAPI_WRITE)Marshal.GetDelegateForFunctionPointer(ptrWrite, typeof(Define.GCMAPI_WRITE));
        }

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
            if (_class.MDefine.GcmapiUnload != null) _class.MDefine.GcmapiUnload();

            _class.MDefine.GcmapiConnect = null;
            _class.MDefine.GcmapiGetSerialNumber = null;
            _class.MDefine.GcmapiIsConnected = null;
            _class.MDefine.GcmapiLoad = null;
            _class.MDefine.GcmapiUnload = null;
            _class.System.Debug("titanOne.log", "[OK] Closed TitanOne GCMAPI");
        }

    }
}
