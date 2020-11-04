using System.Threading.Tasks;

namespace Core2D
{
    public interface ITextClipboard
    {
        Task<bool> ContainsText();

        Task<string> GetText();

        Task SetText(string text);
    }
}
