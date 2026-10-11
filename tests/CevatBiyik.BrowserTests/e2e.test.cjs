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
  assert.equal(await page.locator('script[src*="analytics.min.js"]').count(), 0);
  await page.locator('#cookie-heading').click();
  await clickNavigation(page, page.getByRole('button', {name: 'Analitiğe izin ver', exact:true}));
  await page.waitForFunction(() => !!window.dataLayer);
  assert.equal(await page.locator('script[src*="analytics.min.js"]').count(), 1);
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
  assert.equal(await page.locator('script[src*="analytics.min.js"]').count(), 0);
}));

test('admin logs in, creates/publishes article, views CMS modules and logs out', async () => session(async (page, context) => {
  await login(page);
  const admin = (await context.cookies()).find(x => x.name === '__Host-CevatBiyik.Admin');
  assert.ok(admin.secure && admin.httpOnly && admin.sameSite === 'Strict' && admin.path === '/');
  await goto(page, '/admin/articles/create');
  await page.locator('#Title').fill('TEST_ONLY browser created article');
  await page.locator('#Content-editor').fill('TEST_ONLY browser-created text');
  await page.locator('#Content-editor').press('Control+a');
  await page.getByRole('button', {name:'Kalın', exact:true}).click();
  await page.getByRole('button', {name:'Ortala', exact:true}).click();
  await page.locator('#PublishedAt').fill('2020-01-01T12:00');
  await page.locator('#IsActive').check();
  await clickNavigation(page, page.getByRole('button', {name:'Değişiklikleri kaydet', exact:true}));
  assert.equal((await goto(page, '/makaleler/test-only-browser-created-article')).status(), 200);
  assert.equal(await page.locator('.rich-content .content-align-center').count(), 1);
  assert.equal((await page.locator('.rich-content strong').textContent()).replace(/\s+/gu, ' '), 'TEST_ONLY browser-created text');
  for (const path of ['/admin/taxonomy/categories', '/admin/media', '/admin/contact-messages', '/admin/redirects', '/admin/locations', '/admin/page-seo']) {
    assert.equal((await goto(page,path)).status(), 200);
  }
  await clickNavigation(page, page.getByRole('button', {name:/Çıkış/}));
  assert.equal((await goto(page, '/admin/documents/download/1')).status(), 404);
}));

test('anonymous document denial, CSRF rejection, robots/sitemaps and login rate limiting', async () => session(async (page, context) => {
  assert.equal((await goto(page, '/admin/documents/download/1')).status(), 404);
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


test('rich editor mobile navigation, keyboard exit, empty validation and safe link dialog', async () => session(async (page) => {
  const failures = []; page.on('pageerror', error => failures.push(error.message));
  await page.addInitScript(() => { window.testCspViolations = []; document.addEventListener('securitypolicyviolation', e => window.testCspViolations.push({directive:e.violatedDirective,source:e.sourceFile,line:e.lineNumber,sample:e.sample})); });
  await login(page);
  await page.getByRole('button', {name:'Yönetim menüsünü aç', exact:true}).click();
  assert.ok(await page.locator('#admin-navigation').isVisible());
  await page.locator('#admin-navigation button[data-bs-dismiss]').click();
  await goto(page, '/admin/articles/create');
  assert.ok(await page.locator('#Content-editor').isVisible());
  assert.ok(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth));
  await page.locator('#Title').fill('TEST_ONLY empty editor');
  assert.equal(await page.locator('[data-seo-preview-title]').textContent(), 'TEST_ONLY empty editor');
  await page.getByRole('button', {name:'Değişiklikleri kaydet', exact:true}).click();
  assert.match(await page.locator('.field-error[role=alert]:visible').textContent(), /doldurun/);
  await page.locator('#Content-editor').fill('TEST_ONLY link text');
  await page.locator('#Content-editor').press('Control+a');
  await page.getByRole('button', {name:'Bağlantı', exact:true}).click();
  await page.locator('dialog[open] input').fill('javascript:alert(1)');
  await page.getByRole('button', {name:'Bağlantıyı ekle', exact:true}).click();
  assert.match(await page.locator('dialog[open] [role=alert]').textContent(), /HTTPS/);
  await page.locator('dialog[open] input').fill('/makaleler');
  await page.getByRole('button', {name:'Bağlantıyı ekle', exact:true}).click();
  assert.match(await page.locator('#Content').inputValue(), /href="\/makaleler"/);
  await page.locator('#Content-editor').press('Tab');
  assert.equal(await page.locator('#Content-editor').evaluate(x => x.contains(document.activeElement)), false);
  assert.equal(await page.locator('.admin-editor-menubar summary').count(), 6);
  await page.getByRole('button', {name:'Önizleme', exact:true}).click();
  assert.equal(await page.locator('dialog[open] .rich-content a').getAttribute('href'), '/makaleler');
  await page.getByRole('button', {name:'Kapat', exact:true}).click();
  await page.getByRole('button', {name:'Kaynak görünümü', exact:true}).click();
  assert.match(await page.getByRole('textbox', {name:'Güvenli HTML kaynağı'}).inputValue(), /href="\/makaleler"/);
  await page.getByRole('button', {name:'Kapat', exact:true}).click();
  await page.locator('.admin-editor-menubar summary').filter({hasText:'Düzen'}).click();
  assert.ok(await page.getByRole('button', {name:'Tümünü seç', exact:true}).isVisible());
  await page.getByRole('button', {name:'Tümünü seç', exact:true}).click();
  await page.locator('.admin-editor-menubar summary').filter({hasText:'Araçlar'}).click();
  await page.locator('.admin-editor-menu[open] summary').press('Escape');
  assert.equal(await page.locator('.admin-editor-menu[open]').count(), 0);
  assert.deepEqual(failures, []);
  assert.deepEqual(await page.evaluate(() => window.testCspViolations), []);
  assert.equal(await page.evaluate(() => { const event = new Event('beforeunload', {cancelable:true}); window.dispatchEvent(event); return event.defaultPrevented; }), true);
  await page.evaluate(() => { window.getSelection().removeAllRanges(); document.activeElement.blur(); window.scrollTo(0,0); });
  await page.screenshot({path:'artifacts/test-runs/admin-editor-mobile.png', fullPage:true});
  await page.setViewportSize({width:1440,height:1000});
  await page.evaluate(() => window.scrollTo(0,0));
  await page.screenshot({path:'artifacts/test-runs/admin-editor-desktop.png', fullPage:true});
}));


test('admin form and navigation stay usable without JavaScript', async () => {
  const context = await browser.newContext({ignoreHTTPSErrors:true, javaScriptEnabled:false, extraHTTPHeaders:{'X-Test-Connection-IP':'198.18.0.'+(++sessionId)}, viewport:{width:390,height:844}});
  try {
    const page = await context.newPage(); await login(page); await goto(page,'/admin/articles/create');
    assert.ok(await page.locator('#Content').isVisible());
    assert.ok(await page.locator('#admin-navigation a[href="/admin/articles"]').isVisible());
    assert.ok(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth));
  } finally { await context.close(); }
});
