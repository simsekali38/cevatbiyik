# Güvenli repository temizliği — 2026-10-12

## 1. Başlangıç HEAD ve çalışma ağacı

Repository: D:/Projects/cevat-biyik-av-tr; .git kökte korundu.
HEAD `c42fce1`. Başlangıç Git durumu:

```text
?? docs/qa/REPOSITORY_CLEANUP_AUDIT.md
```

Önceki untracked inceleme raporu byte hash ile doğrulandı, değiştirilmedi. Staged
kullanıcı değişikliği yoktu. Ignored cache/test sonuçları envanterden ayrıldı.

## 2. Düzeltilen artifact yolları

- CmsPreview.cs ve ContactAnalyticsTests.cs artık `RepositoryPaths.Artifacts`
  üzerinden kökteki artifacts/cms-preview yoluna gider.
- test_publish.py, __file__ konumundan yukarı çıkarak aynı checkout marker'larını
  (.git ve project/CevatBiyik.sln) bulur; artifacts/publish-tests kökte oluşur.
- Çalışma dizini/IDE konumundan bağımsız aynı root bulma kuralı kullanıldı. C# helper
  yalnız test projesindedir; production runtime değişmedi.
- CEVATBIYIK_CMS_PREVIEW=1 koşulu ve mevcut export davranışı korunur. Dar koşuda
  flag 0 tutuldu; eski HTML/visual kanıtları yeniden export edilip ezilmedi.
- Yeni testler farklı source başlangıç noktalarını ve root altındaki geçici dosyaların
  cleanup davranışını kontrol eder. Python gerçek komutla project/ ve artifacts/
  çalışma dizinlerinden ayrı doğrulandı.

## 3. Silinen yollar ve gerekçeler

Silme sadece doğrulanmış dar literal yollarla native PowerShell üzerinden yapıldı.
Git clean/wildcard recursive silme, shell'ler arası dosya taşıma veya kullanıcı
verisini yedekleyip toplu silme yapılmadı. Kullanılan hedefler ve TRX kararları
artifacts/REPOSITORY_CLEANUP_PLAN.json içinde; silinen TRX yolları ayrıca
artifacts/REPOSITORY_CLEANUP_TRX_ACTIONS.json içindedir.

| Grup | Silinen dosya | Mantıksal boyut | Kapsam |
| --- | ---: | ---: | --- |
| project src/tests bin | 1947 | 615.29 MiB | Kısmi: konfigürasyon ve belirsiz kaynak/çıktılar kaldı |
| project src/tests obj | 282 | 64.52 MiB | Yeniden üretilebilir dosyalar; kalanlar korundu, restore yeniden üretir |
| project/tools/__pycache__ | 2 | 0.01 MiB | Önce tamamen temizlendi; Python testinde cache yeniden oluşabilir |
| artifacts/ide-cache | 12 | 9.86 MiB | Eski cache yedeği tamamen kaldırıldı |
| project/.vs DesignTimeBuild | 1 | 0.19 MiB | Kısmi IDE cache temizliği; indeks kilidinde durduruldu |
| Eski başarılı referanssız TRX | 137 | 217.26 MiB | TestResults klasörleri topluca silinmedi |

Başlangıçta bilinen IDE/build/test process isimleri için kullanım kontrolü yapıldı;
aktif süreç bulunmadı. Buna rağmen FileContentIndex içindeki bir dosya başka bir
süreç tarafından kilitli çıktı. Force unlock/process kill yapılmadı; IDE cache
silme burada durdu. İlk işlemin log kapanışı bu hatayla kesildiği için ilk cache
kazancı, rebuild başlamadan önce envanter-before/after ile yeniden hesaplandı;
REPOSITORY_CLEANUP_EXECUTION.json bunu açıkça kaydeder. TRX işlemi ayrı loglandı.

## 4. Korunan adaylar

- 20 eski TRX: son başarılı dar/full koşular, açık belge referansları ve belirsiz/
  başarısız tarihsel kanıtlar. Yeni dar testin TRX'i ayrıca korundu (toplam 21).
- 15 appsettings çıktı kopyası silme filtresinden çıkarıldı; gerçek source
  appsettings dosyaları ve sırlar okunmadı, kopyalanmadı veya değiştirilmedi.
- project/.vs applicationhost.config, DocumentLayout ve backup mevcut; IDE ayarları
  silinmedi. FileContentIndex kilitli olduğu için kalan indeks/Copilot cache'leri kaldı.
- artifacts/cms-preview-host tamamen korundu: solution dışında ve kırık proje
  referansı var, fakat iki özgün untracked kaynak dosyasının güvenle yeniden
  üretilebildiği kanıtlanamadı. İkinci büyük kopya oluşturulmadı.
- artifacts/phase07-publish korundu: tarihsel QA referansı var; eşdeğer orijinal
  kanıt/rollback paketi ayrıştırılmadığı için silme koşulu sağlanmadı.
- test.txt silme listesine alınmadı: ignored 18-byte dosyanın güncel özgün içerik
  taşımadığı kesin kanıtlanamadı. Son kontrolde dosya artık mevcut değildi; bu
  kapsam dışı değişiklik bizim silme işlemi/kazancımız olarak sayılmadı. Dosya Git
  dışında olduğu ve kullanıcı içeriği okunmadığı için bir restore işlemi yapılmadı.
- Yedi releases paketi, node_modules, test-runs, phase14/16, EF araç/migration/SQL,
  legacy seed ve asset ZIP'leri, public CSS/JS/min/SVG/WebP/lisanslar korunur.
- Boş .github/workflows/PublishProfiles ve mevcut boş scratch klasörleri sırf görsel
  düzen için kaldırılmadı. Uploads/App_Data/key-ring üzerinde işlem yapılmadı.

### Owner'ın kapsam dışı tasarım silmeleri

Çalışma sırasında design/ altındaki yedi PNG ve UXMagic HTML silindi. Bunlar bizim
silme hedeflerimiz arasında değildi. Owner ayrıca “Evet, ben kaldırdım; silinmiş
kalsın” yanıtıyla teyit etti. Değişiklik geri alınmadı ve temizlik kazancına eklenmedi.
Sekiz path REPOSITORY_CLEANUP_FINAL.json içinde ayrı listelenir. Bunun dışında
korunan 14.838 dosyanın mevcut metadata karşılaştırmasında beklenmeyen değişiklik yok.

## 5. Boyut / kazanç

İlk temizlikte kaldırılan mantıksal toplam **951,198,079 byte
(907.13 MiB)**. Dosya grupları bir kez sayıldı.
Restore/build/test sonrası bazı gerekli çıktılar yeniden üretildi. Aynı envanter
kategorilerinde final net mantıksal fark **754,600,708 byte
(719.64 MiB)**.

Bu gerçek disk allocation/free-space ölçümü değildir; NTFS sıkıştırma/hardlink,
Git object storage ve aynı anda başka işlemlerin disk kullanımını kapsamaz.
Owner'ın design silmeleri bu iki tutarın dışında tutuldu. Derleme çıktılarının
ileride büyümesi normaldir; doğrulama çıktılarını tekrar silip kullanılmaz hale getirilmedi.

## 6. Çalıştırılan kontroller

- Etkilenen IntegrationTests restore: başarılı. Ardından solution restore ile
  Unit/E2E obj metadata'sı da geri yüklendi; yeni package/dependency eklenmedi.
- IntegrationTests Release build: **0 hata / 0 uyarı** (referanslı dört katman dahil).
- RepositoryPathsTests + CmsFlowTests + ContactAnalyticsTests: **20 geçti, 0 fail,
  0 skip**. Önceki test fixture'ları izole in-memory SQLite/yerel mock kullanır;
  dış DB, SQL seed veya migration çalıştırılmadı.
- TRX: project/tests/CevatBiyik.IntegrationTests/TestResults/
  cleanup_artifact_paths_20261011_230331.trx (UTC dosya adı; rapor tarihi İstanbul).
- Python publish-boundary/scratch: **5 geçti**. project/ ve artifacts/ CWD ile
  çalıştı; root artifacts/publish-tests altında geçici dosya sayısı final **0**.
- C# path probe klasörleri final **0**; root marker ve çalışma konumları kontrol edildi.
- git diff --check: geçti. Beklenen LF/CRLF advisory, whitespace hatası değildir.
- Mevcut QA raporu byte hash aynı, korunan TRX/config output dosyaları mevcut,
  release/node_modules/kanıt/source config metadata'sında beklenmeyen değişiklik yok.
- Yerel Markdown bağlantı taraması: 0 kırık bağlantı.

Tam .NET/JS/browser regresyonu, SQL seed, gerçek DB/migration, publish ve deploy
çalıştırılmadı. Bu sonuçlar yeni tam regresyon veya production doğrulaması değildir.

## 7. Son Git durumu

Aşağıdaki çıktı report oluşturulduktan sonra kaydedildi. Staging/commit/push yok;
HEAD değişmedi. İzlenen silmeler yalnız owner'ın sekiz design silmesidir. Üç
izlenen kaynak değişikliği test/scratch yol düzeltmeleridir. İki yeni C# test dosyası
ve bu maintenance raporu eklendi. Önceki untracked QA raporu korundu.

```text
 D "design/Ana Sayfa-export.png"
 D "design/Av. Cevat Bıyık _ Hukuk & Danışmanlık-export (3).png"
 D design/Hakkımda-export.png
 D "design/Makale Detayı-export.png"
 D design/Makaleler-export.png
 D design/uxmagic-homepage.html
 D "design/Çalışma Alanları-export.png"
 D "design/Çalışma Alanı Detayı-export.png"
 M project/tests/CevatBiyik.IntegrationTests/CmsPreview.cs
 M project/tests/CevatBiyik.IntegrationTests/ContactAnalyticsTests.cs
 M project/tools/test_publish.py
?? docs/maintenance/
?? docs/qa/REPOSITORY_CLEANUP_AUDIT.md
?? project/tests/CevatBiyik.IntegrationTests/RepositoryPaths.cs
?? project/tests/CevatBiyik.IntegrationTests/RepositoryPathsTests.cs
```

Tracked diff stat (yeni untracked dosyalar bu sayımda yok):

```text
 design/Ana Sayfa-export.png                        | Bin 520923 -> 0 bytes
 ...an\304\261\305\237manl\304\261k-export (3).png" | Bin 2100903 -> 0 bytes
 "design/Hakk\304\261mda-export.png"                | Bin 183853 -> 0 bytes
 "design/Makale Detay\304\261-export.png"           | Bin 217902 -> 0 bytes
 design/Makaleler-export.png                        | Bin 117723 -> 0 bytes
 design/uxmagic-homepage.html                       | 189 ---------------------
 ...l\304\261\305\237ma Alanlar\304\261-export.png" | Bin 136223 -> 0 bytes
 ...05\237ma Alan\304\261 Detay\304\261-export.png" | Bin 214018 -> 0 bytes
 .../CevatBiyik.IntegrationTests/CmsPreview.cs      |   2 +-
 .../ContactAnalyticsTests.cs                       |   2 +-
 project/tools/test_publish.py                      |  19 ++-
 11 files changed, 20 insertions(+), 192 deletions(-)
```

## 8. Manuel karar / sonraki görev

- Eski preview-host'un iki kaynak dosyası gerekmiyorsa ayrı açık kararla kaldırılabilir.
- FileContentIndex kilidini tutan IDE/service kapandıktan sonra yalnız kalan cache
  tekrar değerlendirilebilir; bu görev süreç kapatmadı.
- Phase07 artifact ve yedi release için kanıt/rollback saklama politikası ayrı belirlenmeli.
- Legacy seed birleştirme/taşıma bu kapsamda yapılmadı.

Detaylı metadata plan/işlem/final JSON'ları ignored artifacts/ altında; hassas değer
raporlanmadı. Runtime tasarım/CMS/SEO/güvenlik kodu değiştirilmedi.
