
namespace Core2D.Shapes
{
    /// <summary>
    /// Defines image shape contract.
    /// </summary>
    public interface IImageShape : ITextShape
    {
        /// <summary>
        /// Gets or sets image key used to retrieve bytes from image cache repository.
        /// </summary>
        string Key { get; set; }
    }
}
