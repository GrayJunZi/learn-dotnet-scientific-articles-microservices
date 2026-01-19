using Auth.Domain.Users;
using BuildingBlocks.Core.Constraints;
using BuildingBlocks.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Persistence.EntityConfigurations;

public class UserEntityConfiguration : EntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        
        builder.Property(x=>x.Name)
            .HasMaxLength(MaxLength.C64)
            .IsRequired();

        builder.Property(x => x.Gender)
            .IsRequired();

        builder.OwnsOne(x=>x.Honorific, y =>
        {
            y.Property(x => x.Value)
                .HasMaxLength(MaxLength.C32)
                .HasColumnName(nameof(User.Honorific));

            y.WithOwner();
        });

        builder.OwnsOne(x =>x.ProfessionalProfile, y =>
        {
            y.Property(x=>x.Position).HasMaxLength(MaxLength.C32)
                .HasColumnNameSameAsProperty();
            y.Property(x=>x.CompanyName).HasMaxLength(MaxLength.C32)
                .HasColumnNameSameAsProperty();
            y.Property(x=>x.Affiliation).HasMaxLength(MaxLength.C32)
                .HasColumnNameSameAsProperty();
            y.WithOwner();
        });

        builder.Property(x => x.PictureUrl).HasMaxLength(MaxLength.C2048);

        builder.HasMany(x => x.UserRoles).WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);;
        
        builder.HasMany(x =>x .RefreshTokens).WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}