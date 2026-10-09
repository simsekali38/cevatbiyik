using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CevatBiyik.Infrastructure.Persistence.Configurations;

public sealed class PracticeAreaConfiguration : IEntityTypeConfiguration<PracticeArea>
{
    public void Configure(EntityTypeBuilder<PracticeArea> builder)
    {
        builder.ToTable("PracticeAreas");
        builder.ConfigureBase();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Slug).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ShortDescription).HasMaxLength(1000);
        builder.Property(e => e.Content).HasColumnType("longtext").IsRequired();
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => new { e.IsDeleted, e.IsActive, e.SortOrder });
    }
}
