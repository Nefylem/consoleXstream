using System.IO;
using System.Xml;

namespace consoleXstream.Config
{
    public class XmlData
    {
        public XmlData(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Read()
        {
            _class.Var.IsReadData = true;
            string strSetting = "";
            if (File.Exists("config.xml"))
            {
                var reader = new XmlTextReader("config.xml");
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.
                            //MessageBox.Show("<" + reader.Name);
                            break;
                        case XmlNodeType.Text: //Display the text in each element.
                            strSetting = reader.Value;
                            break;
                        case XmlNodeType.EndElement: //Display the end of the element.
                            if (strSetting.Length > 0) { _class.Set.Add(reader.Name, strSetting); }
                            strSetting = "";
                            break;
                    }
                }
                reader.Close();
            }
            _class.Var.IsReadData = false;
        }

        public void Save()
        {
            string strSave = "<Configuration>";
            strSave += "<Title>consoleXstream v.0.01</Title>";

            strSave += "<videoCaptureSettings>";
            strSave += _class.Set.Find("VideoCaptureDevice");
            strSave += _class.Set.Find("AudioPlaybackDevice");
            strSave += _class.Set.CheckData("CaptureResolution");
            strSave += _class.Set.CheckData("crossbarVideoPin");
            strSave += _class.Set.CheckData("crossbarAudioPin");
            strSave += _class.Set.CheckData("AVIRender");
            strSave += _class.Set.CheckData("CheckCaptureRes");
            strSave += "</videoCaptureSettings>";

            strSave += "<VR_Mode>";
            strSave += _class.Set.CheckData("VR_Video");
            strSave += _class.Set.CheckData("VR_Width");
            strSave += _class.Set.CheckData("VR_Height");
            strSave += _class.Set.CheckData("VR_OffsetWidth");
            strSave += _class.Set.CheckData("VR_OffsetHeight");
            strSave += "</VR_Mode>";

            strSave += "<DisplaySettings>";
            strSave += _class.Set.CheckData("AutoResolution");
            strSave += _class.Set.CheckData("RefreshRate");
            strSave += _class.Set.CheckData("Resolution");
            strSave += _class.Set.CheckData("StayOnTop");
            strSave += "</DisplaySettings>";

            strSave += "<Controller>";
            strSave += _class.Set.CheckData("DS4Emulation");
            strSave += _class.Set.CheckData("Normalize");
            strSave += _class.Set.CheckData("Rumble");
            strSave += "</Controller>";

            strSave += _class.Set.CheckData("CurrentProfile");
            strSave += _class.Set.CheckData("ControllerMax");
            strSave += _class.Set.CheckData("TitanOne");
            strSave += _class.Set.CheckData("UseTitanOne");
            strSave += _class.Set.CheckData("MenuLog");

            strSave += "</Configuration>";

            // Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strSave);
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
            XmlWriter writer = XmlWriter.Create("config.xml", settings);
            doc.Save(writer);
            writer.Close();
        }

    }
}
