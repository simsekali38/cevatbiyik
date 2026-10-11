using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CevatBiyik.Infrastructure.Persistence.Configurations;

internal static class BaseEntityConfiguration
{
    public static void ConfigureBase<T>(this EntityTypeBuilder<T> builder) where T : BaseEntity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CreatedAt).HasColumnType("datetime(6)").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime(6)");
        builder.Property(e => e.IsActive).HasDefaultValue(false);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
