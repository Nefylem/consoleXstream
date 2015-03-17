using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleXstream.Config
{
    class Debug
    {
        private int _intLastDebugLevel = 0;
        public int intDebugLevel = 5;           //All debug commands

        private async Task WriteTextAsync(string strWrite)
        {
            strWrite = strWrite.Trim();
            string strCurrentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            StreamWriter txtOut = new StreamWriter("system.log", true);
            if (strWrite.Length > 0)
                strWrite = strCurrentTime + " - " + strWrite;
            await txtOut.WriteLineAsync(strWrite);
            txtOut.Close();
        }

        private async Task WriteTextAsync(string strFile, string strWrite)
        {
            strWrite = strWrite.Trim();
            var strCurrentTime = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            var txtOut = new StreamWriter(strFile, true);
            if (strWrite.Length > 0)
                strWrite = strCurrentTime + " - " + strWrite;
            await txtOut.WriteLineAsync(strWrite);
            txtOut.Close();
        }

        public async void debug(string strWrite)
        {
            var intLevel = _intLastDebugLevel;
            var intSysLevel = intDebugLevel;

            if (strWrite.IndexOf(']') == 2)
            {
                string strTest = strWrite.Substring(1, strWrite.IndexOf(']') - 1);
                strWrite = strWrite.Substring(strWrite.IndexOf(']') + 1);
                try
                {
                    intLevel = Convert.ToInt32(strTest);
                }
                catch { }       //Dont care if this errors. Last system debug level will still apply
            }

            _intLastDebugLevel = intLevel;

            if (intLevel <= intSysLevel)
            {
                await WriteTextAsync(strWrite);
            }
        }

        public async void debug(string strFile, string strWrite)
        {
            int intLevel = _intLastDebugLevel;
            int intSysLevel = intDebugLevel;

            if (strWrite.IndexOf(']') == 2)
            {
                string strTest = strWrite.Substring(1, strWrite.IndexOf(']') - 1);
                strWrite = strWrite.Substring(strWrite.IndexOf(']') + 1);
                try
                {
                    intLevel = Convert.ToInt32(strTest);
                }
                catch { }       //Dont care if this errors. Last system debug level will still apply
            }

            _intLastDebugLevel = intLevel;

            if (intLevel <= intSysLevel)
            {
                await WriteTextAsync(strFile, strWrite);
            }
        }

    }
}
