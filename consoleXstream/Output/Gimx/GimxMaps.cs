using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace consoleXstream.Output.Gimx
{
    class GimxMaps
    {

        private struct PS3
        {
            public Int32 leftStickX;    // [-128, 127]
            public Int32 leftStickY;    // [-128, 127]
            public Int32 rightStickX;   // [-128, 127]
            public Int32 rightStickY;   // [-128, 127]

            public Int32 accX;          // [-512, 511]
            public Int32 accY;          // [-512, 511]
            public Int32 accZ;          // [-512, 511]
            public Int32 gyro;          // [-512, 511]

            public Int32 select;        // {0, 1}
            public Int32 start;         // {0, 1}
            public Int32 ps;            // {0, 1}

            public Int32 up;            // [0, 255]
            public Int32 right;         // [0, 255]
            public Int32 down;          // [0, 255]
            public Int32 left;          // [0, 255]

            public Int32 triangle;      // [0, 255]
            public Int32 circle;        // [0, 255]
            public Int32 cross;         // [0, 255]
            public Int32 square;        // [0, 255]

            public Int32 l1;            // [0, 255]
            public Int32 r1;            // [0, 255]
            public Int32 l2;            // [0, 255]
            public Int32 r2;            // [0, 255]

            public Int32 l3;            // {0, 1}
            public Int32 r3;            // {0, 1}
        }


        private PS3 ps3Map;

        private const int PACKETSIZE = 130;
        private int mapSize;

        public byte[] buffer;
        

        public GimxMaps()
        {
            ps3Map = new PS3();
            mapSize = Marshal.SizeOf(ps3Map);
            buffer = new byte[PACKETSIZE];

            // set the packet header
            buffer[0] = 0xff;
            buffer[1] = 0x80;
        }


        public void mapToPS3(Input.GamePadState state)
        {
            ps3Map = default(PS3);

            ps3Map.leftStickX = state.RawSate.Gamepad.sThumbLX >> 8;
            ps3Map.leftStickY = (state.RawSate.Gamepad.sThumbLY >> 8) * -1;
            ps3Map.rightStickX = state.RawSate.Gamepad.sThumbRX >> 8;
            ps3Map.rightStickY = (state.RawSate.Gamepad.sThumbRY >> 8) * -1;

            if (state.Buttons.Back) { if (state.Buttons.B) { ps3Map.ps = 1; } else { ps3Map.select = 1; } };
            if (state.Buttons.Start) { ps3Map.start = 1; };
            if (state.Buttons.Guide) { ps3Map.ps = 1; };

            if (state.DPad.Up) { ps3Map.up = 255; };
            if (state.DPad.Right) { ps3Map.right = 255; };
            if (state.DPad.Down) { ps3Map.down = 255; };
            if (state.DPad.Left) { ps3Map.left = 255; };

            if (state.Buttons.Y) { ps3Map.triangle = 255; };
            if (state.Buttons.B) { ps3Map.circle = 255; };
            if (state.Buttons.A) { ps3Map.cross = 255; };
            if (state.Buttons.X) { ps3Map.square = 255; };

            if (state.Buttons.LeftShoulder) { ps3Map.l1 = 255; };
            if (state.Buttons.RightShoulder) { ps3Map.r1 = 255; };
            ps3Map.r2 = state.RawSate.Gamepad.bLeftTrigger;
            ps3Map.l2 = state.RawSate.Gamepad.bRightTrigger;

            if (state.Buttons.LeftStick) { ps3Map.l3 = 1; };
            if (state.Buttons.RightStick) { ps3Map.r3 = 1; };

            // Convert to byte array
            IntPtr ptr = Marshal.AllocHGlobal(mapSize);
            Marshal.StructureToPtr(ps3Map, ptr, true);
            Marshal.Copy(ptr, buffer, 2, mapSize);
            Marshal.FreeHGlobal(ptr);
        }
    }
}
