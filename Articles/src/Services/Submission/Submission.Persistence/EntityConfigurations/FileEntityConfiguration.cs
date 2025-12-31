using BuildingBlocks.Core.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Submission.Persistence.EntityConfigurations;

internal class FileEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Domain.ValueObjects.File> builder)
    {
        builder.Property(x => x.OriginalName).HasMaxLength(MaxLength.C256)
            .HasComment("Original full file name, with extension");
        builder.Property(x => x.FileServerId).HasMaxLength(MaxLength.C64);
        builder.Property(x => x.Size).HasComment("Size of the file in kilobytes");

        builder.ComplexProperty(x => x.Extension, complextBuilder =>
        {
            complextBuilder.Property(x => x.Value)
                .HasColumnName($"{builder.Metadata.ClrType.Name}_{complextBuilder.Metadata.PropertyInfo.Name}")
                .HasMaxLength(MaxLength.C8);
        });

        builder.ComplexProperty(x => x.Name, complextBuilder =>
        {
            complextBuilder.Property(x => x.Value)
                .HasColumnName($"{builder.Metadata.ClrType.Name}_{complextBuilder.Metadata.PropertyInfo.Name}")
                .HasMaxLength(MaxLength.C64).HasComment("Final name of the file after renaming");
        });
    }
}