#nullable enable
using System.IO;

namespace Core2D.Model;

public interface ITextFieldReader<out T> : IPickerItem
{
    T? Read(Stream stream);
}
