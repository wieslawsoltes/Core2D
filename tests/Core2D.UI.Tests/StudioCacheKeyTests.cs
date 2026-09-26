using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Model.Renderer;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioCacheKeyTests
{
    [AvaloniaFact]
    public void CacheBrowserUsesInterfaceKeysWithoutViewModelOrReflectionRequirements()
    {
        var browser = new StudioAssetBrowser
        {
            Items = new IImageKey[] { new CacheKey { Key = "logo.png" }, new CacheKey { Key = "photo.jpg" } },
            IsCompact = true
        };
        var window = new Window { Width = 320, Height = 260, Content = browser };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Assert.Contains(browser.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "logo.png");
            browser.Query = "photo";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, browser.ResultCount);
            Assert.Contains(browser.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "photo.jpg");
        }
        finally { window.Close(); }
    }

    private sealed class CacheKey : IImageKey
    {
        public string? Key { get; set; }
    }
}
