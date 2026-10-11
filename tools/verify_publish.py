"""Validate a local release without reading server-owned secrets or deploying anything."""
import json
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

def verify(root):
    errors = []
    required = ['CevatBiyik.Web.dll', 'CevatBiyik.Web.runtimeconfig.json', 'web.config',
                'wwwroot/uploads/web.config', 'wwwroot/css/admin.min.css', 'wwwroot/js/admin.min.js']
    for relative in required:
        if not (root / relative).is_file():
            errors.append('missing required artifact: ' + relative)
    for file in root.rglob('*'):
        relative = file.relative_to(root).as_posix()
        if file.is_symlink():
            errors.append('unexpected symlink: ' + relative)
        if not file.is_file():
            continue
        if (file.name.lower().startswith(('appsettings', '.env')) or file.name.lower() == 'secrets.json' or file.suffix.lower() in {'.sql', '.pfx', '.pem', '.key'}
                or any(part.lower() in {'app_data', 'node_modules', 'testresults', 'docs', '.git', 'private-files'} for part in file.parts)
                or (relative.startswith('wwwroot/uploads/') and relative != 'wwwroot/uploads/web.config')):
            errors.append('forbidden release content: ' + relative)
    if not (root / 'runtimes/win-x64/native/libSkiaSharp.dll').is_file():
        errors.append('missing Windows native Skia library')
    if (root / 'web.config').is_file():
        tree = ET.parse(root / 'web.config')
        ancm = tree.find('.//aspNetCore')
        if ancm is None or ancm.get('hostingModel') != 'outofprocess' or ancm.get('stdoutLogEnabled') != 'false':
            errors.append('invalid ANCM hosting/logging settings')
        limit = tree.find('.//requestLimits')
        if limit is None or limit.get('maxAllowedContentLength') != '6291456':
            errors.append('invalid IIS request limit')
        env = tree.find('.//environmentVariable[@name="ASPNETCORE_ENVIRONMENT"]')
        if env is None or env.get('value') != 'Production':
            errors.append('missing Production environment')
    if (root / 'CevatBiyik.Web.runtimeconfig.json').is_file():
        runtime = json.loads((root / 'CevatBiyik.Web.runtimeconfig.json').read_text(encoding='utf-8'))['runtimeOptions']
        if runtime.get('tfm') != 'net8.0':
            errors.append('unexpected target framework')
    return errors

if __name__ == '__main__':
    results = verify(Path(sys.argv[1]).resolve())
    for result in results:
        print(result)
    print('Publish verification: ' + ('FAILED' if results else 'passed'))
    sys.exit(bool(results))
