using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CevatBiyik.Infrastructure.Persistence.Configurations;
public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations"); builder.ConfigureBase();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired(); builder.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Summary).HasMaxLength(500).IsRequired(); builder.Property(x => x.Body).HasColumnType("longtext").HasMaxLength(12000).IsRequired();
        builder.Property(x => x.LocalDetails).HasColumnType("text").HasMaxLength(4000).IsRequired();
        builder.Property(x => x.MetaTitle).HasMaxLength(120); builder.Property(x => x.MetaDescription).HasMaxLength(320);
        builder.Property(x => x.Keywords).HasMaxLength(300); builder.Property(x => x.OpenGraphTitle).HasMaxLength(120);
        builder.Property(x => x.OpenGraphDescription).HasMaxLength(320);
        builder.HasIndex(x => x.Slug).IsUnique(); builder.HasIndex(x => new { x.IsActive, x.IsReviewed, x.FactsVerified });
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OpenGraphMedia).WithMany().HasForeignKey(x => x.OpenGraphMediaId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class LocationPracticeAreaConfiguration : IEntityTypeConfiguration<LocationPracticeArea>
{
    public void Configure(EntityTypeBuilder<LocationPracticeArea> builder)
    {
        builder.ToTable("LocationPracticeAreas"); builder.ConfigureBase(); builder.HasIndex(x => new { x.LocationId, x.PracticeAreaId }).IsUnique();
        builder.HasOne(x => x.Location).WithMany(x => x.PracticeLinks).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.PracticeArea).WithMany().HasForeignKey(x => x.PracticeAreaId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class LocationArticleConfiguration : IEntityTypeConfiguration<LocationArticle>
{
    public void Configure(EntityTypeBuilder<LocationArticle> builder)
    {
        builder.ToTable("LocationArticles"); builder.ConfigureBase(); builder.HasIndex(x => new { x.LocationId, x.ArticleId }).IsUnique();
        builder.HasOne(x => x.Location).WithMany(x => x.ArticleLinks).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Article).WithMany().HasForeignKey(x => x.ArticleId).OnDelete(DeleteBehavior.Restrict);
    }
}
