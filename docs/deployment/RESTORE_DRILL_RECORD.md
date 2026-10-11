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

## Kayıt nasıl doldurulur?

Bu formu her drill için özel operasyon deposunda kopyalayın. Her PASS için gerçekten
çalıştırılmış kontrolün zamanını ve redakte kanıt referansını yazın. Eksik/gözlenmeyen
adım BLOCKED; hata FAIL; uygulanmayan adım gerekçeli N-A olur. Kutuları planlandığı
için PASS yapmayın. Gerçek sırlar, dump, raw HAR ve tam loglar bu repo formuna eklenmez.

| Alan | Gerçek operasyonda kayıt yöntemi |
|---|---|
| drill_id / batch_id | Tekil etiket; DB/files/config/key/ACL aynı batch'e bağlı |
| source_release / target_release | Commit ve güvenilir kayıttan release manifest SHA-256 |
| operator / reviewer / isolated_target | Doğrulanmış kişiler ve izole DB/path etiketi; credential yok |
| backup_recovery_point_utc | Writer freeze/snapshot ile kanıtlı tutarlı veri anı, kopyalama bitişi değildir |
| incident_or_simulated_cutoff_utc | Gerçek kesinti zamanı veya drill'de açıkça simüle kesinti zamanı |
| recovery_started_utc | Operatör restore başlangıcı; süreç süresini ayrıca ölçmek için |
| service_verified_utc | Auth, DB/files ve zorunlu smoke tamamlanıp hizmet doğrulandığı an |
| target_rpo / target_rto | Owner'ın önceden onayladığı hedefler; CONTENT_REQUIRED iken uygunluk verilemez |
| history / table counts / hashes | Kaynak ve restore hedefinin salt okunur karşılaştırma özeti |
| file reconciliation | Tüm cursor sayfaları, eksik/orphan sayıları, değerlendirme; scan auto-purge değildir |
| evidence_refs / outcome / owner_review | Private kanıt etiketleri, PASS/FAIL/BLOCKED ve değerlendirme |

**RPO:** cutoff − backup'ın doğrulanmış recovery point'i; kabul edilebilir veri kaybı
aralığıyla karşılaştırılır. **RTO:** service_verified − cutoff;
alarm/karar/bekleme/restore/smoke süreleri dahil. Restore komutunun tek başına süresi
RTO değildir. Drill'de bu değerler simüle cutoff'a göre ölçülür; gerçek incident SLA
başarısı gibi raporlanmaz. Negatif süre veya eksik timestamp varsa kayıt geçersizdir.

### Açıkça sentetik hesap örneği — operasyon kanıtı değildir

Aşağıdaki zamanlar sadece hesap örneğidir; gerçek tabloya kopyalanmaz:

```text
drill_id: TEST_ONLY_EXAMPLE
backup_recovery_point_utc: 2026-10-11T09:50:00Z
simulated_cutoff_utc:      2026-10-11T10:00:00Z
recovery_started_utc:      2026-10-11T10:05:00Z
service_verified_utc:      2026-10-11T10:35:00Z
measured_rpo: 10 dakika
measured_rto: 35 dakika (restore/smoke bölümü 30 dakika)
owner_target_rpo/rto: CONTENT_REQUIRED
outcome: BLOCKED — hedefler ve gerçek kanıt yok
```

Gerçek batch artefaktları için bir satır/dosya içeren hash mutabakat tablosu tutun:

| Bileşen | Batch | Kaynak SHA-256 / byte | Hedef SHA-256 / byte | Sonuç / kanıt |
|---|---|---|---|---|
| Encrypted DB export | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED |
| Uploads archive / dosya manifesti | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED |
| Private + quarantine | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED |
| Config + keys + ACL export | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED |
| Kod release manifesti | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED | CONTENT_REQUIRED |

Archive transferinde hash'i aynı encrypted byte akışı üzerinden eşleştirin. Restore
edilmiş dosyaları ayrıca kaynak plaintext manifestle private ortamda karşılaştırın;
encrypted archive hash'i unpack edilmiş klasör hash'iyle karşılaştırılmaz. DB dump
hash'i import doğruluğunu tek başına kanıtlamaz: migration history, şema ve tablo
başına kayıt sayıları da doğrulanır. Yeni rotasyon/rekey nedeniyle bilinçli config/key
farkı varsa aynılık iddiası yerine onaylı fark ve decrypt/smoke kanıtı kaydedilir.

Dosya başına salt okunur kontrol (yalnız doğrulanmış private artefakt yolunda):

```powershell
Get-FileHash -LiteralPath '<DOĞRULANMIŞ_ARTEFAKT_YOLU>' -Algorithm SHA256
```

Hash mismatch'te PASS yazmayın; transfer/yedek tekrar incelenir ve geri dönüş kapısı
kapalı kalır. Private anahtar/config hash ve yollarını bile ortak rapora taşımayın;
özet kayıtta private kanıt etiketi kullanın. [Rollback/izleme](ROLLBACK_MONITORING.md)
ve [canlı smoke](LIVE_SMOKE_CHECKLIST.md) ile aynı batch/release ID'yi eşleştirin.
