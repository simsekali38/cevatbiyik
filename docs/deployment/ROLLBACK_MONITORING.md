# Phase 17 — Rollback, kurtarma ve yayın sonrası izleme

Durum: **Operasyon şablonu; gerçek incident/restore/monitoring yapılmadı.**
Bağlantılar: [Yayın sırası](FINAL_LAUNCH_RUNBOOK.md), [Smoke](LIVE_SMOKE_CHECKLIST.md),
[Restore kanıt formu](RESTORE_DRILL_RECORD.md). Tüm zamanlar UTC ve incident ID ile
kaydedilir. Primary/on-call/backup kişiler ve private bildirim kanalı CONTENT_REQUIRED.

## 1. Kritik olayda ilk müdahale

Sürekli 5xx/crash, başlatılamayan ANCM, DB bütünlük hatası, secret/private belge
sızıntısı veya temel admin/contact işlev kaybında nöbetçi operatör incident açar,
owner/DBA'yı bilgilendirir. Güvenlik sızıntısında başarılı HTTP 200 görünmesi
sağlıklı sistem sayılmaz. Otomatik restart loop veya migration retry yapılmaz.

1. Doğrulanmış site kökünde bakım açın (`app_offline.htm`); worker ve **tüm** DB
   yazarları durduğunda timestamp girin. Erişim/log kanıtını koruyun; secret'ı loglamayın.
2. DNS/TLS/IIS/ANCM/config/DB/storage kaynaklı sorunu ayırın. Disk full, sertifika
   veya DB bağlantı problemi kod rollback ile otomatik çözülmez. Hosted ortamın
   gerçek hatasını redakte kaydedin; detailed errors/sensitive EF logging açmayın.
3. Yeni trafik başladığından beri contact/upload gibi writes varsa son tutarlı
   backup batch'i ve aradaki farkı koruyun. Eski DB'ye dönüş yeni veriyi kaybettirebilir;
   DBA delta/kayıp etkisi ve owner RPO kararını kaydeder. Canlı veriyi rastgele birleştirmeyin.
4. Önceki kodun **mevcut şemayla uyumu** kanıtlıysa code rollback; değilse tutarlı
   DB+filesystem restore seçilir. Karar/sorumlu/zaman kaydedilir. SQL Down/drop ile
   kurtarma denenmez; MySQL/MariaDB DDL implicit commit nedeniyle kısmi apply olabilir.

## 2. Code rollback — yalnız şema uyumluysa

Bakım sürerken önceki onaylı release'i ve hash manifestini doğrulayın. Kod
dosyalarını envanterle değiştirin; JSON/uploads/private/keys/ACL/bakımı ezmeyin.
Eski config içindeki iptal edilmiş credential'ı geri yüklemeyin; güncel döndürülmüş
credential ve güvenli hosting ayarları korunur. Önceki kodun yeni config/dialect ile
çalışacağı izole provada teyitli olmalı. Stale DLL/assets paket envanteriyle yönetilir.

Pool recycle, bakımın kontrollü kaldırılması ve [smoke](LIVE_SMOKE_CHECKLIST.md)
tekrarından sonra owner hizmete dönüş onayı verir. Başarısızsa bakım tekrar açılır;
hangi release/config'in çalıştığı bilinmeden peş peşe paket denenmez.

## 3. Tutarlı yedekten dönüş — DBA/hosting operatörü

1. Önceden izole hedefte restore edilmiş **aynı batch** seçilir. DB dump, uploads,
   private/quarantine, config, key ring, ACL ve kod hash/batch eşleştirilir.
2. Production hedef adı/physical paths ikinci operatörce doğrulanır. Bakım ve tüm
   writer stop sürer; mevcut problemli DB/storage delil kopyası erişimi kısıtlı tutulur.
3. DBA onaylı bakım hesabıyla DB restore eder. Aynı batch storage ve kod restore
   edilir; başka tarihin DB'siyle dosyalar karıştırılmaz. Aynı makinede DPAPI key
   decrypt kontrolü; başka makinede önceden onaylı rekey ve login/consent yenilemesi.
4. Backup config'i referans alınır; **güncel döndürülmüş credential/TLS** secure
   editörde yeniden uygulanır. DDL/bootstrap sırları temizlenir; ACL tekrar denetlenir.
5. History/şema/table counts ve file hash/reconciliation kontrol edilir. Restore
   sonrası pending SMTP işleri önce değerlendirilir; SMTP kapalı başlatılır.
   Eski backup gönderilmiş işi tekrar pending yapabilir; at-least-once mail tekrarları
   owner/DBA kararıyla yönetilir. Message payload/PII dışarı taşınmaz.
6. Smoke sonrası sadece onaylı contact/SMTP/analytics/indexing kapsamı açılır.
   Writer freeze bitişi, gerçek RPO/RTO, kayıp etkisi ve owner onayı restore formuna girilir.

Hizmet düzeldiğinde olay kapatılmadan root cause, alınan önlem, etkilenen veri,
rotasyon ihtiyacı ve izleme düzeltmesi kaydedilir. Kamuya incident açıklaması owner'ın
ayrı onayına bağlıdır; otomatik dış bildirim/publish yapılmaz.

## 4. İzleme ve görev devri

Bu tablo monitor kurulumunu iddia etmez. Hosting'in mevcut Plesk/IIS araçlarıyla
uygulanır; yeni agent/servis bağımlılığı eklenmez. Primary ve backup kişi isimleri,
alarm kanalı ve SLA owner tarafından doldurulur. Aşağıdaki sıklıklar **başlangıç
önerisidir**, gerçek alarm/test kaydı olmadan devreye alınmış sayılmaz.

| Sinyal / kaynak | Kontrol ve önerilen eşik/sıklık | Rol / primary / backup | İlk aksiyon |
|---|---|---|---|
| HTTPS GET / ve /admin/login | 5 dk; 2 ardışık hata veya beklenmeyen status/redirect | Hosting / CONTENT_REQUIRED / CONTENT_REQUIRED | TLS/DNS/IIS incele, incident aç |
| IIS access 5xx / ANCM Event Viewer/Plesk | 5 dk; startup crash hemen, 5 dk içinde ≥5 sunucu hatası inceleme | Hosting / CONTENT_REQUIRED / CONTENT_REQUIRED | URL/timestamp redakte, maintenance kararı |
| CPU / app pool recycle / bellek | 5 dk; CPU >80% 15 dk veya tekrarlı recycle; RAM eşiği kota/baseline ile CONTENT_REQUIRED | Hosting / CONTENT_REQUIRED / CONTENT_REQUIRED | Process/kota/slow-query incele; kör restart yok |
| Disk / hosting quota / uploads-private-key | 5 dk; %80 uyarı, %90 kritik ve backup için boş alan rezervi | Hosting / CONTENT_REQUIRED / CONTENT_REQUIRED | Kota artır/retention incele; private/key silme yok |
| SSL sertifikası / yenileme job | Günlük; bitişe 30/14/7 gün ve renew fail alarmı | Hosting / CONTENT_REQUIRED / CONTENT_REQUIRED | Apex+www chain/renewal doğrula |
| SMTP pending/attempts/sent / admin panel | İlk 24 saat saatte bir, sonra günlük; exhausted/worker warning hemen; backlog yaşı >15 dk incele | Uygulama / CONTENT_REQUIRED / CONTENT_REQUIRED | STARTTLS/provider/IIS idle doğrula, kontrollü retry |
| Backup job / offsite batch | Her job bitişinde hash/batch başarı; kaçırılan planlı job alarmı | Yedek operatörü / CONTENT_REQUIRED / CONTENT_REQUIRED | Eksik batch'i başarı sayma, yeniden tutarlı al |
| Restore kanıtı / RPO-RTO | İlk launch öncesi, büyük değişiklikten sonra; periyot owner CONTENT_REQUIRED | DBA / CONTENT_REQUIRED / CONTENT_REQUIRED | İzole drill, hedefle ölçüm karşılaştır |
| Sırlar / runtime-DB destek / paket audit | Her release; destek tarihleri aylık ve deadline öncesi takvim | Geliştirici / CONTENT_REQUIRED / CONTENT_REQUIRED | SH/upgrade kaydı, yeni release testi |
| SEO / consent / CWV | Launch, config/content değişikliği; ilk hafta günlük smoke, field CWV yeterli örnek sonrası | İçerik+geliştirici / CONTENT_REQUIRED / CONTENT_REQUIRED | Robots/canonical/consent düzelt; veri yoksa score uydurma |

5xx eşiği düşük trafik için öneridir; güvenlik/bütünlük hatası sayısal eşiği beklemez.
IIS idle shutdown sırasında worker çalışamaz; app pool AlwaysRunning/preload imkânı
hostingce doğrulanır. Admin paneline ulaşmadan bilinmeyen bir health API oluşturulmaz.
İzleme isteği login POST veya kişisel veri taşıyan iletişim POST'u otomatik tekrarlamaz.

## 5. Log ve alarm hijyeni

IIS access log'ları query/IP içerebilir: uygun retention/ACL ile private tutulur,
form verisi/PII URL'ye taşınmaz. ASP.NET/ANCM'de stdout kapalı; ihtiyaç olursa yetkili
operatör kısa süreli private tanı açıp tekrar kapatır. Parola, cookie, form içeriği,
connection string ve raw stack trace ortak alarm kanalına gitmez. Bildirimde yalnız
incident ID, UTC, servis/HTTP status, redakte hata sınıfı ve private kanıt referansı olur.

Owner alarm alıcılarını ve veri saklama politikasını onaylar. Kontrollü sentetik
alarmla primary/backup teslim kanıtı alınır; canlıyı crash ederek test edilmez.
İlk 24 saat: nöbetçi devir ve saatlik özet; ilk 7 gün: günlük kontrol; ardından onaylı
operasyon takvimi. Gerçek kişi/kanal, takip sıklığı ve devir imzası: **CONTENT_REQUIRED**.
