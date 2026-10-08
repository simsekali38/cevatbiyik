# OPEN_QUESTIONS.md

> **Faz 00 güncellemesi — 2026-10-09**
> Bu liste mimariyi bloke etmez; ancak production içeriği ve deployment öncesinde çözülmelidir.
> Durumlar: ✅ Cevaplandı · ⚠️ Kısmen · ❓ Açık · 🔴 Faz 01'i etkiler

## A. Teknik / Hosting

| # | Soru | Durum | Not |
|---|---|---|---|
| 1 | Production domain | ⚠️ | `avcevatbiyik.com` (geçici/varsayılan). Kayıt sahibi, DNS erişimi, www/non-www canonical tercihi teyit edilmeli. |
| 2 | Plesk'in desteklediği .NET runtime | 🔴 ❓ | Karar: .NET 8 LTS. Hosting Bundle'ın .NET 8 için kurulu olduğu ve **.NET 10 kurulabilirliği** teyit edilmeli. |
| 13 | Plesk yedekleme (DB + dosya) ve geri yükleme | ❓ | Yedek sıklığı, saklama süresi, geri yükleme testi imkânı. |
| 14 | Hosting'den SMTP | ❓ | Giden e-posta portu/limiti; yoksa harici SMTP sağlayıcı. |
| 15 | Disk kotası ve maksimum yükleme boyutu | ❓ | Medya (Faz 05) limitlerini belirler. |
| 5 | E-posta / SMTP sağlayıcısı | ❓ | #14 ile birlikte karara bağlanacak. |

### Yeni sorular (Faz 00'da eklendi)

| # | Soru | Durum | Not |
|---|---|---|---|
| 16 | **.NET 8 desteği 10 Kasım 2026'da bitiyor.** Doğrudan .NET 10 mu, yoksa .NET 8 ile başlayıp sonra geçiş mi? | ✅ (risk kabul edildi) | Karar: **.NET 8 LTS** (iki kez teyit edildi). Yükseltme planı ADR-008'e yazılacak; yükseltme zamanı (launch öncesi mi, sonrası mı) Faz 13 öncesi netleştirilecek. Detay: `PROJECT_AUDIT.md` R-01. |
| 17 | Windows Server sürümü ve IIS sürümü nedir? | ❓ | Hosting Bundle uyumluluğu için. |
| 18 | MySQL 8.x sunucusu: host, port, kullanıcı yetkileri (DDL/migration izni var mı)? | ❓ | Migration stratejisi (Faz 02/13) buna bağlı. |
| 19 | EF Core için MySQL sağlayıcısı seçimi ve hedef .NET sürümüyle uyumu | ❓ | Faz 02 öncesi ADR. |
| 20 | Yerel geliştirme ortamı: .NET SDK 8, MySQL 8, Git kurulu mu? Uzak Git deposu (GitHub/GitLab/Azure DevOps) olacak mı? | ⚠️ | Çalışma dizini: `D:\Projects\cevat-biyik-av-tr\` (dizinde `.git` yok). SDK/MySQL/Git durumu bilinmiyor. Build/test çıktıları yerel makinede çalıştırılacak. |
| 21 | Kapsam: ilk sürümde hangi sayfalar zorunlu? (Hakkımda, Hukuk Alanları, Makaleler, SSS, İletişim, KVKK/Gizlilik/Çerez/Yasal Bilgilendirme) | ❓ | Faz 04 ve 06 kapsamı. |
| 22 | Analytics: GA4/GTM kullanılacak mı? Çerez onay (consent) yaklaşımı? | ❓ | Faz 09. |

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

## C. Faz 01'e Geçmeden Gerekli Minimum Cevaplar

1. **#2 / #17** — Plesk'te desteklenen .NET sürümü (hosting sağlayıcı teyidi). Hosting .NET 8'i desteklemiyorsa plan değişir.
2. **#20** — Yerel ortamda .NET SDK 8, MySQL 8 ve Git sürümleri (çıktıları yapıştırman yeterli) ve Git deposu kararı.

Diğer sorular faz ilerledikçe, ilgili fazdan önce çözülebilir.
