# Admin UX doğrulama raporu

Tarih: 2026-10-11. Kapsam: sahibin altı maddelik yönetim paneli ve iletişim UX isteği.
Onaylı Phase 14 önce **7ccf8c8** ile commit edildi. Bu takip çalışması ayrı değişikliktir;
Phase 15 başlatılmadı. AGENTS.md, skills/admin-panel, frontend, security, accessibility,
BRAND_GUIDELINES ve ADR-017/029/030 esas alındı. Karar: ADR-031.

## Uygulanan değişiklikler

| İstek | Sonuç ve başlıca dosyalar |
|---|---|
| Editör normal alanları engelliyor | `wwwroot/js/admin.js`: seçimi açıkça vererek biçim okuma; altı rich-edit ekranında metin sonrası normal input yazma/Tab regresyonu. Mobilde odak alanı sabit başlığın altında bırakılmaz. |
| Kapsamlı HTML düzenleme | Quill 2.0.3 korunarak düzenlenebilir HTML kaynak penceresi, h4/üstü çizili/alt ve üst simge; Application `Validation/RichText.cs` izin listesi ve sunucu doğrulaması. Uygula ile editör güncellenir, Kaydet ile kalıcılaşır. |
| Silme onayı ve yan yana ikonlar | Native dialog, kayıt adı, öncelikli Vazgeç, onaylı POST; `AdminActionLinkTagHelper.cs`, liste görünümleri, ortak JS/CSS. İkon yanında metin var. JS yokken mevcut onay kutusu korunur. |
| Profesyonel görünüm | Ortak navy/gold sidebar, menü SVG'leri, kartlar, tablo/action stili, mobil boşluklar, bağlama göre yardım ve tutarlı odak görünümü. |
| Yeni kullanıcı rehberi | `GuideController`, `AdminGuideViewModel`, `Views/Guide/Index.cshtml`, `/admin/guide`: sekiz SSR adımı, bölüm bağlantıları ve mevcut içerik göstergeleri; `docs/admin/ADMIN_USER_GUIDE.md` detaylı kılavuz. |
| Özel belgelerin anlaşılması | Documents görünümünde amaç/yükleme/indirme/silme/geri alma açıklaması. Yöneticiye özel PDF deposu olduğu, müvekkil portalı veya public paylaşım olmadığı açıklanır. |
| İletişimde KVKK penceresi | Home/Contact controller, ContactViewModel, Home/Contact görünümü ve site JS/CSS: yalnızca incelenmiş güncel sayfa içeriği, güvenli render; form değerleri/adres korunur, Escape/Close odağı geri verir. |

Mevcut rol, CSRF, brute-force, CSP nonce, dosya güvenlik ve yayın/inceleme kapıları korunur.
Rehber herhangi bir kaydı otomatik yayınlamaz. Gizli belgelerde yeni public yol veya
paylaşım özelliği yoktur. Medya/belge modüllerinde olmayan düzenleme endpoint'i eklenmedi.

## Tekrarlanabilir doğrulama

```powershell
node tools/build-assets.cjs
pwsh -NoProfile -File tools/test.ps1 -Browser -HostedMySql
```

Hosting kontrolü yalnızca kullanıcı tarafından ayrılmış `cevodbtest` bağlantısını
test projesinin user-secrets'inden alır; üretim `cevodb` kullanılmaz. MariaDB 10.3.39
uyumluluk teşhisidir, MySQL 8 doğrulaması değildir. Browser fixture loopback HTTPS,
SQLite ve TEST_ONLY içerikle çalışır; dış ağ istekleri engellenir.

Sonuçlar:

- Release build: 0 hata / 0 uyarı.
- .NET: 203 birim + 210 entegrasyon = **413 geçti**; 10 hosted persistence testi dahil.
- Bir boş şema migration testi opt-in bağlantı verilmediği için atlandı; bu çalışmada
  migration veya şema değişikliği yoktur.
- Analitik JS: **6**; Playwright: **15** (6 mevcut + 4 Phase 14 + 5 admin UX akışı).
- Secret scanner sentetik self-test: **8**; minified asset parity ve format kontrolleri.

Yeni tarayıcı kapsamı: altı içerik ekranında input yazma/Tab/overflow, güvensiz HTML
reddi ve mevcut verinin korunması, güvenli HTML biçimlerinin kaydet/yeniden aç
korunması, edit/delete hizası, vazgeç/CSRF'li onay, rehber ve özel belge açıklaması,
KVKK değer/adres/focus korunması. Açık modal, rehber ve belge sayfasında axe AA
etiket kontrolleri; eski public/editor erişilebilirlik kontrolleri de çalışır.
Başarısız iletişim POST'unda yasal metin/alan değerleri korunur ve mesaj kaydedilmez;
iletişim sayfasındaki çerez bannerı ve footer KVKK bağlantıları da aynı dialog'u açar.

Görsel inceleme çıktıları (git dışında sentetik fixture):

- `artifacts/test-runs/admin-guide-desktop.png`
- `artifacts/test-runs/admin-guide-mobile.png`
- `artifacts/test-runs/phase14-editor-mobile.png`
- `artifacts/test-runs/contact-kvkk-mobile.png`

## Sınırlar ve operasyon notları

- Serbest CSS, tablo, iframe/video ve gömülü resim HTML'si desteklenmez; kapak
  görselleri mevcut medya seçimiyle yönetilir. Tam sınırsız HTML yayınlama hedeflenmez.
- Rehber “İçerik mevcut” sayacı doğruluk, KVKK uygunluğu veya üretime hazır olma
  sertifikası değildir. Görsel adımı isteğe bağlıdır.
- PDF antivirüs/ACL/retention altyapısı bu UX çalışmasında değişmedi. Saklama süresi
  teknik yapılandırmaya bağlıdır; yedekleme yerine geçmez.
- Otomatik axe ve fixture incelemesi tüm WCAG 2.2 AA uygunluğu beyanı değildir;
  gerçek içerik/hosting/yardımcı teknoloji kontrolü yayına alma kapsamındadır.
- Phase 14 Lighthouse ölçümleri tarihsel raporda kalır; bu takip çalışması için yeni
  performans skoru veya saha Core Web Vitals ölçümü iddia edilmez.
- Kullanıcının talimatıyla `appsettings.json` ve mevcut sır bulgusu SH-12-01 değiştirilmedi;
  sırlar rapora, test çıktısına veya commit'e eklenmedi. Üretim öncesi açık kayıt sürer.
- Yeni runtime/NuGet bağımlılığı, migration veya production konfigürasyon değişikliği yoktur.
