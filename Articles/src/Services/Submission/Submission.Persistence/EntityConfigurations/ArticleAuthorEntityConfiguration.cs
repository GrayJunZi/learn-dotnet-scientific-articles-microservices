using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

public class ArticleAuthorEntityConfiguration : IEntityTypeConfiguration<ArticleAuthor>
{
    public void Configure(EntityTypeBuilder<ArticleAuthor> builder)
    {
        builder.Property(x => x.ContributeAreas)
            .HasJsonCollectionConversion()
            .IsRequired();
    }
}