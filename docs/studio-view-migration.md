# Studio view migration

The first presentation pass migrated the static tab-based inspectors below to collapsible sections, labelled fields and numeric controls. That pass checked preservation of the multiset of original binding expressions. The subsequent property-control pass intentionally replaces some direct bindings with transactional numeric values and the bounds adapter; the original model properties and editing operations remain available through those controls.

## Migrated editors

- Shapes: arc, block, cubic and quadratic Bezier, ellipse, image, insert, line, path, point, rectangle, text and wire.
- Path geometry: figures, sizes, arc segments, cubic and quadratic Bezier segments and line segments.
- Styles: fill, stroke, text and shared shape styles.
- Containers: document, layer, options, page and template.
- Renderer settings: grid and shape renderer state.

## Property-control refinement

Rectangle, ellipse and text editors now present position and size instead of separate corner sections. `StudioBoundsEditor` retains both existing corner objects, their orientation and document-scoped undo history. Shape fill/stroke visibility uses compact switches. The text editor retains text content and the existing text-binding action.

Typography uses the searchable font picker, transactional size field, formatting toggles and horizontal/vertical alignment segments. Stroke editing uses transactional thickness/dash-offset fields and a cap selector. Color editing retains the spectrum picker and adds transparency previews, an opacity field and alpha-preserving presets.

Options now uses shared property rows, paired snap-spacing fields and compact switches. Layers search uses the custom search field while retaining the original ProDataGrid search behavior. Advanced details and original data/state views remain accessible.

See [studio-property-controls.md](studio-property-controls.md) for control contracts, expression syntax, lifecycle and history behavior, theme tokens, validation commands and explicit feature boundaries.
