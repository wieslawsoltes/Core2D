# Studio rulers, canvas and layer navigation

The canvas remains native Avalonia and uses the existing PanAndZoom and Core2D renderers. The workspace and Dock layout are not replaced by a web view.

## Rulers

The compact 20-DIP rulers use a bounded, integer-indexed 1/2/5 tick scale. Minor ticks are reduced when screen spacing becomes too small. Automatic formatting retains fractional values at high zoom, and vertical labels are rotated so larger coordinates remain readable. A bounded formatted-text cache avoids recreating every label on cursor movement.

`StudioCanvasBehavior` maps the actual `ContainerPanel` transform into the screen-space overlay. This includes layout centering as well as pan and zoom. It updates ruler origins, page ranges, selection ranges and pointer markers from the same mapping. Selection observation is released and rebuilt when the editor/project/selection changes or the view is reattached. `PageView` code-behind now contains only initialization.

## Canvas interaction

Each realized canvas has its own `CanvasNavigationViewModel`; its commands target that canvas rather than shared global platform delegates. The zoom flyout offers an editable percentage, zoom in/out, 100%, fit page, fit selection, ruler visibility, guide visibility/locking and clear guides.

- Wheel: pan. Shift-wheel: horizontal pan.
- Ctrl/Command-wheel: zoom around the pointer.
- Space + left drag: temporarily pan without changing the drawing tool.
- Hand button: persistent hand mode for that viewport.
- Shift+1: fit page. Shift+2: fit selection. Shift+R: toggle rulers and guides.

Canvas shortcuts are scoped to canvas input and do not replace text-editor keys. The original drawing-tool mappings, including H for the path tool, remain available. Native middle-button and gesture support remain supplied by the existing canvas. The coordinate readout does not intercept pointer input.

## Session guides

Drag from the top ruler for a horizontal guide or the left ruler for a vertical guide. Drag a guide to move it; Alt-drag duplicates it. Shift constrains guide placement to 10-unit increments. Release an existing guide outside the viewport to remove it. Selected guides support arrow nudging (Shift for ten units), Delete/Backspace, and Escape to clear selection. Escape cancels an active drag without committing a change.

Guide hit testing uses a fixed screen-space tolerance independent of zoom. A drag modifies only an overlay preview until release; add, move, remove and clear create document-history snapshots with stable guide identities. The overlay does not intercept ordinary drawing hit tests. The guide store rejects non-finite/out-of-range coordinates and is bounded to 512 guides per page.

**Guides are page-local session state, not saved document geometry.** The editor uses weak page keys so reattached or split views of the same page can reuse guides without a global registry. These guides are not serialized into project files or exported. They do not add guide snapping to Core2D's drawing tools. Existing document snap/grid behavior remains separate.

## Layers panel

`StudioLayerRow` and its disposable adapter add compact object icons, ellipsized names, inline rename and contextual visibility/lock actions to the existing ProDataGrid hierarchy. Actions appear on hover/focus and remain visible for hidden or locked shapes. Containers expose visibility but do not manufacture an unsupported lock property. Rename, visibility and shape locking use the existing document history, preserving unrelated state flags and model identity.

Double-click or F2 starts inline rename; Enter commits valid names and Escape cancels. The hierarchy toolbar provides reveal selection, expand all and collapse all. Existing per-object context menus, selection, drag/drop, filtering, search and sorting infrastructure remain in place. The hierarchy's original supported child relationships are unchanged.

## Validation

The headless suite exercises actual populated workspaces, transformed ruler coordinates, selection ranges, guide gestures and undo, pointer-anchored zoom, panning without document mutation, layer rename/state changes, reattachment and bounded tick arithmetic. Screenshots are produced by Avalonia Headless with Skia in both themes and compact layout.

```sh
dotnet test tests/Core2D.UI.Tests/Core2D.UI.Tests.csproj -c Release
dotnet test tests/Core2D.ViewModels.Tests/Core2D.ViewModels.Tests.csproj -c Release
```

Use a recursive checkout and .NET 10. Set `CORE2D_UI_ARTIFACTS` to retain PNGs. Native touchpad, OS focus, IME, accessibility, drag/drop and floating-window behavior still require interactive platform validation beyond these tests. This is a Figma-inspired interaction pass, not complete Figma feature parity.
