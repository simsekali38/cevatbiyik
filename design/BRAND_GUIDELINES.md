# Görsel Kimlik — Av. Cevat Bıyık

## Kaynak ve kapsam

2026-10-09 tarihinde kullanıcı tarafından verilen kartvizitler tasarım referansıdır:
- Ön yüz — `20261008_105655.jpg`: isim, mesleki unvan, iletişim düzeni ve amblem.
- Arka yüz — `20261008_105703.jpg`: amblemin daha büyük görünümü ve yatay altın şerit.

Kaynak kartvizit fotoğrafları mevcut checkout içinde bulunmuyor; kırık link yerine
yalnızca tarihsel dosya adları korunur. Public varlık olarak eklenmezler.

Bu belge bundan sonraki public UI ve admin tasarımının görsel kimlik kaynağıdır.
Fotoğraflar baskı, perspektif ve ortam ışığı içerir. Aşağıdaki HEX değerleri
web için seçilen uyarlamalardır; matbaa/Pantone veya orijinal logo renk ölçümü değildir.

## Renk paleti

| Tasarım tokenı | HEX | Kullanım |
|---|---|---|
| `--brand-navy` | `#18264B` | Ana marka rengi; header, footer, başlık, ana buton |
| `--brand-navy-hover` | `#101B36` | Ana buton ve koyu bağlantı hover/active |
| `--brand-gold` | `#D6B36A` | Lacivert üzerindeki amblem, ince şerit, dekoratif vurgu |
| `--brand-gold-ink` | `#806020` | Açık zeminde okunabilir altın tonlu bağlantı/vurgu |
| `--surface-paper` | `#FAF8F3` | Okuma alanının kırık beyaz zemini |
| `--surface-white` | `#FFFFFF` | Form ve içerik kartları |
| `--text-muted` | `#566078` | Açık zeminde ikincil metin |
| `--border-subtle` | `#DCD7CC` | Dekoratif ayırıcı; tek başına form sınırı/focus için yeterli değil |

Kartvizitteki lacivert–altın kimlik esas alınır. Mevcut yeşil/teal vurgu paleti,
bir sonraki UI uyarlamasında bu tokenlarla değiştirilir. Uzun metinler açık zeminde
lacivert kalır; altın büyük yüzeyler yerine ölçülü vurgularda kullanılır.
Bootstrap `primary`, link, button hover/active ve focus stilleri birlikte eşlenir.
Başarı/hata durumları semantik renklerini korur ve metin/ikonla da açıklanır.

## Kontrast ve erişilebilirlik

Hesaplanan sRGB kontrast oranları:

| Ön plan / Arka plan | Oran | Kullanım kararı |
|---|---|---|
| Altın / Lacivert | 7.42:1 | Metin ve vurgu için uygun |
| Lacivert / Kırık beyaz | 13.96:1 | Gövde metni ve başlık için uygun |
| Koyu altın / Kırık beyaz | 5.48:1 | Normal metin bağlantısı için uygun |
| Altın / Kırık beyaz | 1.88:1 | Metin, form sınırı ve tek focus göstergesi olarak kullanılmaz |
| İkincil metin / Beyaz | 6.29:1 | Açık kartlarda ikincil metin için uygun |

Normal metin en az 4.5:1, büyük metin en az 3:1 hedefler.
Kontrol sınırları ve focus göstergeleri bitişik yüzeyle en az 3:1 hedefler.
Açık yüzeyde lacivert, koyu yüzeyde altın focus kullanılır; sınırların iki yüzeyle
birden ayrışması gerekiyorsa iki renkli focus uygulanır. Linkler altı çizili veya
başka görünür işaretlidir. Hover, active, disabled, hata ve mobil menü ayrı doğrulanır.
Bu oranlar tüm site için WCAG uygunluk sertifikası değildir.

## Logo ve marka kilidi

Kartvizitteki amblem: ince dairesel altın çerçeve içinde iç içe C/B harfleri,
merkezde adalet terazisi ve altta sütun/kaide. Dairenin iki yanında yatay altın şerit var.
Bu mevcut işaret korunur; yeni bir terazi simgesi veya farklı monogram icat edilmez.

- Esas amblem referansı arka yüzdür; ad ve unvan düzeni için ön yüz kullanılır.
- Logo en-boy oranı, harflerin ve terazinin birbirine göre konumu korunur.
- Logo lacivert zemin üzerinde altın kullanılır; açık zeminde lacivert bir rozet alanıyla sunulur.
- Gölge, eğme, esnetme, animasyon ve farklı simgelerle birleştirme yapılmaz.
- Çevresinde en az amblem çapının %15'i kadar boşluk bırakılır.
- Başlangıç header boyutu 56–72 CSS px; mobilde 44–56 CSS px. Ayrıntılar okunmazsa büyütülür.
- Fotoğraf tam haliyle logo, favicon veya OG görseli olarak yayımlanmaz.
- Üretim varlığı için kartvizitteki ambleme sadık temiz SVG tercih edilir;
  raster gerekiyorsa saydam PNG/WebP ve netlik kontrolü gerekir.
- Mevcut Razor `CB` metin işareti geçicidir; kartvizit ambleminin uygulanmış hali sayılmaz.
- Marka adı yanında dekoratif logo `alt=""` kullanır; tek başına ana sayfa linki
  ise erişilebilir adı `Av. Cevat Bıyık — Ana sayfa` olur.
- Favicon küçük ölçekte ayrıca okunabilirlik kontrolünden geçer; tam karmaşık amblem otomatik küçültülmez.

## Tipografi ve düzen

Marka yazımı: `Av. Cevat Bıyık`; kartvizit başlık biçimi gerektiğinde `CEVAT BIYIK`.
Türkçe büyük/küçük harf dönüşümü doğru yapılır. İkincil marka satırı:
`HUKUK & DANIŞMANLIK`. Bu ifade kartvizit kaynağına dayanır; uzmanlık veya başarı iddiası eklenmez.

Kartvizitte isim güçlü, dik ve kalın sans-serif görünümdedir; font ailesi fotoğraftan
kesin teşhis edilmez. Marka ve UI için yerel `Segoe UI`, Arial, sans-serif kullanılır.
Uzun makalelerde mevcut Georgia/serif tipografi okunabilirlik için korunabilir;
marka ismi italik veya aşırı ince serif biçimine dönüştürülmez. Harici font/CDN eklenmez.

Header/footer lacivert, marka ve ölçülü çizgiler altın; içerik alanı açık renktir.
Kartvizitteki yatay şerit, header ayırıcı veya kısa bölüm çizgisi olarak yorumlanır.
Mobilde şerit veya logo yatay taşma oluşturmaz. Görsel dil sade, dengeli ve kurumsaldır.

## Kaynak fotoğraflar ve içerik güvenliği

Ön yüzde kamuya açık marka/iletişim bilgilerinin yanında kişisel kimlik numarası bulunur.
Bu numara metne geçirilmez; public asset, log, seed, schema, OCR çıktısı veya OG görseline taşınmaz.
Kaynak JPG'ler `wwwroot` altına kopyalanmaz ve site üzerinden servis edilmez.
Kartvizit görsellerini repo dışına yayımlayan bir işlemde ön yüzün bu alanı çıkarılmalıdır.

İletişim bilgileri için kaynak sağlanmıştır; güncellik/yayın kontrolü içerik intake sürecinde
ayrı yürütülür. Bu görsel kimlik güncellemesi DB/ayar seed'i veya otomatik içerik yayını yapmaz.
Kartvizitte olmayan biyografi, baro, sicil, çalışma saatleri ve hizmetler `CONTENT_REQUIRED` kalır.

## Uygulama durumu ve kabul

2026-10-09: CSS/Razor uyarlaması uygulandı. Yerel, font/harici kaynak içermeyen
`project/src/CevatBiyik.Web/wwwroot/images/brand/cevat-biyik-emblem.svg` eklendi;
header/footer ve ana sayfa bu varlığı kullanır. Fotoğraftan elle çizilen SVG,
amblem öğelerini koruyan web uyarlamasıdır; orijinal vektör dosyasının birebir kopyası değildir.
Orijinal vektör temin edilirse aynı varlık yolu üzerinden değiştirilebilir.
Favicon bu çalışmanın kapsamına eklenmedi.

Uyarlamanın doğrulama adımları:

1. CSS tokenları, Bootstrap varyantları ve header/footer bu rehbere eşlenir.
2. Kartvizit ambleminin temiz web varlığı eklenir; mevcut CB placeholder kaldırılır.
3. 390 px mobil ve masaüstü görünüm, menü, klavye/focus ve logo netliği kontrol edilir.
4. Gerçek bileşenlerin kontrastı, form hata durumları ve public rota testleri doğrulanır.
5. `dotnet build`, `dotnet test` ve ilgili format kontrolleri çalıştırılır.

İlk uyarlama Faz 04 sonrasında ayrı, kayıtlı bir UI çalışması olarak tamamlandı.
2026-10-10 Faz 08: aynı SVG/tokenlar korunarak profil, SSS, hukuki sayfalar, dinamik footer
ve mobil menü tamamlandı. 390 px/klavye/gerçek bileşen kontrast kontrolleri
`../qa/PHASE_08_REVIEW.md` içinde kayıtlıdır. SSS ve formlarda sınır rengi #737B8C;
metin altını #806020, koyu zemindeki marka altını #D6B36A olarak kalır.
Kaynak kart fotoğrafları ve kişisel kimlik/sicil verileri public varlık değildir.
