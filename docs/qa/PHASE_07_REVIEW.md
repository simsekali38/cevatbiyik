# Faz 07 doğrulama raporu — 2026-10-10

Durum: Kod uygulandı ve yerelde doğrulandı; proje sahibi 2026-10-10 tarihinde kapanış ve commit onayını verdi.
00–07 tamamlandı; 08 başlatılmadı. ADR-019 hattı korunmuş, ADR-020 eklenmiştir.

## Çalıştırılan kontroller

| Komut / kontrol | Sonuç |
|---|---|
| `dotnet build -c Release --no-restore` | 0 hata, 0 uyarı |
| `dotnet test -c Release --no-restore --logger "trx;LogFilePrefix=phase07-final"` | 86 unit + 67 integration = **153 başarılı**, 0 başarısız, 5 MySQL skip |
| `dotnet format style CevatBiyik.sln --no-restore --diagnostics IDE0011` | Başarılı; son build stil uyarısı yok |
| EF 8 `migrations add MediaFiles`, `migrations script CmsFeatures MediaFiles` | Oluşturuldu; PrivateDocuments ve unique StoragePath, seed/destructive Up yok |
| Model / migration testleri | 18 entity type, pending model change yok, SQL unique index doğrulandı |
| `dotnet publish project/src/CevatBiyik.Web -c Release --no-build --no-restore -o artifacts/phase07-publish` | Başarılı; yayınlanmadı |
| Publish XML/native incelemesi | maxAllowedContentLength **6291456**; win-x64/native/libSkiaSharp.dll var |
| Private publish exclusion smoke | App_Data içine zararsız JSON probe kondu; pakete alınmadığı doğrulandı, probe kaldırıldı |
| `git diff --check` | Whitespace hatası yok; Git CRLF normalizasyon bilgilendirmeleri var |

TRX çıktı dizinleri: project/tests/CevatBiyik.UnitTests/TestResults ve project/tests/CevatBiyik.IntegrationTests/TestResults (ignored).
Yerel QA publish appsettings bağlantısı boşaltıldı, public yayın/content gate'leri kapatıldı; kaynak appsettings.json değiştirilmedi.

## Güvenlik ve iş mantığı kapsamı

- EXE uzantısı, yanıltıcı MIME, MZ/görünürde PDF, fazla byte, bozuk sınır/içerik ve active PDF name/hex escapes reddi.
- İstemci adının storage key'e dönüşmemesi; ADS/kontrol karakteri/son boşluk reddi; public-root altında private-root reddi.
- Antivirüs yok/hata sonucu fail-closed. MVC scan reddinde disk/DB kayıt bırakmama; test scanner sadece izole test factory'de.
- Admin dışında/anonim belge ve mutabakat erişimi engeli; CSRF olmadan upload 400; private/public yanlış file yolları 404.
- Gerçek MVC/Identity cookie/SQLite persistence upload → audited attachment download → delete/404 → restore → checksum tamper/409.
- DB/audit rollback sonrası dosya telafisi; soft-deleted kayıtların orphan sayılmaması; eksik DB dosyalarının raporlanması.
- Genç orphan taşınmasının/traversal'ın reddi; eski partial orphan'ın private quarantine'a taşınması ve overwrite olmadan geri taşınması.
- 105 dosyanın 100+5 cursor sayfasında tekrar etmeden taranması; eksik görsel geri alma işleminde reactivation yapılmaması.
- Orijinal PDF bytes geri konduğunda doğrulanmış indirme tekrar başarılı: izole dosya recovery smoke testi.
- Kamu SEO/sitemap/CSRF/Identity/CMS regresyonları yeşil; private belgeler public sitemap'e girmez.

## Açık operasyonel doğrulamalar

CEVATBIYIK_TEST_MYSQL ayarlı olmadığı için **5** dedicated MySQL test atlandı. Bunlardan biri yeni PrivateDocuments unique-path/deleted-catalog doğrulamasıdır.
SQLite MVC testleri MySQL migration veya gerçek hosting testi yerine geçmez. `cevodb database update` çalıştırılmadı.
Gerçek Defender çalıştırılmadı; Windows app-pool izinleri, güncel imza/exclusion kontrolü ve gerçek tarama hosting gate'idir.
PDF kapısı tam parser/CDR değildir; unsupported PDF özellikleri bilerek reddedilir ve bilinmeyen malware için mutlak güvence verilmez.
Plesk ACL/virtual-directory/IIS limitleri, gerçek app-pool native load, tutarlı DB+iki filesystem backup/restore drill ve RPO/RTO ölçümü çalıştırılmadı.
Hukuki retention süresi **CONTENT_REQUIRED**; otomatik hard purge yoktur. Tüm bu adımlar PLESK_MEDIA_RUNBOOK.md'de somutlaştırılmıştır.

Concurrent project/src/CevatBiyik.Web/appsettings.json değişiklikleri kullanıcıya aittir; korunmuş ve Faz 07 değişikliği olarak değerlendirilmemiştir.
Kapanış ve commit proje sahibi tarafından onaylandı; deployment yapılmadı. Kapanış onayı kod kabulünü temsil eder; açık hosting gate'lerini geçtiği anlamına gelmez.
