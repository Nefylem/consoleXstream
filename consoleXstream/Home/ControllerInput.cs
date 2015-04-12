namespace consoleXstream.Home
{
    public class ControllerInput
    {
        public ControllerInput(Classes classes) { _class = classes; }
        private readonly Classes _class;

        public void Check(bool isSystem)
        {
            Send(_class.BaseClass.Gamepad.Check(isSystem));
        }

        public void Send(byte[] output)
        {
            var titanOne = _class.BaseClass.TitanOne;
            var system = _class.BaseClass.System;
            var controllerMax = _class.BaseClass.ControllerMax;

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
