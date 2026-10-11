using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CevatBiyik.Infrastructure.Persistence.Configurations;

public sealed class RedirectRuleConfiguration : IEntityTypeConfiguration<RedirectRule>
{
    public void Configure(EntityTypeBuilder<RedirectRule> builder)
    {
        builder.ConfigureBase(); builder.ToTable("RedirectRules");
        builder.Property(x => x.SourcePath).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DestinationPath).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.SourcePath).IsUnique();
    }
}
