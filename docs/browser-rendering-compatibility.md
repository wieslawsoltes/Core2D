# Browser rendering compatibility

Core2D uses the pinned Avalonia.Browser 11.3.12 / SkiaSharp 3.119.1 WebAssembly renderer. The BrowserTheme and Dock redesign is unchanged by this fix.

## Reproduced defect

The earlier smoke test proved that pointer input and undo/redo changed the document model, but its screenshots were not assertions. In Playwright 1.55.1's Linux WebKit, the model and recorded Avalonia drawing commands changed while the presented WebGL image could retain an unfinished rectangle or an undone shape. WebGL1 and WebGL2 reproduced this at device pixel ratios 1 and 2. The same software-rendered test passed.

Isolated trials of explicit model invalidation, disabling the GPU resource cache, compositor layer clipping, redundant-resize suppression, and GPU flush/finish did not solve the presented-pixel failure. Those probes are not production code. Retaining the WebGL drawing buffer did pass. The exact underlying WebKit/driver defect has not been established; this is a validated compatibility workaround for the pinned stack, not a claim about every Safari version or physical GPU.

## Scoped retained-buffer adapter

`src/Core2D.Browser/wwwroot/render-compat.mjs` wraps the exported `WebRenderTargetRegistry.create` method before managed JS imports are bound. For main-thread GPU surfaces, it calls the native canvas `getContext` with the pinned framework's creation attributes and `preserveDrawingBuffer: true`. The original factory then receives and registers the same context; native WebGL context attributes are fixed at first creation. The original target attribute object is kept consistent with the actual native value.

The adapter preserves preferred-mode ordering, allows WebGL2 to fall back to WebGL1 and software, respects software-first preferences, and leaves unknown future modes and worker-owned canvases to the original framework. It does not replace native canvas methods, Screen/permission APIs, animation callbacks, models, rendering commands, or Emscripten registration. It adds no repaint loop, polling, readback, or artificial input. Canvas associations are weakly held. Disposal restores the original registry method only while it still owns that method; existing contexts retain their immutable creation attributes.

Install after `.NET runtime.create()` and before `runtime.runMain()`. Bootstrap imports the same unversioned Avalonia module used by managed interop, while the application-local compatibility module is commit-versioned. Startup diagnostics report the adapter name and initial retained/delegated surface counts. Existing screen compatibility remains enabled independently.

This adapter is deliberately pinned to the current exported registry and context-attribute contract. Review it when upgrading Avalonia/SkiaSharp. The current application is single-threaded; worker-owned WebGL surfaces are not claimed fixed by this main-thread adapter.

## Cost and removal criterion

Buffer preservation can carry extra bandwidth/memory and presentation costs on some GPUs. It retains GPU rendering and the same image quality, but no broad hardware performance improvement is claimed. Do not remove it based only on model-state tests. Removal requires the unadapted runtime to pass stationary-pointer rendered draw/undo/redo tests in the supported engines and physical Safari/platform checks.

Reference: [WebGL drawing-buffer and context-creation contract](https://registry.khronos.org/webgl/specs/1.0/). The attributes used by the pinned upstream implementation are in [Avalonia's WebGlRenderTarget](https://github.com/AvaloniaUI/Avalonia/blob/11.3.12/src/Browser/Avalonia.Browser/webapp/modules/avalonia/rendering/webGlRenderTarget.ts).

## Deployment gates

`tools/pages/render-pixels.cjs` decodes actual browser screenshots at CSS scale. It checks a bounded region of a known blank page, rejects invalid image bounds, distinguishes an outline from a corrupt black canvas, and requires the complete snapped rectangle bounds rather than accepting a partial drag preview. Undo must leave zero dark pixels in that region; redo must restore the outline. Every positive smoke scenario repeats three undo/redo cycles without pointer movement, resizing, or test-driven invalidation between those assertions.

`tools/pages/smoke.cjs` continues to check document counts, startup status, source-commit metadata, HTTP/resource failures, native API identities and compact/short viewport behavior. WebKit cases assert that the retained GPU path is exercised rather than accepting an accidental software fallback. The normal Firefox run may use its supported software fallback on a CI runner. The exact known renderer capability diagnostics are retained separately; unrelated errors remain fatal.

Two negative controls run before deployment. Removing only screen compatibility must reproduce the original pointer-tracking exception. Removing only render compatibility must reproduce a pixel mismatch after a model edit; a JavaScript error, network error, or invalid screenshot is not accepted as that regression.

The Pages workflow now runs on pull requests as a read-only build/test gate. Only master can publish. A successful master build deploys the complete tested .NET publish output to the existing Pages configuration, then checks the exact deployed source commit and real editing over public HTTPS in Chromium, Firefox, WebKit DPR 1/2, and Chromium with legacy Screen capabilities. Native OS IME, accessibility, touch and physical-GPU coverage remain separate validation requirements.

```sh
node --test tools/pages/screen-compat.test.mjs tools/pages/render-compat.test.mjs tools/pages/render-pixels.test.cjs
# After staging a real .NET publish output with tools/pages/prepare.py:
node tools/pages/smoke.cjs http://127.0.0.1:4173/Core2D/ artifacts/webkit webkit
```

Browser runs use pinned `playwright@1.55.1` and `pngjs@7.0.0`; plain unit tests need only Node 22. Screenshots and validation JSON retain source identity and exact sampled pixels. Temporary experiment workflows and their generated probe scripts are absent from the production tree.
