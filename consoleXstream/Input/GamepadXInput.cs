using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace consoleXstream.Input
{
    public enum xbox : int
    {
        [Description("Home")]
        home = 0,
        [Description("Back")]
        back = 1,
        [Description("Start")]
        start = 2,
        [Description("Right Shoulder")]
        rightShoulder = 3,
        [Description("Right Trigger")]
        rightTrigger = 4,
        [Description("Right Stick")]
        rightStick = 5,
        [Description("Left Shoulder")]
        leftShoulder = 6,
        [Description("Left Trigger")]
        leftTrigger = 7,
        [Description("Left Stick")]
        leftStick = 8,
        [Description("RightThumb X")]
        rightX = 9,
        [Description("RightThumb Y")]
        rightY = 10,
        [Description("LeftThumb X")]
        leftX = 11,
        [Description("LeftThumb Y")]
        leftY = 12,
        [Description("Up")]
        up = 13,
        [Description("Down")]
        down = 14,
        [Description("Left")]
        left = 15,
        [Description("Right")]
        right = 16,
        [Description("Y")]
        y = 17,
        [Description("B")]
        b = 18,
        [Description("A")]
        a = 19,
        [Description("X")]
        x = 20,
        accX = 21,      //rotate X. 90 = -25, 180 = 0, 270 = +25, 360 = 0 (ng)
        accY = 22,      //shake vertically. +25 (top) to -25 (bottom) (ng)
        accZ = 23,      //tilt up
        gyroX = 24,     //no reading
        gyroY = 25,     //no reading
        gyroZ = 26,     //no reading
        [Description("Touchpad")]
        touch = 27,             //touchpad, 100 = on    (works)
        touchX = 28,            //-100 to 100   (left to right)
        touchY = 29             //-100 to 100   (top to bottom)
    }                               //Control codes for incoming gamepad 

    public enum BatteryTypes : byte
    {
        //
        // Flags for battery status level
        //
        BATTERY_TYPE_DISCONNECTED = 0x00,    // This device is not connected
        BATTERY_TYPE_WIRED = 0x01,    // Wired device, no battery
        BATTERY_TYPE_ALKALINE = 0x02,    // Alkaline battery source
        BATTERY_TYPE_NIMH = 0x03,    // Nickel Metal Hydride battery source
        BATTERY_TYPE_UNKNOWN = 0xFF,    // Cannot determine the battery type
    };

    public enum BatteryLevel : byte
    {
        BATTERY_LEVEL_EMPTY = 0x00,
        BATTERY_LEVEL_LOW = 0x01,
        BATTERY_LEVEL_MEDIUM = 0x02,
        BATTERY_LEVEL_FULL = 0x03
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct XInputBatteryInformation
    {
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        public byte BatteryType;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public byte BatteryLevel;

        public override string ToString()
        {
            return string.Format("{0} {1}", (BatteryTypes)BatteryType, (BatteryLevel)BatteryLevel);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInputVibration
    {
        [MarshalAs(UnmanagedType.I2)]
        public ushort LeftMotorSpeed;

        [MarshalAs(UnmanagedType.I2)]
        public ushort RightMotorSpeed;
    }

    class Imports
    {
        [DllImport("xinput1_3.dll")]
        public static extern int XInputSetState
        (
            int dwUserIndex,  // [in] Index of the gamer associated with the device
            ref XInputVibration pVibration    // [in, out] The vibration information to send to the controller
        );

        [DllImport("xinput1_3.dll")]
        public static extern int XInputGetBatteryInformation
        (
              int dwUserIndex,        // Index of the gamer associated with the device
              byte devType,            // Which device on this user index
              ref XInputBatteryInformation pBatteryInformation // Contains the level and types of batteries
        );

        [DllImport("xinput1_3.dll", EntryPoint = "#100")]
        public static extern uint XInputGetState(uint playerIndex, IntPtr state);
        [DllImport("xinput1_3.dll", EntryPoint = "#103")]
        public static extern uint PowerOff(uint playerIndex);

        public enum Constants
        {
            Success = 0x000,
            NotConnected = 0x48F,
            LeftStickDeadZone = 7849,
            RightStickDeadZone = 8689
        }
    }

    public enum ButtonState
    {
        Pressed,
        Released
    }

    public struct GamePadButtons
    {
        ButtonState start, back, leftStick, rightStick, leftShoulder, rightShoulder, guide, a, b, x, y;

        internal GamePadButtons(ButtonState start, ButtonState back, ButtonState leftStick, ButtonState rightStick,
                                ButtonState leftShoulder, ButtonState rightShoulder, ButtonState guide, ButtonState a, ButtonState b,
                                ButtonState x, ButtonState y)
        {
            this.start = start;
            this.back = back;
            this.guide = guide;
            this.leftStick = leftStick;
            this.rightStick = rightStick;
            this.leftShoulder = leftShoulder;
            this.rightShoulder = rightShoulder;
            this.a = a;
            this.b = b;
            this.x = x;
            this.y = y;
        }

        public bool Start
        {
            get { if (start == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool Back
        {
            get { if (back == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool Guide
        {
            get { if (guide == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool LeftStick
        {
            get { if (leftStick == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool RightStick
        {
            get { if (rightStick == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool LeftShoulder
        {
            get { if (leftShoulder == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool RightShoulder
        {
            get { if (rightShoulder == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool A
        {
            get { if (a == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool B
        {
            get { if (b == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool X
        {
            get { if (x == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool Y
        {
            get { if (y == ButtonState.Pressed) { return true; } else { return false; } }
        }
    }

    public struct GamePadDPad
    {
        ButtonState up, down, left, right;

        internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }

        public bool Up
        {
            get { if (up == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool Down
        {
            get { if (down == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool Left
        {
            get { if (left == ButtonState.Pressed) { return true; } else { return false; } }
        }

        public bool Right
        {
            get { if (right == ButtonState.Pressed) { return true; } else { return false; } }
        }
    }

    public struct GamePadThumbSticks
    {
        public struct StickValue
        {
            float x, y;

            internal StickValue(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public float X
            {
                get { return x; }
            }

            public float Y
            {
                get { return y; }
            }
        }

        StickValue left, right;

        internal GamePadThumbSticks(StickValue left, StickValue right)
        {
            this.left = left;
            this.right = right;
        }

        public StickValue Left
        {
            get { return left; }
        }

        public StickValue Right
        {
            get { return right; }
        }
    }

    public struct GamePadTriggers
    {
        float left;
        float right;

        internal GamePadTriggers(float left, float right)
        {
            this.left = left;
            this.right = right;
        }

        public float Left
        {
            get { return left; }
        }

        public float Right
        {
            get { return right; }
        }
    }

    public struct GamePadState
    {
        internal struct RawState
        {
            public uint dwPacketNumber;
            public GamePad Gamepad;

            public struct GamePad
            {
                public ushort dwButtons;
                public byte bLeftTrigger;
                public byte bRightTrigger;
                public short sThumbLX;
                public short sThumbLY;
                public short sThumbRX;
                public short sThumbRY;
            }
        }

        bool isConnected;
        uint packetNumber;
        GamePadButtons buttons;
        GamePadDPad dPad;
        GamePadThumbSticks thumbSticks;
        GamePadTriggers triggers;

        enum ButtonsConstants
        {
            DPadUp = 0x0001,
            DPadDown = 0x0002,
            DPadLeft = 0x0004,
            DPadRight = 0x0008,
            Start = 0x10,
            Back = 0x20,
            Guide = 0x0400,
            LeftThumb = 0x00000040,
            RightThumb = 0x00000080,
            LeftShoulder = 0x0100,
            RightShoulder = 0x0200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000
        }

        internal GamePadState(bool isConnected, RawState rawState, GamePadDeadZone deadZone)
        {
            this.isConnected = isConnected;

            if (!isConnected)
            {
                rawState.dwPacketNumber = 0;
                rawState.Gamepad.dwButtons = 0;
                rawState.Gamepad.bLeftTrigger = 0;
                rawState.Gamepad.bRightTrigger = 0;
                rawState.Gamepad.sThumbLX = 0;
                rawState.Gamepad.sThumbLY = 0;
                rawState.Gamepad.sThumbRX = 0;
                rawState.Gamepad.sThumbRY = 0;
            }

            packetNumber = rawState.dwPacketNumber;
            buttons = new GamePadButtons(
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.Start) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.Back) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.LeftThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.RightThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.LeftShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.RightShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.Guide) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.A) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.B) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.X) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.Y) != 0 ? ButtonState.Pressed : ButtonState.Released
            );
            dPad = new GamePadDPad(
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadUp) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadDown) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadLeft) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadRight) != 0 ? ButtonState.Pressed : ButtonState.Released
            );

            switch (deadZone)
            {
                case GamePadDeadZone.IndependentAxes:
                    rawState.Gamepad.sThumbLX = ThumbStickDeadZoneIndependantAxes(rawState.Gamepad.sThumbLX, (short)Imports.Constants.LeftStickDeadZone);
                    rawState.Gamepad.sThumbLY = ThumbStickDeadZoneIndependantAxes(rawState.Gamepad.sThumbLY, (short)Imports.Constants.LeftStickDeadZone);
                    rawState.Gamepad.sThumbRX = ThumbStickDeadZoneIndependantAxes(rawState.Gamepad.sThumbRX, (short)Imports.Constants.RightStickDeadZone);
                    rawState.Gamepad.sThumbRY = ThumbStickDeadZoneIndependantAxes(rawState.Gamepad.sThumbRY, (short)Imports.Constants.RightStickDeadZone);
                    break;
            }

            thumbSticks = new GamePadThumbSticks(
                new GamePadThumbSticks.StickValue(
                    rawState.Gamepad.sThumbLX < 0 ? rawState.Gamepad.sThumbLX / 32768.0f : rawState.Gamepad.sThumbLX / 32767.0f,
                    rawState.Gamepad.sThumbLY < 0 ? rawState.Gamepad.sThumbLY / 32768.0f : rawState.Gamepad.sThumbLY / 32767.0f),
                new GamePadThumbSticks.StickValue(
                    rawState.Gamepad.sThumbRX < 0 ? rawState.Gamepad.sThumbRX / 32768.0f : rawState.Gamepad.sThumbRX / 32767.0f,
                    rawState.Gamepad.sThumbRY < 0 ? rawState.Gamepad.sThumbRY / 32768.0f : rawState.Gamepad.sThumbRY / 32767.0f)
            );
            triggers = new GamePadTriggers(rawState.Gamepad.bLeftTrigger / 255.0f, rawState.Gamepad.bRightTrigger / 255.0f);
        }

        public static short ThumbStickDeadZoneIndependantAxes(short value, short deadZone)
        {
            if (value < 0 && value > -deadZone || value > 0 && value < deadZone)
                return 0;
            return value;
        }

        public uint PacketNumber
        {
            get { return packetNumber; }
        }

        public bool IsConnected
        {
            get { return isConnected; }
        }

        public GamePadButtons Buttons
        {
            get { return buttons; }
        }

        public GamePadDPad DPad
        {
            get { return dPad; }
        }

        public GamePadTriggers Triggers
        {
            get { return triggers; }
        }

        public GamePadThumbSticks ThumbSticks
        {
            get { return thumbSticks; }
        }
    }

    public enum PlayerIndex
    {
        One = 0,
        Two,
        Three,
        Four
    }

    public enum GamePadDeadZone
    {
        IndependentAxes,
        None
    }

    public class GamePad
    {
        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            return GetState(playerIndex, GamePadDeadZone.IndependentAxes);
        }

        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZone)
        {
            IntPtr gamePadStatePointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GamePadState.RawState)));
            uint result = Imports.XInputGetState((uint)playerIndex, gamePadStatePointer);
            GamePadState.RawState state = (GamePadState.RawState)Marshal.PtrToStructure(gamePadStatePointer, typeof(GamePadState.RawState));
            return new GamePadState(result == (uint)Imports.Constants.Success, state, deadZone);
        }

        public static void SetState(PlayerIndex playerIndex, double leftMotor, double rightMotor)
        {
            XInputVibration vibration = new XInputVibration() { LeftMotorSpeed = (ushort)(65535d * leftMotor), RightMotorSpeed = (ushort)(65535d * rightMotor) };
            Imports.XInputSetState((int)playerIndex, ref vibration);
        }

        public static string GetBatteryType(PlayerIndex playerIndex)
        {
            string strReturn = "";

            XInputBatteryInformation gamepad = new XInputBatteryInformation();
            Imports.XInputGetBatteryInformation((int)playerIndex, (byte)0, ref gamepad);

            strReturn = gamepad.ToString();
            return strReturn;
        }
    }

    class GamepadXInput
    {
        private Form1 frmMain;

        public GamepadXInput(Form1 mainForm) { frmMain = mainForm; }

        /*
        public static PowerOff TurnOffController(PlayerIndex playerIndex)
        {
            return TurnOffController(playerIndex);
            uint result = Imports.PowerOff((uint)playerIndex);
        }
         */

    }
}
