#nullable disable
using System.Threading.Tasks;

namespace Core2D.Model
{
    public interface IImageImporter
    {
        Task<string> GetImageKeyAsync();
    }
}
