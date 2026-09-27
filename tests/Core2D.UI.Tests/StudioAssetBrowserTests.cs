using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using Core2D.Controls.Studio;
using Core2D.Model.History;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Style;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioAssetBrowserTests
{
    [AvaloniaFact]
    public void ProjectionSortsAndFiltersWithoutChangingSourceOrderOrSelection()
    {
        var first = new ShapeStyleViewModel(null) { Name = "Zinc" };
        var second = new ShapeStyleViewModel(null) { Name = "Azure" };
        var items = new ObservableCollection<ViewModelBase> { first, second };
        var browser = new StudioAssetBrowser { Items = items, SelectedItem = first };
        var window = new Window { Width = 320, Height = 400, Content = browser };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var grid = browser.GetVisualDescendants().OfType<DataGrid>().Single();
            Assert.Equal(2, browser.ResultCount);
            Assert.Same(first, grid.SelectedItem);
            browser.SortIndex = 1;
            Dispatcher.UIThread.RunJobs();
            Assert.Same(second, grid.ItemsSource!.Cast<object>().First());
            Assert.Same(first, items[0]);
            browser.Query = "AZ";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, browser.ResultCount);
            Assert.Same(second, grid.ItemsSource!.Cast<object>().Single());
            Assert.Same(first, browser.SelectedItem);
            browser.Query = "";
            Dispatcher.UIThread.RunJobs();
            Assert.Same(first, grid.SelectedItem);
            grid.SelectedItem = second;
            Assert.Same(second, browser.SelectedItem);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void NameAndSourceChangesRefreshTheProjectionAndDetachOldSubscriptions()
    {
        var item = new ShapeStyleViewModel(null) { Name = "Blue" };
        var items = new ObservableCollection<ViewModelBase> { item };
        var browser = new StudioAssetBrowser { Items = items, Query = "Blue" };
        var window = new Window { Width = 320, Height = 300, Content = browser };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, browser.ResultCount);
            item.Name = "Red";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, browser.ResultCount);
            items.Add(new ShapeStyleViewModel(null) { Name = "Blue highlight" });
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, browser.ResultCount);
            browser.Items = new ObservableCollection<ViewModelBase>();
            Dispatcher.UIThread.RunJobs();
            item.Name = "Blue";
            items.Add(new ShapeStyleViewModel(null) { Name = "Blue border" });
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, browser.ResultCount);
            window.Content = null;
            browser.Items = items;
            window.Content = browser;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(3, browser.ResultCount);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void CompactRowsAndActivationKeepOriginalItemIdentity()
    {
        var item = new ShapeStyleViewModel(null) { Name = "Surface" };
        object? activated = null;
        bool enabled = true;
        var command = new RelayCommand<object?>(value => activated = value, _ => enabled);
        var browser = new StudioAssetBrowser { Items = new[] { item }, SelectedItem = item, OpenCommand = command };
        var window = new Window { Width = 320, Height = 300, Content = browser };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var grid = browser.GetVisualDescendants().OfType<DataGrid>().Single();
            Assert.Equal(64, grid.RowHeight);
            browser.IsCompact = true;
            Assert.Equal(36, grid.RowHeight);
            grid.Focus();
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Assert.Same(item, activated);
            activated = null; enabled = false; command.NotifyCanExecuteChanged();
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Assert.Null(activated);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void AssetNameUsesExistingModelAndDocumentHistory()
    {
        IHistory history = new StackHistory();
        var model = new ShapeStyleViewModel(null) { Name = "Surface" };
        var browser = new StudioAssetBrowser
        {
            Items = new[] { model },
            ItemTemplate = new FuncDataTemplate<ShapeStyleViewModel>((item, _) =>
            {
                var control = new StudioAssetItem { DataContext = item };
                control.Bind(StudioAssetItem.TitleProperty, new Binding(nameof(item.Name)) { Mode = BindingMode.TwoWay });
                return control;
            })
        };
        StudioEditContext.SetHistory(browser, history);
        var window = new Window { Width = 320, Height = 300, Content = browser };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var name = browser.GetVisualDescendants().OfType<StudioNameField>().Single();
            name.DraftText = "Raised surface";
            Assert.Equal("Surface", model.Name);
            Assert.True(name.TryCommit());
            Assert.Equal("Raised surface", model.Name);
            Assert.True(history.Undo());
            Assert.Equal("Surface", model.Name);
            Assert.True(history.Redo());
            Assert.Equal("Raised surface", model.Name);
        }
        finally { window.Close(); }
    }
}
