#!/usr/bin/env python3
"""Refine a freshly imported/skinned theme without dropping native template contracts.

Run import_upstream.py and skin_browser.py first. The handwritten supplementary
resources are checked in alongside the generated templates. No build invokes
this maintainer tool and it does not download or execute external source.
"""
from __future__ import annotations

import argparse
import json
from pathlib import Path
from shutil import copyfile
from xml.dom import minidom as xml

BRUSHES = {
    'SystemControlHighlightAccentBrush': 'BrowserAccentBrush',
    'SystemControlBackgroundBaseLowBrush': 'BrowserInputBrush',
    'SystemControlBackgroundAltHighBrush': 'BrowserSurfaceBrush',
    'SystemControlBackgroundChromeMediumLowBrush': 'BrowserCanvasBrush',
    'SystemControlForegroundBaseHighBrush': 'BrowserTextBrush',
    'SystemControlForegroundBaseMediumBrush': 'BrowserMutedBrush',
    'SystemControlForegroundBaseMediumLowBrush': 'BrowserMutedBrush',
    'SystemControlForegroundBaseLowBrush': 'BrowserDisabledBrush',
    'SystemControlHighlightTransparentBrush': 'BrowserTransparentBrush',
}


def element(document, tag, **attributes):
    result = document.createElement(tag)
    for name, value in attributes.items():
        result.setAttribute(name, value)
    return result


def setter(theme, property_name, value):
    node = next((child for child in theme.childNodes
                 if child.nodeType == child.ELEMENT_NODE and child.tagName == 'Setter'
                 and child.getAttribute('Property') == property_name), None)
    if node is None:
        node = element(theme.ownerDocument, 'Setter', Property=property_name)
        theme.insertBefore(node, theme.firstChild)
    for child in list(node.childNodes):
        node.removeChild(child)
    node.setAttribute('Value', value)


def style(theme, selector, **values):
    node = element(theme.ownerDocument, 'Style', Selector=selector)
    for property_name, value in values.items():
        node.appendChild(element(theme.ownerDocument, 'Setter', Property=property_name, Value=value))
    theme.appendChild(node)


def save(path: Path, document):
    path.write_text(document.toxml().replace('<?xml version="1.0" ?>', '') + '\n')


def refine(root: Path):
    manifest_path = root / 'UpstreamManifest.json'
    manifest = json.loads(manifest_path.read_text())
    if 'contractRefinements' in manifest.get('adaptations', {}):
        raise RuntimeError('Run the importer and skin pass first; do not refine an already refined catalog.')

    for path in sorted((root / 'Controls').glob('*.xaml')):
        text = path.read_text()
        for original, replacement in BRUSHES.items():
            text = text.replace('{DynamicResource ' + original + '}', '{DynamicResource ' + replacement + '}')
        path.write_text(text)
        document = xml.parseString(text)
        main = next((theme for theme in document.getElementsByTagName('ControlTheme')
                     if theme.getAttribute('x:Key') == '{x:Type ' + theme.getAttribute('TargetType') + '}'), None)
        if main is None:
            continue
        target = main.getAttribute('TargetType')
        if target == 'Button':
            for name, background, foreground in (
                ('primary', 'BrowserPrimaryBrush', 'BrowserOnAccentBrush'),
                ('ghost', 'BrowserTransparentBrush', 'BrowserTextBrush'),
                ('danger', 'BrowserDangerBackgroundBrush', 'BrowserDangerBrush'),
            ):
                style(main, '^.' + name, Background='{DynamicResource ' + background + '}',
                      Foreground='{DynamicResource ' + foreground + '}')
        if target == 'TabControl':
            setter(main, 'Padding', '0')
        if target in ('TabItem', 'TabStripItem'):
            style(main, '^:selected', Background='{DynamicResource BrowserInputBrush}',
                  Foreground='{DynamicResource BrowserTextBrush}', FontWeight='SemiBold')
            style(main, '^:selected /template/ Border#PART_SelectedPipe', Opacity='0')
            style(main, '^:selected:pointerover /template/ Border#PART_LayoutRoot',
                  **{'Background': '{DynamicResource BrowserHoverBrush}',
                     'TextElement.Foreground': '{DynamicResource BrowserTextBrush}'})
        if target == 'ComboBoxItem':
            presenter = next(node for node in main.getElementsByTagName('ContentPresenter')
                             if node.getAttribute('Name') == 'PART_ContentPresenter')
            grid = element(document, 'Grid')
            presenter.parentNode.replaceChild(grid, presenter)
            presenter.setAttribute('Padding', '28,5,10,5')
            grid.appendChild(presenter)
            grid.appendChild(element(document, 'Path', Name='PART_Checkmark', Data='M1,6 L4,9 L11,2',
                Stroke='{DynamicResource BrowserAccentBrush}', StrokeThickness='1.5', Width='12', Height='12',
                HorizontalAlignment='Left', VerticalAlignment='Center', Margin='8,0,0,0', IsHitTestVisible='False',
                IsVisible='{Binding IsSelected, RelativeSource={RelativeSource TemplatedParent}}'))
        if target == 'ComboBox':
            setter(main, 'PlaceholderForeground', '{DynamicResource BrowserMutedBrush}')
            setter(main, 'HorizontalContentAlignment', 'Stretch')
        if target == 'ScrollBar':
            style(main, '^:vertical', Width='{DynamicResource ScrollBarSize}')
            style(main, '^:horizontal', Height='{DynamicResource ScrollBarSize}')
        if target == 'TextBox':
            setter(main, 'SelectionBrush', '{DynamicResource BrowserPrimaryBrush}')
        if target == 'GridSplitter':
            setter(main, 'Background', '{DynamicResource BrowserBorderBrush}')
            style(main, '^:pointerover', Background='{DynamicResource BrowserAccentBrush}')
        if target == 'Menu':
            setter(main, 'Background', '{DynamicResource BrowserSurfaceBrush}')
        save(path, document)

    path = root / 'Controls/FlyoutPresenter.xaml'
    document = xml.parse(str(path))
    for border in document.getElementsByTagName('Border'):
        if border.getAttribute('Name') == 'PART_LayoutRoot':
            border.setAttribute('BoxShadow', '{DynamicResource BrowserOverlayShadow}')
    save(path, document)

    path = root / 'BrowserTokens.axaml'
    path.write_text(path.read_text().replace('<sys:Double x:Key="ScrollBarSize">12</sys:Double>',
        '<sys:Double x:Key="ScrollBarSize">12</sys:Double>\n'
        ' <Thickness x:Key="MenuFlyoutPresenterThemePadding">4</Thickness>\n'
        ' <Thickness x:Key="FlyoutBorderThemePadding">12</Thickness>'))

    path = root / 'Controls/TextBox.xaml'
    document = xml.parse(str(path))
    flyout = next(node for node in document.getElementsByTagName('MenuFlyout')
                  if node.getAttribute('x:Key') == 'DefaultTextBoxContextFlyout')
    flyout.appendChild(element(document, 'Separator'))
    flyout.appendChild(element(document, 'MenuItem', Header='Select all', Command='{Binding $parent[TextBox].SelectAll}'))
    save(path, document)

    # Preserve handwritten visual refinements and the exact upstream notices.
    maintained = Path(__file__).resolve().parents[2] / 'src/Avalonia.Themes.Browser'
    for name in ('BrowserVisualRefinements.axaml', 'BrowserCompatibilityResources.axaml',
                 'NOTICE.WinUI.md', 'NOTICE.Silverlight.md'):
        source, destination = maintained / name, root / name
        if source.resolve() != destination.resolve():
            copyfile(source, destination)
        if not destination.is_file():
            raise FileNotFoundError(f'Missing maintained supplement: {destination}')

    path = root / 'BrowserPrimitives.axaml'
    path.write_text(path.read_text().replace('</Styles>',
        '  <Style Selector="TextBox:disabled /template/ Border#PART_BorderElement">\n'
        '    <Setter Property="Background" Value="{DynamicResource BrowserInputBrush}" />\n'
        '    <Setter Property="BorderBrush" Value="{DynamicResource BrowserBorderBrush}" />\n'
        '  </Style>\n'
        '  <StyleInclude Source="avares://Avalonia.Themes.Browser/BrowserVisualRefinements.axaml" />\n</Styles>'))
    path = root / 'BrowserTheme.xaml'
    path.write_text(path.read_text().replace(
        '<ResourceInclude Source="avares://Avalonia.Themes.Browser/BrowserTokens.axaml" />',
        '<ResourceInclude Source="avares://Avalonia.Themes.Browser/BrowserTokens.axaml" />\n'
        '  <ResourceInclude Source="avares://Avalonia.Themes.Browser/BrowserCompatibilityResources.axaml" />'))
    path = root / 'Avalonia.Themes.Browser.csproj'
    text = path.read_text().replace('<PackageLicenseExpression>MIT</', '<PackageLicenseExpression>MIT AND MS-PL</')
    text = text.replace('complete MIT Fluent', 'complete Fluent')
    path.write_text(text.replace('README.md;LICENSE.Avalonia.txt;UpstreamManifest.json',
        'README.md;LICENSE.Avalonia.txt;NOTICE.WinUI.md;NOTICE.Silverlight.md;UpstreamManifest.json'))
    path = root / 'README.md'
    text = path.read_text().replace('An independent, MIT-licensed browser-style design system',
                                    'An independent browser-style design system')
    path.write_text(text.replace('MIT license with redistributed source/binaries.',
        'Avalonia MIT, WinUI MIT and per-file Microsoft MS-PL notices with redistributed\n'
        'source/binaries. Package metadata lists `MIT AND MS-PL` to retain the calendar/\n'
        'date/time template notices; the source headers are not removed or relicensed.'))

    manifest['adaptations']['contractRefinements'] = [
        'Neutral tab selection', 'Dropdown checkmarks', 'Compact scroll extents',
        'Native text context select-all', 'Portable primary/ghost/danger buttons',
        'Separate indicator and label states', 'Readable range and calendar surfaces',
        'Disabled input contrast', 'Preserved per-file Microsoft license notices',
    ]
    manifest_path.write_text(json.dumps(manifest, indent=2) + '\n')
    print('Browser refinements applied without dropping upstream keys or named parts.')


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('root', type=Path)
    refine(parser.parse_args().root)
