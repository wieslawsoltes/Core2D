'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const { measureInk, matchesInk } = require('./render-pixels.cjs');

function image(width = 180, height = 120) {
  return { width, height, data: Buffer.alloc(width * height * 4, 255) };
}
function pixel(value, x, y, r = 0, g = 0, b = 0, a = 255) {
  value.data.set([r, g, b, a], (y * value.width + x) * 4);
}
const region = Object.freeze({ x: 0, y: 0, width: 180, height: 120 });

function rectangle() {
  const value = image();
  for (let x = 10; x <= 165; x++) { pixel(value, x, 10); pixel(value, x, 105); }
  for (let y = 11; y < 105; y++) { pixel(value, 10, y); pixel(value, 165, y); }
  return value;
}

test('Blank pixels satisfy only the absent-shape assertion', () => {
  const sample = measureInk(image(), region);
  assert.equal(sample.count, 0);
  assert.equal(sample.minX, null);
  assert.equal(sample.maxY, null);
  assert.equal(matchesInk(sample, false), true);
  assert.equal(matchesInk(sample, true), false);
});

test('A correctly drawn outline satisfies presence and rejects stale undo pixels', () => {
  const sample = measureInk(rectangle(), region);
  assert.equal(sample.count, 500);
  assert.equal(sample.spanX, 155);
  assert.equal(sample.spanY, 95);
  assert.equal(matchesInk(sample, true), true);
  assert.equal(matchesInk(sample, false), false);
});

test('A line or a small cluster cannot impersonate the expected rectangle', () => {
  const line = image();
  for (let x = 0; x < 180; x++) { pixel(line, x, 10); pixel(line, x, 11); }
  assert.equal(matchesInk(measureInk(line, region), true), false);
  const cluster = image();
  for (let y = 0; y < 20; y++) for (let x = 0; x < 20; x++) pixel(cluster, x, y);
  assert.equal(matchesInk(measureInk(cluster, region), true), false);
});

test('Transparent black does not count as presented ink', () => {
  const value = image();
  value.data.fill(0);
  assert.equal(measureInk(value, region).count, 0);
});

test('Ink outside the requested canvas region is ignored', () => {
  const value = rectangle();
  const sample = measureInk(value, { x: 30, y: 30, width: 60, height: 50 });
  assert.equal(sample.count, 0);
});

test('All color and alpha thresholds are required', () => {
  const value = image();
  pixel(value, 0, 0, 99, 99, 99, 201);
  pixel(value, 1, 0, 100, 0, 0, 255);
  pixel(value, 2, 0, 0, 100, 0, 255);
  pixel(value, 3, 0, 0, 0, 100, 255);
  pixel(value, 4, 0, 0, 0, 0, 200);
  assert.equal(measureInk(value, region).count, 1);
});

test('Offscreen and degenerate regions fail instead of returning a false blank', () => {
  for (const invalid of [
    { x: -1, y: 0, width: 10, height: 10 },
    { x: 175, y: 0, width: 10, height: 10 },
    { x: 0, y: 115, width: 10, height: 10 },
    { x: 0, y: 0, width: 0, height: 10 },
    { x: 0.5, y: 0, width: 10, height: 10 }
  ]) assert.throws(() => measureInk(image(), invalid));
});

test('Invalid RGBA buffer lengths cannot pass a visual assertion', () => {
  assert.throws(() => measureInk({ width: 180, height: 120, data: Buffer.alloc(100) }, region));
});
