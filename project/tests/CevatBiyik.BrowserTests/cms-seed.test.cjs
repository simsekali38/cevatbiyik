const {test} = require('node:test');
const assert = require('node:assert/strict');
const {chromium} = require('playwright');
const {readFileSync, mkdirSync} = require('node:fs');
const seed = JSON.parse(readFileSync('../database/seeds/REVIEW_DRAFT_CONTENT.json', 'utf8'));
const base = 'https://localhost:57413';

test('full isolated CMS content renders at four widths; all 16 practice routes and articles resolve', async () => {
  const browser = await chromium.launch({channel:'msedge', headless:true});
  const context = await browser.newContext({ignoreHTTPSErrors:true, reducedMotion:'reduce'});
  await context.route('**/*', route => new URL(route.request().url()).origin === base ? route.continue() : route.abort());
  mkdirSync('../artifacts/test-runs', {recursive:true});
  try {
    const page = await context.newPage();
    let sequence = 20;
    let assetErrors = [];
    page.on('response', response => {
      if (response.request().resourceType() !== 'document' && response.status() >= 400) {
        assetErrors.push({status:response.status(),url:response.url()});
      }
    });
    page.on('requestfailed', request => assetErrors.push({url:request.url(),failure:request.failure()}));
    const routes = ['/', '/uzmanlik-alanlari', '/uzmanlik-alanlari?page=2', '/makaleler', '/sss', '/hakkimda', '/iletisim', '/kvkk', '/cerez-politikasi', '/yasal-uyari'];
    for (const row of seed.filter(r => ['PracticeAreas','Articles'].includes(r.table))) {
      routes.push((row.table === 'PracticeAreas' ? '/uzmanlik-alanlari/' : '/makaleler/') + row.data.Slug);
    }
    for (const width of [320,390,768,1440]) {
      await page.setViewportSize({width,height:900});
      for (const path of routes) {
        // Each navigation models a separate visitor. Do not weaken app rate limits.
        await context.setExtraHTTPHeaders({'X-Test-Connection-IP':`198.18.0.${++sequence}`});
        assetErrors = [];
        const response = await page.goto(base + path);
        assert.equal(response.headers()['x-test-fixture'], 'isolated-cms-seed');
        assert.equal(response.status(),200,path);
        await page.evaluate(() => document.fonts.ready);
        assert.deepEqual(assetErrors,[],`${width}: ${path}: assets must load`);
        assert.ok(await page.evaluate(() => getComputedStyle(document.body).fontFamily.includes('Segoe UI')), 'public CSS must be applied');
        assert.ok(await page.locator('header img').first().evaluate(img => img.complete && img.naturalWidth > 0), 'local emblem must load');
        assert.ok(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1), `${width}: ${path}`);
        assert.ok(!(await page.locator('main').innerText()).includes('CONTENT_REQUIRED'),path);
        if (['/', '/uzmanlik-alanlari', '/makaleler', '/sss', '/hakkimda', '/iletisim'].includes(path)) {
          await page.screenshot({path:`../artifacts/test-runs/cms-seed-${path === '/' ? 'home' : path.slice(1)}-${width}.png`,fullPage:true});
        }
      }
    }
    await page.goto(base + '/iletisim');
    await page.getByText('iletisim@example.invalid', {exact:true}).first().waitFor();
    await page.goto(base + '/sss');
    assert.equal(await page.locator('main details').count(),12);
  } finally {await context.close();await browser.close();}
});
