#nullable enable
using System.IO;

namespace Core2D.Model;

public interface IFileWriter: IPickerItem
{
    void Save(Stream stream, object item, object? options);
}
