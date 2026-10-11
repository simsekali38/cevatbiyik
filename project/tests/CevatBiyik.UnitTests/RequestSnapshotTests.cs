using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.UnitTests;

public sealed class RequestSnapshotTests
{
    [Fact]
    public async Task ReviewedPages_UseOneReadForConsentLayoutAndContent_InvalidateOnWrite()
    {
        var repository = new FakeRepository<PageContent>();
        var service = new PageContentService(repository);
        var input = new PageContentInput { Path = "/kvkk", Title = "TEST_ONLY privacy", Body = "TEST_ONLY reviewed policy", IsActive = true, IsReviewed = true };
        await service.SaveAsync(null, input);
        Assert.NotNull(await service.PublishedAsync("/kvkk"));
        Assert.Null(await service.PublishedAsync("/cerez-politikasi"));
        Assert.Single(await service.ListAsync(true));
        Assert.Equal(1, repository.ListCount);
        await service.DisableAsync(1);
        Assert.Null(await service.PublishedAsync("/kvkk"));
        Assert.Equal(2, repository.ListCount);
        await service.SaveAsync(1, input);
        Assert.NotNull(await service.PublishedAsync("/kvkk"));
        Assert.Equal(3, repository.ListCount);
        // Another request receives a new scoped instance, not the old reviewed snapshot.
        await new PageContentService(repository).PublishedAsync("/kvkk");
        Assert.Equal(4, repository.ListCount);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.ListAsync(true, new CancellationToken(true)));
    }

    [Fact]
    public async Task PublicSettings_ReuseAllowlistedSnapshot_InvalidateAfterUpdateAndDelete()
    {
        var repository = new FakeRepository<SiteSetting>();
        var service = new SettingService(repository, new UpdateSiteSettingValidator());
        var created = await service.CreateAsync(new UpdateSiteSettingDto { Key = "Contact.Email", Value = "test@example.test", IsActive = true });
        repository.Seed(new SiteSetting { Id = 2, Key = "Smtp.Password", Value = "TEST_ONLY private", IsActive = true });
        Assert.Single(await service.ListPublicAsync());
        Assert.Single(await service.ListPublicAsync());
        Assert.Equal(1, repository.ListCount);
        await service.UpdateAsync(created.Id, new UpdateSiteSettingDto { Key = "Contact.Email", Value = "changed@example.test", IsActive = true });
        Assert.Equal("changed@example.test", Assert.Single(await service.ListPublicAsync()).Value);
        await service.DeleteAsync(created.Id);
        Assert.Empty(await service.ListPublicAsync());
        Assert.Equal(3, repository.ListCount);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.ListPublicAsync(new CancellationToken(true)));
    }

    [Fact]
    public async Task Profile_CachesMissingResult_ThenPublishesFreshReviewedData()
    {
        var repository = new FakeRepository<LawyerProfile>();
        var service = new ProfileService(repository);
        Assert.Null(await service.GetAsync(true)); Assert.Null(await service.GetAsync(true));
        Assert.Equal(1, repository.ListCount);
        await service.SaveAsync(new ProfileInput { Biography = "TEST_ONLY biography", Vision = "TEST_ONLY vision", BarAssociation = "TEST_ONLY bar", IsActive = true, IsReviewed = true });
        Assert.NotNull(await service.GetAsync(true)); Assert.NotNull(await service.GetAsync(true));
        Assert.Equal(3, repository.ListCount); // One locating read in Save plus one new public read.
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetAsync(true, new CancellationToken(true)));
    }
}
