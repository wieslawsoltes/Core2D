using System.Collections.Immutable;
using Core2D.Data;

namespace Core2D
{
    public interface IDataObject
    {
        ImmutableArray<PropertyViewModel> Properties { get; set; }
        RecordViewModel Record { get; set; }
    }
}
