# Browser deployment

The production application is served at https://wieslawsoltes.github.io/Core2D/ . It is the native Avalonia/.NET WebAssembly application, including the shared BrowserTheme and Core2D Dock adapter, not a separate JavaScript imitation.

## Automated publishing

Every push to `master`, or manual execution of `.github/workflows/pages.yml`, publishes `Core2D.Browser` in Release using .NET 10 and the `wasm-tools` workload. `tools/pages/prepare.py` discovers the SDK's actual publish web root, copies the complete output, sets the `/Core2D/` base URL, retains underscore-prefixed runtime files, and creates a matching 404 fallback and `.nojekyll` marker.

The site includes `deployment.json` with the source commit, workflow run, file sizes and SHA-256 hashes. Bootstrap and stylesheet URLs are commit-versioned. Production serves ordinary files; it does not assume GitHub Pages applies content-encoding headers to custom Brotli files.

The workflow reads the repository's existing Pages configuration without changing it. For Actions-based sites it uses `upload-pages-artifact` and `deploy-pages`. For the pre-existing branch-based site it publishes the identical tested artifact to the configured generated-site branch and requests a Pages build. The branch publisher refuses to overwrite `master`, custom source folders or arbitrary branches. Existing custom-domain files are retained.

## Browser acceptance checks

Before publication and again against the public URL, Chromium checks:

- The manifest and HTML identify the expected source commit.
- The published .NET runtime starts and Avalonia produces a visible canvas.
- Native pointer input activates New drawing and creates a rectangle.
- Native keyboard undo and redo restore the expected document shape count.
- The canvas renders after a compact viewport resize, without failed resource requests or unexpected JavaScript/.NET errors.

`BrowserDiagnostics` exports only read-only readiness and model observations. Tests do not call an editor mutation API: all edits use the same native pointer/keyboard path as the UI. Known unsuccessful GPU-capability probes are retained separately in test reports; supported software rendering is accepted only after the same editing checks pass. This is not a claim of hardware GPU validation on a headless CI runner.

Build and live-test artifacts retain full publish logs, screenshots and JSON reports. A successful build alone does not mark the live verification successful. The live check waits for the expected deployed commit before testing the application.

## Local reproduction

```sh
git submodule update --init --recursive
dotnet workload install wasm-tools
dotnet publish src/Core2D.Browser/Core2D.Browser.csproj -c Release
```

Serve the SDK's published `wwwroot` directory rather than the source `wwwroot`. For the CI staging layout, set `GITHUB_SHA` and `GITHUB_REPOSITORY`, run `tools/pages/prepare.py`, then serve the staged files under `/Core2D/`. Native OS features and platform-specific exporters retain their existing browser limitations; deployment does not turn unsupported native APIs into web APIs.
