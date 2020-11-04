using System.Collections.Immutable;
using Core2D.Data;

namespace Core2D
{
    public interface IDataObject
    {
        ImmutableArray<Property> Properties { get; set; }
        Record Record { get; set; }
    }
}
