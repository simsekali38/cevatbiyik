# Phase 15 — Windows/Plesk üretim yayın rehberi

Bu rehber hazırlık ve operatör uygulama adımlarını ayırır. Gerçek hosting kontrolü,
parola rotasyonu, üretim migration ve restore drill yapılmadan “üretime hazır” onayı
verilmez. Phase 16 denetimi ve Phase 17 yayın kararı ayrıdır. Sunucu yolu, pool kimliği,
Windows/IIS sürümü, backup saklama süresi ve RPO/RTO: **CONTENT_REQUIRED**.

Phase 17 operasyon akışı: [Final launch rehberi](FINAL_LAUNCH_RUNBOOK.md),
[Canlı smoke checklist](LIVE_SMOKE_CHECKLIST.md), [Rollback/izleme](ROLLBACK_MONITORING.md)
ve [Restore kayıt örneği](RESTORE_DRILL_RECORD.md). Bunlar hazırlanmış şablonlardır;
gerçek operasyonun yapıldığı anlamına gelmez.

## 1. Runtime, veritabanı ve yayın kapıları

- .NET 8 Hosting Bundle + ASP.NET Core Module V2, x64 Windows Server/IIS gerekir.
  Sunucuda `dotnet --list-runtimes` ile Microsoft.NETCore.App ve Microsoft.AspNetCore.App
  **8.0.x** varlığı kontrol edilir; yalnızca SDK veya .NET Framework kurulumu yeterli değildir.
- Mevcut paketler .NET/EF Core 8 hedefler. .NET 8 desteği 2026-11-10'da biter.
  Hosting'in .NET 10 planı ve EF/Pomelo uyumlu yükseltme testi olmadan sürüm yükseltilmez.
  Destek dışı runtime ile launch Phase 16 blocker'ıdır (ADR-008).
- Hedef mimari MySQL 8'dir; mevcut bildirilen hosting **MariaDB 10.3.39 / SSL kapalı**dır.
  MySQL 8 ve MariaDB aynı ürün değildir. MariaDB 10.3 bakımı Mayıs 2023'te sona erdi.
  Hosting'den destekli MySQL 8/TLS veya destekli MariaDB/TLS geçiş planı alınmalı;
  MySQL 8 hedefinden sapma ve EOL/TLS riski owner tarafından ayrıca karara bağlanmalıdır.
- `cevodbtest` üzerinde geçiş/rollback testleri üretim onayı yerine geçmez. Production
  `cevodb` için bakım aralığı, geri dönüş yedeği ve incelenmiş migration planı gerekir.

## 2. Tekrarlanabilir yayın paketi

Yerelde önce tam kontrol:

```powershell
pwsh -NoProfile -File tools/test.ps1 -Browser -HostedMySql
pwsh -NoProfile -File tools/publish.ps1 -SkipTests
```

`-SkipTests` yalnızca aynı çalışma ağacı için yukarıdaki kontroller geçtiğinde kullanılır.
Normal `tools/publish.ps1` build/.NET/JS/format kontrollerini kendisi yapar. Paket yeni
`artifacts/releases/<UTC>-<id>` dizinine framework-dependent yayımlanır. Windows x64
Skia native dosyası, out-of-process web.config, 6 MiB IIS limiti ve yasak içerikler
denetlenir; SHA-256 manifesti üretilir. Kirli çalışma ağacı bilgisi manifestte yer alır;
nihai release owner onaylı commit'ten yeniden üretilmelidir.

`appsettings*.json`, gerçek uploads, App_Data, SQL/test/demo veri, özel PDF ve anahtarlar
pakete alınmaz. Release paketi config oluşturmaz, veritabanına bağlanmaz ve deploy etmez.
`wwwroot/uploads/web.config` güvenlik dosyası pakette bulunur. Hosting'e repo kaynakları,
docs/database SQL örnekleri veya test host yüklenmez. Node/Python sunucuda gerekli değildir.

## 3. Sunucuya özel JSON konfigürasyonu

Sahibin Phase 15 kararı: sırlar sunucuda `appsettings.json` / `appsettings.Production.json`
içinde saklanabilir. Bu dosyalar Git dışında ve release paketinin dışında tutulur.
Kaynak makinedeki mevcut içerik değiştirilmeden Git takibinden çıkarılmıştır.
Yeni clone'da geliştirme başlangıcı için temel şablonu proje köküne kopyalayın;
bağlantıyı yerelde user-secrets ile sağlayın. Gerçek değerleri sohbete veya komuta yazmayın.

Üretim publish kökünde (DLL ve web.config ile aynı dizin) **ilk kurulumda**, var olan
konfigürasyonu üzerine yazmadan şu şablonları kopyalayın:

- `docs/deployment/templates/appsettings.template.json` → `appsettings.json`
- `docs/deployment/templates/appsettings.Production.template.json` → `appsettings.Production.json`

JSON içinde `CONTENT_REQUIRED` alanlarını korumalı sunucu editöründe doldurun.
Parola tırnak/backslash içeriyorsa JSON escaping uygulanmalıdır; bağlantı dizesinde
`;` gibi karakterler için MySqlConnectionStringBuilder uyumlu quoting gerekir.
Şablon çalıştırılabilir hazır config değildir; placeholder key/Defender yolu reddedilir.

Yapılandırma sırası: temel JSON → Production JSON → ortam değişkenleri → CLI.
Eski `ConnectionStrings__DefaultConnection`, `Database__*`, `ASPNETCORE_ENVIRONMENT`
değerlerinin JSON'u beklenmedik şekilde ezmediğini Plesk'te kontrol edin.
Production JSON varsayılanları: indexing/contact/analytics/SMTP **kapalı**, detaylı hata
**kapalı**, log seviyesi Warning. ContactNotice ve incelenmiş `/kvkk` hazır olmadan
form açılmaz; analytics ayrıca hukuki onay ve ziyaretçi izni gerektirir.

Sır dosyalarına uygulama kimliği **Read**, deployment/backup operatörüne gereken erişim
verilir. Uygulama bu dosyalara Write/Modify almaz. Public/Anonymous/grup erişimi kaldırılır;
parent ACL inheritance kontrol edilir. Dosyalar `wwwroot` içine koyulmaz. IIS root
request filtering ilgili config adlarını ve App_Data/.git segmentlerini gizler.
Doğrudan `/appsettings.json`, `/appsettings.Production.json`, `/App_Data/...` ve `.git`
istekleri hosting üzerinde **404** olmalıdır; 200 dönüyorsa yayın durdurulur.

## 4. SH-12-01 parola rotasyonu

Git takibinden çıkarma geçmişteki parolayı iptal etmez. Yerel dosya eski parolayı
içerebilir; mevcut Git geçmişi değiştirilmedi. Rotasyon hosting erişimi olan operatörce:

1. Bakım aralığı ve tutarlı backup alın; tüm DB yazarlarını durdurun.
2. Plesk **Databases → ilgili üretim kullanıcı hesabı** ekranında yeni güçlü rastgele
   parola belirleyin. Yalnızca `cevodb` ve gerekli izinleri kullanın; test kullanıcısını
   veya DB'yi production yerine kullanmayın. Aynı hesabı kullanan başka uygulamaları belirleyin.
3. Yeni değeri korumalı Production JSON'a girin. Eski parola hiçbir CLI argümanına,
   terminale, ekran görüntüsüne veya rapora kopyalanmaz.
4. Uygulamayı recycle edin; yeni bağlantıyla smoke kontrolü yapın. Eski credential'ın
   artık çalışmadığını güvenli DBA kontrolüyle doğrulayın; değeri loglamayın.
5. Yerel eski bağlantıyı operator güvenli editörde temizlesin/yenilesin. User-secrets,
   CI, yedekler ve diğer tüketicilerde eski credential envanterini tamamlayın.
6. Tarih/operatör/etkilenen hesap (parola değil), yeni bağlantı ve eski parolanın
   reddedildiği kanıtla SH-12-01 kapatılır. Rotasyon kanıtı yokken kayıt açık kalır.

Geçmiş temizliği için ayrı owner onayı, clone/remote envanteri ve koordineli history
rewrite gerekir. Bu faz hiçbir force-push/history rewrite yapmaz. Sadece geçmiş
temizlemek rotasyon yerine geçmez. `python tools/security_scan.py --history` geçmiş
bulguları nedeniyle 1 dönebilir; bu sonuç “temiz” olarak raporlanmaz.

## 5. MariaDB / bağlantı ve migration

Üretimde `Database:Provider=MariaDb`, `Database:ServerVersion=10.3.39` actual server
doğrulandığında kullanılır. MySQL 8 için `MySql` ve doğrulanmış `8.0.x` değeri seçilir.
Provider offline seçilir; `AutoDetect` ile açılışta üretim keşfi veya otomatik migration
yapılmaz. EF snapshot/migration dosyaları bu fazda değiştirilmez.

Bağlantı şablonu VerifyFull ile fail-closed'dur. SSL kapalı hosting'de bu çalışmaz.
`SslMode=Preferred` TLS garantisi değildir; sunucu SSL sunmuyorsa açık TCP'ye düşer.
Hosting'in DB aynı makinede veya gerçekten izole özel ağda olduğuna ilişkin kanıt ve
owner'ın riske ilişkin kararı olmadan public internette `SslMode=Disabled` kullanmayın.
Doğrulanmış aynı makine bağlantısı için `Server=127.0.0.1;...;SslMode=Disabled;`
yalnızca belgelenmiş istisna olabilir; sunucunun uygulamadan gerçekten erişilebildiği
varsayılmaz. Web HTTPS, DB bağlantısını şifrelemez. TLS/sağlayıcı yükseltmesi istenir.

Diğer şablon ayarları: `PersistSecurityInfo=False`, local infile/public key retrieval
kapalı, süreler sınırlı, pool 20 (hosting kotasına göre operatör düzenler). MySqlConnector
Unicode bağlantıyı kendisi kullanır; legacy `CharSet=utf8mb4` garantisine dayanmayın.
Migration'lardaki charset/collation ve Türkçe veri smoke testi korunur. Runtime DB
kullanıcısı SELECT/INSERT/UPDATE/DELETE; DDL yalnızca bakım operatörü hesabında tutulur.

Araç manifesti EF **8.0.29** ile hizalıdır; restore edin:

```powershell
dotnet tool restore
```

Önce aynı yeni provider/config ile ayrı test DB üzerinde ileri migration ve rollback
drill; ardından mevcut üretim `__EFMigrationsHistory` salt okunur DBA incelemesi.
İdempotent SQL üretimi canlı DB'ye bağlanmadan yapılabilir:

```powershell
dotnet ef migrations script --idempotent --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --configuration Release --output artifacts/migrations/reviewed.sql
```

SQL gerçek MariaDB sürümünde izole DB üzerinde yürütülüp incelenmeden canlıya uygulanmaz.
MySQL/MariaDB DDL implicit commit nedeniyle migration zinciri tümüyle rollback edilebilir
tek transaction sayılmaz. Hata halinde devam etmeyin; history, şema ve aynı batch backup
ile DBA kontrollü kurtarma yapın. `database drop`, `EnsureCreated`, otomatik seed ve dummy
SQL üretimde kullanılmaz.

Kaynak/SDK kurulmayan hosting için aynı onaylı release'ten migration bundle hazırlayın:

```powershell
dotnet ef migrations bundle --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --configuration Release --target-runtime win-x64 --output artifacts/migrations/efbundle.exe
```

Bundle dağıtımı site kökünün dışındaki korumalı bakım klasörüne yapılır. Aynı release'in
runtimeconfig/config provider değerleri ve geçici DDL credential'ı ACL korumalı JSON'da,
bundle çalışma dizininde bulunur. DataProtection:KeysPath gerçek korumalı yola işaret eder.
`ASPNETCORE_ENVIRONMENT=Production` process ortamında ayarlanır; IIS web.config CLI için
okunmaz. `--connection` ile parola vermeyin. Üretim bağlantısının DB adını operatör
değeri loglamadan kontrol eder; **cevodb** olduğundan ve backup alındığından emin olur.

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Production'
.\efbundle.exe
if ($LASTEXITCODE -ne 0) { throw 'Migration failed; keep maintenance enabled.' }
```

Bundle preparation/execution bu rehberde seçenek olarak verilmiştir; çalıştırılıp
doğrulanmış artefaktlar Phase 15 raporunda ayrıca belirtilir. Çalışma bitince geçici DDL
credential/config erişimini kaldırın. Runtime kullanıcıya DDL vermeyin.

## 6. Plesk/IIS kurulumu ve HTTPS

- Plesk domain `cevatbiyik.av.tr`, fiziksel yol publish kökü; www ayrı subdomain/app değil,
  aynı site binding/alternatif host. Sertifika iki hostu kapsar. www→apex ve HTTP→HTTPS
  yönlendirmesini Plesk preferred domain/IIS yönetiminde doğrulayın; URL Rewrite modülü
  varsa kurulu olduğunu kontrol etmeden config eklemeyin. Sertifika yenilemesi izlenir.
- Tek dedicated app pool, **No Managed Code**, Integrated, 32-bit kapalı, worker x64,
  normalde tek worker. Gerçek Plesk process kimliği tespit edilir. ASP.NET Framework
  CLR sürümü .NET 8 runtime seçimi değildir. Hosting Bundle IIS'ten sonra kurulmuş olmalı;
  gerekirse hosting yöneticisi repair/restart yapar.
- Kaynak ve publish web.config `hostingModel=outofprocess`, process `dotnet`, DLL argument,
  Production environment, stdout kapalı ve 6 MiB envelope taşır. ANCM Kestrel'i local
  dinamik portla yönetir; public Kestrel portu açılmaz. Kontrolsüz ASPNETCORE_URLS ayarı eklenmez.
- ForwardedHeaders: yalnızca doğrulanmış immediate proxy, bir hop, X-Forwarded-For/Proto;
  X-Forwarded-Host kullanılmaz. IIS integration ve varsa Plesk proxy topolojisinde IP/HTTPS
  smoke doğrulanır. Güvenmek için bütün ağları/proxyleri açmayın.
- Uygulama 443 redirect ve HTTPS'te 180 günlük HSTS sağlar. includeSubDomains/preload
  kapalı kalır. IIS'in uygulama dışındaki hata yanıtları için DENY/nosniff/referrer/permissions
  başlıkları ayrıca hosting yöneticisi tarafından denetlenir; duplicate CSP eklenmez.
- HTML/private payload compression kapalı; sadece CSS/JS/SVG compression. IIS/Plesk
  dynamic HTML compression ile bu kararın bozulmadığını network smoke ile doğrulayın.
- Config/internal dosyalar erişilemez, directory browse kapalı, App_Data hidden. IIS
  locked section kaynaklı 500.19 hosting yöneticisi çözer; güvenlik filtrelerini kaldırmayın.

## 7. ACL, depolar, anahtarlar ve loglar

Uygulama kimliği: **CONTENT_REQUIRED**. Kimlik doğrulanmadan örnek principal kullanılmaz.
`icacls '<DOĞRULANMIŞ_YOL>'` salt okunur denetim içindir. Plesk dosya izinlerinden:

| Alan | Uygulama kimliğinin ihtiyacı |
|---|---|
| Publish kodu ve server JSON | Read/Execute; config dosyasında Read, Write yok |
| wwwroot/uploads | Modify; yalnızca GUID WebP GET/HEAD public, çalıştırma yok |
| Storage:PrivateRoot / quarantine | Modify; wwwroot dışında, public read/mapping yok |
| DataProtection:KeysPath | Modify; wwwroot dışında, yalnızca app ve yetkili operasyon erişimi |
| stdout tanı logu dizini (geçici) | Yalnızca etkinleştirildiğinde gerekli Modify |
| Defender program dizini | Read/Execute; uygulama değiştiremez |

Everyone/Users/Anonymous yazma izinleri verilmez; private/key/config inherited izinleri
ayrıca gözden geçirilir. Symlink/junction veya public virtual directory oluşturulmaz.
Key store Windows DPAPI **local-machine** ile şifrelenir, sabit app discriminator kullanır;
ACL başka uygulamaları engeller. Recycle sonrası admin/consent/CSRF anahtarlarının
kalıcılığı smoke test edilir. DPAPI key backup farklı makinede kendiliğinden açılamaz;
server migration için rekey ve oturum/consent yenilemesi planlanır.

Özel PDF fail-closed Defender doğrulaması; image native Skia decode/re-encode smoke ve
IIS limit testleri için [Medya operasyon rehberi](PLESK_MEDIA_RUNBOOK.md).
MVC envelope 6 MiB, dosya 5 MiB, upstream limiti en az envelope; aşan istek IIS 404.13/
proxy 413 ile engellenmelidir. HTTP TestServer sonucu gerçek IIS limit kanıtı değildir.

Üretim stdout kapalı tutulur; Plesk IIS/ANCM erişim ve hata kayıtları izlenir. Geçici
stdout tanısında özel dizin, kısa saklama, erişim kısıtı ve tekrar kapatma gerekir.
Exception/EF sensitive-data logging açılmaz. İletişim metni, cookie, parola ve kişisel
veri loglanmaz. Disk kota/doluluk, 5xx, worker crash, sertifika, SMTP queue ve backup
başarısızlık bildirimleri için sorumlu ve sıklık **CONTENT_REQUIRED**.

İlk admin hesabı yoksa migration sonrası aynı publish kökünde, korumalı JSON'a geçici
`AdminBootstrap` bölümü ekleyin (UserName, doğrulanmış Email, güçlü Password).
En az 12 karakter/benzersiz karakter, büyük-küçük harf/rakam/özel karakter politikası
uygulanır; gerçek değerler komut satırına yazılmaz. CLI production ortamını açıkça seçin:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Production'
dotnet .\CevatBiyik.Web.dll --provision-admin
if ($LASTEXITCODE -ne 0) { throw 'Admin provisioning failed; inspect redacted diagnostics.' }
```

Ardından geçici AdminBootstrap bölümünü korumalı editörde kaldırın; admin girişini
HTTPS ile test edin. Bu işlem başlangıçta otomatik çalışmaz; mevcut sıradan hesabı
Admin'e yükseltmez. Ayrıntı: [Admin kurulumu](../security/ADMIN_SETUP.md).

## 8. Yayın, rollback ve birlikte backup/restore

1. Owner onaylı commit + manifest + test raporu alın; fake/test/demo içerikleri üretime taşımayın.
2. Bakım aralığında site `app_offline.htm` ile kapatılır; worker/in-flight upload ve tüm
   diğer DB yazarlarının durduğu doğrulanır. Plesk reset/cleanup seçeneği kullanılmaz.
3. Aynı batch içinde DB, public uploads, tüm private-root/quarantine, DP key ring, ACL,
   korumalı config ve kod release alınır. Secret içeren yedekler şifreli/private tutulur.
4. Onaylı migration bakım kimliğiyle uygulanır. Yeni kod mevcut persistent depolar ve
   JSON üzerine silme/sync yapmadan kurulur. Rollback paketi ayrı korunur.
5. Pool recycle, offline kaldırma, listedeki smoke kontrolleri. Sorunda maintenance
   geri açılır; eski kod ancak şema uyumluysa döndürülür, değilse aynı batch DB+dosya
   restore prosedürü uygulanır. Körlemesine EF Down uygulanmaz.

**Periyodik backup:** Plesk scheduled backup açılır; tüm persistent alanların kapsandığı
ve özel dizin/site dışı anahtarların ayrıca dahil edildiği doğrulanır. Canlı DB backup
ile farklı anda dosya kopyası tutarlı set sayılmaz. Bakım aralığı/snapshot koordinasyonu,
sorumlu, sıklık, şifreli offsite kopya, saklama ve RPO/RTO owner tarafından belirlenir;
uydurulmuş periyot uygulanmaz. Başarısız batch alarm verir ve başarı etiketi almaz.

**Restore drill:** Önce ayrı izole hedefte aynı batch DB+uploads+private+config+ACL
restore; manifest/hash ve migration history doğrulaması; tüm cursor sayfalarında dosya
mutabakatı, PNG/WebP ve yetkili PDF indirme. DPAPI aynı makineyi gerektirir; farklı
makine/rekey oturum yenilemeyi gerektirir. Drill tarihi, sonucu, ölçülen RPO/RTO ve
operatör kaydedilir. Ayrıntılı komutlar ve mutabakat: [Medya rehberi](PLESK_MEDIA_RUNBOOK.md).

## Kaynaklar (2026-10-11 kontrolü)

- [Microsoft IIS hosting](https://learn.microsoft.com/en-gb/aspnet/core/host-and-deploy/iis/?view=aspnetcore-10.0): ANCM/pool ve hosting modeli ilkeleri; uygulama .NET 8 hedefler.
- [Microsoft .NET destek politikası](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).
- [Pomelo explicit server version](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options).
- [MariaDB 10.3.39 destek notu](https://mariadb.com/docs/release-notes/community-server/old-releases/10.3/10.3.39).
- [MySqlConnector TLS seçenekleri](https://mysqlconnector.net/connection-options/).
