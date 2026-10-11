# Tasarım son rötuşları ve organizasyon — 2026-10-12

Tarihsel rapor: burada belirtilen 47 inactive seed daha sonraki owner göreviyle
93 aktif izole fixture'a dönüştürüldü. Güncel kapsam/doğrulama:
[REPOSITORY_CMS_REVIEW.md](REPOSITORY_CMS_REVIEW.md). Tasarım sonuçları korunur.

İstenen altı aşamanın yerel uygulaması tamamlandı. Başlangıç HEAD: `9105423`;
başlangıç çalışma ağacı temizdi. Commit/push, deployment ve veritabanı bağlantısı
yapılmadı. Bu rapor canlıya çıkış onayı değildir; operasyonel NO-GO kapıları korunur.

## Klasör organizasyonu

| Önce | Şimdi |
| --- | --- |
| src/, tests/, tools/ | project/src/, project/tests/, project/tools/ |
| Kök solution, build/package props, global.json, .config/ | project/ altında |
| docs/design/ | design/ |
| docs/database/ | database/ |
| skills/, phases/, master/faz promptları, manifest | ai/ altında |
| Kök teknik rehberler ve CHANGELOG/DECISIONS | docs/ altında |
| Test/yayın çıktıları | artifacts/ altında, korunuyor |

Kök README, AGENTS ve CLAUDE keşif giriş noktaları olarak bırakıldı. `.git`, `.vs`,
yerel konfigürasyonlar ve ignored çalışma verileri korundu. Taşıma öncesi
[envanter](ORGANIZATION_PLAN.json) 434 izlenen dosyanın eski/yeni yolunu kaydeder;
434 hedef mevcut, kayıp hedef yok. Plan: [DESIGN_FINAL_PLAN.md](DESIGN_FINAL_PLAN.md).

Dosya silinmedi. Taşıma sırasında eski kaynak yolunda yeniden oluşan beş generated
obj dosyası `artifacts/move-recovery/legacy-generated-src` altında korundu; yalnızca
boş kalan klasörler kaldırıldı. Eski README [arşivlendi](../archive/README_PRE_REORGANIZATION.md).
Kaynak kartvizit fotoğrafları checkout'ta yoktu; kişisel kaynak görseller yayımlanmadı.

Solution içindeki src/tests göreli yolları değişmedi. Script çıktı yolları, scanner
kökü, ignore/editor kapsamları, manifest, prompt ve doküman bağlantıları güncellendi.
Yerel gizli appsettings dosyaları yeni yolda da Git dışında. Yeni paket eklenmedi.
Yeni solution'ı IDE'de yeniden açın: `project/CevatBiyik.sln`.
Güncel çalışma dizini/komut rehberi: [REPOSITORY_LAYOUT.md](../REPOSITORY_LAYOUT.md).
Geçmiş raporlardaki tarihsel komutlar yeni test icrası olarak yorumlanmamalıdır.

## Public arayüz

Tek `_PublicBreadcrumb.cshtml` partial'ı layout içinde, main altında ve içerik
başlığından önce render edilir. Okuma sütunlarının daraltması/ortalaması breadcrumb'ı
etkilemez. Ana sayfada gösterilmez; aktif öğe `aria-current="page"` ile işaretlidir.
Liste/detay makaleler, çalışma alanları, konumlar (üst konum hiyerarşisi dahil),
SSS, hakkımda, iletişim, KVKK, çerez politikası ve yasal uyarı bu yapıyı kullanır.
Eski sayfa breadcrumb blokları ve gereksiz ayrı CSS kuralları kaldırıldı.
URL ve mevcut JSON-LD hiyerarşisi korunur; metinler HTML encode edilir.

Header'ın dikdörtgen CTA metni tam olarak **İletişim**, hedefi `/iletisim`.
Mobil menü ve diğer bağlantılar korunur. Hero içindeki farklı görüşme CTA'sı
header değişikliği kapsamına girmez.

## Hero görseli ve CMS

Görsel yerleşik imagegen aracıyla üretildi; gerçek ofis veya kişi fotoğrafı değildir.
Prompt ve varlık bilgileri: [REPRESENTATIVE_OFFICE.md](../../design/generated/REPRESENTATIVE_OFFICE.md).
Kaynak: `design/generated/representative-law-office.png`, **1122 × 1402 px**.
Public WebP varlıkları `project/src/CevatBiyik.Web/wwwroot/images/hero/` altında:

| Dosya | Ölçü | Byte |
| --- | --- | ---: |
| representative-law-office-480.webp | 480 × 600 | 38580 |
| representative-law-office-768.webp | 768 × 960 | 77610 |
| representative-law-office-1122.webp | 1122 × 1402 | 131980 |

Responsive srcset/sizes, açık width/height, eager yükleme, high fetchpriority ve
async decode kullanılır. Fallback görselinin yanında “Temsili hukuk ofisi — yapay
zekâ ile oluşturulmuştur.” görünür. Yerel resmî SVG değiştirilmedi.

Mevcut IMediaService üzerinden isteğe bağlı `Home.HeroMediaId` ayarı eklendi.
Admin ayar ekranında yalnızca bu anahtar seçeneği ve açıklaması eklendi; ayrı medya
modülü veya Admin tasarım değişikliği yok. Onaylı görseli mevcut medya ekranından
yükleyip ID'yi ayara girin. Aktif/geçerli medya, alt metin ve güvenli yerel upload yolu
doğrulanır; bulunamayan/silinmiş/geçersiz kayıt yerel fallback'e döner.
Domain, veritabanı modeli ve migration değişmedi.

## Taslak seed

[SQL](../../database/seeds/REVIEW_DRAFT_CONTENT.sql),
[okunabilir içerik](../../database/seeds/REVIEW_DRAFT_CONTENT.json) ve
[ön izleme/onay listesi](../../database/seeds/REVIEW_DRAFT_PREVIEW.md) hazırdır.

En fazla 47 yeni kayıt: 6 çalışma alanı, 3 kategori, 6 etiket, 6 makale,
6 makale-etiket bağı, 3 SSS, 4 sayfa içeriği, 4 sayfa SEO, 1 profil ve 8 ayar.
Yeni içerikler inactive/unreviewed; makale yayın tarihleri NULL'dır. ID rezervi
900100001–900100117. FK sırasına uygun conditional INSERT, transaction ve utf8mb4
kullanılır. Mevcut doğal anahtarlar korunur; UPDATE/DELETE/DDL/FK bypass yoktur.
ID çakışmasında istemci ilk hatada durmalı ve rollback yapılmalıdır.

Identity, iletişim başvurusu, audit, bildirim, özel belge ve migration geçmişine
sahte kayıt eklenmez. Fiziksel medya olmadığı için medya satırı; doğrulanmış adres
olmadığı için lokasyon; gerçek ihtiyaç olmadığı için redirect oluşturulmadı.
Biyografi/baro/iletişim ve hukuki sayfa metinleri gerçek veri/onay bekler.
Hizmet alanları gerçek hizmet iddiası değil, onaysız taslaktır.

Motor dayanağı önceki read-only test teşhisidir: MariaDB 10.3.39. Bu görev yeni DB
bağlantısı açmadı. Temiz izole şema bulunmadığından SQL **çalıştırılmadı**;
gerçek engine transaction/idempotence testi tamamlanmış sayılmaz. İki offline test
EF modeliyle alan/uzunluk/yayın durumlarını ve SQL güvenlik sınırlarını doğruladı.
Yetkili operatör önce boş disposable şemada migration'ları ve iki seed çalıştırmasını
doğrulamalıdır; dolu cevodbtest veya production üzerinde otomatik uygulama yoktur.

## Doğrulama

`project/` çalışma dizininde `pwsh -NoProfile -File tools/test.ps1 -Browser` çalıştırıldı.
Test için AllowedHosts ve Site BaseUrl process ayarları verildi; gerçek SMTP/DB
kullanılmadı. Son Release build: **0 hata, 0 uyarı**.

| Kontrol | Sonuç |
| --- | --- |
| .NET unit | 211 geçti |
| .NET integration | 239 geçti, 11 DB opt-in atlandı |
| JavaScript | 6 geçti |
| Browser E2E/public/Admin | 6 + 9 + 5 = 20 geçti |
| Scanner self-test / publish boundary | 8 / 4 geçti |
| Stil ve üretilen CSS/JS parity | Geçti |
| Taşıma envanteri / PowerShell parsing / yerel Markdown linkleri | Geçti |

Breadcrumb ve public düzen 320, 390, 768, 1440 px genişliklerde kontrol edildi:
yatay taşma/öğe çakışması yok; CTA metin/hedefi ve mobil menü geçerli.
Ekranlar: `artifacts/test-runs/final-*.png` ve `home-refined-{320,390,768,1440}.png`.
Yeni hero masaüstü/mobil ve SSS/makale detay screenshot'ları ayrıca görsel incelendi.
Eski `home-design-*` görüntüleri bu son sürümün kanıtı değildir.
Mevcut SEO, güvenlik, safe HTML, CMS ve Admin akışları regresyon testlerinde geçti.
Bu sonuç tüm WCAG 2.2 AA için bağımsız sertifika veya yeni Lighthouse ölçümü değildir.

Yerel publish provası `artifacts/releases/20261011-213056-66d763d0` altında üretildi;
publish verifier geçti. Gizli server JSON, upload/özel dosya, SQL, tasarım kaynağı
pakete alınmadı. Paket dirty working-tree provasıdır, canlı yayın yapılmadı.

## Etkilenen uygulama dosyaları

- Web: HomeController, HomeViewModel, PublicSiteSettings, yeni PublicBreadcrumb;
  _Layout, yeni _PublicBreadcrumb, Home/Index ve eski breadcrumb kullanan public views;
  site.css ve builder tarafından oluşturulan asset; yerel hero WebP'leri.
- Application: PublicSettingKeys, UpdateSiteSettingValidator (isteğe bağlı medya ID).
- Admin: Settings/Edit içinde tek anahtar seçeneği/açıklaması.
- Test: PublicBreadcrumbTests, DraftSeedTests, HomeHeroTests ve browser senaryoları.
- Organizasyon: solution/build/tool yolları, ignore/editor ayarları ve rehber referansları.
- Mimari kayıt: [ADR-036](../DECISIONS.md); [CHANGELOG](../CHANGELOG.md).

## Kalan manuel işler

1. IDE'de yeni solution yolunu açın; yerel Development konfigürasyonu taşındı ve korunuyor.
2. Taslak makale/SSS/hizmet kapsamını avukat incelemeli; profil ve iletişim gerçek bilgilerle
   tamamlanmalı. KVKK/çerez/yasal metinler gerçek veri işleme süreçlerine göre hazırlanmalı.
3. Seed için temiz izole test şeması sağlayıp gerçek motor/idempotence doğrulaması yapılmalı.
4. İstenirse onaylı gerçek hero görseli CMS'ye yüklenip Home.HeroMediaId ayarlanmalı.
5. Canlı SSL/SMTP/backup-restore/SH-12-01 operasyon kanıtları mevcut launch rehberlerine
   göre ayrı doğrulanmalı. Yerel tasarım testleri bu kapıları otomatik kapatmaz.

İstenen kapsam dışında yeni faz başlatılmadı; commit ve push owner talimatını bekler.
