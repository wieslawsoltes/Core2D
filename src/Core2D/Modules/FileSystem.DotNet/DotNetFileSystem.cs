using System;
using System.Text;
using Core2D.Model;

namespace Core2D.Modules.FileSystem.DotNet
{
    public sealed class DotNetFileSystem : IFileSystem
    {
        private readonly IServiceProvider _serviceProvider;

        public DotNetFileSystem(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        string IFileSystem.GetBaseDirectory()
        {
            return AppContext.BaseDirectory;
        }

        bool IFileSystem.Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        System.IO.Stream IFileSystem.Open(string path)
        {
            return new System.IO.FileStream(path, System.IO.FileMode.Open);
        }

        System.IO.Stream IFileSystem.Create(string path)
        {
            return new System.IO.FileStream(path, System.IO.FileMode.Create);
        }

        byte[] IFileSystem.ReadBinary(System.IO.Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using var ms = new System.IO.MemoryStream();
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }

        void IFileSystem.WriteBinary(System.IO.Stream stream, byte[] bytes)
        {
            using var bw = new System.IO.BinaryWriter(stream);
            bw.Write(bytes);
        }

        string IFileSystem.ReadUtf8Text(System.IO.Stream stream)
        {
            using var sr = new System.IO.StreamReader(stream, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        void IFileSystem.WriteUtf8Text(System.IO.Stream stream, string text)
        {
            using var sw = new System.IO.StreamWriter(stream, Encoding.UTF8);
            sw.Write(text);
        }

        string IFileSystem.ReadUtf8Text(string path)
        {
            using var fs = System.IO.File.OpenRead(path);
            return (this as IFileSystem).ReadUtf8Text(fs);
        }

        void IFileSystem.WriteUtf8Text(string path, string text)
        {
            using var fs = System.IO.File.Create(path);
            (this as IFileSystem).WriteUtf8Text(fs, text);
        }
    }
}
