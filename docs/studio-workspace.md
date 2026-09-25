# Studio workspace

Core2D's Figma-inspired presentation layer is implemented with native Avalonia controls and the existing Dock layout. It is not a web overlay, a separate editor, or an implementation of Figma's file format or collaboration service.

## Components

`Controls/Studio` provides `StudioIconButton`, `StudioToolButton`, `StudioToolShelf`, `StudioSurface`, `StudioSection`, `StudioPropertyField`, and `WorkspaceHeader`. The controls use standard Avalonia command, focus, automation, radio-button and expander behavior. Views remain passive; no editor state is duplicated in the controls.

`Themes/StudioPalette.axaml` contains light/dark semantic brushes, spacing and elevation. `Styles/StudioControls.axaml` contains the control themes. `Styles/Studio.axaml` adapts existing inputs, tables and docking chrome without replacing Dock's interaction templates. Theme dictionaries are layered with `ResourceInclude`, not flattened with `MergeResourceInclude`, because the studio palette intentionally overrides legacy keys.

## Preserved workflows

The original menu remains accessible below the workspace header, including file/import/export operations, scripts, advanced tools, options and panel commands. Undo, redo, save and export in the header bind to the existing editor/platform actions.

All twelve drawing tools and five contextual path subtools remain available. Pages, templates and block documents use the same bottom shelf. The shelf reserves its own layout row, so it does not intercept canvas input, and scrolls horizontally in narrow dock panes. Radio groups are local to each shelf, allowing multiple document panes to reflect the same selected editor tool.

The Dock factory, dockable IDs, persistence, document models, renderers, import/export code and canvas behaviors are unchanged. Existing layouts are not reset or migrated. Property editors preserve their original two-way bindings and validation. Stroke, fill, typography, rectangle geometry and coordinate editors use compact sections and labelled fields.

## Validation

Run from a recursive checkout with the .NET 10 SDK:

```sh
git submodule update --init --recursive
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

On Linux, install `libfontconfig1` and `libfreetype6`. Set `CORE2D_UI_ARTIFACTS` to an output directory to retain Skia-rendered PNGs. The Studio UI validation workflow uploads screenshots and TRX reports as `studio-ui-validation`.

The headless suite exercises real application resources and the docked `MainView` at desktop and compact sizes, both theme variants, native keyboard command activation, hosted text editing, all drawing-tool bindings, path subtools, narrow-pane scrolling and selection synchronization across duplicated tool shelves. Native OS drag-and-drop and floating-window chrome still require platform-specific interactive validation; headless rendering is not a substitute for that coverage.
