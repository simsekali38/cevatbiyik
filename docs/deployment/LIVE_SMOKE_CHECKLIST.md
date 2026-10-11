# Phase 17 — Canlı smoke test ve yayın teslim kaydı

Durum: **Çalıştırılmadı.** Release/batch: CONTENT_REQUIRED · Tarih/UTC:
CONTENT_REQUIRED · Operatör: CONTENT_REQUIRED · Site: https://cevatbiyik.av.tr.
Yetkili operatör her satır için sonuç (PASS/FAIL/BLOCKED/N-A), zaman ve redakte kanıt
referansı kaydeder. N-A yalnızca owner'ın kapalı özellik kapsam kararına bağlıdır.

Hazırlık: [Yayın rehberi](FINAL_LAUNCH_RUNBOOK.md). Testleri kontrollü, düşük hacimli
yapın. Brute-force/yük/pixel bomb/malware/500 tetikleme testleri canlıda yapılmaz;
izole prova kanıtı kullanılır. Gerçek başvuru/kişisel veri veya demo içerik yayınlanmaz.

| ID | Kontrol ve beklenen sonuç | Sonuç / zaman / kanıt |
|---|---|---|
| NET-01 | Apex HTTPS sertifika zinciri/tarihi/hostname doğru; HTTPS içerikte mixed-content yok | CONTENT_REQUIRED |
| NET-02 | HTTP ve www URL'leri HTTPS apex'e kalıcı 301/308 ile, loop olmadan gider; www sertifikada kapsanır | CONTENT_REQUIRED |
| NET-03 | /, /hakkimda, /makaleler, /uzmanlik-alanlari, /sss, /iletisim ve hukuki sayfalar beklenen 200 text/html | CONTENT_REQUIRED |
| NET-04 | Proxy gerçek IP/proto aktarır; Kestrel public port yok; HSTS, CSP nonce, DENY/nosniff/referrer/permissions doğru | CONTENT_REQUIRED |
| SEC-01 | /appsettings.json, /appsettings.Production.json, /.git/config, /App_Data/ erişim 404; body'de sır yok | CONTENT_REQUIRED |
| SEC-02 | Anonim /admin ve /admin/documents/download/{doğrulanmış-id} login'e gider/erişim reddedilir; PDF bytes yok | CONTENT_REQUIRED |
| SEC-03 | HTTPS login başarılı; __Host admin cookie Secure/HttpOnly/Strict, Path=/, Domain yok; logout POST sonrası erişim kapanır | CONTENT_REQUIRED |
| SEC-04 | Admin rolü olmayan erişim engeli ve eksik CSRF mutasyon reddi izole prova kanıtıyla eşleşir | CONTENT_REQUIRED |
| SEC-05 | Login/contact/static limit ve trusted proxy kanıtı var; canlıya saldırı yapılmadan gerçek normal IP ayrımı kontrol edildi | CONTENT_REQUIRED |
| ADM-01 | /admin dashboard, /admin/guide, CMS listeleri ve beklenen istatistikler DB ile tutarlı | CONTENT_REQUIRED |
| ADM-02 | Onaylı gerçek taslakta create/edit/save; editörde diğer input focus'u çalışır, safe HTML doğru; yayınlamadan owner inceler | CONTENT_REQUIRED |
| ADM-03 | Soft-delete confirm Cancel veri değiştirmez; yetkili owner kontrollü taslak silme/audit kontrolü yapar; üretim içeriği rastgele silinmez | CONTENT_REQUIRED |
| ADM-04 | Kategori/etiket, uzmanlık/lokasyon ilişkisi, FAQ/profil/hukuki sayfa ve ayarlar doğru; taslak görünmez | CONTENT_REQUIRED |
| CON-01 | Hukuki onaylı contact notice/KVKK var; form Enabled kararı doğru; KVKK modal URL/form değerlerini korur | CONTENT_REQUIRED |
| CON-02 | Yetkili operatörün tek kontrollü mesajı PRG sonrası kaydedilir; admin read/unread çalışır; iletişim verisi logda yok | CONTENT_REQUIRED |
| CON-03 | SMTP etkinse message ID ve queue durumu kaydedilir; 587 STARTTLS, doğru alıcıya tek kontrollü bildirim ulaşır; DB save mail başarı sayılmaz | CONTENT_REQUIRED |
| CON-04 | SMTP kapalı kapsam varsa mail beklenmez; Enabled değişimi recycle sonrası doğru; izinli no-store/consent davranışı bozulmaz | CONTENT_REQUIRED |
| FILE-01 | Owner onaylı temiz görsel ≤5 MiB upload; Skia decode/re-encode WebP, boyut/orientation ve public görüntü doğru | CONTENT_REQUIRED |
| FILE-02 | Temiz onaylı PDF ≤5 MiB yüklenir; Defender scan başarılı; admin attachment/no-store/nosniff/sandbox indirme doğru | CONTENT_REQUIRED |
| FILE-03 | Özel belge wwwroot dışında; public mapping yok; anonim erişim PDF vermez; uploads yalnız GUID WebP GET/HEAD | CONTENT_REQUIRED |
| FILE-04 | 5 MiB file / 6 MiB envelope ve IIS 404.13/proxy 413 engeli izole gerçek IIS kanıtında doğrulandı; executable/spoofing fail-closed | CONTENT_REQUIRED |
| FILE-05 | Pool recycle sonrası dosyalar/DB kayıtları/key ring korunur; cursor reconciliation raporu eksiksiz; yetkisiz otomatik purge yok | CONTENT_REQUIRED |
| ERR-01 | Rastgele mevcut olmayan URL 404 döner; şablon düzgün, stack trace/credential/analytics yok | CONTENT_REQUIRED |
| ERR-02 | Gerçek 500/ANCM crash/500.19 davranışı izole provada doğrulandı; canlıda hata üretmeden mevcut error log/health gözlenir | CONTENT_REQUIRED |
| SEO-01 | Public HTML'de tek title/description/canonical; canonical HTTPS apex, OG URL tutarlı; Host spoof/query kişisel veri taşımıyor | CONTENT_REQUIRED |
| SEO-02 | JSON-LD parse edilebilir ve nonce uyumlu; Article/FAQ/Breadcrumb/LegalService/Person sadece uygun/doğrulanmış olguları içerir | CONTENT_REQUIRED |
| SEO-03 | Indexing kapalıysa noindex/nofollow header+meta, robots Disallow:/, boş sitemap; açıkken indexable içeriğe izin verilir | CONTENT_REQUIRED |
| SEO-04 | /robots.txt text/plain, /sitemap.xml XML; child sitemap URL'leri erişilir, admin/taslak/gelecek/thin/eksik yerel içerik yok | CONTENT_REQUIRED |
| SEO-05 | Arama/pagination/yerel linkler doğru; onaylı eski rota 301 verir, loop/open redirect yok; her ilçeye kopya doorway yayınlanmıyor | CONTENT_REQUIRED |
| ANA-01 | Temiz profilde izin öncesi/reddetmede Google network/script/dataLayer hook yok; admin/error sayfasında analytics yok | CONTENT_REQUIRED |
| ANA-02 | Analitik etkin ve hukuk onaylıysa tek GTM veya GA4 kimliği; consent sonrası nonce ile yüklenir, CSP ihlali yok | CONTENT_REQUIRED |
| ANA-03 | İzinli telefon/WhatsApp/contact olayları gerçek yapılandırılan CTA'da görünür; isim/email/mesaj/query gönderilmez; başarılı save contact_submit tetikler | CONTENT_REQUIRED |
| ANA-04 | İzin geri alma sonraki sayfada script/event'i durdurur; ilgili cookie temizliği/sürüm yenileme; container'da izinsiz üçüncü taraf tag yok | CONTENT_REQUIRED |
| UX-01 | 320/390 px reflow, Tab/Enter/skip link, görünür focus ve dialog focus dönüşü; gerçek içerik contrast/alt/label gözden geçirildi | CONTENT_REQUIRED |
| OPS-01 | Disk, 5xx/ANCM, SMTP backlog, cert ve backup alarm alıcısı/sorumlu test edildi; sonuç ve iletişim kanalı kayıtlı | CONTENT_REQUIRED |

Analitik kapalıysa ANA-02/03 için N-A + owner kapsam kararı girilir; ANA-01'in izinsiz
istek olmaması yine kontrol edilir. WhatsApp/telefon bilinmiyorsa sahte numara eklenmez.
SMTP mesaj ID'leri ve admin test taslağı ID'leri yalnız özel kayıtta tutulur. Kontrollü
işlem sonrası owner'ın retention politikasına göre soft-delete yapılır; SQL purge yok.

Operasyon kanıtına raw HAR/cookie/Authorization/connection string veya form mesajı
eklemeyin; gerekirse özel erişimli depoda redakte kopya tutun. Başarısız security,
DB bütünlüğü veya temel auth kontrolü yayını durdurur ve [rollback](ROLLBACK_MONITORING.md)
kararını tetikler. Sonuç / owner teslim onayı: **CONTENT_REQUIRED**.
