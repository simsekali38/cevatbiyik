# Temsili hukuk kompozisyonu — 2026-10-12

Ana sayfa görsel zenginleştirmesi için built-in imagegen ile üretildi. Kaynak:
`design/generated/representative-legal-still-life.png` (1448×1086). Gerçek kişi,
büro, mahkeme, sertifika veya resmî belge fotoğrafı değildir. İnsan, logo ve
okunabilir yazı içermez. Üretim çıktısı incelendi; mevcut marka SVG'si değiştirilmedi.

Public çıktılar `project/src/CevatBiyik.Web/wwwroot/images/hero/` altında:

| Dosya | Ölçü | Byte |
|---|---|---|
| representative-legal-still-life-480.webp | 480×360 | 20258 |
| representative-legal-still-life-768.webp | 768×576 | 43266 |
| representative-legal-still-life-1200.webp | 1200×900 | 90554 |

Mevcut Pillow ile yalnızca oran koruyan boyutlandırma/WebP dönüşümü yapıldı
(LANCZOS, quality=82, method=6); yeni paket kurulmadı. Orijinal kaynak PNG public
wwwroot dışında kaldı. Ana sayfada görünür temsili/AI açıklaması var. width/height,
responsive srcset/sizes, eager loading ve fetchpriority=high kullanılır.

Home.HeroMediaId üzerinden seçilmiş aktif/güvenli CMS medyası önceliklidir; boş,
silinmiş veya geçersiz seçimde bu yerel kompozisyon gösterilir. Kaynak resim
gerçek fotoğrafmış gibi CMS'ye yüklenmedi ve kayıtlar/yayın durumları değiştirilmedi.

## Üretim promptu

Create one polished photorealistic editorial still-life image for a Turkish lawyer
website hero, landscape 4:3 composition. A refined antique brass justice balance
with two balanced pans on a walnut desk, closed dark navy leather law books with
NO lettering, an open blank notebook and fountain pen. Warm diffuse daylight from
the left, cream and ivory softly blurred background, brass gold highlights, deep
navy shadows. Composition: balance and books on the right two-thirds, left third
quieter softly blurred beige. Premium restrained legal editorial photography,
natural materials, realistic proportions, absolutely no people, portraits, logos,
writing, readable words, court emblems, flags, plaques or real identifiable office.
This is an explicitly representative illustration of legal objects, not a photograph
of an actual lawyer's office. No gavel. Output image only, no UI/text/border.
