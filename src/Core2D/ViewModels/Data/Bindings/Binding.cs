namespace Core2D.ViewModels.Data.Bindings
{
    internal readonly struct Binding
    {
        public readonly int Start;
        public readonly int End;
        public readonly int Length;
        public readonly string Path;
        public readonly string Value;

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
