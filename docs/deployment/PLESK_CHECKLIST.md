# Phase 15 — Plesk yayın kontrol listesi

ADR-033 scope: özel belge modülü kapalı; belge URL'leri admin dahil 404.
PDF/Defender kontrolleri owner kapsam kararıyla N-A; görsel/Skia kontrolleri ve varsa
önceden saklanmış özel dosyaların backup kapsamı korunur.

Hazırlık: [Üretim rehberi](PRODUCTION_RUNBOOK.md), ADR-032.
Kutular gerçek operatör kanıtıyla işaretlenir; yerel test sonuçları hosting kutularını kapatmaz.

## Yerel release hazırlığı

- [ ] Owner onaylı commit; dirty-state olmadan release yeniden üretildi.
- [ ] Build, .NET/JS/tarayıcı/regresyon, format, package audit sonuçları kaydedildi.
- [ ] `tools/publish.ps1` ve release denetimi geçti; SHA-256 manifesti saklandı.
- [ ] Release'de appsettings, gerçek uploads/private, SQL seed, test host, secret/key yok.
- [ ] win-x64 libSkiaSharp.dll ve yerel minified asset'ler tam.

## Sırlar / SH-12-01

- [ ] Production parola korumalı hosting ekranında döndürüldü; eski parola reddediliyor.
- [ ] Yeni credential tüm tüketicilerde korumalı kaynaklarda; değerler loglanmadı.
- [ ] Gerçek JSON Git/publish dışında; server ACL uygulamaya Read / operator yönetimi.
- [ ] Config/App_Data/.git URL'leri public 404; SH-12-01 operatör/tarih/kanıt ile kapatıldı.
- [ ] History cleanup ayrı karar olarak kaydedildi; otomatik history rewrite yapılmadı.

## Runtime / hosting / HTTPS

- [ ] Windows/IIS/Plesk, fiziksel yol ve gerçek app pool kimliği kayıtlı.
- [ ] Patched .NET8 Hosting Bundle / ANCM V2; .NET10 yükseltme takvimi kararlaştırıldı.
- [ ] Dedicated x64 No Managed Code pool; out-of-process, Production, stdout kapalı.
- [ ] DNS/binding/sertifika; www→apex ve HTTP→HTTPS, yenileme takibi doğrulandı.
- [ ] Proxy IP/proto, HTTPS/HSTS, nonce/CSP/cookie başlıkları gerçek hosting'de doğru.
- [ ] Public Kestrel portu ve trust-all yok; rate limiting gerçek IP üzerinden çalışıyor.
- [ ] HTML/private compression kapalı; yalnızca public CSS/JS/SVG sıkıştırılıyor.
- [ ] IIS hata başlıkları/locked sections ve 6 MiB enforcement denetlendi.

## Veritabanı ve migration

- [ ] Actual VERSION(), provider, encoding/collation kaydedildi.
- [ ] MySQL8 hedef farkı, MariaDB10.3 EOL ve DB TLS riskleri owner kararıyla çözüldü.
- [ ] Runtime minimum DML; ayrı/korumalı migration DDL hesabı kullanıldı.
- [ ] EF8 script/bundle aynı commit/dialect ile incelendi ve test DB'de uygulandı.
- [ ] Üretim history/şema incelendi; backup ve bakım aralığı hazır.
- [ ] Canlı migration operatörce uygulandı; fake seed yok; geçici DDL config kaldırıldı.

## Dosyalar / backup / operasyon

- [ ] Kod/config writable değil; uploads/private/key/log ACL'leri doğru.
- [ ] Private/key wwwroot dışında; public mapping/symlink/junction yok.
- [ ] DPAPI key ring recycle sonrası çalışıyor; başka makine rekey planı hazır.
- [ ] Skia görsel ve Defender fail-closed PDF smoke; yetkisiz indirme engeli doğrulandı.
- [ ] Dosya 5 MiB, request 6 MiB, attachment/no-store kontrolleri geçti.
- [ ] DB+uploads+private/quarantine+config+keys aynı batch şifreli/offsite backup'ta.
- [ ] Sıklık/retention/sorumlu/RPO/RTO belirli; backup başarısızlık alarmı var.
- [ ] İzole restore drill, hash/history/tüm cursor mutabakatı ve indirme doğrulandı.
- [ ] Kod/şema rollback ve birlikte restore denendi; körlemesine EF Down yok.
- [ ] Loglarda secret/PII yok; disk/5xx/crash/sertifika/SMTP izleme sorumlusu belli.

## Smoke ve faz çıkışı

- [ ] Public sayfalar/hukuki metinler/iletişim; admin rehber/CRUD/editor/auth/PDF çalışıyor.
- [ ] İletişim DB kaydı ve etkinse STARTTLS SMTP bildirimi doğrulandı.
- [ ] robots/sitemap/canonical/schema/noindex/consent kontrollü analytics doğru.
- [ ] Gerçek içerik/etik/hukuki inceleme tamam; CONTENT_REQUIRED/fake içerik yayımlanmadı.
- [ ] Hosting kanıtları ve Phase 15 raporu tamam; owner Phase 16 geçişini onayladı.

Operatör: CONTENT_REQUIRED · Tarih: CONTENT_REQUIRED · Release/batch: CONTENT_REQUIRED.
