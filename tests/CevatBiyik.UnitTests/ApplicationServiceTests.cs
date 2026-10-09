using System.Linq.Expressions;
using CevatBiyik.Application;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.UnitTests;

public class ApplicationServiceTests
{
    private static readonly DateTime Now = new(2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData("İş ve İşçi Hukuku", "is-ve-isci-hukuku")]
    [InlineData("ÇĞİÖŞÜ ıöçğşü", "cgiosu-iocgsu")]
    [InlineData("  Aile---Hukuku!  ", "aile-hukuku")]
    [InlineData("Dava 2026 / 123", "dava-2026-123")]
    public void Slug_TransliteratesTurkishAndSeparators(string title, string expected)
    {
        Assert.Equal(expected, new SlugService().Generate(title));
    }

    [Fact]
    public async Task Slug_CollisionSuffixStaysWithinDatabaseLimit()
    {
        var calls = new List<string>();
        var slug = await new SlugService().GenerateUniqueAsync(new string('a', 250), (candidate, _) =>
        {
            calls.Add(candidate);
            return Task.FromResult(calls.Count < 3);
        });
        Assert.Equal(200, slug.Length);
        Assert.EndsWith("-3", slug, StringComparison.Ordinal);
        Assert.All(calls, x => Assert.True(x.Length <= 200));
    }

    [Fact]
    public async Task Slug_CancelledOperationDoesNotQueryRepository()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new SlugService().GenerateUniqueAsync("Başlık",
            (_, _) => throw new InvalidOperationException("Must not query."), source.Token));
    }

    [Fact]
    public async Task Article_CreateReservesDeletedSlugAndUpdatePreservesUrl()
    {
        var repository = new FakeRepository<Article>();
        repository.Seed(new Article { Id = 1, Slug = "is-hukuku", IsDeleted = true });
        var service = Articles(repository);
        var created = await service.CreateAsync(new CreateArticleDto { Title = "İş Hukuku", Content = "Taslak metin" });
        Assert.Equal("is-hukuku-2", created.Slug);
        Assert.False(created.IsActive);
        var updated = await service.UpdateAsync(created.Id, new UpdateArticleDto { Title = "Yeni Başlık", Content = "Yeni metin" });
        Assert.Equal(created.Slug, updated.Slug);
        Assert.Equal("Yeni Başlık", updated.Title);
    }

    [Fact]
    public async Task Article_SoftDeleteHidesRecordAndDisablesIt()
    {
        var repository = new FakeRepository<Article>();
        repository.Seed(new Article { Id = 1, IsActive = true });
        var service = Articles(repository);
        await service.DeleteAsync(1);
        Assert.Null(await service.GetAsync(1));
        Assert.Empty(await service.ListAsync());
        Assert.True(repository.Items.Single().IsDeleted);
        Assert.False(repository.Items.Single().IsActive);
        Assert.Single(repository.Items);
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(1));
    }

    [Fact]
    public async Task Article_PublicListExcludesFutureDraftDeletedAndInactiveArea()
    {
        var repository = new FakeRepository<Article>();
        repository.Seed(new Article { Id = 1, IsActive = true, PublishedAt = Now });
        repository.Seed(new Article { Id = 2, IsActive = false, PublishedAt = Now });
        repository.Seed(new Article { Id = 3, IsActive = true, PublishedAt = Now.AddSeconds(1) });
        repository.Seed(new Article { Id = 4, IsActive = true, PublishedAt = Now, IsDeleted = true });
        repository.Seed(new Article
        {
            Id = 5,
            IsActive = true,
            PublishedAt = Now,
            PracticeAreaId = 1,
            PracticeArea = new PracticeArea { Id = 1, IsActive = false }
        });
        repository.Seed(new Article
        {
            Id = 6,
            IsActive = true,
            PublishedAt = Now,
            PracticeAreaId = 2,
            PracticeArea = new PracticeArea { Id = 2, IsActive = true, IsDeleted = true }
        });
        repository.Seed(new Article { Id = 7, IsActive = true });
        Assert.Equal(1, Assert.Single(await Articles(repository).ListPublishedAsync()).Id);
        Assert.Equal(6, (await Articles(repository).ListAsync()).Count);
    }

    [Fact]
    public async Task Article_MissingOrDeletedAreaDoesNotSave()
    {
        var articles = new FakeRepository<Article>();
        var areas = new FakeRepository<PracticeArea>();
        areas.Seed(new PracticeArea { Id = 3, IsDeleted = true });
        await Assert.ThrowsAsync<NotFoundException>(() => Articles(articles, areas).CreateAsync(new CreateArticleDto
        {
            Title = "Başlık",
            Content = "Metin",
            PracticeAreaId = 3
        }));
        Assert.Empty(articles.Items);
        Assert.Equal(0, articles.SaveCount);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [InlineData(int.MaxValue, 100)]
    public async Task List_InvalidPaginationFailsBeforeQuery(int page, int pageSize)
    {
        var repository = new FakeRepository<Article>();
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Articles(repository).ListAsync(page, pageSize));
        Assert.Equal(0, repository.ListCount);
    }

    [Fact]
    public async Task List_UsesBoundedStablePages()
    {
        var repository = new FakeRepository<Article>();
        for (var id = 1; id <= 5; id++)
        {
            repository.Seed(new Article { Id = id });
        }

        Assert.Equal(new long[] { 3, 2 }, (await Articles(repository).ListAsync(2, 2)).Select(x => x.Id));
    }

    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("&lt;script&gt;alert(1)&lt;/script&gt;")]
    [InlineData("&amp;lt;img src=x onerror=alert(1)&amp;gt;")]
    [InlineData("Merhaba\0")]
    public async Task Contact_RejectsMarkupAndEncodedMarkupBeforeSave(string message)
    {
        var repository = new FakeRepository<ContactMessage>();
        var service = Contacts(repository);
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateContactMessageDto
        {
            Name = "Test",
            Email = "test@example.com",
            Message = message,
            Consent = true
        }));
        Assert.Empty(repository.Items);
        Assert.Equal(0, repository.SaveCount);
    }

    [Theory]
    [InlineData("test@example.com", false)]
    [InlineData("invalid", true)]
    [InlineData("test\r\n@example.com", true)]
    [InlineData("", true)]
    public async Task Contact_RequiresConsentAndValidEmail(string email, bool consent)
    {
        var repository = new FakeRepository<ContactMessage>();
        await Assert.ThrowsAsync<ValidationException>(() => Contacts(repository).CreateAsync(new CreateContactMessageDto
        {
            Name = "Test",
            Email = email,
            Message = "Mesaj",
            Consent = consent
        }));
        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task Contact_CreateReadAndDeletePreserveSubmittedMessage()
    {
        var repository = new FakeRepository<ContactMessage>();
        var service = Contacts(repository);
        var created = await service.CreateAsync(new CreateContactMessageDto
        {
            Name = " Test ",
            Email = "test@example.com",
            Message = "Merhaba & teşekkürler",
            Consent = true
        });
        Assert.Equal("Test", created.Name);
        Assert.False(created.IsRead);
        var read = await service.MarkReadAsync(created.Id);
        Assert.Equal(Now, read.ProcessedAt);
        Assert.True(read.IsRead);
        Assert.Equal(created.Message, read.Message);
        await service.MarkReadAsync(created.Id);
        Assert.Equal(2, repository.SaveCount);
        await service.DeleteAsync(created.Id);
        Assert.Null(await service.GetAsync(created.Id));
    }

    [Fact]
    public async Task PracticeArea_CreateUpdateAndDeletePreserveSlug()
    {
        var repository = new FakeRepository<PracticeArea>();
        var service = new PracticeAreaService(repository, new SlugService(), new CreatePracticeAreaValidator());
        var created = await service.CreateAsync(new CreatePracticeAreaDto { Name = "Çalışma Alanı", Content = "Taslak" });
        var updated = await service.UpdateAsync(created.Id, new CreatePracticeAreaDto { Name = "Yeni Alan", Content = "Taslak", SortOrder = 2 });
        Assert.Equal("calisma-alani", updated.Slug);
        Assert.Equal(2, updated.SortOrder);
        await service.DeleteAsync(created.Id);
        Assert.Null(await service.GetAsync(created.Id));
    }

    [Fact]
    public async Task Settings_RejectSecretsAndReserveSoftDeletedKeys()
    {
        var repository = new FakeRepository<SiteSetting>();
        var service = new SettingService(repository, new UpdateSiteSettingValidator());
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new UpdateSiteSettingDto
        {
            Key = "Smtp.Password",
            Value = "value"
        }));
        var created = await service.CreateAsync(new UpdateSiteSettingDto { Key = "Site.Name", Value = "CONTENT_REQUIRED" });
        var updated = await service.UpdateAsync(created.Id, new UpdateSiteSettingDto { Key = "Site.Name", Value = "Taslak" });
        Assert.Equal("Taslak", updated.Value);
        await service.DeleteAsync(created.Id);
        Assert.Null(await service.GetAsync(created.Id));
        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(new UpdateSiteSettingDto { Key = "Site.Name", Value = "Taslak" }));
    }

    [Fact]
    public async Task Settings_CannotRenameIntoAnotherExistingKey()
    {
        var repository = new FakeRepository<SiteSetting>();
        var service = new SettingService(repository, new UpdateSiteSettingValidator());
        var first = await service.CreateAsync(new UpdateSiteSettingDto { Key = "Site.Name", Value = "Taslak" });
        await service.CreateAsync(new UpdateSiteSettingDto { Key = "Site.Description", Value = "Taslak" });
        await Assert.ThrowsAsync<ConflictException>(() => service.UpdateAsync(first.Id,
            new UpdateSiteSettingDto { Key = "Site.Description", Value = "Taslak" }));
        Assert.Equal("Site.Name", (await service.GetAsync(first.Id))!.Key);
    }

    [Fact]
    public async Task Validators_PreventPublishingPlaceholdersAndNonUtcDates()
    {
        var validator = new CreateArticleValidator();
        Assert.False((await validator.ValidateAsync(new CreateArticleDto
        {
            Title = "CONTENT_REQUIRED",
            Content = "Taslak",
            IsActive = true,
            PublishedAt = Now
        })).IsValid);
        Assert.False((await validator.ValidateAsync(new CreateArticleDto
        {
            Title = "Başlık",
            Content = "Taslak",
            PublishedAt = DateTime.SpecifyKind(Now, DateTimeKind.Unspecified)
        })).IsValid);
        Assert.False((await validator.ValidateAsync(new CreateArticleDto
        {
            Title = "Başlık",
            Content = "Taslak",
            IsActive = true
        })).IsValid);
        Assert.False((await new CreatePracticeAreaValidator().ValidateAsync(new CreatePracticeAreaDto
        {
            Name = "Alan",
            Content = "CONTENT_REQUIRED",
            IsActive = true
        })).IsValid);
        Assert.False((await new UpdateSiteSettingValidator().ValidateAsync(new UpdateSiteSettingDto
        {
            Key = "Site.Name",
            Value = "CONTENT_REQUIRED",
            IsActive = true
        })).IsValid);
    }

    [Fact]
    public async Task Validators_EnforceStorageLimitsAndRequiredFields()
    {
        var validator = new CreateContactMessageValidator();
        Assert.False((await validator.ValidateAsync(new CreateContactMessageDto
        {
            Name = "Test",
            Email = "test@example.com",
            Message = new string('a', 5001),
            Consent = true
        })).IsValid);
        Assert.False((await validator.ValidateAsync(new CreateContactMessageDto
        {
            Name = "  ",
            Email = "test@example.com",
            Message = "Mesaj",
            Consent = true
        })).IsValid);
        Assert.False((await new CreateArticleValidator().ValidateAsync(new CreateArticleDto
        {
            Title = new string('a', 251),
            Content = "Metin"
        })).IsValid);
    }

    [Fact]
    public void ApplicationDi_ResolvesAllServicesAndValidatorsInScope()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(IRepository<>), typeof(FakeRepository<>));
        services.AddApplication();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        using var scope = provider.CreateScope();
        Assert.IsType<ArticleService>(scope.ServiceProvider.GetRequiredService<IArticleService>());
        Assert.IsType<PracticeAreaService>(scope.ServiceProvider.GetRequiredService<IPracticeAreaService>());
        Assert.IsType<ContactService>(scope.ServiceProvider.GetRequiredService<IContactService>());
        Assert.IsType<SettingService>(scope.ServiceProvider.GetRequiredService<ISettingService>());
    }

    [Fact]
    public async Task Article_InvalidUpdateDoesNotMutateStoredRecord()
    {
        var repository = new FakeRepository<Article>();
        repository.Seed(new Article { Id = 1, Title = "Önceki", Content = "Metin", Slug = "onceki" });
        await Assert.ThrowsAsync<ValidationException>(() => Articles(repository).UpdateAsync(1,
            new UpdateArticleDto { Title = "Yeni", Content = "<script>alert(1)</script>" }));
        Assert.Equal("Önceki", repository.Items.Single().Title);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task AllServices_ReportMissingRecordsWithoutSaving()
    {
        var areas = new FakeRepository<PracticeArea>();
        var contacts = new FakeRepository<ContactMessage>();
        var settings = new FakeRepository<SiteSetting>();
        await Assert.ThrowsAsync<NotFoundException>(() => Articles(new FakeRepository<Article>()).UpdateAsync(42,
            new UpdateArticleDto { Title = "Başlık", Content = "Metin" }));
        await Assert.ThrowsAsync<NotFoundException>(() => new PracticeAreaService(areas, new SlugService(), new CreatePracticeAreaValidator())
            .UpdateAsync(42, new CreatePracticeAreaDto { Name = "Alan", Content = "Metin" }));
        await Assert.ThrowsAsync<NotFoundException>(() => Contacts(contacts).MarkReadAsync(42));
        await Assert.ThrowsAsync<NotFoundException>(() => new SettingService(settings, new UpdateSiteSettingValidator())
            .UpdateAsync(42, new UpdateSiteSettingDto { Key = "Site.Name", Value = "Taslak" }));
        Assert.Equal(0, areas.SaveCount + contacts.SaveCount + settings.SaveCount);
    }

    [Fact]
    public async Task Settings_AllowUnknownEmailOnlyAsInactivePlaceholder()
    {
        var validator = new UpdateSiteSettingValidator();
        Assert.True((await validator.ValidateAsync(new UpdateSiteSettingDto { Key = "Contact.Email", Value = "CONTENT_REQUIRED" })).IsValid);
        Assert.False((await validator.ValidateAsync(new UpdateSiteSettingDto { Key = "Contact.Email", Value = "CONTENT_REQUIRED", IsActive = true })).IsValid);
        Assert.False((await validator.ValidateAsync(new UpdateSiteSettingDto { Key = "Contact.Email", Value = "x\r\n@example.com" })).IsValid);
    }

    [Fact]
    public async Task Slug_RejectsEmptyUrlsAndStopsAfterBoundedCollisionChecks()
    {
        var slugs = new SlugService();
        Assert.Throws<ArgumentException>(() => slugs.Generate("!!!"));
        var checks = 0;
        await Assert.ThrowsAsync<ConflictException>(() => slugs.GenerateUniqueAsync("Başlık", (_, _) =>
        {
            checks++;
            return Task.FromResult(true);
        }));
        Assert.Equal(1000, checks);
    }

    [Fact]
    public async Task ReadDtos_RestoreUtcKindForMySqlDateTimes()
    {
        var repository = new FakeRepository<Article>();
        var mysqlTime = DateTime.SpecifyKind(Now, DateTimeKind.Unspecified);
        repository.Seed(new Article { Id = 1, PublishedAt = mysqlTime, CreatedAt = mysqlTime, UpdatedAt = mysqlTime });
        var article = (await Articles(repository).GetAsync(1))!;
        Assert.Equal(DateTimeKind.Utc, article.PublishedAt!.Value.Kind);
        Assert.Equal(DateTimeKind.Utc, article.CreatedAt.Kind);
        Assert.Equal(DateTimeKind.Utc, article.UpdatedAt!.Value.Kind);
        Assert.Equal(Now.Ticks, article.PublishedAt.Value.Ticks);
        var contacts = new FakeRepository<ContactMessage>();
        contacts.Seed(new ContactMessage { Id = 1, CreatedAt = mysqlTime, ProcessedAt = mysqlTime });
        var contact = (await Contacts(contacts).GetAsync(1))!;
        Assert.Equal(DateTimeKind.Utc, contact.ProcessedAt!.Value.Kind);
    }

    private static ArticleService Articles(FakeRepository<Article> articles, FakeRepository<PracticeArea>? areas = null) =>
        new(articles, areas ?? new FakeRepository<PracticeArea>(), new SlugService(), new CreateArticleValidator(), new UpdateArticleValidator(), new FixedTimeProvider(Now));

    private static ContactService Contacts(FakeRepository<ContactMessage> repository) =>
        new(repository, new CreateContactMessageValidator(), new FixedTimeProvider(Now));
}

public sealed class FixedTimeProvider(DateTime now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new(now);
}

public sealed class FakeRepository<T> : IRepository<T> where T : BaseEntity
{
    public List<T> Items { get; } = [];
    public int SaveCount { get; private set; }
    public int ListCount { get; private set; }
    public void Seed(T entity) => Items.Add(entity);
    public Task<T?> FindAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Items.SingleOrDefault(x => x.Id == id && !x.IsDeleted));
    }

    public Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int page, int pageSize,
        Expression<Func<T, object>>? orderBy, bool descending, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ListCount++;
        var query = Items.Where(x => !x.IsDeleted).Where(predicate.Compile());
        var ordered = orderBy is null ? query.OrderByDescending(x => x.Id)
            : descending ? query.OrderByDescending(orderBy.Compile()).ThenByDescending(x => x.Id)
            : query.OrderBy(orderBy.Compile()).ThenBy(x => x.Id);
        return Task.FromResult<IReadOnlyList<T>>(ordered.Skip((page - 1) * pageSize).Take(pageSize).ToArray());
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Items.Where(x => !x.IsDeleted).LongCount(predicate.Compile()));
    }

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Items.Where(x => includeDeleted || !x.IsDeleted).Any(predicate.Compile()));
    }

    public void Add(T entity)
    {
        entity.Id = Items.Count == 0 ? 1 : Items.Max(x => x.Id) + 1;
        Items.Add(entity);
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SaveCount++;
        return Task.CompletedTask;
    }
}
