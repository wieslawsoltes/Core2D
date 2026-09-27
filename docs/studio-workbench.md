# Studio workbench: actions, settings and data

This pass extends the secondary-view redesign in [studio-secondary-views.md](studio-secondary-views.md). The application stays native Avalonia, retains Dock and its saved layout, and keeps ProDataGrid as its table/list implementation.

## Quick Actions

The workspace header's Actions button and primary-modifier + K open a centered, size-constrained `StudioCommandPalette`. `StudioActionsView` defines its catalog in compiled XAML against `ProjectEditorViewModel`. It reuses the actual file, edit, project, shape, view and drawing-tool commands and parameters, rather than introducing a second command implementation. Existing menus remain available.

The result list is a ProDataGrid with explicit typed accessors and compiled templates. Query terms match action titles and category paths. Arrow keys navigate available actions, Enter executes the selected action, and Escape, the close action or a backdrop click dismisses the palette. Disabled commands remain visible but cannot execute; the command is rechecked immediately before invocation. Opening, searching and navigating never invoke an action.

The last eight action IDs are kept for ordering within the current control's lifetime; this history does not retain document objects or get saved to disk. Catalog subscriptions are released when closed or detached and recreated against the current data context when opened. Command availability notifications are marshalled to the UI thread. The previous valid focus target is restored before invocation or dismissal. Native preedit composition retains confirmation keys. Opening is disabled while another application dialog is active.

This is a catalog of existing static menu actions, not a plugin marketplace or a dynamically reflected command registry. It does not implement fuzzy search or create missing Figma product features. Catalog traversal is bounded, and new actions should be declared with compiled bindings.

## Paired data fields

`StudioDataTable` replaces the separate name and value lists in the record and custom-property inspectors. Both columns now share one row object and one scroll position. Sorting or filtering cannot independently rearrange a name and its value. The original record values, database columns and property objects are not cloned or reordered.

`DataFieldsViewModel` pairs records by their original schema index. Extra values are preserved with an explicit unmatched-column warning and a fallback label; missing values produce read-only value cells. No value objects or schema columns are silently fabricated. Replacing the source, owner, values or column collection rebuilds the projection and invalidates the old adapters.

`DataFieldRowViewModel` forwards writes to original models, making one document-history snapshot per changed name or value. Undo callbacks capture those models, not a disposable row or view. Disposed rows reject stale writes. Custom-property removal still uses the original owner's command and original property argument. A record field-name edit updates the existing database column, as the previous column/name inspector did; it is a schema-wide name, not a record-local alias.

The grid enables explicit sorting, searching and filtering models. The toolbar searches both names and values and chooses original/A-Z/Z-A order; projection changes do not mutate the document collection. Row-change refresh is deferred until the current edit callback finishes. Data contexts and history are supplied through bindings and the inherited `StudioEditContext.History` property.

`StudioDataValueField` retains the separate draft/commit contract of `StudioTextField` without trimming or imposing its single-line name normalization on stored values. A no-op edit preserves null rather than converting it to an empty string. Existing imported text is not truncated. Numeric expressions and schema-type conversion are not applied to arbitrary data strings.

The Data inspector uses a bounded table rather than putting the table inside a second scrolling viewport. Database record and column toolbars now give search its own full-width row, leaving existing add/remove/apply commands and grid infrastructure intact.

## Searchable settings

`StudioSettingsPanel` and `StudioSettingsSection` provide a bounded settings viewport with an explicit keyword search. Every query term must match a group heading or its declared keywords. Search only controls a group visibility flag; it does not modify editor values, rewrite data contexts or reset expansion state. Clearing the query restores the groups, and detachment releases subscriptions and restores their match flags.

Drawing defaults and renderer/state inspectors adopt the control. Advanced renderer groups start collapsed instead of eagerly showing all nested editors. Renderer cache inspection binds to the actual cache rather than passing a cache object to the project Images view.

## Shortcut ownership

`StudioScopedKeyBindingsBehavior` can now opt into unhandled primary-modifier file shortcuts from text editors. This is enabled at the application root so Save/Open still work after native editing has had priority. The action palette itself consumes document shortcuts within its scope. The database grids use the default stricter scope: Delete inside an editor edits text instead of deleting the record or column.

## Validation and boundaries

Headless tests cover actual application command bindings, native keyboard input, disabled commands, focus restoration, compact and theme-aware rendering, record pairing, search and sort, schema mismatches, null/raw values, history after disposal, source replacement and settings filtering. The existing library, script, dialog, export, canvas, saved-layout and control tests remain enabled.

```sh
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Use a recursive checkout with .NET 10 and the existing native rendering dependencies. Set `CORE2D_UI_ARTIFACTS` to save actual Skia-rendered screenshots. The separate OS matrix builds the full solution; this does not establish interactive touch/IME/accessibility or native window behavior on each operating system. Existing file formats and export engines are retained, not exhaustively revalidated by a visual redesign.
