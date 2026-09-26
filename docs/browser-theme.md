# Browser theme for native Avalonia

`Avalonia.Themes.Browser` is an independent native theme targeting `net8.0`
and Avalonia 11.3.12. It owns the full control-theme source set from
`AvaloniaUI/Avalonia` commit `37fbd9655cc581ff5b1c6b1fb1be4e3118c889d0`, rather
than installing FluentTheme and overriding a handful of selectors. It is an
unofficial package, not an AvaloniaUI release.

## Use in another application

Build the NuGet package and add its output directory as a package source:

```sh
dotnet pack src/Avalonia.Themes.Browser/Avalonia.Themes.Browser.csproj -c Release -o artifacts/packages
dotnet add YourApp.csproj package Avalonia.Themes.Browser --version 11.3.12-preview.1 --source artifacts/packages
```

Replace the application-level FluentTheme/SimpleTheme with a single BrowserTheme:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:browser="using:Avalonia.Themes.Browser">
  <Application.Styles>
    <browser:BrowserTheme Density="Comfortable" />
  </Application.Styles>
</Application>
```

The package depends only on Avalonia. It does not reference Core2D, ProDataGrid,
Dock, AvaloniaEdit, Xaml.Behaviors or Avalonia.Themes.Fluent. The catalog can be
copied outside the repository and compiled against the packed NuGet, as CI does.
No font binaries are included. Hosts may register their chosen fonts and override
`BrowserFontFamily`; otherwise the platform fallback is used.

## Complete source baseline

The pinned import covers all **68 control resource dictionaries**, **100 keyed
ControlThemes** (69 type keys and 31 named component themes), and **212 named
PART entries counted within their original files**. It also includes the source
palette provider, system accent support, invariant strings, base resources and
control-resource vocabulary. Layout-only and self-rendering controls need no
ControlTheme. Some derived controls, such as MaskedTextBox, deliberately reuse
their base control's theme.

Coverage includes menus/flyouts, text and selection handles, buttons and split
buttons, lists/trees/tabs/carousels, numeric inputs and spinners, sliders and
scrolling, expanders/split views, calendars and date/time presenters, progress,
refresh controls, validation, notifications, windows/popups and the managed file
chooser. Original native presenters, template bindings, automation paths and
keyboard/pointer machinery are retained. Native OS file dialogs and window
chrome remain platform-owned; they are not recolored by control templates.

`UpstreamManifest.json` records original hashes, keys and named parts.
`verify_theme.py` fails on missing imports, missing keys/parts, dangling original
Fluent assembly URLs, unexpected package dependencies or bundled font binaries.
The manifest is a source-contract check, not a claim that every native interaction
has been exhaustively tested.

The distribution retains Avalonia and WinUI MIT notices and the Microsoft MS-PL
notice on imported calendar/date/time template sources. Package license metadata
is `MIT AND MS-PL`; source headers are retained. No font binaries are included.

## Browser design system

Light/dark resources provide neutral surfaces, readable text, blue accents,
selection, hover, validation, destructive actions and popover elevation. Adapted
templates reference `Browser*Brush` tokens directly so subtree overrides work.
Fluent-compatible resource aliases remain for third-party templates; changing a
semantic token does not automatically rewrite those compatibility aliases.

`Density` can change on the existing theme instance without rebuilding templates:
Compact uses 28-DIP primary inputs, Comfortable 36, and Touch 44. Density changes
size and spacing resources, not document data. Specialized intrinsic geometry,
such as calendar day layouts and switch tracks, keeps its native relationships;
Touch is not a guarantee that every incidental icon is a 44-DIP target.

Button classes are `primary`, `ghost`, `danger`, plus upstream `accent`.
`Border.browser-card` and `TextBlock.browser-heading` / `browser-muted` are optional
composition styles. Set `RequestedThemeVariant` on an application/window or use
`ThemeVariantScope` for Light/Dark. Override semantic brushes locally, for example:

```xml
<StackPanel.Resources>
  <SolidColorBrush x:Key="BrowserInputBrush" Color="#383040" />
</StackPanel.Resources>
```

Preserved native text features include floating watermarks, password reveal,
clear buttons, left/right content, clipboard actions and composition. Scrollbars
retain native range/viewport, Track and Thumb behavior. Menus and tab controls
retain their placement and selection contracts.

## Core2D migration

Core2D loads BrowserTheme in Compact density. The competing generic Studio input,
menu, tab and surface templates are removed; editor-specific Studio controls and
Dock/ProDataGrid/AvaloniaEdit integrations remain. Native built-ins use the same
package as the independent catalog. Legacy saved `FluentLight` / `FluentDark`
preferences are accepted as aliases, so existing settings still select their
intended light/dark variant. They do not instantiate FluentTheme.

Third-party integrations may still transitively reference Avalonia.Themes.Fluent
or use resource names containing Fluent. That is different from installing
FluentTheme as the application's control-theme provider. Dock and ProDataGrid are
not built-in Avalonia controls, and their separate integrations are retained.

## Validation and maintenance

```sh
python tools/browser-theme/verify_theme.py src/Avalonia.Themes.Browser
dotnet test tests/Avalonia.Themes.Browser.Tests/Avalonia.Themes.Browser.Tests.csproj -c Release
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
```

The independent suite resolves every imported key in both variants and exercises
realized controls, keyboard/pointer input, runtime density, local resources,
popups and screenshots without Core2D styles. The package workflow repeats that
suite against a NuGet-only consumer outside the repository. Core2D's regression
suite validates actual editor integration. Platform CI builds the full solution.
Test counts and actual run status are reported in the PR, not inferred from
source coverage. `BROWSER_THEME_ARTIFACTS` controls standalone PNG output.

For an intentional upstream refresh, use a clean pinned Avalonia checkout and run
`import_upstream.py`, then `skin_browser.py`, then `refine_browser.py`, then
`verify_theme.py`. Import and skin tools are maintainer operations that replace
their generated files; normal builds never download or regenerate templates.
Review the manifest diff, native contracts, package consumer and visual changes.

Browser-like describes a web-application visual language implemented in native
Avalonia, not a DOM engine or pixel identity across Chromium/Safari/Firefox and
all OS font rasterizers. Interactive accessibility, native IME, touch, native file
dialogs and OS window management still require platform-specific validation.
No public package feed is modified by these build/test workflows.
