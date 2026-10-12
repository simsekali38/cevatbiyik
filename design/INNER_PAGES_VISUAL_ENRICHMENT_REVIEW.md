# Public iç sayfa görsel zenginleştirme — 2026-10-12

Ana sayfa onayı `44fd9e3` commit'iyle kaydedildi. Bu rapor, ardından uygulanan
iç sayfa uyarlamasını kapsar; iç sayfa değişiklikleri ayrıca onay/commit bekler.

## Kapsam

- Makale liste/detay: görselli başlık düzeni, mevcut CMS kapakları ve kapaksız
  kartlarda dekoratif kitap simgesi. Filtreler, arama, sayfalama ve tarihler aynı.
  Kapak bulunan detayda ikinci temsili görsel gösterilmez; mevcut kapak korunur.
- Çalışma alanı liste/detay: altın ikonlar, temsili başlık görseli, dengeli kartlar
  ve ikonlu iletişim CTA. Alan isimleri/içerikleri aynı CMS verileridir.
- Hakkımda: eski portre yer tutucusunda açıkça etiketlenmiş temsili ofis görseli.
  Onaylı portre bulunmadığı açıklaması korunur; kişi fotoğrafı üretilmez.
- İletişim: başlık/iletişim kanalı ikonları, sıcak bilgi paneli ve beyaz form kartı.
  Form alanları, hata bağları, CSRF, honeypot, onay ve KVKK modalı değiştirilmez.
- SSS: yardım ikonu, daha tutarlı native details/summary kartları. Sorular,
  yanıtlar, sayfalama ve FAQPage verisi değişmez.
- Hukuki sayfalar: kitap ikonu ve okunabilir beyaz metin paneli. Eksik/yayımlanmamış
  hukuki metinler yazılmaz; mevcut eksik içerik ve noindex kuralları korunur.
- Konum liste/detay: pin ikonları, kartlar ve okuma paneli. Konum/doorway
  kalite değerlendirmesi, hiyerarşi ve indeksleme kuralları değişmez.

## Yeniden kullanım ve dosyalar

Shared/_PublicIcon.cshtml, ana sayfanın mevcut çizgili ikonlarını ortaklaştırır;
Home/_HomeIcon.cshtml aynı giriş noktasını koruyan ince bir wrapper'dır. Ana sayfa
tasarımı değişmez. Shared/_PublicPageIllustration.cshtml yalnızca mevcut yerel
WebP ve görünür temsili/AI açıklamasını render eder. Yeni fotoğraf, ikon paketi,
CDN veya harici font eklenmez. Resmî SVG amblem ve marka tokenları aynı kalır.

Razor değişiklikleri Views/Articles, PracticeAreas, Home/About ve Contact, Faq,
Pages ve Locations altındadır. CSS mevcut site.css içindeki ilgili bölümlerde
düzenlendi; site.min.css build-assets.cjs ile üretildi. Admin/shared layout,
controller, Application/Infrastructure/Domain, DB ve migration değişikliği yok.

ADR-036 sunum bileşenleri notu ve CHANGELOG güncellendi. Mevcut generated görsellerin
kaynağı/promptları design/generated belgelerinde kayıtlıdır; gerçek büro veya
avukat fotoğrafı olarak sunulmazlar. CMS yazıları, biyografi, alıntı, iletişim bilgisi
ve hizmet/başarı iddiası üretilmedi.

## Doğrulama

Genel Release build: 0 hata / 0 uyarı. 216 unit + 247 integration test geçti (463);
11 opt-in MySQL/migration testi atlandı. Önceden bilinen yerel JSON AllowedHosts
testi hariç tutuldu; gerçek ayar/sır dosyası değiştirilmedi. Bu sonuç filtresiz
paketin tamamen yeşil olduğu anlamına gelmez. Hiçbir dış DB/SMTP işlemi yapılmadı.

JS 6/6; asset ve offline 93 kayıt seed parity; scanner sentetik self-testleri 8/8;
publish Python 5/5; IDE0011 ve whitespace kontrolleri geçti. İlk tam browser turu
21/21 geçti: 6 E2E, 9 responsive/erişilebilirlik, 5 admin UX, 1 tam CMS.
Son küçük CTA/hizalama düzenlemelerinden sonra yeni build, ilgili .NET seçimi ve
responsive/erişilebilirlik turu ayrıca çalıştırıldı: build 0 hata/uyarı,
27/27 hedefli integration testi ve 9/9 browser testi geçti (önceki sayımlarla
örtüşür). Sonuç logları aşağıdadır.

İç sayfa başlık/link, canonical, tek H1, odak, form etiketleri, mobil menü, native
FAQ açılımı ve KVKK modalı mevcut testlerle denetlendi. Makale ve çalışma alanları,
hakkımda/iletişim/SSS/hukuki sayfalar 320/390/768/1440 genişliklerinde kontrol edildi.
Konum listesi aynı dört genişlikte son browser turuna eklendi. Konum detayı için
bu işte ayrı tarayıcı fixture kaydı oluşturulmadı; mevcut LocalSeoFlow integration
testleri çalıştırıldı. Bu, gerçek Google indeksleme veya WCAG sertifikası değildir.

Loglar (ignored artifacts/test-runs): inner-enrichment-verification.log,
inner-enrichment-final-build.log, inner-enrichment-final-targeted.log ve
inner-enrichment-final-browser.log. Geçici doğrulama betiği silindi; mevcut runner
ve test kabul kriterleri zayıflatılmadı. Eski screenshot adları korunur; yeni
karşılaştırma çıktıları inner-enriched-* olarak kaydedilir.

## Görsel inceleme ve kalanlar

Masaüstü çalışma alanı detayı ve mobil makale listesi incelendi; fotoğraflar,
ikonlar, kartlar ve form filtreleri taşmadan render edilir. Screenshot içeriği
izole test verisidir, üretim içeriği/onayı değildir. Temsili görsel kullanılmasını
istemeyen içerikler için onaylı CMS kapakları veya gelecekteki ayrı görsel yönetimi
tercih edilebilir; bu görev yeni CMS alanı eklemez.

Gerçek portre/ofis fotoğrafı ve eksik hukuki metinlerin insan onayı halen gerekir.
Hosting yayını veya iç sayfalar için commit yapılmadı. Önce görsel onay, ardından
mevcut yayın prosedürüyle test edilmiş paketin dağıtımı önerilir.
