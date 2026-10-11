# Kalan işlerin tamamlanması — doğrulama raporu

Tarih: 2026-10-11–12, Europe/Istanbul. Kaynak: main / HEAD 60d4a6c.
**Yerel uygulama, test ve paket provası tamamlandı; production NO-GO korunuyor.**
Başlangıç: REMAINING_WORK_START.md. Owner'ın yapıştırdığı A–H kapsamı uygulanır.
Önceden değişmiş makale/çalışma alanı dosyaları korunur; staged dosya yoktur.
Commit/push, hosting değişikliği, gerçek yayın, seed veya DB/migration yazması yapılmaz.

## 1. Tamamlanan aşamalar

| Aşama | Gerçek durum |
|---|---|
| A — Çalışma alanları | Önceki doğrulanmış liste/detay aktarımı korundu; gerçek CMS ilişkisinden lokasyon bağlantıları mevcut. Modelde olmayan ilgili SSS koleksiyonu uydurulmadı. |
| B — Hakkımda | İki kolonlu profil düzeni, yerel amblemli portre yer tutucusu, reviewed biyografi/vizyon/baro, nötr boş durum ve CTA. Yeni profil alanı/migration yok. |
| C — İletişim/SSS/hukuki | Ortak public kart/başlık/breadcrumb, native SSS ve KVKK dialog, CMS legal içerik/yer tutucu; çalışan form backend ve yayın kapıları korundu. |
| D — Ortak UI | Dört genişlikte reflow, kontrast/focus ve mobil navigasyon doğrulandı; tablet contact gutter taşması ve aktif contact nav kontrastı düzeltildi. Admin görünümü değişmedi. |
| E — SEO | Legacy placeholder meta override engellendi; excerpt/profil/schema plain text; reviewed legal body description. Canonical/OG/sitemap/robots/redirect/local quality regresyonları korundu. ADR-035. |
| F — Güvenlik/performans | Yerel statik analiz, secret/dependency taraması, browser güvenlik regresyonu ve Lighthouse/axe denetimi yapıldı. Gerçek hosting penetrasyon testi/WCAG sertifikası değildir. |
| G — DB/hosting hazırlığı | cevodbtest üzerinde sabit SELECT/SHOW ile read-only teşhis yapıldı. Publish/operatör kontrol listesi hazır; gerçek Plesk/backup/restore kanıtı eksik. |
| H — Son regresyon | 432 .NET / 6 JS / 19 browser geçti; yerel release paketi doğrulandı. Gerçek sunucu/yayın testleri yapılmadı. |

## 2. Değiştirilen dosyalar

Bu görevde public UI:
- project/src/CevatBiyik.Web/Views/Home/About.cshtml
- project/src/CevatBiyik.Web/Views/Home/Contact.cshtml
- project/src/CevatBiyik.Web/Views/Faq/Index.cshtml
- project/src/CevatBiyik.Web/Views/Pages/Detail.cshtml
- project/src/CevatBiyik.Web/wwwroot/css/site.css, site.min.css (builder çıktısı)

SEO sunum katmanı:
- project/src/CevatBiyik.Web/Controllers/PublicController.cs, PagesController.cs
- project/src/CevatBiyik.Web/Models/PublicSiteSettings.cs
- project/src/CevatBiyik.Web/Seo/SeoPresentation.cs

Test/araçlar:
- project/tests/CevatBiyik.IntegrationTests/RemainingAboutTests.cs
- project/tests/CevatBiyik.IntegrationTests/RemainingPublicPageTests.cs
- project/tests/CevatBiyik.IntegrationTests/RemainingSeoTests.cs
- project/tests/CevatBiyik.BrowserTests/phase14.test.cjs, phase14-audit.mjs
- project/tools/phase14-audit.ps1 (remaining ölçümü ayrı klasörde)
- project/tools/read-only-db-check.ps1 (yalnızca cevodbtest, sabit read-only sorgular)
- project/src/CevatBiyik.Web/CevatBiyik.Web.csproj (server-owned JSON publish dışlama kuralı)

Belgeler: CHANGELOG.md, DECISIONS.md, SECURITY.md,
docs/qa/REMAINING_WORK_START.md ve bu rapor,
docs/deployment/REMAINING_LAUNCH_ACTIONS.md,
design/PRACTICE_AREAS_TRANSFER_REVIEW.md takip notu.

Bu görev başlangıcında mevcut ve korunan önceki çalışmalar:
ArticlesController.cs, Views/Articles/Index.cshtml ve Detail.cshtml,
Views/PracticeAreas/Index.cshtml ve Detail.cshtml, ArticleDesignTests.cs,
PracticeAreaDesignTests.cs, ARTICLES_TRANSFER_REVIEW.md,
PRACTICE_AREAS_TRANSFER_REVIEW.md, ortak CSS/browser/CHANGELOG katkıları.
Home/Index, _Layout, Admin, Domain/Application/Infrastructure, DB şeması ve paketler
bu görevde değiştirilmedi. Harici UXMagic runtime/ikon/font veya gerçek dışı fotoğraf yok.

## 3. Çalıştırılan testler ve gerçek sonuçları

Aşama B: ilgili About/public testlerinden 45 geçti.
Aşama C: contact/legal/FAQ/analytics akışlarından 44 geçti.
Aşama D: 6 E2E + 8 public/a11y/design + 5 Admin UX = 19 browser testi geçti.
Aşama E: SEO/local SEO grubunda 52 geçti, 3 opt-in DB skip (aşağıdaki 11'in alt kümesi).

Son regresyon: Release build 0 hata / 0 uyarı; 211 unit + 221 integration = 432
.NET testi geçti, 11 opt-in DB testi atlandı. JS 6/6, scanner self-test 8/8,
publish boundary test 4/4 geçti. Son browser: 6 E2E + 8 public/a11y/design + 5
Admin UX = 19/19. IDE0011/asset parity geçti. Publish güvenlik düzeltmesinden sonra
build/.NET/JS/asset/publish sınır paketi ayrıca yeniden çalıştırıldı; browser UI
son doğrulamadan sonra değiştirilmedi.
Warning seviyesinde C# analyzers geçti. Yeni test import sırası ilk tam koşuda
format kontrolünde yakalandı, düzeltildi ve yeniden çalıştırıldı; başarısız ara
koşular başarı olarak sayılmaz. Önceki browser tablet taşması/kontrast ve yanlış
FAQ test selector'ı düzeltilip tüm browser paketi yeniden doğrulandı.

Tekrar çalıştırma (yalnızca test terminali):

```powershell
$env:AllowedHosts = '*'
$env:Site__BaseUrl = 'https://cevatbiyik.av.tr'
pwsh -NoProfile -File project/tools/test.ps1 -Browser
dotnet format analyzers CevatBiyik.sln --verify-no-changes --no-restore --severity warn
python project/tools/security_scan.py
python project/tools/security_scan.py --history
dotnet list CevatBiyik.sln package --vulnerable --include-transitive
npm audit --prefix project/tests/CevatBiyik.BrowserTests --audit-level=high
pwsh -NoProfile -File project/tools/phase14-audit.ps1 -Stage remaining -NoBuild
pwsh -NoProfile -File project/tools/read-only-db-check.ps1
pwsh -NoProfile -File project/tools/publish.ps1 -SkipTests
```

Test env overrides gerçek ignored hosting JSON'un test hostuna sızan Host/origin
kısıtlarını izole eder; production wildcard host önerisi değildir. Test fixture HTTPS
loopback'tir; sahte test kayıtları yalnızca fixture/izole repository içindedir.

Lighthouse: Edge, simüle mobil, dört rotada üçer tekrar medyanı:

| Rota | Performans | Erişilebilirlik | LCP | CLS | TBT | Transfer |
|---|---:|---:|---:|---:|---:|---:|
| / | 100 | 100 | 1351 ms | 0 | 0 ms | 104628 byte |
| /makaleler | 100 | 100 | 1427 ms | 0 | 0 ms | 101893 byte |
| /makaleler/test-browser | 100 | 100 | 1351 ms | 0 | 0 ms | 100574 byte |
| /iletisim | 100 | 100 | 1351 ms | 0 | 0 ms | 104022 byte |

Kanıt: artifacts/phase14/remaining/summary.json ve 12 HTML/JSON Lighthouse raporu.
19 public/Admin ekranında axe: 0 ihlal. 7 ekranda incomplete/manual-review bulguları
var; otomatik araç her kontrast/ARIA/tablo durumuna karar veremedi. Browser audit
artifacts/test-runs/phase14-a11y-review.json içinde hedef/style bilgilerini saklar.
Token kontrastları brand rehberine göre navy/paper 13.96:1, gold/navy 7.42:1,
gold-ink/paper 5.48:1, muted/white 6.29:1. Klavye skip/menu/FAQ/dialog/ordinary-input
akışları test edildi. Manual-review kayıtlarındaki dekoratif oklar aria-hidden'dır;
select kontrollerinin arka plan görseli Bootstrap'ın yerel ok SVG'sidir ve metin
navy/white üzerindedir. Kapalı KVKK native dialog ID'si markup'ta mevcut; açma,
kapatma ve focus dönüşü browser testiyle doğrulanır. Admin dialog hint'i muted/white
üzerindedir; boş Settings tablo başlıkları dolu gerçek veriyle ayrıca incelenmelidir.
Ekran okuyucu ve gerçek içerikle tam WCAG 2.2 AA sertifikası yok.
Bunlar laboratuvar ölçümleridir; production DB/medya/latency ve field INP ölçülmedi.

Görseller: artifacts/test-runs/remaining-after-{about,contact,faq,legal,cookies,disclaimer}-{320,390,768,1440}.png.
Mevcut home/article/practice görüntüleri ve UXMagic referanslarıyla tutarlılık
incelendi. İlk remaining-before-* yakalamalarında hostname 400 sayfası bulundu;
bunlar geçerli tasarım baseline'ı değildir ve pixel-diff başarı kanıtı sayılmadı.
Son görüntüler fixture status/header kontrolleriyle doğrulandı. About'ta gerçek
portre/uzun reviewed içerik bulunmadığından içerik-complete görsel onay ayrıca gerekir.

## 4. Atlanan testler ve nedenleri

10 MySQL persistence testi ile 1 migration upgrade/downgrade testi opt-in skip'tir.
Sadece bağlantı eksikliği değil: bu testler MigrateAsync, insert/update/delete veya
şema oluşturma/geri dönüş çalıştırır; owner bu görevde veri değiştirmeyi yasakladı.
Rollback kullanan vakalar da read-only değildir. -HostedMySql/-MySql/-MigrationMySql
seçilmedi. Salt okunur bağlantı kontrolü bunların yerine persistence onayı sayılmaz.
MySQL 8 kanıtı, yeni boş şema migration provası ve gerçek restore testi çalıştırılmadı.
Gerçek SMTP teslimi, GTM/GA/Search Console/Business Profile hesabı, Plesk ACL/native,
Defender (kapalı PDF modülü için N-A), log/SSL yenileme ve production smoke yapılmadı.

## 5. Güvenlik ve SEO denetimi bulguları

- Son current-tree scanner: 461 metin dosyası, 0 bulgu. Ignored JSON içeriği ve sunucu sırları bu sezgisel taramanın
  kapsamı dışındadır. Git history: 978 blob, 2 redakte bulgu, exit 1 — temiz değildir.
  Biri geçmiş gerçek connection credential, diğeri sentetik example.test test URI.
  Değerler gösterilmedi, geçmiş değiştirilmedi. Owner rotasyonu bildirdi; eski
  credential reddi için operatör kanıtı hâlâ gerekir.
- İlk publish provası üç gerçek JSON kopyası yakaladı; güvenlik sınırı fail-closed
  çalıştı. Yorumlanmış appsettings exclude kuralı yeniden etkinleştirildi ve yeni
  gerçek publish ile doğrulandı (ADR-032 davranışının geri yüklenmesi).
- Gerçek appsettings/base/Development/Production JSON ignored ve untracked;
  publish'den dışlanır. Web yerelde Development JSON, production server-owned JSON
  kullanır; user-secrets mimarisi yeniden değiştirilmedi.
- NuGet transitive audit: bilinen zafiyet raporlanmadı; npm audit 0 vulnerability.
  Yeni dependency yok. Tarama sonucu keşfedilmemiş zafiyet olmayacağının garantisi değildir.
- __Host admin cookie HttpOnly/Strict/Secure, role/aktif kullanıcı, süre, CSRF,
  server validation, nonce CSP/headers/trusted proxy, rate limits ve güvenli HTML
  allowlist regresyonları geçti. Query parametreleri SQL string birleştirmesine dönmez;
  EF filtreleri bounded, public read AsNoTracking ve relation reads AsSplitQuery.
- Görsel 5 MiB file/6 MiB request, MIME/magic/pixel sınırları, re-encode ve storage
  path/symlink kontrolleri korundu. Gizli PDF özelliği ADR-033 gereği kapalı/404;
  yeniden açılmadı ve malware tarama kapısı gevşetilmedi. Saklanmış özel dosyalar yedeklenir.
- HTML nonce/CSRF/consent içerdiği için no-store politikası korunur; ResponseCaching
  eklenmez. Hash sürümlü statik varlıklar immutable, diğerleri kısa süre cache edilir.
  Yerel fontlar/system stack, defer ve boyutlu görseller korunur; assetler gerçek builder ile üretildi.
- Canonical configured HTTPS origin kullanır; Host injection, dış yönlendirme/loop,
  admin/error noindex ve draft/future/soft-delete davranışı mevcut testlerle doğrulandı.
- Local quality/indexing/FactsVerified kapıları, verified-contact-only LegalService,
  reviewed-profile-only Person ve FAQ/Article/Breadcrumb güvenli JSON-LD korundu.
  Hayali adres, yorum, puan, mahkeme kararı veya otomatik şehir/uzmanlık kombinasyonu yok.
- Legacy CONTENT_REQUIRED SEO alanları artık page fallback'i ezmez; rich-text schema
  descriptions plain text'tir. Gerçek reviewed yasal body meta açıklamasını sağlar;
  geçerli Admin SEO override önceliği korunur. Yeni kayıt yayın validator'ları değişmedi.

## 6. İçerik için kullanıcıdan beklenen bilgiler

Onaylı biyografi/vizyon/baro ve varsa eğitim/mesleki geçmiş; gerçek e-posta/telefon/
adres/çalışma saatleri; avukat tarafından incelenmiş KVKK/aydınlatma/çerez/yasal uyarı;
özgün yayımlanmış çalışma alanı, makale ve SSS içerikleri. Bunlar CMS'den girilir.
/gizlilik için ayrı route/CMS alanı mevcut değildir; gizlilik kapsamının KVKK ile
birlikte mi ayrı sayfada mı sunulacağı hukuk/içerik kararı bekler. Sırf tasarım için
route/model/migration eklenmedi. TOC veya practice-area FAQ ilişkisi yok; boş bölüm
üretilmedi. Yayın kuralları gereği onaysız metinler public olmaz.

## 7. Gerçek hero fotoğrafı ve diğer medya eksikleri

Gerçek kişi/ofis fotoğrafı üretilmedi. Onaylı hero/portre fotoğrafı; kullanım izni,
alt metin, crop tercihi ve güvenli medya kaydı gerekir. Mevcut Profile modelinde
portre/education/history alanı yok; onaylı bilgiler biyografi editöründe sunulabilir.
Portre yer tutucusu yerel SVG'dir, kaynak kartvizit veya kimlik bilgisi yayımlanmadı.
Model eklemeden ayrı portre seçimi istenirse sonraki dar CMS/medya tasarımı gerekir.
Mevcut yer tutucuyla site çalışır; gerçek fotoğraf eksikliği güvenlik blocker'ı değil,
onaylanan görsel referansın fotoğraf yönünden tamamlanmamış farkıdır.

## 8. Veritabanı ve Plesk hazırlık durumu

Salt okunur cevodbtest gözlemi: 2026-10-11 20:59:04 UTC
(Europe/Istanbul: 11 Ekim 23:59:04). MariaDB 10.3.39, utf8mb4,
utf8mb4_general_ci, lower_case_table_names=1, TLS=false, 26 tablo, 8 migration kaydı;
beklenen tablo eksikliği yok. artifacts/test-runs/read-only-db-check.json kanıttır.
İlk araç kontrolündeki case-sensitive tablo karşılaştırması yanlış alarm verdi;
lower_case_table_names=1 doğrulanıp case-insensitive kontrolle düzeltildi.
Production cevodb sorgulanmadı; hiçbir row içeriği/PII veya credential raporlanmadı.
Ignored base JSON provider MariaDb/10.3.39; Development bunu miras alır. Bu uyumluluk
MySQL 8 mimari hedefini veya destek dışı/TLS riskini çözmez (ADR-032).

Publish provası başarılı: artifacts/releases/20261011-210557-f97e6158.
SHA-256 RELEASE_MANIFEST.json mevcut; 56 paket dosyası yeniden hash kontrolünde
0 fark gösterdi. dirtyWorkingTree=true, HEAD=60d4a6c.
Bu paket onaylı production release değil, yalnızca yerel prova çıktısıdır. İlk
20261011-210525-1270f7e6 provası appsettings dosyaları içerdiği için verifier tarafından
reddedildi. Csproj publish-exclude kuralı yorumdan çıkarıldı; başarısız paketin
üç config kopyası kaldırılıp DO_NOT_DEPLOY.txt ile işaretlendi. Asıl local/server
JSON dosyaları değiştirilmedi. Yeni paket server-owned config içermez. Gerçek server-owned
JSON, SQL/demo seed, özel belgeler, kullanıcı uploads/test fixture paket sınırı dışında
kalmalıdır. Windows/Plesk Hosting Bundle/out-of-process/IIS HTTPS/ACL/Skia/key-ring,
backup/restore ve monitoring talimatları mevcut runbooklarda; gerçek kanıt henüz yok.
Kısa operatör sırası: docs/deployment/REMAINING_LAUNCH_ACTIONS.md.

## 9. Yayına çıkışı engelleyen sorunlar

- Destekli DB/MySQL8 kararı ve gerçek production DB TLS/topoloji kanıtı açık.
  MariaDB10.3/TLS kapalı test erişimiyle kendiliğinden GO verilemez.
- SH-12-01 owner rotation bildirimi var; eski credential revocation kaydı yok.
- Production hukuk/içerik/contact onayları ve fake seed temizliği teyidi yok;
  DB yalnızca şema metadata'sıyla okundu, content audit yapıldı diye sayılmaz.
- Gerçek Plesk HTTPS/proxy/ACL/native, backup+izole restore, RPO/RTO ve rollback
  smoke kanıtı yok. .NET runtime destek/patch/yükseltme planı ADR-008'e göre teyit edilir.
- SMTP ve analitik mevcut local base ayarlarda kapalı; etkin özellik diye gösterilmez.
  Indexing ve Person/LegalService de false. İlgili owner kapsam/entegrasyon onayı bekler.
- Dirty çalışma ağacı otomatik commit edilmez; owner-approved release commit'i yoktur.

## 10. Yayın öncesi kalan manuel adımlar

1. Gerçek/onaylı CMS içerikleri ve medya; KVKK eksikse production form kapalı kalır.
2. Hosting destekli DB/TLS/runtime, yeni connection/eski credential iptali ve JSON ACL kanıtı.
3. Temiz onaylı commit'ten yeniden test/publish; aynı batch yedek/izole restore ve hash/RPO/RTO.
4. Gerçek domain HTTPS, public/admin/form/görsel/SEO smoke; SMTP/analitik gerekiyorsa teslim/onay testi.
5. Search Console DNS ve gerçek Business Profile; içerik/operasyon kapıları kapandıktan
   sonra indexing ve owner GO kararı. İzleme/SSL/disk/log sorumluları ve rollback paketi kaydı.

Commit önerileri (kendiliğinden uygulanmadı): önce korunmuş article/practice tasarım
paketi; sonra B–D public sayfalar/a11y; ardından E metadata/ADR; son olarak denetim
araçları ve operasyon raporları. Ortak site.css/browser dosyaları için hunk sınırları
incelemeyle ayrılır; user değişiklikleri topluca stage edilmez.
