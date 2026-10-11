# Kalan yayın adımları — 2026-10-12

**Yerel geliştirme/test hazır; gerçek yayın onayı verilmedi. NO-GO korunuyor.**
Bu görev sunucuyu veya veritabanını değiştirmez. Kodda migration farkı yoktur.

1. Hosting sağlayıcıya destekli MySQL 8 hedefini ve TLS bağlantısını teyit ettirin.
   Salt okunur cevodbtest denetimi MariaDB 10.3.39 / TLS kapalı buldu; bu production
   hazırlık kanıtı değildir. Database:Provider/ServerVersion gerçek sunucuya uygun
   olmalı. Development, mevcut ignored base JSON'daki MariaDb/10.3.39'u miras alır.
   Bağlantı çalışsın diye sertifika doğrulamasını veya güvenlik kapılarını gevşetmeyin.
2. CMS'de gerçek profil, portre/hero medyası, iletişim bilgileri ve avukat tarafından
   onaylı KVKK/çerez/yasal metinleri tamamlayın. Modelde portre alanı yok; mevcut
   biyografiye güvenli editör/medya üzerinden eklenebilir. Ana hero fotoğrafı için
   ayrıca onaylı görsel ve mevcut medya entegrasyonuna yönelik dar geliştirme gerekir.
   Eğitim/mesleki geçmiş yalnızca doğrulanmış biyografi içinde sunulabilir.
3. Yeni production bağlantısı çalıştığı ve eski parolanın iptal edildiği operasyon
   kaydını tutun. Gerçek JSON dosyalarını Git'e eklemeyin; uygulamaya read-only,
   operatöre yönetim izni verin. Server-owned config'in publish ile ezilmediğini kontrol edin.
4. Onaylanan temiz commit'ten test ve publish çalıştırın. Mevcut yerel paket dirty
   çalışma ağacından bir provadır; doğrudan onaylı release sayılmaz. Windows/IIS
   Hosting Bundle, out-of-process pool, gerçek HTTPS/domain/AllowedHosts/proxy,
   Skia görsel yükleme, App_Data/uploads ACL ve korumalı key-ring'i kontrol edin.
   Özel belge özelliği kapalı kalır; PDF/Defender testi N-A, mevcut dosya yedeği korunur.
5. Aynı batch DB + uploads + özel saklama + config + key-ring + ACL yedeğini alın.
   Ayrı, izole hedefte restore edin; hash, migration/table sayıları, RPO/RTO ve
   uygulama smoke sonuçlarını RESTORE_DRILL_RECORD.md'ye gerçek kanıtla kaydedin.
   Dolu cevodbtest'i restore/migration provası için sıfırlamayın.
6. Domain/HTTPS, admin CMS, form DB kaydı, görsel yükleme, 404/500, canonical,
   sitemap ve robots smoke kontrolünü tamamlayın. SMTP etkinleştirilecekse server
   JSON ile ayarlayıp gerçek teslimi ayrı doğrulayın; bildirim gönderilmiş varsayılmaz.
7. Aşağıdaki dış entegrasyonları owner onayıyla tamamlayın; ancak ardından indexing
   açın ve FINAL_LAUNCH_RUNBOOK.md üzerinden GO/rollback kararını kaydedin.

## SEO ve analitik dış entegrasyonları

- Search Console: gerçek nihai HTTPS domain mülkiyetini owner hesabında DNS TXT ile
  doğrulayın. Bu yöntem uygulamaya script/meta token eklemez. Site:BaseUrl'yi gerçek
  domainle eşleştirin; onaylı indexing açıldıktan sonra /sitemap.xml gönderin.
- Google Business Profile: gerçek, doğrulanmış ofis adresi/telefon/çalışma saatleriyle
  owner tarafından oluşturun/doğrulayın. İlçe/şehir için sahte şube veya doorway
  sayfası açmayın. Public CMS NAP ile tutarlılığı kontrol edin.
- GTM: mevcut Analytics yapılandırması ve çerez onay akışını kullanın. Gerçek ID ve
  onaylı bildirim metni olmadan etkinleştirmeyin. Kabul öncesi script/ağ isteği yok,
  reddetme ve tercih geri alma çalışıyor; nonce CSP gerçek sunucuda doğrulanmalı.
- LegalService/Person schema opt-in kalır; gerçek iletişim ve reviewed profil
  tamamlanmadan etkinleştirilmez. Sahte yorum/puan/ödül eklenmez.

## Güvenli tekrar çalıştırma

```powershell
# Yerel test sunucusu; gerçek DB yazma testleri bilinçli olarak seçilmez.
$env:AllowedHosts = '*'
$env:Site__BaseUrl = 'https://cevatbiyik.av.tr'
pwsh -NoProfile -File tools/test.ps1 -Browser
pwsh -NoProfile -File tools/phase14-audit.ps1 -Stage remaining -NoBuild
pwsh -NoProfile -File tools/read-only-db-check.ps1
pwsh -NoProfile -File tools/publish.ps1 -SkipTests
```

Bu ortam değişkenleri yalnızca test terminali içindir; Production AllowedHosts='*'
önerisi değildir. Read-only araç yalnızca yerel Development JSON'daki cevodbtest'i
kabul eder; sabit SELECT/SHOW sorguları kullanır ve sırları/logları çıktılamaz.
Gerçek 10 persistence ve 1 migration testi yazma yetkisi gerektirir; bu görevde
çalıştırılmadı. Sonuçlar artifacts/test-runs/read-only-db-check.json'dadır.

Detaylı operasyon: PRODUCTION_RUNBOOK.md, PLESK_CHECKLIST.md,
RESTORE_DRILL_RECORD.md, LIVE_SMOKE_CHECKLIST.md, ROLLBACK_MONITORING.md.
