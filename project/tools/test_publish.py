"""Negative release-boundary checks with empty synthetic files only."""
import json
import tempfile
import unittest
from pathlib import Path
from verify_publish import verify

SCRATCH = Path(__file__).resolve().parents[1] / 'artifacts' / 'publish-tests'
SCRATCH.mkdir(parents=True, exist_ok=True)

class PublishBoundaryTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(dir=SCRATCH)
        self.root = Path(self.temp.name)
        for name in ['CevatBiyik.Web.dll', 'wwwroot/uploads/web.config', 'wwwroot/css/admin.min.css',
                     'wwwroot/js/admin.min.js', 'runtimes/win-x64/native/libSkiaSharp.dll']:
            self.write(name, '')
        self.write('web.config', '<configuration><aspNetCore hostingModel="outofprocess" stdoutLogEnabled="false"><environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" /></aspNetCore><requestLimits maxAllowedContentLength="6291456" /></configuration>')
        self.write('CevatBiyik.Web.runtimeconfig.json', json.dumps({'runtimeOptions': {'tfm': 'net8.0'}}))

    def tearDown(self):
        self.temp.cleanup()

    def write(self, relative, content):
        file = self.root / relative
        file.parent.mkdir(parents=True, exist_ok=True)
        file.write_text(content, encoding='utf-8')

    def test_clean_release_is_accepted(self):
        self.assertEqual(verify(self.root), [])

    def test_server_config_and_user_files_are_rejected(self):
        for relative in ['appsettings.json', 'appsettings.Production.json', 'secrets.json', '.env',
                         'App_Data/private-files/a.pdf', 'wwwroot/uploads/images/a.webp', 'demo.sql']:
            self.write(relative, '')
        self.assertEqual(sum('forbidden release content' in item for item in verify(self.root)), 7)

    def test_invalid_iis_envelope_and_development_mode_are_rejected(self):
        self.write('web.config', '<configuration><aspNetCore hostingModel="inprocess" stdoutLogEnabled="true" /><requestLimits maxAllowedContentLength="99999999" /></configuration>')
        self.assertEqual(len(verify(self.root)), 3)

    def test_missing_native_windows_runtime_is_rejected(self):
        (self.root / 'runtimes/win-x64/native/libSkiaSharp.dll').unlink()
        self.assertIn('missing Windows native Skia library', verify(self.root))

if __name__ == '__main__':
    unittest.main()
