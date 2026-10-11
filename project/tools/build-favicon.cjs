// Faithful rasterization of the local SVG using existing build-only runtimes.
const fs = require('node:fs/promises');
const path = require('node:path');
const os = require('node:os');
const {execFile} = require('node:child_process');
const {promisify} = require('node:util');
const root = path.resolve(__dirname, '..');
const {chromium} = require(require.resolve('playwright', {paths:[path.join(root, 'tests/CevatBiyik.BrowserTests')]}));

(async () => {
  const scratch = await fs.mkdtemp(path.join(os.tmpdir(), 'cevat-favicon-'));
  let browser;
  try {
    const svg = await fs.readFile(path.join(root, 'src/CevatBiyik.Web/wwwroot/images/brand/cevat-biyik-emblem.svg'), 'utf8');
    browser = await chromium.launch({channel:'msedge',headless:true});
    const page = await browser.newPage({viewport:{width:512,height:512},deviceScaleFactor:1});
    await page.setContent('<!doctype html><html><head><style>html,body{margin:0;width:512px;height:512px;background:transparent}svg{display:block;width:512px;height:512px}</style></head><body>'+svg+'</body></html>');
    const master = path.join(scratch,'master.png');
    await page.screenshot({path:master,omitBackground:true});
    const result = await promisify(execFile)('python',[path.join(__dirname,'build-favicon.py'),master,path.join(root,'src/CevatBiyik.Web/wwwroot')]);
    process.stdout.write(result.stdout);
  } finally {
    await browser?.close();
    const absolute = path.resolve(scratch);
    if (path.dirname(absolute) !== path.resolve(os.tmpdir()) || !path.basename(absolute).startsWith('cevat-favicon-')) throw new Error('Unexpected temporary directory');
    await fs.rm(absolute,{recursive:true,force:true});
  }
})().catch(error => {console.error(error.message);process.exitCode=1;});
