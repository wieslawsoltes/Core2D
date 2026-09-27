using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Views;
using Core2D.Views.Docking.Tools;
using Dock.Model.Core;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioPanelMenuTests
{
    [AvaloniaFact]
    public void PrimarySidebarsKeepDockingActionsWithoutLegacyGripChrome()
    {
        using var state = new AppState();
        state.Editor!.OnNewProject();
        var view = new MainView { DataContext = state.Editor };
        var window = new Window { Width = 1440, Height = 900, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var sidebars = view.GetVisualDescendants().OfType<UserControl>()
                .Where(x => x is StudioNavigatorView or StudioInspectorView).ToArray();
            Assert.Equal(2, sidebars.Length);
            Assert.NotSame(sidebars[0].ContextMenu, sidebars[1].ContextMenu);
            foreach (var sidebar in sidebars)
            {
                var dockable = Assert.IsAssignableFrom<IDockable>(sidebar.DataContext);
                Assert.NotNull(dockable.Factory);
                var menu = Assert.IsType<ContextMenu>(sidebar.ContextMenu);
                menu.Open(sidebar);
                Dispatcher.UIThread.RunJobs();
                Assert.Same(dockable, menu.DataContext);
                var actions = menu.Items.OfType<MenuItem>().ToArray();
                Assert.Equal(2, actions.Length);
                Assert.All(actions, item =>
                {
                    Assert.NotNull(item.Command);
                    Assert.Same(dockable, item.CommandParameter);
                });
                menu.Close();
            }
        }
        finally
        {
            window.Close();
        }
    }
}
