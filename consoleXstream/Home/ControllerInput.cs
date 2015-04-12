namespace consoleXstream.Home
{
    public class ControllerInput
    {
        public ControllerInput(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Check(bool IsSystem)
        {
            var controllerMax = _class.BaseClass.ControllerMax;
            var gamepad = _class.BaseClass.Gamepad;
            var titanOne = _class.BaseClass.TitanOne;
            var system = _class.BaseClass.System;

            var output = gamepad.Check(IsSystem);

            if (system.UseTitanOneApi) titanOne.Send(output);
            else
            {
                if (system.UseControllerMax)
                    controllerMax.CheckControllerInput();

                if (system.UseTitanOne)
                    titanOne.Send(output);
            }
        }
    }
}
