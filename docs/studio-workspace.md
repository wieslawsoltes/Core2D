# Studio workspace

Core2D's Figma-inspired presentation layer uses native Avalonia controls and the existing Dock layout. It is not a web overlay or a separate editor.

## Components

`Controls/Studio` provides `StudioIconButton`, `StudioToolButton`, `StudioGlyph`, `StudioToolShelf`, `StudioSurface`, `StudioSection`, `StudioPropertyField`, and `WorkspaceHeader`. Native command, focus, automation, radio-button and expander behavior is retained. Views remain passive; no editor state is duplicated in the controls. `StudioGlyph` tints the alpha mask of shared drawing images with a control-local foreground, avoiding invisible dark-theme icons without mutating shared resources.

`Themes/StudioPalette.axaml` contains light/dark semantic brushes, spacing and elevation. `Styles/StudioControls.axaml` contains control templates. `Styles/Studio.axaml` adapts existing inputs, tables, rulers and docking chrome without replacing Dock's interaction templates. Resource dictionaries are layered with `ResourceInclude`, not flattened with `MergeResourceInclude`, because the studio palette intentionally overrides legacy keys.

## Preserved workflows

The entire original menu remains accessible below the workspace header, including file/import/export operations, scripts, advanced tools, options and panel commands. Undo, redo, save and export bind to existing editor/platform actions. The title follows the project name, including unsaved projects.

All twelve drawing tools and five contextual path subtools remain available. Pages, templates and block documents share a bottom shelf. It reserves its own layout row, so it does not intercept canvas input, and scrolls horizontally in narrow dock panes. Radio groups are local to each shelf, allowing multiple document panes to reflect the selected editor tool.

The Dock factory, sixteen panel IDs, persistence, document models, renderers, import/export code and canvas behaviors are unchanged. Existing layouts are not reset or migrated. Property editors preserve their original two-way bindings and validation. Stroke, fill, typography, rectangle geometry and coordinate editors use compact sections and labelled fields. The empty-selection guidance wraps independently of the property editor's horizontal scroller.

## Validation

Run from a recursive checkout with the .NET 10 SDK:

```sh
git submodule update --init --recursive
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

On Linux, install `libfontconfig1` and `libfreetype6`. Set `CORE2D_UI_ARTIFACTS` to retain Skia-rendered PNGs. The Studio UI validation workflow uploads screenshots, full build logs and TRX reports as `studio-ui-validation`.

The suite exercises real application resources and the docked `MainView` at desktop and compact sizes, both themes, live theme changes, keyboard command activation, section expansion, hosted text editing, header bindings, all drawing-tool bindings, contextual path subtools, narrow-pane scrolling and selection synchronization across duplicated shelves. All sixteen panels are checked in the actual layout tree, not the factory's three locator aliases.

Full-workspace validation also exposed a missing ProDataGrid runtime dependency. Its package reference now excludes only compile assets, preserving the existing explicit assembly reference while allowing FormulaEngine and other runtime dependencies to flow to the application.

Native OS drag-and-drop and floating-window chrome still require platform-specific interactive validation; headless rendering is not a substitute for that coverage.
