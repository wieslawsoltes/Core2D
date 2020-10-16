using System.Threading.Tasks;

namespace Core2D
{
    public interface IImageImporter
    {
        Task<string> GetImageKeyAsync();
    }
}
