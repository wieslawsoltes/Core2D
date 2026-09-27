using System;
using System.Collections.Immutable;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Core2D.Behaviors;
using Core2D.Controls.Studio;
using Core2D.Model.History;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.History;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioWorkbenchSafetyTests
{
    [AvaloniaFact]
    public void ApplicationFileShortcutsRemainAvailableAfterNativeTextHandling()
    {
        int saves = 0;
        var input = new TextBox { Text = "Document name" };
        var root = new Grid { Children = { input } };
        KeyModifiers primary = OperatingSystem.IsMacOS() ? KeyModifiers.Meta : KeyModifiers.Control;
        root.KeyBindings.Add(new KeyBinding { Gesture = new KeyGesture(Key.S, primary), Command = new RelayCommand(() => saves++) });
        Interaction.GetBehaviors(root).Add(new StudioScopedKeyBindingsBehavior { AllowModifiedTextBindings = true });
        var window = new Window { Width = 320, Height = 200, Content = root };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs(); input.Focus();
            RawInputModifiers modifiers = OperatingSystem.IsMacOS() ? RawInputModifiers.Meta : RawInputModifiers.Control;
            window.KeyPressQwerty(PhysicalKey.S, modifiers); window.KeyReleaseQwerty(PhysicalKey.S, modifiers);
            Assert.Equal(1, saves);
            Assert.Equal("Document name", input.Text);
            input.SelectAll();
            window.KeyPressQwerty(PhysicalKey.Delete, RawInputModifiers.None); window.KeyReleaseQwerty(PhysicalKey.Delete, RawInputModifiers.None);
            Assert.Equal(string.Empty, input.Text);
            Assert.Equal(1, saves);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void FocusedDraftIsNotCommittedWhenTheRecordIsReplaced()
    {
        var first = StudioDataTableTests.Record(); var second = StudioDataTableTests.Record();
        IHistory history = new StackHistory();
        var table = new StudioDataTable { Source = first };
        StudioEditContext.SetHistory(table, history);
        var window = new Window { Width = 480, Height = 320, Content = table };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var field = table.GetVisualDescendants().OfType<StudioDataValueField>().Single(x => (x.DataContext as DataFieldRowViewModel)?.Name == "Tag");
            var input = Assert.Single(field.GetVisualDescendants().OfType<TextBox>());
            Assert.True(input.Focus()); input.Text = "Do not publish this draft";
            table.Source = second; Dispatcher.UIThread.RunJobs();
            Assert.Equal("P-101", first.Values[0].Content);
            Assert.Equal("P-101", second.Values[0].Content);
            Assert.False(history.CanUndo());
            first.Values = ImmutableArray<ValueViewModel>.Empty;
            Assert.Equal(3, table.ResultCount);
        }
        finally { window.Close(); }
    }
}
