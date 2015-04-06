namespace consoleXstream.Config
{
    public class Var
    {
        public Var(Classes classes) { _class = classes; }
        private Classes _class;

        public bool IsReadData { get; set; }

    }
}
