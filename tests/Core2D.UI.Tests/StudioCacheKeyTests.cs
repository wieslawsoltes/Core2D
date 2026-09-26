using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Model.Renderer;
using Xunit;
using ReactiveUI;

namespace Core2D.UI.Tests;

public class StudioCacheKeyTests
{
    [AvaloniaFact]
    public void CacheBrowserUsesInterfaceKeysWithoutViewModelOrReflectionRequirements()
    {
        var first = new CacheKey { Key = "logo.png" };
        var browser = new StudioAssetBrowser
        {
            Items = new IImageKey[] { first, new CacheKey { Key = "photo.jpg" } },
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
            browser.Query = "";
            first.Key = "updated.svg";
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(browser.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "updated.svg");
            window.Content = null;
            first.Key = "reattached.svg";
            window.Content = browser;
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(browser.GetVisualDescendants().OfType<TextBlock>(), text => text.Text == "reattached.svg");
            StudioSecondaryViewTests.Capture(window, "image-cache-keys.png");
        }
        finally { window.Close(); }
    }

    private sealed class CacheKey : ReactiveObject, IImageKey
    {
        private string? _key;
        public string? Key { get => _key; set => this.RaiseAndSetIfChanged(ref _key, value); }
    }
}
