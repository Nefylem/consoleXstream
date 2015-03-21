using System;
using System.Collections.Generic;
using DirectShowLib;
using System.Drawing;
using System.Windows.Forms;
using consoleXstream.Config;
using consoleXstream.Menu.SubMenuOptions;

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

        private int _intRestartGraph = 0;
        
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
            _class.Var.IsRestartGraph = true;
        }

        public void LoadUserSettings() { _class.User.LoadSettings(); }

        public void RunGraph()
        {
            _class.Debug.Log("[0] Build capture graph");
            if (_class.Capture.CurrentDevice > -1 && _class.Capture.CurrentDevice < _class.Var.VideoCaptureDevice.Count)
            {
                int hr = 0;

                _class.Var.IsBuildingGraph = true;
                _class.Debug.Log("Using : " + _class.Var.VideoCaptureDevice[_class.Capture.CurrentDevice]);
                _class.Debug.Log("");

                if (_class.Graph.MediaControl != null) _class.Graph.MediaControl.StopWhenReady();
                if (_class.Resolution.Type.Count == 0) { _class.Resolution.Find();  }

                _class.Graph.ClearGraph();

                _class.Graph.CaptureGraph = new FilterGraph() as IGraphBuilder;

                if (_class.GraphBuild.BuildGraph())
                {
                    if (!_class.Var.ShowPreviewWindow)
                        _class.Display.Setup();
                    else
                        setupPreviewWindow();

                    _class.Graph.MediaControl = _class.Graph.CaptureGraph as IMediaControl;
                    _class.Graph.MediaEvent = _class.Graph.CaptureGraph as IMediaEvent;

                    _class.Debug.Log("");
                    _class.Debug.Log("Run compiled graph");
                    hr = _class.Graph.MediaControl.Run();
                    _class.Debug.Log("[2] " + DsError.GetErrorText(hr));

                    boolActiveVideo = true;
                }

                _class.Var.IsBuildingGraph = false;

                if (_class.Graph.XBar != null)
                    if (_class.Var.CrossbarInput.Count == 0)
                        _class.Crossbar.Output();

                if (_class.Var.IsRestartGraph)
                    _intRestartGraph = 3;
            }
            else
                _class.Debug.Log("[ERR] Unknown capture device");
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

        public void checkVideoOutput()
        {
            //Reruns the graph once, find this needs to happen after quick resolution changes (PS3)
            if (_class.Var.IsRestartGraph && !_class.Var.IsVideoFail)           
            {
                _class.Var.IsRestartGraph = false;
                _class.Debug.Log("[3] Update graph");
                RunGraph();
            }

            if (!_class.Var.IsVideoFail && _class.System.IsAutoSetCaptureResolution)
                _class.Resolution.Check();

            if (_class.Var.IsRestartGraph)
            {
                if (_intRestartGraph > 0) 
                    _intRestartGraph--; 
                else 
                { 
                    _class.Debug.Log("[3] Restart graph");
                    _class.Var.IsRestartGraph = false;
                    _class.Var.IsVideoFail = false; 
                    RunGraph(); 
                }
            }
        }

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
        public string GetCrossbarSetting(string type) { return _class.Crossbar.GetActive(type); }
        public int GetCrossbarId(string type) { return _class.Crossbar.GetActiveId(type); }
    }

}
