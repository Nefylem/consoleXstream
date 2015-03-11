using System;
using System.Collections.Generic;
using DirectShowLib;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing;
using System.Linq;
using consoleXstream.Config;
using consoleXstream.VideoCapture.Data;
using consoleXstream.VideoCapture.Sampling;

namespace consoleXstream.VideoCapture
{
    public class VideoCapture
    {
        private readonly Classes _class;


        public bool boolActiveVideo { get; private set; }


        public IMediaEvent MediaEvent
        {
            get { return _class.Graph.MediaEvent; }
            set { _class.Graph.MediaEvent = value; }
        }

        public IAMCrossbar _xBar { get; set; }



        private int _intRestartGraph = 0;

        

        private bool _boolBuildingGraph;
        private bool _boolVideoFail;
        
        private bool _IsChangedDisplayResolution;

        public VideoCapture(Form1 mainForm, Configuration System)
        {
            _class = new Classes(this, mainForm, System);
            _class.DeclareClasses();
        }

        public void InitialzeCapture()
        {
            _class.Var.CrossbarAudio = "none";
            _class.Var.CrossbarVideo = "none";

            //Caches build information
            _class.Capture.Find();
            _class.Audio.Find();

            _class.User.LoadSettings();
            _class.Resolution.Find();

            _class.Var.IsInitializeGraph = true;
            //_class.Var.IsRestartGraph = true;
        }

        public void LoadUserSettings() { _class.User.LoadSettings(); }

        #region DirectShow Graph
        public void runGraph()
        {
            _class.Debug.Log("[0] Build capture graph");
            if (_class.Capture.CurrentDevice > -1 && _class.Capture.CurrentDevice < _class.Var.VideoCaptureDevice.Count)
            {
                int hr = 0;

                _boolBuildingGraph = true;
                _class.Debug.Log("Using : " + _class.Var.VideoCaptureDevice[_class.Capture.CurrentDevice]);
                _class.Debug.Log("");

                if (_class.Graph.MediaControl != null) _class.Graph.MediaControl.StopWhenReady();
                if (_class.Resolution.Type.Count == 0) { _class.Resolution.Find();  }

                _class.Graph.CaptureGraph = null;
                _class.Graph.MediaControl = null;
                _class.Graph.MediaEvent = null;
                _class.Graph.VideoWindow = null;
                _class.Graph.VideoDef = null;
                _class.Graph.XBar = null;

                _class.Graph.CaptureGraph = (IGraphBuilder)new FilterGraph();

                if (_class.GraphBuild.BuildGraph())
                {
                    if (!_class.Var.ShowPreviewWindow)
                        _class.Display.Setup();
                    else
                        setupPreviewWindow();

                    _class.Graph.MediaControl = (IMediaControl)_class.Graph.CaptureGraph;
                    _class.Graph.MediaEvent = (IMediaEvent)_class.Graph.CaptureGraph;

                    _class.Debug.Log("");
                    _class.Debug.Log("Run compiled graph");
                    hr = _class.Graph.MediaControl.Run();
                    _class.Debug.Log("[2] " + DsError.GetErrorText(hr));

                    //TODO: Check for major / minor errors before declaring video run
                    boolActiveVideo = true;
                }

                _boolBuildingGraph = false;

                if (_class.Graph.XBar != null)
                    if (_class.Var.CrossbarInput.Count == 0)
                        _class.Crossbar.Output();

                if (_class.Var.IsRestartGraph)
                    _intRestartGraph = 3;
            }
            else
                _class.Debug.Log("[ERR] Unknown capture device");
        }

        //TODO: add to main declaration list 
        private IntPtr _previewWindow;
        private Point _previewBounds;
        private bool _boolPreviewFail;

        public void setPreviewWindowHandle(IntPtr previewHandle)
        {
            _previewWindow = previewHandle;
        }

        public void setPreviewWindowBounds(Point videoBounds)
        {
            _previewBounds = videoBounds;
        }

        private void setupPreviewWindow()
        {
            _class.Debug.Log("[3]***   Attaching running graph to preview window");

            int hr = 0;

            _boolPreviewFail = false;
            try
            {
                _class.Debug.Log("-> putOwner");

                hr = _class.Graph.VideoPreview.put_Owner(_previewWindow);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }

                _class.Debug.Log("-> putWindowStyle");
                hr = _class.Graph.VideoPreview.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }

                if (_class.Graph.VideoWindow != null)
                {
                    _class.Debug.Log("-> setBounds (" + _previewBounds.X.ToString() + ", " + _previewBounds.Y.ToString() + ")");
                    //Point ptReturn = frmMain.setVideoWindowBounds();
                    _class.Debug.Log("-> setWindowPosition");
                    _class.Graph.VideoPreview.SetWindowPosition(0, 0, _previewBounds.X, _previewBounds.Y);
                }
                _class.Debug.Log("-> putVisible");
                hr = _class.Graph.VideoPreview.put_Visible(OABool.True);
                if (hr != 0) { _class.Debug.Log("-> " + DsError.GetErrorText(hr)); }
            }
            catch (Exception e)
            {
                _class.Debug.Log("[ERR] *setupVideoWindow* " + e.ToString());
                _boolPreviewFail = true;
                return;
            }
        }

        public void checkVideoOutput()
        {
            //Reruns the graph once, find this needs to happen after quick resolution changes (PS3)
            if (_class.Var.IsRestartGraph && !_boolVideoFail)           
            {
                _class.Var.IsRestartGraph = false;
                _class.Debug.Log("[3] Update graph");
                runGraph();
            }

            if (!_boolVideoFail && _class.System.boolAutoSetCaptureResolution)
                _class.Resolution.Check();

            if (_class.Var.IsRestartGraph)
            {
                if (_intRestartGraph > 0) 
                    _intRestartGraph--; 
                else 
                { 
                    _class.Debug.Log("[3] Restart graph");
                    _class.Var.IsRestartGraph = false; 
                    _boolVideoFail = false; 
                    runGraph(); 
                }
            }
        }

        //Compares video capture resolution with video out resolution, adjusts graph if needed

        #endregion

        public List<string> GetVideoCaptureDevices()
        {
            if (_class.Var.VideoCaptureDevice == null || _class.Var.VideoCaptureDevice.Count == 0)
            {
                //relist
            }
            return _class.Var.VideoCaptureDevice;
        }

        public List<string> GetVideoCaptureByName()
        {
            if (_class.Var.VideoCaptureDevice == null || _class.Var.VideoCaptureDevice.Count == 0)
                _class.Capture.Find();

            return _class.Var.VideoCaptureDevice;            
        }
        public void SetVideoCaptureDevice(string device) { _class.Capture.Set(device); }
        public void SetPreviewWindow(bool set) { _class.Var.ShowPreviewWindow = set; }
        public List<string> GetVideoResolution() { return _class.Resolution.List; }
        public int GetVideoResolutionCurrent() { return _class.Var.CurrentResolution; }
        public void SetVideoResolution(int setRes) { _class.Var.VideoResolutionIndex = setRes; _class.Var.SetResolution = setRes; }
        public List<string> GetCrossbarList() { return _class.Var.CrossbarInput; }
        public string GetCrossbarOutput(int id, string type) { return _class.Crossbar.List(id, type); }
        public void SetCrossbar(string cross) { _class.GraphCrossbar.setCrossbar(cross); }
        public void CloseGraph() { _class.Close.CloseGraph(); }
        public string GetVideoDevice() { return _class.Var.VideoDevice; }
        public string GetAudioDevice() { return _class.Var.AudioDevice; }
    }

}
