using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.UnitTests;
public sealed class LocalSeoTests
{
    private static Location Good(long id = 1, string name = "Kayseri", string prefix = "fixturea") => new()
    {
        Id = id, Name = name, Slug = name.ToLowerInvariant(), IsActive = true, IsReviewed = true, FactsVerified = true,
        Summary = "TEST_ONLY summary", Body = string.Join(' ', Enumerable.Range(0, 160).Select(i => prefix + i)),
        LocalDetails = string.Join(' ', Enumerable.Range(200, 45).Select(i => prefix + i)),
        PracticeLinks = [new() { IsActive = true, PracticeAreaId = 1, PracticeArea = new() { Id = 1, Name = "TEST_ONLY", Slug = "test-area", IsActive = true } }]
    };
    private static LocationService Service(FakeRepository<Location> locations, FakeRepository<PracticeArea>? areas = null) =>
        new(locations, areas ?? new(), new FakeRepository<Article>(), new FakeRepository<MediaAsset>(), new SlugService(), TimeProvider.System);
    [Fact]
    public void Quality_DetectsThinRepeatedAndPlaceholderText()
    {
        var item = Good(); Assert.Empty(LocalContentQuality.Check(item, [item]));
        item.Body = string.Join(' ', Enumerable.Repeat("aynı", 200)); Assert.NotEmpty(LocalContentQuality.Check(item, [item]));
        item = Good(); item.LocalDetails = "CONTENT_REQUIRED"; Assert.NotEmpty(LocalContentQuality.Check(item, [item]));
    }
    [Theory]
    [InlineData(false, true, true)] [InlineData(true, false, true)] [InlineData(true, true, false)]
    public void Quality_RequiresPublicationReviewAndVerifiedFacts(bool active, bool reviewed, bool facts)
    { var item = Good(); item.IsActive = active; item.IsReviewed = reviewed; item.FactsVerified = facts; Assert.NotEmpty(LocalContentQuality.Check(item, [item])); }
    [Fact]
    public void Quality_DetectsNameSwapsAndPaddedDuplicatesInBothDirections()
    {
        var a = Good(); var b = Good(2, "Talas"); a.Body += " Kayseri"; b.Body += " Talas";
        b.Body += " " + string.Join(' ', Enumerable.Range(300, 30).Select(i => "padding" + i));
        Assert.Contains(LocalContentQuality.Check(a, [a, b]), x => x.Contains("benzerliği", StringComparison.Ordinal));
        Assert.Contains(LocalContentQuality.Check(b, [a, b]), x => x.Contains("benzerliği", StringComparison.Ordinal));
        b.IsActive = false; Assert.Empty(LocalContentQuality.Check(a, [a, b]));
    }
    [Fact]
    public async Task HierarchyAndLinks_AreCheckedBeforeMutation_AndSlugStaysStable()
    {
        var locations = new FakeRepository<Location>(); locations.Seed(Good()); var service = Service(locations);
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(null, new() { Name = "TEST_ONLY district", Kind = LocationKind.District }));
        Assert.Single(locations.Items);
        await service.SaveAsync(null, new() { Name = "TEST_ONLY district", Kind = LocationKind.District, ParentId = 1 });
        var district = locations.Items.Last(); var slug = district.Slug;
        await service.SaveAsync(district.Id, new() { Name = "TEST_ONLY renamed", Kind = LocationKind.District, ParentId = 1 });
        Assert.Equal(slug, district.Slug);
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(district.Id, new() { Name = "TEST_ONLY", Kind = LocationKind.Region, ParentId = 1 }));
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(null, new() { Name = "TEST_ONLY", Kind = LocationKind.Region, ParentId = 1 }));
        await Assert.ThrowsAsync<ConflictException>(() => service.DeleteAsync(1));
        await service.DeleteAsync(district.Id); await service.DeleteAsync(1); Assert.All(locations.Items, x => Assert.True(x.IsDeleted));
    }
    [Fact]
    public async Task SlugsRemainReservedAfterSoftDeletion_AndMissingLinksRejectSave()
    {
        var locations = new FakeRepository<Location>(); locations.Seed(new() { Id = 1, Name = "Kayseri", Slug = "kayseri", IsDeleted = true });
        await Assert.ThrowsAsync<ConflictException>(() => Service(locations).SaveAsync(null, new() { Name = "Kayseri" }));
        var empty = new FakeRepository<Location>();
        await Assert.ThrowsAsync<ValidationException>(() => Service(empty).SaveAsync(null, new() { Name = "Kayseri", PracticeAreaIds = [99] }));
        Assert.Empty(empty.Items);
    }
    [Fact]
    public async Task RuntimeChecks_RemoveLinksAndDescendantsWhenPublicationChanges()
    {
        var repo = new FakeRepository<Location>(); var parent = Good(); var child = Good(2, "Talas", "fixtureb");
        child.Kind = LocationKind.District; child.ParentId = 1; repo.Seed(parent); repo.Seed(child); var service = Service(repo);
        Assert.Equal(2, (await service.IndexableAsync()).Count);
        Assert.Equal("Kayseri", (await service.PublishedAsync("talas"))!.Ancestors.Single().Title);
        parent.PracticeLinks.Single().PracticeArea.IsActive = false;
        Assert.Empty(await service.IndexableAsync()); Assert.Empty(await service.RelatedAsync(practiceAreaId: 1));
        child.IsActive = false; Assert.Null(await service.PublishedAsync("talas"));
    }
    [Fact]
    public async Task Regions_UseThreeLevelsAndWithdrawWhenAnAncestorIsUnreviewed()
    {
        var repo = new FakeRepository<Location>(); var province = Good(); var district = Good(2, "Talas", "fixtureb");
        district.Kind = LocationKind.District; district.ParentId = 1;
        var region = Good(3, "TEST_ONLY region", "fixturec"); region.Kind = LocationKind.Region; region.ParentId = 2;
        repo.Seed(province); repo.Seed(district); repo.Seed(region); var service = Service(repo);
        Assert.Equal(3, (await service.IndexableAsync()).Count);
        Assert.Collection((await service.GetAsync(3))!.Ancestors, x => Assert.Equal("Kayseri", x.Title), x => Assert.Equal("Talas", x.Title));
        province.IsReviewed = false; Assert.Empty(await service.IndexableAsync());
    }
    [Fact]
    public async Task ScheduledAndInactiveAreaArticles_DoNotKeepLocationIndexable()
    {
        var repo = new FakeRepository<Location>(); var location = Good(); location.PracticeLinks.Clear();
        var article = new Article { Id = 7, Title = "TEST_ONLY", Slug = "test-article", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(1) };
        location.ArticleLinks.Add(new() { IsActive = true, ArticleId = 7, Article = article }); repo.Seed(location); var service = Service(repo);
        Assert.Empty(await service.IndexableAsync());
        article.PublishedAt = DateTime.UtcNow.AddDays(-1); Assert.Single(await service.IndexableAsync());
        article.PracticeAreaId = 9; article.PracticeArea = new() { Id = 9, IsActive = false }; Assert.Empty(await service.IndexableAsync());
        article.PracticeArea.IsActive = true; Assert.Single(await service.IndexableAsync());
        article.IsDeleted = true; Assert.Empty(await service.RelatedAsync(articleId: 7));
    }
    [Fact]
    public async Task DeletedLocationReferences_ProtectMediaFromRemoval()
    {
        var media = new FakeRepository<MediaAsset>(); media.Seed(new() { Id = 1, IsActive = true });
        var locations = new FakeRepository<Location>(); locations.Seed(new() { Id = 1, OpenGraphMediaId = 1, IsDeleted = true });
        var service = new MediaService(media, new FakeRepository<Article>(), new FakeRepository<PageSeo>(), new TestImageStorage(), new FakeRepository<PracticeArea>(), locations);
        await Assert.ThrowsAsync<ConflictException>(() => service.DeleteAsync(1)); Assert.False(media.Items.Single().IsDeleted);
    }
    [Fact]
    public async Task LocationScopeAndPagination_AreBounded()
    {
        var repo = new FakeRepository<Location>(); for (var i = 1; i <= 100; i++) { repo.Seed(new() { Id = i, Slug = "draft-" + i }); }
        var service = Service(repo); Assert.Equal(12, (await service.ListAsync()).Items.Count);
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(null, new() { Name = "Kayseri" }));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListAsync(0));
        repo.Seed(new() { Id = 101 }); await Assert.ThrowsAsync<ConflictException>(() => service.IndexableAsync());
    }
    [Theory]
    [InlineData("<script>alert(1)</script>")] [InlineData("TEST_ONLY\nname")]
    public void Validator_RejectsUnsafeNames(string name) => Assert.False(new LocationValidator().Validate(new LocationInput() { Name = name, Kind = LocationKind.District, ParentId = 1 }).IsValid);
    [Fact]
    public void Validator_RejectsPlaceholderReviewAndUnboundedSelections()
    {
        Assert.False(new LocationValidator().Validate(new LocationInput() { Name = "Kayseri", IsReviewed = true }).IsValid);
        Assert.False(new LocationValidator().Validate(new LocationInput() { Name = "Kayseri", PracticeAreaIds = Enumerable.Range(1, 21).Select(x => (long)x).ToArray() }).IsValid);
        Assert.False(new LocationValidator().Validate(new LocationInput() { Name = "Kayseri", ArticleIds = [1, 1] }).IsValid);
        Assert.False(new LocationValidator().Validate(new LocationInput() { Name = "İstanbul" }).IsValid);
    }
}
