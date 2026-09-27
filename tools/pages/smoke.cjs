'use strict';
// Real browser + .NET/Avalonia input checks. Diagnostics are read-only; edits use native input.
const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const path = require('node:path');
const playwright = require('playwright');

(async () => {
  const scenarios = {
    chromium: { engine: 'chromium' },
    webkit: { engine: 'webkit' },
    firefox: { engine: 'firefox' },
    'chromium-legacy-screen': { engine: 'chromium', legacyScreen: true },
    'screen-regression-negative': { engine: 'chromium', legacyScreen: true, negative: true }
  };
  const scenarioName = process.argv[4] || 'chromium';
  const scenario = scenarios[scenarioName];
  assert.ok(scenario, `Unknown browser scenario: ${scenarioName}`);
  const base = new URL(process.argv[2]);
  if (base.hostname.endsWith('.github.io')) base.protocol = 'https:';
  if (!base.pathname.endsWith('/')) base.pathname += '/';
  const output = process.argv[3] || 'artifacts/browser';
  await fs.mkdir(output, { recursive: true });
  const report = { url: base.href, scenario: scenarioName, engine: scenario.engine,
    expectedCommit: process.env.EXPECTED_COMMIT, checks: [], console: [], errors: [], renderingFallbacks: [], failedRequests: [] };
  let browser;
  let page;
  const assertHealthy = () => {
    assert.deepEqual(report.errors, [], 'No unexpected JavaScript, HTTP or .NET console errors');
    assert.deepEqual(report.failedRequests, [], 'No failed application resource requests');
  };
  const inspectScreen = () => ({
    addEventListener: typeof screen.addEventListener,
    removeEventListener: typeof screen.removeEventListener,
    getScreenDetails: typeof window.getScreenDetails,
    orientation: typeof screen.orientation?.type,
    devicePixelRatio: window.devicePixelRatio
  });
  try {
    let manifest;
    for (let attempt = 0; attempt < 30; attempt++) {
      try {
        const url = new URL('deployment.json', base);
        url.searchParams.set('verify', `${Date.now()}`);
        const response = await fetch(url, { cache: 'no-store', signal: AbortSignal.timeout(15000) });
        if (response.ok) {
          const candidate = await response.json();
          if (!report.expectedCommit || candidate.commit === report.expectedCommit) { manifest = candidate; break; }
        }
      } catch (error) { report.console.push(`Manifest retry: ${error.message}`); }
      await new Promise(resolve => setTimeout(resolve, 10000));
    }
    assert.ok(manifest, 'The public manifest must identify the expected source commit');
    report.commit = manifest.commit;
    report.files = manifest.files.length;
    report.checks.push('Published source commit matches');
    browser = await playwright[scenario.engine].launch({ headless: true,
      ...(scenario.engine === 'chromium' ? { args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader', '--ignore-gpu-blocklist'] } : {}) });
    report.browserVersion = browser.version();
    page = await browser.newPage({ viewport: { width: 1440, height: 900 }, deviceScaleFactor: 1 });
    page.setDefaultTimeout(60000);
    // Deterministic negative control and regression coverage even if a future engine adds Screen events.
    // This is test-only capability injection, not application code or a public bypass switch.
    if (scenario.legacyScreen) await page.addInitScript(() => {
      Object.defineProperties(window.screen, {
        addEventListener: { configurable: true, value: undefined },
        removeEventListener: { configurable: true, value: undefined },
        orientation: { configurable: true, value: undefined }
      });
      Object.defineProperty(window, 'getScreenDetails', { configurable: true, value: undefined });
    });
    if (scenario.negative) await page.route('**/screen-compat.mjs*', route => route.fulfill({
      contentType: 'text/javascript', body: 'export const installScreenCompatibility = () => ({name: "negative-control-no-adapter"});'
    }));
    page.on('pageerror', error => report.errors.push(error.stack || error.message));
    page.on('console', message => {
      const text = message.text();
      report.console.push(`${message.type()}: ${text}`);
      if (message.type() === 'error') {
        // Keep exact, known renderer capability probes separate; all other errors still fail.
        if (/^Failed to create render target for mode [23] : HTMLCanvasElement\.getContext returned null\.$/.test(text)) report.renderingFallbacks.push(text);
        else report.errors.push(text);
      }
    });
    page.on('requestfailed', request => report.failedRequests.push({ url: request.url(), failure: request.failure() }));
    page.on('response', response => { if (response.status() >= 400) report.errors.push(`HTTP ${response.status()}: ${response.url()}`); });
    await page.goto(base.href, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.waitForFunction(() => globalThis.__core2dBoot?.state === 'ready' && globalThis.core2dDiagnostics?.IsReady(), null, { timeout: 180000 });
    assert.equal(await page.locator('meta[name="core2d-source"]').getAttribute('content'), manifest.commit);
    await page.locator('canvas').first().waitFor({ state: 'visible' });
    await page.waitForFunction(() => [...document.querySelectorAll('canvas')].some(c => c.width > 600 && c.height > 400));
    report.screenCapabilities = await page.evaluate(inspectScreen);
    report.userAgent = await page.evaluate(() => navigator.userAgent);
    await page.waitForTimeout(1500);
    await page.screenshot({ path: path.join(output, 'home.png') });
    report.checks.push('Published .NET runtime initialized and Avalonia canvas is visible');
    assert.equal(await page.evaluate(() => core2dDiagnostics.HasProject()), false);
    // The recording fails before New: exercise Dock pointer tracking on the home surface first.
    for (const [x, y] of [[30, 100], [380, 220], [800, 340], [1300, 100], [40, 320], [400, 240]]) {
      await page.mouse.move(x, y, { steps: 12 });
    }
    await page.waitForTimeout(500);
    if (scenario.negative) {
      assert.match(report.errors.join('\n'), /screen\.addEventListener.*(?:not a function|undefined)/,
        'Removing only the compatibility adapter must reproduce the recorded exception');
      assert.equal(await page.evaluate(() => core2dDiagnostics.HasProject()), false);
      report.checks.push('Negative control reproduced the exact screen.addEventListener pointer-tracking failure');
      report.success = true;
      return;
    }
    assertHealthy();
    report.checks.push('Repeated home pointer movement does not throw or interrupt input');
    await page.mouse.click(380, 230);
    await page.waitForFunction(() => core2dDiagnostics.HasProject());
    await page.waitForTimeout(800);
    await page.screenshot({ path: path.join(output, 'new-project.png') });
    report.checks.push('Native New drawing action creates a project');
    await page.mouse.click(680, 420);
    await page.keyboard.press('r');
    await page.waitForFunction(() => core2dDiagnostics.GetCurrentTool() === 'Rectangle');
    const before = await page.evaluate(() => core2dDiagnostics.GetShapeCount());
    await page.mouse.move(650, 400);
    await page.mouse.down();
    await page.mouse.move(810, 520, { steps: 12 });
    await page.mouse.up();
    await page.waitForTimeout(200);
    if (await page.evaluate(() => core2dDiagnostics.GetShapeCount()) === before) await page.mouse.click(810, 520);
    await page.waitForFunction(count => core2dDiagnostics.GetShapeCount() === count + 1, before);
    report.checks.push('Native pointer input creates a rectangle');
    await page.keyboard.press('Escape');
    await page.keyboard.press('Control+z');
    await page.waitForFunction(count => core2dDiagnostics.GetShapeCount() === count, before);
    await page.keyboard.press('Control+y');
    await page.waitForFunction(count => core2dDiagnostics.GetShapeCount() === count + 1, before);
    report.checks.push('Document undo and redo restore the expected shape count');
    await page.waitForTimeout(500);
    await page.screenshot({ path: path.join(output, 'edited-project.png') });
    for (const [width, height, name] of [[1024, 768, 'compact-project'], [1440, 460, 'short-viewport']]) {
      await page.setViewportSize({ width, height });
      await page.waitForTimeout(800);
      await page.mouse.move(380, 120, { steps: 12 });
      await page.mouse.move(width - 40, height - 40, { steps: 12 });
      await page.screenshot({ path: path.join(output, `${name}.png`) });
      assert.equal(await page.evaluate(() => core2dDiagnostics.GetShapeCount()), before + 1);
    }
    report.checks.push('Workspace input survives compact and developer-tools-sized viewport changes');
    assert.deepEqual(await page.evaluate(inspectScreen), report.screenCapabilities, 'Native Screen and permission APIs are not monkey-patched');
    assertHealthy();
    report.success = true;
  } catch (error) {
    report.success = false;
    report.failure = error.stack || error.message;
    if (page) {
      await page.screenshot({ path: path.join(output, 'failure.png') }).catch(() => {});
      await fs.writeFile(path.join(output, 'failure.html'), await page.content()).catch(() => {});
    }
    process.exitCode = 1;
  } finally {
    if (page) report.boot = await page.evaluate(() => globalThis.__core2dBoot || null).catch(() => null);
    if (browser) await browser.close();
    await fs.writeFile(path.join(output, 'validation.json'), JSON.stringify(report, null, 2));
    console.log(JSON.stringify(report, null, 2));
  }
})().catch(error => { console.error(error); process.exitCode = 1; });
