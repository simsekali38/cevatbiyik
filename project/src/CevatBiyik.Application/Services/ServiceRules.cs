using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.Application.Services;

internal static class ServiceRules
{
    // MySQL datetime stores no timezone; Phase 02 writes UTC. Restore that contract for DTO readers.
    public static DateTime AsUtc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
    public static DateTime? AsUtc(DateTime? value) => value.HasValue ? AsUtc(value.Value) : null;

    public static void ValidatePage(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);
        if ((long)(page - 1) * pageSize > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(page));
        }
    }

    public static async Task<T> RequireAsync<T>(IRepository<T> repository, long id, CancellationToken cancellationToken)
        where T : BaseEntity => await repository.FindAsync(id, cancellationToken)
            ?? throw new NotFoundException("Kayıt bulunamadı.");

    public static async Task DeleteAsync<T>(IRepository<T> repository, long id, CancellationToken cancellationToken)
        where T : BaseEntity
    {
        var entity = await RequireAsync(repository, id, cancellationToken);
        entity.IsDeleted = true;
        entity.IsActive = false;
        await repository.SaveAsync(cancellationToken);
    }
}
