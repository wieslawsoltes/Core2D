using System.Collections.Immutable;
using Core2D.ViewModels.Data;

namespace Core2D.Model
{
    public interface IDataObject
    {
        ImmutableArray<PropertyViewModel> Properties { get; set; }
        RecordViewModel Record { get; set; }
    }
}
