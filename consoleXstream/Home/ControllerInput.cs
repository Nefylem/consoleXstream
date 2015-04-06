namespace consoleXstream.Home
{
    public class ControllerInput
    {
        public ControllerInput(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Check()
        {
            var controllerMax = _class.BaseClass.ControllerMax;
            var gamepad = _class.BaseClass.Gamepad;
            var titanOne = _class.BaseClass.TitanOne;
            var system = _class.BaseClass.System;
            
            gamepad.Check();

            if (system.UseTitanOneApi) titanOne.Send();
            else
            {
                if (system.UseControllerMax)
                    controllerMax.CheckControllerInput();

                if (system.UseTitanOne)
                    titanOne.Send();
            }
        }
    }
}
