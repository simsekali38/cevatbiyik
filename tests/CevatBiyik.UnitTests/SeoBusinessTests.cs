using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.UnitTests;

public sealed class SeoBusinessTests
{
    [Theory]
    [InlineData("https://evil.test")]
    [InlineData("//evil.test")]
    [InlineData("/\\evil.test")]
    [InlineData("/admin/login")]
    [InlineData("/ADMIN")]
    [InlineData("/uploads/images/test.webp")]
    [InlineData("/a/../b")]
    [InlineData("/a%2fb")]
    [InlineData("/target?x=1")]
    [InlineData("/target#section")]
    [InlineData("/target\r\nLocation:evil")]
    [InlineData("/target/")]
    [InlineData("/a//b")]
    public void Redirects_RejectUnsafeOrAmbiguousDestinations(string path)
    { Assert.False(new RedirectValidator().Validate(new RedirectInput { SourcePath = "/old", DestinationPath = path }).IsValid); }
    [Fact]
    public async Task Redirects_DisableAndReenable_ReserveSource_AndRejectLoopsChainsAndCurrentContent()
    {
        var repository = new FakeRepository<RedirectRule>(); var routes = new TestRoutes();
        routes.Paths.Add("/makaleler/current"); var service = new RedirectService(repository, routes);
        var input = new RedirectInput { SourcePath = "/Old", DestinationPath = "/makaleler/current", IsActive = true };
        await service.SaveAsync(null, input); Assert.Equal("/makaleler/current", await service.ResolveAsync("/OLD"));
        await Assert.ThrowsAsync<ConflictException>(() => service.SaveAsync(null, input));
        await service.DisableAsync(1); Assert.Null(await service.ResolveAsync("/old")); await service.SaveAsync(1, input);
        routes.Paths.Add("/old"); Assert.Null(await service.ResolveAsync("/old"));
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(1, input)); routes.Paths.Remove("/old");
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(null, new RedirectInput { SourcePath = "/next", DestinationPath = "/old", IsActive = true }));
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(null, new RedirectInput { SourcePath = "/loop", DestinationPath = "/loop" }));
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(1, new RedirectInput { SourcePath = "/rename", DestinationPath = "/makaleler/current" }));
        routes.Paths.Clear(); Assert.Null(await service.ResolveAsync("/old"));
    }
    [Theory]
    [InlineData("<img src=x>")]
    [InlineData("&lt;script&gt;")]
    [InlineData("Test\nheader")]
    [InlineData("CONTENT_REQUIRED")]
    [InlineData("\tTest")]
    public void Metadata_RejectsMarkupPlaceholdersAndControlCharacters(string value)
    {
        Assert.False(new SeoValidator().Validate(new SeoInput { MetaTitle = value }).IsValid);
        Assert.False(new SeoValidator().Validate(new SeoInput { OpenGraphDescription = value }).IsValid);
    }
    [Fact]
    public void SeoRecommendations_AreInformational_AndBlankMetadataRemainsValid()
    {
        Assert.True(new SeoValidator().Validate(new SeoInput()).IsValid);
        var issues = SeoQuality.Check(new SeoInput(), "Test");
        Assert.Contains(issues, x => x.Code == "description-missing"); Assert.Contains(issues, x => x.Code == "title-short");
        var input = new SeoInput { MetaTitle = new string('A', 65), MetaDescription = new string('A', 180) };
        Assert.True(new SeoValidator().Validate(input).IsValid);
        Assert.Contains(SeoQuality.Check(input), x => x.Code == "title-long");
        Assert.Contains(SeoQuality.Check(input), x => x.Code == "description-length");
    }
    [Fact]
    public async Task PracticeAreaSeo_ValidatesMediaAndPreservesStableSlug()
    {
        var areas = new FakeRepository<PracticeArea>(); var media = new FakeRepository<MediaAsset>();
        var service = new PracticeAreaService(areas, new SlugService(), new CreatePracticeAreaValidator(), media);
        var input = new CreatePracticeAreaDto { Name = "Test area", Content = "Test", Seo = new SeoInput { MetaTitle = "Test SEO", OpenGraphMediaId = 1 } };
        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(input)); Assert.Empty(areas.Items);
        media.Seed(new MediaAsset { Id = 1, IsActive = true }); var row = await service.CreateAsync(input);
        var updated = await service.UpdateAsync(row.Id, new CreatePracticeAreaDto { Name = "Test renamed", Content = "Test", Seo = input.Seo });
        Assert.Equal(row.Slug, updated.Slug); Assert.Equal("Test SEO", updated.Seo.MetaTitle);
    }
    private sealed class TestRoutes : IPublicRouteService
    {
        public HashSet<string> Paths { get; } = new(StringComparer.Ordinal);
        public Task<bool> ExistsAsync(string path, CancellationToken ct = default) => Task.FromResult(Paths.Contains(path));
    }
    [Fact]
    public async Task ReferencedAreaOpenGraphMedia_CannotBeDeletedEvenForDeletedArea()
    {
        var media = new FakeRepository<MediaAsset>(); media.Seed(new MediaAsset { Id = 1, IsActive = true });
        var areas = new FakeRepository<PracticeArea>(); areas.Seed(new PracticeArea { Id = 1, OpenGraphMediaId = 1, IsDeleted = true });
        var service = new MediaService(media, new FakeRepository<Article>(), new FakeRepository<PageSeo>(), new TestImageStorage(), areas);
        await Assert.ThrowsAsync<ConflictException>(() => service.DeleteAsync(1)); Assert.False(media.Items[0].IsDeleted);
    }
}
