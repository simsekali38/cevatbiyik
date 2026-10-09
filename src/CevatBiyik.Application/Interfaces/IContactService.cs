using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Application.Interfaces;

public interface IContactService
{
    Task<ContactMessageDto?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactMessageDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ContactMessageDto> CreateAsync(CreateContactMessageDto input, CancellationToken cancellationToken = default);
    Task<ContactMessageDto> MarkReadAsync(long id, CancellationToken cancellationToken = default);
    Task<ContactMessageDto> SetReadAsync(long id, bool isRead, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
