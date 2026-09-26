#!/usr/bin/env python3
"""Check checked-in template coverage and independent binary package contents."""
from __future__ import annotations
import argparse, json, re, zipfile
from pathlib import Path
from xml.dom import minidom as D

def verify(root: Path, package: Path | None = None) -> dict:
    manifest = json.loads((root/'UpstreamManifest.json').read_text())
    dictionaries = keys = parts = 0
    for entry in manifest['files']:
        output = root/entry['output']
        assert output.is_file(), f'Missing source: {output}'
        if not entry['source'].startswith('Controls/') or output.suffix != '.xaml': continue
        doc = D.parse(str(output)); dictionaries += int(doc.documentElement.tagName == 'ResourceDictionary')
        text = output.read_text()
        actual = {n.getAttribute('x:Key') for n in doc.getElementsByTagName('ControlTheme')}
        for key in entry['themeKeys']:
            assert key in actual, f'Dropped theme {key}: {output}'
            keys += 1
        actual_parts = set(re.findall(r'(?:x:)?Name="(PART_[^"]+)"', text))
        for name in entry['templateParts']:
            assert name in actual_parts, f'Dropped template part {name}: {output}'
            parts += 1
        assert 'avares://Avalonia.Themes.Fluent/' not in text
    assert dictionaries == 68, dictionaries
    assert keys == 100, keys
    assert 'PackageReference Include="Avalonia.Themes.Fluent"' not in (root/'Avalonia.Themes.Browser.csproj').read_text()
    if package:
        with zipfile.ZipFile(package) as archive:
            names = archive.namelist()
            assert 'lib/net8.0/Avalonia.Themes.Browser.dll' in names
            assert 'LICENSE.Avalonia.txt' in names
            assert 'NOTICE.WinUI.md' in names
            assert 'NOTICE.Silverlight.md' in names
            assert 'README.md' in names
            assert 'UpstreamManifest.json' in names
            assert not any(n.lower().endswith(('.ttf','.otf','.woff','.woff2')) for n in names)
            assert not any('Avalonia.Themes.Fluent.dll' in n for n in names)
            nuspec = archive.read('Avalonia.Themes.Browser.nuspec').decode('utf-8-sig')
            assert 'dependency id="Avalonia.Themes.Fluent"' not in nuspec
            assert 'dependency id="Core2D"' not in nuspec
            assert 'MIT AND MS-PL' in nuspec
    return {'dictionaries': dictionaries, 'keyedThemes': keys, 'retainedNamedPartsPerFile': parts,
            'upstream': manifest['commit'], 'package': str(package) if package else None}

if __name__ == '__main__':
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('root',type=Path);parser.add_argument('--package',type=Path)
    args=parser.parse_args();print(json.dumps(verify(args.root,args.package),indent=2))
