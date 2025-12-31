using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

public class JournalEntityConfiguration : EntityConfiguration<Journal>
{
    public override void Configure(EntityTypeBuilder<Journal> builder)
    {
        base.Configure(builder);
        
        builder.Property(x => x.Name)
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(x => x.Abreviation)
            .HasMaxLength(8)
            .IsRequired();
    }
}