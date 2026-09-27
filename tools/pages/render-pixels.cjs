'use strict';
// Validate the real presented canvas, not just its document model. Test-only code.
const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const path = require('node:path');

// CSS-pixel region inside the blank test page and enclosing its snapped rectangle.
const drawingRegion = Object.freeze({ x: 625, y: 375, width: 215, height: 175 });

function measureInk(image, region = drawingRegion) {
  const { x, y, width, height } = region;
  for (const value of [x, y, width, height, image.width, image.height]) assert.ok(Number.isSafeInteger(value));
  assert.ok(x >= 0 && y >= 0 && width > 0 && height > 0);
  assert.ok(x + width <= image.width && y + height <= image.height, 'Drawing region must be wholly inside the screenshot');
  assert.equal(image.data.length, image.width * image.height * 4, 'Expected decoded RGBA pixels');
  let count = 0, minX = Infinity, minY = Infinity, maxX = -1, maxY = -1;
  for (let row = y; row < y + height; row++) {
    for (let column = x; column < x + width; column++) {
      const index = (row * image.width + column) * 4;
      if (image.data[index + 3] > 200 && image.data[index] < 100 && image.data[index + 1] < 100 && image.data[index + 2] < 100) {
        count++;
        minX = Math.min(minX, column); maxX = Math.max(maxX, column);
        minY = Math.min(minY, row); maxY = Math.max(maxY, row);
      }
    }
  }
  return { count, minX: count ? minX : null, minY: count ? minY : null,
    maxX: count ? maxX : null, maxY: count ? maxY : null,
    spanX: count ? maxX - minX : 0, spanY: count ? maxY - minY : 0 };
}

function matchesInk(sample, present) {
  return present ? sample.count > 300 && sample.spanX > 100 && sample.spanY > 70 : sample.count === 0;
}

async function expectInk(page, output, phase, present, samples) {
  assert.match(phase, /^[a-z][a-z0-9-]*$/);
  const { PNG } = require('pngjs');
  const started = Date.now();
  let bytes, sample;
  do {
    // CSS scale makes the same region meaningful at DPR 1 and 2. No mouse move,
    // resize, DOM mutation or production invalidation call may refresh the frame.
    bytes = await page.screenshot({ scale: 'css' });
    sample = { phase, present, ...measureInk(PNG.sync.read(bytes)), elapsedMs: Date.now() - started };
    if (matchesInk(sample, present)) {
      samples.push(sample);
      await fs.writeFile(path.join(output, `${phase}-pixels.png`), bytes);
      return;
    }
    await page.waitForTimeout(200);
  } while (Date.now() - started < 10000);
  samples.push(sample);
  await fs.writeFile(path.join(output, `${phase}-pixels-failure.png`), bytes);
  throw new Error(`Rendered frame mismatch: ${JSON.stringify(sample)}`);
}

module.exports = { drawingRegion, measureInk, matchesInk, expectInk };
