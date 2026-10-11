# Phase 15 — Üretim hazırlığı doğrulama raporu

Tarih: 2026-10-11. Durum: **Phase 15 kapanışı ve commit proje sahibi tarafından onaylandı.**
Yerel hazırlık doğrulandı; açık hosting çıkış kapıları Phase 16 denetimine aktarılır.
Phase 16/17 başlatılmadı, üretim deploy edilmedi.
Başlangıç commit'i: `8f0c96c` (owner'ın appsettings/CSS düzenlemesi korunmuştur).
Doğrulanan yayın paketi commit öncesi üretildi; manifesti dirtyWorkingTree=true gösterir.
Dağıtılacak nihai paket, onaylanan commit üzerinden yeniden üretilmelidir.

## İnceleme ve uygulama

AGENTS, Phase 00, manifest/faz sırası, ana rehberler, ADR-008/012/025/030/031 ve
DevOps/security/database/architecture/testing skills okundu. .NET8, tek MVC/Razor
uygulama, doğrusal katman referansı, MySQL8 hedefi ve hosting filesystem korundu.
Gerçek bildirilen MariaDB10.3/SSL farkı yeni karar ADR-032'de açıkça kaydedildi.

| Kapsam | Uygulanan dosyalar / sonuç |
|---|---|
| Server JSON | `.gitignore`, Web csproj: base/Production JSON Git indeksinden çıkarıldı, fiziksel dosyalar korundu; tüm local appsettings publish dışında. Placeholder şablonlar docs/deployment/templates altında. |
| SH-12-01 | Rotasyon/revocation ve consumer envanteri rehberi hazır; production parola değiştirilmedi, geçmiş yeniden yazılmadı. Kayıt açık. |
| IIS | Web csproj/web.config outofprocess, ANCM V2, Production, stdout kapalı, 6 MiB, config/App_Data/.git hidden, directory browse kapalı. |
| Database dialect | Infrastructure `DatabaseServerConfiguration`: açık MySql8/MariaDb sürümü, AutoDetect yok; opt-in hosted testlerde MariaDb10.3.39 aynı dialect. Snapshot/migration değiştirilmedi. |
| Anahtarlar | Web `HostingDataProtection`: private absolute key store, Windows machine DPAPI, sabit app adı; public/placeholder yolu reddi ve gerçek restart/decrypt testi. |
| Publish | `tools/publish.ps1`, `verify_publish.py`, `test_publish.py`: yeni release dizini, yasak içerik/native/IIS kontrolü, SHA-256 manifest; server verisini silme/sync/migration yok. |
| Araç/asset | dotnet-ef 8.0.29 mevcut EF8.0.29 ile eşlendi; deterministic minifier kontrolü Windows CRLF/LF farklılıklarına dayanıklı. |
| Operasyon | PRODUCTION_RUNBOOK, PLESK_CHECKLIST, PLESK_MEDIA_RUNBOOK, DEPLOYMENT, ENVIRONMENTS, SECURITY_HARDENING, OPEN_QUESTIONS ve faz durumu güncellendi. |

## Çalıştırılan doğrulamalar

```powershell
node tools/build-assets.cjs
pwsh -NoProfile -File tools/test.ps1 -Browser -HostedMySql
pwsh -NoProfile -File tools/publish.ps1 -SkipTests
dotnet tool restore
dotnet list CevatBiyik.sln package --vulnerable --include-transitive
python tools/security_scan.py
python tools/security_scan.py --history
```

- Release build **0 hata / 0 uyarı**.
- **425 .NET testi geçti**: 211 unit + 214 integration; 10 yetkili `cevodbtest`
  persistence kontrolü bu kez gerçek MariaDbServerVersion ile geçti.
- Bir boş-schema migration/Down-drill testi, ayrı opt-in empty DB olmadığı için atlandı.
  Bu, gerçek MySQL8 veya production migration doğrulaması değildir.
- **6 analitik JS + 15 Playwright**; 8 scanner sentetik self-test; 4 publish-boundary
  negatif/pozitif test; minified parity ve format kontrolleri geçti.
- DataProtection Windows testi dosyadaki encryptedSecret ve yeni provider ile aynı
  protected payload decrypt kontrolünü gerçekten çalıştırır; gizli key içerikleri raporlanmaz.
- NuGet güncel kaynak denetimi: yedi projede bilinen zafiyet bildirilmedi.
- MariaDb10.3.39 ayarıyla `dotnet ef migrations has-pending-model-changes --no-build`
  kontrolü geçti: son migration sonrası model farkı yok. Yeni migration gerekmez.
- Current-tree scanner: 436 metin dosyası, **0 bulgu**, exit 0. Ignored gerçek JSON bu
  taramanın dışında; bu sonuç yerel credential'ın temizlendiği anlamına gelmez.
- History scanner: 884 historical text blob, **2 bulgu**, exit 1. Bir gerçek historical
  DB credential (SH-12-01), bir açık sentetik `example.test` URI negatif-test fixture.
  Mevcut fixture TEST_ONLY olarak işaretlendi, URI credential reddi testi korunur.
  Historical sonucu temiz olarak raporlanmadı; hiçbir değer yazdırılmadı.

## Üretilen artefaktlar (Git dışında)

- `artifacts/releases/20261011-182946-d7133fe6/`: framework-dependent .NET8 publish;
  artifact denetimi geçti, server JSON ve runtime data yok. Windows x64 Skia DLL
  mevcut. RELEASE_MANIFEST.json commit/dirty-state, relative path, boyut ve SHA-256 taşır.
- `artifacts/migrations/phase15-mariadb.sql`: EF8.0.29 / explicit MariaDb10.3.39
  idempotent migration zinciri SQL, offline üretildi; 56.574 byte. Üretimde uygulanmadı.
- `artifacts/migrations/efbundle.exe`: win-x64 framework-dependent EF8.0.29 bundle
  derlendi, yalnızca `--help` çalıştırıldı. `database update`/bundle uygulama çalıştırılmadı.
- `dotnet --list-runtimes`: yerel ASP.NETCore.App/NETCore.App 8.0.31 mevcut;
  SDK 8.0.425. Bu gerçek hosting runtime kanıtı değildir.

Paket çalışma ağacından üretilmiştir; release son owner onaylı commit'ten yeniden
oluşturulmalı. Paket/bundle sunucuya aktarılmadı. JSON kaydetme ve ACL güvenliği gerçek
hostingde operatör tarafından tamamlanır. Publish script dependency restore yapmaz;
temiz makinede önce locked proje bağımlılıkları restore edilir.

## Açık çıkış kapıları

1. Plesk erişimi/topoloji, Windows/IIS/runtime/ANCM, pool identity ve site/private/key
   yolları teyidi; HTTPS/cert/forwarded IP, IIS limit/headers/HTML compression smoke.
2. **SH-12-01 gerçek production rotation/revocation kanıtı.** Git exclusion tek başına kapatmaz.
3. MySQL8 hedef farkı, destek dışı MariaDB10.3 ve SSL kapalı DB bağlantısı için hosting
   iyileştirmesi/owner kararı. Production şablonu VerifyFull; açık ağda disable uygulanmadı.
4. .NET8 2026-11-10 EOL için destekli yükseltme takvimi ve EF/Pomelo uyumlu test planı.
5. Üretim history/DDL hesabı, empty-schema migration/restore drill ve bakım aralığı;
   bundle/SQL gerçek dialect üzerinde izole hedefte uygulanma kanıtı.
6. Persistent ACL, native image/Defender/PDF smoke, coordinated backup ve private/key
   kapsamı; sıklık/retention/RPO/RTO ve restore drill operatör kaydı.
7. Gerçek içerik/hukuki inceleme, onaylı form/SMTP ve indexing smoke; fake/demo veri taşınmaz.

Bu kapılar olmadan Phase 15 hosting tamamlandı veya üretim güvenli ilan edilmez.
