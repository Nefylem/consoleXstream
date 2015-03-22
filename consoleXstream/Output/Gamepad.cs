using System;
using consoleXstream.Config;
using consoleXstream.Input;
using consoleXstream.Remap;

namespace consoleXstream.Output
{
    public class Gamepad
    {
        public Gamepad(Form1 home, Remapping remap, Configuration system, KeyboardInterface keyboard)
        {
            _frmMain = home;
            _remap = remap;
            _system = system;
            _keyboard = keyboard;

            _shortcut = new Shortcut();
        }

        private readonly Form1 _frmMain;
        private readonly Remapping _remap;
        private readonly Configuration _system;
        private readonly KeyboardInterface _keyboard;
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

            if (_controls.DPad.Left) { Output[_remap.remapGamepad.left] = Convert.ToByte(100); }
            if (_controls.DPad.Right) { Output[_remap.remapGamepad.right] = Convert.ToByte(100); }
            if (_controls.DPad.Up) { Output[_remap.remapGamepad.up] = Convert.ToByte(100); }
            if (_controls.DPad.Down) { Output[_remap.remapGamepad.down] = Convert.ToByte(100); }

            if (_controls.Buttons.A) { Output[_remap.remapGamepad.a] = Convert.ToByte(100); }
            if (_controls.Buttons.B) { Output[_remap.remapGamepad.b] = Convert.ToByte(100); }
            if (_controls.Buttons.X) { Output[_remap.remapGamepad.x] = Convert.ToByte(100); }
            if (_controls.Buttons.Y) { Output[_remap.remapGamepad.y] = Convert.ToByte(100); }

            if (_controls.Buttons.Start) { Output[_remap.remapGamepad.start] = Convert.ToByte(100); }
            if (_controls.Buttons.Guide) { Output[_remap.remapGamepad.home] = Convert.ToByte(100); }
            if (_controls.Buttons.Back)
            {
                if (_system.boolBlockMenuButton == false)
                {
                    _MenuWait++;
                    if (_system.boolMenu == false)
                        if (_MenuWait >= _MenuShow + 20)
                            OpenMenu();
                }

                //_remap back buton to touchpad
                if (_system.IsPs4ControllerMode)
                    Output[_remap.remapGamepad.touch] = Convert.ToByte(100);
                else
                    Output[_remap.remapGamepad.back] = Convert.ToByte(100);
            }

            if (_controls.Buttons.LeftShoulder) { Output[_remap.remapGamepad.leftShoulder] = Convert.ToByte(100); }
            if (_controls.Buttons.RightShoulder) { Output[_remap.remapGamepad.rightShoulder] = Convert.ToByte(100); }
            if (_controls.Buttons.LeftStick) { Output[_remap.remapGamepad.leftStick] = Convert.ToByte(100); }
            if (_controls.Buttons.RightStick) { Output[_remap.remapGamepad.rightStick] = Convert.ToByte(100); }

            if (_controls.Triggers.Left > 0) { Output[_remap.remapGamepad.leftTrigger] = Convert.ToByte(_controls.Triggers.Left * 100); }
            if (_controls.Triggers.Right > 0) { Output[_remap.remapGamepad.rightTrigger] = Convert.ToByte(_controls.Triggers.Right * 100); }

            double dblLX = _controls.ThumbSticks.Left.X * 100;
            double dblLY = _controls.ThumbSticks.Left.Y * 100;
            double dblRX = _controls.ThumbSticks.Right.X * 100;
            double dblRY = _controls.ThumbSticks.Right.Y * 100;

            if (_system.IsNormalizeControls)
            {
                NormalGamepad(ref dblLX, ref dblLY);
                NormalGamepad(ref dblRX, ref dblRY);
            }
            else
            {
                dblLY = -dblLY;
                dblRY = -dblRY;
            }

            if (dblLX != 0) { Output[_remap.remapGamepad.leftX] = (byte)Convert.ToSByte((int)(dblLX)); }
            if (dblLY != 0) { Output[_remap.remapGamepad.leftY] = (byte)Convert.ToSByte((int)(dblLY)); }
            if (dblRX != 0) { Output[_remap.remapGamepad.rightX] = (byte)Convert.ToSByte((int)(dblRX)); }
            if (dblRY != 0) { Output[_remap.remapGamepad.rightY] = (byte)Convert.ToSByte((int)(dblRY)); }

            if (CMHomeCount > 0)
            {
                Output[_remap.remapGamepad.home] = Convert.ToByte(100);
                CMHomeCount--;
            }

            if (Ps4Touchpad)
                Output[_remap.remapGamepad.touch] = Convert.ToByte(100);


            if (_boolLoadShortcuts)
                Output = _shortcut.CheckKeys(Output);

            int intTarget = -1;
            if (_system.IsPs4ControllerMode == false) { intTarget = _remap.remapGamepad.back; } else { intTarget = _remap.remapGamepad.touch; }
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
            if (_keyboard != null && _keyboard.output != null)
            {
                for (int intCount = 0; intCount < _XboxCount; intCount++)
                {
                    if (_keyboard.output[intCount] != 0)
                        Output[intCount] = _keyboard.output[intCount];
                }
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

            _frmMain.OpenMenu();
        }


    }
}
