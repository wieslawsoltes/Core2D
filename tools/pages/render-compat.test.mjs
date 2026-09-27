import test from 'node:test';
import assert from 'node:assert/strict';
import { installRenderCompatibility } from '../../src/Core2D.Browser/wwwroot/render-compat.mjs';

function fixture({ unavailable = [], throwing = [], existing = null } = {}) {
    const calls = [];
    const registrations = [];
    let selected = existing;
    let counter = 0;
    const targets = new Map();
    const canvas = { getContext(type, attrs) {
        calls.push({ type, attrs: { ...attrs } });
        if (throwing.includes(type)) throw new Error('Context unavailable');
        if (unavailable.includes(type)) return null;
        if (selected && selected.type !== type) return null;
        selected ??= { type, getContextAttributes: () => ({ ...attrs }) };
        return selected;
    } };
    const registry = {
        create(thread, value, modes) {
            assert.equal(this, registry);
            const id = ++counter;
            registrations.push({ thread, canvas: value, modes });
            if (thread !== 0) { targets.set(id, { renderTargetType: 'worker' }); return id; }
            for (const mode of modes) {
                const type = mode === 3 ? 'webgl2' : mode === 2 ? 'webgl' : '2d';
                const attrs = { preserveDrawingBuffer: false, majorVersion: mode === 3 ? 2 : 1 };
                let context;
                try { context = value.getContext(type, attrs); } catch { continue; }
                if (context) {
                    targets.set(id, { renderTargetType: mode === 1 ? 'software' : 'webgl', canvas: value, attrs, context });
                    return id;
                }
            }
            throw new Error('No rendering mode available');
        },
        getRenderTarget(id) { assert.equal(this, registry); return targets.get(id); }
    };
    return { registry, canvas, calls, registrations, context: () => selected };
}

test('Native context and ordered preferences retain identity through original registration', () => {
    const f = fixture();
    const getContext = f.canvas.getContext;
    const getTarget = f.registry.getRenderTarget;
    const adapter = installRenderCompatibility(f.registry);
    const modes = Object.freeze([3, 2, 1]);
    const id = f.registry.create(0, f.canvas, modes);
    const target = f.registry.getRenderTarget(id);
    assert.equal(target.context, f.context());
    assert.equal(f.registrations[0].modes, modes);
    assert.equal(f.registrations[0].canvas, f.canvas);
    assert.equal(f.canvas.getContext, getContext);
    assert.equal(f.registry.getRenderTarget, getTarget);
    assert.equal(target.attrs.preserveDrawingBuffer, true);
    assert.equal(target.context.getContextAttributes().preserveDrawingBuffer, true);
    assert.deepEqual(adapter.snapshot(), { retainedContexts: 1, delegatedSurfaces: 0 });
});

test('Every pinned creation attribute except buffer preservation is kept', () => {
    const f = fixture();
    installRenderCompatibility(f.registry);
    f.registry.create(0, f.canvas, [3, 2, 1]);
    assert.deepEqual(f.calls[0].attrs, {
        alpha: true, depth: true, stencil: true, antialias: false,
        premultipliedAlpha: true, preserveDrawingBuffer: true, failIfMajorPerformanceCaveat: true,
        majorVersion: 2, minorVersion: 0, enableExtensionsByDefault: 1, explicitSwapControl: 0
    });
});

test('An unavailable WebGL2 preserves ordered fallback to native WebGL1', () => {
    const f = fixture({ unavailable: ['webgl2'] });
    installRenderCompatibility(f.registry);
    const id = f.registry.create(0, f.canvas, [3, 2, 1]);
    const target = f.registry.getRenderTarget(id);
    assert.equal(target.context.type, 'webgl');
    assert.equal(target.attrs.majorVersion, 1);
    assert.equal(target.attrs.preserveDrawingBuffer, true);
});

test('Software-first preference never probes or acquires WebGL', () => {
    const f = fixture();
    const adapter = installRenderCompatibility(f.registry);
    const id = f.registry.create(0, f.canvas, [1, 3]);
    assert.equal(f.registry.getRenderTarget(id).renderTargetType, 'software');
    assert.deepEqual(f.calls.map(x => x.type), ['2d']);
    assert.deepEqual(adapter.snapshot(), { retainedContexts: 0, delegatedSurfaces: 1 });
});

test('Unavailable or throwing GPU contexts still reach original software fallback', () => {
    for (const options of [{ unavailable: ['webgl2', 'webgl'] }, { throwing: ['webgl2', 'webgl'] }]) {
        const f = fixture(options);
        const adapter = installRenderCompatibility(f.registry);
        const id = f.registry.create(0, f.canvas, [3, 2, 1]);
        assert.equal(f.registry.getRenderTarget(id).renderTargetType, 'software');
        assert.equal(adapter.snapshot().retainedContexts, 0);
    }
});

test('Worker-owned surfaces are never acquired on the main thread', () => {
    const f = fixture();
    const adapter = installRenderCompatibility(f.registry);
    const id = f.registry.create(42, f.canvas, [3, 2, 1]);
    assert.equal(f.registry.getRenderTarget(id).renderTargetType, 'worker');
    assert.deepEqual(f.calls, []);
    assert.equal(adapter.snapshot().delegatedSurfaces, 1);
});

test('Unknown future modes delegate instead of binding a wrong context type', () => {
    const f = fixture();
    const adapter = installRenderCompatibility(f.registry);
    f.registry.create(0, f.canvas, [4, 3]);
    assert.equal(f.calls.length, 1);
    assert.equal(f.calls[0].attrs.preserveDrawingBuffer, false);
    assert.equal(adapter.snapshot().retainedContexts, 0);
});

test('Typed mode arrays are supported without replacement', () => {
    const f = fixture();
    installRenderCompatibility(f.registry);
    const modes = Int32Array.from([2, 1]);
    const id = f.registry.create(0, f.canvas, modes);
    assert.equal(f.registry.getRenderTarget(id).context.type, 'webgl');
    assert.equal(f.registrations[0].modes, modes);
});

test('Repeated registration does not replace or repeatedly seed a context', () => {
    const f = fixture();
    const adapter = installRenderCompatibility(f.registry);
    f.registry.create(0, f.canvas, [3]);
    f.registry.create(0, f.canvas, [3]);
    assert.equal(f.calls.filter(x => x.attrs.preserveDrawingBuffer === true).length, 1);
    assert.equal(adapter.snapshot().retainedContexts, 1);
});

test('Already-created non-preserving contexts are reported truthfully', () => {
    const existing = { type: 'webgl2', getContextAttributes: () => ({ preserveDrawingBuffer: false }) };
    const f = fixture({ existing });
    const adapter = installRenderCompatibility(f.registry);
    const id = f.registry.create(0, f.canvas, [3]);
    assert.equal(f.registry.getRenderTarget(id).attrs.preserveDrawingBuffer, false);
    assert.equal(adapter.snapshot().retainedContexts, 0);
    assert.equal(f.context(), existing);
});

test('Original factory exceptions still propagate', () => {
    const f = fixture();
    const error = new Error('Registration failed');
    f.registry.create = () => { throw error; };
    installRenderCompatibility(f.registry);
    assert.throws(() => f.registry.create(0, f.canvas, [3]), e => e === error);
});

test('Disposal is idempotent, restores identity, and stale wrapper calls delegate', () => {
    const f = fixture();
    const original = f.registry.create;
    const adapter = installRenderCompatibility(f.registry);
    const wrapper = f.registry.create;
    adapter.dispose(); adapter.dispose();
    assert.equal(f.registry.create, original);
    wrapper.call(f.registry, 0, f.canvas, [3]);
    assert.equal(f.calls.length, 1);
    assert.equal(f.calls[0].attrs.preserveDrawingBuffer, false);
});

test('Disposal does not overwrite a later hosts replacement', () => {
    const f = fixture();
    const adapter = installRenderCompatibility(f.registry);
    const later = () => 999;
    f.registry.create = later;
    adapter.dispose();
    assert.equal(f.registry.create, later);
});

test('Unsupported registry contracts fail before installing partial hooks', () => {
    for (const registry of [null, {}, { create() {} }, { create: 1, getRenderTarget() {} }]) {
        assert.throws(() => installRenderCompatibility(registry), TypeError);
    }
});
