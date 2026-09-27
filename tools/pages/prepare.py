"""Stage the complete .NET publish output, with repository-relative URLs and provenance."""
from __future__ import annotations

import hashlib
import json
import os
from pathlib import Path
import re
import shutil


def main() -> None:
    root = Path('src/Core2D.Browser/bin/Release')
    candidates = [p.parent for p in root.rglob('index.html')
                  if 'publish' in p.parts and (p.parent / '_framework/dotnet.js').is_file()]
    if len(candidates) != 1:
        raise RuntimeError(f'Expected one complete published web root, found {candidates}')
    source = candidates[0]
    target = Path('artifacts/site')
    if target.exists():
        shutil.rmtree(target)
    shutil.copytree(source, target)
    repository = os.environ.get('GITHUB_REPOSITORY', 'wieslawsoltes/Core2D')
    commit = os.environ['GITHUB_SHA']
    if not re.fullmatch('[0-9a-f]{40}', commit):
        raise ValueError('A full source commit is required for publication')
    base = '/' + repository.split('/')[1] + '/'
    index = target / 'index.html'
    html = index.read_text(encoding='utf-8')
    html, count = re.subn(r'<base\s+href="[^"]*"\s*/?>', f'<base href="{base}">', html)
    if count != 1:
        raise RuntimeError('The browser entry point must contain exactly one base element')
    html = html.replace('CORE2D_SOURCE_COMMIT', commit)
    html = html.replace('./main.js"', f'./main.js?v={commit}"').replace('./app.css"', f'./app.css?v={commit}"')
    index.write_text(html, encoding='utf-8')
    shutil.copyfile(index, target / '404.html')
    (target / '.nojekyll').touch()
    files = []
    for path in sorted(target.rglob('*')):
        if path.is_file():
            data = path.read_bytes()
            files.append({'path': path.relative_to(target).as_posix(), 'bytes': len(data),
                          'sha256': hashlib.sha256(data).hexdigest()})
    if not any(item['path'].endswith('.wasm') for item in files):
        raise RuntimeError('The published application contains no WebAssembly payload')
    manifest = {'repository': repository, 'commit': commit, 'basePath': base,
                'runId': os.environ.get('GITHUB_RUN_ID'), 'files': files}
    (target / 'deployment.json').write_text(json.dumps(manifest, indent=2) + '\n', encoding='utf-8')
    print(f'Staged {len(files)} files from {source}: {sum(f["bytes"] for f in files):,} bytes; commit {commit}')


if __name__ == '__main__':
    main()
