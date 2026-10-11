using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CevatBiyik.Infrastructure.Persistence.Configurations;
public sealed class ProfileConfiguration : IEntityTypeConfiguration<LawyerProfile>
{
    public void Configure(EntityTypeBuilder<LawyerProfile> builder)
    {
        builder.ConfigureBase(); builder.ToTable("LawyerProfiles");
        builder.Property(x => x.Key).HasMaxLength(20).IsRequired(); builder.HasIndex(x => x.Key).IsUnique();
        builder.Property(x => x.Biography).HasMaxLength(5000).IsRequired(); builder.Property(x => x.Vision).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.BarAssociation).HasMaxLength(150).IsRequired();
    }
}
public sealed class PageContentConfiguration : IEntityTypeConfiguration<PageContent>
{
    public void Configure(EntityTypeBuilder<PageContent> builder)
    {
        builder.ConfigureBase(); builder.ToTable("PageContents");
        builder.Property(x => x.Path).HasMaxLength(100).IsRequired(); builder.HasIndex(x => x.Path).IsUnique();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired(); builder.Property(x => x.Body).HasMaxLength(12000).IsRequired();
    }
}
public sealed class FaqConfiguration : IEntityTypeConfiguration<FaqItem>
{
    public void Configure(EntityTypeBuilder<FaqItem> builder)
    {
        builder.ConfigureBase(); builder.ToTable("FaqItems");
        builder.Property(x => x.Question).HasMaxLength(250).IsRequired(); builder.Property(x => x.Answer).HasMaxLength(4000).IsRequired();
        builder.HasIndex(x => new { x.IsDeleted, x.IsActive, x.IsReviewed, x.SortOrder, x.Id });
    }
}
