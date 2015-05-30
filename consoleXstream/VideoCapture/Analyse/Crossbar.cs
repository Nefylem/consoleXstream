using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirectShowLib;

namespace consoleXstream.VideoCapture.Analyse
{
    public class Crossbar
    {
        public Crossbar(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public string List(int id, string strType)
        {
            var returnVal = "";
            if (_class.Var.CrossbarInput.Count == 0) { Output(); }
            if (_class.Var.CrossbarInput.Count <= 0) return returnVal;
            
            var intConnector = 0;
            foreach (var strTemp in _class.Var.CrossbarInput.Where(strTemp => strTemp.Length > "video_".Length))
            {
                if (strType.ToLower() == "video")
                    if (strTemp.Substring(0, "video_".Length).ToLower() == "video_")
                        if (intConnector == id) { returnVal = strTemp; }

                if (strType.ToLower() == "audio")
                    if (strTemp.Substring(0, "audio".Length).ToLower() == "audio")
                        if (intConnector == id) { returnVal = strTemp; }
                
                intConnector++;
            }
            return returnVal;
        }

        public void Output()
        {
            if (_class.Graph.XBar != null)
            {
                _class.Var.CrossbarInput = new List<string>();

                int inPin;
                int outPin;

                _class.Graph.XBar.get_PinCounts(out inPin, out outPin);

                for (var intCount = 0; intCount < outPin; intCount++)
                {
                    int intRouted;
                    int intPinId;
                    PhysicalConnectorType pinType;

                    _class.Graph.XBar.get_CrossbarPinInfo(true, intCount, out intPinId, out pinType);
                    _class.Graph.XBar.get_IsRoutedTo(intCount, out intRouted);

                    _class.Var.CrossbarInput.Add(pinType.ToString());

                    _class.Debug.Log(intCount + " / " + pinType);
                }
            }
            else { _class.Debug.Log("No crossbar found"); }
        }

        private void write(string write)
        {
            var txtOut = new StreamWriter("getactive.txt", true);
            txtOut.WriteLine(write);
            txtOut.Close();
        }

        public string GetActive(string type)
        {
            try
            {
                var video = "";
                var audio = "";

                if (_class.Graph.XBar != null)
                {
                    int intPinVideo;
                    int intPinAudio;

                    _class.Graph.XBar.get_IsRoutedTo(0, out intPinVideo);
                    _class.Graph.XBar.get_IsRoutedTo(1, out intPinAudio);

                    write("getcrossbaroutput ( video");
                    write("intPinVideo>" + intPinVideo);
                    if (_class.VideoCapture == null) write("videocapture == null");
                    video = _class.VideoCapture.GetCrossbarOutput(intPinVideo, "Video");
                    write("getcrossbaroutput ( audio");
                    audio = _class.VideoCapture.GetCrossbarOutput(intPinAudio, "Audio");
                }
                write("> " + type);
                return type.ToLower() == "video" ? video : audio;
            }
            catch (Exception ex)
            {
                write(ex.Message);
                throw;
            }

        }

        public int GetActiveId(string type)
        {
            int intPinVideo = 0;
            int intPinAudio = 0;
            if (_class.Graph.XBar != null)
            {
                _class.Graph.XBar.get_IsRoutedTo(0, out intPinVideo);
                _class.Graph.XBar.get_IsRoutedTo(1, out intPinAudio);

            }                
            return type.ToLower() == "video" ? intPinVideo : intPinAudio;
        }

    }
}
