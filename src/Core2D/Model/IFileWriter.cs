#nullable disable
using System.IO;

namespace Core2D.Model
{
    public interface IFileWriter
    {
        string Name { get; }

        string Extension { get; }

        void Save(Stream stream, object item, object options);
    }
}
