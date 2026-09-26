using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editors;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.Views.Editors;
using Core2D.Views.Libraries;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioSecondaryViewTests
{
    [AvaloniaTheory]
    [InlineData(false, 0)] [InlineData(true, 0)]
    [InlineData(false, 1)] [InlineData(true, 1)]
    [InlineData(false, 2)] [InlineData(true, 2)]
    [InlineData(false, 3)] [InlineData(true, 3)]
    [InlineData(false, 4)] [InlineData(true, 4)]
    public void LibrariesRenderAndRetainBoundActions(bool dark, int kind)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var project = state.Editor!.Project!;
        var script = new ScriptViewModel(state.ServiceProvider)
        {
            Name = "Create diagram", Code = "// This source is only displayed, never executed by this test.\nvar title = \"Studio\";\n// Use the Run action explicitly."
        };
        project.Scripts = ImmutableArray.Create(script);
        project.CurrentScript = script;
        Control view = kind switch
        {
            0 => new BlocksView(), 1 => new StylesView(), 2 => new TemplatesView(),
            3 => new ScriptsView(), 4 => new ImagesView(), _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        view.DataContext = project;
        var window = new Window
        {
            Width = kind == 3 ? 680 : 380, Height = 680, Content = view,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Assert.NotEmpty(view.GetVisualDescendants().OfType<StudioPanelHeader>());
            var browser = Assert.Single(view.GetVisualDescendants().OfType<StudioAssetBrowser>());
            Assert.NotNull(browser.Items);
            Assert.NotEmpty(view.GetVisualDescendants().OfType<Button>().Where(x => x.Command is not null));
            int count = browser.ResultCount;
            browser.Query = "___not_an_asset___";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, browser.ResultCount);
            browser.Query = "";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(count, browser.ResultCount);
            if (kind == 3)
            {
                var source = Assert.Single(view.GetVisualDescendants().OfType<StudioCodeEditor>());
                Assert.Equal(script.Code, source.Text);
            }
            Capture(window, $"library-{kind}-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void TextBindingTabsUseOriginalTokenCommandsAndEditableSource()
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var text = new TextShapeViewModel(state.ServiceProvider)
        {
            Name = "Caption", Text = "Item: ",
            Properties = ImmutableArray.Create(new PropertyViewModel(null) { Name = "Tag", Value = "T-101" })
        };
        var model = new TextBindingEditorViewModel(state.ServiceProvider) { Editor = state.Editor, Text = text };
        var view = new TextBindingEditorView { DataContext = model };
        var window = new Window { Width = 680, Height = 520, Content = view };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var tabs = view.GetVisualDescendants().OfType<TabControl>().Single();
            tabs.SelectedIndex = 2;
            Dispatcher.UIThread.RunJobs();
            var browser = view.GetVisualDescendants().OfType<StudioAssetBrowser>().Single(x => x.IsEffectivelyVisible);
            Assert.Equal(1, browser.ResultCount);
            var insert = browser.GetVisualDescendants().OfType<Button>().Single(x => Equals(x.Content, "Insert"));
            Assert.NotNull(insert.Command);
            insert.Command!.Execute(insert.CommandParameter);
            Assert.Equal("Item: {Tag}", text.Text);
            var code = Assert.Single(view.GetVisualDescendants().OfType<StudioCodeEditor>());
            Assert.Equal(text.Text, code.Text);
            model.OnResetText();
            Assert.Equal("", code.Text);
            Capture(window, "text-binding.png");
        }
        finally { window.Close(); }
    }

    internal static void Capture(Window window, string name)
    {
        window.MouseMove(new Point(window.ClientSize.Width - 2, window.ClientSize.Height - 2));
        using var frame = window.CaptureRenderedFrame();
        Assert.NotNull(frame);
        string? directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
            frame!.Save(Path.Combine(directory, name));
        }
    }
}
