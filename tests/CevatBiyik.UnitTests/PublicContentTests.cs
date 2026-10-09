using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.UnitTests;
public sealed class PublicContentTests
{
    [Fact]
    public async Task Profile_PublishRequiresReviewedCompleteContent_AndKeepsSingleRecord()
    {
        var repository = new FakeRepository<LawyerProfile>(); var service = new ProfileService(repository);
        await service.SaveAsync(new ProfileInput());
        Assert.Null(await service.GetAsync(true));
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(new ProfileInput { IsActive = true, IsReviewed = true }));
        var input = new ProfileInput { Biography = "TEST_ONLY biography", Vision = "TEST_ONLY vision", BarAssociation = "TEST_ONLY bar", IsActive = true };
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(input));
        input.IsReviewed = true; await service.SaveAsync(input);
        Assert.NotNull(await service.GetAsync(true)); Assert.Single(repository.Items);
        input.IsActive = false; input.IsReviewed = false; await service.SaveAsync(input);
        Assert.Null(await service.GetAsync(true)); Assert.Single(repository.Items);
    }
    [Fact]
    public async Task Pages_RejectUnsupportedPathsAndRenaming_AndCanBeDisabledAndRepublished()
    {
        var repository = new FakeRepository<PageContent>(); var service = new PageContentService(repository);
        var input = new PageContentInput { Path = "/admin/login", Title = "Test", Body = "Test" };
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(null, input));
        input.Path = "/kvkk"; input.IsActive = true; input.IsReviewed = true; await service.SaveAsync(null, input);
        await Assert.ThrowsAsync<ConflictException>(() => service.SaveAsync(null, input));
        await service.DisableAsync(1); Assert.Null(await service.PublishedAsync("/kvkk"));
        await service.SaveAsync(1, input); Assert.NotNull(await service.PublishedAsync("/kvkk"));
        input.Path = "/yasal-uyari";
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(1, input));
        Assert.Equal("/kvkk", repository.Items[0].Path);
    }
    [Fact]
    public async Task Faq_FiltersBeforePagination_SortsAndSoftDeletes()
    {
        var repository = new FakeRepository<FaqItem>(); var service = new FaqService(repository);
        for (var i = 1; i <= 15; i++)
        {
            await service.SaveAsync(null, new FaqInput { Question = "Test " + i, Answer = "Test answer", SortOrder = 16 - i, IsActive = true, IsReviewed = true });
        }

        await service.SaveAsync(null, new FaqInput());
        var first = await service.ListAsync(publicOnly: true); var second = await service.ListAsync(2, true);
        Assert.Equal(12, first.Items.Count); Assert.Equal(3, second.Items.Count); Assert.Equal(15, first.TotalCount);
        Assert.Equal("Test 15", first.Items[0].Question);
        await service.DeleteAsync(first.Items[0].Id);
        Assert.True(repository.Items.Single(x => x.Id == first.Items[0].Id).IsDeleted);
        Assert.Equal(14, (await service.ListAsync(publicOnly: true)).TotalCount);
    }
    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("&lt;img src=x onerror=alert(1)&gt;")]
    public async Task NewContent_RejectsHtmlBeforeMutation(string content)
    {
        var profile = new ProfileInput { Biography = content, Vision = "Test", BarAssociation = "Test" };
        var page = new PageContentInput { Body = content, Title = "Test" };
        var faq = new FaqInput { Question = "Test", Answer = content };
        Assert.False((await new ProfileValidator().ValidateAsync(profile)).IsValid);
        Assert.False((await new PageContentValidator().ValidateAsync(page)).IsValid);
        Assert.False((await new FaqValidator().ValidateAsync(faq)).IsValid);
    }
    [Fact]
    public async Task LegacyPlaceholderRows_NeverBecomePublicEvenWithReviewedFlag()
    {
        var profiles = new FakeRepository<LawyerProfile>(); profiles.Seed(new LawyerProfile { Id = 1, Biography = "content_required", Vision = "Test", BarAssociation = "Test", IsActive = true, IsReviewed = true });
        Assert.Null(await new ProfileService(profiles).GetAsync(true));
        var pages = new FakeRepository<PageContent>(); pages.Seed(new PageContent { Id = 1, Path = "/kvkk", Title = "Test", Body = "content_required", IsActive = true, IsReviewed = true });
        Assert.Null(await new PageContentService(pages).PublishedAsync("/kvkk"));
    }
}
