# ENVIRONMENTS.md — Ortamlar ve Yapılandırma

> Faz 01 — Foundation (2026-10-09)
> Bu doküman ortam ayarlarının **nerede** tutulduğunu anlatır. Gerçek sırlar (bağlantı dizeleri, şifreler, SMTP bilgileri) **hiçbir zaman** repoya yazılmaz.

## 1. Ortamlar

| Ortam | `ASPNETCORE_ENVIRONMENT` | Nerede çalışır | Not |
|---|---|---|---|
| Development | `Development` | Yerel makine (`https://localhost:7080`, `http://localhost:5080`) | Detaylı hata sayfası açık, `launchSettings.json` ile başlar |
| Production | `Production` | Windows + Plesk | Detaylı hata kapalı, HSTS ve exception handler (`/hata`) açık |

Staging ortamı şimdilik yoktur. Gerekirse Faz 13'te ayrı karar alınır.

## 2. Yapılandırma kaynakları (öncelik sırası: sondaki kazanır)

1. `appsettings.json` — tüm ortamlar için güvenli varsayılanlar
2. `appsettings.{Environment}.json` — ortama özel, **sır içermeyen** ayarlar
3. User Secrets — yalnızca Development, repo dışında (`%APPDATA%\Microsoft\UserSecrets\`)
4. Ortam değişkenleri — Production'da sırlar için (Plesk üzerinden)

## 3. Mevcut ayarlar

| Anahtar | Dosya | Açıklama |
|---|---|---|
| `Site:Name` | `appsettings.json` | Site adı |
| `Site:BaseUrl` | `appsettings.json` / `.Development.json` | Canonical taban URL. Production: `https://cevatbiyik.av.tr` (ADR-009). `www` yönlendirmesi `OPEN_QUESTIONS.md` Q16 |
| `ConnectionStrings:DefaultConnection` | user-secrets / Plesk | Repoda boş bırakılır; değer asla commit edilmez |
| `AllowedHosts` | `appsettings.json` | Şimdilik `*`. Domain kesinleşince Faz 10/13'te canonical host'lara daraltılacak |
| `DetailedErrors` | `appsettings.Production.json` | `false` |
| `Logging:*` | Tüm dosyalar | Production'da `Warning` |

## 4. Sırlar (Faz 02'den itibaren)

Bağlantı dizesi anahtarı Faz 02'de netleşecektir; önerilen: `ConnectionStrings:DefaultConnection`.

**Development** (User Secrets, `CevatBiyik.Web` içinde `UserSecretsId` tanımlıdır):

```powershell
cd src\CevatBiyik.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<bağlantı dizesi>"
dotnet user-secrets list
```

**Production** (Plesk ortam değişkeni; iç içe anahtarlar `__` ile yazılır):

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
- İçerik repoya girmez; yalnızca `.gitkeep` takip edilir.
- Production'da uygulama havuzu kimliğinin bu klasöre **yazma** izni olmalı, başka yerlere olmamalı (Faz 05 ve 13).
- `wwwroot` altındaki dosyalar herkese açık servis edilir; gizli belgeler burada tutulmamalıdır (Faz 05'te karar verilecek).

## 7. Doğrulama komutları (yerel)

```powershell
dotnet restore
dotnet build
dotnet test
```

Faz 01 doğrulama (2026-10-09): `dotnet build CevatBiyik.sln -c Release` 0 hata; `dotnet test` 11 unit + 3 integration test başarılı.
