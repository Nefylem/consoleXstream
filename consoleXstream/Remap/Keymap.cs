using System;
using System.IO;
using System.Reflection;
using System.Xml;
using consoleXstream.Home;

namespace consoleXstream.Remap
{
    public class Keymap
    {
        public Keymap(BaseClass classes)
        {
            _class = classes;
            InitializeKeyboardDefaults();
        }

        private BaseClass _class;

        public class KeyboardKeys
        {
            public string DpadDown { get; set; }
            public string DpadUp { get; set; }
            public string DpadLeft { get; set; }
            public string DpadRight { get; set; }

            public string ButtonA { get; set; }
            public string ButtonB { get; set; }
            public string ButtonX { get; set; }
            public string ButtonY { get; set; }

            public string LXleft { get; set; }
            public string LXright { get; set; }
            public string LYup { get; set; }
            public string LYdown { get; set; }

            public string RXleft { get; set; }
            public string RXright { get; set; }
            public string RYup { get; set; }
            public string RYdown { get; set; }

            public string Modifier { get; set; }
            public string RightModifier { get; set; }

            public string ButtonBack { get; set; }
            public string ButtonStart { get; set; }
            public string ButtonHome { get; set; }
        }

        public KeyboardKeys KeyDef;
        public KeyboardKeys KeyAltDef;

        public void InitializeKeyboardDefaults()
        {
            KeyDef = new KeyboardKeys();
            KeyAltDef = new KeyboardKeys();

            KeyDef.DpadDown = "DOWN";
            KeyDef.DpadUp = "UP";
            KeyDef.DpadLeft = "LEFT";
            KeyDef.DpadRight = "RIGHT";

            KeyDef.ButtonY = "KEY_I";
            KeyDef.ButtonX = "KEY_J";
            KeyDef.ButtonA = "KEY_K";
            KeyDef.ButtonB = "KEY_L";
            
            KeyDef.LXleft = "KEY_A";
            KeyDef.LXright = "KEY_D";
            KeyDef.LYup = "KEY_W";
            KeyDef.LYdown = "KEY_S";

            KeyDef.RXleft = "NUMPAD4";
            KeyDef.RXright = "NUMPAD6";
            KeyDef.RYup = "NUMPAD8";
            KeyDef.RYdown = "NUMPAD2";

            KeyDef.Modifier = "LSHIFT";
            KeyDef.RightModifier = "RSHIFT";

            KeyDef.ButtonBack = "ESCAPE";
            KeyDef.ButtonHome = "F2";
            KeyDef.ButtonStart = "F3";

            KeyAltDef.ButtonBack = "F4";

            KeyAltDef.ButtonA = "RETURN";
            KeyAltDef.ButtonB = "BACK";

            KeyDef = LoadKeyboardInputs(KeyDef, "keyboard");
            KeyAltDef = LoadKeyboardInputs(KeyAltDef, "keyboardAlt");

            SaveKeyboardRemap(KeyDef, "keyboard");
            SaveKeyboardRemap(KeyAltDef, "keyboardAlt");
        }

        public KeyboardKeys LoadKeyboardInputs(KeyboardKeys target, string file)
        {
            if (!Directory.Exists("Profiles")) return target;
            if (!File.Exists(@"Profiles\" + file + ".remap")) return target;

            var setting = "";

            var reader = new XmlTextReader(@"Profiles\" + file + ".remap");
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
                        {
                            var title = reader.Name.ToLower();
                            Type myClassType = target.GetType();
                            PropertyInfo[] properties = myClassType.GetProperties();
                            foreach (PropertyInfo property in properties)
                            {
                                if (title == property.Name.ToLower())
                                    if ((string)property.GetValue(target, null) != setting)
                                    {
                                        property.SetValue(target, Convert.ChangeType(setting, property.PropertyType), null);
                                        break;
                                    }
                            }
                        }
                        setting = "";
                        break;
                }
            }
            reader.Close();
            return target;
        }

        public void SaveKeyboardRemap(KeyboardKeys target, string file)
        {
            if (!Directory.Exists("Profiles")) Directory.CreateDirectory("Profiles");

            if (File.Exists(@"Profiles\" + file + ".remap")) File.Delete(@"Profiles\" + file + ".remap");

            var save = "<" + file + ">";
            var myClassType = target.GetType();
            var properties = myClassType.GetProperties();

            foreach (var property in properties)
            {
                try
                {
                    var find = property.GetValue(target, null).ToString();
                    save += "<" + property.Name + ">";
                    save += find;
                    save += "</" + property.Name + ">";
                }
                catch (Exception)
                {
                    //Ignored                    
                }
            }
            save += "</" + file + ">";

            var doc = new XmlDocument();
            doc.LoadXml(save);
            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(@"Profiles\" + file + ".remap", settings);
            doc.Save(writer);
            writer.Close();
        }
    }
}
