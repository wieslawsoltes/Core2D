#nullable enable
using System.IO;

namespace Core2D.Model
{
    public interface ITextFieldWriter<in T>
    {
        string Name { get; }

        string Extension { get; }

        void Write(Stream stream, T? database);
    }
}
