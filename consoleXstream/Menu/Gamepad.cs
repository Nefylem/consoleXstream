using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.Input;

namespace consoleXstream.Menu
{
    class Gamepad
    {
        private Navigation _nav;

        public void GetNavHandle(Navigation nav) { _nav = nav; }

        public void CheckInput()
        {
            var controls = GamePad.GetState(PlayerIndex.One);
            if (controls.DPad.Up) _nav.CheckCommand("up");
            if (controls.DPad.Down) _nav.CheckCommand("down");
            if (controls.DPad.Left) _nav.CheckCommand("left");
            if (controls.DPad.Right) _nav.CheckCommand("right");
            if (controls.Buttons.B || controls.Buttons.Back) _nav.CheckCommand("back");
            if (controls.Buttons.A || controls.Buttons.Start) _nav.CheckCommand("ok");
        }
    }
}
