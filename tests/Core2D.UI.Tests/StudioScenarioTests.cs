using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Model;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.Views;
using Core2D.Views.Docking.Tools;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioScenarioTests
{
    [AvaloniaTheory]
    [InlineData(false, 1440, 900)]
    [InlineData(true, 1440, 900)]
    [InlineData(true, 1024, 768)]
    public void PopulatedDrawingUsesTheRealSelectionInspector(bool dark, int width, int height)
    {
        using var state = new AppState();
        var selected = Populate(state);
        var editor = state.Editor!;
        var view = new MainView { DataContext = editor };
        var window = new Window
        {
            Width = width, Height = height, Content = view,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            editor.Platform?.OnZoomAutoFit();
            Dispatcher.UIThread.RunJobs();
            var inspector = Assert.Single(view.GetVisualDescendants().OfType<StudioInspectorView>());
            Assert.Same(selected, inspector.FindControl<ContentControl>("SelectionGeometry")!.Content);
            Assert.Same(selected.Style, inspector.FindControl<ContentControl>("SelectionAppearance")!.Content);
            Assert.NotEmpty(inspector.GetVisualDescendants().OfType<StudioNumericField>());
            Assert.NotEmpty(inspector.GetVisualDescendants().OfType<StudioColorField>());
            var canvas = Assert.Single(view.GetVisualDescendants().OfType<Controls.Editor.PageView>());
            Assert.True(canvas.Bounds.Width >= 400, "Sidebars must leave usable drawing space at compact sizes.");
            Assert.True(inspector.Bounds.Width >= 240);
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            var directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
                frame!.Save(Path.Combine(directory, $"populated-{(dark ? "dark" : "light")}-{width}.png"));
            }
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void SidebarTabsAndLayerSearchKeepTheirOriginalModels()
    {
        using var state = new AppState();
        Populate(state);
        var view = new MainView { DataContext = state.Editor };
        var window = new Window { Width = 1440, Height = 900, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var navigator = Assert.Single(view.GetVisualDescendants().OfType<StudioNavigatorView>());
            var search = navigator.FindControl<TextBox>("LayerSearch")!;
            var tree = navigator.FindControl<Views.Containers.ProjectContainerView>("ProjectTree")!;
            var grid = tree.GetVisualDescendants().OfType<DataGrid>().Single();
            search.Text = "Feature card";
            Dispatcher.UIThread.RunJobs();
            Assert.NotEmpty(grid.SearchModel.Descriptors);
            Assert.NotEmpty(grid.SearchModel.Results);
            search.Focus();
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Assert.True(grid.SearchModel.CurrentIndex >= 0);
            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();
            Assert.Empty(grid.SearchModel.Descriptors);

            var navigation = navigator.FindControl<TabControl>("NavigationTabs")!;
            navigation.SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();
            var assets = navigator.GetVisualDescendants().OfType<TabControl>().Single(x => x.Classes.Contains("studio-assets"));
            Assert.Equal(6, assets.ItemCount);
            for (var index = 0; index < assets.ItemCount; index++)
            {
                assets.SelectedIndex = index;
                Dispatcher.UIThread.RunJobs();
                var content = Assert.IsAssignableFrom<Control>(assets.SelectedContent);
                Assert.Same(state.Editor!.Project, content.DataContext);
                Assert.True(content.Bounds.Width > 0);
            }
            navigation.SelectedIndex = 0;
            var inspector = view.GetVisualDescendants().OfType<StudioInspectorView>().Single();
            var tabs = inspector.FindControl<TabControl>("InspectorTabs")!;
            for (var index = 0; index < tabs.ItemCount; index++)
            {
                tabs.SelectedIndex = index;
                Dispatcher.UIThread.RunJobs();
                Assert.NotNull(tabs.SelectedContent);
            }
        }
        finally
        {
            window.Close();
        }
    }

    internal static RectangleShapeViewModel Populate(AppState state)
    {
        var editor = state.Editor!;
        editor.OnNewProject();
        var project = editor.Project!;
        project.Name = "Product exploration";
        var page = Assert.IsType<PageContainerViewModel>(project.CurrentContainer);
        page.Name = "Landing page";
        page.Template!.Width = 640;
        page.Template.Height = 480;
        var factory = state.ServiceProvider.GetService<IViewModelFactory>()!;
        var cardStyle = factory.CreateShapeStyle("Feature / Indigo", sr: 88, sg: 78, sb: 220,
            fr: 104, fg: 93, fb: 232, thickness: 1);
        var mutedStyle = factory.CreateShapeStyle("Surface / Soft", fr: 242, fg: 243, fb: 248);
        var titleStyle = factory.CreateShapeStyle("Heading", sr: 36, sg: 38, sb: 52,
            textStyleViewModel: factory.CreateTextStyle(fontName: "Inter", fontFile: "", fontSize: 27,
                fontStyle: FontStyleFlags.Bold, textHAlignment: TextHAlignment.Left));
        var bodyStyle = factory.CreateShapeStyle("Body", sr: 96, sg: 100, sb: 117,
            textStyleViewModel: factory.CreateTextStyle(fontName: "Inter", fontFile: "", fontSize: 14,
                textHAlignment: TextHAlignment.Left));
        var lightTextStyle = factory.CreateShapeStyle("On accent", sr: 255, sg: 255, sb: 255,
            textStyleViewModel: factory.CreateTextStyle(fontName: "Inter", fontFile: "", fontSize: 18,
                fontStyle: FontStyleFlags.Bold, textHAlignment: TextHAlignment.Left));
        var selected = factory.CreateRectangleShape(44, 180, 310, 358, cardStyle, isFilled: true, name: "Feature card");
        project.OnAddShape(factory.CreateTextShape(44, 40, 596, 90, titleStyle, "Make room for better ideas", name: "Headline"));
        project.OnAddShape(factory.CreateTextShape(44, 100, 590, 140, bodyStyle, "A focused workspace for your next design.", name: "Description"));
        project.OnAddShape(selected);
        project.OnAddShape(factory.CreateTextShape(64, 208, 292, 248, lightTextStyle, "Build something new", name: "Card heading"));
        project.OnAddShape(factory.CreateRectangleShape(336, 180, 596, 262, mutedStyle, isStroked: false, isFilled: true, name: "Secondary card"));
        project.OnAddShape(factory.CreateRectangleShape(336, 280, 596, 358, mutedStyle, isStroked: false, isFilled: true, name: "Tertiary card"));
        project.OnAddShape(factory.CreateTextShape(355, 196, 578, 243, bodyStyle, "Explore components", name: "Secondary label"));
        project.OnAddShape(factory.CreateTextShape(355, 295, 578, 340, bodyStyle, "Keep every detail", name: "Tertiary label"));
        project.OnAddShape(factory.CreateTextShape(44, 395, 590, 430, bodyStyle, "CORE2D  /  STUDIO WORKSPACE", name: "Footer"));
        foreach (var document in project.Documents)
        {
            document.IsExpanded = true;
            foreach (var item in document.Pages)
            {
                item.IsExpanded = true;
                foreach (var layer in item.Layers)
                {
                    layer.IsExpanded = true;
                }
            }
        }
        project.SelectedShapes = new HashSet<BaseShapeViewModel> { selected };
        return selected;
    }
}
