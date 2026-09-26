# BrowserTheme integration for Core2D's Dock chrome

This is an application-local token adapter, not a replacement Dock theme or a new package. `DockFluentTheme` still supplies the original control templates, commands, drag areas, drop targets, content recycling and floating/pinned window implementation. `BrowserTheme` remains the application base theme and gains no Dock dependency.

## Version contract

Core2D pins Dock 11.3.11.5. Its NuGet repository metadata identifies source commit `430bd22109660a9a59f8dd5801c95a8b606eb1ae`. That revision predates the newer semantic design-token dictionaries described by current Dock documentation. Declaring only those newer names would silently leave parts of this installed version unchanged.

`Themes/StudioDockTokens.axaml` therefore has two layers: semantic Dock roles mapped to BrowserTheme brushes, and the installed version's actual `DockTheme*`, `DockApplicationAccent*`, `DockToolChromeIconBrush` and `DockWindowChrome*` aliases. `Styles/Dock.axaml` supplies the small property-setter bridge for metrics and states that the pinned templates hard-code. Neither file replaces a `ControlTheme`, `ControlTemplate` or native button template. Document/tool header data templates are Core2D presentation only; the original close and modified-marker templates remain.

## Resource placement and appearance

The token dictionary is merged in `Application.Resources` after the Studio palette. This reaches detached HostWindow, pinned flyouts and drag-preview windows as well as the main DockControl. Aliases resolve separately inside Light and Dark dictionaries; two windows with different requested variants do not acquire one shared wrong-theme brush. Controls consume the aliases with DynamicResource, so changing a requested variant restyles existing controls without rebuilding their templates.

The aliases reuse BrowserTheme brush instances rather than duplicating hexadecimal palettes. One compatibility exception is `DockApplicationAccentBrushIndicator`: the pinned native `SplitterPreviewAdorner` reads this key without a theme variant. It is therefore a root-level brush whose color dynamically follows `BrowserAccentColor` (the same accent in both supplied variants); this also feeds the native drop target. Resource alias creation is lazy but static: replacing a Browser brush object after that alias was resolved is not a live rebinding of the alias. Override the specific Dock brush resource for runtime/local customization. Directly mapped semantic roles and legacy native keys are both documented in the dictionary; for example the pinned drop target consumes `DockApplicationAccentBrushIndicator`.

Tabs and headers use neutral browser surfaces, muted inactive text, selected text and a thin active accent edge. Top/left/right document tabs retain their native layout directions. Selected and inactive tabs reserve the same edge thickness to avoid selection-driven content motion. Tabs and visible tool headers use a compact 32-DIP minimum; document close and chrome buttons have 24-DIP bounds. Width/height constraints on tool content are left to the Dock model.

Native splitter thickness, preview-resize settings and hit areas are unchanged. Idle, hover and drag/preview brushes distinguish resize feedback. Dock target indicators use the browser accent; existing target-selector images are deliberately retained. Floating-window title/client resources and drag-preview palette follow the same browser roles; OS-owned window chrome is not reskinned.

The previous acrylic-era overrides in Common, duplicate Dock background colors in Themes and StudioPalette and competing tab overrides in Studio/StudioInteractionStyles are removed. Native tool-tab placement and suppression of redundant single-tool tab strips are preserved, with the converter owned by the Dock integration styles. This pinned version sets bottom placement locally in its template; a style setter cannot override that local value. The adapter does not introduce a behavior or replace the template merely to move the strip.

## Validation

The dedicated headless tests cover native resource aliases, simultaneous light/dark windows, live switching without template replacement, document tab placement and close wiring, selected-tool activation, original collections/proportions through reattachment, native target colors and local splitter overrides. Populated workspaces expose multiple tool tabs and visible chrome in both themes for screenshot inspection. The existing workspace, canvas, saved-layout and panel-restoration suites remain enabled.

```sh
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
```

Use .NET 10 with recursive submodules. Set `CORE2D_UI_ARTIFACTS` for PNG captures. Headless checks do not establish interactive OS-level floating-window drag/drop or assistive-technology parity. No model schema, Dock dependency version, persisted layout, renderer, exporter or standalone BrowserTheme package is changed.
