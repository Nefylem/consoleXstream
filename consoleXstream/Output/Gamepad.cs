using System;
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

        public int CMHomeCount { get; set; }
        public bool Ps4Touchpad { get; set; }

        private string _strTODevice;

        private bool _boolHoldBack = false;
        private bool _boolLoadShortcuts = false;

        private int _XboxCount;
        private int _MenuWait;
        private int _MenuShow;

        public byte[] Output;

        public void Check()
        {
            _MenuShow = 35;

            //Update gamepad status
            _controls = GamePad.GetState(PlayerIndex.One);

            if (_XboxCount == 0) { _XboxCount = Enum.GetNames(typeof(Xbox)).Length; }
            Output = new byte[_XboxCount];

            if (_controls.DPad.Left) { Output[_class.Remap.RemapGamepad.Left] = Convert.ToByte(100); }
            if (_controls.DPad.Right) { Output[_class.Remap.RemapGamepad.Right] = Convert.ToByte(100); }
            if (_controls.DPad.Up) { Output[_class.Remap.RemapGamepad.Up] = Convert.ToByte(100); }
            if (_controls.DPad.Down) { Output[_class.Remap.RemapGamepad.Down] = Convert.ToByte(100); }

            if (_controls.Buttons.A) { Output[_class.Remap.RemapGamepad.A] = Convert.ToByte(100); }
            if (_controls.Buttons.B) { Output[_class.Remap.RemapGamepad.B] = Convert.ToByte(100); }
            if (_controls.Buttons.X) { Output[_class.Remap.RemapGamepad.X] = Convert.ToByte(100); }
            if (_controls.Buttons.Y) { Output[_class.Remap.RemapGamepad.Y] = Convert.ToByte(100); }

            if (_controls.Buttons.Start) { Output[_class.Remap.RemapGamepad.Start] = Convert.ToByte(100); }
            if (_controls.Buttons.Guide) { Output[_class.Remap.RemapGamepad.Home] = Convert.ToByte(100); }
            if (_controls.Buttons.Back)
            {
                if (_class.System.boolBlockMenuButton == false)
                {
                    _MenuWait++;
                    if (_class.System.boolMenu == false)
                        if (_MenuWait >= (_MenuShow + 80))
                            OpenMenu();
                }

                //_class.Remap back buton to touchpad
                if (_class.System.IsPs4ControllerMode)
                    Output[_class.Remap.RemapGamepad.Touch] = Convert.ToByte(100);
                else
                    Output[_class.Remap.RemapGamepad.Back] = Convert.ToByte(100);
            }
            else
            {
                _MenuWait = 0;
            }

            if (_controls.Buttons.LeftShoulder) { Output[_class.Remap.RemapGamepad.LeftShoulder] = Convert.ToByte(100); }
            if (_controls.Buttons.RightShoulder) { Output[_class.Remap.RemapGamepad.RightShoulder] = Convert.ToByte(100); }
            if (_controls.Buttons.LeftStick) { Output[_class.Remap.RemapGamepad.LeftStick] = Convert.ToByte(100); }
            if (_controls.Buttons.RightStick) { Output[_class.Remap.RemapGamepad.RightStick] = Convert.ToByte(100); }

            if (_controls.Triggers.Left > 0) { Output[_class.Remap.RemapGamepad.LeftTrigger] = Convert.ToByte(_controls.Triggers.Left * 100); }
            if (_controls.Triggers.Right > 0) { Output[_class.Remap.RemapGamepad.RightTrigger] = Convert.ToByte(_controls.Triggers.Right * 100); }

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

            if (dblLx != 0) { Output[_class.Remap.RemapGamepad.LeftX] = (byte)Convert.ToSByte((int)(dblLx)); }
            if (dblLy != 0) { Output[_class.Remap.RemapGamepad.LeftY] = (byte)Convert.ToSByte((int)(dblLy)); }
            if (dblRx != 0) { Output[_class.Remap.RemapGamepad.RightX] = (byte)Convert.ToSByte((int)(dblRx)); }
            if (dblRy != 0) { Output[_class.Remap.RemapGamepad.RightY] = (byte)Convert.ToSByte((int)(dblRy)); }

            if (CMHomeCount > 0)
            {
                Output[_class.Remap.RemapGamepad.Home] = Convert.ToByte(100);
                CMHomeCount--;
            }

            if (Ps4Touchpad)
                Output[_class.Remap.RemapGamepad.Touch] = Convert.ToByte(100);


            if (_boolLoadShortcuts)
                Output = _shortcut.CheckKeys(Output);

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
            if (_class.KeyboardInterface == null || _class.KeyboardInterface.output == null) return;
            for (var intCount = 0; intCount < _XboxCount; intCount++)
            {
                if (_class.KeyboardInterface.output[intCount] != 0)
                    Output[intCount] = _class.KeyboardInterface.output[intCount];
            }
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
            _boolHoldBack = false;
            _MenuWait = 0;

            _class.Home.OpenMenu();
        }
    }
}
