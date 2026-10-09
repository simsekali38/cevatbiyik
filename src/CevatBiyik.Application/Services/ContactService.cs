using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using FluentValidation;

namespace CevatBiyik.Application.Services;

public sealed class ContactService(IRepository<ContactMessage> messages, IValidator<CreateContactMessageDto> validator,
    TimeProvider timeProvider) : IContactService
{
    public async Task<ContactMessageDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await messages.FindAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<ContactMessageDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, pageSize);
        return (await messages.ListAsync(x => true, page, pageSize, cancellationToken)).Select(Map).ToArray();
    }

    public async Task<ContactMessageDto> CreateAsync(CreateContactMessageDto input, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        var entity = new ContactMessage
        {
            Name = input.Name.Trim(), Email = input.Email.Trim(), Phone = input.Phone?.Trim(),
            Subject = input.Subject?.Trim(), Message = input.Message.Trim(), Consent = input.Consent
        };
        messages.Add(entity);
        await messages.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<ContactMessageDto> MarkReadAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await ServiceRules.RequireAsync(messages, id, cancellationToken);
        if (!entity.IsRead)
        {
            entity.IsRead = true;
            entity.ProcessedAt = timeProvider.GetUtcNow().UtcDateTime;
            await messages.SaveAsync(cancellationToken);
        }

        return Map(entity);
    }

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => ServiceRules.DeleteAsync(messages, id, cancellationToken);

    private static ContactMessageDto Map(ContactMessage x) => new(x.Id, x.Name, x.Email, x.Phone, x.Subject,
        x.Message, x.Consent, x.IsRead, ServiceRules.AsUtc(x.CreatedAt), ServiceRules.AsUtc(x.ProcessedAt));
}
