using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace consoleXstream.Output
{
    class ListDevices
    {
        public int GetToCount(string title)
        {
            var search = "";
            
            if (title.ToLower() == "titanone") search = @"VID_2508&PID_0003";

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
                collection = searcher.Get();

            return collection.Cast<ManagementBaseObject>().Select(
                device => device.GetPropertyValue("Dependent").ToString().ToLower()).Where(
                strSearch => strSearch.IndexOf(search.ToLower(), StringComparison.Ordinal) > -1).Count(CheckDevice);
        }

        private static bool CheckDevice(string device)
        {
            if (device.Length <= 5) 
                return false;

            return device.Substring(device.Length - 5) != "0000\"";
        }

        public List<string> FindDevices(string title)
        {
            var search = "";
            if (title.ToLower() == "titanone") search = @"VID_2508&PID_0003";

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
                collection = searcher.Get();

            return (from ManagementBaseObject device in collection select device.GetPropertyValue("Dependent").ToString().ToLower() into strSearch where strSearch.IndexOf(search.ToLower(), StringComparison.Ordinal) > -1 where CheckDevice(strSearch) select strSearch).ToList();
        }
    }
}
