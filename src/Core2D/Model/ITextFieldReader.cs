#nullable enable
using System.IO;

namespace Core2D.Model
{
    public interface ITextFieldReader<out T>
    {
        string Name { get; }

        string Extension { get; }

        T? Read(Stream stream);
    }
}
