# Nested object and geometry inspectors

This pass extends the native Avalonia studio presentation to document pages, layer contents, block children/connectors, block instances and vector-path details. It builds on the preserved library, export-dialog, Quick Actions, data-table and searchable-settings work described in `studio-secondary-views.md` and `studio-workbench.md`. The Dock layout and document schema are unchanged.

## Collection navigation

`StudioCollectionInspector` composes the existing ProDataGrid-backed `StudioAssetBrowser` with selected-object Design and Data details. The browser has bounded height, name search, sort and filter support; it never changes the source collection order or substitutes copied models. Rows carry the original object as their existing drag context. `StudioObjectSummary` shows a live name and explicit kind caption without reflection.

A selected model is presented through Core2D's existing view locator. Clearing selection, replacing the collection, or removing the selected member clears the detail selection. Switching objects resets the detail tab to Design. The Data tab is available only for models implementing the existing data-object contract. Names and object details remain bound to original instances. Nested collections are opened by explicit selection; this is not an eager recursive rendering of an entire drawing.

The selection/lifetime behavior reconnects retained template parts after docking-related detachment. Collection mutations are observed where the source implements `INotifyCollectionChanged`; Core2D's immutable collections are observed through their bound replacement values.

## Coordinate and dimension editing

`StudioCoordinateEditor` adapts existing point X/Y, path-size W/H and template/page-size W/H through `CoordinatePairInspectorViewModel`. It reuses the transactional `StudioNumericField` draft, expression, validation and scrub contracts. Editing a point retains its identity, including shared references. Locked points and non-finite/out-of-range pairs are not edited.

Dimensions optionally preserve their current aspect ratio. A swap command exchanges width and height; it does not rotate artwork. Zero-sized dimensions remain expandable without division by zero. Both destination values are validated before mutation, and a successful edit records one before/after history snapshot. Undo callbacks capture the source model, not the disposable inspector. Replacing the source or detaching the view rejects stale drafts and disposes subscriptions. Coordinates are bounded to +/-1e15 and dimensions to 0..1e15.

The adapter supports `PointShapeViewModel`, `PathSizeViewModel` and `TemplateContainerViewModel`; it does not manufacture a general transform or layout model for other object types. Linking dimensions is inspector-session state. Imported invalid coordinates are displayed as unavailable rather than silently repaired.

## Block data

The old independent lists of child-property names and values are replaced by `StudioDataTable` in child-property mode, with aligned Object/Name/Value columns. The mode enumerates distinct direct child shapes only, not recursive descendants, and keeps each property paired with its original owner. Duplicate references to a child are projected once without modifying the source collection. The block's own properties remain available through the normal property table.

Filtering can match owner name, field name or value. Owner renames update the projected labels; child replacement and property-collection replacement invalidate obsolete row adapters. Value/name edits and supported property removal use existing document history. Null values are preserved by the existing data-field contract rather than converted on inspection. The default table mode is unchanged.

## Migrated areas

- Document/page and layer contents use searchable collection/detail navigation.
- Blocks expose child objects and connectors through the shared inspector, plus an optional paired child-property table.
- Block instances retain the original block selector, edit command and drop handler; instance position uses transactional coordinates.
- Image geometry uses the bounds editor while retaining the image-key field and fill/stroke state.
- Lines, wires, arcs, cubic/quadratic curves and their segment views use labeled compact coordinate groups. All original control points remain editable.
- Path figures expose their start point, closed state and searchable segments. The existing path hierarchy/model behavior is retained, with compact live object summaries.
- Template dimensions use linked/swap controls while preserving background, grid and data settings.

The views remain passive compiled XAML. Controls, themes, reactive adapters and input/lifetime behaviors are separate. Existing asset commands, exporter implementations, hierarchy relationships, file formats and canvas behavior are not rewritten.

## Validation and boundaries

The new tests cover source identity, selection invalidation, clear-selection and retained-template reattachment, search/sort, paired child-property rows, owner-aware search, nulls, obsolete adapters, transactional coordinates, linked dimensions, locked/invalid pairs, zero dimensions and undo after disposal. Real populated block/layer/document/path editors are captured in both themes at a narrow inspector width.

```sh
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Use .NET 10 and a recursive checkout. Set `CORE2D_UI_ARTIFACTS` for rendered PNGs. Existing headless and three-platform full-solution workflows remain enabled. This extends Figma-inspired visual consistency; it is not complete Figma product or pixel parity. Native assistive-technology, IME, touch, drag/drop and floating-window workflows still need interactive platform validation. Existing guide persistence/snapping, gradient, auto-layout and collaboration boundaries remain unchanged.
