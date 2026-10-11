# Phase 17 — Canlıya alma operatör rehberi

Durum: **Belge/şablon hazır; gerçek yayın yapılmadı. NO-GO korunuyor.**

Owner scope update (ADR-033): private PDF module is disabled. Features:PrivateDocuments
stays false/absent; PDF/Defender-specific gates are N-A. Image/Skia, existing private
data backup, and all other release gates remain applicable. Do not enable PDF to run smoke.
Phase 16 commit'i `581a8c0` kapanış onayıdır; açık production kapılarının kapandığı
anlamına gelmez. Bu rehberdeki adımlar yalnızca yetkili hosting/DB operatörünce,
kanıt ve owner yayın kararıyla uygulanır. Bilinmeyenler **CONTENT_REQUIRED**.

Teknik temel: [Üretim rehberi](PRODUCTION_RUNBOOK.md), [Plesk kontrol listesi](PLESK_CHECKLIST.md),
[Hazırlık raporu](../qa/PRODUCTION_READINESS_REPORT.md). .NET 8 / tek MVC uygulaması /
MySQL 8 hedefi / Windows-Plesk / yerel filesystem ve ADR-032 korunur.

## 1. Yayın dosyası ve durdurma kapıları

Her operasyon için ayrı, erişimi kısıtlı bir kayıt oluşturun. Bu boş şablonda gerçek
başarı işaretlemeyin; doldurulmuş kayıt ve redakte kanıtlar özel operasyon deposunda
tutulur. JSON, DB dump, cookie, PII ve anahtarlar Git'e veya paylaşılan rapora girmez.

| Alan | Değer |
|---|---|
| Yayın/incident kimliği, UTC tarih | CONTENT_REQUIRED |
| Owner yayın onayı / operatör / DBA / yedek sorumlu | CONTENT_REQUIRED |
| Onaylanan commit / release manifest hash / önceki release | CONTENT_REQUIRED |
| Site fiziksel kökü / pool kimliği / runtime-ANCM | CONTENT_REQUIRED |
| DB ürün/sürümü / hedef DB adı / TLS kanıtı (credential yok) | CONTENT_REQUIRED |
| Bakım başlangıcı / hedef RPO-RTO / rollback karar zamanı | CONTENT_REQUIRED |
| Backup batch / izole restore kaydı / smoke kaydı | CONTENT_REQUIRED |
| Sonuç: NO-GO / GO / rollback / owner imzası | CONTENT_REQUIRED |

Devam etmeden önce:

- [ ] SH-12-01: yeni bağlantı başarılı, eski credential reddediliyor; operatör/tarih kanıtı var.
- [ ] PR-16-01/02: destekli DB ve MySQL8 hedef kararı, gerçek provider/sürüm, TLS/topoloji çözüldü.
      Destek dışı MariaDB10.3 veya public ağda SSL kapalı bağlantıyla kendiliğinden GO verilmez.
- [ ] Runtime destekli ve patch durumu teyitli; ADR-008 yükseltme takvimi kaydedildi.
- [ ] Plesk/ACL/native/Defender ve aynı batch restore provası başarılı.
- [ ] Hukuki/gerçek içerik onayı var; demo/seed/fake/CONTENT_REQUIRED public içerik yok.
- [ ] İzleme sorumluları, rollback paketi ve bakım aralığı hazır; owner GO kararı kaydedildi.

Herhangi biri eksikse production migration/dağıtım aşamasına geçmeyin. Varsayılan
indexing/contact/SMTP/analytics kapalı kalır. Form/analitik kullanılmayacaksa owner
kapsam kararını kaydedin; kapalı özelliği başarılı aktif entegrasyon olarak işaretlemeyin.

## 2. Release hazırlığı — geliştirici

Onaylanan temiz commit'te, bağlantı secret'larını çıktılamadan:

```powershell
git status --short
dotnet tool restore
pwsh -NoProfile -File tools/test.ps1 -Browser -HostedMySql
pwsh -NoProfile -File tools/publish.ps1 -SkipTests
```

HostedMySql sadece yetkili geçici MariaDB testidir; MySQL8 onayı yerine geçmez.
`-SkipTests` aynı ağacın testleri geçtiyse kullanılır. Nihai manifestte commit
doğru ve dirtyWorkingTree=false olmalı. Script/bundle da bu commit'ten, doğrulanan
dialect ile üretilir; [migration hazırlığı](PRODUCTION_RUNBOOK.md#5-mariadb--bağlantı-ve-migration)
adımlarını izleyin. İzole DB'de prova edilmemiş migration canlıya uygulanmaz.
Şema değişikliği yoksa inceleme sonucunu kaydedin; gereksiz migration üretmeyin.

SHA-256 manifestini paketle birlikte koruyun. Transferden sonra dosya dosya boyut
ve hash'leri karşılaştırın; eksik/fazla/değişmiş release dosyası varsa durun.
Manifestin kendi hash'ini ayrı güvenilir kayıtla eşleştirin. SHA-256 eşleşmesi
paketin kaynağını tek başına doğrulamaz; owner-approved commit/transfer kaydı da gerekir.
Paket server JSON, kullanıcı dosyaları, SQL/dummy seed veya test host içermez.

## 3. Bakım ve SH-12-01 rotasyonu — operatör/DBA

1. Mevcut release/config/ACL envanterini alın; son tutarlı yedeğin geri yüklenebilir
   olduğunu kanıtlayın. Bakım duyurusunu owner onayıyla yapın.
2. Doğrulanmış site kökünde `app_offline.htm` ile bakımı açın; worker, background
   SMTP, in-flight upload ve diğer tüm DB yazarlarının durduğunu doğrulayın.
3. Plesk korumalı DB kullanıcı ekranında üretim hesabının parolasını döndürün.
   [SH-12-01 sırası](PRODUCTION_RUNBOOK.md#4-sh-12-01-parola-rotasyonu) ve tüketici
   envanterini izleyin. Credential'ı CLI, log, ekran görüntüsü veya bu kayda yazmayın.
4. Yeni credential'ı private editörde runtime Production JSON'a girin; ayrı DDL
   hesabını yalnızca bakım config'inde kullanın. DBA güvenli bağlantı kontrolünde
   yeni bağlantının çalıştığını ve eski parolanın reddini doğrulasın. Credential
   açığa çıkmış eski backup config'ini rollback sırasında geri etkinleştirmeyin.
5. Tarih/hesap etiketi/operatör/iki kontrolün redakte sonucu ile SH-12-01 kaydını
   kapatın. Git history temizliği ayrı karardır; rotasyonun yerine geçmez.
6. Rotasyondan sonra aynı batch DB+uploads+private/quarantine+config+keys+ACL+eski
   release yedeğini alın. Batch hash'leri ve timestamp'i özel kayda girin. Backup
   sırasında tüm yazarlar durur; secret'lar encrypted/private backup'ta kalır.

## 4. Production config, migration ve kod kurulumu

1. Fiziksel yolları ve ACL'leri doğrulayın. Server-owned JSON'u ilk kurulumda safe
   şablondan oluşturun; mevcut dosyayı yayın paketiyle ezmeyin. Config'te app Read,
   operatör yönetim; uploads/private/key ayrı izinler. Proxy trust-all yok.
2. Production, AllowedHosts, Site:BaseUrl, gerçek DB provider/sürüm ve TLS değerini
   inceleyin. Ortam değişkenleri JSON'u ezebilir; değerleri loglamadan çakışmayı
   denetleyin. Private/key yolları wwwroot dışında, DPAPI/rekey planı doğrulanmış olmalı.
3. Indexing/contact/SMTP/analytics başlangıçta kapalı. SMTP açıldığında startup
   options validation uygulanır; sadece 587 authenticated STARTTLS desteklenir.
4. DBA production `__EFMigrationsHistory` ve şemayı incelesin. Review edilen SQL
   **veya** bundle yolunu seçin, iki yöntemi gelişigüzel birlikte uygulamayın.
   SQL private DBA aracı üzerinden, bundle protected site dışı bakım dizininde
   çalışır. DB adı `cevodb` doğrulanır; cevodbtest veya restore hedefiyle karıştırılmaz.
5. Bundle çalışma dizinindeki protected JSON geçici DDL hesabını ve gerçek
   provider'ı taşır. Çalıştırılan artifact hash'i aynı izole provadakiyle eşleşir:

```powershell
# Yalnızca yukarıdaki kapılar tamamlanmış, yetkili bakım dizininde.
$env:ASPNETCORE_ENVIRONMENT = 'Production'
.\efbundle.exe
if ($LASTEXITCODE -ne 0) { throw 'Migration failed; keep maintenance enabled.' }
```

6. DDL implicit commit olabilir. Hata varsa devam etmeyin/retry-loop yapmayın;
   history/şema durumunu DBA inceler, [kurtarma planı](ROLLBACK_MONITORING.md) uygulanır.
   `database drop`, otomatik startup migration, `EnsureCreated` ve seed yasaktır.
7. History ve şema son migration ile eşleştiğinde geçici DDL config/yetkisini kaldırın.
   App runtime hesabı minimum DML kalır; CLI'de `--connection` kullanmayın.
8. Bakım sürerken onaylı release'i kurun; sadece kod dosyalarını değiştirin. Eski
   koddan kalmış stale DLL/asset'leri envantere göre yönetin, kök dizini topluca
   temizlemeyin. JSON, uploads, App_Data/private/key ve bakım dosyasını koruyun.
9. Gerekirse ilk admin'i [bootstrap prosedürü](PRODUCTION_RUNBOOK.md#7-acl-depolar-anahtarlar-ve-loglar)
   ile oluşturun; geçici Bootstrap sırlarını kaldırın. Mevcut admin'e otomatik rol verilmez.
10. Config ve artifact denetimini bitirin; operatör bakım dosyasını kontrollü kaldırır.
    Pool recycle/ilk HTTP isteğiyle startup gözlenir. Dış smoke başlayınca gerçek
    trafik yazmaya başlayabilir; bu anı kayıt altına alın. Hata durumunda tekrar bakım açın.

## 5. Kontrollü açılış ve teslim

[Canlı smoke checklist](LIVE_SMOKE_CHECKLIST.md) sonuçlarını saat/operatör/kanıt ile
kaydedin. Sır URL'lerinin engeli ve temel auth/DB/public smoke başarısızsa yayın
başarılı sayılmaz. Onaylı gerçek içerik ve hukuk incelemesiyle contact/SMTP'yi
etkinleştirin, recycle edin; form kaydı ile mail teslimini ayrı doğrulayın.
Önceden pending kayıtlar varsa SMTP açılınca işlenebilir; restore sonrası olası
tekrar gönderim DBA/operator tarafından önce değerlendirilir.

Analytics isteğe bağlıdır; hukuk onayı ve ziyaretçi consent olmadan çalışmaz.
GTM/GA4 seçimi ve nonce/CSP kontrolünden sonra etkinleştirin. Indexing en son,
canonical/robots/sitemap/noindex incelemesi başarılı ve owner içerik onayı varsa
açılır. Değişikliklerden sonra pool recycle ve ilgili smoke tekrarı gerekir.

Operatör sonucun GO/rollback olduğunu kaydeder; owner teslim onayını verir.
[İzleme tablosundaki](ROLLBACK_MONITORING.md) görevleri devredin. Gerçek restore,
smoke ve GO kaydı olmadan Phase 17 canlı yayın tamamlandı diye kapatılmaz.
