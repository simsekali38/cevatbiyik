# Repository temizliği ve tam içerikli CMS seed — 2026-10-12

Son owner talimatı: SQL hedefi cevodbtest olarak değişti. Mevcut doğal anahtarlar
korunur; parent FK değerleri mevcut ID üzerinden çözülür. Aşağıdaki ilk icranın
cevo_cms_seed_isolated/boş şema açıklaması tarihsel olup güncel talimat
database/seeds/REVIEW_DRAFT_PREVIEW.md içindedir. Harici SQL halen burada çalıştırılmadı.

Owner'ın yeni kapsamı uygulandı. Önceki tasarım/breadcrumb/SEO/güvenlik/Admin/hero
çalışmaları korundu. Commit/push/hosting/deployment ve dış DB bağlantısı yapılmadı.
Başlangıç HEAD `9105423`, staged dosya yok; önceki görevden kalan 513 status satırı
korundu. Bu rapor production içerik veya canlıya çıkış onayı değildir.

## Repository

Başlangıç kök envanteri: [REPOSITORY_CMS_INVENTORY.json](REPOSITORY_CMS_INVENTORY.json).
Önceki 434 dosya taşımasının envanteri [ORGANIZATION_PLAN.json](ORGANIZATION_PLAN.json)
korundu. Ana yapı zaten istenen project/docs/design/ai/database/artifacts düzenindeydi;
kaynakları tekrar taşımak gerekmiyordu. Solution `project/CevatBiyik.sln`.

| Öğe | İşlem / gerekçe |
| --- | --- |
| .git, .github | Kök yerleşim korundu; repository taşınmadı |
| README, AGENTS, CLAUDE | Kök keşif rehberleri korundu |
| .gitignore, .editorconfig, .gitattributes | Kök kapsam/kurallar korundu |
| solution, .config, SDK/build/package props | project/ yerleşimi korundu |
| docs/design/ai/database/artifacts | Mevcut kategoriler ve önemli dosyalar korundu |
| .vs/config, sohbet ve kullanıcı ayarları | Kullanıcı/IDE bilgisi olduğu için korundu |
| DesignTimeBuild, FileContentIndex, CopilotIndices, ProjectEvaluation | Yeniden üretilebilir cache; artifacts/ide-cache/20261012/ altına geri alınabilir taşıma |
| test.txt, secrets, uploads, App_Data, test kanıtları | Silinmedi; Git dışı veriler korundu |

Cache taşıması öncesi Visual Studio'nun çalışmadığı ve her kaynak/hedefin workspace
içinde kaldığı kontrol edildi. Toplu silme yapılmadı. Taşınan dört klasörün ayrıntısı:
[IDE_CACHE_ACTIONS.json](IDE_CACHE_ACTIONS.json). Kalıcı dosya silme **0**.
Kalan .vs klasörünü otomatik gereksiz sayıp silmek kullanıcı ayarı/sohbet kaybettirebilir.

Eski bir faz rehberindeki format solution yolu düzeltildi. Araçlar project/ içinde
çalışır; çıktı kök artifacts/'e gider. Güncel rehber:
[REPOSITORY_LAYOUT.md](../REPOSITORY_LAYOUT.md). IDE'de yeni solution'ı açmak gerekir;
editör arayüzü otomasyonla açılmadı, solution CLI build/test ile doğrulandı.

## İçerik

[SQL](../../database/seeds/REVIEW_DRAFT_CONTENT.sql),
[JSON](../../database/seeds/REVIEW_DRAFT_CONTENT.json),
[manifest](../../database/seeds/REVIEW_DRAFT_MANIFEST.json),
[ön izleme/talimatlar](../../database/seeds/REVIEW_DRAFT_PREVIEW.md).

**93 kayıt:** 16 çalışma alanı, 4 kategori, 6 etiket, 12 makale, 21 makale-etiket bağı,
12 SSS, 4 sayfa içeriği, 9 sayfa SEO, 1 profil, 8 site ayarı.
Makale gövdeleri 316–348 kelime; konuya özel altı bölüm, hazırlık listesi ve gerçek
route ile ilgili çalışma alanı bağlantısı içerir. Çalışma alanlarında özgün kapsam,
temel kavram, belge hazırlığı, süreç ve dikkat başlıkları bulunur. İçerik gövdelerinde
CONTENT_REQUIRED, boş başlık veya demo/test metni yoktur.

Alanlar: Ceza; İdare; Aile; İş; KVKK Danışmanlığı; Ticaret; Tüketici; İcra ve İflas;
Sözleşmeler; Miras; Gayrimenkul ve Taşınmaz; Vergi; Sigorta; Fikrî ve Sınaî Mülkiyet;
Basın ve Medya; Dava ve Tahkim. ASCII slug/route tablosu ön izlemededir.
Alanların Av. Cevat Bıyık tarafından gerçekten sunulduğu iddia veya teyit edilmez.

Biography eğitim/mesleki yaklaşımı içeren kurgusal anlatıdır; gerçek kurum/yıl/sicil
uydurulmaz. Model ayrı eğitim sütunu desteklemez. Vizyon/baro/iletişim alanları
uzunluk/format ve tasarım testi için doludur. E-posta example.invalid; telefon ve
WhatsApp +90 000 ile routable olmayan değerlerdir. Site adı nötrdür. Örnek mahalle,
adres, saat ve yayın tarihleri gerçek kişi bilgisi değildir; manifestte listelenmiştir.

KVKK/çerez/yasal uyarı sayfaları doludur. Genel çerçeve ile gerçek veri işleme
uygulamasının doğrulanması ihtiyacı ayrılır; olmayan analytics/SMTP/aktarım veya
kesin saklama süresi ilan edilmez. KVKK sayfası tam hukuki aydınlatma sayılmaz.
Gerçek bilgiler ve avukat onayı olmadan bu test sayfaları canlıya taşınamaz.

Medya, lokasyon ve redirect bilinçli eklenmedi: fiziksel dosya/teyitli adres/gerçek
legacy URL yok. Identity, iletişim başvurusu, audit, notification ve özel belgelere
sahte operasyonel kayıt yok. Test harness'in mevcut geçici Identity hesapları SQL
seed'inin parçası değildir. Domain, servis, query filter, schema ve migration değişmedi.

## SQL sınırı

Exact hedef **cevo_cms_seed_isolated**. Her INSERT DATABASE() koşuluyla başka
şemada no-op olur. Bu başarı kanıtı değildir; manifest sayıları ayrıca kontrol edilir.
Yeni şema boş, disposable ve ayrı yetkili hesapla hazırlanmalı; dolu cevodbtest veya
cevodb kullanılmamalıdır. ID rezervi 910200001–910200157. FK sırası/unique indeksler
devrededir; doğal anahtarlar sessizce güncellenmez. Transaction, ilk hatada durma ve
rollback zorunludur; --force/DDL/FK bypass/UPDATE/DELETE/hesap açma yok.
UTF-8 değerler hex literal ile yazılır; quote/backslash SQL mode farkından etkilenmez.

IsActive/IsReviewed=1 ve geçmiş UTC PublishedAt yalnız test görünürlüğü içindir;
hukuki inceleme veya owner yayın onayı değildir. Manifest indexing/SMTP/analytics/
Person/LegalService kapalı test ayarlarını kaydeder. SQL otomatik startup'a veya
production bağlantısını kullanan araca bağlanmadı. Mimari gerekçe: [ADR-037](../DECISIONS.md).

Motor kaynağı önceki read-only MariaDB 10.3.39 teşhisidir; bu görev yeni dış DB bağlantısı
kurmadı. Temiz yetkili şema yok: **MariaDB migration/seed/ikinci çalıştırma/rollback
testleri yapılmadı**. Offline EF MySQL modeli ve in-memory SQLite içerik testleri
gerçek SQL execution testinin yerine geçmez. MySQL 8 icrası da doğrulanmış değildir.

## Çalıştırılan doğrulamalar

project/ çalışma dizininde `pwsh -NoProfile -File tools/test.ps1 -Browser`.
Process-level AllowedHosts/BaseUrl test ayarları kullanıldı, gerçek ayar dosyalarına
yazılmadı. Son build **0 hata / 0 uyarı**.

- .NET: **211 unit + 241 integration = 452 geçti**, 11 DB opt-in atlandı.
- JavaScript: **6 geçti**. Asset parity, 93 kayıt SQL/JSON/manifest parity geçti.
- Scanner self-test **8**, publish-boundary **4**, stil kontrolü geçti.
- Tüm PowerShell araçları parse edildi; yerel Markdown bağlantıları kontrol edildi.
- Dört seed testi gerçek CMS validators/alan uzunlukları/safe HTML/slug kuralları,
  public route çıktısı, 16 alan ve 12 makale detayları, Admin edit ekranları ve
  servis üzerinden bir çalışma alanı güncellemesini doğruladı.
- Browser: son icra aşağıda kaydedilir. İlk tam fixture denemesinde aynı IP'den
  yoğun istekler statik rate limit'e takıldı; görsel inceleme CSS'siz sonucu yakaladı.
  Test, ayrı ziyaretçi IP'leri ve zorunlu CSS/logo/resource kontrolleriyle düzeltildi.
  Üretim rate limit değiştirilmedi; yalnız assertion geçmesi görsel kanıt sayılmadı.

Son tam icra başarılı: **21 browser testi** (6 E2E + 9 public/accessibility + 5 Admin
UX + 1 tam içerik). Yeni test 38 rotayı dört genişlikte kontrol etti: 320, 390, 768,
1440 px; public CSS uygulanması, yerel logo ve diğer kaynakların yüklenmesi zorunlu.
Yatay taşma yok; 16 çalışma alanı ve 12 makale detay rotası 200, 12 SSS görünür,
iletişim bilgisi render edilmiş. Toplam 24 tam içerik screenshot'ı:
`artifacts/test-runs/cms-seed-{home,uzmanlik-alanlari,makaleler,sss,hakkimda,iletisim}-{width}.png`.
Düzeltilmiş desktop ana sayfa ve mobil hakkımda görüntüleri ayrıca görsel incelendi.
Ana sayfanın introduction seed'i mevcut dar sütun için iki okunabilir paragraf yapıldı.
Mevcut tasarım/CSS değiştirilmedi. Bu testler bağımsız tam WCAG sertifikası değildir.

Son TRX: unit `regression_net8.0_20261012011201.trx` / integration
`regression_net8.0_20261012011219.trx` (project/tests/*/TestResults altında).
434 taşıma hedefi mevcut, 0 kayıp; 0 kırık yerel Markdown bağlantısı; PowerShell parse
0 hata; git diff --check temiz. Current secret scan 482 text dosyası/0 bulgu;
private konfigürasyon ve Git geçmişi bu taramanın kapsamı değildir.
HEAD değişmedi, staged dosya yok. Test sunucuları script finally bloğunda kapatıldı.

Test fixture JSON'u sadece **in-memory SQLite**'a yükler; loader diğer provider ve
disk-backed SQLite'ı reddeder. Browser fixture ayrı yeniden başlatılır, loopback
harici istekler engellenir. Gerçek DB credentials/SMTP kullanılmaz.

## Değişen dosyalar ve kalan işler

Seed SQL/JSON/ön izleme + yeni manifest ve offline builder; DraftSeedTests ve yeni
CmsSeedFixture; E2EHost/FixtureGateway; yeni cms-seed browser testi ve test.ps1;
faz format yol düzeltmesi; repo/ADR/CHANGELOG/QA belgeleri. Public Razor/CSS/Admin
uygulama tasarımı değiştirilmedi. Minify dosyaları veya yeni paket eklenmedi.

Gerekli sonraki işler: temiz yetkili test şeması üzerinde gerçek engine ve rollback/
idempotence kanıtı; kurgu profil/iletişim değerlerinin gerçek bilgilerle değiştirilmesi;
16 alan ve tüm yazıların avukat incelemesi; gerçek veri işleme/çerez operasyonuna göre
hukuki metinlerin tamamlanması. Aktif fixture production'a kopyalanmamalıdır.
Önceki launch/backup/SMTP/güvenlik operasyon kapıları bu görevle kapanmaz.

Mevcut ana sayfanın görüşme süreci girişinde sabit “henüz yayımlanmadı” metni vardır;
model/allowlist bu alan için ayrı CMS anahtarı desteklemiyor. Bu statik UI boşluğu
seed ile doldurulamaz. Tasarımı/servisi yeniden yazmama sınırı içinde korundu;
sonraki ayrı görevde CMS alanı tasarlanabilir. Onaylı gerçek portre de sağlanmadı.
