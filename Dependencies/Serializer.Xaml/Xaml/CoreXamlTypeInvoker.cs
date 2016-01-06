using Portable.Xaml.Schema;

namespace Dependencies
{
    internal class CoreXamlTypeInvoker : XamlTypeInvoker
    {
        public CoreXamlType Type { get; private set; }

        public CoreXamlTypeInvoker(CoreXamlType type)
            : base(type)
        {
            Type = type;
        }
    }
}
