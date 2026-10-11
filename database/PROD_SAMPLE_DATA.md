# Tam dolu demo veritabanı — cevodbtest

`PROD_SAMPLE_DATA.sql` artık tüm **25 uygulama/Identity tablosuna 264 ilişkili kayıt** ekler.
26. tablo olan `__EFMigrationsHistory` mevcut migration kayıtlarıyla korunur; buraya sahte migration eklenmez.
Şema, Domain, uygulama davranışı ve production bağlantısı değiştirilmez.

## Tüm tablo kapsamı

| Tablo | Eklenen satır |
| --- | ---: |
| ArticleCategories | 6 |
| ArticleTags | 12 |
| MediaAssets | 8 |
| PracticeAreas | 6 |
| Articles | 24 |
| ArticleTagLinks | 48 |
| FaqItems | 12 |
| LawyerProfiles | 2 |
| PageContents | 4 |
| PageSeo | 12 |
| Locations | 12 |
| LocationPracticeAreas | 28 |
| LocationArticles | 36 |
| ContactMessages | 12 |
| PrivateDocuments | 3 |
| RedirectRules | 5 |
| AuditLogs | 8 |
| SiteSettings | 10 |
| AspNetRoles | 2 |
| AspNetUsers | 2 |
| AspNetRoleClaims | 2 |
| AspNetUserClaims | 4 |
| AspNetUserLogins | 2 |
| AspNetUserRoles | 2 |
| AspNetUserTokens | 2 |

Makale dağılımı: **18 yayında, 3 taslak, 3 ileri tarihli**. Kategoriler, ikişer etiket,
uzmanlık alanları, kapak ve OpenGraph görselleri ilişkilidir. 18 yayındaki makale, 12'li sayfalamayı da doldurur.
FAQ, primary profil, ana sayfa ve üç hukuki sayfa demo görüntüleme için aktif/simüle onaylıdır.
İletişim mesajları okundu/okunmadı, geçmiş bildirim denemesi ve gönderildi durumlarını;
medya, profil, belge ve yönlendirmeler aktif/pasif/soft-delete örneklerini içerir.

Kayseri → Melikgazi, Kocasinan, Talas; Ankara → Çankaya, Yenimahalle;
İstanbul → Şişli → Çağlayan (`Region`); Nevşehir ve Sivas il kökleri.
Gerçek alan `ParentId` kullanılır; ID'ler `LAST_INSERT_ID()` ile elde edilir.
FAQ tablosunda ilişki FK'sı yoktur; ilgili uzmanlık yolları cevaplarda metin olarak bulunur.
İçerik çok paragraflı düz metindir; uygulamanın HTML encoding sözleşmesi korunur.

## Çalıştırma

1. Migration'ları uygulanmış **cevodbtest** bağlantısını açın.
2. `PROD_SAMPLE_DATA.sql` dosyasının tamamını UTF-8 / `DELIMITER` destekli SQL istemcisinde açın.
3. **İlk hatada durma** seçeneğiyle çalıştırın. `--force` kullanmayın; başka transaction içinde çalıştırmayın.
4. Betik geçici `cevodbtest.seed_prod_sample_v2` yordamını kaldırır ve tablo başına satır sayısı raporu gösterir.
5. Medya/belge ekranlarının fiziksel dosyaları için aşağıdaki eşlik eden dosyaları test sitesine yerleştirin.

Tüm tablo/yordam adları `cevodbtest` ile nitelenir. `CREATE ROUTINE`, `ALTER ROUTINE`, `EXECUTE`,
`SELECT`, `INSERT` izinleri gerekir. 25 gerekli tablonun InnoDB olduğu ve sonunda hiçbirinin boş olmadığı kontrol edilir.
INSERT dışında mevcut satırları değiştiren bir veri işlemi yoktur. Veri eklemeleri transaction içindedir;
hata işleyicisi tüm eklemeleri geri alır, FK kontrollerini açar ve eski oturum SQL mode değerini geri yükler.
Yordam oluşturma/silme DDL'si transaction dışında kalır. Identity ID değişkenleri açıkça
`utf8mb4_unicode_ci` tanımlıdır; hostingin database varsayılan kolasyonundan etkilenmez.

Tekrar çalıştırma, mevcut gerçek sayfa/profil/ayar/rol anahtarı veya slug çakışması hata verir;
**hiçbir mevcut kayıt ezilmez**. Önceki v1 setindeki `ornek-` slug'lar v2 `demo-` slug'larıyla çakışmaz,
ancak önceden oluşturulmuş `primary`, `Admin` veya gerçek sayfa/site ayarı anahtarları çakışabilir.
Mevcut kayıtları silerek bunu aşmaya çalışmayın; ayrı test DB yedeği ve sahiplik incelemesi yapın.
Bu set mevcut verilerin yerine geçmez; boş/migrate edilmiş test DB'ye eklemek için uygundur.

İstemci hatada durduğu için yardımcı kalırsa, bu dosyanın yordamı olduğunu doğruladıktan sonra:

```sql
DROP PROCEDURE `cevodbtest`.`seed_prod_sample_v2`;
```

FK kontrollerini yeniden açmak önceki satırları yeniden doğrulamaz; bu nedenle yeni referansların
tamamı commit öncesinde açıkça kontrol edilir.
[MySQL FK belgesi](https://dev.mysql.com/doc/refman/8.0/en/create-table-foreign-keys.html),
[MariaDB hata işleyicisi](https://mariadb.com/docs/server/reference/sql-statements/programmatic-compound-statements/declare-handler).

## Görseller ve özel belgeler

`prod-sample-assets.zip` içinde 8 gerçek 1200×630 WebP ve 3 okunabilir, aktif içerik içermeyen PDF vardır.
SQL boyut/hash değerleri bu dosyaların gerçek byte değerleridir. SQL fiziksel dosya yazamaz.
Yalnızca **cevodbtest kullanan test uygulamasında** ZIP içeriğini şu şekilde yerleştirin:

| ZIP içeriği | Test uygulamasında hedef |
| --- | --- |
| `images/*.webp` | `wwwroot/uploads/images/` |
| `documents/*.pdf` | `App_Data/private-files/documents/` |
| `manifest.json` | Test operatörünün inceleme dosyası; wwwroot'a gerekmez |

`Storage:PrivateRoot` farklıysa PDF'ler bu kökün `documents/` altına gider; **wwwroot içine konmaz**.
Dosya adlarını değiştirmeyin; SQL StoragePath ve SHA256 bunlarla eşleşir. Soft-delete dosyaları
retention/restore testi için fiziksel olarak korunur. İndirme yine Admin yetkisi ve mevcut antivirüs denetimi ister;
bu fixture antivirüs taramasının yerine geçmez. Uygulamanın malware politikası gevşetilmez.

## Demo güvenliği ve gerçek bilgiler

- Fake veri kullanımı sahibin açık isteğidir. DEMO metinleri ve `example.invalid` adresleri gerçek kişi/kurum yerine geçmez.
- Onay/yayın bayrakları sadece dolu test ekranı simülasyonudur; gerçek hukuki/editoryal onay değildir.
- Avukatın eğitim, deneyim, sicil, baro veya iletişim bilgileri uydurulmaz; gerçek baro bilgisi `CONTENT_REQUIRED` kalır.
  Kurgusal adres açıkça DEMO olarak yazılır; telefon/WhatsApp alanları aranabilir gerçek numara içermez.
- Locals aktif/simüle onaylı olup detay görüntülenebilir; **FactsVerified=0** ve özgün yerel bilgi eksikliği
  nedeniyle indexable değildir. Ulusal konumlar mevcut Kayseri-only admin doğrulamasını değiştirmez.
- Yargıtay/Danıştay konu notları görev/yetki, kurum adresi veya hizmet kapsamı iddiası değildir.
- Test uygulamasında `Site:AllowIndexing=false`, doğrulanmış işletme şeması kapalı ve SMTP/analitik pasif tutulmalıdır.
  SQL bunların uygulama yapılandırmasını değiştirmez.
- İki Identity kullanıcı **IsActive=0 ve 9999 yılına kadar kilitli**; gerçek parola SQL'de yoktur.
  PasswordHash geçerli Identity V3 biçiminde, açıklanmayan rastgele paroladan üretilmiştir.
  Demo dış login/token kayıtları gerçek OAuth sağlayıcısı veya geçerli auth token değildir.
  Gerçek Admin login için mevcut güvenli provision sürecini kullanın; demo kayıtlarını açmayın.
- Tüm ContactMessages için **NotificationPending=0**; fake SMTP gönderimi otomatik tetiklenmez.

## Sahiplik ve test

`TestData.ProdSample.v2.Manifest` tekrar çalıştırma kilidi; `TestData.ProdSample.v2.Ids` yeni ID'lerin JSON kaydıdır.
Composite-key ilişkiler bu sahip olunan kullanıcı/makale ID'leri üzerinden izlenir; kendi sahiplik ayarı sabit anahtarıyla tanınır.
Önceki `--demo cleanup` bu bağımsız SQL setini temizlemez. Temizlik ayrı sahiplik/FK incelemesi gerektirir;
toplu tablo veya slug-prefix silmesi kullanılmaz. Migration geçmişi hiçbir zaman temizlenmez.

```powershell
pwsh -NoProfile -File project/tools/Test-ProdSampleData.ps1
```

2026-10-11 canlı MariaDB 10.3.39 doğrulaması:

- 26 tablo envanteri: 25 veri tablosunun tümü 264 eklemeyle dolduruldu; EF geçmişi korundu.
- Kayıt sayıları, FKs, Çağlayan→Şişli hiyerarşisi, makale durumları, kilitli kullanıcılar,
  kapalı mail kuyruğu ve sahiplik manifesti geçti.
- Geç hata ve mükerrer anahtar senaryoları tüm eklemeleri rollback etti; FK/SQL mode geri yüklendi.
- Önce/sonra tüm tablo satır sayıları eşit; test kalıcı demo veri bırakmadı. Auto-increment sayaçları ilerleyebilir.
- 11 dosyanın boyut/SHA256 değerleri doğru; 3 PDF uygulamanın PdfPolicy kontrolünden geçti;
  8 WebP Pillow ile decode edildi; yönetilen dosya anahtarı formatları doğrulandı.
- SQL dosyası sahibi çalıştırdığında COMMIT eder; test aracı yalnızca doğrulama kopyasını ROLLBACK yapar.
- Gerçek MySQL 8 üzerinde çalışma henüz doğrulanmadı. SQL/dosya fixture değişikliği için application test suite yeniden çalıştırılmadı.

Sayım manifesti: `PROD_SAMPLE_DATA_COUNTS.json`. Production ayarları, kaynak kartvizit fotoğrafları ve sırlar değiştirilmedi.
