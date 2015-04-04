using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;
using consoleXstream.Input;

namespace consoleXstream.Remap
{

    public class Remapping
    {
        public class RemapCodes
        {
            [Description("Home")]
            public int Home { get; set; }
            [Description("Back")]
            public int Back { get; set; }
            [Description("Start")]
            public int Start { get; set; }

            [Description("Right Shoulder")]
            public int RightShoulder { get; set; }
            [Description("Right Trigger")]
            public int RightTrigger { get; set; }   //0 - 100
            [Description("Right Stick")]
            public int RightStick { get; set; }

            [Description("Left Shoulder")]
            public int LeftShoulder { get; set; }
            [Description("Left Trigger")]
            public int LeftTrigger { get; set; }    //0 - 100
            [Description("Left Stick")]
            public int LeftStick { get; set; }

            [Description("RightThumb X")]
            public int RightX { get; set; }
            [Description("RightThumb Y")]
            public int RightY { get; set; }
            [Description("LeftThumb X")]
            public int LeftX { get; set; }
            [Description("LeftThumb Y")]
            public int LeftY { get; set; }

            [Description("D-Pad Up")]
            public int Up { get; set; }
            [Description("D-Pad Down")]
            public int Down { get; set; }
            [Description("D-Pad Left")]
            public int Left { get; set; }
            [Description("D-Pad Right")]
            public int Right { get; set; }

            [Description("Y")]
            public int Y { get; set; }
            [Description("B")]
            public int B { get; set; }
            [Description("A")]
            public int A { get; set; }
            [Description("X")]
            public int X { get; set; }

            [Description("Touchpad")]
            public int Touch { get; set; }
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
        public RemapCodes RemapGamepad;

        public void SetDefaultGamepad()
        {
            RemapGamepad = new RemapCodes
            {
                Home = 0,
                Back = 1,
                Start = 2,
                RightShoulder = 3,
                RightTrigger = 4,
                RightStick = 5,
                LeftShoulder = 6,
                LeftTrigger = 7,
                LeftStick = 8,
                RightX = 9,
                RightY = 10,
                LeftX = 11,
                LeftY = 12,
                Up = 13,
                Down = 14,
                Left = 15,
                Right = 16,
                Y = 17,
                B = 18,
                A = 19,
                X = 20,
                Touch = 27
            };
        }

        public void LoadGamepadRemap()
        {
            if (!Directory.Exists("Profiles")) return;
            if (!File.Exists(@"Profiles\gamepad.remap")) return;
            
            var setting = "";
            
            var reader = new XmlTextReader(@"Profiles\gamepad.remap");
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
                            AddRemapData(reader.Name, setting);

                        setting = "";
                        break;
                }
            }
            reader.Close();
        }

        private void AddRemapData(string title, string setting)
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

        public void SaveGamepadRemap()
        {
            if (!Directory.Exists("Profiles")) Directory.CreateDirectory("Profiles");

            if (File.Exists(@"Profiles\gamepad.remap")) File.Delete(@"Profiles\gamepad.remap");

            var save = "<Gamepad>";
            var myClassType = RemapGamepad.GetType();
            var properties = myClassType.GetProperties();
            
            foreach (var property in properties)
            {
                var find = property.GetValue(RemapGamepad, null).ToString();
                try
                {
                    var id = Convert.ToInt32(find.Trim());
                    save += "<" + property.Name + ">";
                    save += FindGamepadValue(id);
                    save += "</" + property.Name + ">";
                }
                catch
                {
                    // ignored
                }
            }
            save += "</Gamepad>";

            var doc = new XmlDocument();
            doc.LoadXml(save);
            var settings = new XmlWriterSettings {Indent = true};
            var writer = XmlWriter.Create(@"Profiles\gamepad.remap", settings);
            doc.Save(writer);
            writer.Close();
        }

        static string GetDescription(MemberInfo type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptions.Length > 0 ? descriptions[0].Description : "";
        }

        public int FindRemapName(string title)
        {
            title = title.ToLower();
            var myClassType = RemapGamepad.GetType();
            var properties = myClassType.GetProperties();
            
            foreach (var property in properties)
            {
                var newTitle = GetDescription(property);
                if (title != newTitle.ToLower()) continue;
                try
                {
                    return Convert.ToInt32(property.GetValue(RemapGamepad, null).ToString());
                }
                catch
                {
                    // ignored
                }
            }
            return -1;
        }

        private string FindGamepadValue(int value)
        {
            var xboxValue = (Xbox)value;
            return GetEnumDescription(xboxValue);
        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi == null) return value.ToString();
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public void SetRemapCode(string button, string newCommand)
        {
            var set = FindRemapName(newCommand);
            switch (button.ToLower())
            {
                case "home":
                    RemapGamepad.Home = set;
                    break;
                case "back":
                    RemapGamepad.Back = set; break;
                case "rightshoulder":
                    RemapGamepad.RightShoulder= set; break;
                case "righttrigger":
                    RemapGamepad.RightTrigger = set; break;
                case "rightstick":
                    RemapGamepad.RightStick = set; break;
                case "leftshoulder":
                    RemapGamepad.LeftShoulder = set; break;
                case "leftrigger":
                    RemapGamepad.LeftTrigger = set; break;
                case "leftstick":
                    RemapGamepad.LeftStick = set; break;
                case "rightx":
                    RemapGamepad.RightX = set; break;
                case "righty":
                    RemapGamepad.RightY = set; break;
                case "leftx":
                    RemapGamepad.LeftX = set; break;
                case "lefty":
                    RemapGamepad.LeftY = set; break;
                case "d-pad up":
                    RemapGamepad.Up = set; break;
                case "d-pad down":
                    RemapGamepad.Down = set; break;
                case "d-pad left":
                    RemapGamepad.Left = set; break;
                case "d-pad right":
                    RemapGamepad.Right = set; break;
                case "y":
                    RemapGamepad.Y = set; break;
                case "b":
                    RemapGamepad.B = set; break;
                case "a":
                    RemapGamepad.A = set; break;
                case "x":
                    RemapGamepad.X = set; break;
                case "touch":
                    RemapGamepad.Touch = set; break;
            }
            SaveGamepadRemap();
        }
    }
}
