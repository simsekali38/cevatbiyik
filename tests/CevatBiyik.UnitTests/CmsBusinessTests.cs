using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.UnitTests;
public sealed class CmsBusinessTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Taxonomy_SoftDeleteReservesSlug_AndRenamePreservesUrl(bool tags)
    {
        var categories = new FakeRepository<ArticleCategory>(); var labels = new FakeRepository<ArticleTag>();
        var service = new TaxonomyService(categories, labels, new SlugService());
        var first = await service.SaveAsync(tags, null, new TaxonomyInput { Name = "İş Hukuku", IsActive = true });
        await service.SaveAsync(tags, first.Id, new TaxonomyInput { Name = "Yeni başlık", IsActive = true });
        Assert.Equal(first.Slug, (await service.GetAsync(tags, first.Id))!.Slug);
        await service.DeleteAsync(tags, first.Id);
        Assert.Null(await service.GetAsync(tags, first.Id));
        Assert.Empty(await service.ListAsync(tags));
        Assert.Equal("is-hukuku-2", (await service.SaveAsync(tags, null, new TaxonomyInput { Name = "İş Hukuku" })).Slug);
    }
    [Theory]
    [InlineData("<script>x</script>")]
    [InlineData("CONTENT_REQUIRED")]
    public async Task Seo_RejectsUnsafeOrPlaceholderText(string value)
    {
        var result = await new SeoValidator().ValidateAsync(new SeoInput { MetaTitle = value });
        Assert.False(result.IsValid);
    }
    [Theory]
    [InlineData("https://attacker.invalid/")]
    [InlineData("/admin")]
    [InlineData("/makaleler?tag=x")]
    public async Task PageSeo_OnlyAllowsKnownPublicPaths(string path)
    {
        Assert.False((await new PageSeoValidator().ValidateAsync(new PageSeoInput { Path = path })).IsValid);
    }
    [Fact]
    public async Task PageSeo_ActiveOnly_AndDuplicatePathAreEnforced()
    {
        var repository = new FakeRepository<PageSeo>();
        var service = new PageSeoService(repository, new FakeRepository<MediaAsset>());
        await service.SaveAsync(null, new PageSeoInput { Path = "/", Seo = new SeoInput { MetaTitle = "Başlık" } });
        Assert.Null(await service.ForPathAsync("/"));
        await Assert.ThrowsAsync<ConflictException>(() => service.SaveAsync(null, new PageSeoInput { Path = "/" }));
        await service.SaveAsync(1, new PageSeoInput { Path = "/", IsActive = true });
        Assert.NotNull(await service.ForPathAsync("/"));
        await service.DisableAsync(1);
        Assert.Null(await service.ForPathAsync("/"));
        await service.SaveAsync(1, new PageSeoInput { Path = "/", IsActive = true });
        Assert.NotNull(await service.ForPathAsync("/"));
    }
    [Fact]
    public async Task Article_TagAssignmentUpdateAndInvalidIdsDoNotMutateContent()
    {
        var articles = new FakeRepository<Article>(); var tags = new FakeRepository<ArticleTag>(); var categories = new FakeRepository<ArticleCategory>();
        categories.Seed(new ArticleCategory { Id = 1, IsActive = true, Name = "Kategori", Slug = "kategori" });
        tags.Seed(new ArticleTag { Id = 1, IsActive = true, Name = "Etiket", Slug = "etiket" });
        var service = new ArticleService(articles, new FakeRepository<PracticeArea>(), new SlugService(), new CreateArticleValidator(), new UpdateArticleValidator(), TimeProvider.System, categories, tags, new FakeRepository<MediaAsset>());
        var item = await service.CreateAsync(new CreateArticleDto { Title = "Başlık", Content = "İçerik", CategoryId = 1, TagIds = [1] });
        Assert.Single(item.Tags);
        Assert.Equal("Kategori", item.CategoryName);
        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(item.Id, new UpdateArticleDto { Title = "Değişmemeli", Content = "İçerik", TagIds = [99] }));
        Assert.Equal("Başlık", (await service.GetAsync(item.Id))!.Title);
        await service.UpdateAsync(item.Id, new UpdateArticleDto { Title = "Başlık", Content = "İçerik", TagIds = [] });
        Assert.Empty((await service.GetAsync(item.Id))!.Tags);
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateArticleDto { Title = "Başlık", Content = "İçerik", TagIds = [1, 1] }));
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateArticleDto { Title = "Başlık", Content = "İçerik", Seo = null! }));
    }
    [Fact]
    public async Task Media_InUseCannotBeDeleted_UnusedAssetIsSoftDeleted()
    {
        var media = new FakeRepository<MediaAsset>(); var articles = new FakeRepository<Article>(); var storage = new TestImageStorage();
        var service = new MediaService(media, articles, new FakeRepository<PageSeo>(), storage, new FakeRepository<PracticeArea>());
        using var stream = new MemoryStream();
        var image = await service.UploadAsync(stream, "test.png", "image/png", "Görsel");
        articles.Seed(new Article { Id = 1, CoverMediaId = image.Id, IsDeleted = true });
        await Assert.ThrowsAsync<ConflictException>(() => service.DeleteAsync(image.Id));
        articles.Items.Clear(); await service.DeleteAsync(image.Id);
        Assert.True(media.Items.Single().IsDeleted);
        Assert.Equal(0, storage.DeleteCount);
    }
    [Fact]
    public async Task ScheduledPublication_BecomesVisibleAtUtcBoundaryWithoutWorker()
    {
        var date = new DateTime(2026, 10, 10, 12, 0, 0, DateTimeKind.Utc);
        var clock = new CmsClock(date.AddTicks(-1)); var articles = new FakeRepository<Article>();
        articles.Seed(new Article { Id = 1, Slug = "scheduled", Title = "Test", Content = "Test", IsActive = true, PublishedAt = date });
        var service = new ArticleService(articles, new FakeRepository<PracticeArea>(), new SlugService(), new CreateArticleValidator(), new UpdateArticleValidator(), clock);
        Assert.Null(await service.GetPublishedBySlugAsync("scheduled"));
        Assert.Empty((await service.SearchCmsPublishedAsync(null, null, null, null)).Items);
        clock.Now = date;
        Assert.NotNull(await service.GetPublishedBySlugAsync("scheduled"));
        Assert.Single((await service.SearchCmsPublishedAsync(null, null, null, null)).Items);
        Assert.Equal(0, articles.SaveCount);
    }
    [Fact]
    public async Task Media_InvalidAltTextIsRejectedBeforeWritingFile()
    {
        var storage = new TestImageStorage();
        var service = new MediaService(new FakeRepository<MediaAsset>(), new FakeRepository<Article>(), new FakeRepository<PageSeo>(), storage, new FakeRepository<PracticeArea>());
        using var stream = new MemoryStream();
        await Assert.ThrowsAsync<ValidationException>(() => service.UploadAsync(stream, "test.png", "image/png", "<img onerror=x>"));
        Assert.Equal(0, storage.StoreCount);
    }
}
internal sealed class CmsClock(DateTime now) : TimeProvider
{
    public DateTime Now { get; set; } = now;
    public override DateTimeOffset GetUtcNow() => new(Now);
}
public sealed class TestImageStorage : IImageStorage
{
    public int StoreCount { get; private set; }
    public int DeleteCount { get; private set; }
    public Task<StoredImage> StoreAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    { StoreCount++; return Task.FromResult(new StoredImage("images/" + new string('a', 32) + ".webp", "image/webp", 10, 10, 100)); }
    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default) { DeleteCount++; return Task.CompletedTask; }
}
