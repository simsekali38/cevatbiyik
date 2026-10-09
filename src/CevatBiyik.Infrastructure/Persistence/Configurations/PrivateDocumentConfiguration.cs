using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CevatBiyik.Infrastructure.Persistence.Configurations;
public sealed class PrivateDocumentConfiguration : IEntityTypeConfiguration<PrivateDocument>
{
    public void Configure(EntityTypeBuilder<PrivateDocument> builder)
    {
        builder.ConfigureBase(); builder.ToTable("PrivateDocuments");
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.StoragePath).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Sha256).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.StoragePath).IsUnique();
    }
}
