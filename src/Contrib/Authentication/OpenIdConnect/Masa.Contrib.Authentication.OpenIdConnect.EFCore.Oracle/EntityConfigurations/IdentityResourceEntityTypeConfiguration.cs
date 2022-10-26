// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class IdentityResourceEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResource>
{
    public void Configure(EntityTypeBuilder<IdentityResource> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired(false);
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired(false);
        builder.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0").HasDatabaseName("IX_IdResource_Name");

        builder.HasMany(x => x.UserClaims).WithOne(x => x.IdentityResource).HasForeignKey(x => x.IdentityResourceId)
            .HasConstraintName("FK_IdResClaim_ResourceId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Properties).WithOne(x => x.IdentityResource).HasForeignKey(x => x.IdentityResourceId)
            .HasConstraintName("FK_IdResourceProp_ResourceId").IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
