using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CevatBiyik.Infrastructure.Persistence.Configurations;

public sealed class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
{
    public void Configure(EntityTypeBuilder<SiteSetting> builder)
    {
        builder.ToTable("SiteSettings");
        builder.ConfigureBase();
        builder.Property(e => e.Key).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Value).HasMaxLength(4000).IsRequired();
        builder.HasIndex(e => e.Key).IsUnique();
    }
}
