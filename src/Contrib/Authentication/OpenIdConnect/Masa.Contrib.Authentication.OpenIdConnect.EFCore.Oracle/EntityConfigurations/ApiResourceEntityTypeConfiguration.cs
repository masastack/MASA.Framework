// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ApiResourceEntityTypeConfiguration : IEntityTypeConfiguration<ApiResource>
{
    public void Configure(EntityTypeBuilder<ApiResource> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired(false);
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired(false);
        builder.Property(x => x.AllowedAccessTokenSigningAlgorithms).HasMaxLength(100).IsRequired(false).HasColumnName("AllowedTokenSignAlgorithm");
        builder.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0").HasDatabaseName("IX_ApiResource_Name");
        builder.HasMany(x => x.Secrets).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId)
            .HasConstraintName("FK_ApiResSecret_ResourceId").OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ApiScopes).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId)
            .HasConstraintName("FK_ApiResScope_ResourceId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.UserClaims).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId)
            .HasConstraintName("FK_ApiResClaim_ResourceId").OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Properties).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId)
            .HasConstraintName("FK_ApiResProp_ResourceId").OnDelete(DeleteBehavior.Cascade);
    }
}
