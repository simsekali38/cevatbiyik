# Ana sayfa görsel zenginleştirme — 2026-10-12

Referans: `design/Av. Cevat Bıyık Hukuk Sitesi Mockup.png`. Yalnızca ana sayfanın
görsel dili uyarlanmıştır; referanstaki biyografi, hizmet/deneyim iddiaları, gerçek
kişi görselleri ve örnek iletişim bilgileri uygulamaya taşınmamıştır.

## Uygulama

- Hero: sıcak açık zemin, mevcut lacivert başlık, altın CTA ve temsili terazi/kitap
  kompozisyonu. CMS Home.HeroMediaId önce gelir; güvenli varsayılan görsel korunur.
- Yaklaşım/alan kartları: yerel, dekoratif altın çizgili SVG ikonlar, daha yumuşak
  sınırlar/gölgeler. İkonlar aria-hidden ve focusable=false; logoyu değiştirmez.
- Profil: mevcut temsili ofis WebP'si ve görünür AI açıklaması. Onaylı biyografi
  aynı CMS üzerinden gelir; yayımlanmamış profil için mevcut eksik içerik durumu kalır.
- Makaleler: mevcut aktif CMS kapakları, lazy yükleme; kapaksız durumda dekoratif
  kitap ikonu. Pasif kapakların kalktığı ve taslakların görünmediği doğrulandı.
- Altın çizgiler, beyaz/kırık beyaz/bej geçişler ve iletişim ikonları aynı marka
  tokenlarıyla düzenlendi. Açık zeminde koyu altın ve lacivert odak rengi kullanıldı.

## Dosyalar ve sınırlar

`Views/Home/Index.cshtml`, yeni `_HomeIcon.cshtml`, mevcut site.css ana sayfa
bölümü ve build-assets ile üretilen site.min.css değişti. Üç responsive WebP varlığı
eklendi. HomeHeroTests ve phase14 tarayıcı testinin ana sayfa kontrolleri güncellendi.
Kaynak/üretim promptu: `design/generated/REPRESENTATIVE_LEGAL_STILL_LIFE.md`.
Kaynak PNG design/generated içinde; public'e yalnızca WebP çıktıları kondu.
Built-in imagegen kullanıldı; yeni paket veya harici görsel/font/ikon servisi yok.

Controller, servisler, veritabanı, migration, shared layout, Admin ve iç sayfalar
değişmedi. Canonical/metadata/JSON-LD, CSP, çerez izni, form güvenliği ve yayın
kuralları korundu. Ana sayfaya ikinci iletişim formu veya FAQ veri modeli eklenmedi.
Referanstaki koyu navigasyon ve FAQ/form kompozisyonu bu aşamanın kapsamına alınmadı.

## Görsel doğrulama

Önceki `artifacts/test-runs/home-refined-1440.png` ile yeni
`home-decorated-1440.png` incelendi. Mobil 390 görüntüsü de incelendi; 320/390/768/1440
taşma testleri geçti. Yeni çıktılar eski home-refined görüntülerinin üzerine yazılmaz.
Ekran görüntüleri izole test verileri içerir; üretim içeriği veya yayın onayı değildir.

İlk test lazy profil görselinin yüklenmesini beklemeden kontrol ettiği için başarısız
oldu. Tarayıcı testi görseli ekrana kaydırıp decode tamamlanmasını bekleyecek şekilde
düzeltildi. Lazy yükleme kapatılmadı. İzole oturumda analitik reddedilerek ekran
görüntüleri alındı; uygulamanın çerez mekanizması değiştirilmedi.

Hero WebP: 480/768/1200 px, yaklaşık 20/43/91 kB. width/height ve srcset/sizes
tanımlı; hero eager/high, profil ve makale görselleri lazy. Bu dosya boyutları gerçek
cihaz Core Web Vitals ölçümü veya genel WCAG sertifikası değildir.

## Testler

Release build: 0 hata / 0 uyarı. Genel .NET çalıştırması: 216 unit + 246 integration
geçti, 11 opt-in MySQL/migration testi atlandı. Ardından eklenen CMS kapak regresyonu
dahil HomeHeroTests ayrı çalıştırıldı: 6/6 geçti (genel sayımla örtüşür).
Önceden bilinen yerel JSON AllowedHosts testi genel çalıştırmada hariç tutuldu;
yerel ayar dosyası değiştirilmedi. Bu nedenle tüm filtresiz paketin yeşil olduğu
iddia edilmez. Veritabanına veya gerçek SMTP'ye bağlanılmadı.

JS: 6 geçti. Asset/93 kayıt seed parity, scanner sentetik self-testleri (8), publish
Python testleri (5), IDE0011 format ve git diff whitespace kontrolleri geçti.
Tarayıcı: 21/21 geçti (6 E2E, 9 responsive/erişilebilirlik, 5 admin UX, 1 tam CMS).
Ana sayfa için dört genişlikte Axe AA kontrollerinde ihlal yok; klavye/menü,
yerel varlık yükleme, CMS durumları ve iç sayfa regresyonları da doğrulandı.
Tarayıcı sonuçları: `artifacts/test-runs/home-decoration-browser-final.log`.
Geçici doğrulama betikleri mevcut runner'ı değiştirmeden kaldırılır.

## Sonraki aşama

Onaydan sonra aynı ikon/kart/görsel dili çalışma alanları ve makale iç sayfalarına
taşınabilir. Gerçek portre, büro fotoğrafı ve içerik onayları CMS'de ayrıca sağlanmalı.
Bu çalışma commit/push veya hosting yayını yapmaz.
