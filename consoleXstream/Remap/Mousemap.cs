using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using consoleXstream.Home;
using consoleXstream.Input;

namespace consoleXstream.Remap
{
    public class Mousemap
    {
        public Mousemap(BaseClass classes)
        {
            _class = classes;
            InitializeMouseDefaults();
        }

        private BaseClass _class;

        public class MouseCodes
        {
            [Description("Left Mouse Button")]
            public int Lmb { get; set; }

            [Description("Right Mouse Button")]
            public int Rmb { get; set; }
        }

        public MouseCodes RemapMouse;

        private void InitializeMouseDefaults()
        {
            RemapMouse = new MouseCodes
            {
                Lmb = 6,
                Rmb = 3
            };

            SaveGamepadRemap();
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
            title = title.ToLower();
            int id = FindRemapName(setting);
            if (id > -1)
            {
                Type myClassType = RemapMouse.GetType();
                PropertyInfo[] properties = myClassType.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (title == property.Name.ToLower())
                        if ((string)property.GetValue(RemapMouse, null) != id.ToString())
                        {
                            property.SetValue(RemapMouse, Convert.ChangeType(id, property.PropertyType), null);
                            break;
                        }
                }

            }
        }

        public int FindRemapName(string title)
        {
            title = title.ToLower();
            var myClassType = RemapMouse.GetType();
            var properties = myClassType.GetProperties();

            foreach (var property in properties)
            {
                var newTitle = GetDescription(property);
                if (title != newTitle.ToLower()) continue;
                try
                {
                    return Convert.ToInt32(property.GetValue(RemapMouse, null).ToString());
                }
                catch
                {
                    // ignored
                }
            }
            return -1;
        }

        static string GetDescription(MemberInfo type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptions.Length > 0 ? descriptions[0].Description : "";
        }

        public void SaveGamepadRemap()
        {
            if (!Directory.Exists("Profiles")) Directory.CreateDirectory("Profiles");

            if (File.Exists(@"Profiles\mouse.remap")) File.Delete(@"Profiles\mouse.remap");

            var save = "<Mouse>";
            var myClassType = RemapMouse.GetType();
            var properties = myClassType.GetProperties();

            foreach (var property in properties)
            {
                var find = property.GetValue(RemapMouse, null).ToString();
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
            save += "</Mouse>";

            var doc = new XmlDocument();
            doc.LoadXml(save);
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(@"Profiles\mouse.remap", settings);
            doc.Save(writer);
            writer.Close();
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
    }
}
