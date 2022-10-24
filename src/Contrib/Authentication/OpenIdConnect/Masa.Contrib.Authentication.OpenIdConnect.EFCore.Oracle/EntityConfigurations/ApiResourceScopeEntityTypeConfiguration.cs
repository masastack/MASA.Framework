// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Oracle.EntityConfigurations;

public class ApiResourceScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceScope>
{
    public void Configure(EntityTypeBuilder<ApiResourceScope> builder)
    {
        builder.HasKey(apiResourceScope => apiResourceScope.Id);
        builder.HasOne(x => x.ApiScope).WithMany().HasForeignKey(x => x.ApiScopeId).HasConstraintName("FK_ApiResScope_ScopeId");
        builder.HasIndex(x => x.ApiResourceId).HasDatabaseName("IX_ApiResScope_ApiScopeId");
    }
}
