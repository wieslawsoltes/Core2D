const boot = globalThis.__core2dBoot = { state: 'loading', startedAt: performance.now() };
try {
    const runtimeUrl = new URL('./_framework/dotnet.js', import.meta.url);
    const source = document.querySelector('meta[name="core2d-source"]')?.content;
    if (/^[0-9a-f]{40}$/.test(source || '')) runtimeUrl.searchParams.set('v', source);
    const { dotnet } = await import(runtimeUrl.href);
    const runtime = await dotnet.withDiagnosticTracing(false).withApplicationArgumentsFromQuery().create();
    // Load the same framework module that Avalonia imports, after the .NET runtime exists but
    // before runMain binds its ScreenHelper methods. No generated framework files are rewritten.
    const compatibilityUrl = new URL('./screen-compat.mjs', import.meta.url);
    if (/^[0-9a-f]{40}$/.test(source || '')) compatibilityUrl.searchParams.set('v', source);
    const { installScreenCompatibility } = await import(compatibilityUrl.href);
    const { ScreenHelper } = await import(new URL('./_framework/avalonia.js', import.meta.url).href);
    const screenCompatibility = installScreenCompatibility(ScreenHelper);
    boot.screenCompatibility = screenCompatibility.name;
    const config = runtime.getConfig();
    const exports = await runtime.getAssemblyExports(config.mainAssemblyName);
    globalThis.core2dDiagnostics = Object.freeze(exports.Core2D.BrowserDiagnostics);
    await runtime.runMain(config.mainAssemblyName, [globalThis.location.href]);
    boot.state = 'ready';
    boot.readyAt = performance.now();
    document.documentElement.dataset.core2d = 'ready';
    document.querySelector('.avalonia-splash')?.remove();
} catch (error) {
    boot.state = 'failed';
    boot.error = String(error?.stack || error);
    console.error('Core2D startup failed', error);
    const notice = document.createElement('section');
    notice.className = 'startup-error';
    notice.setAttribute('role', 'alert');
    const title = document.createElement('h1');
    title.textContent = 'Core2D could not start';
    const message = document.createElement('p');
    message.textContent = 'Reload this page to retry. The browser console contains the startup details.';
    const retry = document.createElement('button');
    retry.textContent = 'Reload';
    retry.addEventListener('click', () => location.reload());
    notice.append(title, message, retry);
    document.body.append(notice);
}
