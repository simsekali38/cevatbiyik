# Phase 14: tekrar üretilebilir performans ve erişilebilirlik denetimi

## Hazırlık

PowerShell 7, .NET 8 SDK, Node.js ve yerel Microsoft Edge gerekir. Tarayıcı/derleme araçları
yalnızca geliştirme makinesindedir; Plesk üzerinde Node, Lighthouse veya axe kurulmaz.

```powershell
npm ci --ignore-scripts --prefix tests/CevatBiyik.BrowserTests
node tools/build-assets.cjs
pwsh -NoProfile -File tools/test.ps1 -Browser
pwsh -NoProfile -File tools/phase14-audit.ps1 -Stage after
```

`tools/build-assets.cjs --check` üretilmiş `.min.css` / `.min.js` dosyalarının kaynaklarıyla
eşleştiğini doğrular. Kaynak dosyayı değiştirin, tekrar üretin ve kaynak + çıktıyı birlikte commit edin.
CleanCSS 5.3.3 / Terser 5.51.2 sürümleri lock dosyasındadır. Üçüncü taraf Bootstrap/Quill
dosyaları yeniden yazılmaz; editör yalnızca altı CMS düzenleme görünümünde yüklenir.

## Ölçüm

Optimizasyondan önce `-Stage baseline`, sonra `-Stage after` kullanılır. `-NoBuild` yalnızca
güncel Release çıktısı varsa kullanılmalıdır. Betik mevcut 57413 portunu kullanmayı reddeder;
kendi geçici SQLite/HTTPS fixture'ını başlatır, header doğrulaması yapar ve finally içinde kapatır.
Gerçek site URL'si veya veritabanı kabul eden parametre yoktur. TLS istisnası yalnızca yerel
fixture tarayıcı süreci/probu içindir; işletim sistemi sertifika güveni değiştirilmez.

Lighthouse 13.5.0, Edge headless ile `/`, `/makaleler`, `/makaleler/test-browser`, `/iletisim`
için üçer soğuk mobil gezinme ölçer. Varsayılan simülasyon: 412 × 823, 4× CPU, 150 ms RTT,
1638.4 Kbps; üç koşunun medyanı raporlanır. Ölçüm sırasında başka test/derleme çalıştırmayın.
Fixture'da çerez bildirimi görünür, gerçek üçüncü taraf analitik çalıştırılmaz.

Raporlar `artifacts/phase14/{baseline,after}/` altında HTML, tam Lighthouse JSON ve
`summary.json` olarak tutulur (Git dışıdır). `after` komutu otomatik AA ihlali veya
medyan performans <90 / LCP >2500 ms / CLS >0.1 / TBT >200 ms olduğunda başarısız olur.
Bu eşikler yerel fixture regresyon bütçesidir; gerçek kullanıcı INP değeri ölçülmez.

Hızlı klavye/axe doğrulaması:

```powershell
pwsh -NoProfile -File tools/phase14-audit.ps1 -BrowserChecksOnly
```

Tam axe denetimi 19 public/admin rotası, WCAG 2 A/AA, 2.1 AA, 2.2 AA etiketleri ve 390 px
görünümü kapsar. Tarayıcı regresyonu ayrıca 320 px reflow, skip link, görünür odak,
mobil menü, sabit admin başlığı, modal Tab/Shift+Tab/Escape ve forced-colors kontrolü yapar.
`artifacts/test-runs/phase14-a11y-review.json` otomatik karar verilemeyen durumların
hedeflerini ve hesaplanan stil katmanlarını kaydeder; bunlar ihlal veya başarı sayılmaz.

## Önbellek ve Plesk kontrolü

- Sürüm hash'i gerçek dosyayla eşleşen public varlık: `public, max-age=31536000, immutable`.
- Değişmez GUID WebP: aynı politika. Sürümsüz veya sahte `?v=`: bir saat / must-revalidate.
- HTML/admin/hata: `no-store`; tam sayfa ResponseCaching/OutputCache etkin değildir.
- Uygulama yalnızca CSS/JS/SVG için HTTPS Brotli/Gzip sunar. CSRF/session içeren HTML,
  JSON, SMTP/özel belge yanıtları sıkıştırılmaz. Sıkıştırma `Vary: Accept-Encoding` üretir.
- Phase 15'te gerçek IIS/Plesk/proxy'nin Content-Encoding/Vary/cache başlıklarını,
  koşullu 304 yanıtlarını ve HTML'i yeniden sıkıştırmadığını doğrulayın. Upstream dinamik
  HTML sıkıştırmasını kapalı tutun; bu aşamada hosting yapılandırması değiştirilmedi.

## Canlı ortamda devam eden doğrulama

Yerel SQLite fixture MySQL/Plesk yük testi değildir. Gerçek onaylı görseller, yazılar,
çerez durumları ve mobil ağlar ile staging ölçümleri tekrarlanmalıdır. CrUX/Search Console
verisi yeterli trafik oluşunca değerlendirilir. LCP ≤2.5 s, INP ≤200 ms, CLS ≤0.1 hedefleri
gerçek ziyaretlerin 75. yüzdeliğinde değerlendirilir; TBT, INP yerine geçmez.
Tam WCAG beyanından önce gerçek içerikle ekran okuyucu, 200% metin büyütme / 400% zoom,
touch target ve tüm dinamik durumların insan incelemesi gerekir.

Kaynaklar: [Web Vitals](https://web.dev/articles/vitals),
[Playwright accessibility testing](https://playwright.dev/docs/accessibility-testing),
[ASP.NET Core Output Cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-8.0).
