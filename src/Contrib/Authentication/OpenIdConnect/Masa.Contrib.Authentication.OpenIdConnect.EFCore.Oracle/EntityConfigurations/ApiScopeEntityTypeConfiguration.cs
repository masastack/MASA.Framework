// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ApiScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiScope>
{
    public void Configure(EntityTypeBuilder<ApiScope> builder)
    {
        builder.HasIndex(apiScope => apiScope.Name).IsUnique().HasDatabaseName("IX_ApiScope_Name");

        builder.Property(apiScope => apiScope.Name).HasMaxLength(200).IsRequired();
        builder.Property(apiScope => apiScope.DisplayName).HasMaxLength(200).IsRequired(false);
        builder.Property(apiScope => apiScope.Description).HasMaxLength(1000).IsRequired(false);

        builder.HasMany(apiScope => apiScope.UserClaims).WithOne(apiScope => apiScope.ApiScope).HasForeignKey(apiScope => apiScope.ApiScopeId)
            .HasConstraintName("FK_ApiScopeClaim_ApiScopeId").IsRequired().OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Properties).WithOne(x => x.Scope).HasForeignKey(x => x.ScopeId)
            .HasConstraintName("FK_ApiScopeProp_ScopeId").IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
