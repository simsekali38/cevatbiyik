# Tam içerikli CMS seed — yalnızca izole test

## SQL 1267 düzeltmesi

Metin literal'larında CONVERT(... USING utf8mb4) sonrasına açık
COLLATE utf8mb4_unicode_ci eklendi. Şema adı koruması BINARY DATABASE() ile
byte-exact karşılaştırılır. SET NAMES tek başına CONVERT sonucunun varsayılan
collation'ını belirlemeye yeterli kabul edilmez. Tablo/sütun collation'ı değiştirilmedi.
[MariaDB karşılaştırma önceliği](https://mariadb.com/docs/server/reference/sql-functions/secondary-functions/information-functions/coercibility).

Önceki hata sonrası SQL oturumu açıksa aynı oturumda ROLLBACK çalıştırın; güncel
dosyayı yalnızca belirtilen izole şemada baştan uygulayın. Önceki istemci COMMIT'e
devam ettiyse otomatik temizlik yapmayın; mevcut kayıtların durumunu önce kontrol edin.
Bu düzeltme harici MariaDB üzerinde yeniden icra edilmedi; statik/fixture testleri ayrıdır.

Bu sürüm önceki 47 yayın dışı taslağın yerini alır. **93 kayıt** public görünürlük
filtrelerini test etmek için aktiftir. IsReviewed=1 gerçek avukat inceleme/onayı
anlamına gelmez. Production'a export/import edilmemelidir.

## Dosyalar ve kapsam

[SQL](REVIEW_DRAFT_CONTENT.sql), [JSON](REVIEW_DRAFT_CONTENT.json),
[manifest](REVIEW_DRAFT_MANIFEST.json), [offline üretici](build_review_seed.py).

| Tablo | Kayıt |
| --- | ---: |
| PracticeAreas | 16 |
| ArticleCategories | 4 |
| ArticleTags | 6 |
| Articles | 12 |
| ArticleTagLinks | 21 |
| FaqItems | 12 |
| PageContents | 4 |
| PageSeo | 9 |
| LawyerProfiles | 1 |
| SiteSettings | 8 |

Alanların kapsam/kavram/belge/süreç/dikkat metinleri; makalelerin konu bölümleri,
özet/kategori/etiket/çalışma alanı bağlantıları, SEO/OG ve UTC yayın tarihleri doludur.
İçerik gövdelerinde placeholder bulunmaz. Eğitim/sicil için ayrı sütun yoktur;
profil anlatısı Biography içinde tutulur, migration eklenmez.

## Alanlar ve public slug'lar

| Alan | Slug |
| --- | --- |
| Ceza Hukuku | ceza-hukuku |
| İdare Hukuku | idare-hukuku |
| Aile Hukuku | aile-hukuku |
| İş Hukuku | is-hukuku |
| KVKK Danışmanlığı | kvkk-danismanligi |
| Ticaret Hukuku | ticaret-hukuku |
| Tüketici Hukuku | tuketici-hukuku |
| İcra ve İflas Hukuku | icra-ve-iflas-hukuku |
| Sözleşmeler Hukuku | sozlesmeler-hukuku |
| Miras Hukuku | miras-hukuku |
| Gayrimenkul ve Taşınmaz Hukuku | gayrimenkul-ve-tasinmaz-hukuku |
| Vergi Hukuku | vergi-hukuku |
| Sigorta Hukuku | sigorta-hukuku |
| Fikrî ve Sınaî Mülkiyet Hukuku | fikri-ve-sinai-mulkiyet-hukuku |
| Basın ve Medya Hukuku | basin-ve-medya-hukuku |
| Dava ve Tahkim | dava-ve-tahkim |

Detay rotası `/uzmanlik-alanlari/{slug}`. Mevcut 12'li sayfalama korunur.

## Hedef ve güvenli icra

Hedef EXACTLY **cevodbtest**: owner tarafından onaylanan ayrı test veritabanı.
Her INSERT DATABASE() eşitliğiyle korunur. Başka şemada **0 insert** olur;
bu sessiz no-op'tur, başarılı exit seed uygulanmış demek değildir. Adetleri kontrol edin.
`cevodb` production hedefidir; burada çalıştırmayın. Koruma kaldırılmaz. Mevcut test kayıtları korunur.

1. DBA ayrı test hesabının yalnızca bu şemaya yetkili olduğunu; motor/collation ve
   güncel migration durumunu kontrol eder. Concurrent writer bulunmamalıdır. Önce test DB yedeği alınır.
2. Güncel migration'lar yalnızca bu şemaya uygulanır. Connection özel yerel ayarda
   kalır; SQL, terminal logu veya Git'e yazılmaz. project/ çalışma dizini kullanılır.
3. SQL tek session, ilk hatada DURAN istemciyle çalıştırılır. --force kullanılmaz;
   hata sonrası devam eden GUI modu kapatılır. Hata halinde aynı session ROLLBACK
   veya connection close yapılır; COMMIT'e devam edilmez.
4. İlk çalıştırmada mevcut adetler ile eklenen fixture kayıtları karşılaştırılır. İkinci
   çalıştırmada adetler ve içerikler değişmemeli; migration geçmişi korunmalıdır.
5. Ayrı disposable senaryoda farklı slug ile reserved ID çakışması denenip ilk hata,
   rollback ve başlangıç verisinin korunduğu doğrulanır. Production üzerinde denenmez.
6. Uygulama yalnız test bağlantısıyla açılır. Indexing/SMTP/analytics/Person ve
   LegalService şemaları manifestteki ayarlarla kapalı kalır; canlı domain kullanılmaz.
7. Public içerik ve Admin edit kontrolünden sonra temizleme ayrı DBA yetkisiyle
   yapılır. Bu seed silme veya dış veritabanına otomatik çalıştırma aracı içermez.

ID rezervi 910200001–910200157. FK sırası korunur; FK bypass, DDL, UPDATE/DELETE yok.
Mevcut doğal anahtar sessizce değiştirilmez. Reserved ID başka kayıtla çakışırsa insert
hata verir. FK değerleri doğal anahtar üzerinden mevcut parent ID ile çözülür. Mevcut anahtarlar
ve yayın durumları değiştirilmez; bu nedenle 93 yeni kayıt eklenmesi garanti değildir.
Eski profil/ayar/sayfalar varsa korunur ve yeni fixture metni bunların üzerine yazılmaz. Identity/başvuru/audit/notification/özel belge tablolarına kayıt yok.

Motor dayanağı önceki read-only teşhistir: MariaDB 10.3.39. Bu görev dış DB açmadı.
Bu görev DB açmadı: **gerçek migration/SQL/idempotence/rollback testi yapılmadı**.
Offline EF model/validator ve in-memory SQLite Razor/Admin/browser testleri MariaDB
SQL execution testinin yerine geçmez. MySQL 8 üzerinde icra edildi iddiası yoktur.

## Kurgusal değerler ve canlıdan ayrım

Manifest, profilin eğitim/mesleki anlatısı/vizyon/baro açıklaması, görüşme birimi,
örnek mahalle adresi, çalışma saatleri ve makale tarihlerini listeler. Profilde
uydurma üniversite/yıl/sicil veya gerçek mesleki başarı iddiası verilmez. E-posta
example.invalid, telefon/WhatsApp +90 000 değerleridir: gerçek ulaşım kanalı değildir.
Site adı nötrdür; test kayıtları gerçek Av. Cevat Bıyık bilgisi olarak onaylanmış değildir.

KVKK sayfası genel çerçeve sunar; tam ve hukuken onaylı aydınlatma metni değildir.
Veri sorumlusu/amaç/dayanak/alıcı/saklama/aktarım ve hak başvuru kanalı gerçek uygulamayla
ayrı tamamlanmalıdır. Kullanılmayan analytics/SMTP aktarımı gerçekleşiyormuş gibi
ilan edilmez. Kaynaklar: [KVKK aydınlatma](https://www.kvkk.gov.tr/Icerik/2033/Aydinlatma-Yukumlulugu-),
[Kurul duyurusu](https://www.kvkk.gov.tr/Icerik/6765/AYDINLATMA-YUKUMLULUGUNUN-YERINE-GETIRILMESI-HAKKINDA-KAMUOYU-DUYURUSU),
[resmî mevzuat](https://mevzuat.adalet.gov.tr/). Bu kaynaklar fixture'ı onaylamış değildir.

Canlı içerikler ayrı CMS girişi/owner incelemesiyle hazırlanmalı; tüm kurgusal bilgiler
gerçek bilgilerle değişmelidir. Alanların gerçekten sunulduğu teyit edilmelidir.
Medya/lokasyon/redirect eklenmedi: fiziksel upload, doğrulanmış adres ve gerçek ihtiyaç
bulunmadan sahte kayıt oluşturulmaz. Mevcut temsili hero korunur.

## Yerel tekrar

Kökten `python database/seeds/build_review_seed.py` JSON/SQL/manifesti aynı editoryal
kaynaktan üretir, DB açmaz. project/ içinden `pwsh -NoProfile -File tools/test.ps1 -Browser`
regresyonlar ve ayrı in-memory tam CMS fixture'ının dört genişlik kontrolünü çalıştırır.
