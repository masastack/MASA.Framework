// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ApiScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiScope>
{
    public void Configure(EntityTypeBuilder<ApiScope> builder)
    {
        builder.HasIndex(apiScope => apiScope.Name).IsUnique();

        builder.Property(apiScope => apiScope.Name).HasMaxLength(200).IsRequired();
        builder.Property(apiScope => apiScope.DisplayName).HasMaxLength(200);
        builder.Property(apiScope => apiScope.Description).HasMaxLength(1000);

        builder.HasMany(apiScope => apiScope.UserClaims).WithOne(apiScope => apiScope.ApiScope).HasForeignKey(apiScope => apiScope.ApiScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
