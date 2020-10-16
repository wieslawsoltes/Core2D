
namespace Core2D.Renderer
{
    public class ImageKey : IImageKey
    {
        public string Key { get; set; }

        public bool ShouldSerializeKey() => !string.IsNullOrWhiteSpace(Key);
    }
}
