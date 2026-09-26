using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Core2D.Behaviors;
using Core2D.Controls.Studio;
using Xunit;
using PageView = Core2D.Controls.Editor.PageView;

namespace Core2D.UI.Tests;

public class StudioScopedInputTests
{
    [AvaloniaFact]
    public void NativeEditingAndHandledGesturesTakePriorityOverDocumentShortcuts()
    {
        int executions = 0;
        bool consume = false;
        var input = new TextBox { Text = "Name" };
        var target = new Button { Content = "Canvas focus" };
        var panel = new StackPanel { Children = { input, target } };
        panel.KeyBindings.Add(new KeyBinding { Gesture = new KeyGesture(Key.Delete), Command = new RelayCommand(() => executions++) });
        panel.AddHandler(InputElement.KeyDownEvent, (_, e) =>
        {
            if (consume && e.Key == Key.Delete) e.Handled = true;
        }, RoutingStrategies.Tunnel);
        Interaction.GetBehaviors(panel).Add(new StudioScopedKeyBindingsBehavior());
        var window = new Window { Width = 300, Height = 160, Content = panel };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Empty(panel.KeyBindings);
            input.Focus();
            input.SelectAll();
            PressKey(window, PhysicalKey.Delete);
            Assert.Equal(string.Empty, input.Text);
            Assert.Equal(0, executions);
            target.Focus();
            PressKey(window, PhysicalKey.Delete);
            Assert.Equal(1, executions);
            consume = true;
            PressKey(window, PhysicalKey.Delete);
            Assert.Equal(1, executions);
            consume = false;
            for (int iteration = 0; iteration < 3; iteration++)
            {
                window.Content = null;
                Assert.Single(panel.KeyBindings);
                window.Content = panel;
                Dispatcher.UIThread.RunJobs();
                Assert.Empty(panel.KeyBindings);
                target.Focus();
                PressKey(window, PhysicalKey.Delete);
                Assert.Equal(iteration + 2, executions);
            }
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void DrawingShortcutsRemainAvailableAfterCanvasScopeRefinement()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var view = new PageView { DataContext = editor };
        var window = new Window { Width = 900, Height = 700, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var zoom = view.FindControl<ZoomBorder>("PageZoomBorder")!;
            zoom.Focus();
            PressKey(window, PhysicalKey.R);
            Assert.Equal("Rectangle", editor.CurrentTool?.Title);
            PressKey(window, PhysicalKey.H);
            Assert.Equal("Path", editor.CurrentTool?.Title);
            PressKey(window, PhysicalKey.S);
            Assert.Equal("Selection", editor.CurrentTool?.Title);
            var overlay = view.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            window.KeyPressQwerty(PhysicalKey.Digit2, RawInputModifiers.Shift);
            window.KeyReleaseQwerty(PhysicalKey.Digit2, RawInputModifiers.Shift);
            Assert.Equal(44, shape.TopLeft!.X);
            Assert.NotNull(overlay.Navigation);
        }
        finally { window.Close(); }
    }

    private static void PressKey(Window window, PhysicalKey key)
    {
        window.KeyPressQwerty(key, RawInputModifiers.None);
        window.KeyReleaseQwerty(key, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
    }
}
