using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using DirectShowLib;

namespace consoleXstream.VideoCapture.Analyse
{
    public class Resolution
    {
        public Resolution(Classes inClass) { _class = inClass; }
        private readonly Classes _class;

        private string _checkCaptureRes = "";
        private string _checkCaptureHeight;
        private int _rerunGraphCount;
        private int _rerunWait;
        private bool donttryagain;
        public List<string> List;
        public List<AMMediaType> Type;
        public int Current;

        public void Find()
        {
            try
            {
                _class.Debug.Log("[0] Find video device resolution");

                List = new List<string> {"Auto"};

                Type = new List<AMMediaType> {null};

                var dev = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)[_class.Capture.CurrentDevice];
                // ReSharper disable once SuspiciousTypeConversion.Global
                var filterGraph = (IFilterGraph2) new FilterGraph();
                IBaseFilter baseDev;

                filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out baseDev);
                var pin = DsFindPin.ByCategory(baseDev, PinCategory.Capture, 0);
                // ReSharper disable once SuspiciousTypeConversion.Global
                var streamConfig = (IAMStreamConfig) pin;
                int iC, iS;
                streamConfig.GetNumberOfCapabilities(out iC, out iS);
                var ptr = Marshal.AllocCoTaskMem(iS);

                for (var i = 0; i < iC; i++)
                {
                    AMMediaType media;
                    streamConfig.GetStreamCaps(i, out media, ptr);
                    var v = new VideoInfoHeader();
                    Marshal.PtrToStructure(media.formatPtr, v);

                    if (v.BmiHeader.Width == 0) continue;
                    var strRes = v.BmiHeader.Width + " x " + v.BmiHeader.Height;
                    strRes += "  [" + CheckMediaType(media) + "]";
                    Type.Add(media);
                    List.Add(strRes);
                    _class.Debug.Log("->" + strRes);
                }

                _class.System.strCurrentResolution = List[Current];

                _class.Debug.Log("");
            }
            catch (Exception e)
            {
                _class.Debug.Log("[ERR] fail find video resolution : " + e);
            }
        }

        public void Check()
        {
            if (_class.System.IsAutoSetCaptureResolution)
                checkVideoResolution();
        }

        private void checkVideoResolution()
        {
            /*
            int intVideoWidth = 0;
            int intVideoHeight = 0;
            int intLineCount = 0;

            if (_class.Var.CurrentResByName != _checkCaptureRes)
            {
                _class.System.Debug("VideoResolution.Log", "Comparing: " + _class.Var.CurrentResByName + " to " + intLineCount);
                _checkCaptureRes = _class.Var.CurrentResByName;
                if (_checkCaptureRes.IndexOf('x') > -1)
                {
                    _checkCaptureHeight = _checkCaptureRes.Substring(_checkCaptureRes.IndexOf('x') + 1).Trim();
                    if (_checkCaptureHeight.IndexOf(" ", StringComparison.Ordinal) > -1)
                        _checkCaptureHeight = _checkCaptureHeight.Substring(0, _checkCaptureHeight.IndexOf(" ", StringComparison.Ordinal));
                }
            }

            if (_class.Graph.IamAvd != null)
                _class.Graph.IamAvd.get_NumberOfLines(out intLineCount);


            if (intLineCount == 0) return;
            /*
            if (_IsChangedDisplayResolution)
            {
                _IsChangedDisplayResolution = false;
                runGraph();
            }
            *//*
            if (intLineCount > 0)
            {
                _class.FrmMain.intLineSample = intLineCount;
                //_class.FrmMain.Text = intLineCount.ToString();

                if (_class.Var.IsBuildingGraph == false)
                {
                    if (!_boolRerunGraph)
                    {
                        if (_class.Var.IsInitializeGraph == true)           //Do another swap if running on PS3. Counters freezing display
                        {
                            _class.Var.IsInitializeGraph = false;
                            if (intLineCount == 720)
                            {
                                _intVideoResolution = 0;
                                debugVideo("[1] Change res 720");
                                runGraph();
                            }
                        }

                        //Compares video output to display output
                        if (intLineCount != intVideoHeight)
                        {
                            _boolBuildingGraph = true;
                            if (intLineCount == 720) { _boolRerunGraph = true; debugVideo("[0] &FindMe& Rerun graph * 720p"); }
                            _intVideoResolution = 0;
                            if (_boolVideoFail == false)
                            {
                                debugVideo("[1] Change Res : " + intLineCount.ToString());

                                system.autoChangeRes(intLineCount);

                                runGraph();
                            }
                        }
                    }
                    else
                    {
                        //Forces a second display change if moving to 720 (mainly PS3 needs this)
                        if (intLineCount == 720)
                        {
                            debugVideo("[1] Rerun Graph : 720");
                            _boolRerunGraph = false;
                            runGraph();
                        }
                    }
                }
            }
               */
        }

        private void CheckCaptureResolution()
        {
            if (_class.Var.IsBuildingGraph) return;

            var intLineCount = 0;

            if (_class.Graph.IamAvd != null)
                _class.Graph.IamAvd.get_NumberOfLines(out intLineCount);

            if (intLineCount == 0) return;

            if (_class.Var.CurrentResByName != _checkCaptureRes)
            {
                _class.System.Debug("VideoResolution.Log", "Comparing: " + _class.Var.CurrentResByName + " to " + intLineCount);
                _checkCaptureRes = _class.Var.CurrentResByName;
                if (_checkCaptureRes.IndexOf('x') > -1)
                {
                    _checkCaptureHeight = _checkCaptureRes.Substring(_checkCaptureRes.IndexOf('x') + 1).Trim();
                    if (_checkCaptureHeight.IndexOf(" ", StringComparison.Ordinal) > -1)
                        _checkCaptureHeight = _checkCaptureHeight.Substring(0, _checkCaptureHeight.IndexOf(" ", StringComparison.Ordinal));
                }
            }

            if (_checkCaptureHeight == intLineCount.ToString() && _rerunGraphCount == 0) return;


            //Forces a second graph build
            if (_rerunGraphCount > 0)
            {
                if (_rerunWait > 0)
                {
                    _rerunWait--;
                    return;
                }
                else
                {
                    _rerunGraphCount--;
                    _class.System.Debug("VideoResolution.log", "Rerun capture resolution: " + _rerunGraphCount);
                    
                    _class.Graph.MediaControl.Stop();
                    _class.Graph.ClearGraph();
                    _class.VideoCapture.RunGraph();
                    return;                    
                }
            }

            _class.System.Debug("VideoResolution.log", "Capture Resolution Mismatch");
            _class.System.Debug("VideoResolution.log", "Current Output: " + intLineCount);
            _class.System.Debug("VideoResolution.log", "Expected Output: " + _checkCaptureHeight);
            if (intLineCount == 720)
            {
                _class.System.Debug("VideoResolution.log", "Setting 720p refresh mode");
                _rerunWait = 50;
                _rerunGraphCount = 1;
            }
            else
            {
                _rerunWait = 0;
                _rerunGraphCount = 0;                
            }
            _class.System.Debug("VideoResolution.log", "Running graph");
            //donttryagain = true;
            //_class.VideoCapture.ChangeResolution();
            _class.Graph.MediaControl.Stop();
            _class.Graph.ClearGraph();
            _class.VideoCapture.RunGraph();
        }

        public void ForceRebuildAfterResolution()
        {
            //_IsChangedDisplayResolution = true;
        }


        // ReSharper disable once FunctionComplexityOverflow
        private static string CheckMediaType(AMMediaType media)
        {
            if (media.subType == MediaSubType.A2B10G10R10) { return "A2B10G10R10"; }
            if (media.subType == MediaSubType.A2R10G10B10) { return "A2R10G10B10"; }
            if (media.subType == MediaSubType.AI44) { return "AI44"; }
            if (media.subType == MediaSubType.AIFF) { return "AI44"; }
            if (media.subType == MediaSubType.AnalogVideo_NTSC_M) { return "AnalogVideo_NTSC_M"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_B) { return "AnalogVideo_PAL_B"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_D) { return "AnalogVideo_PAL_D"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_G) { return "AnalogVideo_PAL_G"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_H) { return "AnalogVideo_PAL_H"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_I) { return "AnalogVideo_PAL_I"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_M) { return "AnalogVideo_PAL_M"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_N) { return "AnalogVideo_PAL_N"; }
            if (media.subType == MediaSubType.AnalogVideo_PAL_N_COMBO) { return "AnalogVideo_PAL_N_COMBO"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_B) { return "AnalogVideo_SECAM_B"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_D) { return "AnalogVideo_SECAM_D"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_G) { return "AnalogVideo_SECAM_G"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_H) { return "AnalogVideo_SECAM_H"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_K) { return "AnalogVideo_SECAM_K"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_K1) { return "AnalogVideo_SECAM_K1"; }
            if (media.subType == MediaSubType.AnalogVideo_SECAM_L) { return "AnalogVideo_SECAM_L"; }
            if (media.subType == MediaSubType.ARGB1555) { return "ARGB1555"; }
            if (media.subType == MediaSubType.ARGB1555_D3D_DX7_RT) { return "ARGB1555_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.ARGB1555_D3D_DX9_RT) { return "ARGB1555_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.ARGB32) { return "ARGB32"; }
            if (media.subType == MediaSubType.ARGB32_D3D_DX7_RT) { return "ARGB32_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.ARGB32_D3D_DX9_RT) { return "ARGB32_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.ARGB4444) { return "ARGB4444"; }
            if (media.subType == MediaSubType.ARGB4444_D3D_DX7_RT) { return "ARGB4444_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.ARGB4444_D3D_DX9_RT) { return "ARGB4444_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.Asf) { return "Asf"; }
            if (media.subType == MediaSubType.AtscSI) { return "AtscSI"; }
            if (media.subType == MediaSubType.AU) { return "AU"; }
            if (media.subType == MediaSubType.Avi) { return "Avi"; }
            if (media.subType == MediaSubType.AYUV) { return "AYUV"; }
            if (media.subType == MediaSubType.CFCC) { return "CFCC"; }
            if (media.subType == MediaSubType.CLJR) { return "CLJR"; }
            if (media.subType == MediaSubType.CPLA) { return "CPLA"; }
            if (media.subType == MediaSubType.Data708_608) { return "Data708_608"; }
            if (media.subType == MediaSubType.DOLBY_AC3_SPDIF) { return "DOLBY_AC3_SPDIF"; }
            if (media.subType == MediaSubType.DolbyAC3) { return "DolbyAC3"; }
            if (media.subType == MediaSubType.DRM_Audio) { return "DRM_Audio"; }
            if (media.subType == MediaSubType.DssAudio) { return "DssAudio"; }
            if (media.subType == MediaSubType.DssVideo) { return "DssVideo"; }
            if (media.subType == MediaSubType.DtvCcData) { return "DtvCcData"; }
            if (media.subType == MediaSubType.dv25) { return "dv25"; }
            if (media.subType == MediaSubType.dv50) { return "dv50"; }
            if (media.subType == MediaSubType.DvbSI) { return "DvbSI"; }
            if (media.subType == MediaSubType.DVCS) { return "DVCS"; }
            if (media.subType == MediaSubType.dvh1) { return "dvh1"; }
            if (media.subType == MediaSubType.dvhd) { return "dvhd"; }
            if (media.subType == MediaSubType.DVSD) { return "DVSD"; }
            if (media.subType == MediaSubType.dvsl) { return "dvsl"; }
            if (media.subType == MediaSubType.H264) { return "H264"; }
            if (media.subType == MediaSubType.I420) { return "I420"; }
            if (media.subType == MediaSubType.IA44) { return "IA44"; }
            if (media.subType == MediaSubType.IEEE_FLOAT) { return "IEEE_FLOAT"; }
            if (media.subType == MediaSubType.IF09) { return "IF09"; }
            if (media.subType == MediaSubType.IJPG) { return "IJPG"; }
            if (media.subType == MediaSubType.IMC1) { return "IMC1"; }
            if (media.subType == MediaSubType.IMC2) { return "IMC2"; }
            if (media.subType == MediaSubType.IMC3) { return "IMC3"; }
            if (media.subType == MediaSubType.IMC4) { return "IMC4"; }
            if (media.subType == MediaSubType.IYUV) { return "IYUV"; }
            if (media.subType == MediaSubType.Line21_BytePair) { return "Line21_BytePair"; }
            if (media.subType == MediaSubType.Line21_GOPPacket) { return "Line21_GOPPacket"; }
            if (media.subType == MediaSubType.Line21_VBIRawData) { return "Line21_VBIRawData"; }
            if (media.subType == MediaSubType.MDVF) { return "MDVF"; }
            if (media.subType == MediaSubType.MJPG) { return "MJPG"; }
            if (media.subType == MediaSubType.MPEG1AudioPayload) { return "MPEG1AudioPayload"; }
            if (media.subType == MediaSubType.MPEG1Packet) { return "MPEG1Packet"; }
            if (media.subType == MediaSubType.MPEG1Payload) { return "MPEG1Payload"; }
            if (media.subType == MediaSubType.MPEG1System) { return "MPEG1System"; }
            if (media.subType == MediaSubType.MPEG1SystemStream) { return "MPEG1SystemStream"; }
            if (media.subType == MediaSubType.MPEG1Video) { return "MPEG1Video"; }
            if (media.subType == MediaSubType.MPEG1VideoCD) { return "MPEG1VideoCD"; }
            if (media.subType == MediaSubType.Mpeg2Audio) { return "Mpeg2Audio"; }
            if (media.subType == MediaSubType.Mpeg2Data) { return "Mpeg2Data"; }
            if (media.subType == MediaSubType.Mpeg2Program) { return "Mpeg2Program"; }
            if (media.subType == MediaSubType.Mpeg2Transport) { return "Mpeg2Transport"; }
            if (media.subType == MediaSubType.Mpeg2TransportStride) { return "Mpeg2TransportStride"; }
            if (media.subType == MediaSubType.Mpeg2Video) { return "Mpeg2Video"; }
            if (media.subType == MediaSubType.None) { return "None"; }
            if (media.subType == MediaSubType.Null) { return "Null"; }
            if (media.subType == MediaSubType.NV12) { return "NV12"; }
            if (media.subType == MediaSubType.NV24) { return "NV24"; }
            if (media.subType == MediaSubType.Overlay) { return "Overlay"; }
            if (media.subType == MediaSubType.PCM) { return "PCM"; }
            if (media.subType == MediaSubType.PCMAudio_Obsolete) { return "PCMAudio_Obsolete"; }
            if (media.subType == MediaSubType.PLUM) { return "PLUM"; }
            if (media.subType == MediaSubType.QTJpeg) { return "QTJpeg"; }
            if (media.subType == MediaSubType.QTMovie) { return "QTMovie"; }
            if (media.subType == MediaSubType.QTRle) { return "QTRle"; }
            if (media.subType == MediaSubType.QTRpza) { return "QTRpza"; }
            if (media.subType == MediaSubType.QTSmc) { return "QTSmc"; }
            if (media.subType == MediaSubType.RAW_SPORT) { return "RAW_SPORT"; }
            if (media.subType == MediaSubType.RGB1) { return "RGB1"; }
            if (media.subType == MediaSubType.RGB16_D3D_DX7_RT) { return "RGB16_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.RGB16_D3D_DX9_RT) { return "RGB16_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.RGB24) { return "RGB24"; }
            if (media.subType == MediaSubType.RGB32) { return "RGB32"; }
            if (media.subType == MediaSubType.RGB32_D3D_DX7_RT) { return "RGB32_D3D_DX7_RT"; }
            if (media.subType == MediaSubType.RGB32_D3D_DX9_RT) { return "RGB32_D3D_DX9_RT"; }
            if (media.subType == MediaSubType.RGB4) { return "RGB4"; }
            if (media.subType == MediaSubType.RGB555) { return "RGB555"; }
            if (media.subType == MediaSubType.RGB565) { return "RGB565"; }
            if (media.subType == MediaSubType.RGB8) { return "RGB8"; }
            if (media.subType == MediaSubType.S340) { return "S340"; }
            if (media.subType == MediaSubType.S342) { return "S342"; }
            if (media.subType == MediaSubType.SPDIF_TAG_241h) { return "SPDIF_TAG_241h"; }
            if (media.subType == MediaSubType.TELETEXT) { return "TELETEXT"; }
            if (media.subType == MediaSubType.TVMJ) { return "TVMJ"; }
            if (media.subType == MediaSubType.UYVY) { return "UYVY"; }
            if (media.subType == MediaSubType.VideoImage) { return "VideoImage"; }
            if (media.subType == MediaSubType.VPS) { return "VPS"; }
            if (media.subType == MediaSubType.VPVBI) { return "VPVBI"; }
            if (media.subType == MediaSubType.VPVideo) { return "VPVideo"; }
            if (media.subType == MediaSubType.WAKE) { return "WAKE"; }
            if (media.subType == MediaSubType.WAVE) { return "WAVE"; }
            if (media.subType == MediaSubType.WebStream) { return "WebStream"; }
            if (media.subType == MediaSubType.WSS) { return "WSS"; }
            if (media.subType == MediaSubType.Y211) { return "Y211"; }
            if (media.subType == MediaSubType.Y411) { return "Y411"; }
            if (media.subType == MediaSubType.Y41P) { return "Y41P"; }
            if (media.subType == MediaSubType.YUY2) { return "YUY2"; }
            if (media.subType == MediaSubType.YUYV) { return "YUYV"; }
            if (media.subType == MediaSubType.YV12) { return "YV12"; }
            if (media.subType == MediaSubType.YVU9) { return "YVU9"; }
            if (media.subType == MediaSubType.YVYU) { return "YVYU"; }

            return "";
        }
    }
}
