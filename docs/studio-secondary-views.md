# Secondary studio views

This pass extends the native Avalonia presentation system beyond the canvas and inspectors. It does not embed a web view, replace Dock, or change the document format.

## Shared controls

`StudioPanelHeader`, `StudioToolbar`, `StudioEmptyState`, `StudioStatusBadge`, `StudioAssetItem`, `StudioAssetBrowser`, `StudioCodeEditor`, and `StudioStepIndicator` provide the shared visual structure. Their templates live in `Styles/StudioSecondary.axaml` and use the existing light/dark studio resources. Views remain declarative; subscriptions, input routing, focus, and native editor integration live in behaviors.

## Libraries

Blocks, styles, templates, scripts, and image keys use a ProDataGrid-backed asset browser. It exposes a name query, original/A-Z/Z-A order, compact or detailed rows, and an empty state with a result count. Filtering and sorting project the original item instances rather than copying models or reordering the source collection. A filtered-out item remains the document's selection; clearing the query restores its visible selection. Selecting another result updates the existing model binding.

Explicit column accessors supply filtering, sorting, and search text. The browser observes source changes and relevant name/key changes and releases those subscriptions on replacement or detachment. Block rows retain actual block previews; other asset rows show their kind icon and metadata. This is a row-based browser, not a tiled-grid implementation or thumbnail renderer for every asset type.

Original library commands, context menus, model selection, and supported drag/drop operations remain wired. Name fields commit against the original model and inherited document history. Scripts are never executed by browsing or double-clicking; Run/Execute/REPL remain explicit existing actions. Image keys remain read-only identifiers with their original add/remove commands.

Database, record, data-object, object-browser, state, image-options, and zoom-options views receive matching headers, spacing, search presentation, switches, and surfaces. Their existing data adapters and advanced functions remain in place.

## Native source editing and text bindings

`StudioCodeEditor` hosts AvaloniaEdit with TextMate grammars, line numbers, native text selection and undo, find/replace infrastructure, caret position, and a wrap toggle. The pinned AvaloniaEdit 11.3.0 integration is compatible with the app's Avalonia 11 line. Source text is synchronized with the existing script model without evaluating it. Replacing the source or data context clears the local editor undo stack so edits cannot be undone into another script. Detachment disposes search/grammar subscriptions; reattachment does not reuse a disposed coloring transformer.

The text-binding dialog now has Columns/Page properties/Shape properties tabs with searchable original-model lists. Insert actions still invoke the original token-building methods, and the editable expression remains bound to the original text shape. Plain-text expressions use the same native editor without treating binding tokens as executable C#.

Native text input takes priority over ancestor drawing shortcuts, including the AvaloniaEdit text area. The existing explicit script execution shortcuts are retained.

## Modal presentation

Dialogs use the original presenter and visibility flags with shared surface/elevation resources, a draggable title area, a constrained scrolling body, and a Done action. The surface is bounded by the viewport; larger hosted content scrolls instead of forcing the entire modal outside the window. Keyboard focus enters the dialog, Tab cycles within its surface, and the previous valid focus target is restored when the modal detaches. Unhandled Escape dismisses through the original presenter; Escape consumed by an editing control cancels that draft first.

## Export

The export wizard keeps the existing step models, validation, exporter options, scope hierarchy, destination templates, preview, execution, and navigation commands. Its presentation uses a top step rail, contextual step heading, common input controls, a scrollable body, and a persistent navigation footer. Step indicators are status displays, not clickable shortcuts that bypass validation. The destination template controls now occupy their own grid row instead of overlapping the output-folder row. No new export formats or changes to rendering pipelines are introduced.

## Validation and boundaries

The added tests instantiate the real library and export views, asset projections and model identity, native source editing and grammar lifecycle, text-binding insertion, modal sizing/focus/closing, and both theme variants. Existing canvas, guide, selection, docking, property, and editor-catalog tests remain enabled.

```sh
git submodule update --init --recursive
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Use .NET 10. On Linux install `libfontconfig1` and `libfreetype6`. Set `CORE2D_UI_ARTIFACTS` to retain Skia-rendered captures. Full-solution CI also builds on Windows, macOS, and Linux with `wasm-tools` installed.

This is a Figma-inspired native presentation pass, not complete Figma product or pixel parity. It does not add a marketplace, collaboration, new scripting sandbox, new exporters, persisted ruler guides, or tiled asset virtualization. Native platform IME, screen readers, floating windows, and cross-application drag/drop still require interactive validation beyond headless tests. Existing dependency/compiler warnings are not claimed to be eliminated.
