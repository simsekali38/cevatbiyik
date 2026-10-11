# Repository layout — 2026-10-12

```text
root/
  AGENTS.md, CLAUDE.md, README.md     keşif/giriş rehberleri
  project/
    CevatBiyik.sln
    Directory.Build.props, Directory.Packages.props, global.json
    .config/dotnet-tools.json
    src/                            Domain/Application/Infrastructure/Web
    tests/                          Unit/Integration/E2EHost/BrowserTests
    tools/                          build/test/audit/publish araçları
  docs/                             ana rehberler, ADR, SEO/security/deployment/QA
    archive/                        tarihsel README, silinmedi
  design/                           UXMagic/marka raporları ve generated kaynak
  ai/                               master/manifest/phase promptları
    phases/, skills/
  database/                         migration SQL/notlar, korunmuş eski demo
    seeds/                          yalnız izole test için tam CMS fixture (ADR-037)
  artifacts/                        ignored test/release/taşıma kurtarma çıktıları
```

Kökten çalıştırılabilir giriş:

```powershell
pwsh -NoProfile -File project/tools/test.ps1 -Browser
pwsh -NoProfile -File project/tools/publish.ps1 -SkipTests
python project/tools/security_scan.py
node project/tools/build-assets.cjs --check
```

Doğrudan .NET/Node/EF komutları için application working directory **project/**:

```powershell
cd project
dotnet restore CevatBiyik.sln
dotnet build CevatBiyik.sln -c Release
dotnet test CevatBiyik.sln -c Release
dotnet tool restore
node --test tests/CevatBiyik.BrowserTests/analytics.test.cjs
dotnet run --project src/CevatBiyik.Web --launch-profile https
```

EF komutları sadece onaylı hedefte, project/ içinden çalıştırılır; bu düzenleme
migration çalıştırmaz. Eski raporlar tarihsel komutlar içerir; güncel cwd kuralı
bu belgedir. PowerShell girişleri cwd'yi project'e alır ve sonunda geri döner.
Browser artefact yolları project'ten ../artifacts'e gider. solution-folder adları
src/tests ve assembly adları değişmedi, relative ProjectReference'lar değişmedi.

IDE: project/CevatBiyik.sln'ı açın. Root .vs ayarları/sohbetleri korunur; yeniden
üretilebilir dört cache artifacts/ide-cache/ altında saklandı. Yeni path'e reopen
gerekir. .NET restore mevcut paketleri yeni absolute yollarla yeniden çözer.
Yeni dependency kurulmadı. .editorconfig eski migration/test scope'ları yeni prefix'e
uyarlandı; generated migration dosyaları yeniden yazılmadı.

Gerçek base/Development/Production appsettings JSON, uploads, App_Data ve node_modules
native Move-Item ile korundu. JSON artık project/src/... altında ignored; publish
Never kuralı korunur. Hedef önceden varsa taşıma reddedildi. Recursive reset/delete yok.
Root bin/obj'deki boş taşıma artık dizinleri kaldırıldı; eski src'de yeniden üretilmiş
5 obj dosyası artifacts/move-recovery/legacy-generated-src altında korunur.
.git/.vs/.github ve ignored test.txt tutuldu; kaynak kartvizitler mevcut checkout'ta yok.

Envanter: qa/ORGANIZATION_PLAN.json (eski→yeni her tracked dosya).
Rollback: envanter map'ini tersine uygulayın; mevcut hedefi ezmeyin, değişiklikleri
önce kaydedin ve yolları geri inceleyin. Git reset/clean kullanmayın. Commit veya
push bu görevde yapılmaz; rename tespiti owner'ın sonraki git add/review aşamasındadır.
