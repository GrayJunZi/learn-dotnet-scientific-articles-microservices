using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.EntityFramework;

public class EntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class, IEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnOrder(0);
    }
}