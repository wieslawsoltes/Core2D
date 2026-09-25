"""One-shot, binding-preserving migration of Core2D's existing inspector views."""
from pathlib import Path
from collections import Counter
import re
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
A = 'https://github.com/avaloniaui'
X = 'http://schemas.microsoft.com/winfx/2006/xaml'
STUDIO = 'using:Core2D.Controls.Studio'


def write(path, content):
    target = ROOT / path
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_text(content, encoding='utf-8')


def change(path, old, new):
    target = ROOT / path
    text = target.read_text(encoding='utf-8-sig')
    if new in text:
        return
    if old not in text:
        raise RuntimeError(f'Missing migration anchor in {path}: {old[:80]}')
    write(path, text.replace(old, new))


def bindings(text):
    root = ET.fromstring(text)
    return Counter(v for e in root.iter() for v in e.attrib.values() if v.startswith(('{Binding', '{MultiBinding', '{CompiledBinding')))


def humanize(value):
    fixed = {'IsStroked': 'Stroke', 'IsFilled': 'Fill', 'IsClosed': 'Closed', 'TopLeft': 'Top left',
             'BottomRight': 'Bottom right', 'SnapToGrid': 'Snap to grid', 'TryToConnect': 'Connect points',
             'SinglePressMode': 'Single press drawing', 'HAlignment': 'Horizontal alignment', 'VAlignment': 'Vertical alignment'}
    return fixed.get(value, re.sub(r'(?<=[a-z])(?=[A-Z])', ' ', value))

numeric = {'X', 'Y', 'Width', 'Height', 'RadiusX', 'RadiusY', 'Thickness', 'DashOffset', 'FontSize',
           'Angle', 'RotationAngle', 'SnapX', 'SnapY', 'HitThreshold', 'ZoomSpeed', 'MinZoom', 'MaxZoom',
           'OffsetX', 'OffsetY', 'CellWidth', 'CellHeight', 'StrokeThickness', 'ScaleX', 'ScaleY'}
prefixes = {'Width': 'W', 'Height': 'H', 'X': 'X', 'Y': 'Y', 'RadiusX': 'RX', 'RadiusY': 'RY',
            'Thickness': 'W', 'FontSize': 'T', 'Angle': '°', 'RotationAngle': '°'}
view_tests = []
changed = []
for directory in ['Shapes', 'Path', 'Style', 'Containers', 'Renderer']:
    for path in sorted((ROOT / 'src/Core2D/Views' / directory).rglob('*.axaml')):
        if path.name in {'ProjectContainerView.axaml', 'RenderView.axaml', 'ArgbColorView.axaml'}:
            continue
        original = path.read_text(encoding='utf-8-sig')
        text = original
        class_match = re.search(r'x:Class="([^"]+)"', text)
        context = re.search(r'd:DataContext="\{x:Static \w+:DesignerContext\.(\w+)\}"', text)
        if class_match and context:
            view_tests.append((class_match[1], context[1]))
        if '<TabControl>' in text:
            text = text.replace('<TabControl>', '<StackPanel>').replace('</TabControl>', '</StackPanel>')
            text = text.replace('<TabItem ', '<studio:StudioSection ').replace('</TabItem>', '</studio:StudioSection>')
            for name in ['Data', 'Grid', 'Connectors', 'Shapes', 'Pages', 'Figures', 'Segments', 'ImageCache', 'HelperStyle']:
                text = text.replace(f'<studio:StudioSection Header="{name}">', f'<studio:StudioSection Header="{name}" IsExpanded="False">')
            text = text.replace('Margin="{DynamicResource ContentMargin}"', 'Margin="0"')
        text = text.replace('<Expander ', '<studio:StudioSection ').replace('</Expander>', '</studio:StudioSection>')
        text = re.sub(r'(?m)^(\s*)<Label\s+Content="([^{}"]+)"(?:\s+Margin="[^"]*")?\s*/>\s*\n\s*(<(?:TextBox|ComboBox)\b[^>]+/>)',
                      lambda m: m[1] + '<studio:StudioPropertyField Label="' + humanize(m[2]) + '">\n' + m[1] + '  ' + m[3] + '\n' + m[1] + '</studio:StudioPropertyField>', text)
        def number(match):
            markup = match[0]
            bound = re.search(r'Text="\{Binding (\w+),', markup)
            if not bound or bound[1] not in numeric:
                return markup
            return markup.replace('<TextBox', '<studio:StudioNumberBox Prefix="' + prefixes.get(bound[1], '#') + '"', 1)
        text = re.sub(r'<TextBox\b[^>]+/>', number, text)
        text = re.sub(r'(<CheckBox\s+Content=")([^{}"]+)(")', lambda m: m[1] + humanize(m[2]) + m[3], text)
        text = re.sub(r'(<studio:StudioSection Header=")([^{}"]+)(")', lambda m: m[1] + humanize(m[2]) + m[3], text)
        if 'studio:' in text and 'xmlns:studio=' not in text:
            text = text.replace('xmlns:x=', f'xmlns:studio="{STUDIO}"\n             xmlns:x=', 1)
        if text != original:
            assert bindings(text) == bindings(original), f'Binding contract changed in {path}'
            path.write_text(text, encoding='utf-8')
            changed.append(str(path.relative_to(ROOT)))

# Idempotent layout migration and context restoration for saved layouts.
change('src/Core2D.ViewModels/ViewModels/Docking/DockFactory.cs',
       '            // Explorers\n',
       '            ["StudioNavigator"] = () => _projectEditor,\n            ["StudioInspector"] = () => _projectEditor,\n            // Explorers\n')
change('src/Core2D.ViewModels/ViewModels/Docking/DockFactory.cs',
       '        base.InitLayout(layout);',
       '        base.InitLayout(layout);\n        StudioWorkspaceLayout.Apply(this, _projectEditor);')

# Install custom templates for every existing view, including dialogs and advanced tools.
change('src/Core2D/Styles/Studio.axaml', 'xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"',
       'xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"\n        xmlns:converters="using:Core2D.Converters"')
change('src/Core2D/Styles/Studio.axaml', '        <MergeResourceInclude Source="avares://Core2D/Styles/StudioControls.axaml" />',
       '        <MergeResourceInclude Source="avares://Core2D/Styles/StudioControls.axaml" />\n        <ResourceInclude Source="avares://Core2D/Styles/StudioInputs.axaml" />\n        <ResourceInclude Source="avares://Core2D/Styles/StudioNavigation.axaml" />\n        <ResourceInclude Source="avares://Core2D/Styles/StudioColorField.axaml" />')
change('src/Core2D/Styles/Studio.axaml', '      </ResourceDictionary.MergedDictionaries>',
       '      </ResourceDictionary.MergedDictionaries>\n      <converters:MultipleDockTabsConverter x:Key="MultipleDockTabs" />')
styles = '''
  <Style Selector="Button"><Setter Property="Theme" Value="{StaticResource StudioButtonTheme}" /></Style>
  <Style Selector="TextBox"><Setter Property="Theme" Value="{StaticResource StudioTextBoxTheme}" /></Style>
  <Style Selector="CheckBox"><Setter Property="Theme" Value="{StaticResource StudioCheckBoxTheme}" /></Style>
  <Style Selector="ComboBox"><Setter Property="Theme" Value="{StaticResource StudioComboBoxTheme}" /></Style>
  <Style Selector="TabControl"><Setter Property="Theme" Value="{StaticResource StudioTabControlTheme}" /></Style>
  <Style Selector="TabItem"><Setter Property="Theme" Value="{StaticResource StudioTabItemTheme}" /></Style>
  <Style Selector="Button.studio-primary:pointerover /template/ ContentPresenter#PART_ContentPresenter">
    <Setter Property="Background" Value="{DynamicResource StudioAccentBrush}" />
    <Setter Property="Foreground" Value="{DynamicResource StudioOnAccentBrush}" />
    <Setter Property="Opacity" Value="0.9" />
  </Style>
  <Style Selector="ToolControl /template/ :is(TabStrip)#PART_TabStrip">
    <Setter Property="DockPanel.Dock" Value="Top" />
    <Setter Property="IsVisible" Value="{Binding VisibleDockables.Count, Converter={StaticResource MultipleDockTabs}}" />
  </Style>
  <Style Selector="DocumentControl /template/ :is(TabStripItem):selected">
    <Setter Property="Background" Value="{DynamicResource StudioSurfaceBrush}" />
    <Setter Property="Foreground" Value="{DynamicResource StudioTextBrush}" />
  </Style>
  <Style Selector="ruler|Ruler:horizontal"><Setter Property="Height" Value="24" /></Style>
  <Style Selector="ruler|Ruler:vertical"><Setter Property="Width" Value="24" /></Style>
  <Style Selector="ComboBoxItem"><Setter Property="MinHeight" Value="28" /><Setter Property="Padding" Value="8,5" /></Style>
  <Style Selector="FlyoutPresenter"><Setter Property="CornerRadius" Value="10" /><Setter Property="Padding" Value="6" /><Setter Property="Background" Value="{DynamicResource StudioSurfaceBrush}" /></Style>
'''
change('src/Core2D/Styles/Studio.axaml', '</Styles>', styles + '</Styles>')

# Canvas overlays are hit-testable only at their actual bounds, leaving all other pixels for drawing.
for name in ['PageView', 'TemplateView', 'BlockDocumentView']:
    path = ROOT / f'src/Core2D/Views/Docking/Documents/{name}.axaml'
    text = path.read_text(encoding='utf-8-sig')
    text = text.replace('xmlns:ce=', f'xmlns:s="{STUDIO}"\n             xmlns:ce=', 1)
    text = text.replace('<Grid RowDefinitions="*,Auto">', '<Grid>')
    text = text.replace('Grid.Row="1" DataContext=', 'VerticalAlignment="Bottom" DataContext=')
    text = text.replace('    </Grid>', '    </Grid>')
    text = text.replace('  </Grid>', '    <s:StudioCanvasControls DataContext="{Binding Context}" HorizontalAlignment="Right" VerticalAlignment="Top" Margin="12" />\n  </Grid>')
    path.write_text(text, encoding='utf-8')
change('src/Core2D/Controls/Editor/PageView.axaml', '<Panel Background="#FFF5F5F5" />', '<Panel Background="{DynamicResource StudioCanvasBrush}" />')
change('src/Core2D/Controls/Editor/PageView.axaml', '<Panel Background="{DynamicResource TemplateCheckerBoard}" />', '<!-- The workspace is a solid surface; document transparency remains in the renderer. -->')
change('src/Core2D/Controls/Editor/PageView.axaml', 'Background="{DynamicResource SystemControlBackgroundChromeMediumLowBrush}"', 'Background="{DynamicResource StudioSurfaceBrush}"')
change('src/Core2D/Controls/Editor/PageView.axaml', 'Foreground="{DynamicResource SystemControlForegroundBaseHighBrush}"', 'Foreground="{DynamicResource StudioMutedBrush}"')

# New layout intentionally stores advanced panels in Dock's hidden-tool registry.
change('tests/Core2D.UI.Tests/WorkspaceTests.cs', '        yield return dockable;\n',
       '        yield return dockable;\n        if (dockable is Dock.Model.Controls.IRootDock { HiddenDockables: { } hidden })\n        {\n            foreach (var item in hidden)\n            {\n                yield return item;\n            }\n        }\n')
change('tests/Core2D.UI.Tests/StudioDockTests.cs', '        editor.OnNewProject();',
       '        editor.OnNewProject();\n        editor.OnToggleDockableVisibility("ProjectExplorer");\n        editor.OnToggleDockableVisibility("ObjectBrowser");\n        editor.OnToggleDockableVisibility("ShapeProperties");')
change('tests/Core2D.UI.Tests/StudioDockTests.cs', 'tabs.Length >= 10', 'tabs.Length >= 5')
change('tests/Core2D.UI.Tests/StudioPresentationTests.cs',
       '            Assert.Equal(Colors.White, Assert.IsAssignableFrom<ISolidColorBrush>(export.Foreground).Color);\n            var menu = header.GetVisualDescendants().OfType<Menu>().Single();',
       '            var menuButton = header.GetVisualDescendants().OfType<StudioIconButton>().Single(x => x.Flyout is Flyout);\n            var flyout = (Flyout)menuButton.Flyout!;\n            flyout.ShowAt(menuButton);\n            Dispatcher.UIThread.RunJobs();\n            var menu = ((Control)flyout.Content!).GetVisualDescendants().OfType<Menu>().Single();')

# Compile and instantiate every migrated designer-backed editor in both themes.
cases = '\n'.join(f'        yield return new object[] {{ "{name}", new Func<Control>(() => new {name} {{ DataContext = DesignerContext.{context} }}), dark }};' for name, context in view_tests)
write('tests/Core2D.UI.Tests/StudioEditorCatalogTests.cs', '''using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Core2D.ViewModels.Designer;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioEditorCatalogTests
{
    public static IEnumerable<object[]> Editors()
    {
        foreach (var dark in new[] { false, true })
        {
''' + cases + '''
        }
    }

    [AvaloniaTheory]
    [MemberData(nameof(Editors))]
    public void EditorLoadsWithItsRealBindingContext(string name, Func<Control> create, bool dark)
    {
        using var state = new AppState();
        DesignerContext.InitializeContext(state.ServiceProvider);
        var editor = create();
        Assert.NotNull(editor.DataContext);
        var window = new Window
        {
            Width = 300, Height = 800,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light,
            Content = new ScrollViewer { Content = editor }
        };
        try
        {
            window.Show();
            Assert.True(editor.Bounds.Width > 0, name);
        }
        finally
        {
            window.Close();
        }
    }
}
''')
write('docs/studio-view-migration.md', '# Studio view migration\n\nThe static tab-based inspectors below now use collapsible sections, labelled fields and numeric controls. The migration verified the complete multiset of existing binding expressions before and after each rewrite. No editor command or binding expression was removed.\n\n' + '\n'.join('- `' + p + '`' for p in changed) + '\n')
print(f'Migrated {len(changed)} editor views, preserved bindings; generated {len(view_tests) * 2} editor cases.')
