# Faz 07 — Medya ve özel dosyalar

Kod .NET 8 / MySQL 8 / Windows Plesk / tek MVC uygulaması / Razor sınırlarını korur.
Faz 06 görselleri ve ADR-019 değiştirilmez; PrivateDocument ayrı bir varlıktır.

## Modüler dosyalar

| Katman | Dosyalar | Sorumluluk |
|---|---|---|
| Domain | Entities/PrivateDocument.cs | Başlık, rastgele dosya anahtarı, SHA-256, boyut, ortak soft-delete alanları |
| Application | Interfaces/IFileLibrary.cs | Depo, antivirüs, katalog, mutabakat ve belge servis portları |
| Application | Services/DocumentService.cs, FileMaintenance.cs | Yükleme, silme, doğrulanmış indirme/geri alma, bounded mutabakat |
| Infrastructure | Media/FileDocumentStorage.cs, PdfPolicy.cs, DefenderScanner.cs | Özel PDF deposu, içerik kapısı, zorunlu Defender taraması |
| Infrastructure | Media/SafeFilePaths.cs, ManagedFiles.cs | Dosya adı, junction/symlink denetimi, disk taraması, karantina |
| Infrastructure | Persistence/FileCatalog.cs, Configurations/PrivateDocumentConfiguration.cs | Silinmiş kayıtları da içeren bounded EF sorguları |
| Web | Areas/Admin/Controllers/DocumentsController.cs, FilesController.cs | Rol/CSRF, transactional audit, attachment indirme, Razor paneller |

## Kabul edilen dosyalar ve sınırlar

- Kamuya açık görseller: yalnızca JPEG/PNG/WebP; bildirilen MIME, uzantı ve Skia codec biçimi eşleşir. 5 MiB, 8000 px/eksen, 16 MP giriş; 1600 px çıktı; tek kare; iki eşzamanlı decode. Metadata/orijinal bytes saklanmadan WebP yeniden üretilir. Çıktı da 5 MiB sınırındadır.
- Özel belgeler: yalnızca `.pdf` / `application/pdf`, başta `%PDF-`, sonda `%%EOF`, katalog/sayfa işaretleri ve geçerli konumda klasik xref. 5 MiB. Boş/uyumsuz/executable dosyalar reddedilir. Office, ZIP, SVG, HTML kabul edilmez.
- PDF kapısı konservatiftir: JavaScript, açılış eylemleri, uzak bağlantılar, launch, ek dosya, multimedya, formlar, encryption ve object streams reddedilir. PDF name `#xx` kaçışları çözülür. Bazı geçerli modern PDF'ler de reddedilir; klasik xref içeren düz PDF olarak yeniden dışa aktarılmalıdır.
- Bu kapı tam PDF parser veya CDR değildir. Antivirüsün bilinmeyen zararlıları kesin olarak bulacağı iddia edilmez. Her yükleme ve indirme taranır; tarayıcı yoksa/hatalıysa/zaman aşımındaysa işlem reddedilir. Hosting taraması doğrulanmadan özel belge özelliği üretime açılmaz.
- Dosya adından yalnızca doğrulama için uzantı alınır. Kontrol karakterleri, ADS (`:`), son nokta/boşluk ve aşırı uzun adlar reddedilir. İstemci adı disk yolu veya indirme adı olmaz; path bileşenleri atılır. Sunucu GUID anahtarı üretir.
- MVC upload request 6 MiB, tek file 5 MiB; diğer Admin POST sınırı 256 KiB. IIS `web.config` toplam limiti 6 MiB. Gerçek IIS filtreleme TestServer ile kanıtlanamaz.

## Özel depo ve erişim

Varsayılan: `ContentRoot/App_Data/private-files/documents/{guid}.pdf`; wwwroot dışındadır.
`Storage:PrivateRoot` ile hosting klasörü değişebilir; wwwroot altındaki kökler ve reparse point/junction/symlink reddedilir.
İstemci anahtarı güvenilir yol kabul edilmez. Her okuma/silme/taşıma üretilmiş key biçimini doğrular.
Deponun NTFS ACL'leri güven sınırıdır: uygulama kimliği ve yetkili işletim operatörü dışında yazan olmamalıdır.
Yerel saldırganın aynı kimlikle dosya değiştirmesine karşı mutlak TOCTOU garantisi yoktur.

`/admin/documents`, `/admin/files`, `/admin/documents/download/{id}` Admin rolü/politikası gerektirir.
POST işlemleri CSRF ve açık onay içerir. Belge download aktif/silinmemiş kayıt gerektirir, SHA-256 ve antivirüs tekrar doğrulanır.
Yanıt `application/pdf`, `attachment; filename=document-{id}.pdf`, `nosniff`, `no-store, private`, CSP sandbox içerir.
Eksik/bozuk dosya 409, silinmiş/olmayan kayıt 404. Başlık ve fiziksel yol indirme adına alınmaz.
Audit yalnızca actor/action/time tutar; belge içeriği/başlık/istemci adı saklanmaz.

## Retention, recovery ve mutabakat

- Belge ve görsel silme DB soft-delete işlemidir; dosya korunur. Silinmiş belgeler indirilemez.
- ADR-019 gereği soft-deleted kamu görsellerinin eski public URL'leri erişilebilir kalabilir. Gizli dosya için görsel deposu kullanılmaz.
- Belgeler silinenler panelinden, görseller mutabakat panelinden geri alınır. Belge bütünlüğü ve taraması; görsel boyutu/codec/decode doğrulanmadan kayıt aktive edilmez.
- Mutabakat 100 DB kaydı ve 100 disk dosyası/batch işler; devam cursor'ları bağımsız ilerler. Path eşleşmesi tek bounded IN sorgusudur; silinmiş kayıtlar da hesaba katılır.
- Yönetilen final dosyalar ve crash sonrası `.tmp` / `.pending` parçaları taranır. Rastgele/izinsiz isimler otomatik taşınmaz: operatör ayrıca ACL ve klasör içeriğini denetler.
- Eksik veya bozuk görsel / eksik belge kayıtları raporlanır; kayıtlar otomatik silinmez.
- Sahipsiz dosya için POST sırasında DB bağlantısı ve en az 48 saat yaşı tekrar kontrol edilir. Yeni dosyalar işlemde olabileceği için taşınmaz.
- Karantina `PrivateRoot/quarantine/images|documents` altındadır; özgün anahtar korunur, üstüne yazılmaz. Aynı disk hacmi önerilir. Kalıcı purge yoktur; dosyalar inceleme ve kurtarma için tutulur.
- Audit **OrphanQuarantineRequested** bir girişim kaydıdır. DB ve filesystem atomik değildir; intent audit başarısızsa taşıma yapılmaz, audit sonrası taşıma başarısız olursa intent kaydı kalır. Crash sonrası tekrar tarayın.
- İşletim operatörü karantinadan özgün dizine, aynı anahtarla, overwrite olmadan geri taşır; sonra DB yedeğini/ilişkiyi kontrol edip tarar. `.pending`/`.tmp` dosya doğrudan yayımlanmaz; güvenli yükleme hattından yeniden geçirilir.
- Hukuki belge saklama süresi ve silme onayı: **CONTENT_REQUIRED**. Teknik saklama bir hukuki retention beyanı değildir; otomatik zamanlı kalıcı silme uygulanmaz.

DB mutation/audit tek transaction'dadır. Upload sonrası DB/audit hatası fiziksel dosyayı telafi temizliğiyle kaldırır.
Süreç çökmesi telafiyi engelleyebilir; age-grace ve mutabakat bu nedenle gereklidir.

## Kurulum ve migration

Yerel Development ortamında gerçek hosting değerlerini kendi makinenizde girin; sırları repoya yazmayın.

```powershell
dotnet user-secrets set 'Storage:DefenderExecutable' 'C:\ProgramData\Microsoft\Windows Defender\Platform\CONTENT_REQUIRED\MpCmdRun.exe' --project src/CevatBiyik.Web
dotnet user-secrets set 'Storage:PrivateRoot' 'D:\CONTENT_REQUIRED\private-files' --project src/CevatBiyik.Web
dotnet build -c Release
dotnet test -c Release --no-build
.\artifacts\ef8\dotnet-ef.exe migrations script CmsFeatures MediaFiles --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --configuration Release --output artifacts/MediaFiles.sql
```

`MediaFiles` migration mevcut; yeniden migrations add çalıştırmayın. Yalnızca PrivateDocuments tablosu ve unique StoragePath index eklenir; seed/destructive Up işlemi yoktur. SQL: docs/database/MEDIA_FILES.sql. MySQL DDL implicit commit yapabilir; script transaction tam geri alma garantisi değildir. Down tabloyu siler ve belge metadata kaybına yol açar; üretimde geri yükleme planını kullanın.

Dedicated `cevatbiyik_test_*` MySQL 8 doğrulaması, yedek ve SQL incelemesinden sonra hedef yerel DB'ye uygulanacak komut:

```powershell
.\artifacts\ef8\dotnet-ef.exe database update MediaFiles --project src/CevatBiyik.Infrastructure --startup-project src/CevatBiyik.Web --configuration Release
```

Bu görevde canlı/yerel cevodb update çalıştırılmadı. EF CLI 8 kullanılır; root manifestin farklı major sürümü bu iş için kullanılmaz.

Defender çağrısı shell kullanmaz; ArgumentList, tek eşzamanlı tarama ve 60 saniye timeout vardır. `-Scan -ScanType 3 -File ... -DisableRemediation`; yalnızca çıkış 0 kabul edilir. Output private path içerebileceği için uygulama loguna yazılmaz.
Executable yolu güvenilir, yönetici tarafından korunan Microsoft Defender kurulumuna ait olmalı; uygulama hesabı executable dizinine yazamamalıdır.
Kaynak: [Microsoft Defender komut satırı ve custom scan](https://learn.microsoft.com/en-us/defender-endpoint/command-line-arguments-microsoft-defender-antivirus).

Üretim doğrulaması, ACL ve eşzamanlı backup/restore: [PLESK_MEDIA_RUNBOOK.md](../deployment/PLESK_MEDIA_RUNBOOK.md).
