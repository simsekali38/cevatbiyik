using System.Linq.Expressions;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace CevatBiyik.Infrastructure.Persistence;

public sealed class EfRepository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    public Task<T?> FindAsync(long id, CancellationToken cancellationToken = default) =>
        WithRelations(context.Set<T>()).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    private static IQueryable<T> WithRelations(IQueryable<T> query)
    {
        if (typeof(T) == typeof(Article))
        {
            return query.Include("Category").Include("CoverMedia").Include("OpenGraphMedia").Include("TagLinks.Tag").AsSplitQuery();
        }

        if (typeof(T) == typeof(PageSeo))
        {
            return query.Include("OpenGraphMedia");
        }

        return query;
    }

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int page, int pageSize,
        Expression<Func<T, object>>? orderBy, bool descending, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);
        var offset = checked((page - 1) * pageSize);
        var query = WithRelations(context.Set<T>().AsNoTracking()).Where(predicate);
        var ordered = orderBy is null ? query.OrderByDescending(x => x.Id)
            : descending ? query.OrderByDescending(orderBy).ThenByDescending(x => x.Id)
            : query.OrderBy(orderBy).ThenBy(x => x.Id);
        return await ordered.Skip(offset).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        context.Set<T>().LongCountAsync(predicate, cancellationToken);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Set<T>().AsNoTracking();
        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.AnyAsync(predicate, cancellationToken);
    }

    public void Add(T entity) => context.Set<T>().Add(entity);

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is MySqlException { Number: 1062 })
        {
            // Keep provider details, SQL and contact data out of application errors.
            throw new ConflictException("Benzersiz alan başka bir kayıt tarafından kullanılıyor; işlemi yeniden deneyin.");
        }
    }
}
