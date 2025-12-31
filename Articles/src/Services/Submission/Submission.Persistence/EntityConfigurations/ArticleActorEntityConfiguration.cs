using Articles.Abstractions.Enums;
using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

public class ArticleActorEntityConfiguration : IEntityTypeConfiguration<ArticleActor>
{
    public void Configure(EntityTypeBuilder<ArticleActor> builder)
    {
        builder.HasKey(x => new { x.ArticleId, x.PersonId, x.Role });

        builder.Property(x => x.Role).HasEnumConversion().HasDefaultValue(UserRoleType.AUT);

        builder.HasOne(x => x.Article)
            .WithMany(x => x.Actors)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Person)
            .WithMany(x => x.Actors)
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}