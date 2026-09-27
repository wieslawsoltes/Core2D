using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioReattachmentTests
{
    [AvaloniaFact]
    public void SearchAndLegacyScrubbingReconnectWithoutDuplicateHandlers()
    {
        var search = new StudioSearchBox { Text = "First" };
        var number = new StudioNumberBox { Prefix = "X", Text = "10" };
        var panel = new StackPanel { Children = { search, number } };
        var window = new Window { Width = 280, Height = 160, Content = panel };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            for (int iteration = 0; iteration < 3; iteration++)
            {
                panel.Children.Clear();
                panel.Children.Add(search);
                panel.Children.Add(number);
                search.Text = "Next";
                Dispatcher.UIThread.RunJobs();
                var clear = search.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_Clear");
                Point click = clear.TranslatePoint(new Point(clear.Bounds.Width / 2, clear.Bounds.Height / 2), window)!.Value;
                window.MouseDown(click, MouseButton.Left);
                window.MouseUp(click, MouseButton.Left);
                Assert.Equal(string.Empty, search.Text);
                var handle = number.GetVisualDescendants().OfType<ContentPresenter>().Single(x => x.Name == "PART_InnerLeft");
                Point start = handle.TranslatePoint(new Point(handle.Bounds.Width / 2, handle.Bounds.Height / 2), window)!.Value;
                window.MouseDown(start, MouseButton.Left);
                window.MouseMove(start + new Vector(30, 0));
                window.MouseUp(start + new Vector(30, 0), MouseButton.Left);
                Assert.Equal(((iteration + 2) * 10).ToString(), number.Text);
            }
        }
        finally
        {
            window.Close();
        }
    }
}
