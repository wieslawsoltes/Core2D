using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Headless.XUnit;
using Core2D.Json;
using Core2D.ViewModels.Docking;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioPersistenceTests
{
    [AvaloniaFact]
    public void SavedSidebarLayoutRestoresContextAndAllAdvancedPanels()
    {
        using var state = new AppState();
        var editor = state.Editor!;
        editor.OnNewProject();
        var factory = Assert.IsType<DockFactory>(editor.DockFactory);
        var root = Assert.IsAssignableFrom<IRootDock>(editor.RootDock);
        var left = Assert.IsAssignableFrom<IToolDock>(factory.HomeDock!.VisibleDockables![0]);
        left.Proportion = 0.27;
        editor.OnToggleDockableVisibility("BlockLibrary");
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new ListContractResolver(typeof(ObservableCollection<>)),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = { new KeyValuePairConverter() }
        };
        // This uses the same trusted local-layout serializer contract as AppState, not external input.
        var serialized = JsonConvert.SerializeObject(root, settings);
        var restored = JsonConvert.DeserializeObject<RootDock>(serialized, settings)!;
        Assert.NotNull(restored);
        factory.InitLayout(restored);
        editor.RootDock = restored;
        var restoredLeft = Assert.IsAssignableFrom<IToolDock>(factory.HomeDock!.VisibleDockables![0]);
        Assert.Equal(0.27, restoredLeft.Proportion);
        Assert.Contains(restoredLeft.VisibleDockables!, x => x.Id == "BlockLibrary");
        var tools = StudioDockGraph.Enumerate(restored).OfType<ITool>().ToArray();
        Assert.Equal(18, tools.Length);
        Assert.Equal(18, tools.Select(x => x.Id).Distinct().Count());
        Assert.All(tools, tool => Assert.Same(editor, tool.Context));
        foreach (var tool in tools)
        {
            Assert.Same(tool, factory.GetDockable<IDockable>(tool.Id));
        }
        Assert.False(StudioWorkspaceLayout.Apply(factory, editor));
    }
}
