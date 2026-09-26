# Avalonia.Themes.Browser

An independent browser-style design system for native Avalonia 11.3.12.
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
Avalonia MIT, WinUI MIT and per-file Microsoft MS-PL notices with redistributed
source/binaries. Package metadata lists `MIT AND MS-PL` to retain the calendar/
date/time template notices; the source headers are not removed or relicensed.
No public NuGet publish occurs as part of a build.

Browser-style means the cross-platform web-app design language, not a DOM engine
or a claim that OS pickers and text rasterization match every browser. Native
input/automation/IME behaviors remain supplied by Avalonia. Preview maturity:
headless coverage and platform build status are reported in the PR; interactive
screen-reader, touch, native file-dialog and OS window chrome validation remain
separate requirements.
