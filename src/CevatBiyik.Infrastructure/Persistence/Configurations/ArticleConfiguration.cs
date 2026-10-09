using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CevatBiyik.Infrastructure.Persistence.Configurations;

public sealed class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");
        builder.ConfigureBase();
        builder.Property(e => e.Title).HasMaxLength(250).IsRequired();
        builder.Property(e => e.Slug).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Excerpt).HasMaxLength(1000);
        builder.Property(e => e.Content).HasColumnType("longtext").IsRequired();
        builder.Property(e => e.PublishedAt).HasColumnType("datetime(6)");
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.Property(x => x.MetaTitle).HasMaxLength(120);
        builder.Property(x => x.MetaDescription).HasMaxLength(320);
        builder.Property(x => x.Keywords).HasMaxLength(300);
        builder.Property(x => x.OpenGraphTitle).HasMaxLength(120);
        builder.Property(x => x.OpenGraphDescription).HasMaxLength(320);
        builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CoverMedia).WithMany().HasForeignKey(x => x.CoverMediaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OpenGraphMedia).WithMany().HasForeignKey(x => x.OpenGraphMediaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.IsDeleted, e.IsActive, e.PublishedAt });
        builder.HasOne(e => e.PracticeArea).WithMany(e => e.Articles)
            .HasForeignKey(e => e.PracticeAreaId).OnDelete(DeleteBehavior.Restrict);
    }
}
