
namespace Core2D.Renderer
{
    /// <summary>
    /// Image key.
    /// </summary>
    public class ImageKey : IImageKey
    {
        /// <summary>
        /// Gets or sets image key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Check whether the <see cref="Key"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeKey() => !string.IsNullOrWhiteSpace(Key);
    }
}
