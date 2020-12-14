using System.Collections.Generic;

namespace Core2D.ViewModels.Data.Bindings
{
    internal static class BindingParser
    {
        public static char s_startChar = '{';
        public static char s_endChar = '}';

        public static List<Binding> Parse(string text)
        {
            var bindings = new List<Binding>();

            for (int i = 0; i < text.Length; i++)
            {
                var start = text.IndexOf(s_startChar, i);
                if (start >= 0)
                {
                    var end = text.IndexOf(s_endChar, start);
                    if (end >= start)
                    {
                        var length = end - start + 1;
                        var path = text.Substring(start + 1, length - 2);
                        var value = text.Substring(start, length);
                        var binding = new Binding(start, end, length, path, value);
                        bindings.Add(binding);
                        i = end;
                    }
                }
            }

            return bindings;
        }
    }
}
