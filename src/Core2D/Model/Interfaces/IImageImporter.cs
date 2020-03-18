using System.Threading.Tasks;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines image key importer contract.
    /// </summary>
    public interface IImageImporter
    {
        /// <summary>
        /// Get the image key.
        /// </summary>
        /// <returns>The image key.</returns>
        Task<string> GetImageKeyAsync();
    }
}
