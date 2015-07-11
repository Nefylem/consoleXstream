namespace consoleXstream.Output.CronusPlus
{
    class Classes
    {
        public Classes(BaseClass baseClass, Write write) { Create(baseClass, write);  }

        public BaseClass BaseClass { get; set; }
        public Define Define { get; set; }
        public Init Init { get; set; }
        public Write Write { get; set; }

        private void Create(BaseClass baseClass, Write write)
        {
            BaseClass = baseClass;
            Define = new Define();
            Init = new Init(this);
            Write = write;
        }
    }
}
