using System;
using System.Collections.Generic;
using DirectShowLib;
using System.Drawing;
using consoleXstream.Config;
using consoleXstream.Home;

namespace consoleXstream.VideoCapture
{
    public class VideoCapture
    {
        public VideoCapture(BaseClass baseClass)
        {
            baseClass.VideoCapture = this;
            Class = new Classes(baseClass);
        }
        public readonly Classes Class;

        public bool BoolActiveVideo { get; private set; }
        private IntPtr _previewWindow;
        private Point _previewBounds;
        private bool _boolPreviewFail;

        public IMediaEvent MediaEvent
        {
            get { return Class.Graph.MediaEvent; }
            set { Class.Graph.MediaEvent = value; }
        }

        private int _intRestartGraph;
        
        public void InitialzeCapture()
        {
            Class.Var.CrossbarAudio = "none";
            Class.Var.CrossbarVideo = "none";

            Class.Debug.Log("[0] Class.Capture.Find");
            //Caches build information
            Class.Capture.Find();
            Class.Debug.Log("[0] Class.Audio.Find");
            Class.Audio.Find();

            Class.Debug.Log("[0] Class.User.LoadSettings");
            Class.User.LoadSettings();
            Class.Debug.Log("[0] Class.Resolution.Find");
            Class.Resolution.Find();

            Class.Var.IsInitializeGraph = true;
            Class.Var.IsRestartGraph = true;
        }

        public void LoadUserSettings() { Class.User.LoadSettings(); }

        public void RunGraph()
        {
            Class.Debug.Log("[0] Build capture graph");
            if (Class.Capture.CurrentDevice > -1 && Class.Capture.CurrentDevice < Class.Var.VideoCaptureDevice.Count)
            {
                Class.Var.IsBuildingGraph = true;
                Class.Debug.Log("Using : " + Class.Var.VideoCaptureDevice[Class.Capture.CurrentDevice]);
                Class.Debug.Log("");

                if (Class.Graph.MediaControl != null) Class.Graph.MediaControl.StopWhenReady();
                if (Class.Resolution.Type.Count == 0) { Class.Resolution.Find();  }

                Class.Graph.ClearGraph();

                Class.Graph.CaptureGraph = new FilterGraph() as IGraphBuilder;

                if (Class.GraphBuild.BuildGraph())
                {
                    if (!Class.Var.ShowPreviewWindow)
                        Class.Display.Setup();
                    else
                        setupPreviewWindow();

                    Class.Graph.MediaControl = Class.Graph.CaptureGraph as IMediaControl;
                    Class.Graph.MediaEvent = Class.Graph.CaptureGraph as IMediaEvent;

                    Class.Debug.Log("");
                    Class.Debug.Log("Run compiled graph");
                    if (Class.Graph.MediaControl != null)
                    {
                        int hr = Class.Graph.MediaControl.Run();
                        Class.Debug.Log("[2] " + DsError.GetErrorText(hr));
                    }

                    BoolActiveVideo = true;
                }

                Class.Var.IsBuildingGraph = false;

                if (Class.Graph.XBar != null)
                    if (Class.Var.CrossbarInput.Count == 0)
                        Class.Crossbar.Output();

                if (Class.Var.IsRestartGraph)
                    _intRestartGraph = 3;
            }
            else
                Class.Debug.Log("[ERR] Unknown capture device");
        }

        public void ChangeResolution()
        {
            /*
            int hr = 0;
            _class.Graph.MediaControl.Stop();
            _class.GraphPin.ListPin(_class.Graph.CaptureDevice);
            var captureVideoOut = _class.GraphPin.AssumePinOut("Capture", "Video");
            var videoOut = _class.GraphPin.GetPin(_class.Graph.CaptureDevice, captureVideoOut);
            if (videoOut == null)
            {
                _class.Debug.Log("[0] [WARN] Cant find video Capture output to change resolution");
                return;
            }

            IPin videoConnect;
            videoOut.ConnectedTo(out videoConnect);
            if (videoConnect == null) return;

            //videoOut.Disconnect();
            //videoConnect.Disconnect();

            if (_class.Var.VideoResolutionIndex == 0 || _class.System.IsAutoSetCaptureResolution)
                _class.GraphResolution.Get();

            if (_class.Var.VideoResolutionIndex > 0)
            {
                hr = _class.GraphResolution.Set(_class.Graph.CaptureDevice, captureVideoOut);
                MessageBox.Show(DsError.GetErrorText(hr));
                //hr = (IAMStreamConfig) videoConnect.SetFormat(_class.Graph.Resolution);
                //hr = ((IAMStreamConfig)_class.GraphPin.GetPin(_class.Graph.CaptureFeed, _class.Graph.CaptureFeedIn)).SetFormat(_class.Resolution.Type[_class.Var.VideoResolutionIndex]);
                //MessageBox.Show(DsError.GetErrorText(hr));
                //var inPin = videoConnect as IAMStreamConfig;
                //hr = inPin.SetFormat(_class.Graph.Resolution);
                //MessageBox.Show(DsError.GetErrorText(hr));
                //hr = videoConnect as (IAMStreamConfig).SetFormat(_class.Resolution.Type[_class.Var.VideoResolutionIndex]);
                //hr = ((IAMStreamConfig)_class.GraphPin.GetPin(videoConnect).SetFormat(_class.Resolution.Type[_class.Var.VideoResolutionIndex]);
            }
            else
                _class.Debug.Log("[0] [WARN] Cant find capture resolution - no input or unknown resolution type");

            _class.FrmMain.Text = "set " + _class.Var.CurrentResByName;

            hr = videoOut.Connect(videoConnect, _class.Graph.Resolution);
            _class.FrmMain.Text = DsError.GetErrorText(hr);

            //_class.FrmMain.Text = DsError.GetErrorText(hr);
            hr = _class.Graph.MediaControl.Run();

            /*
            if (_class.Var.VideoResolutionIndex == 0 || _class.System.IsAutoSetCaptureResolution)
                _class.GraphResolution.Get();

            if (_class.Var.VideoResolutionIndex > 0)
            {
                hr = _class.GraphResolution.Set(_class.Graph.CaptureDevice, captureVideoOut);
            }
            else
                _class.Debug.Log("[0] [WARN] Cant find capture resolution - no input or unknown resolution type");

            hr = videoConnect.ConnectionMediaType(_class.Graph.Resolution);
            MessageBox.Show(DsError.GetErrorText(hr));
            //hr = videoOut.Connect(videoConnect, _class.Graph.Resolution);
            //MessageBox.Show(DsError.GetErrorText(hr));
            //hr = videoConnect.ReceiveConnection(videoOut, _class.Graph.Resolution);
            //MessageBox.Show(DsError.GetErrorText(hr));
            hr = _class.Graph.CaptureGraph.ConnectDirect(videoOut, videoConnect, null);
            MessageBox.Show(DsError.GetErrorText(hr));
            /*

            if (_class.Var.VideoResolutionIndex == 0 || _class.System.IsAutoSetCaptureResolution)
                _class.GraphResolution.Get();

            if (_class.Var.VideoResolutionIndex > 0)
                _class.GraphResolution.Set(_class.Graph.CaptureDevice, captureVideoOut);
            else
                _class.Debug.Log("[0] [WARN] Cant find capture resolution - no input or unknown resolution type");

            videoConnect.ReceiveConnection(videoOut, _class.Resolution.Type[_class.Var.VideoResolutionIndex]);
            var hr = _class.Graph.CaptureGraph.ConnectDirect(videoOut, videoConnect, null);
            _class.Debug.Log("[0] &&& Reconnect to running graph");
            _class.Debug.Log("-> " + DsError.GetErrorText(hr));
            MessageBox.Show(DsError.GetErrorText(hr));
            
            hr = _class.Graph.MediaControl.Run();
            _class.Debug.Log("[2] " + DsError.GetErrorText(hr));
            MessageBox.Show(DsError.GetErrorText(hr));
            */


            //_class.Graph.CaptureDevice.FindPin("Capture", out videoOutput);

            /*
            IPin videoOutput;
            if (videoOutput == null)
            {
                MessageBox.Show("vo null)");
                return;
            }
            IPin videoConnection;
            videoOutput.ConnectedTo(out videoConnection);
            videoOutput.Disconnect();
            */

        }


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
            /*
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
             */
        }

        public void CheckVideoOutput()
        {
            //Reruns the graph once, find this needs to happen after quick resolution changes (PS3)
            if (Class.Var.IsRestartGraph && !Class.Var.IsVideoFail)
            {
                Class.Var.IsRestartGraph = false;
                Class.Debug.Log("[3] Update graph");
                RunGraph();
            }

            if (!Class.Var.IsVideoFail && Class.System.IsAutoSetCaptureResolution)
                Class.Resolution.Check();

            if (Class.Var.IsRestartGraph)
            {
                if (_intRestartGraph > 0) 
                    _intRestartGraph--; 
                else 
                {
                    Class.Debug.Log("[3] Restart graph");
                    Class.Var.IsRestartGraph = false;
                    Class.Var.IsVideoFail = false; 
                    RunGraph(); 
                }
            }
        }

        public List<string> GetVideoCaptureDevices()
        {
            if (Class.Var.VideoCaptureDevice == null || Class.Var.VideoCaptureDevice.Count == 0)
            {
                //relist
            }
            return Class.Var.VideoCaptureDevice;
        }

        public List<string> GetVideoCaptureByName()
        {
            if (Class.Var.VideoCaptureDevice == null || Class.Var.VideoCaptureDevice.Count == 0)
                Class.Capture.Find();

            return Class.Var.VideoCaptureDevice;            
        }
        public void SetVideoCaptureDevice(string device) { Class.Capture.Set(device); }
        public void SetPreviewWindow(bool set) { Class.Var.ShowPreviewWindow = set; }
        public List<string> GetVideoResolution() { return Class.Resolution.List; }
        public int GetVideoResolutionCurrent() { return Class.Var.CurrentResolution; }
        public void SetVideoResolution(int setRes) { Class.Var.VideoResolutionIndex = setRes; Class.Var.SetResolution = setRes; }
        public List<string> GetCrossbarList() { return Class.Var.CrossbarInput; }
        public string GetCrossbarOutput(int id, string type) { return Class.Crossbar.List(id, type); }
        public void SetCrossbar(string cross) { Class.GraphCrossbar.setCrossbar(cross); }
        public void CloseGraph() { Class.Close.CloseGraph(); }
        public string GetVideoDevice() { return Class.Var.VideoDevice; }
        public string GetAudioDevice() { return Class.Var.AudioDevice; }
        public string GetCrossbarSetting(string type) { return Class.Crossbar.GetActive(type); }
        public int GetCrossbarId(string type) { return Class.Crossbar.GetActiveId(type); }
        public void SetDisplay(string res) { Class.Var.CurrentResByName = res; }
    }

}
