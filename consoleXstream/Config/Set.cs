using System;
using System.Collections.Generic;

namespace consoleXstream.Config
{
    public class Set
    {
        public Set(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public List<string> Title;
        public List<string> Data;

        //Adds or replaces user data
        public void Add(string strTitle, string strSet)
        {
            if (strTitle.ToLower() != "title")
            {
                int intIndex = Title.IndexOf(strTitle);
                if (intIndex > -1)
                {
                    //Overwrite current setting
                    Data[intIndex] = strSet;
                }
                else
                {
                    //Add new setting
                    Title.Add(strTitle);
                    Data.Add(strSet);
                }
            }

            if (!_class.Var.IsReadData) _class.Xml.Save();
        }

        public string Check(string strTitle)
        {
            string strReturn = "";

            int intIndex = Title.FindIndex(x => x.Equals(strTitle, StringComparison.OrdinalIgnoreCase));
            if (intIndex > -1)
            {
                strReturn = Data[intIndex];
            }

            return strReturn;
        }

        public string CheckData(string strTitle)
        {
            string strReturn = "";
            string strTemp = "";

            strTemp = Find(strTitle);

            if (strTemp.Length > 0)
                strReturn += strTemp;

            return strReturn;
        }

        public string Find(string strTitle)
        {
            for (int intCount = 0; intCount < Title.Count; intCount++)
            {
                if (Title[intCount].ToLower() == strTitle.ToLower())
                    return "<" + strTitle + ">" + Data[intCount] + "</" + strTitle + ">";
            }
            return "";
        }

    }
}
