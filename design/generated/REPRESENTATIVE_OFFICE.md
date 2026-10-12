# Temsili ofis hero — 2026-10-12

Güncelleme: ana sayfanın yeni varsayılan hero görseli
REPRESENTATIVE_LEGAL_STILL_LIFE.md içinde belgelenmiştir. Bu mevcut ofis görseli
ana sayfa profil bölümünde temsili açıklamasıyla yeniden kullanılır. Aşağıdaki
üretim ve eski hero kullanım notları tarihsel kayıt olarak korunur.

Built-in imagegen kullanıldı. Kaynak representative-law-office.png: 1122×1402.
Gerçek kişi/ofis fotoğrafı değil; kurgusal hukuk ortamı, insan/kimlik/sertifika/logo yok.
SVG marka varlığı değiştirilmedi. Public varsayılan görselde temsili/AI açıklaması görünür.

Web çıktıları project/src/CevatBiyik.Web/wwwroot/images/hero/ altında:
- representative-law-office-480.webp: 480×600, 38580 byte
- representative-law-office-768.webp: 768×960, 77610 byte
- representative-law-office-1122.webp: 1122×1402, 131980 byte

Kaynak PNG yalnızca design/generated arşivindedir; production'a kopyalanmaz.
WebP: Pillow LANCZOS boyutlandırma, quality=82, method=6; yeni paket kurulmadı.
Responsive srcset/sizes, width/height, eager/high priority ve object-fit cover kullanılır.

## Üretim promptu

Photorealistic editorial architectural photograph for a Turkish lawyer website hero.
Fictional representative modern legal office, not an actual lawyer or firm. Portrait
composition about 4:5, at least 1200px long edge. Cream walls, warm natural wood,
navy chair/cabinetry, restrained brushed brass details, organized law books with
blank spines, blank closed folder. Natural window daylight and realistic textures.
Main desk/room details centered for right-column object-fit cover. Colors harmonize
with #18264B #101B36 #D6B36A #FAF8F3. No people/faces/logos/certificates/seals/readable
text/personal data/screens/identity papers. No scales, gavels, exaggerated gold or
stock clichés. One polished high resolution image. Built-in imagegen, opaque background.

## Mevcut CMS üzerinden değiştirme

1. Medya panelinden onaylı görseli yükleyin; doğru alt metin girin.
2. Site Ayarları → yeni ayar → Ana sayfa hero görseli (medya ID).
3. Medya panelindeki pozitif ID numarasını değer olarak girin ve Sitede kullan'ı açın.
4. Ana sayfada gerçek doğru görseli doğrulayın. URL/file path girilmez.
5. Ayarı kapatmak ya da seçili medyayı silmek temsili varsayılana döndürür.

Mevcut IMediaService/public setting üzerinden reuse; yeni şema/migration/seed medya
kaydı yok. Sadece güvenli /uploads/images/{32hex}.webp, aktif kayıt ve pozitif ölçüler
kabul edilir. Caption sadece üretilen varsayılanda gösterilir. Özel belgeler kapalı kalır.
