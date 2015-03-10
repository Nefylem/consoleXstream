using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using consoleXstream.Input;

namespace consoleXstream.Menu
{
    public class Gamepad
    {
        public Gamepad(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void CheckInput()
        {
            var controls = GamePad.GetState(PlayerIndex.One);
            if (controls.DPad.Up) _class.Nav.CheckCommand("up");
            if (controls.DPad.Down) _class.Nav.CheckCommand("down");
            if (controls.DPad.Left) _class.Nav.CheckCommand("left");
            if (controls.DPad.Right) _class.Nav.CheckCommand("right");
            if (controls.Buttons.B || controls.Buttons.Back) _class.Nav.CheckCommand("back");
            if (controls.Buttons.A || controls.Buttons.Start) _class.Nav.CheckCommand("ok");
        }
    }
}
