# Phase 14 doğrulama — 2026-10-11

Durum: Uygulandı, yerel olarak ölçüldü ve regresyon testleri geçti. **Sahibin kapanış/commit onayı alındı (2026-10-11).**
Phase 15 başlatılmadı; deploy yapılmadı. Phase 00, AGENTS.md, ana rehberler,
manifest/faz sırası, ADR-017/019/021/029 ve performans/erişilebilirlik/frontend/backend/database/
testing/QA/security skill kuralları denetlendi. .NET 8, Clean Architecture, tek SSR MVC uygulaması,
MySQL 8 hedefi ve Plesk/filesystem sınırları korundu.

## Değişiklikler

| Katman / dosyalar | Sonuç |
| --- | --- |
| `src/CevatBiyik.Web/Performance/StaticAssetPolicy.cs`, `Program.cs` | CSS/JS/SVG için HTTPS Brotli/Gzip; doğrulanmış içerik hash'i/GUID WebP immutable cache; sürümsüz/sahte hash bir saat must-revalidate; ETag/304/Vary korunur |
| `Security/SecurityHeadersMiddleware.cs` | Public HTML dahil no-store; nonce, admin/hata/download güvenlik sınırları korunur; HTML compression ve tam sayfa OutputCache/ResponseCaching kullanılmaz |
| `src/CevatBiyik.Application/Services/{PageContent,Profile,Setting}Service.cs` | Public DTO'lar scoped snapshot olarak tekrar kullanılır; başarılı yazmada invalidation, cancellation, eksik profil cache'i; cross-request cache yok |
| `tools/build-assets.cjs`, `wwwroot/css/*.min.css`, `wwwroot/js/*.min.js` | Deterministik yerel minify; kaynak+çıktı birlikte sürümlenir; runtime dependency eklenmez |
| Public `_Layout`, `_AnalyticsConsent`, `Articles/Detail` | Minified/versioned asset referansları; footer lazy/async; detay kapağı eager/high priority ve boyutlar; listelerde mevcut lazy/dimensions korunur |
| Admin `_AdminLayout` ve altı CMS `Edit.cshtml` | Quill sadece zengin içerik düzenlerken yüklenir; login/dashboard/listelerde 209 KB editör JS'i ve core CSS'i istenmez |
| `wwwroot/js/admin.js`, `wwwroot/css/admin.css` | Quill getFormat çağrısının blur/Tab sırasında odağı geri çekmesi düzeltildi; sticky header scroll margin; modal Tab döngüsü, native Escape ve editöre odak dönüşü |
| `RequestSnapshotTests`, `QueryBudgetTests`, `PerformanceAccessibilityTests` | 13 yeni .NET vakası: scoped invalidation/cancellation, sabit query budget/no-tracking, compression/hash/304/no-store/nonce/dimensions/cover policy |
| `phase14.test.cjs`, `phase14-audit.mjs`, `tools/phase14-audit.ps1`, `tools/test.ps1` | Axe, klavye, modal, mobil reflow, forced colors ve tekrar üretilebilir Lighthouse/AA denetimi |
| `DECISIONS.md` | ADR-030: ölçülen asset teslimi, güvenli cache/compression ve scoped public snapshot kararları |

Sistem fontları (Segoe UI/Arial/Georgia) korundu; harici font/FOIT bağımlılığı yok.
Bootstrap CSS ilk boyama için senkron kalır, JS defer yüklenir; FOUC/CLS yaratacak asenkron CSS veya
kanıtsız CSS purge uygulanmadı. Gerçek kişi bilgisi, kartvizit fotoğrafı veya hukuki iddia eklenmedi.

## Önce / sonra ölçümleri

Lighthouse **13.5.0**, Edge **155**, yerel loopback HTTPS fixture + geçici SQLite + sentetik içerik.
Mobil simülasyon 412×823, 4× CPU, 150 ms RTT / 1638.4 Kbps. Her rota için üç soğuk koşunun
medyanı; aynı ortam ve görünür çerez bildirimi. Public optimizasyonundan önce ve sonra ayrı çalıştırıldı.
Ham HTML/JSON: `artifacts/phase14/baseline/` ve `artifacts/phase14/after/` (Git dışı).
Sürümlenen özet: [PHASE_14_METRICS.json](PHASE_14_METRICS.json).

| Rota | Performans önce → sonra | LCP ms önce → sonra | CLS önce → sonra | TBT ms önce → sonra | Aktarım byte önce → sonra |
| --- | --- | --- | --- | --- | --- |
| `/` | 96 → 100 | 2476.44 → 1280.50 | 0 → 0 | 0 → 0 | 339913 → 96145 |
| `/makaleler` | 95 → 100 | 2631.49 → 1351.62 | 0 → 0 | 15 → 0 | 339495 → 95950 |
| `/makaleler/test-browser` | 96 → 100 | 2476.41 → 1276.31 | 0 → 0 | 0 → 0 | 338316 → 94840 |
| `/iletisim` | 96 → 100 | 2476.45 → 1276.75 | 0 → 0 | 0 → 0 | 341732 → 98239 |

Aktarım %71.3–72.0 azaldı; dört rotanın Lighthouse erişilebilirlik medyanı önce/sonra **100**.
Başlangıçta Lighthouse 212 KiB kullanılmayan CSS, 323 KiB cache fırsatı ve render-blocking CSS
uyarıları gösterdi. Sıkıştırma/cache uygulandı; Bootstrap'ın kullanılmayan bölümlerini agresif silmek
yerine responsive/accordion/offcanvas işlevleri korundu. Kalan unused-CSS uyarısı tek başına regresyon değildir.
Ana sayfanın ölçülen LCP öğesi hero H1 metnidir; fixture büyük gerçek kapak fotoğrafı içermez.
Kapak fetchpriority/dimensions/lazy davranışı ayrı HTTP entegrasyon testinde doğrulanmıştır.

Bu değerler **laboratuvar medyanıdır**, gerçek kullanıcı 75. yüzdelik veya INP değildir.
TBT, INP yerine geçmez. Gerçek içerik, kapak fotoğrafları, Plesk latency ve CrUX için yeniden ölçüm gerekir.
[Web Vitals](https://web.dev/articles/vitals) tanımına göre canlı CWV değerlendirmesi ayrı kalır.

## Sorgu ve güvenli önbellek kanıtı

- Repository public liste okumaları zaten `AsNoTracking` + eager split query kullanıyordu.
  SQLite command interceptor testi: kategori/etiket ilişkileriyle 1 ve 10 makale **iki SQL okuması**;
  ChangeTracker boş. Pagination toplamı için ayrı count sorgusu normaldir; lazy loading/N+1 yok.
- Consent, layout ve sayfa içeriği aynı scope içinde tek public sayfa okumasını paylaşır.
  Test; okuma sayısı, disable/republish sonrası yenileme ve yeni scoped instance davranışını doğrular.
- Public settings/profil snapshot'ı başarılı yazmada temizlenir. Private setting allowlist kontrolü,
  yayın/onay/CONTENT_REQUIRED kuralları değişmedi; public DTO'lar EF entity cache'i değildir.
- HTML no-store ve sıkıştırılmamış; art arda yanıt nonce'ları farklı. Version hash sahteciliği
  immutable cache elde edemez. Gerçek hash uzun cache/ETag 304 döndürür; 404 no-store olur.
- Tam sayfa cache, CSRF/consent/TempData ve CSP nonce verisini paylaşabileceği için etkinleştirilmedi.
  Asset sıkıştırması yalnızca public MIME allowlist'iyle yapılır; IIS/proxy aynı kuralı Phase 15'te doğrulamalıdır.

## WCAG 2.2 AA baseline

Axe-core/playwright **4.13.0**: WCAG 2 A/AA, 2.1 AA, 2.2 AA etiketleri; **19 rota, sıfır otomatik ihlal**.
Rotalar: `/`, `/hakkimda`, `/makaleler`, makale detayı, `/uzmanlik-alanlari`, alan detayı, `/sss`,
`/iletisim`, `/kvkk`, `/cerez-politikasi`, `/yasal-uyari`, `/konumlar`, `/admin/login`, `/admin`,
`/admin/articles/create`, `/admin/profile`, `/admin/faq`, `/admin/media`, `/admin/settings`.
Klavye testleri ayrıca açık mobil menü, link/önizleme modalları ve editör durumlarını denetler.

- Skip link Tab ile ilk odak; Enter ile `main#icerik` odağı; görünür outline doğrulandı.
- Mobil menü klavye ile açılır, aria-expanded güncellenir. Bir `main`, bir H1, Türkçe lang,
  form native label/aria ilişkileri ve mevcut validation/success focus akışları korundu.
- 320 ve 390 px'de beş public şablonda yatay overflow yok. 320 px, 1280 px görünümün 400%
  reflow genişliğine karşılık gelir; bu test gerçek tarayıcı zoom/ekran okuyucu sertifikası değildir.
- Admin Title→Excerpt odak sırası, editörden Tab çıkışı, sticky header altında görünür odak,
  dialog Tab/Shift+Tab/Escape/geri dönüş ve Windows forced-colors outline kontrolü geçti.
- Mevcut dokuz renk-token kontrast testi geçti: navy/paper 13.96:1, gold/navy 7.42:1,
  gold-ink/paper 5.48:1, muted/white 6.29:1. Açık zeminde dekoratif açık altın metin kullanılmaz.

### Otomatik karar verilemeyen durumların incelemesi

`artifacts/test-runs/phase14-a11y-review.json` hedefleri/stil katmanlarını kaydeder.
Bu bulgular otomatik başarıya çevrilmedi; aşağıdaki özel kontrol yapıldı:

| Axe incomplete | İnceleme |
| --- | --- |
| Public aria-hidden ok simgeleri | Yalnızca dekoratif Unicode ok; bağlantının metinsel adı var. Miras foreground/background gold-ink/paper 5.48:1, alt section `#F0EDE5` üzerinde 4.97:1 |
| Public/admin Bootstrap select | Axe SVG caret background nedeniyle karar veremiyor. Metin navy/paper 13.96:1; caret `#343A40`/paper 10.84:1; gerçek label/native select korunur |
| Link modal açıklaması | Native dialog katmanları nedeniyle overlap kararsızlığı. 390 px ekran görüntüsünde açıklama/label/input/actions çakışmıyor; muted/white 6.29:1. Modal klavye/axe kontrolleri geçti |
| Admin settings boş tablo header'ları | Fixture'da veri yok; otomatik th-has-data-cells kararsızlığı. Veri olmaması manuel açıklama olarak tutuldu; dolu gerçek içerik staging'de denetlenmeli |

Görsel kanıtlar: `artifacts/test-runs/phase14-editor-mobile.png`,
`artifacts/test-runs/phase14-link-modal-mobile.png`; bağlantı modalı görsel olarak incelendi.
Otomasyon tam WCAG sertifikası değildir. Gerçek içerikle ekran okuyucu, 200% metin büyütme /
gerçek 400% zoom, touch-target ve tüm hata/yayın/consent durumları için insan denetimi rollout gate'tir.

## Doğrulama komutları ve sonuçları

```powershell
node tools/build-assets.cjs
pwsh -NoProfile -File tools/phase14-audit.ps1 -Stage baseline -NoBuild
pwsh -NoProfile -File tools/phase14-audit.ps1 -Stage after -NoBuild
pwsh -NoProfile -File tools/test.ps1 -Browser -HostedMySql
dotnet build CevatBiyik.sln -c Release --no-restore -m:1 -p:UseSharedCompilation=false
dotnet test tests/CevatBiyik.IntegrationTests -c Release --no-build --no-restore --filter FullyQualifiedName~PerformanceAccessibilityTests
node tools/build-assets.cjs --check
dotnet format style CevatBiyik.sln --verify-no-changes --no-restore --diagnostics IDE0011
npm audit --prefix tests/CevatBiyik.BrowserTests --audit-level=high
```

Baseline komutu optimizasyondan önce çalıştırıldı; bugünkü kodla tekrar baseline üretmek önceki
ölçümü yeniden üretmez. Kaynak snapshot'u `3918891` + ölçüm harness'ıdır; sonucu ayrı saklayın.

| Kontrol | Sonuç |
| --- | --- |
| Release build (son statik politika dahil) | **0 hata / 0 uyarı** |
| Tam .NET paket | **199 unit + 208 integration = 407 geçti**, 1 migration opt-in skip |
| Hosted persistence | 208 integration içinde **10 cevodbtest vakası geçti**, işlemler rollback ile sınırlı |
| Son statik politika hedefli tekrar | **9/9** performance/accessibility HTTP testi geçti |
| Analytics JS | **6/6** |
| Playwright | **6 mevcut + 4 Phase 14 = 10/10**; son modal ekran görüntüsü koşusu da 4/4 |
| Lighthouse | Önce 12 + sonra 12 koşu; dört medyan bütçe geçti |
| Axe | 19 rota ve dinamik durumlarda sıfır otomatik AA ihlali; incomplete ayrı incelendi |
| Scanner self-test | **8/8**, yalnızca sentetik veri |
| Minified asset parity / C# style / diff whitespace | Geçti |
| npm audit | **0 vulnerability**; ek araçlar yalnızca devDependencies |

Migration skip: mevcut dolu demo/test veritabanına Up/Down/Up uygulanmadı; ayrı boş opt-in şema
gerektirir. Domain/EF model/migration dosyalarında değişiklik yok; Phase 14 için migration yok.
Gerçek MySQL 8/Plesk doğrulaması bu sonuçtan çıkarılamaz: kullanılan cevodbtest **MariaDB 10.3.39,
SSL kapalı** uyumluluk hedefidir. Production `cevodb` üzerinde işlem yapılmadı.

## Üretim öncesi devam eden kapılar

- SH-12-01 açık: sahibin talimatıyla appsettings.json'daki mevcut parola alanına dokunulmadı,
  değer okunmadı/raporlanmadı/stage edilmedi; üretim öncesi sır taşıma/rotasyon gerekli.
- Kabul edilmiş .NET 8 destek sonu ve gerçek MySQL 8/Windows-Plesk doğrulama riskleri değişmedi.
- Gerçek onaylı içerik/medya, baro/iletişim bilgileri, staging field/CWV ve assistive-technology
  incelemesi, upstream sıkıştırma/cache başlıkları sonraki rollout kontrolüdür.
- Bu faz analitik scriptlerini üretimde açmaz, sahte içerik yayınlamaz ve deploy gerçekleştirmez.

Kullanım: [Measurement guide](../performance/MEASUREMENT_GUIDE.md). Mimari: ADR-030, DECISIONS.md.
