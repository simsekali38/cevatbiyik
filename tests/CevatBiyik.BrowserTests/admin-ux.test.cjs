const {test, before, after} = require('node:test');
const assert = require('node:assert/strict');
const {chromium} = require('playwright');
const AxeBuilder = require('@axe-core/playwright').default;
const base = 'https://localhost:57413';
let browser, number = 210;
before(async () => {browser = await chromium.launch({channel:'msedge',headless:true});});
after(async () => {await browser?.close();});
async function session(run) {
  const context = await browser.newContext({ignoreHTTPSErrors:true, viewport:{width:390,height:844},extraHTTPHeaders:{'X-Test-Connection-IP':`198.18.0.${++number}`}});
  await context.route('**/*', route => new URL(route.request().url()).origin===base ? route.continue() : route.abort());
  const page = await context.newPage(); page.on('dialog', dialog => dialog.accept());
  try {await run(page);} finally {await context.close();}
}
async function goto(page,path) {
  const res=await page.goto(base+path); assert.equal(res.headers()['x-test-fixture'],'isolated-phase13'); assert.equal(res.status(),200);
}
async function navigate(page,action) {await Promise.all([page.waitForNavigation(),action()]);}
async function login(page) {
  await goto(page,'/admin/login'); await page.locator('#UserName').fill('admin'); await page.locator('#Password').fill('Test-Only_Strong9!');
  await navigate(page,()=>page.getByRole('button',{name:'Giriş yap',exact:true}).click());
}
async function axe(page) {
  const r=await new AxeBuilder({page}).withTags(['wcag2a','wcag2aa','wcag21aa','wcag22aa']).analyze();
  assert.deepEqual(r.violations.map(x=>({id:x.id,nodes:x.nodes.map(n=>n.target)})),[]);
}
test('all six rich-edit screens allow typing into ordinary inputs without stealing focus',async()=>session(async page=>{
  await login(page);
  for (const [path,id] of [['/admin/articles/create','Title'],['/admin/practice-areas/create','Name'],['/admin/profile','BarAssociation'],['/admin/page-contents/edit','Title'],['/admin/faq/edit','Question'],['/admin/locations/edit','Name']]) {
    await goto(page,path); const editor=page.locator('.ql-editor').first(); await editor.fill('TEST_ONLY rich body');
    const field=page.locator('#'+id); await field.fill('TEST_ONLY editable field');
    assert.equal(await page.evaluate(()=>document.activeElement.id),id,path);
    await page.keyboard.type(' typed'); assert.equal(await field.inputValue(),'TEST_ONLY editable field typed');
    await field.press('Tab'); assert.notEqual(await page.evaluate(()=>document.activeElement.id),await editor.getAttribute('id'),path);
    assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true,path);
  }
}));
test('HTML source applies supported formats and rejects executable markup without losing content',async()=>session(async page=>{
  await login(page);await goto(page,'/admin/articles/create');await page.locator('#Title').fill('TEST_ONLY HTML source article');
  await page.locator('#Content-editor').fill('TEST_ONLY original');
  const original = await page.locator('#Content').inputValue();
  await page.getByRole('button',{name:'Kaynak görünümü',exact:true}).click();
  const source=page.getByRole('textbox',{name:'Güvenli HTML kaynağı'});
  await source.fill('<p onclick="alert(1)">unsafe</p>');await page.getByRole('button',{name:'HTML değişikliklerini uygula',exact:true}).click();
  assert.match(await page.locator('dialog[open] [role=alert]').textContent(),/değiştirilmedi/);
  assert.equal(await page.locator('#Content').inputValue(),original);
  await source.fill('<h4>TEST_ONLY heading</h4><p><s>Old</s> H<sub>2</sub>O x<sup>2</sup> <strong>safe</strong></p>');
  await page.getByRole('button',{name:'HTML değişikliklerini uygula',exact:true}).click();
  assert.equal(await page.locator('dialog[open]').count(),0);
  await page.locator('#IsActive').uncheck();
  await navigate(page,()=>page.getByRole('button',{name:'Değişiklikleri kaydet',exact:true}).click());
  assert.equal(new URL(page.url()).pathname,'/admin/articles');
  const row=page.locator('tr').filter({hasText:'TEST_ONLY HTML source article'});
  await navigate(page,()=>row.getByRole('link',{name:'Düzenle',exact:true}).click());
  assert.match(await page.locator('#Content').inputValue(),/<h4>.*<s>.*<sub>.*<sup>/s);
}));
test('row actions sit together, cancel makes no change and confirmed delete sends protected POST',async()=>session(async page=>{
  await login(page);await goto(page,'/admin/articles');
  const row=page.locator('tr').filter({hasText:'TEST_ONLY browser article'});
  assert.equal(await row.locator('a.admin-action svg').count(),1);
  const remove=row.getByRole('button',{name:'Sil',exact:true});
  const editBox=await row.getByRole('link',{name:'Düzenle',exact:true}).boundingBox();const deleteBox=await remove.boundingBox();
  assert.ok(Math.abs(editBox.y-deleteBox.y)<5);
  await remove.click();assert.match(await page.locator('.admin-confirm-record').textContent(),/TEST_ONLY browser article/);
  await axe(page);await page.getByRole('button',{name:'Vazgeç',exact:true}).click();assert.equal(await row.count(),1);
  await remove.click();await navigate(page,()=>page.locator('.admin-confirm-dialog').getByRole('button',{name:'Sil',exact:true}).click());
  assert.equal(await page.locator('tr').filter({hasText:'TEST_ONLY browser article'}).count(),0);
}));
test('guided setup without private-document links are usable on desktop and mobile',async()=>session(async page=>{
  await login(page);await goto(page,'/admin/guide');assert.equal(await page.locator('.admin-guide-steps li').count(),8);
  await axe(page);await page.screenshot({path:'artifacts/test-runs/admin-guide-mobile.png',fullPage:true});
  await page.setViewportSize({width:1440,height:1000});await page.screenshot({path:'artifacts/test-runs/admin-guide-desktop.png',fullPage:true});
  await goto(page,'/admin/guide?step=6');assert.match(await page.locator('.admin-guide-card').first().textContent(),/taslak/);
  assert.equal(await page.locator('a[href^="/admin/documents"]').count(),0);assert.equal((await page.goto(base+'/admin/documents')).status(),404);
}));
test('contact privacy modal keeps the URL and typed form values and returns focus',async()=>session(async page=>{
  await goto(page,'/iletisim');await page.locator('#Form_Name').fill('TEST_ONLY modal visitor');
  await page.locator('#Form_Message').fill('TEST_ONLY unsent message');const opener=page.locator('form [data-open-dialog="kvkk-notice"]');
  await opener.click();assert.equal(new URL(page.url()).pathname,'/iletisim');
  assert.match(await page.locator('#kvkk-notice').textContent(),/TEST_ONLY/);await axe(page);
  await page.screenshot({path:'artifacts/test-runs/contact-kvkk-mobile.png',fullPage:true});await page.keyboard.press('Escape');
  assert.equal(await page.locator('#Form_Name').inputValue(),'TEST_ONLY modal visitor');assert.equal(await page.locator('#Form_Message').inputValue(),'TEST_ONLY unsent message');
  assert.equal(await opener.evaluate(x=>document.activeElement===x),true);
  const bannerLink = page.locator('[data-consent-state] a[href="/kvkk"]');
  await bannerLink.click();assert.equal(new URL(page.url()).pathname,'/iletisim');
  await page.locator('#kvkk-notice [data-close-dialog]').click();
  assert.equal(await bannerLink.evaluate(x=>document.activeElement===x),true);
  assert.equal(await page.locator('#Form_Message').inputValue(),'TEST_ONLY unsent message');
}));
