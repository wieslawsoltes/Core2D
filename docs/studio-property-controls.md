# Studio property controls

The property-inspector refinement is implemented in native Avalonia 11 controls and XAML. It extends the sidebar workspace described in [studio-workspace.md](studio-workspace.md); it does not embed a browser or replace Core2D's document model.

## Control inventory

| Control | Contract and integration |
| --- | --- |
| `StudioNumericField` | Nullable decimal value, separate text draft, expressions, keyboard stepping, prefix scrubbing, minimum/maximum and validation. Used for bounds, typography, stroke and color opacity. |
| `StudioSegmentedControl` | Native single-selection list with a compact equal-width presentation and enum bindings. Used for horizontal/vertical text alignment and stroke caps. |
| `StudioIconToggle` | Native toggle semantics with either a vector icon or short text. Used for text formatting and aspect-ratio locking. |
| `StudioSwitch` | Compact label-and-switch presentation for native toggle input. Used for snapping, overlays and shape fill/stroke visibility. |
| `StudioSearchBox` | Native text input with a search glyph and clear action. The Layers view retains its existing hierarchy-search behavior. |
| `StudioFontPicker` | Searchable installed font families using native autocomplete. Arbitrary font-family names remain editable for documents opened on other machines. |
| `StudioColorPalette` | Keyboard-accessible color presets. Selecting a preset changes RGB without discarding alpha. |
| `StudioCheckerboard` | Bounded direct rendering for transparency previews, with theme-aware light/dark brushes. |
| `StudioBoundsEditor` | Position and size fields backed by a disposable adapter over existing corner points. |
| `StudioPropertyRow` | Reusable compact label-and-content row. |

`StudioPropertyControls.axaml` and `StudioSearchBox.axaml` define presentation. Input and lifetime subscriptions live in dedicated `Behaviors` classes, not view code-behind. The existing `StudioNumberBox` remains available for legacy text bindings; migration to the transactional numeric control is explicit rather than a blanket replacement.

## Numeric editing

`Value` is the committed nullable decimal value. `DraftText` is the text currently being edited. Incomplete or invalid input never reaches the model binding. Enter or focus loss commits a valid draft; Escape restores the latest committed value. A model-value or data-context change abandons the pending draft so that selection changes cannot write an old edit into a new selection. Read-only fields neither commit nor scrub. A null value presents the mixed-value watermark.

The bounded expression parser is in the UI-independent ViewModels project. It accepts decimal arithmetic, parentheses, unary operators, scientific notation and the current culture's decimal separator. It does not evaluate scripts, arbitrary identifiers, units or method calls. Examples for a starting value of 80:

| Input | Result |
| --- | ---: |
| `(24+8)/2` | 16 |
| `+=8` | 88 |
| `-=8` | 72 |
| `50%` | 40 |
| `+=10%` | 88 |
| `*=50%` | 40 |
| `-8` | -8, subject to the field's bounds |

Input is limited to 256 characters and 32 nested parser levels. Division by zero, overflow, malformed input and out-of-range commits are rejected with a validation message. Percentages in arithmetic, such as `200*50%`, are fractional operands. A lone percentage scales the current committed value.

Up/Down step by `Increment`, Shift multiplies the step by ten and Alt divides it by ten. Prefix scrubbing previews without updating the model and commits once on release. Escape, lost capture, detachment, template replacement, a model/selection change or an editability change cancels the active scrub. Native preedit composition retains ownership of Enter/Escape while composition is active.

## Bounds and history

`BoundsInspectorViewModel` derives from `ReactiveObject`. It reads and writes existing `PointShapeViewModel` instances without replacing point identity. X/Y translate both points. Width/height preserve the original corner orientation and resize about the selected nine-point anchor; the default anchor remains top left. Optional aspect locking uses the current non-zero ratio; zero dimensions remain editable without division by zero.

A bounds change records one before/after snapshot in the document's `IHistory`. Undo callbacks retain the original points, not the disposable inspector adapter, so undo and redo remain valid after selection changes or view detachment. The adapter releases all point subscriptions when detached or replaced. Non-finite, coincident-identity and excessively large coordinates are not edited.

`StudioEditContext.History` is an inherited attached property supplied by the inspector's project binding. It keeps controls independent of a global service locator. Editors instantiated without a document history still work, but do not manufacture an unrelated undo stack.

## Native interaction and theme boundaries

Segmented selection uses Avalonia's selection model. Arrow keys, Home and End skip disabled/hidden options and respect right-to-left flow. Switches and formatting buttons retain native keyboard and toggle semantics. Text inputs and font autocomplete keep the native text presenter, caret, selection and composition machinery. The native solid-color workbench is described in the advanced-controls document; its Advanced picker retains the existing color-picker library.

New semantic tokens cover raised selection surfaces, switch tracks, transparency cells and error borders in both light and dark themes. The compact property templates use shared input, border, foreground, hover and focus tokens; they do not hard-code a separate per-view theme.

This implements the listed controls and workflows, not every Figma product feature. The subsequent [advanced-controls pass](studio-advanced-controls.md) adds multi-selection aggregation, resize anchors, transactional names/dashes and a native solid-color workbench. Gradient-stop editing, variable fonts, collaborative comments and prototype execution remain outside these passes. Native-platform touch, IME and assistive-technology behavior still require interactive validation in addition to headless tests.

## Validation

The existing `Studio UI validation` workflow builds the real application and runs `Core2D.UI.Tests`. The added cases exercise expression rejection and culture handling, atomic numeric commits, scrubbing during selection changes, bound enum selection, native keyboard toggles, alpha preservation, search clearing, bounds lifetime and undo/redo. The typography gallery uses real editor/model pairs and renders in both themes. Existing workspace, saved-layout migration, Assets, panel restoration and editor-catalog cases remain enabled.

Run from a recursive checkout with .NET 10:

```sh
git submodule update --init --recursive
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Set `CORE2D_UI_ARTIFACTS` to retain PNG captures. On Linux install `libfontconfig1` and `libfreetype6`. Workflow artifacts include screenshots, TRX reports and full build output. The separate CI matrix builds the complete solution on Linux, Windows and macOS, with the WebAssembly workload installed; passing a targeted UI build does not by itself establish full-solution success.
