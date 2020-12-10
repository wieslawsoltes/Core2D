using System.Collections.Generic;
using System.Linq;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Renderer;

namespace Core2D.ViewModels.Containers
{
    public partial class ProjectContainerViewModel : BaseContainerViewModel, IImageCache
    {
        private readonly IDictionary<string, byte[]> _images = new Dictionary<string, byte[]>();

        private IEnumerable<IImageKey> GetKeys() => _images.Select(i => new ImageKeyViewModel() { Key = i.Key }).ToList();

        public IEnumerable<IImageKey> Keys => GetKeys();

        public string AddImageFromFile(string path, byte[] bytes)
        {
            var name = System.IO.Path.GetFileName(path);
            var key = "Images\\" + name;

            if (_images.Keys.Contains(key))
            {
                return key;
            }

            _images.Add(key, bytes);
            RaisePropertyChanged(nameof(Keys));
            return key;
        }

        public void AddImage(string key, byte[] bytes)
        {
            if (_images.Keys.Contains(key))
            {
                return;
            }

            _images.Add(key, bytes);
            RaisePropertyChanged(nameof(Keys));
        }

        public byte[] GetImage(string key)
        {
            if (_images.TryGetValue(key, out byte[] bytes))
            {
                return bytes;
            }
            else
            {
                return null;
            }
        }

        public void RemoveImage(string key)
        {
            _images.Remove(key);
            RaisePropertyChanged(nameof(Keys));
        }

        public void PurgeUnusedImages(ICollection<string> used)
        {
            foreach (var kvp in _images.ToList())
            {
                if (!used.Contains(kvp.Key))
                {
                    _images.Remove(kvp.Key);
                }
            }
            RaisePropertyChanged(nameof(Keys));
        }
    }
}
