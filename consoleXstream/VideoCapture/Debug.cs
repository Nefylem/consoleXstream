using System;
using System.IO;
using System.Threading.Tasks;

namespace consoleXstream.VideoCapture
{
    class Debug
    {
        public Debug(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        private int _intLastDebugLevel;

        protected virtual async Task WriteTextAsync(string strWrite)
        {
            strWrite = strWrite.Trim();
            var strCurrentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            var txtOut = new StreamWriter("video.log", true);
            if (strWrite.Length > 0)
                strWrite = strCurrentTime + " - " + strWrite;
            await txtOut.WriteLineAsync(strWrite);
            txtOut.Close();
        }

        public async void Log(string strWrite)
        {
            if (_class.System == null) return;
            var intLevel = _intLastDebugLevel;
            var intSysLevel = _class.System.intDebugLevel;

            if (strWrite.IndexOf(']') == 2)
            {
                var strTest = strWrite.Substring(1, strWrite.IndexOf(']') - 1);
                strWrite = strWrite.Substring(strWrite.IndexOf(']') + 1);
                try
                {
                    intLevel = Convert.ToInt32(strTest);
                }
                catch
                {
                    // ignored. Dont care if this errors. Last system debug level will still apply
                } 
            }

            _intLastDebugLevel = intLevel;

            if (intLevel <= intSysLevel)
            {
                await WriteTextAsync(strWrite);
            }
        }

    }
}
