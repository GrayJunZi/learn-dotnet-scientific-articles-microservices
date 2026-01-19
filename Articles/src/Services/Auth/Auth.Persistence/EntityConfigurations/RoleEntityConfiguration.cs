using Auth.Domain.Role;
using BuildingBlocks.Core.Constraints;
using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Persistence.EntityConfigurations;

public class RoleEntityConfiguration : EntityConfiguration<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);


        builder.Property(x => x.Type).HasEnumConversion().IsRequired();
        builder.Property(x=>x.Description).HasMaxLength(MaxLength.C256).IsRequired();
    }
}