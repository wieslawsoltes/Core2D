using System.Collections.Immutable;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Core2D.Controls.Studio;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Core2D.Views.Renderer;
using SkiaSharp;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioBlockPreviewTests
{
    [AvaloniaFact]
    public void AssetPreviewUsesInheritedRendererAndReattachesWithLiveShapeState()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var factory = state.ServiceProvider.GetService<IViewModelFactory>()!;
        var block = factory.CreateBlockShape("Card");
        block.Shapes = ImmutableArray.Create<BaseShapeViewModel>(shape);
        var tile = new Core2D.Controls.BlockTileView { Block = block };
        var item = new StudioAssetItem { Title = "Card", Detail = "Block preview", Content = tile };
        RendererOptions.SetRenderer(item, state.Editor!.LibraryRenderer);
        var window = new Window { Width = 320, Height = 72, Content = item };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            for (int iteration = 0; iteration < 2; iteration++)
            {
                Assert.True(tile.Bounds.Width >= 40 && tile.Bounds.Height >= 40);
                var expected = iteration == 0 ? new SKColor(104, 93, 232) : new SKColor(39, 174, 96);
                var renderer = RendererOptions.GetRenderer(tile)!;
                double beforeZoom = renderer.State!.ZoomX;
                window.MouseMove(new Point(319, 71));
                using var frame = window.CaptureRenderedFrame();
                Assert.NotNull(frame);
                Assert.Equal(beforeZoom, renderer.State.ZoomX);
                using var bytes = new MemoryStream();
                frame!.Save(bytes);
                bytes.Position = 0;
                using var bitmap = SKBitmap.Decode(bytes);
                Point center = tile.TranslatePoint(new Point(tile.Bounds.Width / 2, tile.Bounds.Height / 2), window)!.Value;
                SKColor pixel = bitmap.GetPixel((int)center.X, (int)center.Y);
                Assert.Equal(expected.Red, pixel.Red);
                Assert.Equal(expected.Green, pixel.Green);
                Assert.Equal(expected.Blue, pixel.Blue);
                window.Content = null;
                shape.Style!.Fill!.Color = factory.CreateArgbColor(255, 39, 174, 96);
                window.Content = item;
                Dispatcher.UIThread.RunJobs();
            }
        }
        finally { window.Close(); }
    }
}
