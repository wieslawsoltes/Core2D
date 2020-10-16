using System.Threading.Tasks;
using Avalonia;

namespace Core2D.Editor
{
    public sealed class AvaloniaTextClipboard : ITextClipboard
    {
        Task ITextClipboard.SetText(string text)
        {
            return Application.Current.Clipboard.SetTextAsync(text);
        }

        private async Task<string> GetTextAsync()
        {
            return await Application.Current.Clipboard.GetTextAsync();
        }

        Task<string> ITextClipboard.GetText()
        {
            return GetTextAsync();
        }

        private async Task<bool> ContainsTextAsync()
        {
            return !string.IsNullOrEmpty(await GetTextAsync());
        }

        async Task<bool> ITextClipboard.ContainsText()
        {
            return await ContainsTextAsync();
        }
    }
}
