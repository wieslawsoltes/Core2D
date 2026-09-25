# Studio workspace

Core2D's native Avalonia workspace uses a Figma-inspired sidebar-and-canvas layout. The editor, rendering engines, document models, import/export services and existing commands remain the source of application behavior.

## Workspace composition

The default home perspective has a full-height **Layers / Assets** sidebar, the central document canvas, and a single **Design / Data / State** inspector. The document title and main menu live in the navigation sidebar. All twelve drawing tools and five contextual path tools remain in the floating bottom shelf. Zoom out, fit, zoom in and reset are canvas-local actions. Overlays occupy only their actual control bounds; the remainder of the canvas receives drawing input normally. Narrow shelves scroll horizontally.

The design inspector composes existing view models for geometry, fill, stroke, typography, page layout, snapping, overlays and export. It does not copy editable properties into another model. Alignment actions call the existing shape service. Advanced stroke details, arrow editors, shared styles and less-frequently-used settings are collapsible. Assets retain the original blocks, styles, templates, images, databases and scripts views.

Layer search uses the existing ProDataGrid search model and searches its expanded/flattened hierarchy. Enter or F3 navigates forward, Shift navigates backward, and Escape clears the query. It does not claim to recursively search collapsed branches.

The project home screen retains the original New, Open and file-drop actions. No synthetic recent-file entries or nonfunctional collaboration actions are presented.

## Dock layout compatibility

`StudioWorkspaceLayout` adapts legacy four-pane home layouts during factory initialization. It preserves document subtrees and their split structure, moves legacy tools into recoverable sidebar locations, retains pinned collections, and avoids recreating floating document windows. The redundant global menu/status dockables are replaced by the sidebar header, canvas controls and inspector settings.

`StudioDockGraph` traverses visible, hidden, pinned and floating dockables without following cyclic owner references. It registers stable tool IDs in the factory's locator on each initialization, allowing existing panel menu commands to restore tools hidden in nested root docks. Previously hidden legacy tools receive a live restoration owner rather than retaining a reference to a removed pane.

The `StudioNavigator` and `StudioInspector` dockables act as migration markers. They are detected even when pinned, hidden or floated. Subsequent initialization and saved-layout loading retain user-customized proportions and locations rather than resetting the workspace. All sixteen original tool panels remain available through the existing menu. Single-tab tool strips are hidden; restoring an advanced tool reveals the sidebar tab strip.

## Controls and themes

`Controls/Studio` contains the reusable surfaces, icon and tool buttons, tinted image glyphs, collapsible sections, property fields, numeric fields, color fields, arrow editors, alignment bar, canvas controls and workspace header. View code-behind only initializes declarative markup. Interaction behavior lives in `Behaviors`.

The theme dictionaries define original compact templates for buttons, text inputs, check boxes, toggle/radio buttons, combo boxes, tabs, list items, menu items, numeric spinners and color fields. Complex infrastructure such as ProDataGrid virtualization, Dock drag/drop handling, scroll viewers and the color picker's spectrum implementation remains supplied by the existing libraries; it is not reimplemented or replaced with decorative mock controls.

The custom text editor template keeps Avalonia's native text presenter, selection, caret, composition, password, wrapping and validation contracts. Context actions retain cut, copy, paste and select-all. Menus retain their original commands, checked states, shortcuts and submenus. A leaf command dismisses the owning main-menu flyout after command dispatch.

`StudioNumberBox` preserves the existing `TextBox.Text` bindings and numeric conversion. Up/Down step by one, Shift by ten, and Alt by one tenth. Prefix scrubbing previews a value and commits once on release, avoiding a history entry per pointer movement. Escape, lost capture, template replacement and detachment cancel the scrub without changing the model. Read-only fields do not step or scrub.

`StudioColorField` exposes one two-way color value, RGB hex and alpha percentage. Six hexadecimal digits preserve alpha; eight explicitly specify ARGB. Invalid values are rejected through binding validation. The swatch opens the existing spectrum picker. `StudioGlyph` tints an image's alpha mask with the local foreground without modifying shared drawing resources.

Semantic palette tokens live in `Themes/StudioPalette.axaml`. Dictionaries are layered with `ResourceInclude`, because the studio palette intentionally overrides legacy keys. Existing application keyboard shortcuts remain unchanged; familiar Figma positioning does not silently remap Core2D's commands.

## Validation

Run from a recursive checkout with the .NET 10 SDK:

```sh
git submodule update --init --recursive
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Linux rendering requires `libfontconfig1` and `libfreetype6`. Set `CORE2D_UI_ARTIFACTS` to retain real Skia-rendered screenshots. The read-only Studio UI workflow uploads PNGs, complete build logs and TRX reports.

Coverage includes the actual docked workspace, both themes, compact windows, native keyboard command activation, drawing/path tools, duplicate tool shelves, numeric binding preservation and scrub cancellation, color validation, popups, layer search, asset tabs, legacy panel restoration, pinned-layout reinitialization and saved-layout round trips. The editor catalog constructs 32 real view/model pairs in each theme on the Avalonia test thread (64 editor instantiations, reported as two catalog test cases). Populated drawing and project-home screenshots complement empty-workspace and control-gallery captures.

Headless coverage is not an exhaustive end-to-end test of every import/export format. Native OS drag-and-drop, platform floating-window decorations, touch/IME integration and screen-reader behavior still require interactive platform validation. This presentation refactor does not add Figma's collaboration service, prototype runtime or file-format compatibility, and is not a pixel-identical reproduction of every Figma product screen.
