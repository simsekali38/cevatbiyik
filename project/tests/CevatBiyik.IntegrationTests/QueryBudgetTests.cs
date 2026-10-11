using System.Data.Common;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CevatBiyik.IntegrationTests;

public sealed class QueryBudgetTests
{
    [Fact]
    public async Task ArticleRelations_UseFixedSplitQueryBudget_AndNeverTrackPublicReads()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var counter = new ReadCounter();
        await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).AddInterceptors(counter).Options);
        await db.Database.EnsureCreatedAsync();
        var category = new ArticleCategory { Name = "TEST_ONLY category", Slug = "test-category" };
        var tag = new ArticleTag { Name = "TEST_ONLY tag", Slug = "test-tag" };
        for (var i = 0; i < 10; i++)
        {
            db.Articles.Add(new Article { Title = "TEST_ONLY " + i, Slug = "test-" + i, Content = "TEST_ONLY body", Category = category,
                TagLinks = [new ArticleTagLink { Tag = tag }], IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) });
        }
        await db.SaveChangesAsync(); db.ChangeTracker.Clear();
        var repository = new EfRepository<Article>(db);
        counter.Count = 0;
        var single = await repository.ListAsync(x => x.IsActive, 1, 1, null, false);
        var singleBudget = counter.Count;
        Assert.Equal(2, singleBudget); Assert.Single(single[0].TagLinks);
        counter.Count = 0;
        var ten = await repository.ListAsync(x => x.IsActive, 1, 10, null, false);
        Assert.Equal(10, ten.Count);
        Assert.All(ten, item => { Assert.NotNull(item.Category); Assert.Single(item.TagLinks); });
        Assert.Equal(singleBudget, counter.Count);
        Assert.Empty(db.ChangeTracker.Entries());
    }

    private sealed class ReadCounter : DbCommandInterceptor
    {
        public int Count { get; set; }
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Count++;
            return ValueTask.FromResult(result);
        }
    }
}
