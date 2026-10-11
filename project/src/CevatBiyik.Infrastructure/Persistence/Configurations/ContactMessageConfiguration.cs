using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CevatBiyik.Infrastructure.Persistence.Configurations;

public sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("ContactMessages");
        builder.ConfigureBase();
        builder.Property(e => e.Name).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(254).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(30);
        builder.Property(e => e.Subject).HasMaxLength(250);
        builder.Property(e => e.Message).HasMaxLength(5000).IsRequired();
        builder.Property(e => e.ProcessedAt).HasColumnType("datetime(6)");
        builder.HasIndex(e => new { e.IsDeleted, e.IsRead, e.CreatedAt });
        builder.Property(e => e.NotificationLeaseId).HasMaxLength(32);
        builder.Property(e => e.NotificationNextAttemptAt).HasColumnType("datetime(6)");
        builder.Property(e => e.NotificationLeaseUntil).HasColumnType("datetime(6)");
        builder.Property(e => e.NotificationSentAt).HasColumnType("datetime(6)");
        builder.HasIndex(e => new { e.IsDeleted, e.NotificationPending, e.NotificationNextAttemptAt });
    }
}
