# Phase 16 — Üretime hazırlık denetimi

Tarih: 2026-10-11. Kaynak başlangıcı: `72870ca`.
Karar: **NO-GO — yerel doğrulamalar başarılı, üretim çıkış kapıları açık.**
Phase 16 kapanışı ve commit proje sahibi tarafından 2026-10-11 tarihinde onaylandı.
Bu onay üretim yayını veya Phase 17 başlangıcı onayı değildir; açık üretim kapıları
Phase 17 yayın öncesi kontrol listesine aktarılır, kapanış kanıtı hâlâ gereklidir.

## Kapsam ve mimari

AGENTS, Phase 00/16, manifest ve faz sırası, ana mimari/güvenlik/test/SEO rehberleri,
mevcut ADR'ler ve ilgili skills incelendi. Tek MVC/Razor uygulaması, .NET 8,
Web → Infrastructure → Application → Domain yönü, MySQL 8 hedefi ve filesystem
depolama korunur. Mevcut hosting test hedefi MariaDB 10.3.39'dur; bu MySQL 8 kanıtı
değildir. Yeni domain/model/migration veya bağımlılık eklenmedi; mimari değişmediği
için yeni ADR açılmadı. ADR-008/025/030/032 kararları uygulanır.

## Tekrarlanabilir doğrulama

```powershell
pwsh -NoProfile -File tools/test.ps1 -Browser -HostedMySql
dotnet format analyzers CevatBiyik.sln --verify-no-changes --no-restore --severity warn
python tools/security_scan.py
python tools/security_scan.py --history
dotnet list CevatBiyik.sln package --vulnerable --include-transitive
npm audit --prefix tests/CevatBiyik.BrowserTests --audit-level=high
pwsh -NoProfile -File tools/phase14-audit.ps1 -Stage phase16 -NoBuild
pwsh -NoProfile -File tools/publish.ps1 -SkipTests
```

`-SkipTests` yalnızca bu çalışma ağacında tam testler çalıştırıldığı için kullanıldı.
İlk sandbox çalıştırmasında testhost bağlantısı 90 saniyede zaman aşımına uğradı;
çalıştırma durdurulup yerel süreç/ağ erişimiyle yeniden çalıştırıldı. NuGet'in ilk
sandbox DNS hatası da erişim verilerek tekrarlandı. Bu başarısız altyapı denemeleri
test başarısı olarak sayılmaz. Hiçbir komut production veritabanında çalıştırılmadı.

| Kontrol | Gerçek sonuç |
|---|---|
| Release build | 0 hata / 0 uyarı |
| .NET | 211 unit + 214 integration = **425 geçti**, 0 başarısız |
| Opt-in persistence | 10 yetkili `cevodbtest` MariaDB vakası, toplamın içinde |
| Migration geri dönüş testi | 1 skip; ayrı boş opt-in şema yok |
| Analytics JavaScript | 6/6 |
| Playwright | 6 ziyaretçi/admin + 4 erişilebilirlik + 5 admin UX = 15/15 |
| Scanner self-test / publish boundary | 8/8 ve 4/4 |
| Asset parity / IDE0011 / C# warning analyzers | Geçti |
| NuGet / npm | Bilinen zafiyet bildirilmedi; npm 0 vulnerability |
| Current-tree secret scan | Son kontrol: 439 metin dosyası, 0 bulgu |
| History secret scan | 916 blob, 2 bulgu, exit 1; temiz değildir |
| Migration model farkı | Explicit MariaDB 10.3.39 ile fark yok |

## Güvenlik ve SH-12-01

History bulguları: gerçek geçmiş DB credential'ı ve açıkça sentetik `example.test`
URI fixture'ı. Hiçbir sır değeri gösterilmedi. Kaynak taraması sezgisel bir kontroldür;
ignored yerel JSON, sunucu konfigürasyonu ve tüm hassas veri türlerini kapsamaz.

`git check-ignore` gerçek base/Production JSON'un ignored olduğunu; `git ls-files`
bu dosyaların takip edilmediğini doğruladı. Publish denetimi server JSON, SQL,
private depolar ve gerçek uploads içeriğini reddediyor. Yerel dosya içerikleri
değiştirilmedi; Git geçmişi yeniden yazılmadı.

Admin role/policy/antiforgery, __Host cookie, güvenli HTML allowlist, nonce CSP,
trusted proxy sınırı, rate limiting ve yetkili PDF indirme regresyonları geçti.
Bu testler gerçek Plesk ACL, proxy topolojisi veya antivirus kurulumunu kanıtlamaz.
SH-12-01 rotasyon prosedürü [runbook bölüm 4](../deployment/PRODUCTION_RUNBOOK.md#4-sh-12-01-parola-rotasyonu)
üzerinden incelendi. Yeni bağlantının çalışması ve eski credential'ın reddi için
operatör kanıtı alınmadı; kayıt **açık**. Gerçek şifre değişikliği yapılmadı.

## SEO ve yerel içerik denetimi

SeoEnhancementTests, LocalSeoFlowTests, CMS/public HTTP ve E2E regresyonları çalıştı.
Canonical yapılandırılmış HTTPS origin'den üretiliyor; kullanıcı Host başlığına
güvenilmiyor. Metadata HTML encoded; JSON-LD default JSON encoder ve nonce ile
üretiliyor. Global indexing kapalıyken meta/header noindex ve robots disallow
üstün geliyor; sitemap yalnızca yayınlanabilir/indexable içerikleri içeriyor.
Gelecek/taslak makaleler, yetersiz yerel içerikler ve doğrulanmamış gerçekler için
mevcut yayın kapıları korunuyor. Redirect hedef güvenliği ve 301 davranışı testli.

Gerçek production içerik/şema adresi, hukuki metinler, yönlendirme listesi, sitemap
kapsamı ve Google indeksleme sonucu incelenmedi. Demo SQL veya cevodbtest kopyası
üretime taşınmaz. CONTENT_REQUIRED, uydurma ofis/karar/yorum ve test verisiyle
indeksleme açılmaz. Operatör indexing anahtarını ancak insan içerik onayından sonra açar.

## Erişilebilirlik ve performans

Mevcut ölçüm harness'ına `-Stage phase16` eklendi; Phase 14 kanıtları üzerine yazılmaz.
Yeni artefaktlar `artifacts/phase16/audit/` içindedir. Mobil Axe 19 public/admin
rotasında **0 otomatik ihlal** bildirdi (WCAG 2/2.1/2.2 A/AA tag'leri).
Klavye skip link, görünür focus, mobil menü, 320/390 px reflow, editör/dialog odak
dönüşü ve KVKK form değeri korunması Playwright ile doğrulandı.

Lighthouse 13.5.0 / Edge 155, dört rota × üç koşu medyanları:

| Rota | Performans / erişilebilirlik | LCP (ms) | CLS | TBT (ms) |
|---|---|---:|---:|---:|
| / | 100 / 100 | 1277.0 | 0 | 0 |
| /makaleler | 100 / 100 | 1432.8 | 0 | 15.5 |
| /makaleler/test-browser | 100 / 100 | 1275.8 | 0 | 0 |
| /iletisim | 100 / 100 | 1276.8 | 0 | 0 |

Tüm medyanlar mevcut performance≥90, LCP≤2500ms, CLS≤0.1, TBT≤200ms bütçelerini
geçti. Axe viewport 390×844; Lighthouse mobil viewport 412×823, CPU 4× slowdown.
Axe incomplete: /, /makaleler, makale detay ve /admin/articles/create için
color-contrast; /iletisim için aria-valid-attr-value; /admin/settings için
th-has-data-cells. Bunlar ihlal sayılmadı veya sessizce yok sayılmadı; gerçek
içerikle manuel inceleme kuyruğunda tutulur.

Marka tokenlarının sRGB kontrastı tekrar hesaplandı: gold/navy 7.42:1,
navy/paper 13.96:1, gold-ink/paper 5.48:1, muted/white 6.29:1. Bunlar seçilen renk
çiftlerinin kontrolüdür; tüm render edilen durumlar için uygunluk kanıtı değildir.

Ölçüm loopback HTTPS,
sentetik fixture ve simüle mobil koşullardadır; gerçek trafik CWV/INP veya eksiksiz
WCAG 2.2 AA uygunluk beyanı değildir. Axe incomplete bulguları, ekran okuyucu,
zoom/forced-colors ve gerçek içerik/medya insan kontrolü gerektirir.

EF read yollarında AsNoTracking, bounded paging ve ilişki include/split query
korunuyor; sorgu başına gerçek hosting latency/plan profili alınmadı. Assetler
minified/defer, açık görsel ölçüleri ve nonce uyumluluğu regresyon kapsamındadır.

## Dağıtım ve migration dry-run

- Yeni paket: `artifacts/releases/20261011-185253-1898ca97/`. Publish verification
  geçti; net8.0, Production, out-of-process, kapalı stdout, 6 MiB ve Windows x64 Skia
  artefaktı doğrulandı. Manifest başlangıç commit'i 72870ca ve dirtyWorkingTree=true
  içerir; son onaylanan commit üzerinden nihai paket yeniden üretilir.
- Explicit MariaDB 10.3.39 ve MySQL 8.0.0 için offline idempotent migration SQL
  üretildi: `artifacts/migrations/phase16-mariadb.sql` ve `phase16-mysql8.sql`.
  İkisi 56.574 byte ve SHA-256
  `31833AD61FBC37E16C6E6653DC20ACE1555D6D85CA5457A78DE50C6EC5EEC89E`.
  Aynı SQL çıkması her iki sunucuda çalıştığı anlamına gelmez; canlı apply yapılmadı.
- Önceki EF8 bundle yalnızca help çalıştırmasıyla doğrulanmıştı; burada bundle'ın
  gerçek schema üzerinde uygulanması veya production migration iddia edilmez.
- Backup/restore adımları tutarlı batch ve writer freeze açısından incelendi.
  **Gerçek DB+files restore çalıştırılmadı.** Yetkili izole hedef, backup, ACL ve
  Plesk erişimi yok. Lokal publish üretimi gerçek IIS deployment provası değildir.
- [Restore kanıt formu](../deployment/RESTORE_DRILL_RECORD.md) hazırlandı; import,
  hash/history/record count, file reconciliation, PDF/image, DPAPI ve RPO/RTO kanıtını
  operatör doldurur. Dolu cevodbtest'e destructive Down/restore uygulanmaz.

## Açık bulgular ve kapanış kanıtı

| ID / öncelik | Bulgu | Kapanış için gereken |
|---|---|---|
| SH-12-01 / yüksek | Geçmiş credential; rotasyon bilinmiyor | Yeni bağlantı başarılı, eski parola iptal; tarih/operatör kaydı |
| PR-16-01 / yüksek | MariaDB 10.3 destek dışı; MySQL8 hedefine aykırı | Hosting destekli DB/MySQL8 geçişi, izole uyumluluk testleri ve onay |
| PR-16-02 / yüksek | Bildirilen DB SSL kapalı; topoloji bilinmiyor | TLS doğrulaması; ya da gerçek local/private topoloji ve belgeli owner kararı; public ağda açık credential kabul edilmez |
| PR-16-03 / yüksek | Gerçek Plesk/restore/migration dry-run yok | Checklist ve restore formunda gerçek kanıt; startup/native/Defender/ACL/HTTPS smoke |
| PR-16-04 / süreli risk | .NET8 destek bitişi 2026-11-10 | Destekli upgrade takvimi/hosting teyidi; destek dışı runtime ile launch yok |
| PR-16-05 / yayın kapısı | Hukuki/gerçek içerik, SMTP ve indexing onayı yok | Avukat onayı, production içerik temizliği ve gerçek form/SMTP/SEO smoke |
| PR-16-06 / doğrulama sınırı | Field CWV, manuel assistive technology audit yok | Gerçek staging/cihaz/ekran okuyucu ölçümü; Axe incomplete incelemesi |

Sorumlular: uygulama düzeltmeleri geliştirici; sunucu/DB/rotasyon/restore hosting
operatörü; içerik/etik/launch onayı proje sahibi/avukat. Otomatik test başarısı bu
kapıları kaldırmaz. Bu çalışmada yeni doğrulanmış kritik/yüksek uygulama hatası
bulunmadı; mevcut dış ortam blocker'ları çözülmüş sayılmadı.

## Güncel resmi kaynaklar

- [.NET destek politikası](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
  ve [.NET8 destek bitiş duyurusu](https://devblogs.microsoft.com/dotnet/dotnet-8-9-end-of-support/): 2026-11-10.
- [MariaDB 10.3 release notes](https://mariadb.com/kb/en/changelogs-mariadb-10-3-series/): bakım Mayıs 2023'te bitti.

Kapanış: **Phase 16 owner onayıyla kapandı; açık kapılar nedeniyle üretim hazırlığı NO-GO.**
