using System;
using consoleXstream.Home;
using consoleXstream.Input;

namespace consoleXstream.Output
{
    public class Gamepad
    {
        public Gamepad(BaseClass baseClass)
        {
            _class = baseClass;
            _shortcut = new Shortcut();
        }

        private readonly BaseClass _class;
        private readonly Shortcut _shortcut;

        private GamePadState _controls;

        public int CmHomeCount { get; set; }
        public bool Ps4Touchpad { get; set; }

        private string _strToDevice;

        private bool _isHoldBack;
        private int _holdBackCount;

        private bool _boolLoadShortcuts = false;

        private int _xboxCount;
        private int _menuWait;
        private int _menuShow;



        public byte[] Check(bool isSystem)
        {
            _menuShow = 40;

            _controls = GamePad.GetState(PlayerIndex.One);

            if (_xboxCount == 0) { _xboxCount = Enum.GetNames(typeof(Xbox)).Length; }
            var output = new byte[_xboxCount];

            if (_controls.DPad.Left) { output[_class.Remap.RemapGamepad.Left] = Convert.ToByte(100); }
            if (_controls.DPad.Right) { output[_class.Remap.RemapGamepad.Right] = Convert.ToByte(100); }
            if (_controls.DPad.Up) { output[_class.Remap.RemapGamepad.Up] = Convert.ToByte(100); }
            if (_controls.DPad.Down) { output[_class.Remap.RemapGamepad.Down] = Convert.ToByte(100); }

            if (_controls.Buttons.A) { output[_class.Remap.RemapGamepad.A] = Convert.ToByte(100); }
            if (_controls.Buttons.B) { output[_class.Remap.RemapGamepad.B] = Convert.ToByte(100); }
            if (_controls.Buttons.X) { output[_class.Remap.RemapGamepad.X] = Convert.ToByte(100); }
            if (_controls.Buttons.Y) { output[_class.Remap.RemapGamepad.Y] = Convert.ToByte(100); }

            if (_controls.Buttons.Start) { output[_class.Remap.RemapGamepad.Start] = Convert.ToByte(100); }
            if (_controls.Buttons.Guide) { output[_class.Remap.RemapGamepad.Home] = Convert.ToByte(100); }

            if (_controls.Buttons.Back)
            {
                if (_class.System.boolBlockMenuButton == false && isSystem)
                {
                    _isHoldBack = true;
                    _menuWait++;
                    _holdBackCount++;
                    if (_class.System.boolMenu == false)
                        if (_menuWait >= _menuShow + 20)
                        {
                            _isHoldBack = false;
                            _holdBackCount = 0;
                            OpenMenu();
                        }
                }
            }
            else
            {
                if (_isHoldBack)
                {
                    if (_holdBackCount > 0)
                    {
                        _holdBackCount--;
                        if (_class.System.IsPs4ControllerMode)
                            output[_class.Remap.RemapGamepad.Touch] = Convert.ToByte(100);
                        else
                            output[_class.Remap.RemapGamepad.Back] = Convert.ToByte(100);
                    }
                    else
                    {
                        _isHoldBack = false;
                    }
                }
            }

            if (_controls.Buttons.LeftShoulder) { output[_class.Remap.RemapGamepad.LeftShoulder] = Convert.ToByte(100); }
            if (_controls.Buttons.RightShoulder) { output[_class.Remap.RemapGamepad.RightShoulder] = Convert.ToByte(100); }
            if (_controls.Buttons.LeftStick) { output[_class.Remap.RemapGamepad.LeftStick] = Convert.ToByte(100); }
            if (_controls.Buttons.RightStick) { output[_class.Remap.RemapGamepad.RightStick] = Convert.ToByte(100); }

            if (_controls.Triggers.Left > 0) { output[_class.Remap.RemapGamepad.LeftTrigger] = Convert.ToByte(_controls.Triggers.Left * 100); }
            if (_controls.Triggers.Right > 0) { output[_class.Remap.RemapGamepad.RightTrigger] = Convert.ToByte(_controls.Triggers.Right * 100); }

            double dblLx = _controls.ThumbSticks.Left.X * 100;
            double dblLy = _controls.ThumbSticks.Left.Y * 100;
            double dblRx = _controls.ThumbSticks.Right.X * 100;
            double dblRy = _controls.ThumbSticks.Right.Y * 100;

            if (_class.System.IsNormalizeControls)
            {
                NormalGamepad(ref dblLx, ref dblLy);
                NormalGamepad(ref dblRx, ref dblRy);
            }
            else
            {
                dblLy = -dblLy;
                dblRy = -dblRy;
            }

            if (dblLx != 0) { output[_class.Remap.RemapGamepad.LeftX] = (byte)Convert.ToSByte((int)(dblLx)); }
            if (dblLy != 0) { output[_class.Remap.RemapGamepad.LeftY] = (byte)Convert.ToSByte((int)(dblLy)); }
            if (dblRx != 0) { output[_class.Remap.RemapGamepad.RightX] = (byte)Convert.ToSByte((int)(dblRx)); }
            if (dblRy != 0) { output[_class.Remap.RemapGamepad.RightY] = (byte)Convert.ToSByte((int)(dblRy)); }

            if (CmHomeCount > 0)
            {
                output[_class.Remap.RemapGamepad.Home] = Convert.ToByte(100);
                CmHomeCount--;
            }

            if (Ps4Touchpad)
                output[_class.Remap.RemapGamepad.Touch] = Convert.ToByte(100);


            if (_boolLoadShortcuts)
                output = _shortcut.CheckKeys(output);

            var intTarget = -1;
            intTarget = _class.System.IsPs4ControllerMode == false ? _class.Remap.RemapGamepad.Back : _class.Remap.RemapGamepad.Touch;
            /*
            //Back button. Wait until released as its also the menu button
            if (intTarget > -1)
            {
                if (system.boolBlockMenuButton)
                {
                    if (output[intTarget] == 100)
                    {
                        _boolHoldBack = true;
                        output[intTarget] = Convert.ToByte(0);
                        _MenuWait++;
                        if (!system.boolMenu)
                        {
                            if (_MenuWait >= _MenuShow)
                            {
                                _boolHoldBack = false;
                                openMenu();
                            }
                        }
                    }
                    else
                    {
                        if (_boolHoldBack == true)
                        {
                            _boolHoldBack = false;
                            output[intTarget] = Convert.ToByte(100);
                            _MenuWait = 0;
                        }
                        else
                            _MenuWait = 0;
                    }
                }
            }
            */
            //Dont think that needs to be here
            if (_class.KeyboardInterface == null || _class.KeyboardInterface.Output == null) 
                return output;

            for (var intCount = 0; intCount < _xboxCount; intCount++)
            {
                if (_class.KeyboardInterface.Output[intCount] != 0)
                    output[intCount] = _class.KeyboardInterface.Output[intCount];
            }
            return output;
        }

        private void NormalGamepad(ref double dblLx, ref double dblLy)
        {
            var dblNewX = dblLx;
            var dblNewY = dblLy;

            var dblLength = Math.Sqrt(Math.Pow(dblLx, 2) + Math.Pow(dblLy, 2));
            if (dblLength > 99.9)
            {
                var dblTheta = Math.Atan2(dblLy, dblLx);
                var dblAngle = (90 - ((dblTheta * 180) / Math.PI)) % 360;

                if ((dblAngle < 0) && (dblAngle >= -45)) { dblNewX = (int)(100 / Math.Tan(dblTheta)); dblNewY = -100; }
                if ((dblAngle >= 0) && (dblAngle <= 45)) { dblNewX = (int)(100 / Math.Tan(dblTheta)); dblNewY = -100; }
                if ((dblAngle > 45) && (dblAngle <= 135)) { dblNewY = -(int)(Math.Tan(dblTheta) * 100); dblNewX = 100; }
                if ((dblAngle > 135) && (dblAngle <= 225)) { dblNewX = -(int)(100 / Math.Tan(dblTheta)); dblNewY = 100; }
                if (dblAngle > 225) { dblNewY = (int)(Math.Tan(dblTheta) * 100); dblNewX = -100; }
                if (dblAngle < -45) { dblNewY = (int)(Math.Tan(dblTheta) * 100); dblNewX = -100; }
            }
            else
            {
                dblNewY = -dblNewY;
            }

            //Return values
            dblLx = dblNewX;
            dblLy = dblNewY;
        }

        private void OpenMenu()
        {
            _menuWait = 0;

            _class.HomeClass.Menu.Open();
        }
    }
}
