# Advanced studio controls

This pass extends the existing native Avalonia workspace; it does not embed a web view or change the Core2D document schema.

## Collective selection editing

`StudioSelectionEditor` consumes the document's selected objects and inherited `StudioEditContext.History`. Its disposable `SelectionInspectorViewModel` observes selection members and their current corner points. Replacing the selection, replacing corner objects, removing the view, or changing its history scope rebuilds/disposes the adapter. An observable selection collection is supported as well as the existing replacement-based `SelectedShapes` set.

The selection inspector exposes collective X/Y/W/H, aspect locking, a nine-point resize origin, corner-geometry reflections, and mixed-state fill/stroke/visibility/lock switches. Centered switches indicate mixed values; setting a value applies to every selected object. Changes preserve unrelated state flags.

Collective geometry currently supports rectangles, ellipses, text and images with two distinct, finite, unlocked, non-connector corner points. An unsupported or locked member disables the entire geometric transform; the inspector explains the boundary rather than modifying only a subset. Individual inspectors retain access to other geometry. Reflection changes corner geometry and placement; it does not mirror glyph outlines or image pixels.

Transforms deduplicate shared points, compute and validate every result before mutation, preserve point identity and corner orientation, and add one document-history snapshot. Undo callbacks capture original model objects, not disposable controls or adapters. Undo/redo remains usable after selection changes. Coordinates are limited to +/-1e15. Resizing a completely collapsed collective dimension cannot recover its old relative positions; undo the collapse first. Single-object bounds remain independently expandable.

`StudioAnchorPicker` is a native nine-option selecting control with two-dimensional arrow navigation, Home/End and right-to-left horizontal navigation. The selected origin also applies to the existing single-object `StudioBoundsEditor`.

## Transactional names and stroke patterns

`StudioTextField` separates draft text from committed `Value`. Enter or focus loss commits valid input; Escape restores the model value. An external value/data-context update abandons a stale draft. Native text selection, clipboard operations and IME composition remain owned by Avalonia's text presenter. Behaviors reconnect retained template parts after docking-related reattachment.

`StudioNameField` trims names and accepts non-empty single-line names of up to 256 characters. `StudioDashField` validates invariant dash patterns, offers Solid/Dashed/Dotted/Dash-dot presets, and displays a vector preview of the committed pattern. Custom input supports spaces, commas and semicolons as separators; decimals use a dot to match the existing renderer. Up to 32 non-negative lengths are accepted, each at most 100000, with at least one positive component. Empty input means solid. Invalid drafts do not change the document or preview.

Explicit, non-reflection history adapters record name/dash changes against the bound model and document history. Editors without a history scope still edit their models, but do not create an unrelated undo stack. `StudioStrokePreview` caches its dash pen outside the render loop. The stroke inspector now also exposes the existing dash-scale property.

## Native solid-color workbench

`StudioColorEditor`, `StudioColorPlane`, `StudioColorSlider` and `StudioHexField` provide a compact native color popup. The plane is rendered from hue/white/black brush fills and a selection ring, without a per-pixel bitmap. The hue and alpha ramps use native Slider/Track/Thumb interaction. Keyboard alternatives are available for the plane and every numeric component.

The canonical color remains Avalonia's byte-accurate ARGB value. RGB/HSL/HSB mode changes update presentation only, never round-trip or rewrite the color. A separate floating-point HSV state retains hue while saturation is zero. RGB edits preserve untouched channels; six-digit hex preserves alpha; eight-digit hex explicitly means ARGB, consistent with existing Core2D fields. Opacity edits keep RGB bytes unchanged.

Hex and numeric text are transactional. Color-plane and slider changes remain live, matching the existing color picker: dragging updates the bound color; Escape during a captured plane gesture restores its initial HSV value. Capture loss, detachment, disabling or an external color/selection update stops the gesture. These live color changes do not introduce a new coalesced document undo transaction.

The existing spectrum picker remains accessible in the collapsed Advanced picker section. Color presets remain independent color values, not linked shared styles. Gradient stops, perceptual color spaces, HDR and variable-font editing are not added by this pass.

## Presentation and validation

The new templates use existing light/dark studio tokens for surfaces, borders, text, selected state, hover, validation and focus. Intrinsic color ramps are deliberately theme-independent. Native text inputs, selecting controls and sliders are retained below the custom presentation; platform assistive-technology and IME behavior still need interactive verification.

The added tests cover bounded dash parsing, nine resize origins, shared point identity, atomic selection undo/redo, mixed flags, lock/connector/unsupported-member rejection, lifetime cleanup, name/dash commits, retained-template reattachment, real docked multi-selection/history, color model switching, alpha/hex semantics, native color input, and rendered galleries/workspaces in both themes.

Run from a recursive checkout with .NET 10:

```sh
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Set `CORE2D_UI_ARTIFACTS` to retain actual Skia-rendered screenshots. The existing CI matrix builds the complete solution on Windows, macOS and Linux. Successful targeted tests do not replace full-solution validation.
