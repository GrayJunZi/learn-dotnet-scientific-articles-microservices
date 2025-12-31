using BuildingBlocks.Core.Constraints;
using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission.Domain.Entities;

namespace Submission.Persistence.EntityConfigurations;

public class AssetTypeDefinitionEntityConfiguration : IEntityTypeConfiguration<AssetTypeDefinition>
{
    public void Configure(EntityTypeBuilder<AssetTypeDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).HasEnumConversion()
            .HasMaxLength(MaxLength.C64)
            .IsRequired()
            .HasColumnOrder(1);

        builder.Property(x => x.MaxFileSizeInMB).HasDefaultValue(5);
        builder.Property(x => x.DefaultFileExtension)
            .HasMaxLength(MaxLength.C8)
            .HasDefaultValue("pdf")
            .IsRequired();

        builder.ComplexProperty(x => x.AllowedFileExtensions, builder =>
        {
            var converter = BuilderExtensions.BuildJsonListConvertor<string>();

            builder.Property(x => x.Extensions)
                .HasConversion(converter)
                .HasColumnName(builder.Metadata.PropertyInfo.Name)
                .IsRequired();
        });
    }
}