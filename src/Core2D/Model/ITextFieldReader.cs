using System.IO;

namespace Core2D
{
    public interface ITextFieldReader<T>
    {
        string Name { get; }

        string Extension { get; }

        T Read(Stream stream);
    }
}
