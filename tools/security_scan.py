"""Dependency-free, redacted heuristic scan. Does not replace provider/entropy tooling or human review."""
import argparse
import json
import re
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
TEXT = {'.cs', '.cshtml', '.json', '.js', '.cjs', '.xml', '.config', '.props', '.targets', '.md', '.ps1', '.py', '.yml', '.yaml', '.txt', '.env', '.pem', '.key', '.sql', '.ini', '.sh', '.cmd', '.bat', '.http'}
PATTERNS = {
    'provider-token': re.compile(r'(?<![A-Za-z0-9])(?:AKIA[0-9A-Z]{16}|AIza[0-9A-Za-z_-]{35}|gh[pousr]_[0-9A-Za-z]{30,}|github_pat_[0-9A-Za-z_]{50,}|sk-(?:proj-)?[0-9A-Za-z_-]{32,})(?![A-Za-z0-9])'),
    'private-key': re.compile(r'-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----'),
    'literal-credential': re.compile(r"""(?i)\b(?:password|pwd|api_?key|client_?secret|smtp_?password)\b["']?\s*[:=]\s*["']([^"'\r\n]+)["']"""),
}

def placeholder(value):
    value = value.strip()
    return (not value or value.startswith(('<', '$', '{', 'Test-', 'TEST_', 'Wrong-', 'unused'))
            or value in {'CONTENT_REQUIRED', 'CHANGE_ME', 'YOUR_PASSWORD', 'Password', 'password', 'pwd', 'StringComparison.OrdinalIgnoreCase)'}
            or 'example' in value.lower() or 'şablon' in value.lower())

def inspect(path, text):
    findings = set()
    for category, pattern in PATTERNS.items():
        for match in pattern.finditer(text):
            if category in {'literal-credential', 'connection-password'}:
                if placeholder(match.group(1)):
                    continue
            if category == 'literal-credential' and path.removeprefix('history:').endswith('tests/CevatBiyik.UnitTests/ContactNotificationTests.cs') and match.group(1) == 'secret':
                continue  # Reviewed synthetic SMTP option fixture, never a provider token.
            findings.add((path, text.count('\n', 0, match.start()) + 1, category))
    for candidate in re.finditer(r'(?i)(?:Server|Host|Data Source)\s*=[^"\r\n]{0,2000}', text):
        for match in re.finditer(r'(?i)(?:password|pwd)\s*=\s*([^;]*)', candidate.group()):
            if not placeholder(match.group(1)):
                findings.add((path, text.count('\n', 0, candidate.start()) + 1, 'connection-password'))
    # Credentials embedded in URI userinfo are not rendered in output.
    for match in re.finditer(r'(?i)\b(?:mysql|smtp|https?)://[^\s/:]+:([^\s/@]+)@', text):
        if not placeholder(match.group(1)):
            findings.add((path, text.count('\n', 0, match.start()) + 1, 'uri-credential'))
    return sorted(findings)

def is_text_path(path):
    return path.suffix.lower() in TEXT or path.name.startswith('.env') or not path.suffix

def git(*args):
    return subprocess.check_output(['git', *args], cwd=ROOT)

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--history', action='store_true')
    parser.add_argument('--self-test', action='store_true')
    args = parser.parse_args()
    if args.self_test:
        assert inspect('fixture', 'Pass' + 'word = ' + chr(34) + 'nonplaceholder-secret' + chr(34))
        assert not inspect('fixture', 'Password = "Test-Only_Strong9!"')
        assert inspect('fixture', 'mysql://' + 'user:' + 'realcredential' + '@localhost/db')
        assert inspect('fixture', '-----BEGIN ' + 'PRIVATE KEY-----')
        assert inspect('fixture', 'AK' + 'IA' + 'A' * 16)
        assert not inspect('fixture', 'var password = input.Password;')
        assert not inspect('fixture', 'Server=localhost;Password=;Database=test;')
        assert inspect('fixture', 'Server=localhost;Pass' + 'word=' + 'nonplaceholder-secret' + ';')
        print('Secret scan self-tests: 8 passed (synthetic fixtures only).')
        return 0
    findings = []
    count = 0
    paths = git('ls-files', '-z', '--cached', '--others', '--exclude-standard').decode('utf-8').split('\0')
    for name in sorted(set(paths)):
        path = ROOT / name
        if not name or not is_text_path(path) or not path.is_file():
            continue
        count += 1
        findings.extend(inspect(name, path.read_text(encoding='utf-8-sig', errors='replace')))
    historical = 0
    if args.history:
        objects = git('rev-list', '--objects', '--all').decode('utf-8').splitlines()
        selected = []
        for line in objects:
            parts = line.split(' ', 1)
            if len(parts) == 2 and is_text_path(Path(parts[1])):
                selected.append(parts)
        proc = subprocess.run(['git', 'cat-file', '--batch'], cwd=ROOT,
            input=('\n'.join(x[0] for x in selected) + '\n').encode(), stdout=subprocess.PIPE, check=True)
        data = proc.stdout
        offset = 0
        for oid, name in selected:
            end = data.index(b'\n', offset)
            header = data[offset:end].split()
            size = int(header[2])
            body = data[end + 1:end + 1 + size]
            offset = end + 1 + size + 1
            if header[1] != b'blob' or b'\0' in body:
                continue
            historical += 1
            findings.extend(inspect('history:' + oid[:12] + ':' + name, body.decode('utf-8-sig', errors='replace')))
    findings = sorted(set(findings))
    print(json.dumps({'working_text_files': count, 'historical_text_blobs': historical,
        'findings': [{'path': p, 'line': line, 'category': category} for p, line, category in findings]}, ensure_ascii=True, indent=2))
    return 1 if findings else 0

if __name__ == '__main__':
    sys.exit(main())
