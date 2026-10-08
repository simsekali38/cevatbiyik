# PROJECT_AUDIT.md — Av. Cevat Bıyık Web Sitesi

> **Faz:** 00 — Discovery
> **Tarih:** 2026-10-09
> **Durum:** TASLAK (kullanıcı onayı bekliyor)
> **Kapsam kuralı:** Bu fazda üretim davranışına dokunulmaz, kod yazılmaz.

---

## 1. Yönetici Özeti

- Bu bir **greenfield** projedir. Çalışma dizini (`D:\Projects\cevat-biyik-av-tr\`) incelendi: içinde yalnızca MCP bağlantı testi için oluşturulmuş `test.txt` var. `.sln`, `.csproj`, `.git`, `web.config` veya başka kaynak dosya yok.
- Elde yalnızca **blueprint paketi** vardır: 60 dosya (ana dokümanlar, 16 faz dosyası, 14 skill, `docs/` altı şablonlar, `project-manifest.json`).
- Teknoloji kararı: **ASP.NET Core MVC + Razor + Bootstrap, .NET 8 LTS, MySQL 8, Windows + Plesk, tek domain.**
- **En kritik bulgu:** .NET 8 desteği **10 Kasım 2026** tarihinde sona eriyor (bugünden yaklaşık 1 ay sonra). Kullanıcı .NET 8 kararını bu uyarıdan sonra tekrar teyit etti; risk **kabul edilmiş risk** olarak kaydedildi ve bir yükseltme planı zorunlu kılındı. Bkz. R-01.
- Plesk'in gerçekte hangi .NET sürümünü desteklediği **doğrulanmadı**. Bu doğrulama Faz 01'e geçmeden yapılmalıdır.

---

## 2. İncelenen Girdiler

| Girdi | Durum |
|---|---|
| Çalışma dizini `D:\Projects\cevat-biyik-av-tr\` | Erişim doğrulandı (Filesystem MCP, tek izinli dizin). İçerik: yalnızca `test.txt` |
| Kod deposu (repo / `.git`) | **Yok** — greenfield |
| Solution / projeler | **Yok** |
| Deployment dosyaları (`web.config`, publish profile) | **Yok** |
| Plesk panel erişimi / ortam dokümanı | **Yok** (henüz verilmedi) |
| MySQL sunucu bilgisi | **Yok** |
| Blueprint (`AGENTS.md`, `MASTER_PROMPT.md`, `ARCHITECTURE.md`, `DECISIONS.md`, `DEPLOYMENT.md`, `OPEN_QUESTIONS.md`, `PHASE_ORDER.md`, faz 00/01, `skills/architecture`, `skills/devops`, `docs/deployment/PLESK_CHECKLIST.md`, `docs/content/CONTENT_INTAKE.md`) | Okundu |
| Blueprint'in kalanı (faz 02-15, diğer 12 skill, `API.md`, `DATABASE.md`, `SECURITY.md`, `SEO_STRATEGY.md`, `TESTING.md`, `docs/*`) | Dosya listesi çıkarıldı; ilgili fazda okunacak |

### Doğrulanamayanlar

Aşağıdakiler **çalıştırılıp doğrulanmadı**:

- Filesystem MCP yalnızca dosya okuma/yazma sağlıyor; komut çalıştıramıyor. Bu nedenle `dotnet --version`, `dotnet build`, `dotnet test` ve MySQL bağlantısı denenemedi. Yerel makinede kurulu .NET SDK ve MySQL sürümleri bilinmiyor.
- Plesk paneline erişim yok → .NET Hosting Bundle / ASP.NET Core Module durumu bilinmiyor.
- `avcevatbiyik.com` için DNS/WHOIS/sahiplik kontrolü yapılmadı.

Faz 01'den itibaren build/test çıktıları **kendi makinende** çalıştırılıp bana yapıştırılmalıdır; çalıştırılmamış hiçbir testi "geçti" diye raporlamayacağım.

---

## 3. Karar Özeti

| Konu | Karar | Kaynak |
|---|---|---|
| Framework | **.NET 8 LTS** (blueprint'in .NET 10 tercihi geçersiz kılındı; iki kez teyit edildi) | Kullanıcı kararı, 2026-10-09 |
| Domain | **avcevatbiyik.com** (geçici/varsayılan; kesin değil) | Kullanıcı kararı, 2026-10-09 |
| Çalışma dizini | `D:\Projects\cevat-biyik-av-tr\` | Kullanıcı, 2026-10-09 |
| Mimari | Modüler monolit, ASP.NET Core MVC + Razor | Blueprint |
| Katmanlar | `Web → Application → Domain`; `Infrastructure` Application soyutlamalarını uygular | Blueprint |
| Veritabanı | MySQL 8.x + EF Core | ADR-003 (Accepted) |
| Dosya depolama | Hosting filesystem, `wwwroot/uploads` | ADR-004 (Accepted) |
| API | İlk sürümde yok; gerekirse `/api/v1/` | ADR-005 (Accepted) |
| UI | Bootstrap | ADR-006 (Accepted) |
| Cloudflare | Bağımlılık yok | ADR-007 (Accepted) |
| MVC/Razor, tek domain | Blueprint'te hâlâ **Proposed** | ADR-001, ADR-002 |
| Kimlik doğrulama | ASP.NET Core Identity, `/admin` (Areas/Admin) | Blueprint |

---

## 4. Blueprint Tutarlılık Bulguları

### F-01 — Tek parça `.md` ile zip arasındaki çelişki
`claude_free_ai_blueprint.md` genel bir şablondur; REST/GraphQL, JWT, CORS ve "Node.js / .NET / Next.js" seçeneklerinden söz eder. Zip'teki manifest ve `AGENTS.md` ise API'siz, tek domain, server-rendered Razor mimarisini zorunlu kılar. **Çözüm:** Zip esas alınır. Faz 10 (Security Hardening) içindeki "CORS" maddesi tek domain nedeniyle büyük ölçüde geçerli değildir.

### F-02 — .NET 10 referansları güncellenmeli
.NET 8 kararı verildiği için şu dosyalar güncellenmelidir (Faz 01'de uygulanacak, bu fazda sadece öneri):
- `project-manifest.json` → `"framework": ".NET 8 LTS"`
- `README.md`, `MASTER_PROMPT.md`, `DEPLOYMENT.md` → ".NET 10 tercih / .NET 8 fallback" ifadeleri
- `DECISIONS.md` → yeni **ADR-008: Target Framework = .NET 8 LTS** (gerekçe: hosting uyumluluğu ve kullanıcı kararı; risk: 2026-11-10 destek sonu; yükseltme planı)

### F-03 — ADR statüleri
ADR-001 ve ADR-002 hâlâ "Proposed". Onaylanıp "Accepted" yapılması önerilir.

### F-04 — Faz 00 görev listesi ile kullanım şekli
Faz 00 "repository'yi incele" der; repo yoktur. Bu nedenle audit, başlangıç durumu (baseline) belgesi olarak yazıldı.

### F-05 — `CHANGELOG.md` boş
İlk kayıt Faz 00 çıktısı olarak eklenmelidir (aşağıda önerildi).

---

## 5. Riskler

| ID | Risk | Etki | Olasılık | Önlem |
|---|---|---|---|---|
| **R-01** | **.NET 8 desteği 10 Kasım 2026'da bitiyor.** Microsoft bu tarihten sonra güvenlik düzeltmesi ve teknik destek vermeyecek (uygulamalar çalışmaya devam eder). Site iletişim formu ve admin paneli içerdiğinden yamasız kalması kabul edilebilir bir durum değildir. **Durum: kullanıcı tarafından kabul edildi (.NET 8 ile devam).** | Yüksek | Kesin (tarih belli) | (1) `TargetFramework` tek yerden (`Directory.Build.props`) yönetilsin. (2) ADR-008'e **.NET 10 yükseltme planı** yazılsın; yükseltme, Faz 13 (Plesk Deployment) öncesinde veya en geç launch sonrası ilk bakım döneminde yapılsın. (3) Plesk'te .NET 10 Hosting Bundle kurulabilirliği hosting sağlayıcıya şimdiden sorulsun. (4) NuGet paketleri .NET 8 ve .NET 10 ile uyumlu seçilsin. |
| R-02 | Plesk/Windows hosting .NET 8 (veya 10) runtime'ını / Hosting Bundle'ı desteklemiyor olabilir | Yüksek | Orta | Faz 00'da hosting sağlayıcıdan yazılı teyit; gerekirse plan değişikliği |
| R-03 | EF Core MySQL sağlayıcısı: seçilen sağlayıcının hedef .NET sürümüyle uyumu doğrulanmalı | Orta | Düşük | Faz 02'den önce sağlayıcı seçimi + sürüm uyumu ADR'ye yazılsın |
| R-04 | Gerçek içerik (adres, baro, biyografi, uzmanlık alanları) yok | Orta | Kesin | `CONTENT_REQUIRED` kuralı; `docs/content/CONTENT_INTAKE.md` doldurulsun |
| R-05 | Domain sahipliği/yönlendirmesi doğrulanmadı | Orta | Bilinmiyor | Domain kayıt sahibi ve DNS erişimi teyit edilsin |
| R-06 | SMTP, yedekleme, depolama limiti ve yükleme limiti bilinmiyor | Orta | Bilinmiyor | `OPEN_QUESTIONS.md` madde 13-15 |
| R-07 | Hukuki reklam/etik uygunluk ve KVKK metinleri | Yüksek | Kesin | Yayın öncesi **insan (avukat) incelemesi** zorunlu; metinler uydurulmayacak |
| R-08 | Hosting filesystem'de yükleme dosyaları: yazma izni ve yedek eksikliği | Orta | Orta | ADR-004 önlemleri; Faz 05 ve 13 |
| R-09 | Yerel geliştirme ortamı (SDK, MySQL, Git) henüz doğrulanmadı; Faz 01 build/test çıktıları buna bağlı | Orta | Bilinmiyor | Madde 20: SDK sürümü, MySQL 8, Git kurulumu teyit edilsin |

---

## 6. Önerilen Başlangıç Mimarisi (Baseline)

```text
CevatBiyik.sln
├── src
│   ├── CevatBiyik.Web             # MVC, Razor, Areas/Admin, middleware, wwwroot
│   ├── CevatBiyik.Application     # Use case, DTO, validation, servis soyutlamaları
│   ├── CevatBiyik.Domain          # Saf domain modeli
│   └── CevatBiyik.Infrastructure  # EF Core + MySQL, Identity persistence, dosya, e-posta
├── tests
│   ├── CevatBiyik.UnitTests
│   └── CevatBiyik.IntegrationTests
├── docs
├── skills
└── phases
```

Bu faz yalnızca yapıyı **doğrular**; projeler Faz 01'de oluşturulur.

---

## 7. Plesk / .NET Doğrulama Planı

Hosting sağlayıcıya (veya Plesk panelinden kendin) şunlar sorulmalı:

1. Sunucu Windows Server mı, sürümü ne?
2. **ASP.NET Core Hosting Bundle** hangi .NET sürümleri için kurulu? (8 / 10)
3. .NET 10 Hosting Bundle kurulabilir mi, takvim var mı?
4. MySQL 8.x mevcut mu, uzaktan/yerel bağlantı, kullanıcı yetkileri?
5. Uygulama havuzu kimliği için `wwwroot/uploads` yazma izni verilebilir mi?
6. Plesk yedekleme (veritabanı + dosya) ve geri yükleme imkânı?
7. Giden SMTP (port/limit) serbest mi?
8. Disk kotası ve maksimum yükleme boyutu?
9. Let's Encrypt / SSL ve www → canonical yönlendirme ayarı?

---

## 8. Faz 00 Çıkış Kriterleri

- [x] Çalışma dizini incelendi (greenfield doğrulandı)
- [x] Blueprint incelendi ve çelişkiler listelendi
- [x] Teknoloji kararları kaydedildi (.NET 8, domain)
- [x] Riskler çıkarıldı; R-01 kabul edilmiş risk olarak işaretlendi
- [x] `OPEN_QUESTIONS.md` güncellendi
- [ ] Hosting/Plesk .NET desteği **doğrulandı** (bekliyor)
- [ ] Yerel geliştirme ortamı (SDK, MySQL, Git) **doğrulandı** (bekliyor)
- [ ] Kullanıcı Faz 00 çıktısını onayladı

## 9. Önerilen `CHANGELOG.md` Kaydı

```markdown
## [Unreleased]
### Phase 00 — Discovery (2026-10-09)
- Added PROJECT_AUDIT.md and updated OPEN_QUESTIONS.md.
- Decision: target framework .NET 8 LTS (user-confirmed; see R-01 re: end of support 2026-11-10; upgrade plan to be recorded in ADR-008).
- Decision: working domain avcevatbiyik.com (unconfirmed).
```

## 10. Bir Sonraki Adım (Faz 01 öncesi)

1. Hosting sağlayıcıdan bölüm 7'deki yanıtlar (özellikle .NET 8 / .NET 10 Hosting Bundle durumu).
2. Yerel ortam teyidi: `dotnet --list-sdks`, MySQL 8, `git --version` çıktıları.
3. Git deposu kararı (yerel `git init` + uzak depo).
4. Domain sahipliği teyidi.
