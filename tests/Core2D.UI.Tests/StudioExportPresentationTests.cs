using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Wizard.Export;
using Core2D.ViewModels.Wizard.Export.Steps;
using Core2D.Views.Wizard.Export;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioExportPresentationTests
{
    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void ExportKeepsStepValidationAndRendersDestinationWithoutOverlap(bool dark)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var context = new ExportWizardContext(state.ServiceProvider) { Project = state.Editor!.Project };
        var scope = new ScopeWizardStepViewModel(state.ServiceProvider);
        var destination = new DestinationWizardStepViewModel(state.ServiceProvider);
        var wizard = new ExportWizardViewModel(state.ServiceProvider, context, new IWizardStepViewModel[] { scope, destination }, new WizardNavigationService(), new ExportWizardTelemetry(null));
        var view = new ExportWizardView { DataContext = wizard };
        var window = new Window { Width = 820, Height = 680, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Assert.Same(scope, wizard.CurrentStep);
            Assert.False(wizard.NextCommand.CanExecute(null));
            Assert.Equal(2, view.GetVisualDescendants().OfType<StudioStepIndicator>().Count());
            // Inspect a second real step template without bypassing validation in the UI itself.
            wizard.CurrentStep = destination;
            Dispatcher.UIThread.RunJobs();
            var folder = view.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Text == "Output folder");
            var subfolder = view.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Text == "Subfolder template");
            Assert.True(subfolder.TranslatePoint(default, view)!.Value.Y > folder.TranslatePoint(default, view)!.Value.Y + folder.Bounds.Height);
            context.DestinationFolder = "/tmp/studio-exports";
            Assert.Contains(view.GetVisualDescendants().OfType<TextBox>(), x => x.Text == "/tmp/studio-exports");
            Assert.All(view.GetVisualDescendants().OfType<StudioStepIndicator>(), x => Assert.Same(destination, x.CurrentStep));
            StudioSecondaryViewTests.Capture(window, $"export-destination-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }
}
