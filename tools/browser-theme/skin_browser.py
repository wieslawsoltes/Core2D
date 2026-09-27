#!/usr/bin/env python3
"""Apply the browser design system to the complete pinned Fluent import.

Run only immediately after import_upstream.py. The outputs are ordinary, checked-in
C#/XAML; consumers never run this script or download upstream sources.
"""
from __future__ import annotations
import json
import re
from pathlib import Path
from xml.dom import minidom as D
import sys

NS = 'https://github.com/avaloniaui'
X = 'http://schemas.microsoft.com/winfx/2006/xaml'
P = Path(sys.argv[1])

def write(name, text):
    p=P/name;p.parent.mkdir(parents=True,exist_ok=True);p.write_text(text.strip()+'\n',encoding='utf-8')
def element(document, tag, **attrs):
    node=document.createElement(tag)
    for key,val in attrs.items():node.setAttribute(key,str(val))
    return node

def direct(parent, tag):return [x for x in parent.childNodes if x.nodeType==x.ELEMENT_NODE and x.tagName==tag]
def set_value(theme, prop, val):
    nodes=[x for x in direct(theme,'Setter') if x.getAttribute('Property')==prop]
    node=nodes[0] if nodes else element(theme.ownerDocument,'Setter',Property=prop)
    for child in list(node.childNodes):node.removeChild(child)
    node.setAttribute('Value',val)
    if not nodes:theme.insertBefore(node,theme.firstChild)

def style(theme,selector,values):
    node=element(theme.ownerDocument,'Style',Selector=selector)
    for p,v in values.items():node.appendChild(element(theme.ownerDocument,'Setter',Property=p,Value=v))
    theme.appendChild(node)

def save_dom(file,doc):
    text=doc.toxml().replace('<?xml version="1.0" ?>','')
    file.write_text(text+'\n',encoding='utf-8')

def resource_doc():
    return D.parseString(f'<ResourceDictionary xmlns="{NS}" xmlns:x="{X}" xmlns:sys="using:System"/>')

# Every original template stays compiled in this assembly. Modify defaults in the
# actual ControlThemes, not global selectors that unexpectedly restyle template children.
row_types={'Button','RepeatButton','ToggleButton','DropDownButton','SplitButton','HyperlinkButton','TextBox','NumericUpDown','AutoCompleteBox','ComboBox','CheckBox','RadioButton','ToggleSwitch','DatePicker','TimePicker','CalendarDatePicker','ListBoxItem','ComboBoxItem','TabItem','TabStripItem','TreeViewItem','MenuItem'}
round_types={'Button','RepeatButton','ToggleButton','DropDownButton','SplitButton','HyperlinkButton','TextBox','NumericUpDown','AutoCompleteBox','ComboBox','ListBox','ListBoxItem','ComboBoxItem','TabItem','TabStripItem','TreeViewItem','MenuItem','Calendar','CalendarDatePicker','DatePicker','TimePicker','Expander'}
for file in sorted((P/'Controls').glob('*.xaml')):
    doc=D.parse(str(file))
    for theme in doc.getElementsByTagName('ControlTheme'):
        target=theme.getAttribute('TargetType')
        key=theme.getAttribute('x:Key')
        if key != '{x:Type '+target+'}':continue
        if target in row_types:set_value(theme,'MinHeight','{DynamicResource BrowserControlHeight}')
        if target in round_types:set_value(theme,'CornerRadius','{DynamicResource BrowserControlRadius}')
        if target in row_types or target in {'Label','Calendar','Expander'}:
            set_value(theme,'FontSize','{DynamicResource BrowserFontSize}')
            set_value(theme,'FontFamily','{DynamicResource BrowserFontFamily}')
        if target in {'Button','RepeatButton','ToggleButton','DropDownButton','SplitButton'}:
            set_value(theme,'HorizontalContentAlignment','Center');set_value(theme,'VerticalContentAlignment','Center')
            set_value(theme,'Padding','{DynamicResource BrowserButtonPadding}')
        if target in {'Window','EmbeddableControlRoot','PopupRoot','OverlayPopupHost'}:
            set_value(theme,'Background','{DynamicResource BrowserSurfaceBrush}')
            set_value(theme,'Foreground','{DynamicResource BrowserTextBrush}')
            set_value(theme,'FontFamily','{DynamicResource BrowserFontFamily}')
            set_value(theme,'FontSize','{DynamicResource BrowserFontSize}')
        if target in {'FlyoutPresenter','MenuFlyoutPresenter','ContextMenu','NotificationCard'}:
            set_value(theme,'CornerRadius','{DynamicResource BrowserOverlayRadius}')
            set_value(theme,'Background','{DynamicResource BrowserSurfaceBrush}')
            set_value(theme,'BorderBrush','{DynamicResource BrowserBorderBrush}')
        if target=='ToolTip':
            set_value(theme,'Background','{DynamicResource BrowserTooltipBrush}')
            set_value(theme,'Foreground','{DynamicResource BrowserTooltipTextBrush}')
            set_value(theme,'CornerRadius','6');set_value(theme,'Padding','9,6');set_value(theme,'MaxWidth','320')
        if target=='TextBox':
            set_value(theme,'SelectionForegroundBrush','White')
            set_value(theme,'VerticalContentAlignment','Center')
            set_value(theme,'MinWidth','0')
            # Named, optional slots support application scrub handles without replacing
            # the upstream presenter, validation, mobile menu or composition machinery.
            for cp in theme.getElementsByTagName('ContentPresenter'):
                if cp.getAttribute('Content')=='{TemplateBinding InnerLeftContent}':
                    cp.setAttribute('Name','PART_InnerLeft');cp.setAttribute('Margin','6,0,0,0')
                    cp.setAttribute('IsVisible','{Binding InnerLeftContent, RelativeSource={RelativeSource TemplatedParent}, Converter={x:Static ObjectConverters.IsNotNull}}')
        if target in {'Button','ToggleButton','RepeatButton','HyperlinkButton','DropDownButton','SplitButton'}:
            # Web buttons change surface, not scale their label and hit target.
            for setter in list(theme.getElementsByTagName('Setter')):
                if setter.getAttribute('Property') in {'RenderTransform','Transitions'}:
                    setter.parentNode.removeChild(setter)
        if target=='Button':
            presenter=' /template/ ContentPresenter#PART_ContentPresenter'
            style(theme,'^:focus-visible'+presenter,{'BorderBrush':'{DynamicResource BrowserAccentBrush}'})
            for cls,bg,fg,border in [('primary','BrowserPrimaryBrush','BrowserOnAccentBrush','BrowserPrimaryBrush'),('ghost','BrowserTransparentBrush','BrowserTextBrush','BrowserTransparentBrush'),('danger','BrowserDangerBackgroundBrush','BrowserDangerBrush','BrowserDangerBrush')]:
                style(theme,'^.'+cls+presenter,{'Background':'{DynamicResource '+bg+'}','Foreground':'{DynamicResource '+fg+'}','BorderBrush':'{DynamicResource '+border+'}'})
                style(theme,'^.'+cls+':pointerover'+presenter,{'Opacity':'0.86'})
                style(theme,'^.'+cls+':disabled'+presenter,{'Background':'{DynamicResource BrowserInputBrush}','Foreground':'{DynamicResource BrowserDisabledBrush}','BorderBrush':'{DynamicResource BrowserBorderBrush}'})
    # All source names and behavioral template bindings are intentionally preserved.
    save_dom(file,doc)

# Compatibility resource vocabulary is retained for third-party Fluent integrations.
# The semantic mapping covers all resource brushes, with intrinsic color glyphs and
# transparencies deliberately retained where they express control behavior.
controls=D.parse(str(P/'Accents/FluentControlResources.xaml'))
keys={}
for n in controls.getElementsByTagName('*'):
    k=n.getAttribute('x:Key')
    if k and n.tagName in {'SolidColorBrush','StaticResource'}:
        src=n.getAttribute('ResourceKey')
        if src.endswith('Color') and n.tagName=='StaticResource':continue
        keys.setdefault(k,(n.tagName,src,n.getAttribute('Color')))

def token(k,src,color):
    if 'Disabled' in k:
        if any(x in k for x in ('Foreground','Glyph','Text')):return 'Disabled'
        if 'Transparent' in src:return 'Transparent'
        return 'Input' if any(x in k for x in ('Background','Fill')) else 'Border'
    if k.startswith('CaptionButton'):return None
    if k.startswith('Hyperlink'):
        if 'Foreground' in k or k.endswith('Brush'):return 'Accent' if 'Visited' not in k else None
        return 'Hover' if 'PointerOver' in k else 'Transparent'
    if 'LightDismiss' in k:return 'Scrim'
    if k.startswith('ScrollBar'):
        if 'Thumb' in k or 'ArrowForeground' in k:return 'Muted'
        return 'Hover' if 'PointerOver' in k else 'Transparent'
    if k.startswith('AccentButton'):
        if 'Foreground' in k:return 'OnAccent'
        return 'Primary' if 'Background' in k else 'Transparent'
    if any(x in k for x in ('CheckGlyph','CheckMark')):
        if 'Unchecked' in k:return 'Transparent'
        return 'OnAccent' if k.startswith('CheckBox') else 'Accent'
    if k.startswith('CheckBoxCheckBackground'):
        if 'Unchecked' not in k:
            return 'Accent' if 'Fill' in k or 'Stroke' in k else None
        return 'BorderHover' if 'Stroke' in k else 'Input'
    if k.startswith('RadioButton') and ('Ellipse' in k or 'Outer' in k or 'CheckBackground' in k):
        return 'Accent' if 'Checked' in k and 'Unchecked' not in k else ('BorderHover' if 'Stroke' in k else 'Input')
    if k.startswith('ToggleSwitch'):
        if 'Knob' in k:return 'OnAccent'
        if 'On' in k and 'Off' not in k and ('Fill' in k or 'Stroke' in k):return 'Accent'
    if 'SelectionHighlight' in k or 'SelectedPipe' in k:return 'Accent'
    if 'Foreground' in k or 'TextBrush' in k:
        if 'Placeholder' in k or 'Watermark' in k:return 'Muted'
        if 'Selected' in k or ('Checked' in k and 'Unchecked' not in k):return 'SelectionText'
        return 'Text'
    if 'Border' in k or 'Stroke' in k:
        if 'Transparent' in src:return 'Transparent'
        if 'Focus' in k:return 'Accent'
        return 'BorderHover' if 'PointerOver' in k or 'Pressed' in k else 'Border'
    if 'Background' in k or k.endswith('Fill'):
        if 'Transparent' in src and not ('Selected' in k or 'Checked' in k):return 'Transparent'
        if 'Selected' in k and 'Unselected' not in k:return 'Selection'
        if 'Checked' in k and 'Unchecked' not in k:return 'Selection'
        if 'PointerOver' in k:return 'Hover'
        if 'Pressed' in k:return 'Selection'
        if any(x in k for x in ('Flyout','Calendar','Menu','Expander','TreeView','ListView','TabItem','Refresh')):return 'Surface'
        return 'Input'
    return None

mapped={k:token(k,src,color) for k,(_,src,color) in keys.items()};mapped={k:v for k,v in mapped.items() if v}
# Preserve radio/checkbox semantics precisely; these tiny components do not use
# the surrounding text's selected-state foreground color.
for k in mapped:
    if k.startswith('RadioButtonCheckGlyphFill') and 'Unchecked' not in k and 'Disabled' not in k:mapped[k]='Accent'
    if k.startswith('ToggleSwitchKnob') and 'Disabled' not in k:mapped[k]='OnAccent'

# Make every adapted template resolve the public semantic brush keys directly.
# Compatibility aliases remain available for third-party themes, but consumers can
# override Browser*Brush on a subtree without a second dictionary of aliases.
for file in sorted((P/'Controls').glob('*.xaml')):
    text=file.read_text()
    text=re.sub(r'\{DynamicResource ([^}]+)\}',
        lambda m: '{DynamicResource Browser'+mapped[m.group(1)]+'Brush}'
        if m.group(1) in mapped else m.group(0), text)
    file.write_text(text)

colors={
 'Light':{'Surface':'#FFFFFF','Canvas':'#F5F5F5','Input':'#F5F5F5','Hover':'#EBEBEB','Border':'#E5E5E5','BorderHover':'#B3B3B3','Text':'#242424','Muted':'#666666','Disabled':'#929292','Selection':'#D9ECFF','SelectionText':'#075BAE','Primary':'#0969DA','OnAccent':'#FFFFFF','Danger':'#B42318','DangerBackground':'#FFF1F0','Tooltip':'#222222','TooltipText':'#FFFFFF','Scrim':'#55000000','Transparent':'Transparent'},
 'Dark':{'Surface':'#2C2C2C','Canvas':'#1E1E1E','Input':'#383838','Hover':'#444444','Border':'#474747','BorderHover':'#777777','Text':'#F5F5F5','Muted':'#B3B3B3','Disabled':'#808080','Selection':'#164B70','SelectionText':'#D1E9FF','Primary':'#1976DE','OnAccent':'#FFFFFF','Danger':'#FFB4AB','DangerBackground':'#4B2424','Tooltip':'#222222','TooltipText':'#FFFFFF','Scrim':'#88000000','Transparent':'Transparent'}}
for variant in colors:colors[variant]['Accent']='#0D99FF'
lines=[f'<ResourceDictionary xmlns="{NS}" xmlns:x="{X}" xmlns:sys="using:System">','  <ResourceDictionary.ThemeDictionaries>']
for variant,palette in colors.items():
    lines.append(f'    <ResourceDictionary x:Key="{variant}">')
    for name,color in palette.items():
        lines.append(f'      <Color x:Key="Browser{name}Color">{color}</Color>')
        lines.append(f'      <SolidColorBrush x:Key="Browser{name}Brush" Color="{color}" />')
    for k,v in sorted(mapped.items()):lines.append(f'      <SolidColorBrush x:Key="{k}" Color="{palette[v]}" />')
    lines.append('    </ResourceDictionary>')
lines+=['  </ResourceDictionary.ThemeDictionaries>',
 '<FontFamily x:Key="BrowserFontFamily">$Default</FontFamily>',
 '<FontFamily x:Key="ContentControlThemeFontFamily">$Default</FontFamily>',
 '<CornerRadius x:Key="BrowserControlRadius">6</CornerRadius>',
 '<CornerRadius x:Key="BrowserOverlayRadius">10</CornerRadius>',
 '<CornerRadius x:Key="ControlCornerRadius">6</CornerRadius>',
 '<CornerRadius x:Key="OverlayCornerRadius">10</CornerRadius>',
 '<Thickness x:Key="TextControlBorderThemeThicknessFocused">1</Thickness>',
 '<sys:Double x:Key="ScrollBarSize">12</sys:Double>',
 '<BoxShadows x:Key="BrowserOverlayShadow">0 4 20 0 #22000000</BoxShadows>',
 '</ResourceDictionary>']
write('BrowserTokens.axaml','\n'.join(lines))
# Locally declared metrics take precedence over global resources. Rewrite their
# default values as well as the density dictionary used by the resource provider.
for density,height,size,pad in [('Compact',28,12,'8,4'),('Comfortable',36,13,'12,7'),('Touch',44,14,'14,10')]:
    metrics={'BrowserControlHeight':('sys:Double',str(height)),'BrowserFontSize':('sys:Double',str(size)),
      'BrowserButtonPadding':('Thickness',pad),'ControlContentThemeFontSize':('sys:Double',str(size)),
      'TextControlThemeMinHeight':('sys:Double',str(height)),'TextControlThemeMinWidth':('sys:Double','0'),
      'TextControlThemePadding':('Thickness','7,4' if density=='Compact' else pad),
      'ListBoxItemPadding':('Thickness',pad),'ListViewItemMinHeight':('sys:Double',str(height)),
      'ComboBoxMinHeight':('sys:Double',str(height)),'ComboBoxPadding':('Thickness',pad),
      'ComboBoxItemThemePadding':('Thickness',pad),'CheckBoxMinHeight':('sys:Double',str(height)),
      'RadioButtonMinHeight':('sys:Double',str(height)),'TreeViewItemMinHeight':('sys:Double',str(height)),
      'TabItemMinHeight':('sys:Double',str(height)),'TabItemHeaderMargin':('Thickness','10,0'),
      'ButtonPadding':('Thickness',pad),'MenuFlyoutItemThemePaddingNarrow':('Thickness',pad),
      'MenuBarItemPadding':('Thickness',pad),'TimePickerHostPadding':('Thickness','0,3'),
      'DatePickerHostPadding':('Thickness','0,3'),'DatePickerHostMonthPadding':('Thickness','8,3,0,3')}
    write('DensityStyles/'+density+'.xaml',f'<ResourceDictionary xmlns="{NS}" xmlns:x="{X}" xmlns:sys="using:System">\n'+
       '\n'.join(f'  <{t} x:Key="{k}">{v}</{t}>' for k,(t,v) in metrics.items())+'\n</ResourceDictionary>')

# Common web typography and composition primitives have no stateful behavior.
write('BrowserPrimitives.axaml',f'''<Styles xmlns="{NS}" xmlns:x="{X}">
  <Style Selector="TextBlock"><Setter Property="FontSize" Value="{{DynamicResource BrowserFontSize}}" /></Style>
  <Style Selector="ToolTip TextBlock"><Setter Property="TextWrapping" Value="Wrap" /></Style>
  <Style Selector="Border.browser-card">
    <Setter Property="Background" Value="{{DynamicResource BrowserSurfaceBrush}}" />
    <Setter Property="BorderBrush" Value="{{DynamicResource BrowserBorderBrush}}" />
    <Setter Property="BorderThickness" Value="1" /><Setter Property="CornerRadius" Value="10" />
    <Setter Property="Padding" Value="20" />
  </Style>
  <Style Selector="TextBlock.browser-heading"><Setter Property="FontSize" Value="22" /><Setter Property="FontWeight" Value="SemiBold" /></Style>
  <Style Selector="TextBlock.browser-muted"><Setter Property="Foreground" Value="{{DynamicResource BrowserMutedBrush}}" /></Style>
</Styles>''')

# Replace the upstream entry point with a standalone Styles provider. All source
# helper classes were imported, and do not reference Avalonia.Themes.Fluent.dll.
write('BrowserTheme.xaml',f'''<Styles x:Class="Avalonia.Themes.Browser.BrowserTheme" xmlns="{NS}" xmlns:x="{X}"
 xmlns:browser="using:Avalonia.Themes.Browser" xmlns:accents="using:Avalonia.Themes.Browser.Accents">
 <Styles.Resources><ResourceDictionary><ResourceDictionary.MergedDictionaries>
  <ResourceInclude Source="avares://Avalonia.Themes.Browser/Accents/BaseColorsPalette.xaml" />
  <accents:SystemAccentColors /><browser:ColorPaletteResourcesCollection />
  <ResourceInclude Source="avares://Avalonia.Themes.Browser/Accents/BaseResources.xaml" />
  <ResourceInclude Source="avares://Avalonia.Themes.Browser/Accents/FluentControlResources.xaml" />
  <ResourceInclude Source="avares://Avalonia.Themes.Browser/Strings/InvariantResources.xaml" />
  <ResourceInclude Source="avares://Avalonia.Themes.Browser/BrowserTokens.axaml" />
 </ResourceDictionary.MergedDictionaries>
 <ResourceInclude x:Key="CompactStyles" Source="avares://Avalonia.Themes.Browser/DensityStyles/Compact.xaml" />
 <ResourceInclude x:Key="ComfortableStyles" Source="avares://Avalonia.Themes.Browser/DensityStyles/Comfortable.xaml" />
 <ResourceInclude x:Key="TouchStyles" Source="avares://Avalonia.Themes.Browser/DensityStyles/Touch.xaml" />
 </ResourceDictionary></Styles.Resources>
 <StyleInclude Source="avares://Avalonia.Themes.Browser/Controls/BrowserControls.xaml" />
 <StyleInclude Source="avares://Avalonia.Themes.Browser/BrowserPrimitives.axaml" />
</Styles>''')
write('BrowserTheme.xaml.cs','''// Browser design system. Fluent-derived support resources: LICENSE.Avalonia.txt.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.Themes.Browser;

/// <summary>Control density in device-independent pixels.</summary>
public enum BrowserDensity
{
    /// <summary>28-DIP inputs for dense authoring applications.</summary>
    Compact,
    /// <summary>36-DIP inputs for ordinary desktop/web-style interfaces.</summary>
    Comfortable,
    /// <summary>44-DIP inputs for larger pointer targets.</summary>
    Touch
}

/// <summary>A standalone, complete browser-style theme for Avalonia's built-in controls.</summary>
public sealed class BrowserTheme : Styles, IResourceNode
{
    private readonly ResourceDictionary[] _densities;
    private BrowserDensity _density = BrowserDensity.Comfortable;
    /// <summary>Defines the live control-density selection.</summary>
    public static readonly DirectProperty<BrowserTheme, BrowserDensity> DensityProperty =
        AvaloniaProperty.RegisterDirect<BrowserTheme, BrowserDensity>(nameof(Density), x => x.Density, (x, value) => x.Density = value);
    /// <summary>Loads all local template and support resources; no FluentTheme instance is required.</summary>
    public BrowserTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);
        _densities = new[] { Take("CompactStyles"), Take("ComfortableStyles"), Take("TouchStyles") };
        Palettes = Resources.MergedDictionaries.OfType<ColorPaletteResourcesCollection>().Single();
    }
    /// <summary>Gets or sets live density without recreating control templates or their input state.</summary>
    public BrowserDensity Density
    {
        get => _density;
        set
        {
            if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(nameof(value));
            if (SetAndRaise(DensityProperty, ref _density, value)) Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
        }
    }
    /// <summary>Legacy base-palette customization for Fluent-compatible third-party resources.</summary>
    public IDictionary<ThemeVariant, ColorPaletteResources> Palettes { get; }
    private ResourceDictionary Take(string name)
    {
        ResourceDictionary dictionary = (ResourceDictionary)Resources[name]!;
        Resources.Remove(name);
        return dictionary;
    }
    bool IResourceNode.TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        if (_densities is not null && _densities[(int)_density].TryGetResource(key, theme, out value)) return true;
        return base.TryGetResource(key, theme, out value);
    }
}
''')
# Remove the obsolete enum declaration copied with FluentTheme; the new class is above.
write('Properties/AssemblyInfo.cs','''using Avalonia.Metadata;
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "Avalonia.Themes.Browser")]
[assembly: XmlnsDefinition("https://themes.core2d.dev/browser", "Avalonia.Themes.Browser")]
[assembly: XmlnsPrefix("https://themes.core2d.dev/browser", "browser")]
''')
write('Directory.Build.props','<Project><PropertyGroup><Deterministic>true</Deterministic><LangVersion>latest</LangVersion></PropertyGroup></Project>')
write('Directory.Packages.props','<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>')
write('Avalonia.Themes.Browser.csproj','''<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup>
  <TargetFramework>net8.0</TargetFramework><Nullable>enable</Nullable><ImplicitUsings>disable</ImplicitUsings>
  <PackageId>Avalonia.Themes.Browser</PackageId><Version>11.3.12-preview.1</Version>
  <Authors>Wiesław Šoltés; AvaloniaUI contributors</Authors>
  <Description>Independent browser-style native Avalonia theme, derived from the complete MIT Fluent 11.3.12 template set.</Description>
  <PackageLicenseExpression>MIT</PackageLicenseExpression><PackageReadmeFile>README.md</PackageReadmeFile>
  <RepositoryUrl>https://github.com/wieslawsoltes/Core2D</RepositoryUrl><RepositoryType>git</RepositoryType>
  <PackageTags>avalonia;theme;browser;design-system;xaml</PackageTags>
  <IsPackable>true</IsPackable><GenerateDocumentationFile>true</GenerateDocumentationFile>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
 </PropertyGroup>
 <ItemGroup><PackageReference Include="Avalonia" Version="11.3.12" /></ItemGroup>
 <ItemGroup>
  <AvaloniaResource Include="**/*.xaml" />
  <None Update="README.md;LICENSE.Avalonia.txt;UpstreamManifest.json" Pack="true" PackagePath="/" />
 </ItemGroup>
</Project>''')
write('README.md','''# Avalonia.Themes.Browser

An independent, MIT-licensed browser-style design system for native Avalonia 11.3.12.
This is not an official Avalonia package. It contains the entire Fluent template
catalog and support resources from commit `37fbd9655cc581ff5b1c6b1fb1be4e3118c889d0`,
with a new neutral surface system, density metrics, compact typography and button
variants. The package depends only on Avalonia, not Core2D or FluentTheme.

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:browser="using:Avalonia.Themes.Browser">
  <Application.Styles>
    <browser:BrowserTheme Density="Comfortable" />
  </Application.Styles>
</Application>
```

Use `RequestedThemeVariant` on the application/window or `ThemeVariantScope` for
Light/Dark. `Density` supports Compact (28), Comfortable (36), Touch (44). Change
it on the existing BrowserTheme instance to retain input focus and edit state.
`Button` supports `primary`, `ghost`, `danger` and upstream `accent` classes.
`Border.browser-card`, `TextBlock.browser-heading` and `TextBlock.browser-muted`
provide composition primitives. Override `Browser*` resources in application or
local resources; existing Fluent resource names remain as compatibility hooks.
No font binary is bundled. Register Inter in the host or use the system fallback.

Every imported control theme is shipped as compiled XAML, including date/time
presenters, calendar parts, notifications, managed file dialogs and selection
handles. Layout-only and self-rendering controls do not require control templates.
Third-party Dock, ProDataGrid and AvaloniaEdit templates are not built-in Avalonia
controls and remain app-owned integrations. Their base resources are available.

## Build and provenance

`dotnet pack Avalonia.Themes.Browser.csproj -c Release` builds without Core2D.
`UpstreamManifest.json` records original file hashes, theme keys and named parts.
The maintainer importer and skin tool live in `tools/browser-theme` in Core2D;
normal builds never download or regenerate upstream sources. Keep the original
MIT license with redistributed source/binaries. No public NuGet publish occurs
as part of a build.

Browser-style means the cross-platform web-app design language, not a DOM engine
or a claim that OS pickers and text rasterization match every browser. Native
input/automation/IME behaviors remain supplied by Avalonia. Preview maturity:
headless coverage and platform build status are reported in the PR; interactive
screen-reader, touch, native file-dialog and OS window chrome validation remain
separate requirements.
''')
manifest=json.loads((P/'UpstreamManifest.json').read_text()); manifest['adaptations']={'brushMappings':len(mapped),'densityModes':3,'templateCatalog':'Controls/BrowserControls.xaml','sourceBuildDependency':'none'}
(P/'UpstreamManifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
print(f'Browser skin applied: {len(mapped)} state brushes, three density modes.')
