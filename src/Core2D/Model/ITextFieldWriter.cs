using System.IO;

namespace Core2D
{
    public interface ITextFieldWriter<T>
    {
        string Name { get; }

        string Extension { get; }

        void Write(Stream stream, T database);
    }
}
