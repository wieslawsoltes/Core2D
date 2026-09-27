import test from 'node:test';
import assert from 'node:assert/strict';
import { installScreenCompatibility } from '../../src/Core2D.Browser/wwwroot/screen-compat.mjs';

function fixture({ native = false, legacyMedia = false } = {}) {
    const calls = { changed: 0, permissions: 0, query: 0, details: 0, warnings: [] };
    const win = new EventTarget();
    const screen = native ? new EventTarget() : {};
    Object.assign(screen, { width: 1920, height: 1080, availWidth: 1920, availHeight: 1040, colorDepth: 24 });
    Object.assign(win, { screen, devicePixelRatio: 1, Event, EventTarget,
        console: { warn: (...args) => calls.warnings.push(args) }, navigator: { permissions: {
            query: async () => { calls.query++; return { state: 'prompt' }; }
        } }, getScreenDetails: undefined
    });
    const queries = [];
    win.matchMedia = value => {
        const target = new EventTarget();
        const query = legacyMedia ? {
            addListener: callback => target.addEventListener('change', callback),
            removeListener: callback => target.removeEventListener('change', callback)
        } : target;
        queries.push({ value, target, query });
        return query;
    };
    const changed = () => calls.changed++;
    const helper = {
        detailedScreens: undefined,
        subscribeOnChanged(global) {
            if (this.detailedScreens) {
                global.screen.removeEventListener('change', changed);
                this.detailedScreens.addEventListener('screenschange', changed);
                for (const s of this.detailedScreens.screens) s.addEventListener('change', changed);
            } else global.screen.addEventListener('change', changed);
        },
        async checkPermissions(global) {
            calls.permissions++;
            const permission = await global.navigator.permissions.query({ name: 'window-management' });
            if (permission.state === 'granted') await this.requestDetailedScreens(global);
        },
        async requestDetailedScreens(global) {
            calls.details++;
            this.detailedScreens = await global.getScreenDetails();
            this.subscribeOnChanged(global);
        },
        getCurrentOrientation(s) { return { 'landscape-primary': 1, 'portrait-primary': 2,
            'landscape-secondary': 4, 'portrait-secondary': 8 }[s.orientation.type]; }
    };
    return { calls, win, screen, helper, queries };
}

test('original framework contract reproduces the recording without Screen EventTarget', () => {
    const { helper, win } = fixture();
    assert.throws(() => helper.subscribeOnChanged(win), /addEventListener/);
});

test('native screen methods and subscriptions are preserved', () => {
    const { helper, win, screen, calls, queries } = fixture({ native: true });
    const add = screen.addEventListener;
    const adapter = installScreenCompatibility(helper);
    helper.subscribeOnChanged(win);
    helper.subscribeOnChanged(win);
    screen.dispatchEvent(new Event('change'));
    assert.equal(calls.changed, 1);
    assert.equal(queries.length, 0);
    assert.equal(screen.addEventListener, add);
    assert.equal(win.screen, screen);
    adapter.dispose();
});

test('fallback forwards changed screen metadata and deduplicates repeated subscriptions', () => {
    const { helper, win, screen, calls } = fixture();
    const before = Object.getOwnPropertyDescriptors(screen);
    const adapter = installScreenCompatibility(helper);
    helper.subscribeOnChanged(win);
    helper.subscribeOnChanged(win);
    win.dispatchEvent(new Event('resize'));
    assert.equal(calls.changed, 0, 'viewport-only resize must not invalidate screens');
    screen.width = 2560;
    win.dispatchEvent(new Event('resize'));
    win.dispatchEvent(new Event('focus'));
    assert.equal(calls.changed, 1);
    screen.availHeight = 1000;
    win.dispatchEvent(new Event('pageshow'));
    assert.equal(calls.changed, 2);
    screen.width = before.width.value;
    screen.availHeight = before.availHeight.value;
    assert.deepEqual(Object.getOwnPropertyDescriptors(screen), before, 'no native Screen patching');
    adapter.dispose();
    screen.width = 3000;
    win.dispatchEvent(new Event('resize'));
    assert.equal(calls.changed, 2, 'disposed watchers are detached');
});

for (const legacyMedia of [false, true]) test(`DPR watcher rearms and detaches (${legacyMedia ? 'legacy' : 'modern'} MediaQueryList)`, () => {
    const { helper, win, calls, queries } = fixture({ legacyMedia });
    const adapter = installScreenCompatibility(helper);
    helper.subscribeOnChanged(win);
    assert.match(queries[0].value, /1dppx/);
    win.devicePixelRatio = 2;
    queries[0].target.dispatchEvent(new Event('change'));
    assert.equal(calls.changed, 1);
    assert.match(queries[1].value, /2dppx/);
    queries[0].target.dispatchEvent(new Event('change'));
    assert.equal(queries.length, 2, 'old resolution listener was removed');
    adapter.dispose();
    win.devicePixelRatio = 3;
    queries[1].target.dispatchEvent(new Event('change'));
    assert.equal(calls.changed, 1);
});

test('real ScreenDetails upgrade tears down single-screen fallback without touching screens', async () => {
    const { helper, win, screen, calls, queries } = fixture();
    const adapter = installScreenCompatibility(helper);
    helper.subscribeOnChanged(win);
    const detail = new EventTarget();
    const details = Object.assign(new EventTarget(), { screens: [detail] });
    win.getScreenDetails = async () => details;
    win.navigator.permissions.query = async () => ({ state: 'granted' });
    await helper.checkPermissions(win);
    assert.equal(calls.details, 1);
    screen.width = 2560;
    win.dispatchEvent(new Event('resize'));
    queries[0].target.dispatchEvent(new Event('change'));
    assert.equal(calls.changed, 0);
    detail.dispatchEvent(new Event('change'));
    details.dispatchEvent(new Event('screenschange'));
    assert.equal(calls.changed, 2);
    assert.equal(screen.addEventListener, undefined);
    adapter.dispose();
});

test('unsupported screen-management capability never queries permissions', async () => {
    const { helper, win, calls } = fixture();
    const query = win.navigator.permissions.query;
    const adapter = installScreenCompatibility(helper);
    await helper.checkPermissions(win);
    assert.equal(calls.permissions, 0);
    assert.equal(calls.query, 0);
    assert.equal(win.navigator.permissions.query, query);
    adapter.dispose();
});

test('denied and unsupported optional permissions do not reject the void-imported promise', async () => {
    const { helper, win, calls } = fixture();
    const adapter = installScreenCompatibility(helper);
    win.getScreenDetails = async () => { throw new DOMException('denied', 'NotAllowedError'); };
    win.navigator.permissions.query = async () => { throw new TypeError('unsupported permission descriptor'); };
    await helper.checkPermissions(win);
    win.navigator.permissions.query = async () => ({ state: 'granted' });
    await helper.checkPermissions(win);
    assert.equal(calls.warnings.length, 0);
    win.navigator.permissions.query = async () => { throw new Error('unexpected platform failure'); };
    await helper.checkPermissions(win);
    assert.equal(calls.warnings.length, 1, 'unexpected discovery failures remain observable');
    adapter.dispose();
});

test('orientation missing or unknown remains readable without mutating the screen', () => {
    const { helper, screen } = fixture();
    const adapter = installScreenCompatibility(helper);
    assert.equal(helper.getCurrentOrientation(screen), 1);
    screen.width = 800;
    assert.equal(helper.getCurrentOrientation(screen), 2);
    assert.equal(screen.orientation, undefined);
    screen.orientation = { type: 'portrait-secondary' };
    assert.equal(helper.getCurrentOrientation(screen), 8);
    screen.orientation.type = 'unknown';
    assert.equal(helper.getCurrentOrientation(screen), 0);
    delete screen.orientation;
    screen.width = 0;
    assert.equal(helper.getCurrentOrientation(screen), 0);
    adapter.dispose();
});

test('opposite windows have independent fallback lifetimes', () => {
    const first = fixture(), second = fixture();
    const adapter = installScreenCompatibility(first.helper);
    first.helper.subscribeOnChanged(first.win);
    first.helper.subscribeOnChanged(second.win);
    first.screen.width++;
    first.win.dispatchEvent(new Event('resize'));
    second.screen.height++;
    second.win.dispatchEvent(new Event('orientationchange'));
    assert.equal(first.calls.changed, 2);
    adapter.dispose();
    first.screen.width++;
    first.win.dispatchEvent(new Event('resize'));
    assert.equal(first.calls.changed, 2);
});

test('disposal restores only owned methods and installation guards framework drift', () => {
    const { helper } = fixture();
    const original = helper.checkPermissions;
    const adapter = installScreenCompatibility(helper);
    const replacement = () => {};
    helper.subscribeOnChanged = replacement;
    adapter.dispose();
    adapter.dispose();
    assert.equal(helper.subscribeOnChanged, replacement);
    assert.equal(helper.checkPermissions, original);
    assert.throws(() => installScreenCompatibility({}), /Unsupported Avalonia/);
});
