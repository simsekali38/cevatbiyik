using System.Linq.Expressions;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.Application.Interfaces;

/// <summary>Bounded reads exclude deleted records; uniqueness checks may include them.</summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> FindAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default) =>
        ListAsync(predicate, page, pageSize, null, true, cancellationToken);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int page, int pageSize,
        Expression<Func<T, object>>? orderBy, bool descending, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, CancellationToken cancellationToken = default);
    void Add(T entity);
    Task SaveAsync(CancellationToken cancellationToken = default);
}
