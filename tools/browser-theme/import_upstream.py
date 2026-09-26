#!/usr/bin/env python3
"""Import the complete MIT Fluent template set from a pinned Avalonia checkout.

No package build downloads source. This maintainer tool generates checked-in files.
A SHA manifest records the original files, control keys and template-part names.
"""
from __future__ import annotations
import argparse
import hashlib
import json
import re
import subprocess
from pathlib import Path

UPSTREAM_SHA = '37fbd9655cc581ff5b1c6b1fb1be4e3118c889d0'
SOURCE_NAME = 'Avalonia.Themes.Fluent'
TARGET_NAME = 'Avalonia.Themes.Browser'


def import_theme(checkout: Path, destination: Path) -> None:
    actual = subprocess.check_output(['git', '-C', str(checkout), 'rev-parse', 'HEAD'], text=True).strip()
    if actual != UPSTREAM_SHA:
        raise ValueError(f'Expected Avalonia {UPSTREAM_SHA}, got {actual}; review the port before updating.')
    source = checkout / 'src' / SOURCE_NAME
    destination.mkdir(parents=True, exist_ok=True)
    manifest = {'repository': 'AvaloniaUI/Avalonia', 'tag': '11.3.12', 'commit': actual, 'files': []}
    for path in sorted(source.rglob('*')):
        if not path.is_file() or path.suffix not in ('.cs', '.xaml'):
            continue
        relative = path.relative_to(source)
        if str(relative) == 'Properties/AssemblyInfo.cs':
            continue
        raw = path.read_bytes()
        text = raw.decode('utf-8-sig')
        original_keys = re.findall(r'<ControlTheme\s[^>]*x:Key="([^"]+)"', text)
        original_parts = sorted(set(re.findall(r'(?:x:)?Name="(PART_[^"]+)"', text)))
        text = text.replace(SOURCE_NAME, TARGET_NAME).replace('FluentTheme', 'BrowserTheme')
        text = text.replace('FluentControls.xaml', 'BrowserControls.xaml')
        name = str(relative).replace('FluentTheme', 'BrowserTheme').replace('FluentControls', 'BrowserControls')
        output = destination / name
        output.parent.mkdir(parents=True, exist_ok=True)
        if path.suffix == '.xaml':
            # Infer a template's source type explicitly; retain upstream typed DataTemplates.
            text = re.sub(r'(<ControlTheme\b[^>]*TargetType="([^"]+)"[^>]*)(>)',
                lambda m: m.group(1) + ('' if 'x:DataType=' in m.group(1) else f' x:DataType="{m.group(2)}"') + m.group(3), text)
            text = re.sub(r'Source="/(Accents|Strings|DensityStyles|Controls)/',
                          'Source="avares://' + TARGET_NAME + r'/\1/', text)
            text = '<!-- Derived from AvaloniaUI/Avalonia 11.3.12 (MIT). See LICENSE.Avalonia.txt and UpstreamManifest.json. -->\n' + text
        else:
            text = '// Derived from AvaloniaUI/Avalonia 11.3.12 (MIT); see LICENSE.Avalonia.txt.\n' + text
        output.write_text(text, encoding='utf-8')
        manifest['files'].append({'source': str(relative), 'output': name, 'sha256': hashlib.sha256(raw).hexdigest(),
                                  'themeKeys': original_keys, 'templateParts': original_parts})
    licence = checkout / 'licence.md'
    (destination / 'LICENSE.Avalonia.txt').write_bytes(licence.read_bytes())
    (destination / 'UpstreamManifest.json').write_text(json.dumps(manifest, indent=2) + '\n')
    print(f'Imported {len(manifest["files"])} source files from Avalonia {actual}.')

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('checkout', type=Path)
    parser.add_argument('destination', type=Path)
    options = parser.parse_args()
    import_theme(options.checkout, options.destination)
