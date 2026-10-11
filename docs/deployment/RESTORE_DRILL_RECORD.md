# İzole yayın / yedekten dönüş kanıt formu

Durum: **Çalıştırılmadı — operatör kanıtı bekleniyor.** Bu formun doldurulması bir
başarı kanıtı değildir; aşağıdaki sonuçlar gerçekten gözlemlenmelidir.
Prosedür: [Üretim rehberi](PRODUCTION_RUNBOOK.md#8-yayın-rollback-ve-birlikte-backuprestore).

## Ön koşullar

- Operatör, tarih, bakım aralığı, onaylanan commit ve release SHA-256: CONTENT_REQUIRED.
- İzole restore veritabanı ve public/private/key dizinleri: CONTENT_REQUIRED.
- Hedef **cevodb veya mevcut dolu cevodbtest değildir**; hedef ve yetkiler DBA tarafından doğrulanır.
- SMTP/analitik kapalı, indeksleme kapalı; restore verisine public erişim yok.
- Aynı batch DB dump + uploads + private/quarantine + korumalı config + key ring + ACL
  alınır; tüm yazarların durduğu kayıt altına alınır. Parolalar bu forma girilmez.
- DB sürümü/dialect, runtime/ANCM, disk kapasitesi ve backup hash'leri doğrulanır.

## Çalıştırma ve kanıt

| Kontrol | Beklenen | Gözlem / kanıt |
|---|---|---|
| Backup hash ve bütünlüğü | Tüm bileşenler aynı batch, checksum eşleşmesi | CONTENT_REQUIRED |
| İzole DB import | Hatasız; migration history ve tablo/kayıt sayıları kaynakla eşleşir | CONTENT_REQUIRED |
| Migration provası | Onaylanan SQL/bundle sadece izole DB'de; tekrar uygulanmada veri kaybı yok | CONTENT_REQUIRED |
| Dosya restore | Public, private, quarantine ve ACL eksiksiz | CONTENT_REQUIRED |
| Reconciliation | Tüm cursor sayfaları incelenir; eksik/orphan listesi değerlendirilir | CONTENT_REQUIRED |
| Kimlik doğrulama | Admin giriş/çıkış, yetkisiz belge reddi ve CSRF çalışır | CONTENT_REQUIRED |
| Dosya smoke | Yeni görsel decode/resize, taranmış PDF yükleme ve yetkili indirme | CONTENT_REQUIRED |
| DPAPI/key ring | Aynı makinede decrypt; farklı makinede onaylı rekey ve yeniden login | CONTENT_REQUIRED |
| Yapılandırma | Sır URL'leri engelli; Production, HTTPS/CSP ve kapalı stdout | CONTENT_REQUIRED |
| Kod rollback | Şema uyumu doğrulanmış; aksi halde aynı batch restore | CONTENT_REQUIRED |
| RPO/RTO | Gerçek kayıp aralığı ve geçen süre ölçülür, owner hedefiyle karşılaştırılır | CONTENT_REQUIRED |

Hata halinde site kapalı tutulur; production'a SQL uygulanmaz. `Down` migration veya
mevcut tabloları silme, restore yerine geçmez. Hassas yedekler ve loglar Git'e konulmaz.
Başarılı/başarısız sonuç, operatör ve owner değerlendirmesi: **CONTENT_REQUIRED**.
