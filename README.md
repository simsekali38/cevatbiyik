# Av. Cevat Bıyık

Uygulama: project/CevatBiyik.sln. IDE'de bu solution'ı yeniden açın.

```powershell
cd project
dotnet restore CevatBiyik.sln
pwsh -NoProfile -File tools/test.ps1 -Browser
dotnet run --project src/CevatBiyik.Web --launch-profile https
```

project/: tek kaynak/test/build alanı; docs/: mimari/operasyon; design/: referanslar;
ai/: prompt/faz/skill/manifest; database/: SQL; artifacts/: ignored test çıktıları.
Gerçek JSON/uploads/özel saklama ve sırlar Git'e eklenmez.
Ana kurallar AGENTS/CLAUDE; faz kaynağı ai/project-manifest.json.
Taşıma envanteri docs/qa/ORGANIZATION_PLAN.json.

Eski README: docs/archive/README_PRE_REORGANIZATION.md (tarihsel kayıt).
