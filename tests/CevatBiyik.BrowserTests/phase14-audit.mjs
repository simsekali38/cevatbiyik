import lighthouse from 'lighthouse';
import {launch} from 'chrome-launcher';
import {chromium} from 'playwright';
import AxeBuilder from '@axe-core/playwright';
import {mkdir, writeFile} from 'node:fs/promises';
import https from 'node:https';
const base = 'https://localhost:57413';
const stage = process.argv[2];
if (!['baseline', 'after'].includes(stage)) throw new Error('Use baseline or after.');
await new Promise((resolve, reject) => {
  const req = https.get(base, {rejectUnauthorized: false}, res => {
    res.resume();
    if (res.statusCode === 200 && res.headers['x-test-fixture'] === 'isolated-phase13') resolve();
    else reject(new Error('Refusing nonfixture target.'));
  });
  req.on('error', reject); req.setTimeout(2000, () => req.destroy());
});
const folder = `artifacts/phase14/${stage}`;
await mkdir(folder, {recursive: true});
const result = {stage, measuredAt: new Date().toISOString(), environment: 'Loopback HTTPS synthetic fixture; simulated mobile; no field INP', lighthouse: [], accessibility: []};
let sequence = 30;
const browser = await chromium.launch({channel: 'msedge', headless: true});
try {
  for (const path of ['/', '/hakkimda', '/makaleler', '/makaleler/test-browser', '/uzmanlik-alanlari', '/uzmanlik-alanlari/test-area', '/sss', '/iletisim', '/kvkk', '/cerez-politikasi', '/yasal-uyari', '/konumlar', '/admin/login', '/admin', '/admin/articles/create', '/admin/profile', '/admin/faq', '/admin/media', '/admin/settings']) {
    const context = await browser.newContext({ignoreHTTPSErrors: true, viewport: {width: 390, height: 844}, extraHTTPHeaders: {'X-Test-Connection-IP': `198.18.0.${++sequence}`}});
    await context.route('**/*', route => new URL(route.request().url()).origin === base ? route.continue() : route.abort());
    const page = await context.newPage();
    if (path.startsWith('/admin') && path !== '/admin/login') {
      await page.goto(base + '/admin/login');
      await page.locator('#UserName').fill('admin'); await page.locator('#Password').fill('Test-Only_Strong9!');
      await Promise.all([page.waitForNavigation(), page.getByRole('button', {name: 'Giriş yap', exact: true}).click()]);
    }
    const response = await page.goto(base + path);
    if (response.headers()['x-test-fixture'] !== 'isolated-phase13' || response.status() !== 200) throw new Error(`Unexpected fixture route: ${path} (${response.status()})`);
    const axe = await new AxeBuilder({page}).withTags(['wcag2a', 'wcag2aa', 'wcag21aa', 'wcag22aa']).analyze();
    const audit = {path, violations: axe.violations.map(v => ({id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.map(n => ({target: n.target, failureSummary: n.failureSummary}))})), incomplete: axe.incomplete.map(v => v.id)};
    result.accessibility.push(audit);
    console.log(`axe ${path}: ${audit.violations.length} violations`);
    await context.close();
  }
} finally { await browser.close(); }
const chrome = await launch({chromePath: 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe', chromeFlags: ['--headless=new', '--ignore-certificate-errors']});
try {
  for (const path of ['/', '/makaleler', '/makaleler/test-browser', '/iletisim']) {
    const runs = [];
    for (let iteration = 1; iteration <= 3; iteration++) {
      const name = (path === '/' ? 'home' : path.slice(1).replaceAll('/', '-')) + `-${iteration}`;
      const report = await lighthouse(base + path, {port: chrome.port, output: ['json', 'html'], onlyCategories: ['performance', 'accessibility'], extraHeaders: {'X-Test-Connection-IP': `198.18.0.${++sequence}`}, blockedUrlPatterns: ['*googletagmanager.com*', '*google-analytics.com*']});
      await writeFile(`${folder}/${name}.json`, report.report[0]); await writeFile(`${folder}/${name}.html`, report.report[1]);
      const audits = report.lhr.audits;
      result.toolchain = {lighthouse: report.lhr.lighthouseVersion, browser: report.lhr.environment.hostUserAgent, throttling: report.lhr.configSettings.throttling, screen: report.lhr.configSettings.screenEmulation};
      const metrics = {performance: report.lhr.categories.performance.score * 100, accessibility: report.lhr.categories.accessibility.score * 100, lcp: audits['largest-contentful-paint'].numericValue, cls: audits['cumulative-layout-shift'].numericValue, tbt: audits['total-blocking-time'].numericValue, bytes: audits['total-byte-weight'].numericValue, fcp: audits['first-contentful-paint'].numericValue};
      runs.push(metrics); console.log(`Lighthouse ${path} ${iteration}: ${JSON.stringify(metrics)}`);
    }
    const median = Object.fromEntries(Object.keys(runs[0]).map(key => [key, runs.map(r => r[key]).sort((a,b) => a-b)[1]]));
    result.lighthouse.push({path, runs, median});
  }
} finally { await chrome.kill(); }
await writeFile(`${folder}/summary.json`, JSON.stringify(result, null, 2));
if (stage === 'after' && result.accessibility.some(a => a.violations.length)) process.exitCode = 1;
if (stage === 'after' && result.lighthouse.some(a => a.median.performance < 90 || a.median.lcp > 2500 || a.median.cls > 0.1 || a.median.tbt > 200)) process.exitCode = 1;
