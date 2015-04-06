using consoleXstream.Config;
using consoleXstream.Home;
using consoleXstream.Input;

namespace consoleXstream.Output.TitanOne
{
    class Classes
    {
        public Classes(Write write, BaseClass baseClass)
        {
            Write = write;
            BaseClass = baseClass;
        }

        public BaseClass BaseClass;

        public Init Init;
        public Define Define;
        public Write Write;

        public GCMAPI.Init MInit;
        public GCMAPI.Define MDefine;
        public GCMAPI.Write MWrite;
        public GCMAPI.Devices MDevices;

        public void Create()
        {
            Init = new Init(this);
            Define = new Define();

            MInit = new GCMAPI.Init(this);
            MDefine = new GCMAPI.Define();
            MDevices = new GCMAPI.Devices(this);
            MWrite = new GCMAPI.Write(this);
        }
    }
}
