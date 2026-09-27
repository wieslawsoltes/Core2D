using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using Core2D.Controls.Studio;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioActionSafetyTests
{
    [AvaloniaTheory]
    [InlineData("disabled")]
    [InlineData("hidden")]
    [InlineData("command")]
    [InlineData("parameter")]
    public void ImmediateExecutionRejectsStaleCatalogEntries(string change)
    {
        int executions = 0;
        var item = new MenuItem { Header = "Run", CommandParameter = "original", Command = new RelayCommand(() => executions++) };
        var category = new MenuItem { Header = "Actions", Items = { item } };
        var palette = new StudioCommandPalette { Items = { category } };
        var window = new Window { Width = 640, Height = 480, Content = palette };
        try
        {
            window.Show(); palette.IsOpen = true; Dispatcher.UIThread.RunJobs();
            var search = palette.GetVisualDescendants().OfType<TextBox>().Single(x => x.Name == "PART_Search");
            switch (change)
            {
                case "disabled": category.IsEnabled = false; break;
                case "hidden": category.IsVisible = false; break;
                case "command": item.Command = new RelayCommand(() => executions++); break;
                case "parameter": item.CommandParameter = "replacement"; break;
                default: throw new ArgumentOutOfRangeException(nameof(change));
            }
            // Deliberately route before queued binding/catalog refresh jobs run.
            search.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
            Assert.Equal(0, executions);
            Assert.True(palette.IsOpen);
            Dispatcher.UIThread.RunJobs();
            palette.IsOpen = false;
        }
        finally { window.Close(); }
    }
}
