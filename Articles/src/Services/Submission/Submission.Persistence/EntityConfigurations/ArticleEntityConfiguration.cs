using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

public class ArticleEntityConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
        builder.Property(x => x.Title).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Scope).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Type).HasEnumConversion();
        builder.Property(x => x.Stage).HasEnumConversion();

        builder.HasOne(x => x.Journal)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.JournalId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(x => x.Assets).WithOne(x => x.Article)
            .HasForeignKey(x => x.ArticleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}