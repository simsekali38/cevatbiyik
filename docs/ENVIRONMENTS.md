# ENVIRONMENTS.md — Ortamlar ve Yapılandırma

## Yerel JSON yapılandırması (ADR-034, 2026-10-11)

Yerel Web uygulaması artık user-secrets yüklemez. `https` çalıştırma profili hem
`ASPNETCORE_ENVIRONMENT` hem `DOTNET_ENVIRONMENT` için `Development` kullanır.
Önce `appsettings.json`, ardından `appsettings.Development.json` yüklenir; ortam
JSON'u temel dosyadaki aynı anahtarları ezer. Ortam değişkenleri ve CLI seçenekleri
JSON'dan daha önceliklidir; eski `ConnectionStrings__DefaultConnection` gibi ortam
anahtarları varsa yerel JSON değerini ezebilir.

Gerçek `appsettings.Development.json` da Git/publish dışındadır. Yeni clone'da
`docs/deployment/templates/appsettings.Development.template.json` dosyasını
`src/CevatBiyik.Web/appsettings.Development.json` olarak kopyalayıp
`ConnectionStrings:DefaultConnection` alanına yalnızca geliştirme/test bağlantısını yazın.
Temel `appsettings.json` dosyasını temel şablondan oluşturun. Mevcut yerel bağlantı,
owner isteğiyle temel JSON'dan Development JSON'una değer gösterilmeden aktarılmıştır.
Eski user-secrets dosyaları silinmez; Web artık onları otomatik okumaz.
Integration testlerinin ayrı opt-in test secret kaynağı bu karardan etkilenmez.

Yerelde başlatma:

```powershell
dotnet run --project src/CevatBiyik.Web --launch-profile https
```

Adres: `https://localhost:7080`. İzinli hostlar localhost/loopback ile sınırlıdır;
Development'da `Site:AllowIndexing=false`. Hosting JSON dosyaları değişmemiştir.

## Phase 15 güncel karar (ADR-032)

Üretim sırları owner talimatıyla sunucuda ACL korumalı `appsettings.json` /
`appsettings.Production.json` içinde tutulur. Bu iki üretim Web dosyası Git ve publish
paketi dışındadır; yerel mevcut içerik değiştirilmeden takibi kaldırılmıştır.
Sır içermeyen şablonlar `docs/deployment/templates/` altında tutulur.
Yeni clone için temel şablonu `src/CevatBiyik.Web/appsettings.json` olarak kopyalayın;
Development bağlantısını Git dışında tutulan `appsettings.Development.json` ile verin. Production JSON sadece sunucuda
oluşturulur. Ayrıntı: [Üretim rehberi](deployment/PRODUCTION_RUNBOOK.md).

## Phase 04 public UI configuration

`Site:AllowIndexing` defaults false. `Site:BaseUrl` supplies the trusted HTTPS canonical origin.
Production contact POST is disabled until `Site:ContactFormEnabled=true` and a reviewed non-placeholder `Site:ContactNotice` is supplied.
Both values belong in deployment configuration; the flag is confirmation of editorial review, not an automated legal compliance check.
Development/test forms remain available for verification. Real contact/profile details still require approved content.
Bootstrap is served locally; no CDN settings are required. Configure trusted proxies/HTTPS for actual Plesk/IIS topology before launch.

> Faz 01 — Foundation (2026-10-09)
> Bu doküman ortam ayarlarının **nerede** tutulduğunu anlatır. Gerçek sırlar (bağlantı dizeleri, şifreler, SMTP bilgileri) **hiçbir zaman** repoya yazılmaz.

## 1. Ortamlar

| Ortam | `ASPNETCORE_ENVIRONMENT` | Nerede çalışır | Not |
|---|---|---|---|
| Development | `Development` | Yerel makine (`https://localhost:7080`, `http://localhost:5080`) | Detaylı hata sayfası açık, `launchSettings.json` ile başlar |
| Production | `Production` | Windows + Plesk | Detaylı hata kapalı, HSTS ve exception handler (`/hata`) açık |

Staging ortamı şimdilik yoktur. Gerekirse Faz 15'te ayrı karar alınır.

## 2. Yapılandırma kaynakları (öncelik sırası: sondaki kazanır)

1. `appsettings.json` — tüm ortamlar için güvenli varsayılanlar
2. `appsettings.{Environment}.json` — ortama özel ayarlar; Production gerçek dosyası ACL korumalı, Git dışında
3. Ortam değişkenleri — Plesk/CLI environment; JSON'u ezebilir, eski değerler denetlenir
4. CLI seçenekleri — açıkça verilirse önceki kaynakları ezer; komut satırında sır kullanmayın

## 3. Mevcut ayarlar

| Anahtar | Dosya | Açıklama |
|---|---|---|
| `Site:Name` | `appsettings.json` | Site adı |
| `Site:BaseUrl` | `appsettings.json` / `.Development.json` | Canonical taban URL. Production: `https://cevatbiyik.av.tr` (ADR-009). `www` yönlendirmesi `OPEN_QUESTIONS.md` Q16 |
| `ConnectionStrings:DefaultConnection` | Yerel Development JSON / sunucu Production JSON | Gerçek dosyalar Git/publish dışında; değer commit edilmez |
| `AllowedHosts` | temel config şablonu | `cevatbiyik.av.tr;www.cevatbiyik.av.tr`; hosting binding/redirect ayrıca doğrulanır |
| `DetailedErrors` | `appsettings.Production.json` | `false` |
| `Logging:*` | Tüm dosyalar | Production'da `Warning` |

## 4. Sırlar (Faz 02'den itibaren)

Bağlantı dizesi anahtarı Faz 02'de netleşecektir; önerilen: `ConnectionStrings:DefaultConnection`.

**Development**: bağlantıyı Git dışında tutulan `appsettings.Development.json` dosyasına yazın.
Şablon `docs/deployment/templates/appsettings.Development.template.json` altındadır.
Web'de `UserSecretsId` artık tanımlı değildir. Gerçek bağlantı değerini terminale yazdırmayın.

**Production**: birincil kaynak korumalı sunucu JSON'dur. Alternatif ortam değişkeni
anahtarı (JSON'u ezer; gerçek değeri kaynak koduna veya terminale koymayın):

```
ConnectionStrings__DefaultConnection=<bağlantı dizesi>
```

Kurallar:
- Sırlar sohbete, issue'ya, commit mesajına veya log'a yazılmaz.
- `.gitignore` şunları dışarıda tutar: `secrets.json`, `appsettings.*.local.json`, `appsettings.Secrets.json`, `.env*`, `*.pfx`, `*.pem`, `*.key`.
- Bir sır yanlışlıkla commit'lenirse şifresi hemen değiştirilir; geçmişten silmek tek başına yeterli sayılmaz.

## 5. Veritabanı stratejisi (Faz 02 öncesi açık konu)

Yerel MySQL kurulu değil; hosting'deki uzak MySQL kullanılacak. Dikkat edilecekler:

- Migration'lar veritabanını değiştirir. Geliştirme sırasında **canlı veritabanına** doğrudan migration uygulanmamalı; mümkünse ayrı bir geliştirme/test veritabanı açılmalı (`OPEN_QUESTIONS.md` #26).
- Hosting sağlayıcı dışarıdan (uzak) MySQL bağlantısına izin vermeyebilir; Plesk'te "Remote MySQL access" ayarı ve IP izni teyit edilmeli (#18).
- Uzak bağlantıda TLS kullanılabilirliği teyit edilmeli.
- Veritabanı kullanıcısı en az yetkiyle tanımlanmalı. Migration için gereken DDL yetkisi ile uygulamanın çalışma zamanı yetkisi ideal olarak ayrılmalı.

## 6. Dosya yüklemeleri

- Yol: `src/CevatBiyik.Web/wwwroot/uploads/` (ADR-004).
- İçerik repoya girmez; `.gitkeep` ve güvenlik için `uploads/web.config` takip edilir.
- Production'da uygulama havuzu kimliğinin bu klasöre **yazma** izni olmalı, başka yerlere olmamalı (Faz 07 ve 13).
- Faz 06 yalnızca doğrulanıp yeniden kodlanan public WebP görsellerini kabul eder; gizli belgeler burada tutulmaz.
- Plesk ACL/native bağımlılık/upload konfigürasyonu: `cms/CMS_SETUP.md`.
- `Site:LegalServiceSchemaEnabled` varsayılan false; doğrulanmış görünür iletişim bilgileri sonrası açılır.

## 7. Doğrulama komutları (yerel)

```powershell
dotnet restore
dotnet build
dotnet test
```

Faz 01 doğrulama (2026-10-09): `dotnet build CevatBiyik.sln -c Release` 0 hata; `dotnet test` 11 unit + 3 integration test başarılı.

## Phase 05 administrator provisioning

AdminBootstrap:UserName/Email/Password are temporary keys in ignored Development JSON (Development) or protected server configuration (Production), per ADR-034/032. They are read only by the explicit --provision-admin CLI. Remove bootstrap values afterward. Authentication cookies always require HTTPS, including Development. Database rollout, bootstrap commands and Data Protection/Plesk gates are in security/ADMIN_SETUP.md.

## Phase 11 integration switches

Absent Smtp:Enabled and Analytics:Enabled are false. SMTP is startup-bound mandatory STARTTLS/587;
credentials belong in ignored local/server JSON (or explicit environment overrides) and never SiteSettings. Analytics selects one public GTM/GA4 ID,
reviewed config/legal-page gate, versioned protected consent and per-request CSP. Persist private Data Protection keys.
Complete key/command templates and AlwaysRunning/IP/container/legal checks: contact/CONTACT_ANALYTICS_SETUP.md.
No agent changes to user-owned appsettings.json or activation of production external services.
