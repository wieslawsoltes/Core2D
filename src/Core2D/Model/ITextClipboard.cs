using System.Threading.Tasks;

namespace Core2D.Model
{
    public interface ITextClipboard
    {
        Task<bool> ContainsText();

        Task<string> GetText();

        Task SetText(string text);
    }
}
