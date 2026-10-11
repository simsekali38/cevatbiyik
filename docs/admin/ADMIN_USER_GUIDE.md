# Yönetim paneli kullanım kılavuzu

Güncelleme: 11 Ekim 2026. İlk kullanımda panelin **Başlangıç ve kullanım rehberi**
bağlantısını açın (`/admin/guide`). Aynı bağlantı yönetim özetinde, üst çubukta ve
menüde bulunur. Sekiz adım ilgili ekrana götürür; mevcut içerikler otomatik sayılır.
“İçerik mevcut” bilgisi hukuki doğruluk veya yayına hazır olma onayı değildir.
Görsel yüklemek zorunlu değildir; kullanmıyorsanız bu adımı geçebilirsiniz.

## Giriş ve temel işlemler

1. `/admin/login` adresinde size verilen yönetici hesabıyla giriş yapın.
2. Telefonda **Menü** düğmesiyle bölümleri açın. **Siteyi görüntüle** ziyaretçi sitesini
   yeni sekmede açar; yönetim formunuz kapanmaz.
3. Listede **Düzenle** (kalem) kayıt formunu açar. **Sil** (çöp kutusu) önce onay
   penceresi açar: kayıt adını kontrol edin. **Vazgeç** hiçbir şeyi değiştirmez.
4. Formu doldurup **Değişiklikleri kaydet** düğmesine basın. Kırmızı hata varsa
   belirtilen alanı düzeltin. Başarı bildirimi kaydın tamamlandığını gösterir.
5. İşiniz bitince **Çıkış yap** düğmesini kullanın.

Listelerde “Bu sayfada ara” yalnızca açık sayfadaki kayıtları arar; diğer kayıtlar
için sayfa bağlantılarını kullanın. Kaydedilmemiş değişiklik varken başka sayfaya
gitmeniz tarayıcı uyarısı açabilir. Silme bazı modüllerde devre dışı bırakma veya
mantıksal silmedir; her modülde geri alma ekranı bulunduğunu varsaymayın.

## Sıfırdan veri giriş sırası

### 1. İletişim bilgilerini hazırlayın

**Site ayarları → Ayar ekle** bölümünde E-posta, Telefon ve Adres bilgilerini ayrı
kayıtlar halinde girin. “Ayar” listesinden bilgiyi seçin, “Değer” alanını doldurun,
**Sitede kullan** seçeneğini işaretleyip kaydedin. Var olan bilgiyi yeniden eklemek
yerine düzenleyin. İsteğe bağlı çalışma saatleri, WhatsApp, site adı ve açıklamasını
aynı şekilde girin. Parola ve SMTP kimlik bilgileri bu ekrana girilmez.

### 2. Avukat profilini oluşturun

**Avukat profili** bölümünde biyografi, vizyon ve doğrulanmış baro adını girin.
İnceleme onayını ve **Yayımla** seçeneğini işaretleyip kaydedin. Kimlik veya sicil
numarası gibi özel verileri girmeyin. Bilinmeyen bilgiyi tahmin etmeyin.
Metin değiştiğinde inceleme onayı kaldırılır; yeniden okuyup onaylayın.

### 3. Hukuki metinleri tamamlayın

**Sayfa içerikleri** bölümünde KVKK, çerez politikası ve yasal uyarı sayfalarını
oluşturun veya düzenleyin. Başlık ve metni girin; incelenmiş ve etkin seçeneklerini
onaylayın. Kullanılan metinlerin gerçek veri işleme sürecine uygunluğu sorumlu
tarafından incelenmelidir; örnek içerik otomatik olarak geçerli sayılmaz.
İletişim formunun açılması ayrıca iletişim/aydınlatma yapılandırmasına bağlıdır.
Metin hazır olduğu halde form kapalıysa teknik sorumluya bildirin.

### 4. Kapak görsellerini yükleyin

**Görsel kütüphanesi** bölümünde JPG, PNG veya WebP seçin (en fazla 5 MiB).
“Görselin kısa açıklaması” alanına görselin neyi anlattığını yazın; bu alternatif metindir. Yükledikten sonra makale formunda
kapak olarak seçebilirsiniz. Bu görseller kamuya açık olabilir; gizli evrak yüklemeyin.
Görsel kullanılmayan içerik için yükleme gerekmez.

### 5. Uzmanlık alanlarını girin

**Uzmanlık alanları → Uzmanlık alanı ekle** bölümünde ad ve açıklama girin.
SEO bölümündeki başlık ve açıklamayı kontrol edin. Hazır içeriği etkinleştirip
kaydedin. Kesin sonuç, başarı garantisi veya doğrulanmamış iddia eklemeyin.

### 6. İlk makaleyi taslak olarak kaydedin

1. İsterseniz önce **Kategoriler** ve **Etiketler** bölümlerinde içerik grupları oluşturun.
2. **Makaleler → Makale ekle** bölümünde başlık, kısa açıklama ve içerik girin.
3. İlgili uzmanlık alanını, kategori/etiketleri ve varsa kapak görselini seçin.
4. **Yayını etkinleştir** kapalıyken kaydedin; içerik taslaktır ve ziyaretçiye görünmez.
5. Düzenleyip **Önizleme** ile biçimini kontrol edin. Önizleme yalnızca metindir;
   ziyaretçi sayfasının tüm düzenini veya arama motoru sonucunu garanti etmez.
6. Hazır olduğunda **Yayını etkinleştir** seçeneğini açın ve yayın zamanı girin.
   Tarih alanı UTC kullanır; altındaki Türkiye saati bilgisini kontrol edin.
   Gelecek tarihli içerik o zaman gelene kadar ziyaretçiye görünmez.
7. Kaydedip ziyaretçi sitesinde makaleyi kontrol edin.

### 7. Sıkça sorulan soruları ekleyin

**Sıkça sorulan sorular → Soru ekle** bölümünde anlaşılır soru ve yanıt girin.
Sıra numarası listedeki konumu belirler. İnceleme ve etkin seçeneklerini onaylayıp
kaydedin. Yanıtı değiştirdiğinizde tekrar inceleyin.

### 8. Ana sayfayı ve ziyaretçi akışını kontrol edin

**Sayfa içerikleri** bölümünde ana sayfa metnini hazırlayın. Ziyaretçi sitesinde
profil, uzmanlık alanları, makale, SSS ve iletişim bilgilerini kontrol edin.
İletişim formundan deneme gönderin ve **İletişim mesajları** bölümünde kaydını görün.
E-posta bildirimi ayrı teknik yapılandırma gerektirir. **SEO kontrolü** uyarılarını
inceleyin. Rehberdeki sayaç tek başına üretim yayınına izin vermez; teknik yayına
alma ve içerik onayı ayrıca yapılır.

## Metin editörünü kullanma

Başlık 2/3/4, kalın, italik, altı/üstü çizili, alt/üst simge, liste, alıntı,
hizalama ve bağlantı araçları bulunur. Metni seçip araca basın. **Tab** formdaki
sonraki alana geçer. **Ctrl+B / Ctrl+I** kalın/italik uygular.

**Kaynak görünümü** HTML bilen kullanıcılar içindir. Düzenledikten sonra
**HTML değişikliklerini uygula** düğmesine basın; bu yalnızca editörü günceller.
Kalıcı kayıt için ayrıca **Değişiklikleri kaydet** gerekir. Güvensiz veya
desteklenmeyen etiketler reddedilir ve mevcut içerik korunur. Kod bilmeniz gerekmez;
normal araç çubuğunu kullanabilirsiniz. Tablo, video, iframe, serbest CSS ve gömülü
görsel desteklenmez. Görseller kapak seçiminden yönetilir. Ayrıntılar:
[Zengin metin rehberi](RICH_TEXT_GUIDE.md).

## Diğer bölümler

| Bölüm | Ne zaman kullanılır? |
|---|---|
| İletişim mesajları | Ziyaretçi mesajını okuyup okundu/okunmadı işaretlemek için. Yanıt kendi e-posta uygulamanızdan verilir; panel e-posta yazma aracı değildir. |
| Sayfa SEO ayarları | Başlık, açıklama ve paylaşım görselini düzenlemek için. SEO uyarısı kesin sıralama garantisi değildir. |
| Yönlendirmeler | Eski URL'yi yeni sayfaya kalıcı yönlendirmek için. Günlük içerik girişi için gerekli değildir. |
| Yerel içerikler | Gerçek, özgün ve ziyaretçiye faydalı yerel içerik için. Kopya ilçe sayfaları üretmeyin; eksik içerikler indekslenmez. |
| Dosya kontrolü | Eksik/sahipsiz dosyaların teknik denetimi için. Karantina ve temizlik işlemlerini anlamadan uygulamayın. |

## Sık karşılaşılan durumlar

- **Kaydettim ama görünmüyor:** Etkin/yayın seçeneğini, yayın tarihini ve varsa
  inceleme onayını kontrol edin. İlgili sayfayı ziyaretçi görünümünde açın.
- **Hata çıktı:** Kırmızı alan mesajını okuyun. Kaydetmeden ayrılmayın; parolaları
  veya kişisel mesaj içeriğini destek kaydına kopyalamayın.
- **Bildirim e-postası gelmedi:** Önce mesajın panelde kaydedildiğini kontrol edin;
  SMTP ve bildirim kuyruğu teknik sorumlu tarafından incelenir.
- **KVKK bağlantısı:** İletişim formunda metin pencere içinde açılır. Kapat veya
  Escape ile forma dönülür; yazılan bilgiler ve sayfa adresi korunur.
  JavaScript kapalıysa bağlantı normal KVKK sayfasını açar.

Kısa sözlük: **Taslak** görünmeyen hazırlık; **Etkin** kullanılabilir kayıt;
**İncelenmiş** sorumlunun içerik onayı; **Slug** URL'deki okunabilir bölüm;
**Meta açıklaması** arama motorlarına sunulan sayfa özeti;
**Alternatif metin** görselin ekran okuyucuya aktarılan açıklamasıdır.
