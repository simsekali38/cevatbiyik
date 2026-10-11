// Build-only dependencies are locked in BrowserTests; no Node runtime is required on Plesk.
const fs = require('node:fs/promises');
const path = require('node:path');
const root = path.resolve(__dirname, '..');
const dependencies = path.join(root, 'tests/CevatBiyik.BrowserTests');
const CleanCSS = require(require.resolve('clean-css', {paths: [dependencies]}));
const terser = require(require.resolve('terser', {paths: [dependencies]}));
(async () => {
  for (const relative of ['css/site.css', 'css/admin.css', 'js/site.js', 'js/admin.js', 'js/analytics.js']) {
    const source = path.join(root, 'src/CevatBiyik.Web/wwwroot', relative);
    const text = (await fs.readFile(source, 'utf8')).replace(/\r\n/g, '\n');
    const result = relative.endsWith('.css')
      ? new CleanCSS({level: 1}).minify(text)
      : await terser.minify(text, {compress: true, mangle: true, format: {comments: false}});
    if (result.errors?.length) throw new Error(result.errors.join('\n'));
    const output = (result.styles ?? result.code) + '\n';
    const target = source.replace(/\.(css|js)$/, '.min.$1');
    if (process.argv.includes('--check')) {
      if ((await fs.readFile(target, 'utf8')).replace(/\r\n/g, '\n') !== output) throw new Error(`Stale generated asset: ${relative}`);
    } else await fs.writeFile(target, output);
    console.log(`${relative}: ${Buffer.byteLength(text)} -> ${Buffer.byteLength(output)} bytes`);
  }
})().catch(error => {console.error(error.message); process.exitCode = 1;});
