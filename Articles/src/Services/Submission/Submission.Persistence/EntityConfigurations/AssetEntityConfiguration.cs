using BuildingBlocks.Core.Constraints;
using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

public class AssetEntityConfiguration : EntityConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Type).HasEnumConversion();

        builder.ComplexProperty(x => x.Name, builder =>
        {
            builder.Property(x => x.Value)
                .HasColumnName(builder.Metadata.PropertyInfo.Name)
                .HasMaxLength(MaxLength.C64)
                .IsRequired();
        });
    }
}