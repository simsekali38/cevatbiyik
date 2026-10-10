const {test, before, after} = require('node:test');
const assert = require('node:assert/strict');
const {chromium} = require('playwright');
const base = 'https://localhost:57413';
let browser;
let sessionId = 0;
before(async () => { browser = await chromium.launch({channel: process.env.CEVATBIYIK_BROWSER_CHANNEL || 'msedge', headless: true}); });
after(async () => { await browser?.close(); });
async function session(run) {
  const context = await browser.newContext({ignoreHTTPSErrors: true, serviceWorkers: 'block', extraHTTPHeaders: {'X-Test-Connection-IP': '198.18.0.'+(++sessionId)}, viewport: {width: 390, height: 844}});
  const external = [];
  // Real third-party code is NEVER loaded; assert only our consent-controlled loader request.
  await context.route('**/*', route => {
    const url = new URL(route.request().url());
    if (url.origin !== base) { external.push(url.origin); return route.abort(); }
    return route.continue();
  });
  try { await run(await context.newPage(), context, external); } finally { await context.close(); }
}
async function goto(page, path) {
  const response = await page.goto(base + path);
  assert.equal(response.headers()['x-test-fixture'], 'isolated-phase13', 'Refuse any nonfixture target');
  return response;
}
async function clickNavigation(page, locator) {
  await Promise.all([page.waitForNavigation(), locator.click()]);
}
async function login(page) {
  await goto(page, '/admin/login');
  await page.locator('#UserName').fill('admin');
  await page.locator('#Password').fill('Test-Only_Strong9!');
  await clickNavigation(page, page.getByRole('button', {name: 'Giriş yap', exact: true}));
  assert.equal(new URL(page.url()).pathname, '/admin');
}

test('visitor navigates/searches/reads FAQ with mobile layout and real CSP/JSON-LD', async () => session(async (page, context, external) => {
  for (const path of ['/', '/makaleler', '/uzmanlik-alanlari', '/uzmanlik-alanlari/test-area', '/sss', '/iletisim']) {
    const response = await goto(page, path);
    assert.equal(response.status(), 200);
    assert.equal(await page.locator('h1').count(), 1);
    assert.equal(response.headers()['x-frame-options'], 'DENY');
    assert.match(response.headers()['content-security-policy'], /'nonce-/);
    assert.ok(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth));
  }
  await goto(page, '/makaleler');
  await page.locator('#q').fill('browser');
  await clickNavigation(page, page.getByRole('button', {name: 'Ara', exact: true}));
  assert.match(page.url(), /q=browser/);
  await clickNavigation(page, page.locator('h2 a').filter({hasText: 'TEST_ONLY browser article'}));
  assert.match(page.url(), /\/makaleler\/test-browser$/);
  const schemas = await page.locator('script[type="application/ld+json"]').allTextContents();
  assert.ok(schemas.some(x => JSON.stringify(JSON.parse(x)).includes('Article')));
  assert.equal(await page.locator('link[rel=canonical]').getAttribute('href'), 'https://cevatbiyik.av.tr/makaleler/test-browser');
  await goto(page, '/sss');
  await page.getByText('TEST_ONLY question?', {exact:true}).click();
  assert.ok(await page.getByText('TEST_ONLY answer', {exact:true}).isVisible());
  assert.equal(external.length, 0);
}));

test('consent opt-in/withdrawal, successful contact PRG and no pre-consent tracking', async () => session(async (page, context, external) => {
  await goto(page, '/'); assert.equal(external.length, 0);
  await clickNavigation(page, page.getByRole('button', {name: 'Analitiği reddet', exact:true}));
  assert.equal(await page.locator('script[src*="analytics.js"]').count(), 0);
  await page.locator('#cookie-heading').click();
  await clickNavigation(page, page.getByRole('button', {name: 'Analitiğe izin ver', exact:true}));
  await page.waitForFunction(() => !!window.dataLayer);
  assert.equal(await page.locator('script[src*="analytics.js"]').count(), 1);
  assert.ok(external.includes('https://www.googletagmanager.com'));
  const consent = (await context.cookies()).find(x => x.name === '__Host-CevatBiyik.Consent');
  assert.ok(consent.secure && consent.httpOnly && consent.sameSite === 'Strict');
  await goto(page, '/iletisim');
  await page.locator('#Form_Name').fill('TEST_ONLY browser visitor');
  await page.locator('#Form_Email').fill('test@example.test');
  await page.locator('#Form_Message').fill('TEST_ONLY browser contact');
  await page.locator('#Form_Consent').check();
  await clickNavigation(page, page.getByRole('button', {name: /Mesajı gönder/}));
  assert.ok(await page.locator('#contact-success').isVisible());
  assert.equal(await page.locator('#contact-success').evaluate(x => document.activeElement === x), true);
  await goto(page, '/iletisim'); assert.equal(await page.locator('#contact-success').count(), 0);
  await page.locator('#cookie-heading').click();
  await clickNavigation(page, page.getByRole('button', {name: 'Analitiği reddet', exact:true}));
  assert.equal(await page.locator('script[src*="analytics.js"]').count(), 0);
}));

test('admin logs in, creates/publishes article, views CMS modules and logs out', async () => session(async (page, context) => {
  await login(page);
  const admin = (await context.cookies()).find(x => x.name === '__Host-CevatBiyik.Admin');
  assert.ok(admin.secure && admin.httpOnly && admin.sameSite === 'Strict' && admin.path === '/');
  await goto(page, '/admin/articles/create');
  await page.locator('#Title').fill('TEST_ONLY browser created article');
  await page.locator('#Content').fill('TEST_ONLY browser-created text');
  await page.locator('#PublishedAt').fill('2020-01-01T12:00');
  await page.locator('#IsActive').check();
  await clickNavigation(page, page.getByRole('button', {name:'Kaydet', exact:true}));
  assert.equal((await goto(page, '/makaleler/test-only-browser-created-article')).status(), 200);
  for (const path of ['/admin/taxonomy/categories', '/admin/media', '/admin/documents', '/admin/contact-messages', '/admin/redirects', '/admin/locations', '/admin/page-seo']) {
    assert.equal((await goto(page,path)).status(), 200);
  }
  await clickNavigation(page, page.getByRole('button', {name:/Çıkış/}));
  await goto(page, '/admin/documents/download/1');
  assert.equal(new URL(page.url()).pathname, '/admin/login');
}));

test('anonymous document denial, CSRF rejection, robots/sitemaps and login rate limiting', async () => session(async (page, context) => {
  await goto(page, '/admin/documents/download/1');
  assert.equal(new URL(page.url()).pathname, '/admin/login');
  const csrf = await context.request.post(base+'/iletisim', {form:{'Form.Name':'TEST_ONLY'}});
  assert.equal(csrf.status(), 400);
  const robots = await context.request.get(base+'/robots.txt');
  assert.match(await robots.text(), /Disallow: \//);
  const sitemap = await context.request.get(base+'/sitemap.xml');
  assert.equal(sitemap.status(), 200); assert.match(await sitemap.text(), /<urlset[^>]*\/\s*>/);
  const html = await context.request.get(base+'/makaleler');
  assert.match(html.headers()['x-robots-tag'], /noindex/);
  let response;
  for(let i=0;i<11;i++) { response = await context.request.post(base+'/admin/login', {form:{UserName:'missing'}}); }
  assert.equal(response.status(),429); assert.equal(response.headers()['retry-after'],'60');
}));
