// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.EntityFramework.EntityConfigurations;

public class ApiResourceScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceScope>
{
    public void Configure(EntityTypeBuilder<ApiResourceScope> builder)
    {
        builder.HasKey(apiResourceScope => apiResourceScope.Id);
    }
}
