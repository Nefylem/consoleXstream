using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using consoleXstream.Input;

namespace consoleXstream.Remap
{

    public class Remapping
    {
        public class remapCodes
        {
            [Description("Home")]
            public int home { get; set; }
            [Description("Back")]
            public int back { get; set; }
            [Description("Start")]
            public int start { get; set; }

            [Description("Right Shoulder")]
            public int rightShoulder { get; set; }
            [Description("Right Trigger")]
            public int rightTrigger { get; set; }   //0 - 100
            [Description("Right Stick")]
            public int rightStick { get; set; }

            [Description("Left Shoulder")]
            public int leftShoulder { get; set; }
            [Description("Left Trigger")]
            public int leftTrigger { get; set; }    //0 - 100
            [Description("Left Stick")]
            public int leftStick { get; set; }

            [Description("RightThumb X")]
            public int rightX { get; set; }
            [Description("RightThumb Y")]
            public int rightY { get; set; }
            [Description("LeftThumb X")]
            public int leftX { get; set; }
            [Description("LeftThumb Y")]
            public int leftY { get; set; }

            [Description("Up")]
            public int up { get; set; }
            [Description("Down")]
            public int down { get; set; }
            [Description("Left")]
            public int left { get; set; }
            [Description("Right")]
            public int right { get; set; }

            [Description("Y")]
            public int y { get; set; }
            [Description("B")]
            public int b { get; set; }
            [Description("A")]
            public int a { get; set; }
            [Description("X")]
            public int x { get; set; }

            [Description("Touchpad")]
            public int touch { get; set; }
             /*
            accX = 21,      //rotate X. 90 = -25, 180 = 0, 270 = +25, 360 = 0 (ng)
            accY = 22,      //shake vertically. +25 (top) to -25 (bottom) (ng)
            accZ = 23,      //tilt up
            gyroX = 24,     //no reading
            gyroY = 25,     //no reading
            gyroZ = 26,     //no reading
            touchX = 28,            //-100 to 100   (left to right)
            touchY = 29
             */
        }
        public remapCodes remapGamepad;

        public void setDefaultGamepad()
        {
            remapGamepad = new remapCodes();

            remapGamepad.home = 0;
            remapGamepad.back = 1;
            remapGamepad.start = 2;

            remapGamepad.rightShoulder = 3;
            remapGamepad.rightTrigger = 4;
            remapGamepad.rightStick = 5;

            remapGamepad.leftShoulder = 6;
            remapGamepad.leftTrigger = 7;
            remapGamepad.leftStick = 8;

            remapGamepad.rightX = 9;
            remapGamepad.rightY = 10;
            remapGamepad.leftX = 11;
            remapGamepad.leftY = 12;

            remapGamepad.up = 13;
            remapGamepad.down = 14;
            remapGamepad.left = 15;
            remapGamepad.right = 16;

            remapGamepad.y = 17;
            remapGamepad.b = 18;
            remapGamepad.a = 19;
            remapGamepad.x = 20;

            remapGamepad.touch = 27;
        }

        public void loadGamepadRemap()
        {
            if (Directory.Exists("Profiles"))
            {
                if (File.Exists(@"Profiles\gamepad.remap"))
                {
                    string setting = "";
                    XmlTextReader reader = new XmlTextReader(@"Profiles\gamepad.remap");
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                //MessageBox.Show("<" + reader.Name);
                                break;
                            case XmlNodeType.Text: //Display the text in each element.
                                setting = reader.Value;
                                break;
                            case XmlNodeType.EndElement: //Display the end of the element.
                                if (setting.Length > 0) 
                                    addRemapData(reader.Name, setting);

                                setting = "";
                                break;
                        }
                    }
                    reader.Close();
                }
            }
        }

        private void addRemapData(string title, string setting)
        {
            /*
            title = title.ToLower();
            int id = findRemapName(setting);
            if (id > -1)
            {
                Type myClassType = remapGamepad.GetType();
                PropertyInfo[] properties = myClassType.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (title == property.Name.ToLower())
                        if ((string) property.GetValue(remapGamepad, null) != id.ToString())
                        {
                            property.SetValue(remapGamepad, Convert.ChangeType(id, property.PropertyType), null);
                            break;
                        }
                }

            }
             */
        }

        public void saveGamepadRemap()
        {
            if (!Directory.Exists("Profiles"))
                Directory.CreateDirectory("Profiles");

            if (File.Exists(@"Profiles\gamepad.remap") == true) File.Delete(@"Profiles\gamepad.remap");

            string save = "<Gamepad>";
            Type myClassType = remapGamepad.GetType();
            PropertyInfo[] properties = myClassType.GetProperties();
            
            foreach (PropertyInfo property in properties)
            {
                string find = property.GetValue(remapGamepad, null).ToString();
                try
                {
                    int id = Convert.ToInt32(find.Trim());
                    save += "<" + property.Name + ">";
                    save += findGamepadValue(id);
                    save += "</" + property.Name + ">";
                }
                catch { }
                //save += "<" + property.Name + ">" + property.GetValue(remapGamepad, null) + "</" + property.Name + ">";
            }
            save += "</Gamepad>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(save);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(@"Profiles\gamepad.remap", settings);
            doc.Save(writer);
            writer.Close();
        }

        static string GetDescription(MemberInfo type)
        {
            DescriptionAttribute[] descriptions = (DescriptionAttribute[])
                type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length > 0)
                return descriptions[0].Description;
            else
                return "";
        }

        public int findRemapName(string title)
        {
            title = title.ToLower();
            Type myClassType = remapGamepad.GetType();
            PropertyInfo[] properties = myClassType.GetProperties();
            
            foreach (PropertyInfo property in properties)
            {
                string newTitle = GetDescription(property);
                if (title == newTitle.ToLower())
                {
                    try
                    {
                        return Convert.ToInt32(property.GetValue(remapGamepad, null).ToString());
                    }
                    catch { }
                }
            }
            return -1;
        }

        private string findGamepadValue(int value)
        {
            Xbox xboxValue = (Xbox)value;
            return GetEnumDescription(xboxValue);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                    return attributes[0].Description;
                else
                    return value.ToString();
            }

            return value.ToString();
        }
    }
}
