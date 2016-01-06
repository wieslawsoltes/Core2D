using System;
using System.Reflection;
using Portable.Xaml;
using Portable.Xaml.Schema;

namespace Dependencies
{
    internal class CoreXamlType : XamlType
    {
        public CoreXamlType(Type underlyingType, XamlSchemaContext schemaContext)
            : base(underlyingType, schemaContext)
        {
        }

        protected override XamlTypeInvoker LookupInvoker()
        {
            return new CoreXamlTypeInvoker(this);
        }
    }
}
