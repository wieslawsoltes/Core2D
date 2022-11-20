#nullable enable
using System.IO;

namespace Core2D.Model;

public interface ITextFieldWriter<in T>: IPickerItem
{
    void Write(Stream stream, T? database);
}
