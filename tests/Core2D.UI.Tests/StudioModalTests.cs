using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Dialog;
using Core2D.Controls.Studio;
using Core2D.Model.Editor;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioModalTests
{
    [AvaloniaFact]
    public void ModalConstrainsItsSurfaceAndRestoresPriorFocus()
    {
        var prior = new TextBox { Text = "Workspace" };
        var root = new Grid { Children = { prior } };
        var window = new Window { Width = 640, Height = 480, Content = root };
        var presenter = new Presenter();
        var model = new DialogViewModel(null, presenter)
        {
            Title = "Asset settings", ViewModel = new RectangleShapeViewModel(null) { Name = "Card" },
            IsOverlayVisible = true, IsTitleBarVisible = true, IsCloseButtonVisible = true
        };
        var dialog = new DialogView { DataContext = model };
        var input = new StudioNameField { Value = "Card", Width = 900, Height = 600 };
        dialog.FindControl<ContentControl>("DialogContent")!.ContentTemplate = new FuncDataTemplate<object>((_, _) => input);
        presenter.Closed = () => root.Children.Remove(dialog);
        try
        {
            window.Show(); prior.Focus();
            root.Children.Add(dialog);
            Dispatcher.UIThread.RunJobs();
            var surface = dialog.FindControl<Border>("DragBorder")!;
            Assert.True(surface.Bounds.Width <= 608);
            Assert.True(surface.Bounds.Height <= 448);
            Assert.True(input.IsKeyboardFocusWithin);
            input.DraftText = "Discarded draft";
            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            Assert.Equal("Card", input.DraftText);
            Assert.Equal(0, presenter.CloseCount);
            dialog.FindControl<Button>("CloseButton")!.Focus();
            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, presenter.CloseCount);
            Assert.True(prior.IsFocused);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void CloseCommandUsesOriginalPresenterAndVisibilityFlagsRemainBound()
    {
        var presenter = new Presenter();
        var model = new DialogViewModel(null, presenter)
        {
            Title = "About", ViewModel = new AboutInfoViewModel(null) { Title = "Core2D", Description = "Native diagram studio", Version = "Test", License = "MIT" },
            IsTitleBarVisible = true, IsOverlayVisible = true, IsCloseButtonVisible = true
        };
        var dialog = new DialogView { DataContext = model };
        var window = new Window { Width = 700, Height = 500, Content = dialog };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            StudioSecondaryViewTests.Capture(window, "about-dialog.png");
            model.IsTitleBarVisible = false;
            Assert.False(dialog.FindControl<Border>("TitleBorder")!.IsVisible);
            model.IsOverlayVisible = false;
            Assert.False(dialog.FindControl<Button>("DialogBackdrop")!.IsVisible);
            var close = dialog.FindControl<Button>("CloseButton")!;
            close.Command!.Execute(close.CommandParameter);
            Assert.Equal(1, presenter.CloseCount);
            Assert.Same(model, presenter.LastClosed);
        }
        finally { window.Close(); }
    }

    private sealed class Presenter : IDialogPresenter
    {
        public IList<DialogViewModel>? Dialogs { get; set; }
        public int CloseCount { get; private set; }
        public DialogViewModel? LastClosed { get; private set; }
        public Action? Closed { get; set; }
        public void ShowDialog(DialogViewModel? model) { }
        public void CloseDialog(DialogViewModel? model) { LastClosed = model; CloseCount++; Closed?.Invoke(); }
    }
}
