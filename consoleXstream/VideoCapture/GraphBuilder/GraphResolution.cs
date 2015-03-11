using DirectShowLib;

namespace consoleXstream.VideoCapture.GraphBuilder
{
    public class GraphResolution
    {
        public GraphResolution(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Set(IBaseFilter pCaptureDevice, string strCaptureVideoOut)
        {
            if (_class.Var.VideoResolutionIndex < _class.Resolution.List.Count)
            {
                _class.Debug.Log("[3] set resolution " + _class.Resolution.List[_class.Var.VideoResolutionIndex]);
                var hr = ((IAMStreamConfig)_class.GraphPin.GetPin(pCaptureDevice, strCaptureVideoOut)).SetFormat(_class.Resolution.Type[_class.Var.VideoResolutionIndex]);
                if (hr == 0)
                {
                    _class.Debug.Log("[OK] Set resolution " + _class.Resolution.List[_class.Var.VideoResolutionIndex]);
                    _class.Var.CurrentResolution = _class.Var.VideoResolutionIndex;
                    _class.Var.CurrentResByName = _class.Resolution.List[_class.Var.VideoResolutionIndex];

                    if (_class.Var.CurrentResByName.IndexOf('[') > -1)
                        _class.Var.CurrentResByName = _class.Var.CurrentResByName.Substring(0, _class.Var.CurrentResByName.IndexOf('['));
                }
                else
                {
                    _class.Debug.Log("[NG] Can't set resolution " + _class.Resolution.List[_class.Var.VideoResolutionIndex]);
                    _class.Debug.Log("-> " + DsError.GetErrorText(hr));
                }
            }
            else
                _class.Debug.Log("[0] [ERR] cant find resolution " + _class.Var.VideoResolutionIndex.ToString());
        }

        public void Get()
        {
            if (_class.Graph.IamAvd != null && _class.Var.SetResolution == 0)
            {
                int intLineCount = 0;
                _class.Graph.IamAvd.get_NumberOfLines(out intLineCount);

                if (intLineCount > 0)
                {
                    string strLineCount = intLineCount.ToString();
                    _class.System.autoChangeRes(intLineCount);

                    for (int intCount = 0; intCount < _class.Resolution.List.Count; intCount++)
                    {
                        string strRes = _class.Resolution.List[intCount];
                        if (strRes.IndexOf('[') > -1)
                        {
                            strRes = strRes.Substring(0, strRes.IndexOf('['));
                        }
                        strRes = strRes.Trim();
                        string[] strSplit = strRes.Split('x');

                        if (strSplit.Length == 2)
                        {
                            if (strSplit[1].Trim() == strLineCount)
                            {
                                _class.Var.VideoResolutionIndex = intCount;
                            }
                        }
                    }
                }
            }
        }

    }
}
