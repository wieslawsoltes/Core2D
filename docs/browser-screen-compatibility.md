# Browser screen compatibility

## Recorded failure

The September 27 recording shows `TypeError: e.screen.addEventListener is not a function`
while the pointer moves over Core2D's Home view. Dock's `GetScreenPoint` initializes
Avalonia's browser screen service. Avalonia 11.3.12's `ScreenHelper.subscribeOnChanged`
unconditionally subscribes to the optional Screen `change` event. A browser can expose
screen bounds without making `Screen` an EventTarget. The resulting exception prevents
ordinary pointer actions, including opening a new drawing; this is not a Dock theme error.

Pinned source contracts:

- `AvaloniaUI/Avalonia`, tag `11.3.12`, `src/Browser/Avalonia.Browser/webapp/modules/avalonia/screens.ts`.
- `src/Browser/Avalonia.Browser/Interop/ScreenHelper.cs` imports the asynchronous
  `checkPermissions` method as `void`, so optional permission discovery must observe rejection.

## Application-local correction

`src/Core2D.Browser/wwwroot/screen-compat.mjs` adapts only the exported framework
`ScreenHelper` methods. Bootstrap installs the adapter after creating the .NET runtime
and before `runMain` binds Avalonia's JS imports. It imports the same `_framework/avalonia.js`
module as Avalonia. Generated framework assets, native Screen objects/prototypes,
Permissions methods and Dock controls are not patched or replaced. No browser-name sniffing,
permission prompts, fabricated permission grants or global error suppression are used.

Native Screen/ScreenDetails events retain Avalonia's original subscription behavior.
When Screen lacks event methods, its original callback is subscribed to a private,
per-window EventTarget. Only the subscription method receives this facade; actual bounds,
working-area and scaling queries still read the real screen. Screen metric changes are
observed on resize, orientation, focus, page restoration and a rearmed resolution media
query. Unchanged viewport-only resizes do not broadcast screen invalidations. The adapter
uses no polling and repeated framework subscriptions do not duplicate callbacks.

The optional window-management permission discovery is skipped when the associated API
is absent. Expected unsupported/denied rejections use single-screen behavior; unexpected
failures remain visible as warnings. Missing ScreenOrientation is handled by primary
orientation inferred from physical screen dimensions, not the canvas viewport. This is
not a multi-monitor or orientation-lock polyfill. Physical monitor changes without any
browser notification are discovered at the next observed event.

A disposer detaches adapter-owned listeners and restores only its own method replacements.
Upgrading to genuine ScreenDetails tears down the single-screen fallback. The adapter is
version-specific; remove it when an upgraded Avalonia version handles these capabilities,
and rerun the regression gate rather than silently carrying it indefinitely.

## Regression and deployment gate

Run `node --test tools/pages/screen-compat.test.mjs` for the independent JS boundary tests.
They cover native-path preservation, legacy events, deduplication, metric changes,
resolution rearming, permission rejection, missing orientation, ScreenDetails upgrade,
independent window lifetimes and disposal.

The Pages workflow runs the published application with pinned Playwright Chromium,
WebKit and Firefox, plus a Chromium case with absent optional screen APIs. Each positive
case moves the pointer across Home repeatedly, creates a drawing, draws a rectangle,
undoes/redoes, resizes to compact and developer-tools-sized viewports, and checks errors,
requests and document shape counts. Native capabilities must be unchanged after startup.

A **local-only negative control** injects the recorded missing capabilities and intercepts
only the compatibility module, leaving the original framework unadapted. It must reproduce
the exact screen.addEventListener failure; otherwise the regression gate fails. The deployed
application has no URL flag or production API for disabling the adapter.

Publication is gated on all local cases. After publication the four positive cases repeat
against the public HTTPS URL, requiring the HTML and deployment manifest to identify the
expected source commit. Each browser keeps its own JSON report and screenshots. This is
cross-engine headless validation, not a claim of interactive testing on physical macOS/iOS
or physical-GPU performance. Existing application feature boundaries remain unchanged.

WebKit runs with a device scale factor of two to cover the high-DPI setup shown in the
recording. The test retains and separately reports its two exact startup
`WEBGL_debug_renderer_info not enabled` diagnostics; these probe the optional renderer
identification extension. Only that precise WebKit startup message, at most twice, is
classified separately. It is accepted only after all native editing checks pass. Other
WebGL errors, screen errors, JavaScript/.NET exceptions and failed requests still fail
the gate. Production logging and renderer selection are not suppressed or replaced.
The test also compares native screen and permission object/method identities captured
before bootstrap, not just their reported types after startup.
