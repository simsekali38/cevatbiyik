using System.Text.Json;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace CevatBiyik.IntegrationTests;

/// <summary>Manual test-only fixture. Never registered by Application/Infrastructure or Web startup.</summary>
public sealed class DemoDataSeeder(AppDbContext db)
{
    public const string MarkerKey = "Demo.TestOnly.v1.Manifest";
    public const string Prefix = "demo-test-only-v1-";
    public const string ProfileKey = "demo-test-only-v1";
    private const string Text = "TEST_ONLY DEMO v1";
    private static readonly string[] Districts = ["Melikgazi", "Kocasinan"];
    public string LastStep { get; private set; } = "target";

    public static void ValidateTarget(string? connection)
    {
        var parsed = new MySqlConnectionStringBuilder(MySqlTestGuard.Validate(connection));
        if (parsed.Database != "cevodbtest" || parsed.Server != "217.195.207.24" || parsed.UserID != "cevodbusertest")
        { throw new InvalidOperationException("Demo data is restricted to the exact owner-approved cevodbtest target."); }
    }

    private void EnsureTarget()
    {
        if (db.Database.IsSqlite() && new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(db.Database.GetConnectionString()).DataSource == ":memory:") { return; }
        ValidateTarget(db.Database.GetConnectionString());
    }
    public async Task<string> StatusAsync()
    {
        EnsureTarget();
        return await db.SiteSettings.IgnoreQueryFilters().AnyAsync(x => x.Key == MarkerKey)
            ? "Demo manifest exists in cevodbtest; production unchanged." : "No owned demo manifest in cevodbtest.";
    }

    public async Task<string> SeedAsync()
    {
        EnsureTarget();
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        if (await db.SiteSettings.IgnoreQueryFilters().AnyAsync(x => x.Key == MarkerKey))
        { return "Demo already seeded; existing rows not overwritten."; }
        if (await db.Articles.IgnoreQueryFilters().AnyAsync(x => x.Slug.StartsWith(Prefix))
            || await db.PracticeAreas.IgnoreQueryFilters().AnyAsync(x => x.Slug.StartsWith(Prefix))
            || await db.Locations.IgnoreQueryFilters().AnyAsync(x => x.Slug.StartsWith(Prefix))
            || await db.LawyerProfiles.IgnoreQueryFilters().AnyAsync(x => x.Key == ProfileKey)
            || await db.MediaAssets.IgnoreQueryFilters().AnyAsync(x => x.AltText.StartsWith(Text))
            || await db.FaqItems.IgnoreQueryFilters().AnyAsync(x => x.Question.StartsWith(Text)))
        { throw new InvalidOperationException("Unowned demo markers exist; refusing to adopt or overwrite rows."); }
        var areas = Enumerable.Range(1, 2).Select(i => new PracticeArea
        { Name = Text + " uzmanlık " + i, Slug = Prefix + "area-" + i, Content = Text + " arayüz örneği; hukuki bilgi değildir.", IsActive = true }).ToArray();
        var articles = Enumerable.Range(1, 3).Select(i => new Article
        {
            Title = Text + " makale " + i, Slug = Prefix + "article-" + i, Content = Text + " içerik örneği; hukuki görüş değildir.",
            Excerpt = Text + " örnek özet", PracticeArea = areas[(i - 1) % 2], IsActive = i != 2,
            PublishedAt = i == 2 ? null : i == 3 ? new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc) : new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        }).ToArray();
        var province = new Location { Name = Text + " Kayseri", Slug = Prefix + "kayseri", Kind = LocationKind.Province, IsActive = true, Body = Text, Summary = Text, LocalDetails = "CONTENT_REQUIRED" };
        var locations = new[] { province }.Concat(Districts.Select(name => new Location
        { Name = Text + " " + name, Slug = Prefix + name.ToLowerInvariant(), Kind = LocationKind.District, Parent = province, IsActive = true, Body = Text, Summary = Text, LocalDetails = "CONTENT_REQUIRED" })).ToArray();
        var faq = new FaqItem { Question = Text + " arayüz nasıl sınanır?", Answer = Text + " Bu kayıt yalnızca test içindir.", IsActive = true, IsReviewed = false };
        var profile = new LawyerProfile { Key = ProfileKey, Biography = Text + " CONTENT_REQUIRED", Vision = "CONTENT_REQUIRED", BarAssociation = "CONTENT_REQUIRED", IsActive = true, IsReviewed = false };
        var media = Enumerable.Range(1, 2).Select(i => new MediaAsset
        { StoragePath = "images/" + i.ToString("x32", System.Globalization.CultureInfo.InvariantCulture) + ".webp", AltText = Text + " metadata-only " + i, ContentType = "image/webp", Width = 1, Height = 1, Bytes = 0, IsActive = false }).ToArray();
        var areaLinks = locations.Select(x => new LocationPracticeArea { Location = x, PracticeArea = areas[0] }).ToArray();
        var articleLinks = locations.Select(x => new LocationArticle { Location = x, Article = articles[0] }).ToArray();
        db.AddRange(areas); db.AddRange(articles); db.AddRange(locations); db.AddRange(media); db.Add(faq); db.Add(profile); db.AddRange(areaLinks); db.AddRange(articleLinks);
        await db.SaveChangesAsync();
        var manifest = new Dictionary<string, long[]> { ["areas"] = areas.Select(x => x.Id).ToArray(), ["articles"] = articles.Select(x => x.Id).ToArray(), ["locations"] = locations.Select(x => x.Id).ToArray(),
            ["media"] = media.Select(x => x.Id).ToArray(), ["faq"] = [faq.Id], ["profile"] = [profile.Id], ["areaLinks"] = areaLinks.Select(x => x.Id).ToArray(), ["articleLinks"] = articleLinks.Select(x => x.Id).ToArray() };
        db.SiteSettings.Add(new SiteSetting { Key = MarkerKey, Value = JsonSerializer.Serialize(manifest), IsActive = false });
        await db.SaveChangesAsync(); await transaction.CommitAsync();
        return "Seeded: 3 articles, 2 practice areas, 3 locations, FAQ, active unreviewed profile, 2 metadata-only media and 6 links. TEST_ONLY; no files created.";
    }

    public async Task<string> CleanupAsync()
    {
        EnsureTarget();
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var marker = await db.SiteSettings.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Key == MarkerKey);
        if (marker is null) { return "No owned demo data to clean."; }
        var ids = JsonSerializer.Deserialize<Dictionary<string, long[]>>(marker.Value) ?? throw new InvalidOperationException("Invalid ownership manifest.");
        // Require every tracked row to retain its test identity. Never sweep a table by a prefix alone.
        await Check(Owned<Article>(ids["articles"]).Where(x => x.Slug.StartsWith(Prefix)), ids["articles"]);
        await Check(Owned<PracticeArea>(ids["areas"]).Where(x => x.Slug.StartsWith(Prefix)), ids["areas"]);
        await Check(Owned<Location>(ids["locations"]).Where(x => x.Slug.StartsWith(Prefix)), ids["locations"]);
        await Check(Owned<MediaAsset>(ids["media"]).Where(x => x.AltText.StartsWith(Text)), ids["media"]);
        await Check(Owned<FaqItem>(ids["faq"]).Where(x => x.Question.StartsWith(Text)), ids["faq"]);
        await Check(Owned<LawyerProfile>(ids["profile"]).Where(x => x.Key == ProfileKey && x.Biography.StartsWith(Text)), ids["profile"]);
        await Check(Owned<LocationArticle>(ids["articleLinks"]), ids["articleLinks"]);
        await Check(Owned<LocationPracticeArea>(ids["areaLinks"]), ids["areaLinks"]);
        if ((await Owned<LocationArticle>(ids["articleLinks"]).ToArrayAsync()).Any(x => !ids["locations"].Contains(x.LocationId) || !ids["articles"].Contains(x.ArticleId))
            || (await Owned<LocationPracticeArea>(ids["areaLinks"]).ToArrayAsync()).Any(x => !ids["locations"].Contains(x.LocationId) || !ids["areas"].Contains(x.PracticeAreaId)))
        { throw new InvalidOperationException("Demo link ownership changed."); }
        if (await MatchIds<ArticleTagLink>(ids["articles"], nameof(ArticleTagLink.ArticleId)).AnyAsync()
            || await MatchIds<LocationArticle>(ids["locations"], nameof(LocationArticle.LocationId)).CountAsync() != ids["articleLinks"].Length
            || await MatchIds<LocationPracticeArea>(ids["locations"], nameof(LocationPracticeArea.LocationId)).CountAsync() != ids["areaLinks"].Length)
        { throw new InvalidOperationException("Non-demo dependent rows would cascade; cleanup rejected."); }
        await Owned<LocationArticle>(ids["articleLinks"]).ExecuteDeleteAsync();
        await Owned<LocationPracticeArea>(ids["areaLinks"]).ExecuteDeleteAsync();
        await Owned<Article>(ids["articles"]).ExecuteDeleteAsync();
        await Owned<PracticeArea>(ids["areas"]).ExecuteDeleteAsync();
        await Owned<Location>(ids["locations"]).Where(x => x.ParentId != null).ExecuteDeleteAsync();
        await Owned<Location>(ids["locations"]).ExecuteDeleteAsync();
        await Owned<MediaAsset>(ids["media"]).ExecuteDeleteAsync();
        await Owned<FaqItem>(ids["faq"]).ExecuteDeleteAsync();
        await Owned<LawyerProfile>(ids["profile"]).ExecuteDeleteAsync();
        await db.SiteSettings.IgnoreQueryFilters().Where(x => x.Id == marker.Id && x.Key == MarkerKey).ExecuteDeleteAsync();
        await transaction.CommitAsync(); db.ChangeTracker.Clear();
        return "Owned demo rows cleaned; tables, migration history, accounts, non-demo rows and files preserved.";
    }
    private async Task Check<T>(IQueryable<T> rows, long[] ids) where T : class
    {
        LastStep = typeof(T).Name;
        if (ids.Length == 0 || ids.Any(x => x <= 0) || ids.Distinct().Count() != ids.Length || await rows.CountAsync() != ids.Length)
        { throw new InvalidOperationException("Demo ownership changed; cleanup rejected."); }
    }

    private IQueryable<T> Owned<T>(long[] ids) where T : BaseEntity => MatchIds<T>(ids, nameof(BaseEntity.Id));
    private IQueryable<T> MatchIds<T>(long[] ids, string property) where T : class
    {
        if (ids.Length is < 1 or > 20) { throw new InvalidOperationException("Invalid manifest size."); }
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "row");
        System.Linq.Expressions.Expression predicate = System.Linq.Expressions.Expression.Constant(false);
        foreach (var id in ids)
        {
            predicate = System.Linq.Expressions.Expression.OrElse(predicate, System.Linq.Expressions.Expression.Equal(
                System.Linq.Expressions.Expression.Property(parameter, property), System.Linq.Expressions.Expression.Constant(id)));
        }
        return db.Set<T>().IgnoreQueryFilters().Where(System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(predicate, parameter));
    }
}
