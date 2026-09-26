# Object state, database tools and shared surfaces

This pass extends the existing native Avalonia studio workspace. It keeps Dock, the document schema, ProDataGrid projections, original commands and editor instances intact.

## Contextual state editing

`StudioShapeStateEditor` presents the ten existing `ShapeStateFlags` through full-row `StudioSettingToggle` controls with contextual descriptions. The right-hand State tab now describes the selected original objects, not the canvas renderer's settings. Renderer preferences remain in their existing tool and in a collapsed Design section.

The UI-independent `ShapeFlagsInspectorViewModel` observes distinct original objects. Mixed values are represented as nullable booleans; assigning null is a no-op. Edits change only the requested bit, preserve unknown bits, and record one before/after history snapshot per change. Already-recorded history references the original models and remains valid after disposing the adapter. The explicit `None` role is bit 64, not a request to clear the mask; connector role combinations are not silently normalized.

The adapter caches a 32-bit population count array. A flag lookup is O(1); an external change updates only changed bit counts. A group edit still necessarily touches each affected object. No timing speedup is claimed without profiling. Empty selections and unchanged values do not create history. The host observes replacement or observable selection membership and disposes obsolete subscriptions.

Multiple-selection Design fields distinguish collective editing from first-object details. The latter now live in an explicitly labelled collapsible section. The pre-existing first-object style editor is retained and clearly labelled; this pass does not implement mixed-style aggregation.

## Native database and hierarchy tools

Records and Columns retain their existing data-grid behavior, filtering, sorting, drag payloads, editing and commands. `StudioTableStatus` adds native-view result counts and selected-row counts. It reads counts from supported collection interfaces and does not walk lazy sources just to manufacture a count. Collection updates are coalesced with generation-checked callbacks so old targets cannot overwrite current status. Database names use the existing transactional name control; empty data-source and record selections have guidance instead of blank detail panels.

`StudioFindBar` uses Object Browser's existing ProDataGrid search model. Queries are literal case-insensitive Contains terms over visible columns; all query terms must match. Enter/Shift+Enter navigate, Escape clears, and explicit expand/collapse controls operate on the existing hierarchy. Search describes the currently expanded projection; it does not silently scan or expand collapsed branches, filter rows, reorder models or build a second hierarchy.

The bar temporarily owns a search descriptor, remembers the previous descriptors and restores them only while it still owns the current query. An external search client supersedes that ownership without being overwritten on detach. Navigation temporarily enables native selection-on-navigate and restores that setting. A single-match search is explicitly revealed even when the native current index cannot change.

Named Object Browser rows reuse the existing inline-rename hierarchy row. Visibility and lock actions appear only when those capabilities exist on the source model; databases and shared styles do not acquire invented visibility properties. Existing export context commands and type-based diagnostic rows are retained.

## Shared presentation

`StudioReadout` separates selectable, read-only diagnostics from editable fields. It is used for viewport transforms, version information and the active renderer mask. `StudioNotice` supplies inline contextual guidance. Viewport settings retain all existing navigation commands and text-based unbounded limit editing.

`StudioSurfaceChrome.axaml` supplies compact neutral ComboBoxItem checkmarks, dark wrapping tooltips, context-menu surfaces and native ScrollBar templates. The scrollbars retain ScrollBar/Track/Thumb and named page-button contracts rather than implementing a parallel scroll engine. Dropdowns now render their existing PlaceholderText when nothing is selected. Shared brushes remain theme-aware, and these resources reach libraries, inspectors, popups, source editing and data grids through the application theme.

## Validation and scope

Headless tests exercise every state bit, unknown-bit preservation, mixed state, undo after disposal, collection replacement, the actual docked State tab, view counts, query ownership, records/column filtering, hierarchy navigation, native scrollbar dragging/range/viewport synchronization, placeholders and shared-surface rendering in both themes. Earlier tests and full-solution CI are retained.

```sh
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Use .NET 10 and a recursive checkout. `CORE2D_UI_ARTIFACTS` enables rendered PNG captures. The CI matrix validates compilation on Windows, macOS and Linux; headless control tests run with Skia on Linux. Native OS accessibility, IME, touch, file pickers, drag/drop and floating-window behavior still need interactive testing. This is a broader Figma-inspired presentation and workflow refinement, not complete Figma product or pixel parity. Existing guide persistence/snapping, gradient, auto-layout and collaboration boundaries remain unchanged.

## Database cell binding repair

Rendered database captures exposed cells with accessor metadata but no native display/edit binding. Both record and schema grids now provide explicit compiled paths in addition to their existing typed sorting/filtering/search accessors. Record paths follow the original `Values` collection, indexed `ValueViewModel`, and `Content` property; owner captions follow `Owner.Name`. No expression compilation or reflected property lookup is introduced.

The paths observe external value edits, collection replacement and owner changes. Absent schema values remain absent, and null content is not normalized by inspection. Native grid editing still writes the original fields, using the pre-existing database editing/history semantics rather than a new transaction model. Regression tests assert actual visible cell text, native edit commits, stale-source isolation and docking reattachment, not merely successful filtering.

Descriptive setting toggles retain Avalonia's native behavior: activating a mixed value clears it; activating false sets it. The mixed display is not an additional value written into document flags.
