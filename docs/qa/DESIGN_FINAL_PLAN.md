# Son rötuşlar — taşıma planı (2026-10-12)

Başlangıç main/9105423; staged/unstaged değişiklik yok. Envanter ORGANIZATION_PLAN.json.
Taşıma: src/tests/tools + solution/build/package/sdk/tool manifest => project/;
docs/design => design/; docs/database => database/; skills/phases/master/manifest => ai/;
kök teknik rehberler => docs/. Root README/AGENTS/CLAUDE keşif giriş noktaları kalır.
Artifaktlar kökte; .vs ve ignored geçici test.txt silinmez. Hiçbir yinelenen dosya
kanıtsız kaldırılmaz. Yerel JSON, App_Data, uploads ve node_modules aynı taşıma içinde
korunur; yeni yollar önce .gitignore'a eklenir. CI yok (tracked workflow bulunmadı).
Solution içi src/tests relative yolları değişmez; araç cwd project olur, çıktı kökte.
Tüm referans/Markdown bağlantıları çözülerek yeni hedefe taşınır. Taşıma sonrası
restore/build/test ve browser zorunlu. Sonra breadcrumb/header, taslak seed, temsili
hero ve son regresyon. Seed gerçek DB'de otomatik çalıştırılmaz; temiz DB yoksa skip.
