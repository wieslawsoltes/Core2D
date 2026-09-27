// Copyright (c) Wiesław Šoltés. Licensed under the MIT license.
// Application-local adapter for Avalonia.Browser 11.3.12's exported render-target registry.
// It does not patch native canvas methods, schedule frames, or replace Avalonia's renderer.

/**
 * Create main-thread Avalonia WebGL contexts with a retained drawing buffer. The pinned
 * framework requests a discardable buffer; WebKit can leave its presented image stale
 * after a real edit even though the model and drawing commands are current.
 *
 * A canvas returns its existing context on subsequent same-type getContext calls, so
 * seeding its creation attributes lets the original framework register that exact native
 * context with Emscripten. All other attributes and the ordered renderer fallback remain
 * unchanged. Install after runtime.create(), before runtime.runMain().
 *
 * Worker-owned surfaces deliberately retain the framework path: this single-threaded
 * application does not transfer or acquire an OffscreenCanvas behind a worker's back.
 * Disposing restores registry methods, not the immutable attributes of existing contexts.
 */
export function installRenderCompatibility(registry) {
    if (typeof registry?.create !== 'function' || typeof registry?.getRenderTarget !== 'function') {
        throw new TypeError('Unsupported Avalonia WebRenderTargetRegistry contract');
    }
    const originalCreate = registry.create;
    const originalGetTarget = registry.getRenderTarget;
    const prepared = new WeakMap();
    let disposed = false;
    let retainedContexts = 0;
    let delegatedSurfaces = 0;

    function prepare(canvas, modes) {
        if (prepared.has(canvas)) return prepared.get(canvas);
        // Unknown future modes must keep their original semantics, not acquire a context
        // of a different type. No defaults are invented for an absent preference list.
        if (!modes || typeof modes[Symbol.iterator] !== 'function') return null;
        const ordered = Array.from(modes);
        if (ordered.some(mode => mode !== 1 && mode !== 2 && mode !== 3)) return null;
        for (const mode of ordered) {
            if (mode === 1) break; // Software has priority: do not lock this canvas to WebGL.
            const attributes = {
                alpha: true, depth: true, stencil: true, antialias: false,
                premultipliedAlpha: true, preserveDrawingBuffer: true,
                failIfMajorPerformanceCaveat: true,
                majorVersion: mode === 2 ? 1 : 2, minorVersion: 0,
                enableExtensionsByDefault: 1, explicitSwapControl: 0
            };
            let context;
            try {
                context = canvas.getContext(mode === 2 ? 'webgl' : 'webgl2', attributes);
            } catch {
                // Let the original factory own error reporting and fallback decisions.
                continue;
            }
            if (context) {
                prepared.set(canvas, context);
                if (context.getContextAttributes?.()?.preserveDrawingBuffer === true) retainedContexts++;
                return context;
            }
        }
        return null;
    }

    function create(pthreadId, canvas, modes) {
        if (disposed) return Reflect.apply(originalCreate, this, arguments);
        let context = null;
        if (pthreadId === 0 && canvas && typeof canvas.getContext === 'function') {
            context = prepare(canvas, modes);
        }
        const id = Reflect.apply(originalCreate, this, arguments);
        if (context) {
            const target = Reflect.apply(originalGetTarget, this, [id]);
            // Keep the original attribute object (also held by Emscripten) consistent
            // with native attributes, without replacing the target or its context.
            if (target?.renderTargetType === 'webgl' && target.canvas === canvas && target.attrs) {
                target.attrs.preserveDrawingBuffer = context.getContextAttributes()?.preserveDrawingBuffer === true;
            }
        } else {
            delegatedSurfaces++;
        }
        return id;
    }
    registry.create = create;
    return Object.freeze({
        name: 'avalonia-11.3.12-retained-webgl-buffer',
        snapshot: () => Object.freeze({ retainedContexts, delegatedSurfaces }),
        dispose() {
            if (disposed) return;
            disposed = true;
            if (registry.create === create) registry.create = originalCreate;
        }
    });
}
