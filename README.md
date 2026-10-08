# Av. Cevat Bıyık — AI Development Blueprint

Bu paket, Av. Cevat Bıyık için geliştirilecek production-ready kişisel hukuk web platformunun AI coding agent'lar ile geliştirilmesi için hazırlanmıştır.

## Hedef

- Tek avukat kişisel marka sitesi
- Kayseri odaklı Local SEO
- Yönetim panelli CMS
- MySQL 8
- ASP.NET Core MVC + Razor
- Bootstrap
- Windows + Plesk hosting
- Dosyaların hosting üzerinde tutulması
- Tek domain
- Ayrı frontend/API deployment'ı yok
- Türkçe
- Güvenlik, SEO, erişilebilirlik ve performans öncelikli

## Kullanım

1. `MASTER_PROMPT.md` dosyasını AI coding agent'a ilk talimat olarak ver.
2. AI'ın önce repository audit ve proje dokümantasyonunu oluşturmasını bekle.
3. `phases/` altındaki fazları sırayla uygulat.
4. Her fazın sonunda ilgili `skills/*/SKILL.md` kurallarını uygulat.
5. Production öncesi `docs/qa/PRODUCTION_READINESS.md` checklist'ini tamamlat.

## Varsayımlar

- Hedef framework: .NET 10 LTS.
- Plesk .NET 10 desteklemiyorsa .NET 8 LTS fallback'tir.
- MySQL 8.x.
- Cloudflare zorunlu değildir.
- Public site server-rendered Razor ile çalışır.
- Admin `/admin` altında aynı uygulamadadır.
- API ilk sürümde yoktur. İleride gerekirse `/api/v1/` aynı domain altında eklenebilir.
- Dosyalar hosting filesystem'inde tutulur.

## Kritik kural

Avukatın gerçek mesleki bilgileri, uzmanlık alanları, baro bilgileri, adresi, telefonları, ödülleri, yorumları veya dava başarıları verilmeden asla uydurulmaz. Eksik alanlar `CONTENT_REQUIRED` olarak bırakılır.
