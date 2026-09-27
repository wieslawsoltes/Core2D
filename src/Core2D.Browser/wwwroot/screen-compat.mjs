// Copyright (c) Wiesław Šoltés. Licensed under the MIT license.
// Application-local adapter for Avalonia.Browser 11.3.12's exported ScreenHelper.
// Do not modify Screen.prototype, replace window.screen, fabricate permissions, or disable Dock.

/**
 * Install capability-based single-screen fallbacks before Avalonia imports its JS bindings.
 * Native ScreenDetails and native Screen events still use the original framework implementation.
 * The returned disposer is primarily useful to isolated host/test lifetimes; bootstrap installs once.
 */
export function installScreenCompatibility(helper) {
    const original = {
        subscribeOnChanged: helper.subscribeOnChanged,
        checkPermissions: helper.checkPermissions,
        getCurrentOrientation: helper.getCurrentOrientation
    };
    for (const [name, method] of Object.entries(original)) {
        if (typeof method !== 'function') throw new TypeError(`Unsupported Avalonia ScreenHelper contract: ${name}`);
    }
    const subscriptions = new Map();
    const canListen = target => typeof target?.addEventListener === 'function'
        && typeof target?.removeEventListener === 'function';

    function snapshot(win) {
        const screen = win.screen;
        return [screen.width, screen.height, screen.availWidth, screen.availHeight,
            screen.availLeft, screen.availTop, screen.colorDepth, screen.pixelDepth,
            screen.orientation?.type, screen.orientation?.angle, win.devicePixelRatio];
    }

    function singleScreenEvents(win) {
        let state = subscriptions.get(win);
        if (state) return state;
        // Only subscribeOnChanged sees this facade. Bounds/scaling queries still see the actual Screen.
        // The original method installs its own ScreensChanged callback on this native EventTarget.
        const events = new win.EventTarget();
        let previous = snapshot(win);
        let stopResolution = () => {};
        const detach = [];
        let disposed = false;
        function listen(target, name, callback) {
            if (!canListen(target)) return;
            target.addEventListener(name, callback);
            detach.push(() => target.removeEventListener(name, callback));
        }
        function check() {
            if (disposed) return;
            const next = snapshot(win);
            if (next.every((value, index) => Object.is(value, previous[index]))) return;
            previous = next;
            events.dispatchEvent(new win.Event('change'));
        }
        function watchResolution() {
            stopResolution();
            stopResolution = () => {};
            if (disposed || typeof win.matchMedia !== 'function') return;
            const query = win.matchMedia(`(resolution: ${win.devicePixelRatio || 1}dppx)`);
            const changed = () => { watchResolution(); check(); };
            if (canListen(query)) {
                query.addEventListener('change', changed);
                stopResolution = () => query.removeEventListener('change', changed);
            } else if (typeof query?.addListener === 'function' && typeof query?.removeListener === 'function') {
                query.addListener(changed);
                stopResolution = () => query.removeListener(changed);
            }
        }
        listen(win, 'resize', check);
        listen(win, 'orientationchange', check);
        listen(win, 'focus', check);
        listen(win, 'pageshow', check);
        listen(win.screen.orientation, 'change', check);
        watchResolution();
        state = { events, dispose() {
            disposed = true;
            stopResolution();
            for (const remove of detach) remove();
            subscriptions.delete(win);
        } };
        subscriptions.set(win, state);
        return state;
    }

    const subscribe = win => {
        if (canListen(win.screen)) {
            subscriptions.get(win)?.dispose();
            return original.subscribeOnChanged.call(helper, win);
        }
        const state = singleScreenEvents(win);
        original.subscribeOnChanged.call(helper, { screen: state.events });
        // Upgrading to real ScreenDetails removes the framework callback from the fallback hub.
        if (helper.detailedScreens) state.dispose();
    };
    const permissions = async win => {
        // The framework imports this asynchronous method as void. Observe the promise here so
        // unsupported/denied optional window-management APIs cannot become unhandled rejections.
        if (typeof win.getScreenDetails !== 'function' || typeof win.navigator?.permissions?.query !== 'function') return;
        try {
            await original.checkPermissions.call(helper, win);
        } catch (error) {
            // Expected denial/unsupported-feature cases use the existing single-screen behavior.
            if (!['TypeError', 'NotAllowedError', 'NotSupportedError', 'SecurityError'].includes(error?.name)) {
                win.console?.warn('Core2D: optional screen-details discovery failed; using the current screen.', error);
            }
        }
    };
    const orientation = screen => {
        if (typeof screen.orientation?.type === 'string') {
            return original.getCurrentOrientation.call(helper, screen) || 0;
        }
        // Older engines may also omit ScreenOrientation. Infer only primary orientation from
        // physical screen dimensions, never from the editor viewport, and never invent a lock API.
        if (!(screen.width > 0) || !(screen.height > 0)) return 0;
        return screen.width >= screen.height ? 1 : 2;
    };
    helper.subscribeOnChanged = subscribe;
    helper.checkPermissions = permissions;
    helper.getCurrentOrientation = orientation;
    return Object.freeze({
        name: 'avalonia-11.3.12-screen-compatibility',
        dispose() {
            for (const state of subscriptions.values()) state.dispose();
            // Do not remove a later host's replacement method.
            if (helper.subscribeOnChanged === subscribe) helper.subscribeOnChanged = original.subscribeOnChanged;
            if (helper.checkPermissions === permissions) helper.checkPermissions = original.checkPermissions;
            if (helper.getCurrentOrientation === orientation) helper.getCurrentOrientation = original.getCurrentOrientation;
        }
    });
}
