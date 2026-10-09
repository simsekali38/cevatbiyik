# Windows/Plesk dosya operasyonları — Faz 07

Bu bir uygulama rehberidir; gerçek Plesk üzerinde çalıştırıldığı anlamına gelmez.
Hosting kimliği, özel kök, yedek konumu, retention onaylayanı ve RPO/RTO: **CONTENT_REQUIRED**.

## Yayına çıkış kapıları

1. Ayrı MySQL 8 test DB'sinde MediaFiles migration + opt-in integration testleri. CEVATBIYIK_TEST_MYSQL yalnızca `cevatbiyik_test_*` veritabanı olabilir. Sır değeri terminal çıktısına/repoya konmaz.
2. Hosting Bundle .NET 8 / ASP.NET Core Module V2 / app pool x64 ve Plesk uyumu. SkiaSharp 3.119.1 native win-x64 libSkiaSharp.dll publish çıktısında bulunmalı; tüm native/runtime dosyaları birlikte yayımlanmalı. Gerçek app pool kimliğiyle PNG→WebP smoke testi. Mevcut .NET 8 destek bitiş riski ADR-008/R-01'de kabul edilmiştir.
3. Root publish web.config `maxAllowedContentLength=6291456`, uploads/web.config yalnızca WebP GET/HEAD allowlist. Plesk'in inherited requestFiltering ayarları ve locked sections kontrol edilir; 500.19 varsa hosting yöneticisi çözer. App_Data hidden segment, directory browsing kapalı, private kök için hiçbir IIS virtual directory/static mapping yok.
4. Gerçek HTTPS endpoint'te 5 MiB üzeri file reddi, 6 MiB üzeri toplam request reddi (IIS 404.13 veya proxy 413), yanlış MIME/magic, EXE/SVG/Office/ZIP reddi. TestServer MVC limit attribute'ları gerçek IIS enforcement kanıtı değildir.
5. Üretim Defender yolu korumalı olmalı; app pool hesabı değiştirememeli. Defender etkin, güncel signature, doğru servis/izinler, staging/private klasörleri scan exclusion dışında olmalı. Application identity ile benign PDF taraması ve kontrollü antivirüs test prosedürü. Defender çalışmıyorsa özelliği bypass etmeyin; upload/download fail-closed kalır. Özel belgeleri bulut tarama servisine göndermeyin.
6. Yetkisiz ve Admin rolü olmayan kullanıcıyla belge download engeli; public /App_Data ve /uploads/documents 404; başarılı Admin indirme attachment/no-store/nosniff; silme sonrası 404; checksum bozulursa 409.

## NTFS ACL

Uygulama kimliğini Plesk panelinden doğrulayın; `IIS AppPool\...` varsayımı yapmayın.
Public uploads için yalnızca gerekli uygulama kimliği Modify, IIS static-read için gerekli kimlik Read; Everyone/Users write kaldırılır.
PrivateRoot ve quarantine için yalnızca uygulama kimliği Modify, yetkili backup operator gerekli erişim ve Administrators/SYSTEM yönetim erişimi.
Anonymous/public gruplar private depoya erişemez. App pool'un site koduna/Defender executable dizinine yazması gerekmez.
ACL inheritance ile parent geniş erişimlerinin private dizine gelmediğini kontrol edin; uygulama için bir deny kuralının gerekli allow'u da engellemediğini test edin.
Junction/symlink/reparse point, virtual directory ve başka web site root'una bağlama kullanmayın.
`icacls 'D:\CONTENT_REQUIRED\private-files'` ile salt okunur inceleyin; gerçek principal doğrulanmadan genel grant komutu çalıştırmayın.
Public ve private depolar aynı volume üzerinde olmalı; karantina move + overwrite engeli ve geri taşıma smoke testi yapılır.

## Birlikte yedek alma

1. Yedek batch kimliği belirleyin; web ve tüm CLI/background/diğer DB yazarlarını durdurun. IIS/ANCM için app_offline.htm ile uygulamayı maintenance'a alın, process kapanmasını/in-flight uploads bitmesini doğrulayın. HTTP dışı yazarlar ayrıca durdurulur.
2. Aynı kesinti aralığında MySQL dump, `wwwroot/uploads` ve **bütün** PrivateRoot (documents, quarantine, varsa pending parçalar) kopyasını aynı batch'e alın. Kod/config sürümü, EF migration history, storage root mapping ve ACL export'u ekleyin. Secret değerlerini genel manifest/loga koymayın.
3. Örnek, gerçek değerler operatörce yerine konur; credential CLI argümanına yazılmaz:

```powershell
mysqldump --login-path=cevat-backup --single-transaction --routines --triggers --no-tablespaces --result-file=D:/Backups/CONTENT_REQUIRED/database.sql cevodb
Copy-Item -LiteralPath 'D:\CONTENT_REQUIRED\wwwroot\uploads' -Destination 'D:\Backups\CONTENT_REQUIRED\uploads' -Recurse
Copy-Item -LiteralPath 'D:\CONTENT_REQUIRED\private-files' -Destination 'D:\Backups\CONTENT_REQUIRED\private-files' -Recurse
```

4. Komut exit code'ları, dosya sayısı/boyut ve SHA-256 manifest doğrulanır. Dump'ın tamamlandığı, bütün dosyaların aynı freeze aralığından geldiği kaydedilir. Yedek diski şifreli ve erişimi kısıtlı olmalı; private belgeler public web dizininde yedeklenmez.
5. Başarılı bütünlük denetiminden sonra maintenance kaldırılır; yeni yükleme/indirme smoke testi yapılır. Başarısız batch başarı etiketi almaz. Snapshot olmadan canlı DB dump + farklı zamanda files kopyalama tutarlı backup sayılmaz.

## Birlikte geri yükleme

1. Önce izole staging restore drill. Mevcut DB/disk durumunun geri dönüş yedeğini alın; tüm yazarlar kapalı tutulur.
2. **Aynı batch** DB dump ve public/private dosyalarını geri yükleyin. Boş/izole restore target kullanın; mevcut prod klasörünü körlemesine üzerine kopyalamayın, eski/yeni dosyaları karıştırmayın. Silme/taşıma hedeflerinin mutlak yolunu operatör ayrıca denetler.
3. Windows mysql client örneği (PowerShell `<` yönlendirmesi kullanılmaz):

```powershell
mysql --login-path=cevat-restore --database=CONTENT_REQUIRED --execute='source D:/Backups/CONTENT_REQUIRED/database.sql'
```

4. StorageRoot mapping, app sürümü/migration history, NTFS ACL, native assets ve Defender ayarları eski batch ile uyumlu olmalı. Public/private mapping kontrollerini yineleyin. Migration gerektiğinde test edilmiş forward adım uygulayın; Down PrivateDocuments metadata'yı siler.
5. Disk SHA-256 manifestini doğrulayın. Admin dosya mutabakatının **tüm cursor sayfalarını** tarayın. Soft-deleted kayıtlar korunmalı; orphan/missing raporu çözümlenir. Eksik dosya için DB kaydını otomatik silmeyin. PDF indirme ayrıca DB SHA-256 ile kontrol edilir.
6. Aktif/silinmiş belge izinleri, görsel decode, attachment, audit, public URL ve kötü MIME smoke testleri. Her şey doğruysa maintenance kaldırın; aksi halde birlikte alınmış geri dönüş batch'ini kullanın.
7. Tarih, batch, operator, sonuç, ölçülen RPO/RTO ve sorunlar restore drill tutanağına yazılır. Gerçek hosting drill tamamlanmadan üretim hazır onayı verilmez.

## Karantinadan kurtarma

Özgün `images/{guid}.webp` veya `documents/{guid}.pdf` anahtarı quarantine altında korunur.
Tüm yazarlar kapalıyken source/destination absolute path, reparse yokluğu ve hedefin boşluğu denetlenir; overwrite olmadan aynı key'e taşıyın.
İlgili DB kaydı aynı backup batch'ten kurtarılmalı veya içerik güvenli upload hattından yeniden alınmalı. DB metadata uydurulmaz.
Yarım dosyayı final WebP/PDF olarak yeniden adlandırmayın. Tekrar tarama ve yetki/bütünlük smoke testinden sonra açın.
Teknik retention kalıcı purge içermez; hukuki/kişisel veri saklama süresi ayrı onay gerektirir.
