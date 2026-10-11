const {test, before, after} = require('node:test');
const assert = require('node:assert/strict');
const {chromium} = require('playwright');
const AxeBuilder = require('@axe-core/playwright').default;
const {writeFile, mkdir} = require('node:fs/promises');
const base = 'https://localhost:57413';
let browser, sequence = 180;
const manualReview = [];
before(async () => {await mkdir('artifacts/test-runs', {recursive:true}); browser = await chromium.launch({channel: 'msedge', headless: true});});
after(async () => {await browser?.close(); await writeFile('artifacts/test-runs/phase14-a11y-review.json', JSON.stringify(manualReview, null, 2));});
async function session(run) {
  const context = await browser.newContext({ignoreHTTPSErrors: true, viewport: {width: 390, height: 844}, reducedMotion: 'reduce', extraHTTPHeaders: {'X-Test-Connection-IP': `198.18.0.${++sequence}`}});
  await context.route('**/*', route => new URL(route.request().url()).origin === base ? route.continue() : route.abort());
  try {await run(await context.newPage());} finally {await context.close();}
}
async function goto(page, path) {
  const response = await page.goto(base + path);
  assert.equal(response.headers()['x-test-fixture'], 'isolated-phase13'); assert.equal(response.status(), 200);
}
async function audit(page) {
  const result = await new AxeBuilder({page}).withTags(['wcag2a', 'wcag2aa', 'wcag21aa', 'wcag22aa']).analyze();
  assert.deepEqual(result.violations.map(v => ({id:v.id, nodes:v.nodes.map(n=>n.target)})), []);
  for (const check of result.incomplete) for (const node of check.nodes) {
    const styles = await page.locator(node.target[0]).evaluate(el => {
      const layers = []; for (let current = el; current; current = current.parentElement) {
        const style = getComputedStyle(current);
        layers.push({element:current.tagName, color:style.color, background:style.backgroundColor, image:style.backgroundImage});
      }
      return {text:el.textContent.slice(0,80), layers};
    });
    manualReview.push({path:new URL(page.url()).pathname, id:check.id, target:node.target, reason:node.failureSummary, styles});
  }
}
async function login(page) {
  await goto(page, '/admin/login');
  await page.locator('#UserName').fill('admin'); await page.locator('#Password').fill('Test-Only_Strong9!');
  await Promise.all([page.waitForNavigation(), page.getByRole('button', {name: 'Giriş yap', exact:true}).click()]);
}
test('public keyboard skip link, visible focus and mobile menu work without a mouse', async () => session(async page => {
  await goto(page, '/');
  await page.keyboard.press('Tab');
  assert.equal(await page.evaluate(() => document.activeElement.getAttribute('href')), '#icerik');
  assert.equal(await page.evaluate(() => getComputedStyle(document.activeElement).outlineStyle), 'solid');
  await page.keyboard.press('Enter');
  assert.equal(await page.evaluate(() => document.activeElement.id), 'icerik');
  await goto(page, '/');
  for (let i = 0; i < 10; i++) {
    await page.keyboard.press('Tab');
    if (await page.evaluate(() => document.activeElement.classList.contains('navbar-toggler'))) break;
  }
  assert.equal(await page.evaluate(() => document.activeElement.classList.contains('navbar-toggler')), true);
  await page.keyboard.press('Enter');
  assert.equal(await page.locator('.navbar-toggler').getAttribute('aria-expanded'), 'true');
  await audit(page);
}));
test('public pages reflow at 320px and 390px, and expose native labels and AA semantics', async () => session(async page => {
  for (const path of ['/', '/makaleler', '/makaleler/test-browser', '/sss', '/iletisim']) {
    await goto(page, path);
    for (const width of [320, 390]) {
      await page.setViewportSize({width, height:844});
      assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), true, `${path} ${width}px`);
    }
    assert.equal(await page.locator('main').count(), 1); assert.equal(await page.locator('h1').count(), 1);
    await audit(page);
  }
  assert.equal(await page.locator('#Form_Email').evaluate(el => !!el.labels?.length), true);
}));
test('admin loads editor only on editing screens and keyboard focus clears the sticky header', async () => session(async page => {
  await login(page);
  assert.equal(await page.locator('script[src*="quill.js"]').count(), 0);
  await goto(page, '/admin/articles/create');
  assert.equal(await page.locator('script[src*="quill.js"]').count(), 1);
  await page.locator('#Title').focus();
  assert.equal(await page.evaluate(() => document.activeElement.id), 'Title');
  await page.keyboard.press('Tab');
  assert.equal(await page.evaluate(() => document.activeElement.id), 'Excerpt');
  const visible = await page.evaluate(() => {
    const active = document.activeElement.getBoundingClientRect();
    return active.top >= document.querySelector('.admin-header').getBoundingClientRect().bottom && active.bottom <= innerHeight;
  });
  assert.equal(visible, true);
  await audit(page);
  await page.screenshot({path:'artifacts/test-runs/phase14-editor-mobile.png', fullPage:true});
}));
test('editor dialogs meet AA checks, trap focus natively and return focus on Escape', async () => session(async page => {
  await login(page); await goto(page, '/admin/articles/create');
  await page.locator('#Content-editor').fill('TEST_ONLY accessible link');
  await page.locator('#Content-editor').press('Control+a');
  await page.getByRole('button', {name:'Bağlantı', exact:true}).click();
  await audit(page);
  await page.screenshot({path:'artifacts/test-runs/phase14-link-modal-mobile.png', fullPage:true});
  await page.locator('dialog[open] input').focus();
  await page.keyboard.press('Shift+Tab');
  assert.equal(await page.evaluate(() => !!document.activeElement.closest('dialog[open]')), true);
  await page.keyboard.press('Escape');
  assert.equal(await page.locator('dialog[open]').count(), 0);
  assert.equal(await page.evaluate(() => document.activeElement.id), 'Content-editor');
  await page.getByRole('button', {name:'Önizleme', exact:true}).click();
  await audit(page);
  await page.keyboard.press('Escape');
  assert.equal(await page.evaluate(() => document.activeElement.id), 'Content-editor');
  await page.emulateMedia({forcedColors:'active'});
  await page.locator('#Title').focus();
  await page.keyboard.press('Tab');
  assert.equal(await page.evaluate(() => getComputedStyle(document.activeElement).outlineStyle), 'solid');
}));
