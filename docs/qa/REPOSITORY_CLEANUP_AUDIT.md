# Kullanılmayan dosya ve klasör denetimi — 2026-10-12

Başlangıç HEAD `c42fce1`, çalışma ağacı temiz. Bu görev incelemedir: kaynak/config
değiştirilmedi, dosya silinmedi, DB bağlantısı/commit/push yapılmadı. Bu rapor ve
ignored `artifacts/REPOSITORY_UNUSED_INVENTORY.json` oluşturuldu.

## Sonuç

524 izlenen dosya incelendi. Diskteki kalabalığın ana kaynağı ignored derleme,
test ve yayın çıktılarıdır. Üretim kaynakları içinde **silinmesi gerektiği kanıtlanan
bir C#/Razor/CSS/JS dosyası bulunmadı**. Bu, tam statik program analiziyle bütün
metotların kullanıldığı iddiası değildir. İsim/referans taraması, solution üyeliği,
Razor convention, DI ve EF assembly scanning birlikte değerlendirildi.

## Temizlik adayları

| Yol / grup | Yaklaşık boyut | Değerlendirme |
| --- | ---: | --- |
| project/**/bin ve obj | 770,46 MiB | Yeniden üretilir. Çalışan uygulama/IDE kapatıldıktan sonra temizlenebilir; restore/build gerekir |
| artifacts/cms-preview-host | 136,84 MiB | En güçlü kullanılmayan aday: eski ad-hoc host; solution/script referansı yok; csproj'nin ../../tests/... referansı artık bulunmuyor |
| artifacts/releases | 413,26 MiB / 7 paket | Eski yayın provaları. Onaylı release/rollback kopyası seçilmeden topluca silinmez; eski paketler saklama politikasıyla ayrılmalı |
| project/tests/*/TestResults | 251,59 MiB / 157 dosya | TRX birikimi. Son unit/integration ve referans verilen faz kanıtları korunarak tekrar koşular daraltılabilir |
| artifacts/phase07-publish | 52,97 MiB | Faz 07 tarihsel publish çıktısı; QA raporu referanslıyor. İlgili kanıt arşivlenirse çalışma kopyası kaldırılabilir |
| artifacts/ide-cache | 9,86 MiB | Önceki geri alınabilir cache yedeği; solution taşınması doğrulandı. Kullanıcı ayarı değil, teknik cache; süreli saklama adayı |
| project/.vs | 16,69 MiB | Proje açılınca yeniden oluşur. applicationhost.config ve DocumentLayout kullanıcı/IDE bilgisi içerir; klasörün tümü aynı şekilde değerlendirilemez |
| project/**/__pycache__ | 0,01 MiB | Yeniden üretilir; Python cache |
| test.txt | 18 byte | İlk erişim denemesinden kalan ignored dosya; güncel build/test kullanımı yok. Tarihsel discovery raporu adını anıyor |
| Boş .github/workflows, PublishProfiles ve bazı test scratch dizinleri | 0 | Görsel kalabalık; disk kazanımı yok. Aktif CI workflow dosyası bulunmadı |

Sayılar mantıksal dosya boyutudur; gerçek disk allocation ve Git object storage
hesabı değildir. Aynı dosya grupları farklı satırlarda sayılmış olabilir; tabloyu
körlemesine toplayıp garantili kazanım olarak yorumlamayın. Örneğin yalnız project
bin/obj ve eski preview-host yaklaşık **907 MiB** çalışma kopyası tutuyor.

## Gereksiz görünse de korunması gerekenler

- `project/tests/CevatBiyik.BrowserTests/node_modules`: 162,11 MiB; test ve asset
  minifier bağımlılığı. Uygulamanın canlı runtime'ı değildir ama yerel testte kullanılır.
  Silinirse kilitli dependency restore gerekir; default ilk temizlik hedefi değildir.
- CSS/JS kaynakları ve `.min` dosyaları: kaynaklar builder tarafından okunur,
  minified dosyalar Razor'da servis edilir. İkisi gereksiz kopya değildir.
- Üç responsive hero WebP, SVG amblem, Bootstrap/Quill: Razor/JS/build referansları
  var. Vendor LICENSE/README dağıtım/bakım belgeleridir; runtime isteği yok diye silinmez.
- EF configuration sınıfları `ApplyConfigurationsFromAssembly` ile yüklenir.
  İsim aramasında çağrısı görünmeyen config'ler kullanılmıyor sayılmaz. ArticleValidator
  aynı dosyada türetilir; CmsRelations aynı dosyadaki servis metotlarında kullanılır.
- Domain migrations ve migration SQL/notları: şema/yükseltme zinciri ve operasyonel
  kanıt; eski tarihli oldukları için gereksiz değildir.
- Özel belgeler kodu: özellik public/Admin menüde kapalı olsa da model, dosya kataloğu,
  güvenlik ve test bağımlılıkları var. Kalıcı kaldırma ayrı kapsam/şema kararı gerektirir.
- `design/` referansları ve generated kaynak PNG: runtime dışında ama tasarım kanıtı.
- `docs/qa`, `ai/phases`, mimari rehberler: owner sözleşmesi/faz geçmişi; faz kapandı
  diye silinmez. Gerekirse bağlantılar korunarak arşivlenir.
- `artifacts/ef8`: belgelerde kullanılan yerel EF CLI. Güncel dotnet tool manifest
  aynı EF sürümünü sağlar; komut rehberleri tek yönteme geçmeden kaldırılmamalı.
- `artifacts/test-runs`, phase14 ve phase16: güncel/önce-sonra ölçüm ve test kanıtları;
  bu denetimde gereksiz kabul edilmedi.
- appsettings, uploads, App_Data, key-ring ve yerel sırlar: bu denetim temizlik hedefi
  değildir. İçerikleri okunmadı veya rapora yazılmadı.

## Yinelenen seed ve görsel setleri

Üç farklı seed yaklaşımı var; birbirinin aynısı değil:

1. `database/seeds/REVIEW_DRAFT_CONTENT.*`: güncel 93 kayıt CMS fixture'ı.
2. `database/PROD_SAMPLE_DATA.*`, `prod-sample-assets/` ve zip: eski 264 kayıt/all-table
   fixture. `project/tools/Test-ProdSampleData.ps1` ve belgeler kullanıyor. Identity ve
   operasyonel fixture kapsamı güncel CMS seed'den farklı; varsayılan seçenek olmamalı.
3. `DemoDataSeeder.cs` / E2EHost `DemoCommand`: eski opt-in seed/status/cleanup yolu;
   CLI ve DemoDataTests referansları var. Sessizce silmek test/cleanup akışını bozar.

Öneri: tek güncel seed girişini açıkça belirtmek; diğer iki akışı tool/test/link
referanslarıyla birlikte `legacy` olarak ayırmak. Bu denetimde otomatik taşınmadı.

Hash karşılaştırmasında yalnız iki izlenen dosya birebir aynı çıktı:
`database/prod-sample-assets/images/ded00000000000000000000000000006.webp` ve
`...00007.webp` (her biri 15.568 byte). İki farklı fixture storage kaydı ve manifest
bunları ayrı dosya olarak bekler. Fiziksel duplicate olması silmeye yeterli değil.
Zip ile açılmış asset klasörü de teslim paketi/inceleme kopyası ilişkisine sahip.

## Klasörlerin yeniden dağılmasına neden olabilecek yol hataları

Üç yer hâlâ application root içine artifact üretir:

- `project/tests/CevatBiyik.IntegrationTests/CmsPreview.cs:15`
- `project/tests/CevatBiyik.IntegrationTests/ContactAnalyticsTests.cs:71`

Bu iki opt-in helper Web content root'tan `../../artifacts/cms-preview` çözer;
yeni yapıda hedef repository root yerine **project/artifacts/cms-preview** olur.
Preview yalnız CEVATBIYIK_CMS_PREVIEW=1 olduğunda yazılır; normal testte çalışmaz.

- `project/tools/test_publish.py:8`: parents[1] / artifacts da project/artifacts
  üretir. Temp dosyaları test sonunda temizlenir; scratch klasörü kalabilir.

Bu bir kaynak kullanım/yol bulgusudur; uygulamanın public runtime'ına müdahale
etmez. Sonraki temizlikte yollar tek root resolver'a taşınmalı; sadece dosyaları
silmek bu tekrar oluşum nedenini çözmez.

## Çalıştırılan kontroller

- Git durum/envanter, 524 tracked dosyanın hash ve referans incelemesi.
- Solution list: 7 proje bulundu; eski preview-host solution'da yok.
- CSS/JS asset parity geçti; seed SQL/JSON/manifest parity geçti.
- docs/design/ai yerel Markdown bağlantıları: 0 kırık bağlantı.
- Gerçek dosya içerikleri yerine cache/runtime klasörlerinde metadata/size incelemesi.
- Derleme, .NET/JS/browser regresyonu yeniden çalıştırılmadı: uygulama kodu ve
  konfigurasyonu değişmedi. Önceki test sonuçları yeni icra edilmiş gibi raporlanmaz.

## Önerilen temizlik sırası

1. Üç yanlış artifact yolunu düzeltip ilgili opt-in/scratch kontrollerini doğrulamak.
2. Eski orphan preview-host, Python cache ve çalışmayan bin/obj kopyalarını temizlemek.
3. Güncel test kanıtı ve belirlenmiş release/rollback kopyasını koruyarak geçmiş
   TRX ve publish çıktılarına saklama sınırı koymak.
4. Legacy seed araçlarını ayrı kategoride tutup güncel CMS seed'i tek giriş yapmak.

Bu sıra kullanıcı dosyaları/test kanıtları korunarak uygulanabilir. İnceleme sonucu
hazırdır; kaynak dosyalar veya geçmiş kanıtlar bu görevde silinmemiştir.
