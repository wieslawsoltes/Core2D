using System.Collections.Generic;
using System.ComponentModel;

namespace Core2D.Renderer
{
    public interface IImageCache : INotifyPropertyChanged
    {
        IEnumerable<IImageKey> Keys { get; }

        string AddImageFromFile(string path, byte[] bytes);

        void AddImage(string key, byte[] bytes);

        byte[] GetImage(string key);

        void RemoveImage(string key);

        void PurgeUnusedImages(ICollection<string> used);
    }
}
