using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;
public sealed class ProfileService(IRepository<LawyerProfile> profiles) : IProfileService
{
    private ProfileDto? publicSnapshot;
    private bool publicSnapshotLoaded;
    public async Task<ProfileDto?> GetAsync(bool publicOnly = false, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (publicOnly && publicSnapshotLoaded) { return publicSnapshot; }
        var rows = await profiles.ListAsync(x => x.Key == "primary" && (!publicOnly || (x.IsActive && x.IsReviewed)), 1, 1, ct);
        var result = rows.Count == 0 || (publicOnly && !ContentPublication.Complete(rows[0].Biography, rows[0].Vision, rows[0].BarAssociation)) ? null : Map(rows[0]);
        if (publicOnly) { publicSnapshot = result; publicSnapshotLoaded = true; }
        return result;
    }
    public async Task SaveAsync(ProfileInput input, CancellationToken ct = default)
    {
        await new ProfileValidator().ValidateAndThrowAsync(input, ct);
        var rows = await profiles.ListAsync(x => x.Key == "primary", 1, 1, ct);
        var item = rows.Count == 0 ? new LawyerProfile() : await ServiceRules.RequireAsync(profiles, rows[0].Id, ct);
        item.Biography = RichText.Normalize(input.Biography); item.Vision = RichText.Normalize(input.Vision); item.BarAssociation = input.BarAssociation.Trim();
        item.IsActive = input.IsActive; item.IsReviewed = input.IsReviewed;
        if (rows.Count == 0)
        {
            profiles.Add(item);
        }

        await profiles.SaveAsync(ct);
        publicSnapshotLoaded = false;
        publicSnapshot = null;
    }
    private static ProfileDto Map(LawyerProfile x) => new(x.Id, x.Biography, x.Vision, x.BarAssociation, x.IsActive, x.IsReviewed);
}
