using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CevatBiyik.Infrastructure.Persistence.Configurations;
public sealed class CategoryConfiguration : IEntityTypeConfiguration<ArticleCategory>
{
    public void Configure(EntityTypeBuilder<ArticleCategory> builder)
    {
        builder.ConfigureBase(); builder.ToTable("ArticleCategories");
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired(); builder.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
public sealed class TagConfiguration : IEntityTypeConfiguration<ArticleTag>
{
    public void Configure(EntityTypeBuilder<ArticleTag> builder)
    {
        builder.ConfigureBase(); builder.ToTable("ArticleTags");
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired(); builder.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
public sealed class TagLinkConfiguration : IEntityTypeConfiguration<ArticleTagLink>
{
    public void Configure(EntityTypeBuilder<ArticleTagLink> builder)
    {
        builder.ToTable("ArticleTagLinks"); builder.HasKey(x => new { x.ArticleId, x.TagId });
        builder.HasOne(x => x.Article).WithMany(x => x.TagLinks).HasForeignKey(x => x.ArticleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.Article.IsDeleted && !x.Tag.IsDeleted);
    }
}
public sealed class MediaConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ConfigureBase(); builder.ToTable("MediaAssets");
        builder.Property(x => x.StoragePath).HasMaxLength(100).IsRequired(); builder.HasIndex(x => x.StoragePath).IsUnique();
        builder.Property(x => x.AltText).HasMaxLength(200).IsRequired(); builder.Property(x => x.ContentType).HasMaxLength(32).IsRequired();
    }
}
public sealed class PageSeoConfiguration : IEntityTypeConfiguration<PageSeo>
{
    public void Configure(EntityTypeBuilder<PageSeo> builder)
    {
        builder.ConfigureBase(); builder.ToTable("PageSeo"); builder.Property(x => x.Path).HasMaxLength(150).IsRequired(); builder.HasIndex(x => x.Path).IsUnique();
        builder.Property(x => x.MetaTitle).HasMaxLength(120); builder.Property(x => x.MetaDescription).HasMaxLength(320);
        builder.Property(x => x.Keywords).HasMaxLength(300); builder.Property(x => x.OpenGraphTitle).HasMaxLength(120);
        builder.Property(x => x.OpenGraphDescription).HasMaxLength(320);
        builder.HasOne(x => x.OpenGraphMedia).WithMany().HasForeignKey(x => x.OpenGraphMediaId).OnDelete(DeleteBehavior.Restrict);
    }
}
