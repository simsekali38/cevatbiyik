# OPEN_QUESTIONS.md

> **Faz 00 kapanış / Faz 01 güncellemesi — 2026-10-09**
> Bu liste mimariyi bloke etmez; ancak production içeriği ve deployment öncesinde çözülmelidir.
> Durumlar: ✅ Cevaplandı · ⚠️ Kısmen · ❓ Açık · 🔴 Faz 01'i etkiler

## A. Teknik / Hosting

| # | Soru | Durum | Not |
|---|---|---|---|
| 1 | Production domain | ✅ | `cevatbiyik.av.tr` (ADR-009). ❓ `www` → apex 301 hâlâ açık. |
| 2 | Plesk'in desteklediği .NET runtime | ⚠️ | ✅ .NET 8 destekleniyor (proje sahibi beyanı; yazılı teyit yok). ❓ **.NET 10 Hosting Bundle'ın kurulabilirliği ve takvimi** hâlâ açık (ADR-008 yükseltme planı buna bağlı). |
| 13 | Plesk yedekleme (DB + dosya) ve geri yükleme | ❓ | Yedek sıklığı, saklama süresi, geri yükleme testi imkânı. |
| 14 | Hosting'den SMTP | ❓ | Giden e-posta portu/limiti; yoksa harici SMTP sağlayıcı. |
| 15 | Disk kotası ve maksimum yükleme boyutu | ❓ | Medya (Faz 07) limitlerini belirler. |
| 5 | E-posta / SMTP sağlayıcısı | ❓ | #14 ile birlikte karara bağlanacak. |

### Yeni sorular (Faz 00'da eklendi)

| # | Soru | Durum | Not |
|---|---|---|---|
| 16 | **.NET 8 desteği 10 Kasım 2026'da bitiyor.** Doğrudan .NET 10 mu, yoksa .NET 8 ile başlayıp sonra geçiş mi? | ✅ (risk kabul edildi) | Karar: **.NET 8 LTS** (iki kez teyit edildi). Yükseltme planı **ADR-008**'de yazıldı. Yükseltme zamanı Faz 15'te yeniden değerlendirilecek; destek dışı runtime ile launch, Faz 16'te blocker'dır. Detay: `PROJECT_AUDIT.md` R-01. |
| 17 | Windows Server sürümü ve IIS sürümü nedir? | ❓ | Hosting Bundle uyumluluğu için. |
| 18 | MySQL 8.x sunucusu: host, port, **uzaktan bağlantı izni (IP/TLS)**, kullanıcı yetkileri (DDL/migration izni var mı)? | ❓ | Bilgiler Faz 02'de paylaşılacak. Sırlar sohbete yazılmaz, user-secrets ile saklanır (`docs/ENVIRONMENTS.md`). |
| 19 | EF Core için MySQL sağlayıcısı seçimi ve hedef .NET sürümüyle uyumu | ❓ | Faz 02 öncesi ADR. |
| 20 | Yerel geliştirme ortamı: .NET SDK 8, MySQL 8, Git kurulu mu? Uzak Git deposu (GitHub/GitLab/Azure DevOps) olacak mı? | ⚠️ | ✅ .NET 8 SDK kurulu, Git 2.56.0.windows.1, yerel `git init` ve ilk commit yapıldı; `dotnet restore/build/test` başarılı (proje sahibi bildirimi). ✅ Yerel MySQL kurulmayacak (uzak hosting MySQL). ❓ **Uzak Git deposu** kararı açık (yedekleme açısından önerilir, özel/private depo). |
| 21 | Kapsam: ilk sürümde hangi sayfalar zorunlu? (Hakkımda, Hukuk Alanları, Makaleler, SSS, İletişim, KVKK/Gizlilik/Çerez/Yasal Bilgilendirme) | ❓ | Faz 04 ve 08 kapsamı. |
| 22 | Analytics: GA4/GTM kullanılacak mı? Çerez onay (consent) yaklaşımı? | ❓ | Faz 11. |
| 26 | Geliştirme veritabanı: canlı veritabanından ayrı bir geliştirme/test veritabanı açılabilir mi? Hosting uzak MySQL erişimine (IP izni, TLS) izin veriyor mu? | ❓ 🔴 | Faz 02'yi etkiler. Migration'lar canlı veriyi değiştireceği için canlı DB'ye doğrudan geliştirme önerilmez. Bkz. `docs/ENVIRONMENTS.md` bölüm 5. |

## B. İçerik (Hiçbiri uydurulmayacak — eksikler `CONTENT_REQUIRED`)

| # | Soru | Durum |
|---|---|---|
| 3 | Tam ofis adresi | ❓ `CONTENT_REQUIRED` |
| 4 | Telefon ve WhatsApp | ❓ `CONTENT_REQUIRED` |
| 6 | Gerçek uzmanlık / hukuk alanları | ❓ `CONTENT_REQUIRED` |
| 7 | Baro ve sicil bilgileri (hangi bilgilerin yayınlanacağı dahil) | ❓ `CONTENT_REQUIRED` |
| 8 | Mesleki biyografi (eğitim, deneyim) | ❓ `CONTENT_REQUIRED` |
| 9 | Google Business Profile durumu | ❓ |
| 10 | Sosyal/profesyonel profiller | ❓ |
| 11 | Logo / marka varlıkları, profil fotoğrafı | ❓ |
| 12 | Nihai KVKK / gizlilik / çerez / yasal bilgilendirme metinleri | ❓ — avukat onayı gerekir |
| 23 | Çalışma saatleri ve harita konumu | ❓ `CONTENT_REQUIRED` |
| 24 | Mevcut makale/yayınlar ve doğrulanmış SSS içerikleri | ❓ |
| 25 | Mesleki reklam yasağı ve etik kurallar açısından yayın öncesi kontrolü kim yapacak? | ❓ — insan incelemesi zorunlu |

## C. Faz Geçiş Kapıları

**Faz 01'e geçiş (tamamlandı):** #2 (.NET 8 desteği), #16 (.NET sürüm kararı), #20 (yerel ortam) karşılandı.

**Faz 02'ye başlamadan gerekenler:**
1. **#18** — MySQL bağlantı bilgileri (user-secrets ile) ve uzaktan erişim izni.
2. **#26** — Geliştirme veritabanı stratejisi.
3. **#19** — EF Core MySQL sağlayıcısı seçimi (.NET 8 ve .NET 10 uyumlu olmalı; ADR-008).

Diğer sorular faz ilerledikçe, ilgili fazdan önce çözülebilir.
