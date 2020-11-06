namespace Core2D.Bindings
{
    internal struct Binding
    {
        public int Start;
        public int End;
        public int Length;
        public string Path;
        public string Value;

        public Binding(int start, int end, int length, string path, string value)
        {
            Start = start;
            End = end;
            Length = length;
            Path = path;
            Value = value;
        }
    }
}
