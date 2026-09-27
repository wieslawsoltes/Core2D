'use strict';
// Actual Chromium + .NET/Avalonia input tests. Diagnostics are read-only; edits use real input.
const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const path = require('node:path');
const { chromium } = require('playwright');

(async () => {
  const base = new URL(process.argv[2]);
  // Legacy Pages metadata may advertise HTTP even though the canonical site supports HTTPS.
  if (base.hostname.endsWith('.github.io')) base.protocol = 'https:';
  if (!base.pathname.endsWith('/')) base.pathname += '/';
  const output = process.argv[3] || 'artifacts/browser';
  await fs.mkdir(output, { recursive: true });
  const report = { url: base.href, expectedCommit: process.env.EXPECTED_COMMIT, checks: [], console: [], errors: [], renderingFallbacks: [], failedRequests: [] };
  let browser;
  let page;
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
    browser = await chromium.launch({ headless: true, args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader', '--ignore-gpu-blocklist'] });
    page = await browser.newPage({ viewport: { width: 1440, height: 900 }, deviceScaleFactor: 1 });
    page.setDefaultTimeout(60000);
    page.on('pageerror', error => report.errors.push(error.stack || error.message));
    page.on('console', message => {
      const text = message.text();
      report.console.push(`${message.type()}: ${text}`);
      if (message.type() === 'error') {
        // Avalonia probes GPU modes before its supported software renderer. Do not hide these
        // messages, but only accept these exact capability failures after all editing checks pass.
        if (/^Failed to create render target for mode [23] : HTMLCanvasElement\.getContext returned null\.$/.test(text)) report.renderingFallbacks.push(text);
        else report.errors.push(text);
      }
    });
    page.on('requestfailed', request => report.failedRequests.push({ url: request.url(), failure: request.failure() }));
    page.on('response', response => { if (response.status() >= 400) report.errors.push(`HTTP ${response.status()}: ${response.url()}`); });
    await page.goto(base.href, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.waitForFunction(() => globalThis.__core2dBoot?.state === 'ready' && globalThis.core2dDiagnostics?.IsReady(), null, { timeout: 180000 });
    assert.equal(await page.locator('meta[name="core2d-source"]').getAttribute('content'), manifest.commit, 'The HTML and manifest must identify the same deployment');
    const canvas = page.locator('canvas').first();
    await canvas.waitFor({ state: 'visible' });
    await page.waitForFunction(() => [...document.querySelectorAll('canvas')].some(c => c.width > 600 && c.height > 400));
    await page.waitForTimeout(1500);
    await page.screenshot({ path: path.join(output, 'home.png') });
    report.checks.push('Published .NET runtime initialized and Avalonia canvas is visible');
    assert.equal(await page.evaluate(() => core2dDiagnostics.HasProject()), false);
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
    report.checks.push('Document undo and redo restore the original shape count');
    await page.waitForTimeout(500);
    await page.screenshot({ path: path.join(output, 'edited-project.png') });
    await page.setViewportSize({ width: 1024, height: 768 });
    await page.waitForTimeout(800);
    await page.screenshot({ path: path.join(output, 'compact-project.png') });
    report.checks.push('Workspace renders after compact viewport resize');
    assert.deepEqual(report.errors, [], 'No unexpected JavaScript, HTTP or .NET console errors');
    assert.deepEqual(report.failedRequests, [], 'No failed application resource requests');
    report.boot = await page.evaluate(() => globalThis.__core2dBoot);
    report.success = true;
  } catch (error) {
    report.success = false;
    report.failure = error.stack || error.message;
    if (page) {
      await page.screenshot({ path: path.join(output, 'failure.png') }).catch(() => {});
      report.boot = await page.evaluate(() => globalThis.__core2dBoot || null).catch(() => null);
      await fs.writeFile(path.join(output, 'failure.html'), await page.content()).catch(() => {});
    }
    process.exitCode = 1;
  } finally {
    if (browser) await browser.close();
    await fs.writeFile(path.join(output, 'validation.json'), JSON.stringify(report, null, 2));
    console.log(JSON.stringify(report, null, 2));
  }
})().catch(error => { console.error(error); process.exitCode = 1; });
