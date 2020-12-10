namespace Core2D.Model
{
    public interface IFileSystem
    {
        string GetBaseDirectory();

        bool Exists(string path);

        System.IO.Stream Open(string path);

        System.IO.Stream Create(string path);

        byte[] ReadBinary(System.IO.Stream stream);

        void WriteBinary(System.IO.Stream stream, byte[] bytes);

        string ReadUtf8Text(System.IO.Stream stream);

        void WriteUtf8Text(System.IO.Stream stream, string text);

        string ReadUtf8Text(string path);

        void WriteUtf8Text(string path, string text);
    }
}
