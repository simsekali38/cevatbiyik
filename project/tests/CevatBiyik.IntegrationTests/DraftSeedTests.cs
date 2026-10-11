using System.Globalization;
using System.Net;
using System.Text.Json;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed class DraftSeedTests
{
    internal static string SeedPath(string name)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CevatBiyik.sln"))) { directory = directory.Parent; }
        return Path.Combine(directory!.Parent!.FullName, "database", "seeds", name);
    }

    [Fact]
    public void IsolatedRows_RespectRealEfColumnsLengthsAndPublicationGates()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseMySql("Server=127.0.0.1;Database=unused;", new MariaDbServerVersion(new Version(10, 3, 39))).Options;
        using var db = new AppDbContext(options);
        using var json = JsonDocument.Parse(File.ReadAllText(SeedPath("REVIEW_DRAFT_CONTENT.json")));
        foreach (var row in json.RootElement.EnumerateArray())
        {
            var entity = db.Model.GetEntityTypes().Single(x => x.GetTableName() == row.GetProperty("table").GetString());
            var data = row.GetProperty("data");
            if (data.TryGetProperty("IsActive", out var active)) { Assert.Equal(1, active.GetInt32()); }
            if (data.TryGetProperty("IsReviewed", out var reviewed)) { Assert.Equal(1, reviewed.GetInt32()); }
            if (data.TryGetProperty("PublishedAt", out var published)) { Assert.True(DateTime.Parse(published.GetString()!, CultureInfo.InvariantCulture) < DateTime.UtcNow); }
            foreach (var column in data.EnumerateObject())
            {
                var property = entity.FindProperty(column.Name);
                Assert.NotNull(property);
                if (column.Value.ValueKind == JsonValueKind.Null) { Assert.True(property.IsNullable); }
                if (column.Value.ValueKind == JsonValueKind.String && property.GetMaxLength() is { } limit) { Assert.True(column.Value.GetString()!.Length <= limit); }
                if (column.Value.ValueKind == JsonValueKind.String) { Assert.DoesNotContain("CONTENT_REQUIRED", column.Value.GetString()!, StringComparison.Ordinal); }
            }
        }
    }

    [Fact]
    public void Sql_HasNoDestructiveOrOperationalWrites_AndKeepsForeignKeysEnabled()
    {
        var sql = File.ReadAllText(SeedPath("REVIEW_DRAFT_CONTENT.sql"));
        var statements = sql.Split('\n').Select(x => x.Trim()).Where(x => !x.StartsWith("--", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(x)).ToArray();
        Assert.All(statements, line => Assert.True(line.StartsWith("INSERT INTO", StringComparison.Ordinal) || line.StartsWith("SET ", StringComparison.Ordinal) || line is "START TRANSACTION;" or "COMMIT;"));
        foreach (var table in new[] { "AspNetUsers", "AspNetRoles", "ContactMessages", "AuditLogs", "Locations", "MediaAssets", "__EFMigrationsHistory" }) { Assert.DoesNotContain("INSERT INTO `" + table + "`", sql, StringComparison.Ordinal); }
        Assert.DoesNotContain("FOREIGN_KEY_CHECKS", string.Join('\n', statements), StringComparison.Ordinal);
        var inserts = statements.Where(x => x.StartsWith("INSERT INTO", StringComparison.Ordinal)).ToArray();
        Assert.Equal(93, inserts.Length);
        var targetHex = Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes("cevodbtest")).ToLowerInvariant();
        Assert.All(inserts, line => Assert.Contains("WHERE BINARY DATABASE()=0x" + targetHex + " AND NOT EXISTS", line, StringComparison.Ordinal));
        var converted = System.Text.RegularExpressions.Regex.Matches(sql, @"CONVERT\(0x[a-f0-9]+ USING utf8mb4\)");
        Assert.NotEmpty(converted);
        foreach (System.Text.RegularExpressions.Match match in converted)
        {
            Assert.StartsWith(" COLLATE utf8mb4_unicode_ci)", sql[(match.Index + match.Length)..], StringComparison.Ordinal);
        }
        Assert.Single(statements, x => x == "START TRANSACTION;");
        Assert.Equal("COMMIT;", statements[^1]);
        Assert.All(inserts.Where(line => line.StartsWith("INSERT INTO `Articles`", StringComparison.Ordinal)), line =>
        {
            Assert.Contains("(SELECT `Id` FROM `PracticeAreas` WHERE `Slug`=", line, StringComparison.Ordinal);
            Assert.Contains("(SELECT `Id` FROM `ArticleCategories` WHERE `Slug`=", line, StringComparison.Ordinal);
        });
    }

    [Fact]
    public void SeedContent_PassesActualCmsValidatorsAndSlugPolicy()
    {
        using var json = JsonDocument.Parse(File.ReadAllText(SeedPath("REVIEW_DRAFT_CONTENT.json")));
        foreach (var row in json.RootElement.EnumerateArray())
        {
            var d = row.GetProperty("data");
            string S(string name) => d.GetProperty(name).GetString()!;
            var seo = new SeoInput();
            if (d.TryGetProperty("MetaTitle", out _))
            {
                seo = new() { MetaTitle = S("MetaTitle"), MetaDescription = S("MetaDescription"), OpenGraphTitle = S("OpenGraphTitle"), OpenGraphDescription = S("OpenGraphDescription") };
                Assert.True(new SeoValidator().Validate(seo).IsValid);
            }
            FluentValidation.Results.ValidationResult? result = row.GetProperty("table").GetString() switch
            {
                "PracticeAreas" => new CreatePracticeAreaValidator().Validate(new CreatePracticeAreaDto { Name = S("Name"), ShortDescription = S("ShortDescription"), Content = S("Content"), SortOrder = d.GetProperty("SortOrder").GetInt32(), IsActive = true, Seo = seo }),
                "Articles" => new CreateArticleValidator().Validate(new CreateArticleDto { Title = S("Title"), Excerpt = S("Excerpt"), Content = S("Content"), PublishedAt = DateTime.SpecifyKind(DateTime.Parse(S("PublishedAt"), CultureInfo.InvariantCulture), DateTimeKind.Utc), PracticeAreaId = d.GetProperty("PracticeAreaId").GetInt64(), CategoryId = d.GetProperty("CategoryId").GetInt64(), IsActive = true, Seo = seo }),
                "ArticleCategories" or "ArticleTags" => new TaxonomyValidator().Validate(new TaxonomyInput { Name = S("Name"), IsActive = true }),
                "FaqItems" => new FaqValidator().Validate(new FaqInput { Question = S("Question"), Answer = S("Answer"), SortOrder = d.GetProperty("SortOrder").GetInt32(), IsActive = true, IsReviewed = true }),
                "LawyerProfiles" => new ProfileValidator().Validate(new ProfileInput { Biography = S("Biography"), Vision = S("Vision"), BarAssociation = S("BarAssociation"), IsActive = true, IsReviewed = true }),
                "PageContents" => new PageContentValidator().Validate(new PageContentInput { Path = S("Path"), Title = S("Title"), Body = S("Body"), IsActive = true, IsReviewed = true }),
                "PageSeo" => new PageSeoValidator().Validate(new PageSeoInput { Path = S("Path"), Seo = seo, IsActive = true }),
                "SiteSettings" => new UpdateSiteSettingValidator().Validate(new UpdateSiteSettingDto { Key = S("Key"), Value = S("Value"), IsActive = true }),
                _ => null
            };
            if (result is not null) { Assert.True(result.IsValid, string.Join(";", result.Errors.Select(x => x.ErrorMessage))); }
            if (d.TryGetProperty("Slug", out var slug))
            {
                var title = d.TryGetProperty("Name", out var name) ? name.GetString()! : S("Title");
                Assert.Equal(new SlugService().Generate(title), slug.GetString());
            }
            if (d.TryGetProperty("Content", out var content)) { Assert.True(content.GetString()!.Length > 1200); }
        }
    }

    [Fact]
    public async Task SeedFixture_PublicRoutesRenderContent_AndAdminCanOpenAndEdit()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using (var scope = factory.Services.CreateScope())
        {
            await CmsSeedFixture.LoadIntoSqliteFixtureAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());
        }
        using var client = factory.CreateHttpsClient();
        using var json = JsonDocument.Parse(File.ReadAllText(SeedPath("REVIEW_DRAFT_CONTENT.json")));
        var rows = json.RootElement.EnumerateArray().ToArray();
        Assert.Equal(16, rows.Count(x => x.GetProperty("table").GetString() == "PracticeAreas"));
        foreach (var row in rows.Where(x => x.GetProperty("table").GetString() is "PracticeAreas" or "Articles"))
        {
            var area = row.GetProperty("table").GetString() == "PracticeAreas";
            var path = (area ? "/uzmanlik-alanlari/" : "/makaleler/") + row.GetProperty("data").GetProperty("Slug").GetString();
            var response = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(row.GetProperty("data").GetProperty(area ? "Name" : "Title").GetString()!, WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()), StringComparison.Ordinal);
        }
        foreach (var path in new[] { "/", "/uzmanlik-alanlari", "/uzmanlik-alanlari?page=2", "/makaleler", "/sss", "/hakkimda", "/iletisim", "/kvkk", "/cerez-politikasi", "/yasal-uyari" })
        {
            var response = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("CONTENT_REQUIRED", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        }
        var form = await client.GetStringAsync("/admin/login");
        var token = System.Text.RegularExpressions.Regex.Match(form, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;
        var login = await client.PostAsync("/admin/login", new FormUrlEncodedContent(new Dictionary<string, string> { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password, ["__RequestVerificationToken"] = WebUtility.HtmlDecode(token) }));
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
        foreach (var row in rows.Where(x => x.GetProperty("table").GetString() is "PracticeAreas" or "Articles"))
        {
            var prefix = row.GetProperty("table").GetString() == "PracticeAreas" ? "/admin/practice-areas/edit/" : "/admin/articles/edit/";
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(prefix + row.GetProperty("id").GetInt64())).StatusCode);
        }
        using var editScope = factory.Services.CreateScope();
        var service = editScope.ServiceProvider.GetRequiredService<CevatBiyik.Application.Interfaces.IPracticeAreaService>();
        var first = (await service.GetAsync(910200001))!;
        await service.UpdateAsync(first.Id, new CreatePracticeAreaDto { Name = first.Name, Content = first.Content, ShortDescription = first.ShortDescription + " Belgeler birlikte değerlendirilir.", SortOrder = first.SortOrder, IsActive = true, Seo = first.Seo });
        Assert.EndsWith("Belgeler birlikte değerlendirilir.", (await service.GetAsync(first.Id))!.ShortDescription, StringComparison.Ordinal);
    }
}
